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

    public class mapProductCategory : mapCommon
    {
        public List<ProductCategoryModel> getAllList(string search, int statusDel)
        {
            var result = db.ProductCategories.Where(q => q.StatusDel == statusDel)
                .Where(q => q.Name.ToLower().Contains(search) || String.IsNullOrEmpty(search)
                ).Select(q => new ProductCategoryModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).ToList();
            return result;
        }



        public int insert(ProductCategoryModel model)
        {
            db.ProductCategories.Add(model.db);
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
            var result = db.ProductCategories.Where(q => q.StatusDel == 1).Select(q => new CommonModel
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
            var result = db.ProductCategories.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public int edit(ProductCategoryModel model)
        {
            var item = db.ProductCategories.Find(model.db.ID);
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

        public ProductCategoryModel details(int id)
        {
            return db.ProductCategories.Where(q => q.ID == id).Select(q => new ProductCategoryModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                db = q,
            }).SingleOrDefault();
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.ProductCategories.Find(id);
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
