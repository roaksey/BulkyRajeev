using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;

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
                productVm.Product = _unitOfWork.Product.Get(x => x.Id == id);
                return View(productVm);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRoot = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRoot, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRoot, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    };
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            }
            return View(productVM);
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
            var prodToDelete = _unitOfWork.Product.Get(x => x.Id == id);
            if(prodToDelete is null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,prodToDelete.ImageUrl.TrimStart('\\'));  
            if(System.IO.File.Exists(oldImagePath)) { 
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(prodToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product deleted successfully." });
        }
        #endregion

    }
}
