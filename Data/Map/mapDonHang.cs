using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Admin.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{

    public class mapDonHang : mapCommon
    {
        public List<DonHangModel> getAllList(string search, int statusDel, string idShop)
        {
            var result = db.DonHangs.Where(q => q.StatusDel == statusDel)
                .Where(q => q.SoDienThoai.ToLower().Contains(search) || q.DiaChiGiaoHang.ToLower().Contains(search) || String.IsNullOrEmpty(search))
                .Where(q => q.idShop.ToString() == idShop || idShop == "-1")
                .Select(q => new DonHangModel
                {
                    db = q,
                    TenShop = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                    TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.NguoiCapNhat).Select(d => d.Username).FirstOrDefault()
                }).OrderByDescending(q => q.db.NgayCapNhat).ToList();
            return result;
        }



        public int insert(DonHangModel model)
        {
            db.DonHangs.Add(model.db);
            db.SaveChanges();
            return 1;
        }




        public int edit(DonHangModel model)
        {
            var item = db.DonHangs.Where(q => q.ID == model.db.ID).SingleOrDefault();
            if (item != null)
            {
                item.idShop = model.db.idShop;
                item.SoDienThoai = model.db.SoDienThoai;
                item.DiaChiGiaoHang = model.db.DiaChiGiaoHang;
                item.ThoiGianDatHang = model.db.ThoiGianDatHang;
                item.TrangThai = model.db.TrangThai;
                item.TongTienSanPham = model.db.TongTienSanPham;
                item.TrietKhau = model.db.TrietKhau;
                item.PhiShip = model.db.PhiShip;
                item.NguoiCapNhat = model.db.NguoiCapNhat;
                item.Vat = model.db.Vat;
                item.TongThanhToan = model.db.TongThanhToan;
                item.KhacHangGhiChu = model.db.KhacHangGhiChu;
                item.NgayCapNhat = DateTime.Now;
                item.idVat = model.db.idVat;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public DonHangModel details(string id)
        {
            var result = db.DonHangs.Where(q => q.ID.ToString() == id).Select(q => new DonHangModel
            {
                db = q,
                TenShop = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.NguoiCapNhat).Select(d => d.Username).FirstOrDefault(),

            }).SingleOrDefault();
            result.TongTienSanPhamView = ((decimal)result.db.TongTienSanPham).ToString("#,##0");
            result.TrietKhauView = ((decimal)result.db.TrietKhau).ToString("#,##0");
            result.PhiShipView = ((decimal)result.db.PhiShip).ToString("#,##0");
            result.VatView = ((decimal)result.db.Vat).ToString("#,##0");
            result.TongThanhToanView = ((decimal)result.db.TongThanhToan).ToString("#,##0");
            return result;
        }

        public int updateStatusDel(string id, int statusDel)
        {
            var item = db.DonHangs.Where(q => q.ID.ToString() == id).SingleOrDefault();
            if (item != null)
            {
                item.StatusDel = statusDel;
                item.NgayCapNhat = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public List<CommonModelRef> getListUse()
        {
            var result = db.TaiKhoanShops.Where(q => q.StatusDel == 1).Select(q => new CommonModelRef
            {
                id = q.ID.ToString(),
                name = q.TenShop
            }).ToList();
            return result;
        }
    }
}
