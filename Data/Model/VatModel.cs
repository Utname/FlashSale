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
}