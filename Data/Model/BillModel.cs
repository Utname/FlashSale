using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Admin.Model
{
    public class BillModel
    {
        public BillModel()
        {
            db = new Bill();
            listBillDetail = new List<BillDetailModel>();
        }
        public Bill db { get; set; }
        public string TotalQantityView { get; set; }

        public string TotalView { get; set; }
        public string ShippingFeeView { get; set; }
        public string ProductName { get; set; }
        public string idProduct { get;set; }
        public string UpdateByName { get; set; }
        public string TransactionStatusName { get; set; }

        public List<BillDetailModel> listBillDetail { get; set; }

    }




    public class BillViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<BillModel> Bills { get; set; }
        //1: Get List, 2:Export
        public int? TypeAction { get; set; }
    }

    public class BillDetailModel
    {
        public int? ID { get; set; }
        public string ProductName { get; set; }
        public string idProduct { get; set; }
        public string Image { get; set; }
        public decimal? UnitPrice { get; set; }
        public long? Quantity { get; set; }
        public decimal? Total { get; set; }
    }

}