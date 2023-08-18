using Core.Data;
using Core.DataAccess.Repository.IRepository;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository db)
        {
            _categoryRepository = db;
        }
        public IActionResult Index()
        {
            var objectCategoryList = _categoryRepository.GetAll().ToList();
            return View(objectCategoryList);
        }
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name != null && obj.Name == obj.DisplayOrder.ToString())
            { //custom validation
                ModelState.AddModelError("displayorder",
                    "Display Order can't match the Name.");
            }
            if (ModelState.IsValid) // server validation (from model data annotations)
            {
                _categoryRepository.Add(obj);
                _categoryRepository.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            } 
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
           // var category = _categoryRepository.Categories.Find(id);
            var category = _categoryRepository.Get(c => c.Id == id);
            //var category2 = _categoryRepository.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid) // server validation (from model data annotations)
            {
                _categoryRepository.Update(obj);
                _categoryRepository.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // var category = _categoryRepository.Categories.Find(id);
            var category = _categoryRepository.Get(c => c.Id == id);
            //var category2 = _categoryRepository.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _categoryRepository.Get(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _categoryRepository.Remove(obj);
            _categoryRepository.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index", "Category");
        }
    }
}
