using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class TaiKhoanShopModel
    {
        public TaiKhoanShopModel()
        {
            db = new TaiKhoanShop();
        }
        public TaiKhoanShop db { get; set; }
        public string TenNguoiCapNhat { get; set; }
    }

    public class PhanQuyenTaiKhoanModel
    {
        public PhanQuyenTaiKhoanModel(){
            ListChucNang = new List<ChucNang>();
            TaiKhoanShop = new TaiKhoanShopModel();
        }
        public TaiKhoanShopModel TaiKhoanShop { get; set; }
        public List<ChucNang> ListChucNang { get; set; }
        public bool CheckAll { get; set; }


    }

}