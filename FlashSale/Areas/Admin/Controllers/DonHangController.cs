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
    public class DonHangController : Controller
    {
        // GET: Admin/DonHang
        mapDonHang map = new mapDonHang();

        [AuthorizationCheck(ChucNang = "DonHang_Index")]
        public ActionResult Index(DonHangViewModel model)
        {
            model.TypeAction = 1;
            model = map.getAllList(model);
            return View(model);
        }


        [AuthorizationCheck(ChucNang = "DonHang_Insert")]
        public ActionResult Insert()
        {
            return View(new DonHangModel()
            {
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Insert(DonHangModel model)
        {
           

            int check = CheckValidation(model, 1);
            if (check == 1)
            {
                model.db.TongTienSanPham = map.FormatDecimalView(model.TongTienSanPhamView);
                model.db.TrietKhau = map.FormatDecimalView(model.TrietKhauView); ;
                model.db.PhiShip = map.FormatDecimalView(model.PhiShipView); ;
                model.db.Vat = map.FormatDecimalView(model.VatView); ;
                model.db.TongThanhToan = map.FormatDecimalView(model.TongThanhToanView); ;
                model.db.ID = Guid.NewGuid();
                model.db.NgayTao = DateTime.Now;
                model.db.StatusDel = 1;
                model.db.NgayCapNhat = DateTime.Now;
                map.insert(model);
                return Redirect("Index");
            }
            return View(model);

        }


        int CheckValidation(DonHangModel model, int type)
        {
            
            if (model.db.idShop == null)
            {
                ModelState.AddModelError("db.idShop", Data.Helpers.Common.Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.db.SoDienThoai))
            {
                ModelState.AddModelError("db.SoDienThoai", Data.Helpers.Common.Constants.required);
            }
            else
            {
                if (!Regex.IsMatch(model.db.SoDienThoai, map.phonePattern))
                {
                    ModelState.AddModelError("db.SoDienThoai", Data.Helpers.Common.Constants.phoneNotCorrect);
                }
            }


          


            if (string.IsNullOrWhiteSpace(model.db.DiaChiGiaoHang))
            {
                ModelState.AddModelError("db.DiaChiGiaoHang", Data.Helpers.Common.Constants.required);
            }
            if (String.IsNullOrEmpty(model.TongTienSanPhamView))
            {
                ModelState.AddModelError("TongTienSanPhamView", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.TongTienSanPhamView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("TongTienSanPhamView", "Tổng tiền sản phâm phải lớn hơn 0");
            }

            if (String.IsNullOrEmpty(model.TrietKhauView))
            {
                ModelState.AddModelError("TrietKhauView", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.TrietKhauView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("TrietKhauView", "Triết khấu phải lớn hơn 0");
            }

            if (String.IsNullOrEmpty(model.PhiShipView))
            {
                ModelState.AddModelError("PhiShipView", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.PhiShipView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("PhiShipView", "Phí ship phải lớn hơn 0");
            }

            if(model.db.idVat != null)
            {
                if (String.IsNullOrEmpty(model.VatView))
                {
                    ModelState.AddModelError("VatView", Data.Helpers.Common.Constants.required);
                }
                else if (decimal.Parse(model.VatView.Replace(",", "")) <= 0)
                {
                    ModelState.AddModelError("VatView", "Vat phải lớn hơn 0");
                }
            }
         
            if (String.IsNullOrEmpty(model.TongThanhToanView))
            {
                ModelState.AddModelError("TongThanhToanView", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.TongThanhToanView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("TongThanhToanView", "Tổng tiền thanh toán phải lớn hơn 0");
            }

            return ModelState.IsValid ? 1 : 0;
        }


        [AuthorizationCheck(ChucNang = "DonHang_Edit")]
        public ActionResult Edit(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(DonHangModel model)
        {
          
            int check = CheckValidation(model, 2);
            if (check == 1)
            {
                model.db.TongTienSanPham = map.FormatDecimalView(model.TongTienSanPhamView);;
                model.db.TrietKhau = map.FormatDecimalView(model.TrietKhauView);;
                model.db.PhiShip = map.FormatDecimalView(model.PhiShipView); ;
                model.db.Vat = map.FormatDecimalView(model.VatView); ;
                model.db.TongThanhToan = map.FormatDecimalView(model.TongThanhToanView); ;
                model.db.NgayCapNhat = DateTime.Now;
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "DonHang_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [AuthorizationCheck(ChucNang = "DonHang_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }

       

        [AuthorizationCheck(ChucNang = "DonHang_Export")]
        public ActionResult Export(DonHangViewModel model)
        {
            try
            {
                // Set the LicenseContext during application startup
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial
                model.TypeAction = 2;

                var modelFilter = map.getAllList(model);

                // Tạo một file Excel mới với EPPlus
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Đặt tiêu đề cột và dữ liệu
                    worksheet.Cells[1, 1].Value = "Tên shop";
                    worksheet.Cells[1, 2].Value = "Số điện thoại";
                    worksheet.Cells[1, 3].Value = "Địa chỉ giao hàng";
                    worksheet.Cells[1, 4].Value = "Thời gian đặt hàng";
                    worksheet.Cells[1, 5].Value = "Trạng thái";
                    worksheet.Cells[1, 6].Value = "Tổng tiền sản phẩm";
                    worksheet.Cells[1, 7].Value = "Triết khấu";
                    worksheet.Cells[1, 8].Value = "Phí ship";
                    worksheet.Cells[1, 9].Value = "Vat";
                    worksheet.Cells[1, 10].Value = "Tổng thanh toán";
                    worksheet.Cells[1, 11].Value = "Khách hàng ghi chú";
                    worksheet.Cells[1, 12].Value = "Ngày cập nhật";
                    // Đặt màu nền và màu chữ cho dòng 1
                    using (ExcelRange range = worksheet.Cells["A1:L1"])
                    {
                        range.Style.Font.Color.SetColor(Color.White); // Màu chữ là màu trắng
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Màu nền là màu xanh
                        range.Style.Font.Bold = true; // Đặt đậm cho font chữ
                        range.Style.Font.Size = 13; // Đặt cỡ chữ
                    }

                    // Đổ dữ liệu từ danh sách vào Excel
                    int row = 2;
                    foreach (var DonHang in modelFilter.DonHang)
                    {
                        var tenShop = map.db.TaiKhoanShops.Where(q => q.ID == DonHang.db.idShop).Where(q => q.StatusDel == 1).Select(q => q.TenShop).FirstOrDefault();
                        var nguoiCapNhat = map.db.TaiKhoanShops.Where(q => q.ID.ToString() == DonHang.db.NguoiCapNhat).Where(q => q.StatusDel == 1).Select(q => q.Username).FirstOrDefault();

                        worksheet.Cells[row, 1].Value = DonHang.TenShop;
                        worksheet.Cells[row, 2].Value = DonHang.db.SoDienThoai;
                        worksheet.Cells[row, 3].Value = DonHang.db.DiaChiGiaoHang;
                        worksheet.Cells[row, 4].Value = DonHang.db.ThoiGianDatHang.ToString();
                        worksheet.Cells[row, 5].Value = DonHang.db.TrangThai.ToString();
                        worksheet.Cells[row, 6].Value = DonHang.db.TongTienSanPham;
                        worksheet.Cells[row, 7].Value = DonHang.db.TrietKhau;
                        worksheet.Cells[row, 8].Value = DonHang.db.PhiShip;
                        worksheet.Cells[row, 9].Value = DonHang.db.Vat;
                        worksheet.Cells[row, 10].Value = DonHang.db.TongThanhToan;
                        worksheet.Cells[row, 11].Value = DonHang.db.KhacHangGhiChu;
                        worksheet.Cells[row, 12].Value = DonHang.db.NgayCapNhat != null ? DonHang.db.NgayCapNhat.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        row++;
                    }

                    using (ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, 12])
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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DonHang.xlsx");
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