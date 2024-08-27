using Data.Entity;
using Data.Helpers.Common;
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

    public class mapAdvertisement : mapCommon
    {
        public AdvertisementViewModel getAllList(AdvertisementViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Type = model.Type ?? -1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.IdGroup = model.IdGroup ?? -1;
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var result = db.Advertisements.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.idGroupProduct == model.IdGroup || model.IdGroup == -1)
                .Where(q => q.Type == model.Type || model.Type == -1)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Select(q => new AdvertisementModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    GroupProductName = db.NhomSanPhams.Where(d => d.ID == q.idGroupProduct).Select(d => d.TenNhom).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).Take(model.PageSize).ToList();
            result.ForEach(q =>
            {
                q.TypeName = Constants.advertisementList.Where(d => d.id == q.db.Type).Select(d => d.name).FirstOrDefault();
            });
            model.TotalCount = db.Advertisements.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.idGroupProduct == model.IdGroup || model.IdGroup == -1)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Count();
            var resultNew = result;
            model.CurrentPage = model.Page;
            model.Advertisement = resultNew;
            return model;
        }


        public int insert(AdvertisementModel model)
        {
            db.Advertisements.Add(model.db);
            db.SaveChanges();
            return 1;
        }

        public List<CommonModel> getListTypeFilter()
        {
            var list = new List<CommonModel>();
            var all = new CommonModel();
            all.id = -1;
            all.name = tatCa;
            list.Add(all);
            var result = Constants.advertisementList;
            list.AddRange(result);
            return list;
        }

      

        public List<CommonModel> getListType()
        {
            var result = Constants.advertisementList;
            return result;
        }

        public int edit(AdvertisementModel model)
        {
            var item = db.Advertisements.Find(model.db.ID);
            if (item != null)
            {
                item.Name = model.db.Name;
                item.idGroupProduct = model.db.idGroupProduct;
                item.Image = model.db.Image;
                item.Description = model.db.Description;
                item.Type = model.db.Type;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = model.db.UpdateBy;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public AdvertisementModel details(int id)
        {
            var result = db.Advertisements.Where(q => q.ID == id).Select(q => new AdvertisementModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                GroupProductName = db.NhomSanPhams.Where(d => d.ID == q.idGroupProduct).Select(d => d.TenNhom).FirstOrDefault(),
                db = q,
            }).SingleOrDefault();
            result.TypeName = Constants.advertisementList.Where(d => d.id == result.db.Type).Select(d => d.name).FirstOrDefault();
            return result;
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.Advertisements.Find(id);
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
