using FlashSale.Areas.Admin.Model;
using FlashSale.Areas.Portal.Model;
using Portal.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Portal.Controllers
{
    public class HomeController : Controller
    {
        mapHome map = new mapHome();
        // GET: Portal/Home
        public ActionResult Index()
        {
            var model = new HomeModel();
            model.listGroupProduct = map.getListGroupProduct();
            return View(model);
        }
    }
}