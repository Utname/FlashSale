using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class DonHangChiTietModel
    {
        public DonHangChiTietModel()
        {
            db = new DonHangChiTiet();
        }
        public DonHangChiTiet db { get; set; }
        public string TenDonHang { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public string GiaSaleView { get; set; }
        public string GiaBanView { get; set; }
        public string SoLuongView { get; set; }

    }
}