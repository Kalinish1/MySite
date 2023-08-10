using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
