using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class AdvertisementModel
    {
        public AdvertisementModel()
        {
            db = new Advertisement();
        }
        public Advertisement db { get; set; }
        public string UpdateByName { get; set; }
        public string GroupProductName { get; set; }
        public string TypeName { get; set; }

    }

    public class AdvertisementViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int? IdGroup { get; set; }
        public int? Type { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<AdvertisementModel> Advertisement { get; set; }
    }
}