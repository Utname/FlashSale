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

namespace FlashSale.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: Admin/SanPham
        mapSanPham map = new mapSanPham();
        private static List<ImageProduct> images = new List<ImageProduct>();

        [AuthorizationCheck(ChucNang = "SanPham_Index")]
        public ActionResult Index(string search, string statusDel, int? idNhomSanPham, int page = 1)
        {
            statusDel = statusDel ?? "1";
            idNhomSanPham = idNhomSanPham ?? -1;
            int pageSize = 10;  // Kích thước trang
            int skip = (page - 1) * pageSize;

            var allItems = map.getAllList(search, int.Parse(statusDel), idNhomSanPham);
            var result = allItems.Skip(skip).Take(pageSize).ToList();

            ViewBag.search = search;
            ViewBag.statusDel = int.Parse(statusDel);
            ViewBag.idNhomSanPham = idNhomSanPham;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = allItems.Count();

            return View(result);
        }


        public ActionResult UploadImage(string id)
        {
            var imageList = GetImageList(id);
            return View(imageList);
        }

        private ImageProductModel GetImageList(string id)
        {
            var images = map.db.ImageProducts.Where(q => q.idProduct.ToString() == id).ToList();
            var model = new ImageProductModel();
            model.images = images;
            model.idProduct = id;
            model.NameProduct = map.db.SanPhams.Where(q => q.ID.ToString() == id).Select(q=>q.TenSanPham).FirstOrDefault();
            return model;
        }

        [HttpPost]
        public ActionResult Save(List<ImageProduct> updatedImages)
        {
            if (updatedImages.Count() == 0) return View();
            // Xóa các hình ảnh đã bị loại khỏi danh sách
            var imagesToDelete = images.Where(i => !updatedImages.Any(ui => ui.ID == i.ID)).ToList();
            foreach (var image in imagesToDelete)
            {
                var path = Server.MapPath(image.FilePath);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                images.Remove(image);
            }

            // Cập nhật tên file mới
            foreach (var updatedImage in updatedImages)
            {
                var image = images.FirstOrDefault(i => i.ID == updatedImage.ID);
                if (image != null)
                {
                    image.FileName = updatedImage.FileName;
                    image.FileExtension = Path.GetExtension(updatedImage.FileName); // Cập nhật phần mở rộng
                }
            }

            // Cập nhật thông tin hình ảnh vào cơ sở dữ liệu
            map.UpdateImageInfo(updatedImages);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath( map.imagePath), fileName);
                    
                    file.SaveAs(path);

                    // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                    //map.SaveImageInfo(new ImageProduct
                    //{
                    //    FileName = fileName,
                    //    FilePath = map.imagePath + fileName,
                    //    FileSize = file.ContentLength,
                    //    FileExtension = Path.GetExtension(fileName).TrimStart('.')
                    //});
                }
            }

            return Json(new { success = true }); // Trả về JSON để thông báo thành công
        }


        [AuthorizationCheck(ChucNang = "SanPham_Insert")]
        public ActionResult Insert()
        {
            return View(new SanPhamModel()
            {
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Insert(SanPhamModel model)
        {
         

            int check = CheckValidation(model, 1);
            if (check == 1)
            {
                model.db.GiaNiemYet = decimal.Parse(model.GiaNiemYetView.Replace(",", ""));
                model.db.GiaSale = decimal.Parse(model.GiaSaleView.Replace(",", ""));
                model.db.ID = Guid.NewGuid();
                model.db.idShop = Guid.Parse(map.GetUserId());
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
                //4.Kiểm tra tồn tại => Có tồn tại file cũ thì xóa
                //if(System.IO.File.Exists(ddTuyetDoi) == true)
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

        int CheckValidation(SanPhamModel model, int type)
        {
            if (string.IsNullOrWhiteSpace(model.db.TenSanPham))
            {
                ModelState.AddModelError("db.TenSanPham", Data.Helpers.Common.Constants.required);
            }
            else
            {
                var userId = map.GetUserId();
                var countSanPham = map.db.SanPhams.Where(q => q.TenSanPham == model.db.TenSanPham).Where(q => q.ID != model.db.ID).Where(q => q.idShop.ToString() == userId).Count();
                if (countSanPham > 0)
                {
                    ModelState.AddModelError("db.TenSanPham", Data.Helpers.Common.Constants.existed);
                }

            }

            if (String.IsNullOrEmpty(model.GiaNiemYetView))
            {
                ModelState.AddModelError("db.GiaNiemYet", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.GiaNiemYetView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("db.GiaNiemYet", "Giá niêm yết phải lớn hơn 0");
            }


            if (String.IsNullOrEmpty(model.GiaSaleView))
            {
                ModelState.AddModelError("db.GiaSale", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.GiaSaleView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("db.GiaSale", "Giá sale phải lớn hơn 0");
            }
            else if (decimal.Parse(model.GiaSaleView.Replace(",", "")) > decimal.Parse(model.GiaNiemYetView.Replace(",", "")))
            {
                ModelState.AddModelError("db.GiaSale", "Giá sale phải nhỏ hơn hoặc bằng giá niêm yết");
            }

            if (model.db.GioBatDau == null)
            {
                ModelState.AddModelError("db.GioBatDau", Data.Helpers.Common.Constants.required);
            }

            if (model.db.GioKetThuc == null)
            {
                ModelState.AddModelError("db.GioKetThuc", Data.Helpers.Common.Constants.required);
            }

            if (model.db.GioBatDau != null && model.db.GioKetThuc == null)
            {
                if (model.db.GioBatDau >= model.db.GioKetThuc)
                {
                    ModelState.AddModelError("db.GioKetThuc", "Giờ bắt đầu phải nhỏ hơn giờ kết thúc");
                }
            }
            if (model.db.idNhomSanPham == null)
            {
                ModelState.AddModelError("db.idNhomSanPham", Data.Helpers.Common.Constants.required);
            }

            return ModelState.IsValid ? 1 : 0;
        }


        [AuthorizationCheck(ChucNang = "SanPham_Edit")]
        public ActionResult Edit(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(SanPhamModel model)
        {
           
            int check = CheckValidation(model, 2);
            if (check == 1)
            {
                model.db.GiaNiemYet = decimal.Parse(model.GiaNiemYetView.Replace(",", ""));
                model.db.GiaSale = decimal.Parse(model.GiaSaleView.Replace(",", ""));
                model.db.NgayCapNhat = DateTime.Now;
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "SanPham_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [AuthorizationCheck(ChucNang = "SanPham_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }

       


        [HttpPost]
        [AuthorizationCheck(ChucNang = "SanPham_Import")]
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

                List<SanPhamModel> listNhomSanPham = new List<SanPhamModel>();
                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    SanPhamModel SanPham = new SanPhamModel();
                    SanPham.db.TenSanPham = ((Excel.Range)range.Cells[row, 1]).Text;
                    SanPham.db.GiaNiemYet =  decimal.Parse(((Excel.Range)range.Cells[row, 2]).Text);
                    SanPham.db.GiaSale = decimal.Parse(((Excel.Range)range.Cells[row, 3]).Text);
                    SanPham.db.GioBatDau = DateTime.Parse(((Excel.Range)range.Cells[row, 4]).Text);
                    SanPham.db.GioKetThuc = DateTime.Parse(((Excel.Range)range.Cells[row, 5]).Text);
                    SanPham.TenNhomSanPham = ((Excel.Range)range.Cells[row, 6]).Text;
                    var idNhomSanPham = map.db.NhomSanPhams.Where(q => q.TenNhom == SanPham.TenNhomSanPham).Select(q => q.ID).FirstOrDefault();
                    SanPham.db.StatusDel = 1;
                    SanPham.db.idNhomSanPham = idNhomSanPham;
                    SanPham.db.ID = Guid.NewGuid();
                    SanPham.db.NgayTao = DateTime.Now;
                    SanPham.db.NgayCapNhat = DateTime.Now;
                    listNhomSanPham.Add(SanPham);
                    map.insertExcel(SanPham.db);
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
            var result = map.getAllList("", 1,-1);
            return View("Index", result);
        }
        [AuthorizationCheck(ChucNang = "SanPham_DownloadExcel")]
        public ActionResult DownloadExcel()
        {
            // Đường dẫn tới file trên server
            var filePath = Server.MapPath(map.pathFileUpLoadDowload + "SanPham.xlsx");
            // Tên file khi người dùng tải về
            var fileName = "SanPham.xlsx";
            // Trả về file như một response
            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [AuthorizationCheck(ChucNang = "SanPham_Export")]
        public ActionResult Export()
        {
            try
            {
                // Set the LicenseContext during application startup
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

                var listNhomSanPham = map.getAllList("", 1,-1);

                // Tạo một file Excel mới với EPPlus
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Đặt tiêu đề cột và dữ liệu
                    worksheet.Cells[1, 1].Value = "Tên sản phẩm";
                    worksheet.Cells[1, 2].Value = "Giá niêm yết";
                    worksheet.Cells[1, 3].Value = "Giá sale";
                    worksheet.Cells[1, 4].Value = "Giờ bắt đầu";
                    worksheet.Cells[1, 5].Value = "Giờ kết thúc";
                    worksheet.Cells[1, 6].Value = "Nhóm sản phẩm";
                    worksheet.Cells[1, 7].Value = "Ngày cập nhật";
                    worksheet.Cells[1, 8].Value = "Người cập nhật";



                    // Đặt màu nền và màu chữ cho dòng 1
                    using (ExcelRange range = worksheet.Cells["A1:H1"])
                    {
                        range.Style.Font.Color.SetColor(Color.White); // Màu chữ là màu trắng
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Màu nền là màu xanh
                        range.Style.Font.Bold = true; // Đặt đậm cho font chữ
                        range.Style.Font.Size = 13; // Đặt cỡ chữ
                    }

                    // Đổ dữ liệu từ danh sách vào Excel
                    int row = 2;
                    foreach (var SanPham in listNhomSanPham)
                    {
                        var tenNhomSanPham = map.db.NhomSanPhams.Where(q => q.ID == SanPham.db.idNhomSanPham).Where(q => q.StatusDel == 1).Select(q => q.TenNhom).FirstOrDefault();
                        var nguoiCapNhat = map.db.TaiKhoanShops.Where(q => q.ID == SanPham.db.idShop).Where(q => q.StatusDel == 1).Select(q => q.Username).FirstOrDefault();

                        worksheet.Cells[row, 1].Value = SanPham.db.TenSanPham;
                        worksheet.Cells[row, 2].Value = SanPham.db.GiaNiemYet;
                        worksheet.Cells[row, 3].Value = SanPham.db.GiaSale;
                        worksheet.Cells[row, 4].Value = SanPham.db.GioBatDau.ToString();
                        worksheet.Cells[row, 5].Value = SanPham.db.GioKetThuc.ToString();
                        worksheet.Cells[row, 6].Value = tenNhomSanPham;
                        worksheet.Cells[row, 7].Value = SanPham.db.NgayCapNhat != null ? SanPham.db.NgayCapNhat.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 8].Value = nguoiCapNhat;
                        row++;
                    }

                    using (ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, 8])
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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SanPham.xlsx");
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