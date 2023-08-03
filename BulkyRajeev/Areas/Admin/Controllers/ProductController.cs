using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var products = _unitOfWork.Product.GetAll();
            
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
            if(id is null)
            {
                return View(productVm);
            }
            else
            {
                productVm.Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
                return View(productVm);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRoot = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRoot, @"images\product");
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    };
                    productVM.Product.ImageUrl = @"images\product\" + fileName;
                }
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(Index)); 
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
            }
            return View(productVM);
        }
        public IActionResult Edit(int id) {
            Product product = _unitOfWork.Product.GetFirstOrDefault(x=>x.Id  == id);
            if (product != null)
            {
                return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(model);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            Product product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                return View(product);
            }
            return NotFound();
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            Product product = _unitOfWork.Product.GetFirstOrDefault(x=>x.Id==id);   
            if (product == null)
            {                
                return NotFound();
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
