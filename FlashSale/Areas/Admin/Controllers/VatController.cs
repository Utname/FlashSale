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
    public class VatController : Controller
    {
        // GET: Admin/Vat
        mapVat map = new mapVat();

        [AuthorizationCheck(ChucNang = "Vat_Index")]
        public ActionResult Index(VatViewModel model)
        {
            model = map.getAllList(model);
            return View(model);
        }

        [AuthorizationCheck(ChucNang = "Vat_Insert")]
        public ActionResult Insert()
        {
            return View(new VatModel());
        }

        [HttpPost]
        public ActionResult Insert(VatModel model)
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

        int CheckValidation(VatModel model)
        {



            if (string.IsNullOrWhiteSpace(model.db.Code))
            {
                ModelState.AddModelError("db.Code", Data.Helpers.Common.Constants.required);
            }

            if (model.db.PercentVat == null || model.db.PercentVat < 0)
            {
                ModelState.AddModelError("db.PercentVat", Data.Helpers.Common.Constants.required);
            }
           
            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "Vat_Edit")]
        public ActionResult Edit(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(VatModel model)
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
        [AuthorizationCheck(ChucNang = "Vat_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "Vat_Details")]
        public ActionResult Details(int id)
        {
            return View(map.details(id));
        }

    }
}