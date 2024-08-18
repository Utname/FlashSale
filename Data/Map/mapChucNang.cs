using Data.Entity;
using FlashSale.Areas.Admin.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    
    public class mapChucNang : mapCommon
    {
      
        public FunctionViewModel getAllList(FunctionViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" :  model.Search.ToLower();
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var result = db.ChucNangs.Where(q=>q.StatusDel == model.StatusDel)
                .Where(q=>q.TenChucNang.ToLower().Contains(model.Search) || q.MaChucNang.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).OrderByDescending(q => q.NgayCapNhat).Skip(skip).Take(model.PageSize).ToList();

            model.TotalCount = db.ChucNangs.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.TenChucNang.ToLower().Contains(model.Search) || q.MaChucNang.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Count();

            var resultNew = result;
            model.CurrentPage = model.Page;
            model.ChucNangs = resultNew;
            return model;
        }



        public int insert(ChucNang model)
        {
            db.ChucNangs.Add(model);
            db.SaveChanges();
            return 1;
        }
        public int edit(ChucNang model)
        {
            var item = db.ChucNangs.Find(model.MaChucNang);
            if(item != null)
            {
                item.TenChucNang = model.TenChucNang;
                item.NhomChucNang = model.NhomChucNang;
                item.NgayCapNhat = DateTime.Now;    
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public ChucNang details(string id)
        {
            return db.ChucNangs.Find(id);
        }

        public int updateStatusDel(string maChucNang,int statusDel)
        {
            var item = db.ChucNangs.Find(maChucNang);
            if (item != null)
            {
                item.StatusDel = statusDel;
                item.NgayCapNhat = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
