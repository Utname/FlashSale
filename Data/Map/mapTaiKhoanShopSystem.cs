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

    public class mapTaiKhoanShopSystem : mapCommon
    {
        mapCommon map = new mapCommon();
        public TaiKhoanShopViewModel getAllList(TaiKhoanShopViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var resultNew = new List<TaiKhoanShopModel>();
            var result = db.TaiKhoanShops.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.Email.ToLower().Contains(model.Search) || q.Username.ToLower().Contains(model.Search) || q.TenShop.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Select(q => new TaiKhoanShopModel
                {
                    db = q,
                    TenNguoiCapNhat = db.TaiKhoanShops.Where(d => d.ID == q.ID).Select(d => d.Username).FirstOrDefault()
                }).OrderByDescending(q => q.db.NgayCapNhat);
            if(model.TypeAction == 1)
            {
                var data = result.Skip(skip).Take(model.PageSize).ToList();
                resultNew = data;
            }
            else
            {
                var data = result.ToList();
                resultNew = data;
            }


            model.TotalCount = db.TaiKhoanShops.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.Email.ToLower().Contains(model.Search) || q.Username.ToLower().Contains(model.Search) || q.TenShop.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search)
                ).Count();
            model.CurrentPage = model.Page;
            model.TaiKhoanShop = resultNew;
            return model;
        }


        public int insert(TaiKhoanShopModel model)
        {
            db.TaiKhoanShops.Add(model.db);
            db.SaveChanges();
            return 1;
        }
        public List<CommonModelRef> getListUseFilter()
        {
            var list = new List<CommonModelRef>();
            var all = new CommonModelRef();
            all.id = "-1";
            all.name = tatCa;
            list.Add(all);
            var result = db.TaiKhoanShops.Where(q => q.StatusDel == 1).Select(q => new CommonModelRef
            {
                id = q.ID.ToString(),
                name = q.TenShop
            }).ToList();
            list.AddRange(result);
            return list;
        }
        public List<CommonModelRef> getListUse()
        {
            var list = new List<CommonModelRef>();
            var result = db.TaiKhoanShops.Where(q => q.StatusDel == 1).Select(q => new CommonModelRef
            {
                id = q.ID.ToString(),
                name = q.TenShop
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public void insertExcel(TaiKhoanShop model)
        {
            db.TaiKhoanShops.Add(model);
            db.SaveChanges();
        }

        public int edit(TaiKhoanShopModel model)
        {
            var item = db.TaiKhoanShops.Where(q => q.ID == model.db.ID).SingleOrDefault();
            if (item != null)
            {
                item.TenShop = model.db.TenShop;
                item.DiaChi = model.db.DiaChi;
                item.SoDienThoai = model.db.SoDienThoai;
                item.Email = model.db.Email;
                item.Facebook = model.db.Facebook;
                item.Username = model.db.Username;
                item.AnhDaiDien = model.db.AnhDaiDien;
                item.AnhBia = model.db.AnhBia;
                item.NgayCapNhat = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public TaiKhoanShopModel details(string id)
        {
            var taiKhoan = db.TaiKhoanShops.Where(q=>q.ID.ToString() == id).Select(q => new TaiKhoanShopModel
            {
                db = q,
            }).SingleOrDefault();
            taiKhoan.db.AnhBia = taiKhoan.db.AnhBia ?? map.anhMacDinh;
            taiKhoan.db.AnhDaiDien = taiKhoan.db.AnhDaiDien ?? map.anhMacDinh;

            return taiKhoan;
        }

        public int updateStatusDel(string id, int statusDel)
        {
            var item = db.TaiKhoanShops.Where(q => q.ID.ToString() == id).SingleOrDefault();
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
