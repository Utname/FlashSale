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

    public class mapProductClassification : mapCommon
    {
        public List<ProductClassificationModel> getAllList(string search, int statusDel, int? idGroup)
        {
            var result = db.ProductClassifications.Where(q => q.StatusDel == statusDel)
                .Where(q => q.idProductCategory == idGroup || idGroup == -1)
                .Where(q => q.Name.ToLower().Contains(search) || String.IsNullOrEmpty(search)
                ).Select(q => new ProductClassificationModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Select(d => d.Name).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).ToList();
            return result;
        }



        public int insert(ProductClassificationModel model)
        {
            db.ProductClassifications.Add(model.db);
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
            var result = db.ProductClassifications.Where(q => q.StatusDel == 1).Select(q => new CommonModel
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
            var result = db.ProductClassifications.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public int edit(ProductClassificationModel model)
        {
            var item = db.ProductClassifications.Find(model.db.ID);
            if (item != null)
            {
                item.Name = model.db.Name;
                item.idProductCategory = model.db.idProductCategory;
                item.Image = model.db.Image;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = model.db.UpdateBy;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public ProductClassificationModel details(int id)
        {
            return db.ProductClassifications.Where(q => q.ID == id).Select(q => new ProductClassificationModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Select(d => d.Name).FirstOrDefault(),
                db = q,
            }).SingleOrDefault();
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.ProductClassifications.Find(id);
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
