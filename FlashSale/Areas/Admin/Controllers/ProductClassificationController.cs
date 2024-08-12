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
    public class ProductClassificationController : Controller
    {
        // GET: Admin/ProductClassification
        mapProductClassification map = new mapProductClassification();

        [AuthorizationCheck(ChucNang = "ProductClassification_Index")]
        public ActionResult Index(string search, string statusDel, int? idGroup, int page = 1)
        {
            statusDel = statusDel ?? "1";
            int pageSize = 10;  // Kích thước trang
            int skip = (page - 1) * pageSize;
            idGroup = idGroup ?? -1;
            var allItems = map.getAllList(search, int.Parse(statusDel),idGroup);
            var result = allItems.Skip(skip).Take(pageSize).ToList();
            ViewBag.search = search;
            ViewBag.statusDel = int.Parse(statusDel);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = allItems.Count();
            return View(result);
        }


        [AuthorizationCheck(ChucNang = "ProductClassification_Insert")]
        public ActionResult Insert()
        {
            return View(new ProductClassificationModel());
        }

        [HttpPost]
        public ActionResult Insert(ProductClassificationModel model, HttpPostedFileBase Image)
        {


            int check = CheckValidation(model);
           
            if (check == 1)
            {
                model.db.Image = map.anhMacDinh;
                if (Image != null)
                {
                    model.db.Image = getFilePath(Image);
                }
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

        string getFilePath(HttpPostedFileBase fileUpload)
        {
            List<string> exts = new List<string>() { ".jpeg", ".png", ".gif", ".jpg" };
            string ten = Path.GetFileNameWithoutExtension(fileUpload.FileName);
            string ext = Path.GetExtension(fileUpload.FileName);
            string duongDanLuuFile = "";
            if (fileUpload.ContentLength > 0 & exts.Count(m => m == ext.ToLower()) > 0)
            {
                //Lưu
                //--Thêm thư mục con teho năm-tháng-ngày
                string folderThoiGian = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                //1.Xác định thư mục lưu
                string folder = map.pathFileUpLoadImamge + folderThoiGian + "/";
                if (System.IO.File.Exists(Server.MapPath(folder)) == false)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(folder));
                }
                //Tính kilobyte
                var kb = fileUpload.ContentLength / 1024;
                var mb = (float)kb / 1024;
                //2.Xác định tên file
                string tenFile = fileUpload.FileName;
                //3.Xác định đường dẫn tuyệt đối của file
                string ddTuyetDoi = Server.MapPath(folder + tenFile);
                //4.Kiểm tra tồn tại => Có tồn tại file cũ thì xó   a
                //if (System.IO.File.Exists(ddTuyetDoi) == true)
                //{
                //    System.IO.File.Delete(ddTuyetDoi);
                //}
                //5.Trùng tên file
                int i = 0;
                duongDanLuuFile = folder + tenFile;
                while (System.IO.File.Exists(ddTuyetDoi) == true)
                {
                    i++;
                    tenFile = ten + "_" + i + ext;
                    ddTuyetDoi = Server.MapPath(folder + tenFile);
                }
                fileUpload.SaveAs(ddTuyetDoi);

            }
            return duongDanLuuFile;
        }

        int CheckValidation(ProductClassificationModel model)
        {
            if (string.IsNullOrWhiteSpace(model.db.Name))
            {
                ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.required);
            }
            else
            {
               
                var countClassification = map.db.ProductClassifications.Where(q => q.Name == model.db.Name).Where(q => q.ID != model.db.ID).Where(q => q.idProductCategory == model.db.idProductCategory).Count();
                if (countClassification > 0)
                {
                    ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.existed);
                }
            }

            if (model.db.idProductCategory == null)
            {
                ModelState.AddModelError("db.idProductCategory", Data.Helpers.Common.Constants.required);
            }
           
            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "ProductClassification_Edit")]
        public ActionResult Edit(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(ProductClassificationModel model, HttpPostedFileBase Image)
        {
            int check = CheckValidation(model);
            if (check == 1)
            {
                var data = map.details(model.db.ID);
                model.db.Image = data.db.Image;
                if (Image != null)
                {
                    model.db.Image = getFilePath(Image);
                }
                model.db.UpdateBy = map.GetUserId();
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "ProductClassification_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "ProductClassification_Details")]
        public ActionResult Details(int id)
        {
            return View(map.details(id));
        }

    }
}