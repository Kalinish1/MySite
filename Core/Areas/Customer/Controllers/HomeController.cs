using Core.DataAccess.Repository.IRepository;
using Core.Models;
using Core.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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
            ShoppingCart cart = new() {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId

            };
            return View(cart);

        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get( u => u.ApplicationUserId == userId &&
                u.ProductId == cart.ProductId);
            if (cartFromDb != null) 
            { //cart exists
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {// add cart

                _unitOfWork.ShoppingCart.Add(cart);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
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