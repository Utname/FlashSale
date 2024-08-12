using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.App_Start
{
    public static class SessionConfig
    {
        public static void SetUser(TaiKhoanShop user)
        {
            HttpContext.Current.Session.Remove("user");
            HttpContext.Current.Session["user"] = user;
            HttpContext.Current.Session.Timeout = 14400;
        }

        public static TaiKhoanShop GetUser()
        {
            return (TaiKhoanShop)HttpContext.Current.Session["user"];
        }
    }
}