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

    public class mapWarranty : mapCommon
    {
        public List<WarrantyModel> getAllList(string search, int statusDel)
        {
            var result = db.Warranties.Where(q => q.StatusDel == statusDel)
                .Where(q => q.Name.ToLower().Contains(search) || String.IsNullOrEmpty(search)
                ).Select(q => new WarrantyModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).ToList();
            return result;
        }


        public WarrantyViewModel getAllList(WarrantyViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;

            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var result = db.Warranties.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Select(q => new WarrantyModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).Take(model.PageSize).ToList();

            model.TotalCount = db.Warranties.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Count();
            var resultNew = result;
            model.CurrentPage = model.Page;
            model.Warranty = resultNew;
            return model;
        }


        public int insert(WarrantyModel model)
        {
            db.Warranties.Add(model.db);
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
            var result = db.Warranties.Where(q => q.StatusDel == 1).Select(q => new CommonModel
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
            var result = db.Warranties.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

       

        public int edit(WarrantyModel model)
        {
            var item = db.Warranties.Find(model.db.ID);
            if (item != null)
            {
                item.Name = model.db.Name;
                item.Note = model.db.Note;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = model.db.UpdateBy;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public WarrantyModel details(int id)
        {
            return db.Warranties.Where(q => q.ID == id).Select(q => new WarrantyModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                db = q,
            }).SingleOrDefault();
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.Warranties.Find(id);
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
