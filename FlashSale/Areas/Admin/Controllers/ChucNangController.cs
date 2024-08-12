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
using Constants = Data.Helpers.Common.Constants;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using FlashSale.Areas.Admin.Model;

namespace FlashSale.Areas.Admin.Controllers
{
    public class ChucNangController : Controller
    {
        // GET: Admin/ChucNang
        mapChucNang map = new mapChucNang();

        public ActionResult Index(string search, string statusDel, int page = 1)
        {
            statusDel = statusDel ?? "1";
            int pageSize = 10;
            int skip = (page - 1) * pageSize;

            var result = map.getAllList(search, int.Parse(statusDel))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();

            ViewBag.search = search;
            ViewBag.statusDel = int.Parse(statusDel);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = map.getAllList(search, int.Parse(statusDel)).Count();

            return View(result);
        }




        [HttpGet]
        public ActionResult GetData(string search, int? statusDel, int? start, int? length, string sortColumn, string sortDirection)
        {
            // Đặt giá trị mặc định nếu không có giá trị từ tham số
            int startIndex = start ?? 0;
            int pageSize = length ?? 10; // Số lượng bản ghi mặc định là 10 nếu length là null
            string orderByColumn = sortColumn ?? "MaChucNang"; // Cột sắp xếp mặc định
            string orderDirection = sortDirection ?? "asc"; // Hướng sắp xếp mặc định

            var query = map.db.ChucNangs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.MaChucNang.Contains(search) || x.TenChucNang.Contains(search));
            }

            if (statusDel.HasValue)
            {
                query = query.Where(x => x.StatusDel == statusDel.Value);
            }

            // Sắp xếp theo cột và hướng
            query = query.OrderBy($"{orderByColumn} {orderDirection}");

            // Lấy dữ liệu cần thiết
            var data = query
                .Skip(startIndex)
                .Take(pageSize)
                .ToList()
                .Select(x => new
                {
                    x.MaChucNang,
                    x.TenChucNang,
                    x.NhomChucNang,
                    x.StatusDel
                });

            return Json(new
            {
                draw = 1, // Đảm bảo draw có giá trị từ yêu cầu
                recordsTotal = map.db.ChucNangs.Count(),
                recordsFiltered = query.Count(),
                data = data
            }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult Insert()
        {
            return View(new ChucNang());
        }

       

        [HttpPost]
        public ActionResult Insert(ChucNang model)
        {
            int check = CheckValidationCreate(model);
            if (check == 1)
            {
                model.NgayTao = DateTime.Now;
                model.StatusDel = 1;
                model.NgayCapNhat = DateTime.Now;
                model.NguoiTao = map.GetUserId();
                model.NguoiCapNhat = model.NguoiTao;
                map.insert(model);
                return Redirect("Index");
            }
            return View(model);
        }

        public ActionResult InsertSync()
        {
            var model = new FunctionModel();
            model.listFunction = Constants.listFunction;
            return View(model);
        }

        [HttpPost]
        public ActionResult InsertSync(FunctionModel model)
        {
            int check = CheckValidationSync(model);
            
            if (check == 1)
            {
                string[] listCode = model.Code.Split(',');
                string[] listName = model.Name.Split(',');
                string[] listGroup = model.Group.Split(',');
                for (int i = 0; i < listCode.Length; i++)
                {
                    var code = listCode[i];
                    var name = listName[i];
                    var group = listGroup[i];

                    var listIdFunction = String.Join(",", model.SelectedFunctionIds);
                    var listFunction = Constants.listFunction.Where(q=>listIdFunction.Contains(q.id.ToString())).ToList();
                    listFunction.ForEach(q =>
                    {
                        var dbChucNang = new ChucNang();
                        dbChucNang.MaChucNang = code + q.name;
                        var function = Constants.functions[q.name];
                        dbChucNang.TenChucNang = function.Replace("@@ChucNang@@",name);
                        dbChucNang.NhomChucNang = group;
                        dbChucNang.NgayTao = DateTime.Now;
                        dbChucNang.StatusDel = 1;
                        dbChucNang.NgayCapNhat = DateTime.Now;
                        dbChucNang.NguoiTao = map.GetUserId();
                        dbChucNang.NguoiCapNhat = dbChucNang.NguoiTao;
                        map.db.ChucNangs.Add(dbChucNang);
                        map.db.SaveChanges();

                    });

                }
                return Redirect("Index");
            }
            model.listFunction = Constants.listFunction;
            return View(model);
        }

        int CheckValidationCreate(ChucNang model)
        {
            if (string.IsNullOrWhiteSpace(model.TenChucNang))
            {
                ModelState.AddModelError("TenChucNang",Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.MaChucNang))
            {
                ModelState.AddModelError("MaChucNang", Constants.required);
            }

            var checkTonTaiChucNang = map.db.ChucNangs.Where(q => q.MaChucNang == model.MaChucNang).Count() > 0;
            if(checkTonTaiChucNang)
            {
                ModelState.AddModelError("MaChucNang", Constants.existed);
            }
            return ModelState.IsValid ? 1 : 0;
        }

        int CheckValidationSync(FunctionModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                ModelState.AddModelError("Code", Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("Name", Constants.required);
            }
            return ModelState.IsValid ? 1 : 0;
        }

        int CheckValidationEdit(ChucNang model)
        {
            if (string.IsNullOrWhiteSpace(model.TenChucNang))
            {
                ModelState.AddModelError("TenChucNang", Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.MaChucNang))
            {
                ModelState.AddModelError("MaChucNang", Constants.required);
            }
            
            return ModelState.IsValid ? 1 : 0;
        }

        public ActionResult Edit(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(ChucNang model)
        {
            int check = CheckValidationEdit(model);
            if (check == 1)
            {
                model.NgayCapNhat = DateTime.Now;
                model.NguoiCapNhat = map.GetUserId();
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }

    }
}