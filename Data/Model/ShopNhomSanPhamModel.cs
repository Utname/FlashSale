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

    public class ShopNhomSanPhamViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public string IdShop { get; set; }
        public int? StatusDel { get; set; }
        public int? IdGroup { get; set; }

        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<ShopNhomSanPhamModel> ShopNhomSanPham { get; set; }
    }
}