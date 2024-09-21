﻿using FlashSale.Areas.Admin.Model;
using FlashSale.Areas.Portal.Model;
using Portal.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Portal.Controllers
{
    public class HomeController : BaseController
    {
        mapHome map = new mapHome();




        // GET: Portal/Home
        public ActionResult Index()
        {
            var model = new HomeModel();
            model.listBanner = map.getListBanner();
            model.listMenuGroupProduct = map.getListMenuPGroupProduct().Skip(0).Take(8).ToList();
            model.listMenuGroupProductMore = map.getListMenuPGroupProduct().Skip(9).ToList();
            model.listGroupProduct = map.getListGroupProduct();
            model.listHotSaleProduct= map.getListHotSaleProduct();
            model.listProductHome = map.getListProductHome();
            model.advertisementHot = map.getAdvertisementHot();
            model.listAdvertisementBanner = map.getAdvertisementBanner();
            return View(model);
        }
    }
}