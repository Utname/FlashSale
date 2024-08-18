using Data.Entity;
using FlashSale.Areas.Admin.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    
    public class mapShopNhomSanPham : mapCommon
    {
       


        public ShopNhomSanPhamViewModel getAllList(ShopNhomSanPhamViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.IdGroup = model.IdGroup ?? -1;
            model.IdShop = model.IdShop ?? "-1";

            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var result = db.ShopNhomSanPhams.Where(q => q.StatusDel == model.StatusDel)
             .Where(q => q.idShop.ToString() == model.IdShop || model.IdShop == "-1")
             .Where(q => q.idNhomSanPham == model.IdGroup || model.IdGroup == -1)
               .Select(q => new ShopNhomSanPhamModel
               {
                   db = q,
                   TenNhomSanPham = db.NhomSanPhams.Where(d => d.ID == q.idNhomSanPham).Select(d => d.TenNhom).FirstOrDefault(),
                   TenShop = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                   TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.NguoiCapNhat).Select(d => d.Username).FirstOrDefault()
               }).OrderByDescending(q => q.db.NgayCapNhat).Take(model.PageSize).ToList();

            model.TotalCount = db.ShopNhomSanPhams.Where(q => q.StatusDel == model.StatusDel)
             .Where(q => q.idShop.ToString() == model.IdShop || model.IdShop == "-1")
             .Where(q => q.idNhomSanPham == model.IdGroup || model.IdGroup == -1).Count();
            var resultNew = result;
            model.CurrentPage = model.Page;
            model.ShopNhomSanPham = resultNew;
            return model;
        }

        public int insert(ShopNhomSanPhamModel model)
        {
            db.ShopNhomSanPhams.Add(model.db);
            db.SaveChanges();
            return 1;
        }


        public void insertExcel(ShopNhomSanPham model)
        {
            db.ShopNhomSanPhams.Add(model);
            db.SaveChanges();
        }

        public int edit(ShopNhomSanPhamModel model)
        {
            var item = db.ShopNhomSanPhams.Where(q=>q.ID == model.db.ID).FirstOrDefault();
            if(item != null)
            {
              //  item.idShop = model.db.idShop;
             //   item.idNhomSanPham = model.db.idNhomSanPham;
                item.GhiChu = model.db.GhiChu;
                item.NgayCapNhat = DateTime.Now;
                item.NguoiCapNhat = model.db.NguoiCapNhat;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public ShopNhomSanPhamModel details(String id)
        {
            return db.ShopNhomSanPhams.Where(q=>q.ID.ToString() == id).Select(q => new ShopNhomSanPhamModel
            {
                db = q,
                TenNhomSanPham = db.NhomSanPhams.Where(d => d.ID == q.idNhomSanPham).Select(d => d.TenNhom).FirstOrDefault(),
                TenShop = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.NguoiCapNhat).Select(d => d.Username).FirstOrDefault()
            }).SingleOrDefault();
        }

        public int updateStatusDel(int id,int statusDel)
        {
            var item = db.ShopNhomSanPhams.Find(id);
            if (item != null)
            {
                item.StatusDel = statusDel;
                item.NgayCapNhat = DateTime.Now;
                item.NguoiCapNhat = GetUserId();
                db.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
