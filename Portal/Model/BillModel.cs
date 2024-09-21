using Data.Entity;
using Data.Helpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Portal.Model
{
    public class BillModel
    {
        
        public Guid? ID { get; set; }
        public Guid? idShoppingCart { get; set; }

        public Guid? idUser { get; set; }
        public Guid? idShop { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public DateTime? OrderDate { get;set; }
        public string TransactionStatusName { get; set; }
        public string ShopName { get; set; }
        public string PhoneShop { get; set; }
        public string AddressShop { get; set; }

        public long? TotalQantity { get; set; }
        public decimal? Total { get; set; }
        public decimal? ShippingFee { get; set; }


    }

}