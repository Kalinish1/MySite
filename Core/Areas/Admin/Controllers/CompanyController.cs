using Core.Data;
using Core.DataAccess.Repository.IRepository;
using Core.Models;
using Core.Models.Models;
using Core.Models.ViewModels;
using Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Core.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var objList = _unitOfWork.Company.GetAll().ToList();
            return View(objList);
        }
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //create
                Company company = new();
                return View(company);
            }
            else
            {
                //update
                var company = _unitOfWork.Company.Get(u => u.Id == id);
                return View(company);
            }

        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid) // server validation (from model data annotations)
            {
                if (company.Id == 0)
                {
                    TempData["success"] = "Company created successfully";
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    TempData["success"] = "Product updated successfully";
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index", "Company");
            }
            return View();

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objToDelete = _unitOfWork.Company.Get(u => u.Id == id);
            if (objToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(objToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }
        #endregion
    }
}
