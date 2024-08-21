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
using System.Web.Mvc;

namespace Data
{

    public class mapDonHang : mapCommon
    {
       
        public DonHangViewModel getAllList(DonHangViewModel model)
        {
            model.StatusDel = model.StatusDel ??1;
            model.IdShop = model.IdShop ?? "-1";
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var resultNew = new List<DonHangModel>();
            var result = db.DonHangs.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.SoDienThoai.ToLower().Contains(model.Search) || q.DiaChiGiaoHang.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search))
                .Where(q => q.idShop.ToString() == model.IdShop || model.IdShop == "-1")
                .Select(q => new DonHangModel
                {
                    db = q,
                    TenShop = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                    TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.NguoiCapNhat).Select(d => d.Username).FirstOrDefault()
                }).OrderByDescending(q => q.db.NgayCapNhat);

            if(model.TypeAction == 1)
            {
                if (model.TimeOrderTo != null && model.TimeOrderCome == null)
                {
                    resultNew = result.Where(q => q.db.ThoiGianDatHang >= DateTime.Parse(model.TimeOrderTo)).Skip(skip).Take(model.PageSize).ToList();
                }
                else if (model.TimeOrderTo == null && model.TimeOrderCome != null)
                {
                    resultNew = result.Where(q => q.db.ThoiGianDatHang <= DateTime.Parse(model.TimeOrderCome)).Skip(skip).Take(model.PageSize).ToList();
                }
                else if (model.TimeOrderTo != null && model.TimeOrderCome != null)
                {
                    resultNew = result.Where(q => q.db.ThoiGianDatHang >= DateTime.Parse(model.TimeOrderTo) && q.db.ThoiGianDatHang <= DateTime.Parse(model.TimeOrderCome)).Skip(skip).Take(model.PageSize).ToList();
                }
                else
                {
                    resultNew = result.Skip(skip).Take(model.PageSize).ToList();
                }
            }
            else
            {
                if (model.TimeOrderTo != null && model.TimeOrderCome == null)
                {
                    resultNew = result.Where(q => q.db.ThoiGianDatHang >= DateTime.Parse(model.TimeOrderTo)).ToList();
                }
                else if (model.TimeOrderTo == null && model.TimeOrderCome != null)
                {
                    resultNew = result.Where(q => q.db.ThoiGianDatHang <= DateTime.Parse(model.TimeOrderCome)).ToList();
                }
                else if (model.TimeOrderTo != null && model.TimeOrderCome != null)
                {
                    resultNew = result.Where(q => q.db.ThoiGianDatHang >= DateTime.Parse(model.TimeOrderTo) && q.db.ThoiGianDatHang <= DateTime.Parse(model.TimeOrderCome)).ToList();
                }
                else
                {
                    resultNew = result.ToList();
                }
            }
            model.TotalCount = db.DonHangs.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.SoDienThoai.ToLower().Contains(model.Search) || q.DiaChiGiaoHang.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search))
                .Where(q => q.idShop.ToString() == model.IdShop || model.IdShop == "-1")
                .Select(q => new DonHangModel
                {
                    db = q,
                    TenShop = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                    TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.NguoiCapNhat).Select(d => d.Username).FirstOrDefault()
                }).Count();


            if (model.TimeOrderTo != null) {
                var timeOrderTo = DateTime.Parse(model.TimeOrderTo);
                model.TimeOrderTo = timeOrderTo.ToString("yyyy-MM-ddTHH:mm");

            }
            if (model.TimeOrderCome != null)
            {
                var timeOrderCome = DateTime.Parse(model.TimeOrderCome);
                model.TimeOrderCome = timeOrderCome.ToString("yyyy-MM-ddTHH:mm");
            }

            model.CurrentPage = model.Page;
            model.DonHang = resultNew;
            return model;
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
            result.TongTienSanPhamView = FormatDecimalViewString((decimal)result.db.TongTienSanPham);
            result.TrietKhauView = FormatDecimalViewString((decimal)result.db.TrietKhau);
            result.PhiShipView = FormatDecimalViewString((decimal)result.db.PhiShip);
            result.VatView = FormatDecimalViewString((decimal)result.db.Vat);
            result.TongThanhToanView = FormatDecimalViewString((decimal)result.db.TongThanhToan);
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
