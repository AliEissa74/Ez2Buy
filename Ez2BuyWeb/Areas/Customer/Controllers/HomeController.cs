using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Ez2BuyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        //get all products from database
        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        //single product details Page
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.GetById(u => u.Id == productId, includeProperties: "Category"),
                Quantity = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]     //Authorize the user to add to cart
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            //first we get id of the user who is logged in
            var claimsIdentity = (ClaimsIdentity)User.Identity;  //Get the claims identity
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value; //Get the user id from the claims identity
            shoppingCart.AppUserId = userId; //Set the user id to the shopping cart

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ProductId == shoppingCart.ProductId && u.AppUserId == userId);
            if (cartFromDb != null) {
                //shopping cart exists, update the quantity
                cartFromDb.Quantity += shoppingCart.Quantity; //Update the quantity of the shopping cart
                _unitOfWork.ShoppingCart.Update(cartFromDb); //Update the shopping cart in the database
                TempData["success"] = "Cart Updated Successfully"; 

            }
            else
            {
                //add the new cart
                _unitOfWork.ShoppingCart.Add(shoppingCart); //Add the shopping cart to the database

                TempData["success"] = "Product added to Cart"; 
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index)); //Redirect to the index page
        }

        //code to manage add to cart btn in home page for product

        //[HttpPost]
        //[Authorize]
        //public IActionResult AddToCart(int productId)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    var cartItem = _unitOfWork.ShoppingCart.Get(
        //        u => u.ProductId == productId && u.AppUserId == userId);

        //    if (cartItem != null)
        //    {
        //        cartItem.Quantity += 1;
        //        _unitOfWork.ShoppingCart.Update(cartItem);
        //    }
        //    else
        //    {
        //        ShoppingCart newCart = new()
        //        {
        //            AppUserId = userId,
        //            ProductId = productId,
        //            Quantity = 1
        //        };
        //        _unitOfWork.ShoppingCart.Add(newCart);
        //    }

        //    _unitOfWork.Save();

        //    // Optional: show success message using TempData
        //    TempData["success"] = "Product added to cart";

        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
