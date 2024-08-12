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

    public class mapTypeProduct : mapCommon
    {
        public List<TypeProductModel> getAllList(string search, int statusDel,int? idNhomSanPham)
        {
            var result = db.TypeProducts.Where(q => q.StatusDel == statusDel)
                .Where(q => q.Code.ToLower().Contains(search) || q.Name.ToLower().Contains(search) || String.IsNullOrEmpty(search)
                ).Where(q=>q.idProductGroup == idNhomSanPham || idNhomSanPham == -1).Select(q => new TypeProductModel
                {
                    UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                    NameProductGroup = db.NhomSanPhams.Where(d => d.ID == q.idProductGroup).Select(d => d.TenNhom).FirstOrDefault(),
                    db = q,
                }).OrderByDescending(q => q.db.UpdateDate).ToList();
            return result;
        }



        public int insert(TypeProductModel model)
        {
            db.TypeProducts.Add(model.db);
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
            var result = db.TypeProducts.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.Code
            }).ToList();
            list.AddRange(result);
            return list;
        }

        public List<CommonModelRef> getListUse()
        {
            var list = new List<CommonModelRef>();
            var result = db.TypeProducts.Where(q => q.StatusDel == 1).Select(q => new CommonModelRef
            {
                id = q.Name,
                name = q.Code
            }).ToList();
            list.AddRange(result);
            return list;
        }

       

        public int edit(TypeProductModel model)
        {
            var item = db.TypeProducts.Find(model.db.ID);
            if (item != null)
            {
                item.Code = model.db.Code;
                item.Name = model.db.Name;
                item.idProductGroup = model.db.idProductGroup;
                item.Image = model.db.Image;
                item.UpdateDate = DateTime.Now;
                item.UpdateBy = model.db.UpdateBy;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public TypeProductModel details(int id)
        {
            return db.TypeProducts.Where(q => q.ID == id).Select(q => new TypeProductModel
            {
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID.ToString() == q.UpdateBy).Select(d => d.Username).FirstOrDefault(),
                NameProductGroup = db.NhomSanPhams.Where(d => d.ID == q.idProductGroup).Select(d => d.TenNhom).FirstOrDefault(),
                db = q,
            }).SingleOrDefault();
        }

        public int updateStatusDel(int id, int statusDel)
        {
            var item = db.TypeProducts.Find(id);
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
