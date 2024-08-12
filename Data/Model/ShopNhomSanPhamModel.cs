using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class ShopNhomSanPhamModel
    {
        public ShopNhomSanPhamModel()
        {
            db = new ShopNhomSanPham();
        }
        public ShopNhomSanPham db { get; set; }
        public string TenNguoiCapNhat { get; set; }
        public string TenShop { get; set; }
        public string TenNhomSanPham { get; set; }

    }
}