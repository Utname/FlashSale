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

    public class DonHangChiTietViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<DonHangChiTietModel> DonHangChiTiet { get; set; }
    }
}