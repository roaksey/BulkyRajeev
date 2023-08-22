using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin +","+SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Details updated successfully.";
            return RedirectToAction(nameof(Detail), new { orderId = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirst(u=>u.Id == OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order shipped successfully.";
            return RedirectToAction(nameof(Detail),new {orderId= OrderVM.OrderHeader.Id});  
        }
        #region API calls
        public IActionResult GetAll(string status) {
            List<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser").ToList();
            }
            else
            {
                var identity = (ClaimsIdentity)User.Identity;
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.AppUserId == userId,includeProperties:"AppUser").ToList(); 
            }
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
