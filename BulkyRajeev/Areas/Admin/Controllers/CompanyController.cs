using Bulky.DataAccess.Repository.IReposiotry;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Company> companies = _unitOfWork.Company.GetAll();
            return View(companies);
        }

        public IActionResult Upsert(int? id)
        {
            if(id == null)
            {
                return View(new Company());
            }
            Company company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
        [HttpPost]
        public IActionResult Upsert(Company company) {
            if (ModelState.IsValid)
            {
                if(company.Id == 0) {
                    _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company added successffully";
                }
                else
                {
                    _unitOfWork.Company.update(company);
                    TempData["success"] = "Company updated successffully";
                }
                _unitOfWork.Save();
                
                return RedirectToAction(nameof(Index));
            }
            return View(company);   
        }

        #region Api Handler
        [HttpGet]
        public IActionResult GetAll() {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return Json(new { Data = companies });
        }

        public IActionResult Delete(int? id)
        {
            var companyToDelete = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            if (companyToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting company." });
            }
            _unitOfWork.Company.Remove(companyToDelete);
            TempData["success"] = "Company deleted successffully";
            _unitOfWork.Save();
            return Json(new { success = true,message="Company deleted successfully" });
        }
        #endregion
    }
}
