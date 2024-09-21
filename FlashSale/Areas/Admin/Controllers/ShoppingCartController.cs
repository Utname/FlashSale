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
using CKFinder.Settings;
using System.Text;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Security.Cryptography;

namespace FlashSale.Areas.Admin.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: Admin/ShoppingCart
        mapShoppingCart map = new mapShoppingCart();
        [AuthorizationCheck(ChucNang = "ShoppingCart_Index")]
        public ActionResult Index(ShoppingCartViewModel model)
        {
            model.TypeAction = 1;
            model = map.getAllList(model);
            return View(model);
        }


        [HttpGet]
        [AuthorizationCheck(ChucNang = "ShoppingCart_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [AuthorizationCheck(ChucNang = "ShoppingCart_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }



        [AuthorizationCheck(ChucNang = "ShoppingCart_Export")]
        [HttpGet]
        public ActionResult Export(ShoppingCartViewModel model)
        {
            try
            {
                // Set the LicenseContext during application startup
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial
                model.TypeAction = 1;

                var modelFilter = map.getAllList(model);

                // Tạo một file Excel mới với EPPlus
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Đặt tiêu đề cột và dữ liệu
                    worksheet.Cells[1, 1].Value = "Tên shop";
                    worksheet.Cells[1, 2].Value = "Tên user";
                    worksheet.Cells[1, 3].Value = "Sản phẩm";
                    worksheet.Cells[1, 4].Value = "Tổng số tiền sản phẩm";
                    worksheet.Cells[1, 5].Value = "Tổng số lượng";
                    worksheet.Cells[1, 5].Value = "Phí ship";
                    worksheet.Cells[1, 6].Value = "Mã voucher giảm giá";
                    worksheet.Cells[1, 7].Value = "Số tiền giảm";
                    worksheet.Cells[1, 8].Value = "Tổng thanh toán";
                    worksheet.Cells[1, 9].Value = "Trạng thái";
                    worksheet.Cells[1, 10].Value = "Ngày cập nhật";
                    worksheet.Cells[1, 11].Value = "Người cập nhật";
                    // Đặt màu nền và màu chữ cho dòng 1
                    using (ExcelRange range = worksheet.Cells["A1:K1"])
                    {
                        range.Style.Font.Color.SetColor(Color.White); // Màu chữ là màu trắng
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Màu nền là màu xanh
                        range.Style.Font.Bold = true; // Đặt đậm cho font chữ
                        range.Style.Font.Size = 13; // Đặt cỡ chữ
                    }

                    // Đổ dữ liệu từ danh sách vào Excel
                    int row = 2;
                    foreach (var ShoppingCart in modelFilter.ShoppingCarts)
                    {

                        var shopName = map.db.TaiKhoanShops.Where(q => q.ID == ShoppingCart.db.IdShop).Select(q => q.TenShop).SingleOrDefault();
                        var userName = map.db.TaiKhoanShops.Where(q => q.ID.ToString() == ShoppingCart.db.CreateBy).Select(q => q.TenShop).SingleOrDefault();
                        var productName = "";

                        var listIdProduct = map.db.ShoppingCartDetails.Where(q => q.idShoppingCart == ShoppingCart.db.ID).Where(q => q.StatusDel == 1).Select(q => q.idProudct).Distinct().ToList();
                        listIdProduct.ForEach(q =>
                        {
                            productName = productName + map.db.Products.Where(d => d.ID == q).Select(d => d.Name).FirstOrDefault() + "";
                        });

                        worksheet.Cells[row, 1].Value = shopName;
                        worksheet.Cells[row, 2].Value = userName;
                        worksheet.Cells[row, 3].Value = productName;
                        worksheet.Cells[row, 4].Value =
                          ShoppingCart.db.TotalProductPrice == null ? "0" : map.FormatDecimalViewString((decimal)ShoppingCart.db.TotalProductPrice)
                            + " - "
                            + ShoppingCart.db.TotalQantity == null ? "0" : map.FormatIntViewString((int)ShoppingCart.db.TotalQantity);
                        worksheet.Cells[row, 5].Value =
                            ShoppingCart.db.ShippingFee == null ? "0" : map.FormatDecimalViewString((int)ShoppingCart.db.ShippingFee);

                        worksheet.Cells[row, 6].Value = ShoppingCart.db.VoucherCode;
                        worksheet.Cells[row, 7].Value =
                            ShoppingCart.db.DiscountAmount == null ? "0" : map.FormatDecimalViewString((decimal)ShoppingCart.db.DiscountAmount)
                           ;
                        worksheet.Cells[row, 8].Value =
                           ShoppingCart.db.Total == null ? "0" : map.FormatDecimalViewString((decimal)ShoppingCart.db.Total)
                          ;
                        worksheet.Cells[row, 9].Value = "Đang xử lý";
                        worksheet.Cells[row, 10].Value = ShoppingCart.db.CreateDate != null ? ShoppingCart.db.CreateDate.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 11].Value = ShoppingCart.UpdateByName;
                        row++;
                    }

                    using (ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, 11])
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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ShoppingCart.xlsx");
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