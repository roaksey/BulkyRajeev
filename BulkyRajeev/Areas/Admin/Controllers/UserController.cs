﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.CodeAnalysis.Scripting.Hosting;
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
            var roles = _db.Roles.ToList();
            var userRoles = _db.UserRoles.ToList();

            foreach (var user in UserList)
            {
                var role = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                user.Role = roles.FirstOrDefault(x => x.Id == role.RoleId).Name;
                //if(user.Company == null)
                //{
                //    user.Company = new Company { Name = ""};
                //}
            }
            return Json(new { data = UserList });
        }
        public IActionResult RoleManagement(string userId)
        {
            var RoleId = _db.UserRoles.FirstOrDefault(x=> x.UserId == userId).RoleId;
            RoleManagementVM roleManagementVM = new RoleManagementVM
            {
                AppUser = _db.AppUsers.Include(u => u.Company).FirstOrDefault(x => x.Id == userId),
                RoleLisst = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }).ToList(),
                CompanyList = _db.Companies.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                })
            };
            roleManagementVM.AppUser.Role = _db.Roles.FirstOrDefault(x => x.Id == RoleId).Name;
            
            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.AppUsers.FirstOrDefault(x=> x.Id == id);                                                       
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking User." });
            }
            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true,message="Operation Successful." }); 
        }
        #endregion

    }
}
