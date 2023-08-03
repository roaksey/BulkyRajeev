using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();
            
            return View(products);
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll()
                .Select(x =>
                    new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }
                );
            //ViewBag.CategoryList = categoryList;

            // Using ViewData need type conversion. ViewBag internall use ViewData. Try Not to use ViewBag, ViewData instead go for View Model
            //ViewData["CategoryList"] = categoryList;
            ProductVM productVm = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
            };     
          return View(productVm);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
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
