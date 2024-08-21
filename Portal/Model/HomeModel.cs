using Data.Entity;
using Data.Helpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Portal.Model
{
    public class HomeModel
    {
        public List<BannerHomeModel> listBanner { get; set; }
        public List<CommonModelImage> listGroupProduct { get; set; }
        public List<MenuGroupProductModel> listMenuGroupProduct { get; set; }
        public List<MenuGroupProductModel> listMenuGroupProductMore { get; set; }


    }

    public class MenuGroupProductModel 
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<CommonModel> listTypeProduct { get; set; }
    
    }

    public class BannerHomeModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

    }

}