using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess.Repository.IReposiotry;

namespace BulkyRajeev.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _dbCategory;
        public CategoryController(ICategoryRepository db)
        {
           _dbCategory = db;
        }
        public IActionResult Index()
        {
            List<Category> categories = _dbCategory.GetAll().ToList();
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
                _dbCategory.Add(model);
                _dbCategory.Save();
                TempData["success"] = "Category created sucessfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model); 

        }

        public IActionResult Edit(int? id)
        {
            if(id == null && id==0)
            {
                return NotFound();
            }
            //Category? category1 = _db.Categories.Find(id);
            Category? category2 = _dbCategory.GetFirstOrDefault(x=>x.Id == id);
            //Category? category3 = _db.Categories.Where(x=>x.Id == id).FirstOrDefault();
            if(category2 is null)
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
                _dbCategory.Update(model);
                _dbCategory.Save();
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
			Category? category2 = _dbCategory.GetFirstOrDefault(x => x.Id == id);
			//Category? category3 = _db.Categories.Where(x=>x.Id == id).FirstOrDefault();
			if (category2 is null)
			{
				return NotFound();
			}
			return View(category2);
		}
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
		public IActionResult DeletePost(int? id)
		{
			var obj = _dbCategory.GetFirstOrDefault(x => x.Id == id);
			if (obj == null)
			{
				return NotFound();
			}

			_dbCategory.Remove(obj);
			_dbCategory.Save();
            TempData["success"] = "Category deleted sucessfully";
            return RedirectToAction("Index");
		}
	}
}
