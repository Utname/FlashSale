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
using System.Security.Policy;
using System.Text.RegularExpressions;
using FlashSale.App_Start;
using FlashSale.Areas.Admin.Model;

namespace FlashSale.Areas.Admin.Controllers
{
    public class DonHangChiTietController : Controller
    {
        // GET: Admin/DonHangChiTiet
        mapDonHangChiTiet map = new mapDonHangChiTiet();

        [AuthorizationCheck(ChucNang = "DonHangChiTiet_Index")]
        public ActionResult Index(string search, string statusDel, int page = 1)
        {
            statusDel = statusDel ?? "1";
            int pageSize = 10;  // Kích thước trang
            int skip = (page - 1) * pageSize;

            var allItems = map.getAllList(search, int.Parse(statusDel));
            var result = allItems.Skip(skip).Take(pageSize).ToList();

            ViewBag.search = search;
            ViewBag.statusDel = int.Parse(statusDel);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = allItems.Count();

            return View(result);
        }



        int CheckValidation(DonHangChiTietModel model, int type)
        {


            if (model.db.GiaBan == null)
            {
                ModelState.AddModelError("db.GiaBan", Data.Helpers.Common.Constants.required);
            }
            else if (model.db.GiaBan <= 0)
            {
                ModelState.AddModelError("db.GiaBan", "Giá bán phải lớn hơn 0");
            }

            if (model.db.SoLuong == null)
            {
                ModelState.AddModelError("db.SoLuong", Data.Helpers.Common.Constants.required);
            }
            else if (model.db.GiaBan <= 0)
            {
                ModelState.AddModelError("db.SoLuong", "Số lượng phải lớn hơn 0");
            }


            if (model.db.GiaSale == null)
            {
                ModelState.AddModelError("db.GiaSale", Data.Helpers.Common.Constants.required);
            }
            else if (model.db.GiaSale <= 0)
            {
                ModelState.AddModelError("db.GiaSale", "Giá sale phải lớn hơn 0");
            }
            else if (model.db.GiaSale > model.db.GiaBan)
            {
                ModelState.AddModelError("db.GiaSale", "Giá sale phải nhỏ hơn hoặc bằng giá bán");
            }

            return ModelState.IsValid ? 1 : 0;
        }


        [AuthorizationCheck(ChucNang = "DonHangChiTiet_Edit")]
        public ActionResult Edit(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(DonHangChiTietModel model)
        {
            model.db.GiaBan = decimal.Parse(model.GiaBanView.Replace(",", ""));
            model.db.SoLuong = float.Parse(model.SoLuongView.Replace(",", ""));
            model.db.GiaSale = decimal.Parse(model.GiaSaleView.Replace(",", ""));
            int check = CheckValidation(model, 2);
            if (check == 1)
            {
                model.db.NgayCapNhat = DateTime.Now;
                model.db.TenSanPham = map.db.SanPhams.Where(q => q.ID == model.db.idSanPham).Where(q => q.StatusDel == 1).Select(q => q.TenSanPham).FirstOrDefault();
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);
        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "DonHangChiTiet_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [AuthorizationCheck(ChucNang = "DonHangChiTiet_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }

    }
}