using Microsoft.AspNetCore.Mvc;

namespace BulkyRajeev.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
