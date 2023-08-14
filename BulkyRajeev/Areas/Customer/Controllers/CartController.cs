using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product")
            };

            return View(shoppingCartVM);
        }
    }
}
