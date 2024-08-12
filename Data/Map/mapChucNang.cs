using Data.Entity;
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
        public List<ChucNang> getAllList(string search,int statusDel)
        {
            var result = db.ChucNangs.Where(q=>q.StatusDel == statusDel)
                .Where(q=>q.TenChucNang.ToLower().Contains(search) || q.MaChucNang.ToLower().Contains(search) || String.IsNullOrEmpty(search)
                ).ToList();
            return result;
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
