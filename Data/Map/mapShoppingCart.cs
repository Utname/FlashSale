using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Admin.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{

    public class mapShoppingCart : mapCommon
    {

        public ShoppingCartViewModel getAllList(ShoppingCartViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var resultNew = new List<ShoppingCartModel>();
            var  query = db.ShoppingCarts.Where(q => q.StatusDel == model.StatusDel)
              .Select(q => new ShoppingCartModel
              {
                  db = q,
                  UserName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.CreateBy).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                  ShopName = db.TaiKhoanShops.Where(d => d.ID == q.IdShop).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                  UpdateByName = db.TaiKhoanShops.Where(d => d.ID == q.IdShop).Select(d => d.Username).FirstOrDefault()
              }).OrderByDescending(q => q.db.UpdateDate);
            var result = query.Where(q => q.UserName.ToLower().Contains(model.Search) || q.UserName.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search));
           
            model.TotalCount = result.Count();
            
            if (model.TypeAction == 1)
            {
                var data = result.Skip(skip).Take(model.PageSize).ToList();
                resultNew = data;
            }
            else
            {
                var data = result.ToList();
                resultNew = data;
            }
            resultNew.ForEach(q =>
            {
                var shoppingCartDetails = db.ShoppingCartDetails.Where(d => d.idShoppingCart == q.db.ID).Where(d => d.StatusDel == 1).Select(d => d.idProudct).Distinct().ToList();
                var products = db.Products.Where(d => shoppingCartDetails.Contains(d.ID)).Where(d=>d.StatusDel == 1).Select(d=>d.Name).ToList();
                q.ProductName = String.Join(";", products);
            });

            model.CurrentPage = model.Page;
            model.ShoppingCarts = resultNew;
            return model;
        }

        public List<CommonModel> getListUse()
        {
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.TenNhom
            }).ToList();
            return result;
        }
      

        public int editExcel(ShoppingCartModel model)
        {
            var item = db.ShoppingCarts.Where(q => q.ID == model.db.ID).SingleOrDefault();
            if (item != null)
            {
                item.IdShop = model.db.IdShop;
                item.DiscountCode = model.db.DiscountCode;
                item.Total = model.db.Total;
                item.DiscountAmount = model.db.DiscountAmount;
                item.TotalProductPrice = model.db.TotalProductPrice;
                item.TotalQantity = model.db.TotalQantity;
                item.StatusOrder = model.db.StatusOrder;
                item.StatusDel = model.db.StatusDel;
                item.ShippingFee = model.db.ShippingFee;
                item.VoucherCode = model.db.VoucherCode;
                item.UpdateBy = GetUserId();
                item.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public ShoppingCartModel details(string id)
        {
            var result = db.ShoppingCarts.Where(q => q.ID.ToString() == id).Select(q => new ShoppingCartModel
            {
                db = q,
                UserName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.CreateBy).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                ShopName = db.TaiKhoanShops.Where(d => d.ID == q.IdShop).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID == q.IdShop).Select(d => d.Username).FirstOrDefault()

            }).SingleOrDefault();
            result.TotalView = result.db.Total == null ? "0" : FormatDecimalViewString((decimal)result.db.Total);
            result.DiscountAmountView = result.db.DiscountAmount == null ? "0" : FormatDecimalViewString((decimal)result.db.DiscountAmount);
            result.TotalProductPriceView = result.db.TotalProductPrice == null ? "0" : FormatDecimalViewString((decimal)result.db.TotalProductPrice);
            result.TotalQantityView = result.db.TotalQantity == null ? "0" : result.db.TotalQantity == null ? "0" : FormatDecimalViewString((decimal)result.db.TotalQantity);
            result.ShippingFeeView = result.db.ShippingFee == null ? "0" : FormatDecimalViewString((decimal)result.db.ShippingFee);
            result.listShoppingCartDetail = db.ShoppingCartDetails.Where(q => q.idShoppingCart == result.db.ID).Where(q => q.StatusDel == 1).Select(q => new ShoppingCartDetailModel
            {
                ID = q.ID,
                ProductName = q.ProductName,
                Image = q.Image,
                UnitPrice = q.UnitPrice ?? 0,
                Quantity = q.Quantity ?? 0,
                Total = q.Total ?? 0,
            }).ToList();
            //result.RemainingQuantityView = ((decimal)result.db.RemainingQuantity).ToString("#,##0");
            return result;
        }

        public int updateStatusDel(string id, int statusDel)
        {
            var item = db.ShoppingCarts.Where(q => q.ID.ToString() == id).SingleOrDefault();
            if (item != null)
            {
                item.StatusDel = statusDel;
                item.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
