using Core.Data;
using Core.DataAccess.Repository.IRepository;
using Core.Models;
using Core.Models.Models;
using Core.Models.ViewModels;
using Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Core.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var objectProductList = _unitOfWork.Product
                .GetAll(includeProperties: ("Category")).ToList();
            return View(objectProductList);
        }
        public IActionResult Upsert(int? id)
        {
            //преобразуем список в SelectListItem для создания dropdown 
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                    .GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (productVM.Product.Title != null && productVM.Product.Title == productVM.Product.Description.ToString())
            { //custom validation
                ModelState.AddModelError("description",
                    "Description can't match the Name.");
            }

            if (ModelState.IsValid) // server validation (from model data annotations)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    // saving image
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath,
                                productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"images\product\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    TempData["success"] = "Product created successfully";
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    TempData["success"] = "Product updated successfully";
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();

                return RedirectToAction("Index", "Product");
            }
            else // dropdown populating if modelState.isValid == false
            {
                productVM.CategoryList = _unitOfWork.Category
                    .GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                return View(productVM);
            }

        }
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    var product = _unitOfWork.Product.Get(c => c.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    var obj = _unitOfWork.Product.Get(c => c.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully";
        //    return RedirectToAction("Index", "Product");
        //}
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() {
            var objectProductList = _unitOfWork.Product
                    .GetAll(includeProperties: ("Category")).ToList();
            return Json(new { data = objectProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBedeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBedeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                                productToBedeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }
            _unitOfWork.Product.Remove(productToBedeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }
        #endregion
    }
}
