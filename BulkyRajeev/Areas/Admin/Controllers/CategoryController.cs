using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess.Repository.IReposiotry;
using Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category model)
        {
            //if(model.Name== model.DisplayOrder.ToString()) {
            //    ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            //}
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(model);
                _unitOfWork.Save();
                TempData["success"] = "Category created sucessfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);

        }

        public IActionResult Edit(int? id)
        {
            if (id == null && id == 0)
            {
                return NotFound();
            }
            //Category? category1 = _db.Categories.Find(id);
            Category? category2 = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            //Category? category3 = _db.Categories.Where(x=>x.Id == id).FirstOrDefault();
            if (category2 is null)
            {
                return NotFound();
            }
            return View(category2);
        }
        [HttpPost]
        public IActionResult Edit(Category model)
        {
            //if(model.Name== model.DisplayOrder.ToString()) {
            //    ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            //}
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(model);
                _unitOfWork.Save();
                TempData["success"] = "Category updated sucessfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);

        }

        public IActionResult Delete(int? id)
        {
            if (id == null && id == 0)
            {
                return NotFound();
            }
            //Category? category1 = _db.Categories.Find(id);
            Category? category2 = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            //Category? category3 = _db.Categories.Where(x=>x.Id == id).FirstOrDefault();
            if (category2 is null)
            {
                return NotFound();
            }
            return View(category2);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted sucessfully";
            return RedirectToAction("Index");
        }
    }
}
