﻿using System;
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
using System.Net;

namespace FlashSale.Areas.Admin.Controllers
{
    public class TaiKhoanShopController : Controller
    {
        // GET: Admin/TaiKhoanShop
        mapTaiKhoanShopSystem map = new mapTaiKhoanShopSystem();

        [AuthorizationCheck(ChucNang = "TaiKhoanShop_Index")]
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


        [AuthorizationCheck(ChucNang = "TaiKhoanShop_Insert")]
        public ActionResult Insert()
        {
            return View(new TaiKhoanShopModel()
            {
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Insert(TaiKhoanShopModel model, HttpPostedFileBase AnhBiaUpload, HttpPostedFileBase AnhDaiDienUpload)
        {

            int check = CheckValidation(model);
            model.db.AnhBia = map.anhMacDinh;
            model.db.AnhDaiDien = map.anhMacDinh;
            if (AnhBiaUpload != null)
            {
                model.db.AnhBia = getFilePath(AnhBiaUpload);
            }
            if (AnhDaiDienUpload != null)
            {
                model.db.AnhDaiDien = getFilePath(AnhDaiDienUpload);
            }
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

        int CheckValidation(TaiKhoanShopModel model)
        {
            if (string.IsNullOrWhiteSpace(model.db.TenShop))
            {
                ModelState.AddModelError("sb.TenShop", Data.Helpers.Common.Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.db.Email))
            {
                ModelState.AddModelError("db.Email", Data.Helpers.Common.Constants.required);
            }

            else
            {
                if (map.IsValidEmail(model.db.Email) == false)
                {
                    ModelState.AddModelError("db.Email", Data.Helpers.Common.Constants.emailNotCorrect);
                }
                else
                {
                    var countSanPham = map.db.TaiKhoanShops.Where(q => q.Email == model.db.Email).Where(q => q.ID != model.db.ID).Count();
                    if (countSanPham > 0)
                    {
                        ModelState.AddModelError("db.Email", Data.Helpers.Common.Constants.existed);
                    }

                }
            }
            if (string.IsNullOrWhiteSpace(model.db.Username))
            {
                ModelState.AddModelError("db.Username", Data.Helpers.Common.Constants.required);
            }

            if (!string.IsNullOrEmpty(model.db.SoDienThoai))
            {
                if (!Regex.IsMatch(model.db.SoDienThoai, map.phonePattern))
                {
                    ModelState.AddModelError("db.SoDienThoai", Data.Helpers.Common.Constants.phoneNotCorrect);
                }
            }

            return ModelState.IsValid ? 1 : 0;
        }


        [AuthorizationCheck(ChucNang = "TaiKhoanShop_Edit")]
        public ActionResult Edit(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(TaiKhoanShopModel model, HttpPostedFileBase AnhBiaUpload, HttpPostedFileBase AnhDaiDienUpload)
        {
            int check = CheckValidation(model);
            var data = map.details(model.db.ID.ToString());
            model.db.AnhBia = data.db.AnhBia;
            model.db.AnhDaiDien = data.db.AnhDaiDien;
            if (AnhBiaUpload != null)
            {
                model.db.AnhBia = getFilePath(AnhBiaUpload);
            }
            if (AnhDaiDienUpload != null)
            {
                model.db.AnhDaiDien = getFilePath(AnhDaiDienUpload);
            }
            if (check == 1)
            {
                model.db.NgayCapNhat = DateTime.Now;
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "TaiKhoanShop_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [AuthorizationCheck(ChucNang = "TaiKhoanShop_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }

        public ActionResult PhanQuyen(string id, string search, string filter)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID không hợp lệ");
            }

            var taiKhoan = map.details(id);
            if (taiKhoan == null)
            {
                return HttpNotFound("Tài khoản không tồn tại");
            }

            var listChucNang = map.db.ChucNangs
                .Where(q => q.StatusDel == 1 &&
                            (q.TenChucNang.Contains(search) || q.MaChucNang.Contains(search) || string.IsNullOrEmpty(search)))
                .OrderBy(q => q.MaChucNang)
                .ToList();

            var result = new PhanQuyenTaiKhoanModel
            {
                TaiKhoanShop = taiKhoan,
                ListChucNang = listChucNang,
                CheckAll = map.db.PhanQuyens.Count(q => q.idTaiKhoanShop == taiKhoan.db.ID) == map.db.ChucNangs.Count()
            };

            ViewBag.search = search;
            return View(result);
        }


        [HttpPost]
        public ActionResult PhanQuyen(string idTaiKhoanShop, string maChucNang)
        {
            if (string.IsNullOrEmpty(idTaiKhoanShop))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID không hợp lệ");
            }

            Guid taiKhoanId;
            if (!Guid.TryParse(idTaiKhoanShop, out taiKhoanId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID tài khoản không hợp lệ");
            }

            var db = new FlashSaleEntities();

            try
            {
                if (maChucNang == "-1") // Xử lý CheckAll
                {
                    var existingPermissions = db.PhanQuyens.Where(m => m.idTaiKhoanShop == taiKhoanId).ToList();
                    var allChucNangs = db.ChucNangs.ToList();

                    // Nếu chưa phân quyền cho tất cả các chức năng, thêm quyền cho tất cả các chức năng
                    if (existingPermissions.Count != allChucNangs.Count)
                    {
                        db.PhanQuyens.RemoveRange(existingPermissions);
                        db.SaveChanges();

                        var newPermissions = allChucNangs.Select(item => new PhanQuyen
                        {
                            idTaiKhoanShop = taiKhoanId,
                            MaChucNang = item.MaChucNang
                        }).ToList();

                        db.PhanQuyens.AddRange(newPermissions);
                        db.SaveChanges();
                    }
                }
                else // Xử lý thêm/xóa quyền cho từng chức năng
                {
                    var existingPermission = db.PhanQuyens.SingleOrDefault(m => m.idTaiKhoanShop == taiKhoanId && m.MaChucNang == maChucNang);
                    if (existingPermission != null)
                    {
                        db.PhanQuyens.Remove(existingPermission);
                    }
                    else
                    {
                        var newPermission = new PhanQuyen
                        {
                            idTaiKhoanShop = taiKhoanId,
                            MaChucNang = maChucNang
                        };
                        db.PhanQuyens.Add(newPermission);
                    }
                    db.SaveChanges();
                }

                return Json(new { status = "Đã phân quyền" });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi trong quá trình xử lý");
            }
        }


        public ActionResult BaoChuaPhanQuyen()
        {
            return View();
        }



        [HttpPost]
        [AuthorizationCheck(ChucNang = "TaiKhoanShop_Import")]
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

            string path = Server.MapPath(map.pathFileUploadExcel + excelfile.FileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            excelfile.SaveAs(path);

            Excel.Application application = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;
            Excel.Range range = null;

            try
            {
                application = new Excel.Application();
                workbook = application.Workbooks.Open(path);
                worksheet = workbook.ActiveSheet;
                range = worksheet.UsedRange;

                List<TaiKhoanShop> listNhomSanPham = new List<TaiKhoanShop>();
                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    TaiKhoanShop taiKhoanShop = new TaiKhoanShop();
                    taiKhoanShop.TenShop = ((Excel.Range)range.Cells[row, 1]).Text;
                    taiKhoanShop.Username = ((Excel.Range)range.Cells[row, 2]).Text;
                    taiKhoanShop.Email = ((Excel.Range)range.Cells[row, 3]).Text;
                    taiKhoanShop.SoDienThoai = ((Excel.Range)range.Cells[row, 4]).Text;
                    taiKhoanShop.DiaChi = ((Excel.Range)range.Cells[row, 5]).Text;
                    taiKhoanShop.Facebook = ((Excel.Range)range.Cells[row, 6]).Text;
                    taiKhoanShop.StatusDel = 1;
                    taiKhoanShop.ID = Guid.NewGuid();
                    taiKhoanShop.NgayTao = DateTime.Now;
                    taiKhoanShop.NgayCapNhat = DateTime.Now;
                    listNhomSanPham.Add(taiKhoanShop);
                    map.insertExcel(taiKhoanShop);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                return Redirect("Index");
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close(false, Type.Missing, Type.Missing);
                    Marshal.ReleaseComObject(workbook);
                }
                if (application != null)
                {
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                }

                if (range != null)
                {
                    Marshal.ReleaseComObject(range);
                }
                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(worksheet);
                }

                // Ensure all COM objects are properly released
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            var result = map.getAllList("", 1);
            return View("Index", result);
        }
        [AuthorizationCheck(ChucNang = "TaiKhoanShop_DownloadExcel")]
        public ActionResult DownloadExcel()
        {
            // Đường dẫn tới file trên server
            var filePath = Server.MapPath(map.pathFileUpLoadDowload + "TaiKhoanShop.xlsx");
            // Tên file khi người dùng tải về
            var fileName = "TaiKhoanShop.xlsx";
            // Trả về file như một response
            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [AuthorizationCheck(ChucNang = "TaiKhoanShop_Export")]
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
                    worksheet.Cells[1, 1].Value = "Tên Shop";
                    worksheet.Cells[1, 2].Value = "Tên tài khoản";
                    worksheet.Cells[1, 3].Value = "Email";
                    worksheet.Cells[1, 4].Value = "Số điện thoại";
                    worksheet.Cells[1, 5].Value = "Địa chỉ";
                    worksheet.Cells[1, 6].Value = "FaceBook";
                    worksheet.Cells[1, 7].Value = "Ngày cập nhật";


                    // Đặt màu nền và màu chữ cho dòng 1
                    using (ExcelRange range = worksheet.Cells["A1:G1"])
                    {
                        range.Style.Font.Color.SetColor(Color.White); // Màu chữ là màu trắng
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Màu nền là màu xanh
                        range.Style.Font.Bold = true; // Đặt đậm cho font chữ
                        range.Style.Font.Size = 13; // Đặt cỡ chữ
                    }

                    // Đổ dữ liệu từ danh sách vào Excel
                    int row = 2;
                    foreach (var TaiKhoanShop in listNhomSanPham)
                    {
                        worksheet.Cells[row, 1].Value = TaiKhoanShop.db.TenShop;
                        worksheet.Cells[row, 2].Value = TaiKhoanShop.db.Username;
                        worksheet.Cells[row, 3].Value = TaiKhoanShop.db.Email;
                        worksheet.Cells[row, 4].Value = TaiKhoanShop.db.SoDienThoai;
                        worksheet.Cells[row, 5].Value = TaiKhoanShop.db.DiaChi;
                        worksheet.Cells[row, 6].Value = TaiKhoanShop.db.Facebook;
                        worksheet.Cells[row, 7].Value = TaiKhoanShop.db.NgayCapNhat != null ? TaiKhoanShop.db.NgayCapNhat.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        row++;
                    }

                    using (ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, 7])
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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TaiKhoanShop.xlsx");
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