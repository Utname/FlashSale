using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Data.Entity;
using Data.Helpers.Model;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.IO.Packaging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Data.Helpers.Common;
using FlashSale.App_Start;
using FlashSale.Areas.Admin.Model;

namespace FlashSale.Areas.Admin.Controllers
{
    public class TypeProductController : Controller
    {
        // GET: Admin/TypeProduct
        mapTypeProduct map = new mapTypeProduct();

        [AuthorizationCheck(ChucNang = "TypeProduct_Index")]
        public ActionResult Index(string search, string statusDel, int? idNhomSanPham, int page = 1)
        {
            statusDel = statusDel ?? "1";
            idNhomSanPham = idNhomSanPham ?? -1;
            int pageSize = 10;  // Kích thước trang
            int skip = (page - 1) * pageSize;

            var allItems = map.getAllList(search, int.Parse(statusDel), idNhomSanPham);
            var result = allItems.Skip(skip).Take(pageSize).ToList();

            ViewBag.search = search;
            ViewBag.statusDel = int.Parse(statusDel);
            ViewBag.idNhomSanPham = idNhomSanPham;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = allItems.Count();

            return View(result);
        }


        [AuthorizationCheck(ChucNang = "TypeProduct_Insert")]
        public ActionResult Insert()
        {
            return View(new TypeProductModel());
        }

        [HttpPost]
        public ActionResult Insert(TypeProductModel model)
        {

            int check = CheckValidation(model);
            if (check == 1)
            {
                model.db.CreateDate = DateTime.Now;
                model.db.UpdateDate = DateTime.Now;
                model.db.StatusDel = 1;
                model.db.CreateBy =map.GetUserId();
                model.db.UpdateBy = map.GetUserId();
                map.insert(model);
                return Redirect("Index");
            }
            return View(model);

        }

        public JsonResult getListUseByGroup(int? groupId)
        {

            var result = map.db.TypeProducts.Where(q => q.StatusDel == 1).Where(q => q.idProductGroup == groupId).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Code
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        int CheckValidation(TypeProductModel model)
        {
            if (string.IsNullOrWhiteSpace(model.db.Code))
            {
                ModelState.AddModelError("db.Code", Data.Helpers.Common.Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.db.Name))
            {
                ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.required);
            }

            if (model.db.idProductGroup == null || model.db.idProductGroup == 0)
            {
                ModelState.AddModelError("db.idProductGroup", Data.Helpers.Common.Constants.required);
            }
           
            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "TypeProduct_Edit")]
        public ActionResult Edit(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(TypeProductModel model)
        {
            int check = CheckValidation(model);
            if (check == 1)
            {
                model.db.UpdateBy = map.GetUserId();
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "TypeProduct_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "TypeProduct_Details")]
        public ActionResult Details(int id)
        {
            return View(map.details(id));
        }

    }
}