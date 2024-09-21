﻿using Data.Entity;
using Data.Helpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Portal.Model
{
    public class ProductModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public double? AverageRating { get; set; }
        public string Description { get; set; }
        public string SendFrom { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        //Info Product
        public string TypeProductName { get; set; }
        public string GroupProductName { get; set; }
        public string ProductCategoryName { get; set; }
        //Price Product
        public decimal? StartingPrice { get; set; }
        public decimal? EndingPrice { get; set; }
        public decimal? DiscountFrom { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountUpTo { get; set; }
        //Info ship
        public string ShippingMethodName { get; set; }
        public decimal? ShippingFee { get; set; }
        //Info different
        public string WanrrantyName { get; set; }
        public string ReturnAndExchangePolicyName { get; set; }
        public decimal? ReturnAndExchangePolicyFee { get; set; }
        public decimal? RefundFee { get; set; }
        public string ShopName { get; set; }
        public List<string> ListImage { get; set; }
        public List<CommonModelImage> ListProductClassification { get; set; }
        public List<ProductHomeModel> ListProductCategory { get; set; }


    }



    public class ProductFilterModel
    {
        public int? idGroup { get; set; }
        public string idType { get; set; }
        public string Search { get; set; }
        public List<string> SelectedClassifications { get; set; }
        public int ShortBy { get; set; } = -1;
        public int PageNumber { get; set; } = 1; // New
        public int PageSize { get; set; } = 12;  // New
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<ProductHomeModel> Products { get; set; }
        public string Ads { get; set; }
    }
}