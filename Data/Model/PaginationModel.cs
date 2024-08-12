using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class PaginationModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageRange { get; set; }
        public string Search { get; set; }
        public string StatusDel { get; set; }
        public string IdShop { get; set; }
        public DateTime? TimeOrderTo { get; set; }
        public DateTime? TimeOrderCome { get; set; }
        public int? IdNhomSanPham { get; set; }
        public int? idGroup { get; set; }
        public int? Type { get; set; }
    }
}
