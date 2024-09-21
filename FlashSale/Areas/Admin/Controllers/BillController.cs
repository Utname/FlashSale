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
    public class BillController : Controller
    {
        // GET: Admin/Bill
        mapBill map = new mapBill();
        [AuthorizationCheck(ChucNang = "Bill_Index")]
        public ActionResult Index(BillViewModel model)
        {
            model.TypeAction = 1;
            model = map.getAllList(model);
            return View(model);
        }


        [HttpGet]
        [AuthorizationCheck(ChucNang = "Bill_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [AuthorizationCheck(ChucNang = "Bill_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }



        [AuthorizationCheck(ChucNang = "Bill_Export")]
        [HttpGet]
        public ActionResult Export(BillViewModel model)
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
                    worksheet.Cells[1, 2].Value = "Số điện thoại shop";
                    worksheet.Cells[1, 3].Value = "Địa chỉ shop";
                    worksheet.Cells[1, 4].Value = "Tên user";
                    worksheet.Cells[1, 5].Value = "Số điện thoại user";
                    worksheet.Cells[1, 6].Value = "Email user";
                    worksheet.Cells[1, 7].Value = "Địa chỉ user";
                    worksheet.Cells[1, 8].Value = "Sản phẩm";
                    worksheet.Cells[1, 9].Value = "Địa chỉ giao hàng";
                    worksheet.Cells[1, 10].Value = "Ngày đặt hàng";
                    worksheet.Cells[1, 11].Value = "Ngày giao hàng";
                    worksheet.Cells[1, 12].Value = "Trạng thái đơn hàng";
                    worksheet.Cells[1, 13].Value = "Tổng số lượng";
                    worksheet.Cells[1, 14].Value = "Phí ship";
                    worksheet.Cells[1, 15].Value = "Tổng thành tiền";
                    worksheet.Cells[1, 16].Value = "Ngày cập nhật";
                    worksheet.Cells[1, 17].Value = "Người cập nhật";
                    // Đặt màu nền và màu chữ cho dòng 1
                    using (ExcelRange range = worksheet.Cells["A1:Q1"])
                    {
                        range.Style.Font.Color.SetColor(Color.White); // Màu chữ là màu trắng
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Màu nền là màu xanh
                        range.Style.Font.Bold = true; // Đặt đậm cho font chữ
                        range.Style.Font.Size = 13; // Đặt cỡ chữ
                    }

                    // Đổ dữ liệu từ danh sách vào Excel
                    int row = 2;
                    foreach (var Bill in modelFilter.Bills)
                    {
                        var productName = "";
                        var listIdProduct = map.db.ShoppingCartDetails.Where(q => q.idShoppingCart == Bill.db.ID).Where(q => q.StatusDel == 1).Select(q => q.idProudct).Distinct().ToList();
                        listIdProduct.ForEach(q =>
                        {
                            productName = productName + map.db.Products.Where(d => d.ID == q).Select(d => d.Name).FirstOrDefault() + "";
                        });

                        worksheet.Cells[1, 1].Value = "Tên shop";
                        worksheet.Cells[1, 2].Value = "Số điện thoại shop";
                        worksheet.Cells[1, 3].Value = "Địa chỉ shop";
                        worksheet.Cells[1, 4].Value = "Tên user";
                        worksheet.Cells[1, 5].Value = "Số điện thoại user";
                        worksheet.Cells[1, 6].Value = "Email user";
                        worksheet.Cells[1, 7].Value = "Sản phẩm";
                        worksheet.Cells[1, 8].Value = "Địa chỉ giao hàng";
                        worksheet.Cells[1, 9].Value = "Ngày đặt hàng";
                        worksheet.Cells[1, 10].Value = "Ngày giao hàng";
                        worksheet.Cells[1, 11].Value = "Trạng thái đơn hàng";
                        worksheet.Cells[1, 12].Value = "Tổng số lượng";
                        worksheet.Cells[1, 13].Value = "Phí ship";
                        worksheet.Cells[1, 14].Value = "Tổng thành tiền";
                        worksheet.Cells[1, 15].Value = "Ngày cập nhật";
                        worksheet.Cells[1, 16].Value = "Người cập nhật";

                        worksheet.Cells[row, 1].Value = Bill.db.ShopName;
                        worksheet.Cells[row, 2].Value = Bill.db.PhoneShop;
                        worksheet.Cells[row, 3].Value = Bill.db.AddressShop;
                        worksheet.Cells[row, 4].Value = Bill.db.UserName;
                        worksheet.Cells[row, 5].Value = Bill.db.PhoneUser;
                        worksheet.Cells[row, 6].Value = Bill.db.EmailUser;
                        worksheet.Cells[row, 7].Value = productName;
                        worksheet.Cells[row, 8].Value = Bill.db.DeliveryAddress;
                        worksheet.Cells[row, 9].Value = Bill.db.OrderDate != null ? Bill.db.OrderDate.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 10].Value = Bill.db.DeliveryDate != null ? Bill.db.DeliveryDate.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 11].Value ="Đang xử lý";
                        worksheet.Cells[row, 12].Value =
                          Bill.db.TotalQantity == null ? "0" : map.FormatDecimalViewString((int)Bill.db.TotalQantity);
                        worksheet.Cells[row, 13].Value =
                            Bill.db.ShippingFee == null ? "0" : map.FormatDecimalViewString((int)Bill.db.ShippingFee);
                        worksheet.Cells[row, 14].Value =
                           Bill.db.Total == null ? "0" : map.FormatDecimalViewString((decimal)Bill.db.Total)
                          ;
                        worksheet.Cells[row, 15].Value = "Đang xử lý";
                        worksheet.Cells[row, 16].Value = Bill.db.CreateDate != null ? Bill.db.CreateDate.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 17].Value = Bill.UpdateByName;
                        row++;
                    }

                    using (ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, 17])
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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Bill.xlsx");
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