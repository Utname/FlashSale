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

    public class mapVoucher : mapCommon
    {
        public VoucherViewModel getAllList(VoucherViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.idShop = model.idShop ?? "-1";
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var result = db.Vouchers.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.idShop.ToString() == model.idShop || model.idShop.ToString() == "-1")
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Select(q => new VoucherModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).Take(model.PageSize).ToList();

            model.TotalCount = db.Vouchers.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.idShop.ToString() == model.idShop || model.idShop.ToString() == "-1")
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Count();
            var resultNew = result;
            model.CurrentPage = model.Page;
            model.Voucher = resultNew;
            return model;
        }


        public int insert(VoucherModel model)
        {
            var userId = GetUserId();
            model.db.Value = String.IsNullOrEmpty(model.ValueView) ? 0 : FormatIntView(model.ValueView);
            model.db.CreateDate = DateTime.Now;
            model.db.UpdateDate = DateTime.Now;
            model.db.idShop = model.db.idShop?? Guid.Parse(userId);
            model.db.StatusDel = 1;
            model.db.CreateBy = userId;
            model.db.UpdateBy = userId;
            db.Vouchers.Add(model.db);
            db.SaveChanges();
            return 1;
        }

        public List<CommonModel> getListUseFilter()
        {
            var list = new List<CommonModel>();
            var all = new CommonModel();
            all.id = -1;
            all.name = tatCa;
            list.Add(all);
            var result = db.Vouchers.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public List<CommonModel> getListUse()
        {
            var list = new List<CommonModel>();
            var result = db.Vouchers.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public List<CommonModel> getListUseByShop(string idShop)
        {
            var list = new List<CommonModel>();
            var result = db.Vouchers.Where(q=>q.idShop.ToString() == idShop).Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public int edit(VoucherModel model)
        {
            var item = db.Vouchers.Find(model.db.ID);
            model.db.Value = String.IsNullOrEmpty(model.ValueView) ? 0 : FormatIntView(model.ValueView);
            if (item != null)
            {
                item.Value =model.db.Value;
                item.Name = model.db.Name;
                item.idShop = model.db.idShop;
                item.Value = model.db.Value;
                item.Code = model.db.Code;
                item.StartTime = model.db.StartTime;
                item.EndTime = model.db.EndTime;
                item.Image = model.db.Image;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = model.db.UpdateBy;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public VoucherModel details(int id)
        {
            return db.Vouchers.Where(q => q.ID == id).Select(q => new VoucherModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                db = q,
            }).SingleOrDefault();
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.Vouchers.Find(id);
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
