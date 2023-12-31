﻿using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties:"ProductImages");
            return View(products);
        }
        public IActionResult Detail(int productId) {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.GetFirst(x => x.Id == productId, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = productId
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Detail(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.AppUserId = userId;
            var sCartDb = _unitOfWork.ShoppingCart.GetFirst(u=>u.AppUserId == userId && u.ProductId == shoppingCart.ProductId);
            if(sCartDb != null)
            {
                sCartDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(sCartDb);
                TempData["success"] = "Cart updated successfully.";
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                TempData["success"] = "Cart added successfully.";
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.AppUserId == userId).Count());
            }
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