using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API calls
        public IActionResult GetAll() {
            List<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
