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
    public class mapBill : mapCommon
    {
        public FlashSaleEntities db = new FlashSaleEntities();

        public List<BillModel> bills()
        {
            var userId = GetUserId();
            var bills = db.Bills.Where(q => q.TransactionStatus == 1).Where(q => q.StatusDel == 1).Where(q => q.idUser.ToString() == userId).Select(q => new BillModel
            {

                ID = q.ID,
                idShoppingCart = q.idShopingCart,
                idUser = (Guid)q.idUser,
                DeliveryDate = q.DeliveryDate,
                OrderDate = q.OrderDate,
                TransactionStatusName = q.TransactionStatus == 1 ? "Đang xử lý" : "Đang giao hàng",
                idShop = q.idShop,
                ShopName = q.ShopName,
                PhoneShop = q.PhoneShop,
                AddressShop = q.AddressShop,
                ShippingFee= q.ShippingFee??0,
                TotalQantity = q.TotalQantity,
                Total = q.Total,
            }).OrderByDescending(q=>q.DeliveryDate).ToList();
            return bills;
        }

    }
}
