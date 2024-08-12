using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public decimal? RefundFee { get; set; }

        public string ShopName { get; set; }
        public string UpdateByName { get; set; }

    }
}