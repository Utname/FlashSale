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
    public class DefaultImageController : Controller
    {
        // GET: Admin/DefaultImage
        mapDefaultImage map = new mapDefaultImage();

        [AuthorizationCheck(ChucNang = "DefaultImage_Index")]

        public ActionResult Index(DefaultImageViewModel model)
        {
            model = map.getAllList(model);

            return View(model);
        }


        [AuthorizationCheck(ChucNang = "DefaultImage_Insert")]
        public ActionResult Insert()
        {
            var model = new DefaultImageModel();
            model.db.Image = map.anhMacDinh;
            model.db.CoverImage = map.anhMacDinh;
            return View(model);
        }

        [HttpPost]
        public ActionResult Insert(DefaultImageModel model, HttpPostedFileBase Image, HttpPostedFileBase CoverImage)
        {
           
         
            int check = CheckValidation(model);
            if (check == 1)
            {
                if (Image != null)
                {
                    model.db.Image = getFilePath(Image);
                }
                else
                {
                    model.db.Image = map.anhMacDinh;
                }
                if (CoverImage != null)
                {
                    model.db.CoverImage = getFilePath(CoverImage);
                }
                else
                {
                    model.db.CoverImage = map.anhMacDinh;

                }
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




        int CheckValidation(DefaultImageModel model)
        {
            if (string.IsNullOrWhiteSpace(model.db.Name))
            {
                ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.required);
            }
            else
            {
                var countClassification = map.db.DefaultImages.Where(q => q.Name == model.db.Name).Where(q => q.ID != model.db.ID).Where(q => q.Type == model.db.Type).Count();
                if (countClassification > 0)
                {
                    ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.existed);
                }
            }

            if (model.db.Type == null)
            {
                ModelState.AddModelError("db.Type", Data.Helpers.Common.Constants.required);
            }


            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "DefaultImage_Edit")]
        public ActionResult Edit(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(DefaultImageModel model, HttpPostedFileBase Image, HttpPostedFileBase CoverImage)
        {
            int check = CheckValidation(model);
            if (check == 1)
            {
                var data = map.details(model.db.ID);
                model.db.Image = data.db.Image;
                model.db.CoverImage = data.db.CoverImage;
                if (Image != null)
                {
                    model.db.Image = getFilePath(Image);
                }
                if (CoverImage != null)
                {
                    model.db.CoverImage = getFilePath(CoverImage);
                }
               
                model.db.UpdateBy = map.GetUserId();
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "DefaultImage_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "DefaultImage_Details")]
        public ActionResult Details(int id)
        {
            return View(map.details(id));
        }

    }
}