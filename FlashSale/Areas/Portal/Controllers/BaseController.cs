using FlashSale.Areas.Portal.Model;
using Portal.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Portal.Controllers
{
    public abstract class BaseController : Controller
    {
        mapBase map = new mapBase();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        
            ViewBag.ShoppingCart =map.getOrderShoppingCart();
            base.OnActionExecuting(filterContext);
        }
    }
}