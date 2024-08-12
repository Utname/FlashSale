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
    
    public class mapNhomSanPham : mapCommon
    {
        public List<NhomSanPhamModel> getAllList(string search,int statusDel)
        {
            var result = db.NhomSanPhams.Where(q=>q.StatusDel == statusDel)
                .Where(q=>q.TenNhom.ToLower().Contains(search) || search.Contains(q.idCapCha+"") || String.IsNullOrEmpty(search)
                ).Select(q=> new NhomSanPhamModel {

                    db = q,
                }).OrderByDescending(q => q.db.NgayCapNhat).ToList();
            return result;
        }

        public int insert(NhomSanPhamModel model)
        {
            db.NhomSanPhams.Add(model.db);
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
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.TenNhom
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public List<CommonModel> getListUse()
        {
            var list = new List<CommonModel>();
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.TenNhom
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public void insertExcel(NhomSanPham model)
        {
            db.NhomSanPhams.Add(model);
            db.SaveChanges();
        }

        public int edit(NhomSanPhamModel model)
        {
            var item = db.NhomSanPhams.Find(model.db.ID);
            if(item != null)
            {
                item.ThuTu = model.db.ThuTu;
                item.idCapCha = model.db.idCapCha;
                item.TenNhom = model.db.TenNhom;
                item.NgayCapNhat = DateTime.Now;    
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public NhomSanPhamModel details(int id)
        {
            return db.NhomSanPhams.Where(q=>q.ID == id).Select(q => new NhomSanPhamModel
            {

                db = q,
            }).SingleOrDefault();
        }

        public int updateStatusDel(int id,int statusDel)
        {
            var item = db.NhomSanPhams.Find(id);
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
