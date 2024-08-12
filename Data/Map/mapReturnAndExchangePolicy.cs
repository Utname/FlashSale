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

    public class mapReturnAndExchangePolicy : mapCommon
    {
        public List<ReturnAndExchangePolicyModel> getAllList(string search, int statusDel,int? Type)
        {
            var result = db.ReturnAndExchangePolicies.Where(q => q.StatusDel == statusDel)
                .Where(q => q.Name.ToLower().Contains(search) || q.Name.ToLower().Contains(search) || String.IsNullOrEmpty(search)
                ).Where(q=>q.Type == Type || Type == -1).Select(q => new ReturnAndExchangePolicyModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    TypePolicyName = q.Type == 1 ? "Đổi trả miễn phí" : "Đổi trả có phí",
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).ToList();
            return result;
        }



        public int insert(ReturnAndExchangePolicyModel model)
        {
            db.ReturnAndExchangePolicies.Add(model.db);
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
            var result = db.ReturnAndExchangePolicies.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public List<CommonModelRef> getListUse()
        {
            var list = new List<CommonModelRef>();
            var result = db.ReturnAndExchangePolicies.Where(q => q.StatusDel == 1).Select(q => new CommonModelRef
            {
                id = q.Name,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public List<CommonModel> getListTypeReturnAndChangePolicy()
        {
            var result = new List<CommonModel>()
            {
                new CommonModel(){id = 1,name="Đổi trả miễn phí"},
                new CommonModel(){id = 2,name="Đổi trả có phí"}
            };
            return result;
        }

        public List<CommonModel> getListTypeReturnAndChangePolicyFilter()
        {
            var result = new List<CommonModel>()
            {
                new CommonModel(){id = -1,name="Tất cả"},
                new CommonModel(){id = 1,name="Đổi trả miễn phí"},
                new CommonModel(){id = 2,name="Đổi trả có phí"}
            };
            return result;
        }



        public int edit(ReturnAndExchangePolicyModel model)
        {
            var item = db.ReturnAndExchangePolicies.Find(model.db.ID);
            if (item != null)
            {
                item.Type = model.db.Type;
                item.Name = model.db.Name;
                item.RefundFee = model.db.RefundFee;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = model.db.UpdateBy;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public ReturnAndExchangePolicyModel details(int id)
        {
            var result =  db.ReturnAndExchangePolicies.Where(q => q.ID == id).Select(q => new ReturnAndExchangePolicyModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
               
                db = q,
            }).SingleOrDefault();
            result.RefundFeeView = ((decimal)result.db.RefundFee).ToString("#,##0");
            return result;
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.ReturnAndExchangePolicies.Find(id);
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
