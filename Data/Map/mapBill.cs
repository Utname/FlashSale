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

    public class mapBill : mapCommon
    {

        public BillViewModel getAllList(BillViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var resultNew = new List<BillModel>();
            var result = db.Bills.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.UserName.ToLower().Contains(model.Search) || q.PhoneUser.ToLower().Contains(model.Search) 
                || q.PhoneShop.ToLower().Contains(model.Search) || q.ShopName.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search))
              .Select(q => new BillModel
              {
                  db = q,
                  UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.CreateBy).Select(d => d.Username).FirstOrDefault()
              }).OrderByDescending(q => q.db.UpdateDate);
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
                var BillDetails = db.ShoppingCartDetails.Where(d => d.idShoppingCart == q.db.idShopingCart).Where(d => d.StatusDel == 1).Select(d => d.idProudct).Distinct().ToList();
                var products = db.Products.Where(d => BillDetails.Contains(d.ID)).Where(d=>d.StatusDel == 1).Select(d=>d.Name).ToList();
                q.ProductName = String.Join(";", products);
            });
            model.CurrentPage = model.Page;
            model.Bills = resultNew;
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
      


        public BillModel details(string id)
        {
            var result = db.Bills.Where(q => q.ID.ToString() == id).Select(q => new BillModel
            {
                db = q,
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault()

            }).SingleOrDefault();
            result.TotalView = result.db.Total == null ? "0" : FormatDecimalViewString((decimal)result.db.Total);
            result.TotalQantityView = result.db.TotalQantity == null ? "0" : result.db.TotalQantity == null ? "0" : FormatDecimalViewString((decimal)result.db.TotalQantity);
            result.ShippingFeeView = result.db.ShippingFee == null ? "0" : FormatDecimalViewString((decimal)result.db.ShippingFee);
            result.listBillDetail = db.ShoppingCartDetails.Where(q => q.idShoppingCart == result.db.idShopingCart).Where(q => q.StatusDel == 1).Select(q => new BillDetailModel
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
            var item = db.Bills.Where(q => q.ID.ToString() == id).SingleOrDefault();
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
