using FlashSale.Areas.Portal.Model;
using Portal.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Portal.Controllers
{
    public class BillController : BaseController
    {
        mapBill map = new mapBill();
        // GET: Portal/Bill
        public ActionResult Index()
        {
            var model = new List<BillModel>();
            model = map.bills();
            return View(model);
        }
    }
}