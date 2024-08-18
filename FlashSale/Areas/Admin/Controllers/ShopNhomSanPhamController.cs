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
    public class ShopNhomSanPhamController : Controller
    {
        // GET: Admin/ShopNhomSanPham
        mapShopNhomSanPham map = new mapShopNhomSanPham();

        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_Index")]
        public ActionResult Index(ShopNhomSanPhamViewModel model)
        {
            model = map.getAllList(model);
            return View(model);
        }

        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_Insert")]
        public ActionResult Insert()
        {
            return View(new ShopNhomSanPhamModel());
        }

        [HttpPost]
        public ActionResult Insert(ShopNhomSanPhamModel model)
        {

            int check = CheckValidation(model);
            if (check == 1)
            {
                model.db.ID = Guid.NewGuid();
                model.db.NgayTao = DateTime.Now;
                model.db.StatusDel = 1;
                model.db.NgayCapNhat = DateTime.Now;
                map.insert(model);
                return Redirect("Index");
            }
            return View(model);

        }

        int CheckValidation(ShopNhomSanPhamModel model)
        {
            if (model.db.idShop == null)
            {
                ModelState.AddModelError("db.idShop", Data.Helpers.Common.Constants.required);
            }

            if (model.db.idNhomSanPham == null)
            {
                ModelState.AddModelError("db.idNhomSanPham", Data.Helpers.Common.Constants.required);
            }
            else
            {
                var existed = map.db.ShopNhomSanPhams.Where(q => q.idShop == model.db.idShop).Where(q => q.idNhomSanPham == model.db.idNhomSanPham).Where(q => q.StatusDel == 1).Where(q => q.ID != model.db.ID).Count() > 1;
                if (existed)
                {
                    ModelState.AddModelError("db.idNhomSanPham", Data.Helpers.Common.Constants.existed);
                }
            }
           
            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_Edit")]
        public ActionResult Edit(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(ShopNhomSanPhamModel model)
        {
            int check = CheckValidation(model);
            if (check == 1)
            {
                model.db.NgayCapNhat = DateTime.Now;
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_Import")]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Please select a excel file";
                return Redirect("Index");
            }
            if (!excelfile.FileName.EndsWith("xls") && !excelfile.FileName.EndsWith("xlsx"))
            {

                ViewBag.Error = "Please select a excel file";
                return Redirect("Index");
            }

            string path = Server.MapPath("~/Areas/Admin/Content/FileUpload/Excel/" + excelfile.FileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            excelfile.SaveAs(path);
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(path);
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            List<ShopNhomSanPhamModel> listShopNhomSanPham = new List<ShopNhomSanPhamModel>();
            for (int row = 2; row <= range.Rows.Count; row++)
            {
                ShopNhomSanPhamModel ShopNhomSanPham = new ShopNhomSanPhamModel();
                ShopNhomSanPham.TenShop = ((Excel.Range)range.Cells[row, 1]).Text;
                ShopNhomSanPham.TenNhomSanPham = int.Parse(((Excel.Range)range.Cells[row, 2]).Text);
                ShopNhomSanPham.db.GhiChu = int.Parse(((Excel.Range)range.Cells[row, 3]).Text);
                ShopNhomSanPham.db.StatusDel = 1;
                ShopNhomSanPham.db.NgayTao = DateTime.Now;
                ShopNhomSanPham.db.NgayCapNhat = DateTime.Now;

                ShopNhomSanPham.db.idShop = map.db.TaiKhoanShops.Where(x => x.TenShop == ShopNhomSanPham.TenShop).Where(q => q.StatusDel == 1).Select(q => q.ID).FirstOrDefault();
                ShopNhomSanPham.db.idNhomSanPham = map.db.NhomSanPhams.Where(x => x.TenNhom == ShopNhomSanPham.TenNhomSanPham).Where(q => q.StatusDel == 1).Select(q => q.ID).FirstOrDefault();

                listShopNhomSanPham.Add(ShopNhomSanPham);
                map.insertExcel(ShopNhomSanPham.db);
            }
            application.Quit();
            var modelFilter = new ShopNhomSanPhamViewModel();
            modelFilter.StatusDel = 1;
            modelFilter.IdGroup = -1;
            modelFilter.PageSize = 10; // Kích thước trang
            modelFilter = map.getAllList(modelFilter);
            return View("Index", modelFilter);
        }

        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_DownloadExcel")]
        public ActionResult DownloadExcel()
        {
            // Đường dẫn tới file trên server
            var filePath = Server.MapPath("~/Areas/Admin/Content/Templates/Excel/ShopNhomSanPham.xlsx");
            // Tên file khi người dùng tải về
            var fileName = "ShopNhomSanPham.xlsx";
            // Trả về file như một response
            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [AuthorizationCheck(ChucNang = "ShopNhomSanPham_Export")]
        public ActionResult Export(ShopNhomSanPhamViewModel model)
        {
            try
            {
                // Set the LicenseContext during application startup
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

                var modelFilter = map.getAllList(model);

                // Tạo một file Excel mới với EPPlus
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Đặt tiêu đề cột và dữ liệu
                    worksheet.Cells[1, 1].Value = "Tên shop";
                    worksheet.Cells[1, 2].Value = "Tên nhóm sản phẩm";
                    worksheet.Cells[1, 3].Value = "Ghi chú";
                    worksheet.Cells[1, 4].Value = "Ngày cập nhật";

                    // Đặt màu nền và màu chữ cho dòng 1
                    using (ExcelRange range = worksheet.Cells["A1:D1"])
                    {
                        range.Style.Font.Color.SetColor(Color.White); // Màu chữ là màu trắng
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Màu nền là màu xanh
                        range.Style.Font.Bold = true; // Đặt đậm cho font chữ
                        range.Style.Font.Size = 13; // Đặt cỡ chữ
                    }

                    // Đổ dữ liệu từ danh sách vào Excel
                    int row = 2;
                    foreach (var ShopNhomSanPham in modelFilter.ShopNhomSanPham)
                    {
                        var tenShop = map.db.TaiKhoanShops.Where(q => q.ID == ShopNhomSanPham.db.idShop).Where(q => q.StatusDel == 1).Select(q => q.TenShop).FirstOrDefault();
                        var tenNhomSanPham = map.db.NhomSanPhams.Where(q => q.ID == ShopNhomSanPham.db.idNhomSanPham).Where(q => q.StatusDel == 1).Select(q => q.TenNhom).FirstOrDefault();
                        worksheet.Cells[row, 1].Value = tenShop;
                        worksheet.Cells[row, 2].Value = tenNhomSanPham;
                        worksheet.Cells[row, 3].Value = ShopNhomSanPham.db.GhiChu;
                        worksheet.Cells[row, 4].Value = ShopNhomSanPham.db.NgayCapNhat != null ? ShopNhomSanPham.db.NgayCapNhat.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        row++;
                    }

                    using (ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, 4])
                    {
                        // Thiết lập border cho các cạnh
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        // Thiết lập màu của border
                        range.Style.Border.Top.Color.SetColor(Color.Black);
                        range.Style.Border.Left.Color.SetColor(Color.Black);
                        range.Style.Border.Right.Color.SetColor(Color.Black);
                        range.Style.Border.Bottom.Color.SetColor(Color.Black);
                    }


                    // Tự động điều chỉnh độ rộng cột
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Lưu file Excel vào một MemoryStream
                    MemoryStream stream = new MemoryStream(package.GetAsByteArray());

                    // Đặt vị trí của stream về đầu
                    stream.Position = 0;

                    // Trả về FileStreamResult để hiển thị file Excel trực tiếp trong trình duyệt
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ShopNhomSanPham.xlsx");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Result = ex.Message;
                // Xử lý lỗi nếu cần thiết
            }

            return RedirectToAction("Index");
        }


    }
}