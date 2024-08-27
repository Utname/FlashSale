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

    public class mapDefaultImage : mapCommon
    {
        public DefaultImageViewModel getAllList(DefaultImageViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.Type = model.Type ?? -1;
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var result = db.DefaultImages.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.Type == model.Type || model.Type == -1)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Select(q => new DefaultImageModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                 

                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).Take(model.PageSize).ToList();
            result.ForEach(q =>
            {
                q.TypeName = Constants.defaultImageList.Where(d => d.id == q.db.Type).Select(d => d.name).FirstOrDefault();
            });
            model.TotalCount = db.DefaultImages.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.Type == model.Type || model.Type == -1)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Count();
            var resultNew = result;
            model.CurrentPage = model.Page;
            model.DefaultImage = resultNew;
            
            return model;
        }

        public int insert(DefaultImageModel model)
        {
          
            model.db.CreateDate = DateTime.Now;
            model.db.UpdateDate = DateTime.Now;
            model.db.StatusDel = 1;
            model.db.CreateBy =GetUserId();
            model.db.UpdateBy = GetUserId();
            db.DefaultImages.Add(model.db);
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
            var result = db.DefaultImages.Where(q => q.StatusDel == 1).Select(q => new CommonModel
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
            var result = db.DefaultImages.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Name
            }).ToList();
            list.AddRange(result);
            return list;
        }


    
        public int edit(DefaultImageModel model)
        {
            var item = db.DefaultImages.Find(model.db.ID);
            if (item != null)
            {
                item.Name = model.db.Name;
                item.Type = model.db.Type;
                item.Image = model.db.Image;
                item.CoverImage = model.db.CoverImage;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = model.db.UpdateBy;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public DefaultImageModel details(int id)
        {
             var result = db.DefaultImages.Where(q => q.ID == id).Select(q => new DefaultImageModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                TypeName = Constants.defaultImageList.Where(d => d.id == q.Type).Select(d => d.name).FirstOrDefault(),
                db = q,
            }).SingleOrDefault();
            result.TypeName = Constants.defaultImageList.Where(d => d.id == result.db.Type).Select(d => d.name).FirstOrDefault();
            return result;
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.DefaultImages.Find(id);
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
