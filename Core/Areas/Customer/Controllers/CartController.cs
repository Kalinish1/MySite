using Core.DataAccess.Repository.IRepository;
using Core.Models;
using Core.Models.Models;
using Core.Models.ViewModels;
using Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Core.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {

            var claimIdentity = (ClaimsIdentity)User.Identity; //getting user id
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity; //getting user id
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;



            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName(nameof(Summary))]
		public IActionResult SummaryPOST()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity; //getting user id
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUser;

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
			
            

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{   // it is a regular customer, payment required
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
            else
            {   // company, payment can be delayed
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (ShoppingCartVM.OrderHeader.ApplicationUser.CompanyId.GetValueOrDefault() == 0)
            { // capturing payment for customer

            }

			return RedirectToAction(nameof(OrderConfirmation), new { id=ShoppingCartVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count++;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            { //remove
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count--;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
                return cart.Product.Price;
            else if (cart.Count <= 100)
                return cart.Product.Price50;
            else
                return cart.Product.Price100;
        }
    }
}
