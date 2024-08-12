using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class ProductCategoryModel
    {
        public ProductCategoryModel()
        {
            db = new ProductCategory();
        }
        public ProductCategory db { get; set; }
        public string UpdateByName { get; set; }
     
    }
}