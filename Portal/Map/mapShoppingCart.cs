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
    public class mapShoppingCart : mapCommon
    {
        public FlashSaleEntities db = new FlashSaleEntities();
        public ShoppingCartModel getShoppingCart(Guid id)
        {
            var userId = GetUserId();
            var shoppingCart = db.ShoppingCarts.Where(q=>q.ID == id).Where(q => q.StatusOrder == 1).Where(q => q.StatusDel == 1).FirstOrDefault();
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
                    idProduct = q.idProudct.ToString()
                }).ToList();
                result.ID = shoppingCart.ID.ToString();
                result.SubTotal = shoppingCart.TotalProductPrice??0;
                result.DiscountAmount = shoppingCart.DiscountAmount ?? 0;
                result.VoucherCode = shoppingCart.VoucherCode;
                result.Total = shoppingCart.Total ?? 0;
                result.idShop = shoppingCart.IdShop.ToString();
                result.TotalQuantity = shoppingCart.TotalQantity ?? 0;
            }
            return result;
        }

        public CheckOutModel getCheckOut(Guid id,int StatusOrder)
        {
            var userId = GetUserId();
            var shoppingCart = db.ShoppingCarts.Where(q=>q.ID ==id).Where(q => q.StatusOrder == StatusOrder || StatusOrder == -1).Where(q => q.StatusDel == 1).FirstOrDefault();
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
                result.idShop = shoppingCart.IdShop;
                var shop = db.TaiKhoanShops.Where(q => q.ID == result.idShop).FirstOrDefault();
                result.PhoneShop = shop.SoDienThoai;
                result.ShopName = shop.TenShop;
                result.idShop = shop.ID;
                result.AddressShop = shop.DiaChi;
                result.ShippingFee = shoppingCart.ShippingFee;

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
                DeliveryAddress = model.Address,
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
                idShop = model.idShop,
                ShopName = model.ShopName,
                PhoneShop = model.PhoneShop,
                AddressShop = model.AddressShop,
                TotalQantity = model.TotalQuantity,
                Total = model.Total,
                ShippingFee = model.ShippingFee
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

        public int applyVoucher(string voucherCode, string id,string idShop)
        {
            var voucher = db.Vouchers.Where(q => q.Code == voucherCode).Where(q => q.StatusDel == 1).Where(q=>q.idShop.ToString() == idShop).FirstOrDefault();
            if(voucher == null)
            {
                return 0;
            }
            var shoppingCart = db.ShoppingCarts.Where(q => q.ID.ToString() == id).FirstOrDefault();
            var discountAmount = shoppingCart.Total * voucher.Value??0 / 100;
            shoppingCart.VoucherCode = voucherCode;
            shoppingCart.DiscountAmount = discountAmount;
            shoppingCart.Total = shoppingCart.Total - discountAmount;
            db.SaveChanges();
            return 1;

        }


        public int update(CartUpdateModel model){
            var details = new List<ShoppingCartDetail>();
            long quantityChange = 0;
            for(int i = 0;i< model.cartItems.Count; i++)
            {
                var item = model.cartItems[i];
                var detail = db.ShoppingCartDetails.Where(q => q.ID == item.id).FirstOrDefault();
                quantityChange += (long)(item.quantity - detail.Quantity);
                detail.Quantity = item.quantity;
                detail.Total = item.quantity * detail.UnitPrice;
                detail.UpdateDate = DateTime.Now;
                detail.UpdateBy = GetUserId();
                details.Add(detail);
                db.SaveChanges();
            }
            var cart = db.ShoppingCarts.Where(q => q.ID.ToString() == model.idShoppingCart).SingleOrDefault();
            cart.TotalQantity += quantityChange;
            cart.TotalProductPrice = details.Sum(q => q.Total);
            cart.Total = details.Sum(q => q.Total);
            if (!string.IsNullOrEmpty(cart.VoucherCode))
            {
                var voucher = db.Vouchers.Where(q => q.Code == cart.VoucherCode).Where(q=>q.StatusDel == 1).FirstOrDefault();
                cart.DiscountAmount = cart.Total * voucher.Value ?? 0 / 100;
                cart.Total = cart.Total - cart.DiscountAmount;
            }
            cart.UpdateBy = GetUserId();
            cart.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return 1;
        }

        public int deleteOrderShoppingCart(DeleteShoppingCartOrderModel model)
        {
            try
            {
                var checkDetails = db.ShoppingCartDetails.Where(q => q.idShoppingCart.ToString() == model.idShoppingCart).Where(q => q.StatusDel == 1).ToList();
                var details = db.ShoppingCartDetails.Where(q => q.ID == model.idShoppingCartDetails).SingleOrDefault();
                var cart = db.ShoppingCarts.Where(q => q.ID.ToString() == model.idShoppingCart).SingleOrDefault();

                if (checkDetails.Count() == 1)
                {
                    cart.StatusDel = 2;
                    cart.UpdateDate = DateTime.Now;
                    cart.UpdateBy = GetUserId();
                    db.SaveChanges();
                }
                else
                {
                    cart.TotalQantity -= details.Quantity;
                    cart.TotalProductPrice -= details.Total;
                    var discountAmount = cart.TotalProductPrice * 10 / 100;
                    cart.DiscountAmount = discountAmount;
                    cart.Total = cart.TotalProductPrice - cart.DiscountAmount;
                    cart.UpdateBy = GetUserId();
                    cart.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
                details.StatusDel = 2;
                details.UpdateDate = DateTime.Now;
                details.UpdateBy = GetUserId();
                db.SaveChanges();

                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
