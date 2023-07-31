
using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyRajeev.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
           _db = db;
        }
        public IActionResult Index()
        {
            List<Category> categories = _db.Categories.ToList();
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
                _db.Categories.Add(model);
                _db.SaveChanges();
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
            Category? category2 = _db.Categories.FirstOrDefault(x=>x.Id == id);
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
                _db.Categories.Update(model);
                _db.SaveChanges();
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
			Category? category2 = _db.Categories.FirstOrDefault(x => x.Id == id);
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
			var obj = _db.Categories.Find(id);
			if (obj == null)
			{
				return NotFound();
			}

			_db.Categories.Remove(obj);
			_db.SaveChanges();
            TempData["success"] = "Category deleted sucessfully";
            return RedirectToAction("Index");
		}
	}
}
