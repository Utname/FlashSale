using Data;
using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Admin.Model;
using FlashSale.Areas.Portal.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Map
{
    public class mapShoppingCart : mapCommon
    {
        public FlashSaleEntities db = new FlashSaleEntities();
        public ShoppingCartModel getShoppingCart()
        {
            var userId = GetUserId();
            var shoppingCart = db.ShoppingCarts.Where(q => q.IdShop.ToString() == userId).Where(q => q.StatusOrder == 1).Where(q => q.StatusDel == 1).FirstOrDefault();
            var result = new ShoppingCartModel();
            if (shoppingCart != null)
            {
                result.listShoppingCartDetail = db.ShoppingCartDetails.Where(q => q.idShoppingCart == shoppingCart.ID).Where(q => q.StatusDel == 1).Select(q => new ShoppingCartDetailModel
                {
                    ID = q.ID,
                    ProductName = q.ProductName,
                    Image = q.Image,
                    UnitPrice = q.UnitPrice??0,
                    Quantity = q.Quantity??0,
                    Total = q.Total??0,
                }).ToList();
                result.ID = shoppingCart.ID.ToString();
                result.SubTotal = shoppingCart.TotalProductPrice??0;
                result.Total = shoppingCart.Total ?? 0;
                result.TotalQuantity = shoppingCart.TotalQantity ?? 0;
            }
            return result;
        }

        public CheckOutModel getCheckOut()
        {
            var userId = GetUserId();
            var shoppingCart = db.ShoppingCarts.Where(q => q.IdShop.ToString() == userId).Where(q => q.StatusOrder == 1).Where(q => q.StatusDel == 1).FirstOrDefault();
            var result = new CheckOutModel();
            if (shoppingCart != null)
            {
                result.listShoppingCartDetail = db.ShoppingCartDetails.Where(q => q.idShoppingCart == shoppingCart.ID).Where(q => q.StatusDel == 1).Select(q => new ShoppingCartDetailModel
                {
                    ID = q.ID,
                    ProductName = q.ProductName,
                    Image = q.Image,
                    UnitPrice = q.UnitPrice ?? 0,
                    Quantity = q.Quantity ?? 0,
                    Total = q.Total ?? 0,
                }).ToList();
                result.ID = shoppingCart.ID.ToString();
                result.SubTotal = shoppingCart.TotalProductPrice ?? 0;
                result.Total = shoppingCart.Total ?? 0;
                result.TotalQuantity = shoppingCart.TotalQantity ?? 0;
                result.idUser = userId;
                var account = db.TaiKhoanShops.Where(q=>q.ID.ToString() == userId).FirstOrDefault();
                result.UserName = account.Username;
                result.Phone = account.SoDienThoai;
                result.Email = account.Email;
                result.Address = account.DiaChi;
                result.OrderTime = DateTime.Now;
            }
            return result;
        }

        public int insertBill(CheckOutModel model)
        {

            var bill = new Bill()
            {
                ID = Guid.NewGuid(),
                idUser = Guid.Parse(GetUserId()),
                UserName = model.UserName,
                PhoneUser = model.Phone,
                EmailUser = model.Email,
                DeliveryAdress = model.Address,
                DeliveryDate = model.OrderTime,
                OrderDate = model.OrderTime,
                Note = model.Note,
                TransactionStatus = 1,
                idShopingCart = Guid.Parse(model.ID),
                StatusDel = 1,
                UpdateDate = DateTime.Now,
                CreateBy = GetUserId(),
                CreateDate = DateTime.Now,
                UpdateBy = GetUserId(),
            };
            db.Bills.Add(bill);
            db.SaveChanges();
            updateShopingCart(model.ID);
            return 1;
        }

        public int updateShopingCart(string idShoppingCart)
        {
            var shoppingCart = db.ShoppingCarts.Where(q => q.ID.ToString() == idShoppingCart).SingleOrDefault();
            shoppingCart.StatusOrder = 2;
            shoppingCart.UpdateDate = DateTime.Now;
            shoppingCart.UpdateBy = GetUserId();
            db.SaveChanges();
            return 1;
        }

    }
}
