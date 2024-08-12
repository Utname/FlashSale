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
    public class ReturnAndExchangePolicyController : Controller
    {
        // GET: Admin/ReturnAndExchangePolicy
        mapReturnAndExchangePolicy map = new mapReturnAndExchangePolicy();

        [AuthorizationCheck(ChucNang = "ReturnAndExchangePolicy_Index")]
        public ActionResult Index(string search, string statusDel, int? Type, int page = 1)
        {
            statusDel = statusDel ?? "1";
            Type = Type ?? -1;
            int pageSize = 10;  // Kích thước trang
            int skip = (page - 1) * pageSize;

            var allItems = map.getAllList(search, int.Parse(statusDel), Type);
            var result = allItems.Skip(skip).Take(pageSize).ToList();

            ViewBag.search = search;
            ViewBag.statusDel = int.Parse(statusDel);
            ViewBag.Type = Type;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = allItems.Count();
            return View(result);
        }


        [AuthorizationCheck(ChucNang = "ReturnAndExchangePolicy_Insert")]
        public ActionResult Insert()
        {
            return View(new ReturnAndExchangePolicyModel());
        }

        [HttpPost]
        public ActionResult Insert(ReturnAndExchangePolicyModel model)
        {

            int check = CheckValidation(model);
            if (check == 1)
            {
                model.db.RefundFee = decimal.Parse(model.RefundFeeView.Replace(",", ""));
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

        int CheckValidation(ReturnAndExchangePolicyModel model)
        {
            if (string.IsNullOrWhiteSpace(model.db.Name))
            {
                ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.required);
            }
            else
            {
                var countSanPham = map.db.ReturnAndExchangePolicies.Where(q => q.Name == model.db.Name).Where(q => q.ID != model.db.ID).Count();
                if (countSanPham > 0)
                {
                    ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.existed);
                }
            }

            if (model.db.Type == null)
            {
                ModelState.AddModelError("db.Type", Data.Helpers.Common.Constants.required);
            }

            else if (model.db.Type == 2)
            {

                if (String.IsNullOrEmpty(model.RefundFeeView))
                {
                    ModelState.AddModelError("db.RefundFee", Data.Helpers.Common.Constants.required);
                }
                else if (decimal.Parse(model.RefundFeeView.Replace(",", "")) <= 0)
                {
                    ModelState.AddModelError("db.RefundFee", "Phí hoàn trả phải lớn hơn 0");
                }
            }
           
            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "ReturnAndExchangePolicy_Edit")]
        public ActionResult Edit(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(ReturnAndExchangePolicyModel model)
        {
            int check = CheckValidation(model);
            if (check == 1)
            {
                model.db.RefundFee = decimal.Parse(model.RefundFeeView.Replace(",", ""));
                model.db.UpdateBy = map.GetUserId();
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "ReturnAndExchangePolicy_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "ReturnAndExchangePolicy_Details")]
        public ActionResult Details(int id)
        {
            return View(map.details(id));
        }

    }
}