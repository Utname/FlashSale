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


    public class NhomSanPhamViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<NhomSanPhamModel> NhomSanPham { get; set; }
        //1: Get List, 2:Export
        public int? TypeAction { get; set; }
    }
}