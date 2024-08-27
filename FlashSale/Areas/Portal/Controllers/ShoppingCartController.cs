using FlashSale.Areas.Admin.Model;
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
    public class ShoppingCartController : BaseController
    {
        mapShoppingCart map = new mapShoppingCart();
        // GET: Portal/ShoppingCart
        public ActionResult Index()
        {
            var model = new ShoppingCartModel();
            model = map.getShoppingCart();
            return View(model);
        }

        public ActionResult CheckOut()
        {
            var model = map.getCheckOut();
            return View(model);
        }

        [HttpPost]
        public ActionResult CheckOut(CheckOutModel model)
        {

            int check = CheckValidation(model);
            if (check == 1)
            {
                map.insertBill(model);
                return Redirect("Portal/Home/Index");
            }
            model = map.getCheckOut();
            return View(model);
        }

        int CheckValidation(CheckOutModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                ModelState.AddModelError("UserName", Data.Helpers.Common.Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError("Email", Data.Helpers.Common.Constants.required);
            }

            else
            {
                if (map.IsValidEmail(model.Email) == false)
                {
                    ModelState.AddModelError("Email", Data.Helpers.Common.Constants.emailNotCorrect);
                }

            }
            if (string.IsNullOrWhiteSpace(model.Address))
            {
                ModelState.AddModelError("Address", Data.Helpers.Common.Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError("Phone", Data.Helpers.Common.Constants.required);
            }
            else
            {
                if (!Regex.IsMatch(model.Phone, map.phonePattern))
                {
                    ModelState.AddModelError("Phone", Data.Helpers.Common.Constants.phoneNotCorrect);
                }
            }

            return ModelState.IsValid ? 1 : 0;
        }
    }
}