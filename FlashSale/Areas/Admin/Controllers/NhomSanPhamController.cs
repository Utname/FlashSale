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
    public class NhomSanPhamController : Controller
    {
        // GET: Admin/NhomSanPham
        mapNhomSanPham map = new mapNhomSanPham();

        [AuthorizationCheck(ChucNang = "NhomSanPham_Index")]
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




        [AuthorizationCheck(ChucNang = "NhomSanPham_Insert")]
        public ActionResult Insert()
        {
            return View(new NhomSanPhamModel());
        }

        [HttpPost]
        public ActionResult Insert(NhomSanPhamModel model)
        {

            int check = CheckValidation(model);
            if (check == 1)
            {
                model.db.NgayTao = DateTime.Now;
                model.db.StatusDel = 1;
                model.db.NgayCapNhat = DateTime.Now;
                map.insert(model);
                return Redirect("Index");
            }
            return View(model);

        }

        int CheckValidation(NhomSanPhamModel model)
        {
            if (string.IsNullOrWhiteSpace(model.db.TenNhom))
            {
                ModelState.AddModelError("db.TenNhom", Data.Helpers.Common.Constants.required);
            }

            if (model.db.idCapCha == null || model.db.idCapCha == 0)
            {
                ModelState.AddModelError("db.idCapCha", Data.Helpers.Common.Constants.required);
            }

            if (model.db.ThuTu == 0 || model.db.ThuTu == null)
            {
                ModelState.AddModelError("db.ThuTu", Data.Helpers.Common.Constants.required);
            }
            return ModelState.IsValid ? 1 : 0;
        }

        [AuthorizationCheck(ChucNang = "NhomSanPham_Edit")]
        public ActionResult Edit(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(NhomSanPhamModel model)
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
        [AuthorizationCheck(ChucNang = "NhomSanPham_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(int id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizationCheck(ChucNang = "NhomSanPham_Details")]
        public ActionResult Details(int id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        [AuthorizationCheck(ChucNang = "NhomSanPham_Import")]
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
            List<NhomSanPham> listNhomSanPham = new List<NhomSanPham>();
            for (int row = 2; row <= range.Rows.Count; row++)
            {
                NhomSanPham nhomSanPham = new NhomSanPham();
                nhomSanPham.TenNhom = ((Excel.Range)range.Cells[row, 1]).Text;
                nhomSanPham.idCapCha = int.Parse(((Excel.Range)range.Cells[row, 2]).Text);
                nhomSanPham.ThuTu = int.Parse(((Excel.Range)range.Cells[row, 3]).Text);
                nhomSanPham.StatusDel = 1;
                nhomSanPham.NgayTao = DateTime.Now;
                nhomSanPham.NgayCapNhat = DateTime.Now;
                listNhomSanPham.Add(nhomSanPham);
                map.insertExcel(nhomSanPham);
            }
            application.Quit();
            var result = map.getAllList("", 1);
            return View("Index", result);
        }

        [AuthorizationCheck(ChucNang = "NhomSanPham_DownloadExcel")]
        public ActionResult DownloadExcel()
        {
            // Đường dẫn tới file trên server
            var filePath = Server.MapPath("~/Areas/Admin/Content/Templates/Excel/NhomSanPham.xlsx");
            // Tên file khi người dùng tải về
            var fileName = "NhomSanPham.xlsx";
            // Trả về file như một response
            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [AuthorizationCheck(ChucNang = "NhomSanPham_Export")]
        public ActionResult Export()
        {
            try
            {
                // Set the LicenseContext during application startup
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

                var listNhomSanPham = map.getAllList("", 1);

                // Tạo một file Excel mới với EPPlus
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Đặt tiêu đề cột và dữ liệu
                    worksheet.Cells[1, 1].Value = "Tên Nhóm";
                    worksheet.Cells[1, 2].Value = "ID Cấp cha";
                    worksheet.Cells[1, 3].Value = "Số thứ tự";
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
                    foreach (var nhomSanPham in listNhomSanPham)
                    {
                        worksheet.Cells[row, 1].Value = nhomSanPham.db.TenNhom;
                        worksheet.Cells[row, 2].Value = nhomSanPham.db.idCapCha;
                        worksheet.Cells[row, 3].Value = nhomSanPham.db.ThuTu;
                        worksheet.Cells[row, 4].Value = nhomSanPham.db.NgayCapNhat != null ? nhomSanPham.db.NgayCapNhat.ToString() : DateTime.Now.ToString("MM/dd/yyyy");

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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NhomSanPham.xlsx");
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