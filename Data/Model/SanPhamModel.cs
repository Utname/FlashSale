using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class SanPhamModel
    {
        public SanPhamModel()
        {
            db = new SanPham();
        }
        public SanPham db { get; set; }
        public string TenNhomSanPham { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public string GiaSaleView { get; set; }
        public string GiaNiemYetView { get; set; }
    }
}