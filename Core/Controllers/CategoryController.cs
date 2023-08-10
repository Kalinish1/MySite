using Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var objectCategoryList = _db.Categories.ToList();
            return View(objectCategoryList);
        }
    }
}
