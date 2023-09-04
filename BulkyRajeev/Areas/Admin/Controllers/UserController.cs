using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _db;
        public UserController(AppDbContext db)
        {
          _db = db;
        }
        public IActionResult Index()
        {            
            return View();
        }
       
        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var UserList = _db.AppUsers.Include(x=>x.Company).ToList();
            return Json(new { data = UserList });
        }
        #endregion

    }
}
