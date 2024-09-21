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
        public ActionResult Index(Guid id)
        {
            var model = new ShoppingCartModel();
            model = map.getShoppingCart(id);
            return View(model);
        }

        public ActionResult CheckOut(Guid id)
        {
            var model = map.getCheckOut(id,1);
            return View(model);
        }


        public ActionResult Details(Guid id)
        {
            var model = map.getCheckOut(id,-1);
            return View(model);
        }


        [HttpPost]
        public ActionResult ApplyVoucher(string voucherCode, string id,string idShop)
        {

            var apply = map.applyVoucher(voucherCode, id, idShop);

            if (apply == 0)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ." });
            }
            else
            {
                return Json(new { success = true, message = "Mã giảm giá đã được áp dụng thành công." });
            }
        }

        [HttpPost]
        public ActionResult Update(CartUpdateModel model)
        {
            if(map.update(model) == 1)
            {
                return Json(new { success = true });

            }
            return Json(new { success = false });

        }


        [HttpPost]
        public ActionResult CheckOut(CheckOutModel model)
        {

            int check = CheckValidation(model);
            if (check == 1)
            {
                map.insertBill(model);
                return Redirect("/Portal/Home/Index");
            }
            model = map.getCheckOut(Guid.Parse(model.ID),1);
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteOrderShoppingCart(DeleteShoppingCartOrderModel model)
        {
           
             var apply = map.deleteOrderShoppingCart(model);
            if (apply == 0)
            {
                return Json(new { success = false, message = "Xóa sản phẩm thất bại." });
            }
            else
            {
                return Json(new { success = true, message = "Xóa sản phẩm thành công." });
            }
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