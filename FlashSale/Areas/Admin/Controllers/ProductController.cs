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
    public class ProductController : Controller
    {
        // GET: Admin/Product
        mapProduct map = new mapProduct();
        private static List<ImageProduct> images = new List<ImageProduct>();

        [AuthorizationCheck(ChucNang = "Product_Index")]
        public ActionResult Index(ProductViewModel model)
        {
            model.TypeAction = 1;
            model = map.getAllList(model);
            return View(model);
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
            model.NameProduct = map.db.Products.Where(q => q.ID.ToString() == id).Select(q => q.Name).FirstOrDefault();
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
                    var path = Path.Combine(Server.MapPath(map.imagePath), fileName);

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


        [AuthorizationCheck(ChucNang = "Product_Insert")]
        public ActionResult Insert()
        {
            return View(new ProductModel()
            {
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Insert(ProductModel model)
        {
            int check = CheckValidation(model, 1);
            if (check == 1)
            {
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

        int CheckValidation(ProductModel model, int type)
        {
            if (string.IsNullOrWhiteSpace(model.db.Name))
            {
                ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.required);
            }
            else
            {
                var userId = map.GetUserId();
                var countProduct = map.db.Products.Where(q => q.Name == model.db.Name).Where(q => q.ID != model.db.ID).Where(q => q.idShop.ToString() == userId).Count();
                if (countProduct > 0)
                {
                    ModelState.AddModelError("db.Name", Data.Helpers.Common.Constants.existed);
                }

            }


            if (model.db.idGroup == null)
            {
                ModelState.AddModelError("db.idGroup", Data.Helpers.Common.Constants.required);
            }

            if (model.db.idType == null)
            {
                ModelState.AddModelError("db.idType", Data.Helpers.Common.Constants.required);
            }




            if (String.IsNullOrEmpty(model.QuantityView))
            {
                ModelState.AddModelError("QuantityView", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.QuantityView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("QuantityView", "Số lượng phải lớn hơn 0");
            }

            if (String.IsNullOrEmpty(model.StartingPriceView))
            {
                ModelState.AddModelError("StartingPriceView", Data.Helpers.Common.Constants.required);
            }
            else if (decimal.Parse(model.StartingPriceView.Replace(",", "")) <= 0)
            {
                ModelState.AddModelError("StartingPriceView", "Giá từ phải lớn hơn 0");
            }


            if (!String.IsNullOrEmpty(model.EndingPriceView))
            {
                if (decimal.Parse(model.EndingPriceView.Replace(",", "")) <= 0)
                {
                    ModelState.AddModelError("EndingPriceView", "Giá đến phải lớn hơn 0");
                }
                else if (decimal.Parse(model.StartingPriceView.Replace(",", "")) > decimal.Parse(model.EndingPriceView.Replace(",", "")))
                {
                    ModelState.AddModelError("StartingPriceView", "Giá từ phải nhỏ hơn hoặc bằng giá đến");
                }
            }


            if ((model.db.StartTime != null && model.db.EndTime != null))
            {

                if (model.db.StartTime >= model.db.EndTime)
                {
                    ModelState.AddModelError("db.StartTime", "Giờ bắt đầu phải nhỏ hơn giờ kết thúc");
                }
            }



            return ModelState.IsValid ? 1 : 0;
        }


        [AuthorizationCheck(ChucNang = "Product_Edit")]
        public ActionResult Edit(string id)
        {
            return View(map.details(id));
        }

        [HttpPost]
        public ActionResult Edit(ProductModel model)
        {

            int check = CheckValidation(model, 2);
            if (check == 1)
            {
                map.edit(model);
                return Redirect("Index");
            }
            return View(model);

        }

        [HttpGet]
        [AuthorizationCheck(ChucNang = "Product_UpdateStatusDel")]
        public ActionResult UpdateStatusDel(string id, int statusDel)
        {
            map.updateStatusDel(id, statusDel);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [AuthorizationCheck(ChucNang = "Product_Details")]
        public ActionResult Details(string id)
        {
            return View(map.details(id));
        }





        [HttpPost]
        [AuthorizationCheck(ChucNang = "Product_Import")]

        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Please select an excel file";
                return RedirectToAction("Index");
            }

            if (!excelfile.FileName.EndsWith("xls") && !excelfile.FileName.EndsWith("xlsx"))
            {
                ViewBag.Error = "Please select a valid excel file";
                return RedirectToAction("Index");
            }

            string path = Server.MapPath(map.pathFileUploadExcel + excelfile.FileName);
            if (System.IO.File.Exists(path))
            {
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        // Đảm bảo file không còn bị khóa
                        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fileStream.Close(); // Đóng file để giải phóng quyền truy cập
                        }

                        System.IO.File.Delete(path);
                    }
                    catch (IOException ex)
                    {
                        // Ghi log lỗi xóa file
                        // Thực hiện các bước cần thiết để xử lý lỗi và tiếp tục
                        ViewBag.Error = "Unable to delete the existing file. Please ensure the file is not open and try again.";
                    }
                }
            }
            excelfile.SaveAs(path);

            Excel.Application application = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;
            Excel.Range range = null;

            var errorLogStream = new MemoryStream();
            var hasError = false;

            try
            {
                application = new Excel.Application();
                workbook = application.Workbooks.Open(path);
                worksheet = workbook.ActiveSheet;
                range = worksheet.UsedRange;

                List<ProductModel> listNhomProduct = new List<ProductModel>();

                using (var writer = new StreamWriter(errorLogStream, Encoding.UTF8, 1024, true))
                {
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        ProductModel Product = new ProductModel();
                        // Xử lý dữ liệu
                        Product.db.Name = ((Excel.Range)range.Cells[row, 1]).Text;
                        Product.ShopName = ((Excel.Range)range.Cells[row, 2]).Text;
                        Product.GroupProductName = ((Excel.Range)range.Cells[row, 3]).Text;
                        Product.TypeProductName = ((Excel.Range)range.Cells[row, 4]).Text;
                        Product.ShippingMethodName = ((Excel.Range)range.Cells[row, 5]).Text;
                        Product.ShippingFeeView = ((Excel.Range)range.Cells[row, 6]).Text;
                        Product.StartingPriceView = ((Excel.Range)range.Cells[row, 7]).Text;
                        Product.EndingPriceView = ((Excel.Range)range.Cells[row, 8]).Text;
                        Product.DiscountPercentageView = ((Excel.Range)range.Cells[row, 9]).Text;
                        Product.QuantityView = ((Excel.Range)range.Cells[row, 10]).Text;
                        Product.ProductCategoryName = ((Excel.Range)range.Cells[row, 11]).Text;
                        Product.ProductClassificationName = ((Excel.Range)range.Cells[row, 12]).Text;
                        Product.WanrrantyName = ((Excel.Range)range.Cells[row, 13]).Text;
                        Product.ReturnAndExchangePolicyName = ((Excel.Range)range.Cells[row, 14]).Text;
                        Product.db.StartTime = DateTime.Parse(((Excel.Range)range.Cells[row, 15]).Text);
                        Product.db.EndTime = DateTime.Parse(((Excel.Range)range.Cells[row, 16]).Text);
                       
                        writer.WriteLine($"{row} -----------------------------------------------------------------------");
                       
                        // Kiểm tra lỗi và ghi vào MemoryStream nếu có
                        var idShop = map.db.TaiKhoanShops.Where(q => q.TenShop.ToLower() == Product.ShopName.ToLower()).Select(q => q.ID).FirstOrDefault();
                        if (idShop == null)
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row} - {Product.ShopName} : Shop không tồn tại.");
                        }
                        else
                        {
                            Product.db.idShop = idShop;
                        }
                       

                        var idGroup = map.db.NhomSanPhams.Where(q => q.TenNhom.ToLower() == Product.GroupProductName.ToLower()).Select(q => q.ID).FirstOrDefault();
                        if (idGroup == 0)
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row} - {Product.GroupProductName}: Nhóm sản phẩm không tồn tại.");
                        }
                        else
                        {
                            Product.db.idGroup = idGroup;
                        }

                        var idType = map.db.TypeProducts.Where(q => q.Name.ToLower() == Product.TypeProductName.Trim().ToLower()).Select(q => q.ID).FirstOrDefault();
                        if (idType == 0)
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row} - {Product.TypeProductName}: Loại sản phẩm không tồn tại.");
                        }
                        else
                        {
                            Product.db.idType = idType;
                        }

                        if (String.IsNullOrEmpty(Product.ShippingMethodName))
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row}:Vui lòng nhập phương thức thanh toán.");
                        }
                        else if (Product.ShippingMethodName.ToLower() != "miễn phí" && Product.ShippingMethodName.ToLower() != "có phí")
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row} - {Product.ShippingMethodName}:Phương thức thanh toán không đúng.");
                        }
                        else if (Product.ShippingMethodName.ToLower() == "có phí")
                        {
                            if (String.IsNullOrEmpty(Product.ShippingFeeView))
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row}:Vui lòng nhập số tiền vận chuyển.");
                            }

                            else if (decimal.Parse(Product.ShippingFeeView.Replace(",", "")) <= 0)
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row} - {Product.ShippingFeeView}:Số tiền vận chuyển chưa đúng.");
                            }
                            else
                            {
                                Product.db.ShippingMethod = 2;
                            }
                        }
                        else
                        {
                            Product.db.ShippingMethod = 1;
                        }

                        if (String.IsNullOrEmpty(Product.StartingPriceView))
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row}:Vui lòng nhập đơn giá.");
                        }

                        else if (decimal.Parse(Product.StartingPriceView.Replace(",", "")) <= 0)
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row} - {Product.StartingPriceView}:Đơn giá từ phải lơn hơn 0.");
                        }
                        else
                        {
                            Product.db.StartingPrice = String.IsNullOrEmpty(Product.StartingPriceView) ? 0 : decimal.Parse(Product.StartingPriceView.Replace(",", ""));
                        }


                        if (!String.IsNullOrEmpty(Product.EndingPriceView))
                        {
                            if (decimal.Parse(Product.EndingPriceView.Replace(",", "")) <= 0)
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row}:Đơn giá đến phải lơn hơn 0.");
                            }

                            else if (decimal.Parse(Product.EndingPriceView.Replace(",", "")) <= decimal.Parse(Product.StartingPriceView.Replace(",", "")))
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row}:Đơn giá đến ({Product.EndingPriceView}) phải lơn hơn  đơn giá từ ({Product.StartingPriceView}).");
                            }
                            else
                            {
                                Product.db.EndingPrice = String.IsNullOrEmpty(Product.EndingPriceView) ? 0 : decimal.Parse(Product.EndingPriceView.Replace(",", ""));
                            }
                        }
                       


                        if (!String.IsNullOrEmpty(Product.DiscountPercentageView))
                        {
                            if (decimal.Parse(Product.DiscountPercentageView.Replace(",", "")) <= 0 && decimal.Parse(Product.DiscountPercentageView.Replace(",", "")) > 100)
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row} - {Product.DiscountPercentageView}:Phần trăm giảm giá phải lớn hơn 0 và phải nhở hơn bằng 100.");
                            }
                            else
                            {
                                Product.db.DiscountPercentage = String.IsNullOrEmpty(Product.DiscountPercentageView) ? 0 : int.Parse(Product.DiscountPercentageView.Replace(",", ""));
                            }
                        }


                        if (String.IsNullOrEmpty(Product.QuantityView))
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row}:Vui lòng nhập số lượng sản phẩm.");
                        }

                        else if (decimal.Parse(Product.QuantityView.Replace(",", "")) <= 0)
                        {
                            hasError = true;
                            writer.WriteLine($"Dòng {row} - {Product.QuantityView}:Sơ lượng sản phẩm phải lớn hơn 0.");
                        }
                        else
                        {
                            Product.db.Quantity = String.IsNullOrEmpty(Product.QuantityView) ? 0 : int.Parse(Product.QuantityView.Replace(",", ""));
                        }

                        if (Product.ProductCategoryName != null)
                        {
                            var productCategory= map.db.ProductCategories.Where(q => q.Name.ToLower() == Product.ProductCategoryName.ToLower()).FirstOrDefault();
                            if (productCategory == null)
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row} - {Product.ProductCategoryName}: Không tìm thấy danh mục sản phẩm.");
                            }
                            else
                            {
                                Product.db.idProductCategory = productCategory.ID;
                            }

                            if (String.IsNullOrEmpty(Product.ProductCategoryName))
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row} - {Product.ProductCategoryName}: Vui lòng nhập phân loại sản phẩm.");
                            }
                            else
                            {
                                var listProdictClassification = Product.ProductClassificationName.Split(',').ToList();
                                var idProductClassification = "";
                                foreach (var item in listProdictClassification)
                                {
                                    var productClassficationId = map.db.ProductClassifications.Where(q => q.Name.ToLower() == item.ToLower()).Where(q=>q.idProductCategory == productCategory.ID).Select(q => q.ID).FirstOrDefault();
                                    if (productClassficationId == 0)
                                    {
                                        hasError = true;
                                        writer.WriteLine($"Dòng {row} - {item}: Không tìm thấy phân loại sản phẩm.");
                                    }
                                    else
                                    {
                                        idProductClassification += productClassficationId + ",";
                                    }
                                }
                                Product.db.idProductClassification = idProductClassification;
                            }

                        }

                        if (!String.IsNullOrEmpty(Product.WanrrantyName))
                        {
                            var idWarranty = map.db.Warranties.Where(q => q.Name.ToLower() == Product.WanrrantyName.ToLower()).Select(q => q.ID).FirstOrDefault();
                            if (idWarranty == 0)
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row} - {Product.WanrrantyName}: Không tìm thấy bảo phành.");
                            }
                            else
                            {
                                Product.db.idWanrranty = idWarranty;
                            }
                        }


                        if (!String.IsNullOrEmpty(Product.ReturnAndExchangePolicyName))
                        {
                            var idReturnAndChangePolicy = map.db.ReturnAndExchangePolicies.Where(q => Product.ReturnAndExchangePolicyName.ToLower().Contains(q.Name.ToLower())).Select(q => q.ID).FirstOrDefault();
                            if (idReturnAndChangePolicy == 0)
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row} - {Product.ReturnAndExchangePolicyName}: Không tìm thấy chính sách đổi trả.");
                            }
                            else
                            {
                                Product.db.idReturnAndExchangePolicy = idReturnAndChangePolicy;
                            }
                        }

                        if ((Product.db.StartTime != null && Product.db.EndTime != null))
                        {

                            if (Product.db.StartTime > Product.db.EndTime)
                            {
                                hasError = true;
                                writer.WriteLine($"Dòng {row}: Ngày bắt đầu ({Product.db.StartTime})  không được lớn hơn ngày kết thúc ({Product.db.EndTime})");
                            }
                        }

                        if(hasError == true)
                        {
                            writer.WriteLine($"-----------------------------------------------------------------------");
                        }

                        listNhomProduct.Add(Product);
                    }
                }

                if (hasError)
                {
                    errorLogStream.Position = 0;
                    return File(errorLogStream, "text/plain", "ErrorLog.txt");
                }
                else
                {
                    // Lưu dữ liệu vào cơ sở dữ liệu
                    foreach (var product in listNhomProduct)
                    {
                        product.db.DiscountPercentage = String.IsNullOrEmpty(product.DiscountPercentageView) ? 0 : int.Parse(product.DiscountPercentageView.Replace(",", ""));
                        if (product.db.DiscountPercentage != 0)
                        {
                            product.db.DiscountFrom = (product.db.StartingPrice * product.db.DiscountPercentage) / 100;
                            product.db.DiscountUpTo = (product.db.EndingPrice * product.db.DiscountPercentage) / 100;
                        }
                        product.db.UpdateDate = DateTime.Now;
                        product.db.UpdateBy = map.GetUserId();

                        product.db.RemainingQuantity = product.db.Quantity;
                        var userId = map.GetUserId();
                        var resultFIndProduct = map.db.Products.Where(q => q.Name == product.db.Name).Where(q => q.idShop == product.db.idShop).FirstOrDefault();
                        if (resultFIndProduct == null)
                        {
                            product.db.StatusDel = 1;
                            product.db.ID = Guid.NewGuid();
                            product.db.CreateDate = DateTime.Now;
                            product.db.CreateBy = map.GetUserId();
                            map.insertExcel(product.db);
                        }
                        else
                        {
                            product.db.ID = resultFIndProduct.ID;
                            map.editExcel(product);
                        }
                       
                    }
                }
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

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            var modelFilter = new ProductViewModel();
            modelFilter.StatusDel =1;
            modelFilter.IdGroup =  -1;
            modelFilter.PageSize = 10; // Kích thước trang
            modelFilter = map.getAllList(modelFilter);
            return View("Index", modelFilter);
        }




        [AuthorizationCheck(ChucNang = "Product_DownloadExcel")]
        public ActionResult DownloadExcel()
        {
            // Đường dẫn tới file trên server
            var filePath = Server.MapPath(map.pathFileUpLoadDowload + "Product.xlsx");
            // Tên file khi người dùng tải về
            var fileName = "Product.xlsx";
            // Trả về file như một response
            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [AuthorizationCheck(ChucNang = "Product_Export")]
        [HttpGet]
        public ActionResult Export(ProductViewModel model)
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
                    worksheet.Cells[1, 1].Value = "Tên sản phẩm";
                    worksheet.Cells[1, 2].Value = "Tên shop";
                    worksheet.Cells[1, 3].Value = "Nhóm sản phẩm";
                    worksheet.Cells[1, 4].Value = "Loại sản phẩm";
                    worksheet.Cells[1, 5].Value = "Phương thức ship";
                    worksheet.Cells[1, 6].Value = "Giá";
                    worksheet.Cells[1, 7].Value = "Phẩm trăm giảm giá";
                    worksheet.Cells[1, 8].Value = "Giảm giá";
                    worksheet.Cells[1, 9].Value = "Số lượng";
                    worksheet.Cells[1, 10].Value = "Số lượng còn lại";
                    worksheet.Cells[1, 11].Value = "Danh mục sản phẩm";
                    worksheet.Cells[1, 12].Value = "Phân loại sản phẩm";
                    worksheet.Cells[1, 13].Value = "Bảo hành";
                    worksheet.Cells[1, 14].Value = "Chính sách đổi trả";
                    worksheet.Cells[1, 15].Value = "Thời gian bắt đầu giảm giá";
                    worksheet.Cells[1, 16].Value = "Thời gian kết thúc giảm giá";
                    worksheet.Cells[1, 17].Value = "Ngày cập nhật";
                    worksheet.Cells[1, 18].Value = "Người cập nhật";
                    // Đặt màu nền và màu chữ cho dòng 1
                    using (ExcelRange range = worksheet.Cells["A1:R1"])
                    {
                        range.Style.Font.Color.SetColor(Color.White); // Màu chữ là màu trắng
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Blue); // Màu nền là màu xanh
                        range.Style.Font.Bold = true; // Đặt đậm cho font chữ
                        range.Style.Font.Size = 13; // Đặt cỡ chữ
                    }

                    // Đổ dữ liệu từ danh sách vào Excel
                    int row = 2;
                    foreach (var Product in modelFilter.Products)
                    {
                        worksheet.Cells[row, 1].Value = Product.db.Name;
                        worksheet.Cells[row, 2].Value = Product.ShopName;
                        worksheet.Cells[row, 3].Value = Product.GroupProductName;
                        worksheet.Cells[row, 4].Value = Product.TypeProductName;
                        worksheet.Cells[row, 5].Value = Product.ShippingMethodName;
                        worksheet.Cells[row, 6].Value =
                          Product.db.StartingPrice == null ? "0" : ((decimal)Product.db.StartingPrice).ToString("#,##0")
                            + " - "
                            + Product.db.EndingPrice == null ? "0" : ((decimal)Product.db.EndingPrice).ToString("#,##0");
                        worksheet.Cells[row, 7].Value =
                            Product.db.DiscountPercentage == null ? "0" : ((int)Product.db.DiscountPercentage).ToString("#,##0");
                        worksheet.Cells[row, 8].Value =
                            Product.db.DiscountFrom == null ? "0" : ((decimal)Product.db.DiscountFrom).ToString("#,##0")
                            + " - " +
                            Product.db.DiscountUpTo == null ? "0" : ((decimal)Product.db.DiscountUpTo).ToString("#,##0");
                        worksheet.Cells[row, 9].Value =
                            Product.db.Quantity == null ? "0" : ((int)Product.db.Quantity).ToString("#,##0");
                        worksheet.Cells[row, 10].Value =
                            Product.db.RemainingQuantity == null ? "0" : ((int)Product.db.RemainingQuantity).ToString("#,##0");
                        worksheet.Cells[row, 11].Value = Product.ProductCategoryName;
                        worksheet.Cells[row, 12].Value = Product.ProductClassificationName;
                        worksheet.Cells[row, 13].Value = Product.WanrrantyName;
                        worksheet.Cells[row, 14].Value = Product.ReturnAndExchangePolicyName + " - " + ((int)Product.RefundFee).ToString("#,##0");
                        worksheet.Cells[row, 15].Value = Product.db.StartTime != null ? Product.db.StartTime.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 16].Value = Product.db.EndTime != null ? Product.db.EndTime.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 17].Value = Product.db.CreateDate != null ? Product.db.CreateDate.ToString() : DateTime.Now.ToString("MM/dd/yyyy");
                        worksheet.Cells[row, 18].Value = Product.UpdateByName;
                        row++;
                    }

                    using (ExcelRange range = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, 18])
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
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Product.xlsx");
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