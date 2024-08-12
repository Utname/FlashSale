using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class NhomSanPhamModel
    {
        public NhomSanPhamModel()
        {
            db = new NhomSanPham();
        }
        public NhomSanPham db { get; set; }
        public string TenNguoiCapNhat { get; set; }
     
    }
}