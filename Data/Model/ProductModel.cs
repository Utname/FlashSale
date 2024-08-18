using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Admin.Model
{
    public class ProductModel
    {
        public ProductModel()
        {
            db = new Product();
        }
        public Product db { get; set; }

        //Info Product
        public string TypeProductName { get; set; }
        public string GroupProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductClassificationName { get; set; }

        //Price Product
        public string StartingPriceView { get; set; }
        public string EndingPriceView { get; set; }
        public string DiscountFromView { get; set; }
        public string DiscountPercentageView { get; set; }
        public string DiscountUpToView { get; set; }


        //Info Quantity
        public string QuantityView { get; set; }
        public string RemainingQuantityView { get; set; }


        //Info ship
        public string ShippingMethodName { get; set; }
        public string ShippingFeeView { get; set; }


        //Info different
        public string PolicyName { get; set; }
        public string WanrrantyName { get; set; }
        public string ReturnAndExchangePolicyName { get; set; }
        public decimal? ReturnAndExchangePolicyFee { get; set; }
        public decimal? RefundFee { get; set; }

        public string ShopName { get; set; }
        public string UpdateByName { get; set; }
        public string selectedClassification { get; set; }


     

    }
    public class ProductViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int? IdGroup { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
        //1: Get List, 2:Export
        public int? TypeAction { get; set; }
    }
}