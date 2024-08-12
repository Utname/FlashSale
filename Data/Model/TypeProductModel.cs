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
}