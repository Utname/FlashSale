using FlashSale.Areas.Admin.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Admin.Controllers
{
    public class ImagesController : Controller
    {
        private readonly string _imagePath = "~/Areas/Admin/Content/FileUpload/Images/";
        private static List<ImageViewModel> images = new List<ImageViewModel>(); // Đây là danh sách hình ảnh hiện tại, bạn nên thay bằng cơ sở dữ liệu thực tế

        // GET: Images
        public ActionResult Index()
        {
            var imageList = GetImageList();
            return View(imageList);
        }

        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath(_imagePath), fileName);
                    file.SaveAs(path);

                    // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                    SaveImageInfo(new ImageViewModel
                    {
                        FileName = fileName,
                        FilePath = _imagePath + fileName,
                        FileSize = file.ContentLength,
                        FileExtension = Path.GetExtension(fileName).TrimStart('.')
                    });
                }
            }

            return Json(new { success = true }); // Trả về JSON để thông báo thành công
        }

        [HttpPost]
        public ActionResult Save(List<ImageViewModel> updatedImages)
        {
            // Xóa các hình ảnh đã bị loại khỏi danh sách
            var imagesToDelete = images.Where(i => !updatedImages.Any(ui => ui.Id == i.Id)).ToList();
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
                var image = images.FirstOrDefault(i => i.Id == updatedImage.Id);
                if (image != null)
                {
                    image.FileName = updatedImage.FileName;
                    image.FileExtension = Path.GetExtension(updatedImage.FileName); // Cập nhật phần mở rộng
                }
            }

            // Cập nhật thông tin hình ảnh vào cơ sở dữ liệu
            UpdateImageInfo(updatedImages);

            return RedirectToAction("Index");
        }
        private void SaveImageInfo(ImageViewModel image)
        {
            // Lưu thông tin hình ảnh vào cơ sở dữ liệu
            // Ví dụ: _context.Images.Add(image); _context.SaveChanges();
        }

        private void UpdateImageInfo(List<ImageViewModel> updatedImages)
        {
            // Cập nhật thông tin hình ảnh vào cơ sở dữ liệu
            // Ví dụ:
            // foreach (var image in updatedImages)
            // {
            //     var existingImage = _context.Images.Find(image.Id);
            //     if (existingImage != null)
            //     {
            //         existingImage.FileName = image.FileName;
            //         existingImage.FileExtension = image.FileExtension;
            //         _context.SaveChanges();
            //     }
            // }
        }

        private IEnumerable<ImageViewModel> GetImageList()
        {
            // Lấy danh sách hình ảnh từ cơ sở dữ liệu hoặc thư mục
            // Ví dụ: return _context.Images.ToList();
            return images; // Thay bằng dữ liệu thực tế
        }
    }
}