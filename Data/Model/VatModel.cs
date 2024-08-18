using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class VatModel
    {
        public VatModel()
        {
            db = new Vat();
        }
        public Vat db { get; set; }
        public string UpdateByName { get; set; }
     
    }

    public class VatViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }

        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<VatModel> Vat { get; set; }
    }
}