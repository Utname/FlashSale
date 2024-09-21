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
    public class VoucherController : Controller
    {
        // GET: Admin/Voucher
        mapVoucher map = new mapVoucher();

        [AuthorizationCheck(ChucNang = "Voucher_Index")]

        public ActionResult Index(VoucherViewModel model)
        {
            model = map.getAllList(model);
            return View(model);
        }

        [AuthorizationCheck(ChucNang = "Voucher_Insert")]
        public ActionResult Insert()
        {
            var model = new VoucherModel();
            model.db.Image = map.getDefaultImage(1).image;
            return View( model);
        }

        [HttpPost]
        public ActionResult Insert(VoucherModel model, HttpPostedFileBase Image)
        {


            int check = CheckValidation(model);
           
            if (check == 1)
            {
                model.db.Image = map.anhMacDinh;
                if (Image != null)
                {
                    model.db.Image = getFilePath(Image);
                }
                else
                {
                    model.db.Image = map.getDefaultImage(1).image;
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

        int CheckValidation(VoucherModel model)
        {
            if (String.IsNullOrEmpty(model.ValueView))
            {
                ModelState.AddModelError("ValueView", Data.Helpers.Common.Constants.required);
            }
            else if (map.FormatDecimalView(model.ValueView) <= 0)
            {
                ModelState.AddModelError("ValueView", "Phần trăm giảm phải lớn hơn 0");
            }

            if (string.IsNullOrEmpty(model.db.Code))
            {
                ModelState.AddModelError("db.Code", Data.Helpers.Common.Constants.required);
            }
            else
            {
               
                var countClassification = map.db.Vouchers.Where(q => q.Code == model.db.Code).Where(q => q.ID != model.db.ID).Where(q => q.idShop == model.db.idShop).Count();
                if (countClassification > 0)
                {
                    ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.existed);
                }
            }

            if (string.IsNullOrEmpty(model.db.Name.ToString()))
            {
                ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.required);
            }

            //if (string.IsNullOrEmpty(model.db.idShop.ToString()))
            //{
            //    ModelState.AddModelError("db.idShop", Data.Helpers.Common.Constants.required);
            //}

            if ((model.db.StartTime != null && model.db.EndTime != null))
            {

                if (model.db.StartTime >= model.db.EndTime)
                {
                    ModelState.AddModelError("db.StartTime", "Giờ bắt đầu phải nhỏ hơn giờ kết thúc");
                }
            }
            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "Voucher_Edit")]
        public ActionResult Edit(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(VoucherModel model, HttpPostedFileBase Image)
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
        [AuthorizationCheck(ChucNang = "Voucher_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "Voucher_Details")]
        public ActionResult Details(int id)
        {
            return View(map.details(id));
        }

    }
}