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

    public class DonHangViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public string IdShop { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string TimeOrderTo { get; set; }
        public string TimeOrderCome { get; set; }
        //1: Get List, 2:Export
        public int? TypeAction { get; set; }

        public IEnumerable<DonHangModel> DonHang { get; set; }
    }

   
}