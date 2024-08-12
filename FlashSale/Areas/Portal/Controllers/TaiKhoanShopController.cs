using Data;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Portal.Map;
using Data.Helpers.Common;
using FlashSale.App_Start;
using Microsoft.Office.Interop.Excel;

namespace FlashSale.Areas.Portal.Controllers
{
    public class TaiKhoanShopController : Controller
    {
        mapTaiKhoanShop map = new mapTaiKhoanShop();
        //GET: Portal/TaiKhoanShop
        public ActionResult Login()
        {
            //var userCurrent = CookieConfig.GetUserFromCookie();
            //if(userCurrent != null)
            //{
            //    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            //}
            return View(new TaiKhoanShop() { });
        }

        [HttpPost]
        public ActionResult Login(TaiKhoanShop model)
        {
            var check = CheckValidationLogin(model);
            if(check == 1)
            {
                var user = map.SearchWithEmail(model.Email);
                CookieConfig.StoreUserInCookie(user);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
               
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View(new TaiKhoanShop() {});
        }

        [HttpPost]
        public ActionResult Register(TaiKhoanShop model)
        {
            int check = CheckValidationRegister(model);
            if (check == 1)
            {
                model.ID = Guid.NewGuid();
                model.TenShop = model.Username;
                model.NgayCapNhat = DateTime.Now;
                model.NgayTao = DateTime.Now;
                model.StatusDel = 1;
                model.Password = map.encrypt(model.Password);
                map.Insert(model);
               
                return RedirectToAction("Login", "TaiKhoanShop", new { area = "Portal" });
            }
            return View(model);
        }

        int CheckValidationRegister(TaiKhoanShop model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                ModelState.AddModelError("Username", Data.Helpers.Common.Constants.required);
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError("Email", Data.Helpers.Common.Constants.required);
            }
            else
            {
                if(map.IsValidEmail(model.Email) == false)
                {
                    ModelState.AddModelError("Email", Data.Helpers.Common.Constants.emailNotCorrect);
                }
                else
                {
                    var checkUser = map.CheckSearchWithEmail(model.Email);
                    if(checkUser)
                    {
                        ModelState.AddModelError("Email", Data.Helpers.Common.Constants.EmailIsValid);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", Data.Helpers.Common.Constants.required);
            }
            else
            {
                if (map.IsValidPassword(model.Password) == false)
                {
                    ModelState.AddModelError("Password", Data.Helpers.Common.Constants.passworNotCorrect);
                }
            }

            return ModelState.IsValid ? 1 : 0;
        }

        int CheckValidationLogin(TaiKhoanShop model)
        {
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

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", Data.Helpers.Common.Constants.required);
            }
            else
            {
                if (map.IsValidPassword(model.Password) == false)
                {
                    ModelState.AddModelError("Password", Data.Helpers.Common.Constants.passworNotCorrect);
                }
              

            }

            if(map.IsValidEmail(model.Email) == true && map.IsValidPassword(model.Password) == true)
            {
                var user = map.SearchWithEmail(model.Email);
                if(user == null)
                {
                    ModelState.AddModelError("Email", Data.Helpers.Common.Constants.emailNotCorrect);
                }
                else
                {
                    var password = map.encrypt(model.Password);
                    if(user.Password != password)
                    {
                        ModelState.AddModelError("Password", Data.Helpers.Common.Constants.passwordWrong);
                    }
                }
            }

            return ModelState.IsValid ? 1 : 0;
        }
    }
}