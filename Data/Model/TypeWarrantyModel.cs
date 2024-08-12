using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class TypeProductModel
    {
        public TypeProductModel()
        {
            db = new TypeProduct();
        }
        public TypeProduct db { get; set; }
        public string UpdateByName { get; set; }
        public string NameProductGroup { get; set; }

    }
}