using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FlashSale.App_Start
{
    public class AuthorizationCheck : AuthorizeAttribute
    {
        public string ChucNang { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //If don't have session, requierd login
            var User = CookieConfig.GetUserFromCookie();
            if (User == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                (new
                {
                    controller = "TaiKhoanShop",
                    action = "Login",
                    area ="Portal"
                }));
                return;
            }

            if (string.IsNullOrEmpty(ChucNang))
            {
                return;
            }
            //check Authorization
            var db = new FlashSaleEntities();
            var authorization = db.PhanQuyens.Count(m => m.idTaiKhoanShop == User.ID && m.MaChucNang == ChucNang);
            if(authorization == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
               (new
               {
                   controller = "TaiKhoanShop",
                   action = "BaoChuaPhanQuyen",
                   area = "Admin"
               }));
                return;
            }

        }
    }
}