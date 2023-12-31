﻿using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Net.Http.Headers;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(products);
        }
        //ViewBag.CategoryList = categoryList;

        // Using ViewData need type conversion. ViewBag internall use ViewData. Try Not to use ViewBag, ViewData instead go for View Model
        //ViewData["CategoryList"] = categoryList;
        public IActionResult Upsert(int? id)
        {
            ProductVM productVm = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }),
                Product = new Product()
            };
            if (id is null)
            {
                return View(productVm);
            }
            else
            {
                productVm.Product = _unitOfWork.Product.GetFirst(x => x.Id == id,includeProperties: "ProductImages");
                return View(productVm);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                //if (file != null)
                //{
                    //string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //string productPath = Path.Combine(wwwRoot, @"images\product");
                    //if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    //{
                    //    var oldImagePath = Path.Combine(wwwRoot, productVM.Product.ImageUrl.TrimStart('\\'));
                    //    if (System.IO.File.Exists(oldImagePath))
                    //    {
                    //        System.IO.File.Delete(oldImagePath);
                    //    }
                    //}
                    //using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    //{
                    //    file.CopyTo(fileStream);
                    //};
                    //productVM.Product.ImageUrl = @"\images\product\" + fileName;
                //}
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    //TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    //TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            }
            return View(productVM);
        }

       public IActionResult DeleteImage(int imageId)
        {
            ProductImage imageToBeDeleted = _unitOfWork.ProductImage.GetFirst(x => x.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if(imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,imageToBeDeleted.ImageUrl.Trim('\\'));
                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                    _unitOfWork.Save();

                    TempData["success"] = "Image removed successfully.";
                }
            }
            return RedirectToAction(nameof(Upsert), new {id=productId});
        }
        //public IActionResult Delete(int id)
        //{
        //    Product product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        //    if (product != null)
        //    {
        //        return View(product);
        //    }
        //    return NotFound();
        //}
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeletePost(int id)
        //{
        //    Product product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(product);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully";
        //    return RedirectToAction(nameof(Index));
        //}
        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = productList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var prodToDelete = _unitOfWork.Product.GetFirst(x => x.Id == id);
            if(prodToDelete is null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath,productPath);
            if(Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach(string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }
            //var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,prodToDelete.ImageUrl.TrimStart('\\'));  
            //if(System.IO.File.Exists(oldImagePath)) { 
            //    System.IO.File.Delete(oldImagePath);
            //}
            _unitOfWork.Product.Remove(prodToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product deleted successfully." });
        }
        #endregion

    }
}
