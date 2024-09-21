using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class VoucherModel
    {
        public VoucherModel()
        {
            db = new Voucher();
        }
        public Voucher db { get; set; }
        public string UpdateByName { get; set; }
        public string ShopName { get; set; }
        public string ValueView { get; set; }

    }

    public class VoucherViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string idShop { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<VoucherModel> Voucher { get; set; }
    }
}