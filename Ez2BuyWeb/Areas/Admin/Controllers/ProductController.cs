using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Services.Contracts;
using Ez2Buy.Services.Services;
using Ez2BuyWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _WebhostEnvironment; //this to access folder inside wwwroot (images)
		public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_WebhostEnvironment = webHostEnvironment;
		}
		//get all categories from database
		public IActionResult Index()
        {
            //get all categories from database
            List<Product> objProductList = _unitOfWork.Product.GetAll(includePorperties:"Category").ToList();
			//var products= _ProductService.GetProducts();
			return View(objProductList);  //passing all categories to view
        }


		// GET: Create
		public IActionResult Create()
		{
			ProductVM productVM = new()
			{
				CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				}),
				Product = new Product()
			};

			return View(productVM);
		}

		// POST: Create
		[HttpPost]
		public IActionResult Create(ProductVM productVM, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				//code to handle image upload
				string wwwRootPath = _WebhostEnvironment.WebRootPath; //this to access folder inside wwwroot
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString();  //this give random name to our image
																  //wecan combine fileName with extension in fileName by + Path.GetExtension(file.FileName)
					string productPath = Path.Combine(wwwRootPath, @"images\product");
					var extension = Path.GetExtension(file.FileName); //we send file as logo.png and he get extension from it by getextension func
					using (var fileStream = new FileStream(Path.Combine(productPath, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\product\" + fileName + extension;
				}
				_unitOfWork.Product.Add(productVM.Product);
				_unitOfWork.Save();
				TempData["success"] = "Product created successfully";
				return RedirectToAction("Index");
			}

			productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
			{
				Text = i.Name,
				Value = i.Id.ToString()
			});

			return View(productVM);
		}


        // GET: Edit
        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Product.GetById(p => p.Id == id);
            if (product == null) return NotFound();

            ProductVM productVM = new()
            {
                Product = product,
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            return View(productVM);
        }

        // POST: Edit
        [HttpPost]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
				//code to handle image upload
				string wwwRootPath = _WebhostEnvironment.WebRootPath; //this to access folder inside wwwroot
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString();  //this give random name to our image
																  //wecan combine fileName with extension in fileName by + Path.GetExtension(file.FileName)
					string productPath = Path.Combine(wwwRootPath, @"images\product");
					var extension = Path.GetExtension(file.FileName); //we send file as logo.png and he get extension from it by getextension func

					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl)) //if its not null or empty so delete old image
					{
						//delete old image
						var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

						if (System.IO.File.Exists(oldImagePath))  //if any image is here so delete it
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(productPath, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\product\" + fileName + extension;
				}

				_unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }

            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(productVM);
        }



        //Combine update and insert in one method (optional)
        //public IActionResult Upsert(int? id)      //updateInsert  why int? in update we need id when insert not
        //{
        //	ProductVM productVM = new()
        //          { 
        //	    CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem()
        //              {
        //                  Text = i.Name,
        //                  Value = i.Id.ToString()
        //              }),
        //		Product = new Product()
        //	};
        //	if (id == null || id == 0)
        //	{
        //		//create product
        //		return View(productVM);
        //	}
        //	else
        //	{
        //		//update product
        //		productVM.Product = _unitOfWork.Product.GetById(u => u.Id == id);
        //	    return View(productVM);
        //	}
        //      }

        //      [HttpPost]
        //      public IActionResult Upsert(ProductVM productVM,IFormFile? file)  //updateInsert
        //      {
        //          if (ModelState.IsValid)
        //          {
        //              _unitOfWork.Product.Add(productVM.Product);
        //              _unitOfWork.Save();
        //              TempData["success"] = "Product Created successfully";
        //              return RedirectToAction("Index");
        //          }
        //          else
        //          {
        //              productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem()
        //		{
        //			Text = i.Name,
        //			Value = i.Id.ToString()
        //		});
        //		return View(productVM);
        //          }

        //      }


        //api calls part
        #region API CALLS

        
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includePorperties: "Category").ToList();
            return Json(new {data = objProductList}); 
        }

        [HttpDelete]        
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.GetById(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_WebhostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
                
            if (System.IO.File.Exists(oldImagePath))  //if any image is here so delete it
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Delete(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }



        #endregion
    }
}
