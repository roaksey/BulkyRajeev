using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM  OrderVM { get; set; }
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
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirst(u => u.Id == orderId, includeProperties: "AppUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId == orderId,includeProperties: "Product")
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult updateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirst(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Detail), new { orderId = orderHeaderFromDb.Id });
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
