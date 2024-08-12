using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class ImageProductModel
    {
        public ImageProductModel()
        {
            images = new List< ImageProduct>();
        }
        public  List<ImageProduct> images { get; set; }
        public string idProduct { get; set; }
        public string NameProduct  { get; set; }

    }
}