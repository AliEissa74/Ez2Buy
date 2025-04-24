using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]  //bind the properties of the model to the view automate proccessing of data from the view(field,query string) to the controller and vice versa
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "AppUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};

			return View(OrderVM);
		}

        //update order details
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id); //get the order header from the database
            //update the order header with the new values
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.Governorate = OrderVM.OrderHeader.Governorate;
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
			{
				orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb); //update the order header in the database
            _unitOfWork.Save();
			TempData["success"] = "Order Details Updated Successfully"; //show success message
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id }); //redirect to the details page of the order and pass route value (order id) to method details
        }

        //change order status to in process
        [HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
			TempData["success"] = "Order Status Updated Successfully";
			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
		}

        //change order status to shipped
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id); //get the order header from the database
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;

			_unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult MarkAsDelivered()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader.OrderStatus == SD.StatusShipped)
            {
                _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusDelivered);
                _unitOfWork.Save();
                TempData["success"] = "Order Delivered successfully!";
            }
            else
            {
                TempData["error"] = "Cannot mark this order as Delivered. It must be in Shipped status.";
            }
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        //cancel order & refund by admin or employee
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
		{
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id); //get the order header from the database
			if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                //refund the payment
				var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled,SD.StatusRefunded);

            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled,SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        //cancel order by user before if he doesnt pay
        [HttpPost]
        [Authorize(Roles = SD.Role_Customer)]
        public IActionResult CancelOrderUser()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id); //get the order header from the database
            if (orderHeader.OrderStatus == SD.StatusPending && orderHeader.PaymentStatus == SD.PaymentStatusPending)
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                _unitOfWork.Save();
                TempData["success"] = "Order Cancelled Successfully";
            }
            else
            {
                TempData["error"] = "Cannot cancel this order. Please contact technical support.";
            }
             return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPayNow()
        {
           OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "AppUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";  //This line builds the base URL of your website dynamically
                                                                             //Example result: https://Ez2Buy.com/
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",  //redirect to the order confirmation page after payment success
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",                                                   //redirect to the cart page after payment cancel
                LineItems = new List<SessionLineItemOptions>(),                                               //The actual items the customer is buying
                Mode = "payment",                                                                             //payment tells Stripe this is a one-time payment
            };

            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name                            //product Name shown in the Stripe UI
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();        //This creates the Stripe session using the options defined above
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url); //redirect to the stripe checkout page
            return new StatusCodeResult(303); //303 is the status code for redirect
        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if(orderHeader.PaymentStatus == SD.PaymentStatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId); //get the session from stripe using the session id
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
  
            }

            return View(orderHeaderId);
        }

        //api calls part
        #region API CALLS


        [HttpGet]
		public IActionResult GetAll(string status)
		{

			IEnumerable<OrderHeader> objOrderHeaders;

            //logic to make sure that only admin and employee can see all orders and the rest of the users can only see their orders only
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            }else {
                //if the user is not admin or employee, get only the orders of the logged in user(first we need to get user id)
				var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.AppUserId == userId, includeProperties: "AppUser");
            }

            switch (status)
			{
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
					break;
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
					break;
				case "approved":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusApproved && u.OrderStatus != SD.StatusInProcess && u.OrderStatus != SD.StatusShipped && u.OrderStatus != SD.StatusDelivered);
					break;
				case "shipped":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped && u.OrderStatus != SD.StatusCancelled && u.OrderStatus != SD.StatusDelivered);
					break;
				case "completed":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusDelivered);
					break;
				case "cancelled":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusCancelled);
					break;
				default:
					break;
			}

			return Json(new { data = objOrderHeaders });
		}



		#endregion
	}
}
