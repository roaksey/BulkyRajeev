using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

        public IActionResult Detail(int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirst(u => u.Id == orderId, includeProperties: "AppUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId == orderId,includeProperties: "Product")
            };
            return View(orderVM);
        }
        #region API calls
        public IActionResult GetAll(string status) {
            List<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x=>x.PaymentStatus == SD.PaymentStatusPending).ToList();
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == SD.StatusInProcess).ToList();
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == SD.StatusShipped).ToList();
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == SD.PaymentStatusApproved).ToList();
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
