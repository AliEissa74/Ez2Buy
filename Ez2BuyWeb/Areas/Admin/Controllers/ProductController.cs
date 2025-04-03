using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //get all categories from database
        public IActionResult Index()
        {
            //get all categories from database
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);  //passing all categories to view
        }

        //add Product
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }

        //Edit Product
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.GetById(u => u.Id == id);
            //Category CategoryFromDb = _db.Categories.FirstOrDefault(u=>u.Id==id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        //Delete Product
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.GetById(u => u.Id == id);
            //Category CategoryFromDb = _db.Categories.FirstOrDefault(u=>u.Id==id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]   //endpoint here is delete
        public IActionResult DeletePost(int? id)  //deletepost bec the cant have same name &  param
        {
            Product? obj = _unitOfWork.Product.GetById(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Delete(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
