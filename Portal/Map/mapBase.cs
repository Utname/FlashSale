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
            var shoppingCart = db.ShoppingCarts.Where(q => q.CreateBy.ToString() == userId).Where(q => q.StatusOrder == 1).Where(q => q.StatusDel == 1).ToList();
            var result = new ShoppingCartOrderModel();
            result.ListGroupShoppingCart = new List<ShoppingCartGroupModel>();
            shoppingCart.ForEach(cart =>
            {
                var itemCart  = db.ShoppingCarts.Where(q => q.StatusOrder == 1).Where(q=>q.ID == cart.ID).Where(q => q.StatusDel == 1).Where(q => q.CreateBy == userId).Select(q => new ShoppingCartGroupModel
                {
                    ID = q.ID.ToString(),
                    ShopName = db.TaiKhoanShops.Where(d => d.ID == q.IdShop).Select(d => d.TenShop).FirstOrDefault(),
                    ListShoppingCart = db.ShoppingCartDetails.Where(t => t.idShoppingCart ==  q.ID).Where(t => t.StatusDel == 1).Select(t => new ShoppingCartPortalModel
                    {
                        ID = t.ID.ToString(),
                        ProductName = t.ProductName,
                        Image = t.Image,
                        UnitPrice = t.UnitPrice,
                        Quantity = t.Quantity,
                        Total = t.Total,
                    }).ToList()
                }).ToList();
                result.ListGroupShoppingCart.AddRange(itemCart);
            });
            result.ListGroupShoppingCart.ForEach(q =>
            {
                q.TotalShoppingCart = q.ListShoppingCart.Sum(sum => sum.Total) ?? 0;
                q.TotalItem = q.ListShoppingCart.Sum(sum => sum.Quantity) ?? 0;
            });
          
            result.TotalItemGroup = result.ListGroupShoppingCart.Count();
            result.TotalShoppingCartGroup = result.ListGroupShoppingCart.Sum(q => q.TotalShoppingCart)??0;
            return result;
        }

       


        

    }

}
