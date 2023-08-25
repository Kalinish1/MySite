using Core.DataAccess.Repository.IRepository;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Core.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var productsList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productsList);
        }
        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category");
            //if(product.ImageUrl.Contains("/Customer/Home")) 
            //{
            //    product.ImageUrl.Replace("/Customer/Home", "");
            //}
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}