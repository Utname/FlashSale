using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class ProductClassificationModel
    {
        public ProductClassificationModel()
        {
            db = new ProductClassification();
        }
        public ProductClassification db { get; set; }
        public string UpdateByName { get; set; }
        public string ProductCategoryName { get; set; }


    }
}