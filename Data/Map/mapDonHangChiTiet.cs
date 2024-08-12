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

    public class mapDonHangChiTiet : mapCommon
    {
        public List<DonHangChiTietModel> getAllList(string search, int statusDel)
        {
            var result = db.DonHangChiTiets.Where(q => q.StatusDel == statusDel)
                .Where(q => q.TenSanPham.ToLower().Contains(search) || String.IsNullOrEmpty(search)
                ).Select(q=> new DonHangChiTietModel
                {
                    db = q,
                    TenNguoiCapNhat = db.TaiKhoanShops.Where(d=>d.ID.ToString() == q.NguoiCapNhat).Select(d=>d.Username).FirstOrDefault()
                }).OrderByDescending(q => q.db.NgayCapNhat).ToList();
            return result;
        }

        public int insert(DonHangChiTietModel model)
        {
            db.DonHangChiTiets.Add(model.db);
            db.SaveChanges();
            return 1;
        }

        public int edit(DonHangChiTietModel model)
        {
            var item = db.DonHangChiTiets.Where(q => q.ID == model.db.ID).SingleOrDefault();
            if (item != null)
            {
                item.idSanPham = model.db.idSanPham;
                item.TenSanPham = model.db.TenSanPham;
                item.GiaSale = model.db.GiaSale;
                item.GiaBan = model.db.GiaBan;
                item.GiaSale = model.db.GiaSale;
                item.SoLuong = model.db.SoLuong;
                item.NguoiCapNhat = model.db.NguoiCapNhat;
                item.NgayCapNhat = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public DonHangChiTietModel details(string id)
        {
            return db.DonHangChiTiets.Where(q => q.ID.ToString() == id).Select(q=> new DonHangChiTietModel { 
                db = q,
                TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.NguoiCapNhat).Select(d => d.Username).FirstOrDefault(),
                GiaBanView = q.GiaBan.ToString(),
                SoLuongView = q.SoLuong.ToString(),
                GiaSaleView = q.GiaSale.ToString(),
            }).SingleOrDefault();
        }

        public int updateStatusDel(string id, int statusDel)
        {
            var item = db.DonHangChiTiets.Where(q => q.ID.ToString() == id).SingleOrDefault();
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
