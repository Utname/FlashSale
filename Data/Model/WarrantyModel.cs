using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class WarrantyModel
    {
        public WarrantyModel()
        {
            db = new Warranty();
        }
        public Warranty db { get; set; }
        public string UpdateByName { get; set; }
        public string NameProductGroup { get; set; }

    }

    public class WarrantyViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }

        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<WarrantyModel> Warranty { get; set; }
    }
}