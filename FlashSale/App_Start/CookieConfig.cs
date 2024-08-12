using Data.Entity;
using Portal.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.App_Start
{
    public  class CookieConfig
    {
        static mapTaiKhoanShop map = new mapTaiKhoanShop();
        public static void StoreUserInCookie(TaiKhoanShop taiKhoan)
        {
            // Tạo một cookie mới
            HttpCookie authCookie = new HttpCookie("TaiKhoan");
            // Thiết lập các giá trị cho cookie
            authCookie.Values["IdTaiKhoan"] = taiKhoan.ID.ToString();
            // Thiết lập thời gian sống cho cookie (ví dụ: 30 ngày)
            authCookie.Expires = DateTime.Now.AddDays(30);
            // Thêm cookie vào phản hồi
            HttpContext.Current.Response.Cookies.Add(authCookie);

        }

        public static TaiKhoanShop GetUserFromCookie()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["TaiKhoan"];
            if(authCookie != null)
            {
                string idTaiKhoan = authCookie.Values["IdTaiKhoan"].ToString();
                TaiKhoanShop taiKhoan = map.SearchWithId(idTaiKhoan);
                return taiKhoan;
            }
            return null;
        }

      
    }
}