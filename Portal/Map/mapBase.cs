using Data;
using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Portal.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Map
{
    public class mapBase : mapCommon
    {
        public FlashSaleEntities db = new FlashSaleEntities();
        public ShoppingCartOrderModel getOrderShoppingCart()
        {
            var userId = GetUserId();
            var shoppingCart = db.ShoppingCarts.Where(q => q.IdShop.ToString() == userId).Where(q => q.StatusOrder == 1).Where(q => q.StatusDel == 1).FirstOrDefault();
            var result = new ShoppingCartOrderModel();
            result.ListShoppingCart = new List<ShoppingCartPortalModel>();
            if (shoppingCart != null)
            {
                result.ListShoppingCart = db.ShoppingCartDetails.Where(q => q.idShoppingCart == shoppingCart.ID).Where(q => q.StatusDel == 1).Select(q=> new ShoppingCartPortalModel
                 {
                     ID = q.ID.ToString(),
                     ProductName = q.ProductName,
                     Image = q.Image,
                     UnitPrice = q.UnitPrice,
                     Quantity = q.Quantity,
                     Total = q.Total,
                 }).ToList();
            }
            result.TotalItem = result.ListShoppingCart.Count();
            result.TotalShoppingCart = result.ListShoppingCart.Sum(q => q.Total) ??0;
            return result;
        }

    }

}
