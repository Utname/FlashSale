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
        public ProductClassificationViewModel getAllList(ProductClassificationViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.IdGroup = model.IdGroup ?? -1;
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var result = db.ProductClassifications.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.idProductCategory == model.IdGroup || model.IdGroup == -1)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Select(q => new ProductClassificationModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Select(d => d.Name).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).Take(model.PageSize).ToList();

            model.TotalCount = db.ProductClassifications.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.idProductCategory == model.IdGroup || model.IdGroup == -1)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Count();
            var resultNew = result;
            model.CurrentPage = model.Page;
            model.ProductClassification = resultNew;
            return model;
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

        public List<CommonModel> getListUseByGroup(string idGroup)
        {
            var list = new List<CommonModel>();
            var result = db.ProductClassifications.Where(q=>q.idProductCategory.ToString() ==idGroup).Where(q => q.StatusDel == 1).Select(q => new CommonModel
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
