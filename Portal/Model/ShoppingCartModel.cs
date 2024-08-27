using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashSale.Areas.Portal.Model
{

    public class ShoppingCartModel
    {
        public string ID { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public long? TotalQuantity { get; set; }
        public List<ShoppingCartDetailModel> listShoppingCartDetail { get; set; }
    }



    public class CheckOutModel : ShoppingCartModel
    {
        public string idUser { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime OrderTime {get;set ;}
        public string Note { get; set; }
    }




    public class ShoppingCartDetailModel {
        public int? ID { get; set; }
        public string ProductName { get;set; }
        public string idProduct { get; set; }
        public string Image { get; set; }
        public decimal? UnitPrice { get;set; }
        public long? Quantity { get; set; }
        public decimal? Total { get; set; }
    }


    public class ShoppingCartOrderModel
    {
        public List<ShoppingCartPortalModel> ListShoppingCart { get; set; }
        public int? TotalItem { get; set; }
        public decimal? TotalShoppingCart { get; set; }

    }

    public class ShoppingCartPortalModel
    {
        public string ID { get; set; }
        public string ProductName { get; set; }
        public long? Quantity { get; set; }
        public string Image { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Total { get; set; }
    }
}
