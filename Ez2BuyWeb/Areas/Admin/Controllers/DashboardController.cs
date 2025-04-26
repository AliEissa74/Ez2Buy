using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
	public class DashboardController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public DashboardController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			DashboardVM dashboardVM = new()
			{
				OrderCount = _unitOfWork.OrderHeader.GetAll().Count(),
				ProductCount = _unitOfWork.Product.GetAll().Count(),
				CategoryCount = _unitOfWork.Category.GetAll().Count(),
				UserCount = _unitOfWork.AppUser.GetAll().Count(),
				Balance = (double)_unitOfWork.OrderHeader.GetAll().Where(i => i.PaymentStatus == SD.PaymentStatusApproved).Select(i => i.OrderTotal).Sum()
			};
			return View(dashboardVM);
		}
	}
}
