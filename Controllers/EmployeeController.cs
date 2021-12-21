using AutoMapper;
using Demo.BL.Interface;
using Demo.BL.Models;
using Demo.DAL.Entity;
using Demo.Language;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Demo.BL.Helper;

namespace Demo.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ICityRep city;
        private readonly IDistrictRep district;
        private readonly IStringLocalizer<SharedResource> localizer;


        #region Fiels

        // Loosly Coupled
        private readonly IEmployeeRep employee;
        private readonly IDepartmentRep department;
        private readonly IMapper mapper;

        // Tightly Coupled
        //DepartmentRep department ;

        #endregion


        #region Ctor

        public EmployeeController(IStringLocalizer<SharedResource> localizer,ICityRep city, IDistrictRep district,IEmployeeRep employee, IDepartmentRep department, IMapper mapper)
        {
            this.city = city;
            this.district = district;
            this.employee = employee;
            this.department = department;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        #endregion


        #region Actions


        public IActionResult Index(string SearchValue = "")
        {
            if( SearchValue == "")
            {
                var data = employee.Get();
                var model = mapper.Map<IEnumerable<EmployeeVM>>(data);
                return View(model);
            }
            else
            {
                var data = employee.SearchByName(SearchValue);
                var model = mapper.Map<IEnumerable<EmployeeVM>>(data);
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var data = employee.GetById(id);
            var model = mapper.Map<EmployeeVM>(data);
            ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name",model.DepartmentId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    model.PhotoName = FileUploader.UploadFile("/wwwroot/Files/Imgs",model.Photo);
                    model.CvName = FileUploader.UploadFile("/wwwroot/Files/Docs", model.Cv);

                    var data = mapper.Map<Employee>(model);
                    
                    employee.Create(data);
                    return RedirectToAction("Index");
                }


                ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name");
                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name");
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = employee.GetById(id);
            var model = mapper.Map<EmployeeVM>(data);
            ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeVM model)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    var data = mapper.Map<Employee>(model);
                    employee.Edit(data);
                    return RedirectToAction("Index");
                }


                ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
        }




        [HttpGet]
        public IActionResult Delete(int id)
        {
            var data = employee.GetById(id);
            var model = mapper.Map<EmployeeVM>(data);
            ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
            return View(model);
        }


        [HttpPost]
        public IActionResult Delete(EmployeeVM model)
        {

            try
            {


                var data = mapper.Map<Employee>(model);
                employee.Delete(data);

                FileUploader.RemoveFile("/wwwroot/Files/Imgs/", model.PhotoName);
                FileUploader.RemoveFile("/wwwroot/Files/Docs/", model.CvName);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.DepartmentList = new SelectList(department.Get(), "Id", "Name", model.DepartmentId);
                return View(model);
            }
        }

        #endregion


        #region Ajax Requests

        [HttpPost]
        public JsonResult GetCityDataByCountryId(int CtryId)
        {
            var data = city.Get(a => a.CountryId == CtryId);
            var model = mapper.Map<IEnumerable<CityVM>>(data);
            return Json(model);
        }


        [HttpPost]
        public JsonResult GetDistrictDataByCityId(int CtyId)
        {
            var data = district.Get(a => a.CityId == CtyId);
            var model = mapper.Map<IEnumerable<DistrictVM>>(data);
            return Json(model);
        }


        #endregion
    }
}
