using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Admin.Model
{
    public class ShoppingCartModel
    {
        public ShoppingCartModel()
        {
            db = new ShoppingCart();
            listShoppingCartDetail = new List<ShoppingCartDetailModel>();
        }
        public ShoppingCart db { get; set; }
        public string ShopName { get;set; }
        public string UserName   { get;set; }
        public string TotalView { get; set; }
        public string DiscountAmountView { get; set; }
        public string TotalProductPriceView { get; set; }
        public string TotalQantityView { get; set; }
        public string ShippingFeeView { get; set; }
        public string ProductName { get; set; }

        public string idProduct { get;set; }
        public string UpdateByName { get; set; }

        public List<ShoppingCartDetailModel> listShoppingCartDetail { get; set; }

    }




    public class ShoppingCartViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<ShoppingCartModel> ShoppingCarts { get; set; }
        //1: Get List, 2:Export
        public int? TypeAction { get; set; }
    }

    public class ShoppingCartDetailModel
    {
        public int? ID { get; set; }
        public string ProductName { get; set; }
        public string idProduct { get; set; }
        public string Image { get; set; }
        public decimal? UnitPrice { get; set; }
        public long? Quantity { get; set; }
        public decimal? Total { get; set; }
    }

}