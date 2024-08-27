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
        public List<HotSaleProductModel> listHotSaleProduct { get; set; }
        public List<GroupProductHomeModel> listProductHome { get; set; }
        public AdvertisementModel advertisementHot { get; set; }
        public List<AdvertisementModel> listAdvertisementBanner { get; set; }

    }

    public class MenuGroupProductModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<CommonModel> listTypeProduct { get; set; }

    }

    public class AdvertisementModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Image { get; set; }
    }

    public class BannerHomeModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

    }


    public class GroupProductHomeModel {
        public int? idGroup { get; set; }
        public string GroupName { get; set; }

        public List<TypeProductHomeModel> listTypeProdctHome { get; set; }
        public List<ProductHomeModel> listProductHome { get; set; }
        public List<AdvertisementModel> listAdvertisement { get; set; }


    }

    public class TypeProductHomeModel {
        public int? idType { get; set; }
        public string TypeName { get; set; }
        public DateTime? UpdateDate { get; set; }


    }

    public class ProductHomeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShopName { get; set; }
        public string Image { get; set; }
        public double? AverageRating { get; set; }
        public int? DiscountPercentage { get; set; }
        public decimal? StartingPrice { get; set; }
        public decimal? EndingPrice { get; set; }
        public decimal? DiscountFrom { get; set; }
        public decimal? DiscountUpTo { get; set; }
        public DateTime? UpdateDate { get; set; }


    }

    public class HotSaleProductModel : ProductHomeModel
    {
        public string TypeProductName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

}