using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Portal.Controllers
{
    public class HomeController : Controller
    {
        // GET: Portal/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}