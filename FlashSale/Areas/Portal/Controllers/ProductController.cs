using Data.Helpers.Model;
using FlashSale.Areas.Portal.Model;
using Microsoft.Office.Interop.Excel;
using Portal.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashSale.Areas.Portal.Controllers
{
  
    public class ProductController : BaseController
    {
        mapProduct map = new mapProduct();
        // GET: Portal/Product
        public ActionResult Details(string id)
        {
            var model = new ProductModel();
            model = map.details(id);
            return View(model);
        }

        public ActionResult Index(ProductFilterModel model)
        {
            model.Products = map.index(model);
            return View(model);
        }


      

        //[HttpPost]
        //public ActionResult AddToCart(string idProduct, int quantity)
        //{
        //    map.upsetShoppingCart(idProduct,quantity);
        //    return RedirectToAction("Details", "Product", new { id = idProduct });
        //}

        [HttpPost]
        public JsonResult AddToCart(string idProduct, int quantity)
        {
            try
            {
                // Assuming map.upsetShoppingCart is the logic to add the product to the shopping cart
                map.upsetShoppingCart(idProduct, quantity);

                // Return JSON result with success status and redirect URL
                return Json(new { success = true, redirectUrl = Url.Action("Details", "Product", new { id = idProduct }) });
            }
            catch (Exception ex)
            {
                // Handle the exception and return JSON result with failure status and error message
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}