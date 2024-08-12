using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Admin.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{

    public class mapSanPham : mapCommon
    {
        public List<SanPhamModel> getAllList(string search, int statusDel,int? idNhomSanPham)
        {
            var result = db.SanPhams.Where(q => q.StatusDel == statusDel)
                .Where(q => q.TenSanPham.ToLower().Contains(search) || String.IsNullOrEmpty(search))
                .Where(q=>q.idNhomSanPham == idNhomSanPham || idNhomSanPham == -1)
                .Select(q=> new SanPhamModel
                {
                    db = q,
                    TenNhomSanPham = db.NhomSanPhams.Where(d=>d.ID == q.idNhomSanPham).Select(d=>d.TenNhom).FirstOrDefault(),
                    TenNguoiCapNhat = db.TaiKhoanShops.Where(d=>d.ID == q.idShop).Select(d=>d.Username).FirstOrDefault()
                }).OrderByDescending(q => q.db.NgayCapNhat).ToList();
            return result;
        }



        public List<CommonModel> getListUse()
        {
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.TenNhom
            }).ToList();
            return result;
        }

        public void SaveImageInfo(ImageProduct image)
        {
            // Lưu thông tin hình ảnh vào cơ sở dữ liệu
            image.NguoiCapNhat =GetUserId();
            image.NgayCapNhat = DateTime.Now;
            db.ImageProducts.Add(image);
            db.SaveChanges();
        }

        public void UpdateImageInfo(List<ImageProduct> updatedImages)
        {
            var idProduct = updatedImages.Select(q => q.idProduct).FirstOrDefault();
            var listIdImage = updatedImages.Where(q=>q.ID != 0).Select(q=>q.ID);

            if(listIdImage.Count() > 0)
            {
                var listImageToRomove = db.ImageProducts.Where(q => !listIdImage.Contains(q.ID)).ToList();
                db.ImageProducts.RemoveRange(listImageToRomove);
                db.SaveChanges();
            }

            // Cập nhật thông tin hình ảnh vào cơ sở dữ liệu
            foreach (var image in updatedImages)
            {

                var existingImage = db.ImageProducts.Find(image.ID);
                if (existingImage != null)
                {
                    existingImage.FileName = image.FileName;
                    existingImage.FileExtension = image.FileExtension;
                    db.SaveChanges();
                }
                else
                {
                    SaveImageInfo(image);
                }
               
            }
        }

        public int insert(SanPhamModel model)
        {
            db.SanPhams.Add(model.db);
            db.SaveChanges();
            return 1;
        }


        public void insertExcel(SanPham model)
        {
            db.SanPhams.Add(model);
            db.SaveChanges();
        }

        public int edit(SanPhamModel model)
        {
            var item = db.SanPhams.Where(q => q.ID == model.db.ID).SingleOrDefault();
            if (item != null)
            {
                item.TenSanPham = model.db.TenSanPham;
                item.GiaNiemYet = model.db.GiaNiemYet;
                item.GiaSale = model.db.GiaSale;
                item.ConHang = model.db.ConHang;
                item.GioBatDau = model.db.GioBatDau;
                item.GioKetThuc = model.db.GioKetThuc;
                item.idLoaiSanPham = model.db.idLoaiSanPham;
                item.idLoaiSanPham = model.db.idLoaiSanPham;
                item.NguoiCapNhat = model.db.NguoiCapNhat;
                item.idNhomSanPham = model.db.idNhomSanPham;
                item.NgayCapNhat = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public SanPhamModel details(string id)
        {
            var result = db.SanPhams.Where(q => q.ID.ToString() == id).Select(q=> new SanPhamModel { 
                db = q,
                TenNhomSanPham = db.NhomSanPhams.Where(d => d.ID == q.idNhomSanPham).Select(d => d.TenNhom).FirstOrDefault(),
                TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.Username).FirstOrDefault(),
                
            }).SingleOrDefault();
            result.GiaNiemYetView = ((decimal)result.db.GiaNiemYet).ToString("#,##0");
            result.GiaSaleView = ((decimal)result.db.GiaSale).ToString("#,##0");
            return result;
        }

        public int updateStatusDel(string id, int statusDel)
        {
            var item = db.SanPhams.Where(q => q.ID.ToString() == id).SingleOrDefault();
            if (item != null)
            {
                item.StatusDel = statusDel;
                item.NgayCapNhat = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
