using Data;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Map
{
    public class mapTaiKhoanShop : mapCommon
    {
        public FlashSaleEntities db = new FlashSaleEntities();
        public string Insert(TaiKhoanShop model)
        {
            try
            {
                db.TaiKhoanShops.Add(model);
                db.SaveChanges();
                return "1";
            }
            catch { return "0"; }
        }

        public TaiKhoanShop Search(string email, string passowrd)
        {
            var account = db.TaiKhoanShops.Where(q => q.Email == email).Where(q => q.Password == passowrd).SingleOrDefault();
            return account;

        }

        public bool CheckSearchWithEmail(string email)
        {
            var checkAccount = db.TaiKhoanShops.Where(q => q.Email == email).Count() > 0;
            return checkAccount;
        }

        public TaiKhoanShop SearchWithEmail(string email)
        {
            var account = db.TaiKhoanShops.Where(q => q.Email == email).SingleOrDefault();
            return account;
        }

        public TaiKhoanShop SearchWithId(string id)
        {
            var account = db.TaiKhoanShops.Where(q => q.ID.ToString() == id).SingleOrDefault();
            return account;
        }

    }
}
