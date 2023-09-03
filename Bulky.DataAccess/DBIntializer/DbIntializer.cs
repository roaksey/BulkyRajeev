using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DBIntializer
{
    public class DbIntializer : IDbInitializer
    {
        private readonly AppDbContext _db;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbIntializer(AppDbContext db,UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager; 
        }
        public void Intialize()
        {
            //migrations if  not applied 
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

            }
            catch (Exception ex)
            {

            }

            //Create Role if they are not created
            if(!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                // Create Admin User
                _userManager.CreateAsync(new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "roaksey@gmail.com",
                    PhoneNumber = "1234567890",
                    StreetAddress = "Test Admin",
                    State = "NP",
                    PostalCode = "12345",
                    City = "Kathmandu"
                }, "Admin@123").GetAwaiter().GetResult();

                AppUser user = _db.AppUsers.FirstOrDefault(u => u.Email == "roaksey@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }
            return;
        }
    }
}
