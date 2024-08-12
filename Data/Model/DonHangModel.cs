using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class DonHangModel
    {
        public DonHangModel()
        {
            db = new DonHang();
        }
        public DonHang db { get; set; }
        public string TenShop { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public string TongTienSanPhamView { get; set; }
        public string TrietKhauView { get; set; }
        public string PhiShipView { get; set; }
        public string VatView { get; set; }
        public string TongThanhToanView { get; set; }
    }
}