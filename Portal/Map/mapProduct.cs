using Data;
using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Portal.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Map
{
    public class mapProduct : mapCommon
    {
        public FlashSaleEntities db = new FlashSaleEntities();

        public ProductModel details(string id)
        {
            var result = db.Products.Where(q => q.ID.ToString() == id).Select(q => new ProductModel
            {
                ID = q.ID.ToString(),
                Name = q.Name,

                AverageRating = q.AverageRating ?? 0,
                Description = q.Description,
                StartTime = q.StartTime,
                EndTime = q.EndTime,
                SendFrom = q.SendFrom,
                GroupProductName = db.NhomSanPhams.Where(d => d.ID == q.idGroup).Where(d => d.StatusDel == 1).Select(d => d.TenNhom).FirstOrDefault(),
                TypeProductName = db.TypeProducts.Where(d => d.ID == q.idType).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ShippingMethodName = q.ShippingMethod == 1 ? "Miễn phí" : "Có phí",
                ReturnAndExchangePolicyName = db.ReturnAndExchangePolicies.Where(d => d.ID == q.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ReturnAndExchangePolicyFee = db.ReturnAndExchangePolicies.Where(d => d.ID == q.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).Select(d => d.RefundFee).FirstOrDefault() ?? 0,
                WanrrantyName = db.Warranties.Where(d => d.ID == q.idWanrranty).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                StartingPrice = q.StartingPrice,
                EndingPrice = q.EndingPrice,
                DiscountFrom = q.DiscountFrom,
                DiscountUpTo = q.DiscountUpTo,
                ShippingFee = q.ShippingFee,
                DiscountPercentage = q.DiscountPercentage,
                ListImage = db.ImageProducts.Where(d => d.idProduct == q.ID).Select(d => d.FilePath).ToList(),
                ListProductClassification = db.ProductClassifications.Where(d => q.idProductClassification.Contains(d.ID.ToString())).Select(d => new CommonModelImage
                {
                    id = d.ID,
                    name = d.Name,
                    image = d.Image
                }).ToList(),
                ListProductCategory = db.Products.Where(product => product.StatusDel == 1).Where(product => product.idProductCategory == q.idProductCategory).Select(product => new ProductHomeModel
                {
                    Id = product.ID.ToString(),
                    Name = product.Name,
                    AverageRating = product.AverageRating ?? 3,
                    ShopName = db.TaiKhoanShops.Where(d => d.StatusDel == 1).Where(d => d.ID == product.idShop).Select(d => d.TenShop).FirstOrDefault(),
                    DiscountPercentage = product.DiscountPercentage ?? 0,
                    StartingPrice = product.StartingPrice ?? 0,
                    EndingPrice = product.EndingPrice ?? 0,
                    DiscountFrom = product.DiscountFrom ?? 0,
                    DiscountUpTo = product.DiscountUpTo ?? 0,
                    Image = db.ImageProducts.Where(d => d.idProduct == product.ID).Select(d => d.FilePath).FirstOrDefault() ?? "~/Areas/Admin/Content/FileUpload/Images/2024-7-7/design_patten.jpg",
                    UpdateDate = product.UpdateDate
                }).OrderByDescending(product => product.UpdateDate).ToList()
            }).SingleOrDefault();
            return result;
        }

        public List<ProductHomeModel> index(ProductFilterModel model)
        {
            model.idGroup = model.idGroup ?? -1;
            model.idType = model.idType ?? "-1";
            model.ShortBy = model.ShortBy;
            model.Ads = db.Advertisements.Where(d => d.Type == 2).Where(d => d.StatusDel == 1).OrderByDescending(d => d.CreateDate).Select(d => d.Image).FirstOrDefault();

            var data = db.Products.Where(q => q.idGroup == model.idGroup || model.idGroup == -1)
                        .Where(q => model.idType.Contains(q.idType + "") || model.idType == "-1")
                        .Where(q => q.Name.Contains(model.Search) || string.IsNullOrEmpty(model.Search))
                        .Where(q => q.StatusDel == 1)
                        .Select(q => new ProductHomeModel
                        {
                            Id = q.ID.ToString(),
                            Name = q.Name,
                            StartingPrice = q.StartingPrice,
                            EndingPrice = q.EndingPrice ?? 0,
                            DiscountFrom = q.DiscountFrom ?? 0,
                            DiscountUpTo = q.DiscountUpTo ?? 0,
                            DiscountPercentage = q.DiscountPercentage ?? 0,
                            UpdateDate = q.UpdateDate,
                            AverageRating = q.AverageRating ?? 0,
                            ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                            Image = db.ImageProducts.Where(d => d.idProduct == q.ID).Select(d => d.FilePath).FirstOrDefault(),
                            idProductClassification = q.idProductClassification
                        });
            if(model.SelectedClassifications != null)
            {
                data = data.Where(q => model.SelectedClassifications.Contains(q.idProductClassification));
            }
            // Apply sorting based on ShortBy
            switch (model.ShortBy)
            {
                case 1:
                    data = data.OrderBy(d => d.Name);
                    break;
                case 2:
                    data = data.OrderByDescending(d => d.Name);
                    break;
                case 3:
                    data = data.OrderBy(d => d.StartingPrice);
                    break;
                case 4:
                    data = data.OrderByDescending(d => d.StartingPrice);
                    break;
                case 5:
                    data = data.OrderBy(d => d.AverageRating);
                    break;
                default:
                    data = data.OrderByDescending(d => d.AverageRating);
                    break;
            }
            model.TotalItems = data.Count();
            // Apply pagination
            return data.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
        }

        public List<MenuCategoryProductModel> getListMenuCategoryProduct()
        {
            var result = db.ProductCategories.Where(q => q.StatusDel == 1).GroupBy(p => new { p.ID, p.Name }).Select(q => new MenuCategoryProductModel
            {
                Id = q.Key.ID,
                Name = q.Key.Name,
                listClassficationProduct = db.ProductClassifications.Where(d => d.StatusDel == 1).Where(d => d.idProductCategory == q.Key.ID).Select(d => new CommonModel
                {
                    id = d.ID,
                    name = d.Name
                }).OrderByDescending(d => d.id).ToList()
            }).OrderByDescending(q => q.Id).ToList();
            return result;
        }

        public async Task<string> upsetShoppingCart(string idProduct, int quantity)
        {
            var userId = GetUserId();

            var product = db.Products.Where(q => q.ID.ToString() == idProduct).SingleOrDefault();
            var shoppingCart = db.ShoppingCarts.Where(q => q.StatusDel == 1).Where(q => q.StatusOrder == 1)
                .Where(q => q.CreateBy.ToString() == userId)
                .Where(q=>q.IdShop == product.idShop).FirstOrDefault();

            if (shoppingCart == null)
            {
                await insertShoppingCart(product, quantity);
            }
            else
            {
                await updateShoppingCart(shoppingCart, product, quantity);
            }
            return "";
        }


        public async Task<string> updateShoppingCart(ShoppingCart shoppingCart, Product product, int quantity)
        {
            shoppingCart.UpdateDate = DateTime.Now;
            shoppingCart.UpdateBy = GetUserId();
            var checkProduct = db.ShoppingCartDetails.Where(q => q.idProudct == product.ID).Where(q => q.StatusDel == 1).Count() > 0;
            if (!checkProduct)
            {
                shoppingCart.ShippingFee += product.ShippingFee ?? 0;
            }
            var total = (product.StartingPrice - product.DiscountFrom) * quantity;
            shoppingCart.Total +=  total;
            shoppingCart.TotalQantity += quantity;
            shoppingCart.TotalProductPrice += total;
            db.SaveChanges();
            await updateShoppingCartDetail(shoppingCart, product, quantity);
            return "";
        }

        public async Task<string> updateShoppingCartDetail(ShoppingCart shoppingCart, Product product, int quantity)
        {
            var cartDetail = db.ShoppingCartDetails.Where(q => q.idShoppingCart == shoppingCart.ID).Where(q => q.StatusDel == 1).Where(q => q.idProudct == product.ID).FirstOrDefault();
            if(cartDetail == null)
            {
                await insertShoppingCartDetail(shoppingCart, product, quantity);
                return "";

            }
            var total = (product.StartingPrice - product.DiscountFrom) * quantity;
            cartDetail.Quantity += quantity;
            cartDetail.Total += total;
            cartDetail.CreateBy = GetUserId();
            cartDetail.UpdateBy = cartDetail.CreateBy;
            db.SaveChanges();
            return "";

        }


        public async Task<string> insertShoppingCart(Product product, int quantity)
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.ID = Guid.NewGuid();
            shoppingCart.StatusOrder = 1;
            shoppingCart.StatusDel = 1;
            shoppingCart.UpdateDate = DateTime.Now;
            shoppingCart.CreateDate = DateTime.Now;
            shoppingCart.CreateBy = GetUserId();
            shoppingCart.UpdateBy = shoppingCart.CreateBy;
            shoppingCart.IdShop = product.idShop;
            shoppingCart.ShippingFee = product.ShippingFee ?? 0;
            shoppingCart.TotalQantity = quantity;
            var total = (product.StartingPrice - product.DiscountFrom) * quantity;
            shoppingCart.Total = total;
            shoppingCart.TotalProductPrice = total;
            shoppingCart.DiscountAmount = 0;
            db.ShoppingCarts.Add(shoppingCart);
             db.SaveChanges();
            await insertShoppingCartDetail(shoppingCart, product, quantity);
            return "";
        }

        public async Task<string> insertShoppingCartDetail(ShoppingCart shoppingCart, Product product, int quantity)
        {
            try
            {
                var shoppingCartDetail = new ShoppingCartDetail();
                shoppingCartDetail.idShoppingCart = shoppingCart.ID;
                shoppingCartDetail.idProudct = product.ID;
                shoppingCartDetail.Image = db.ImageProducts.Where(q => q.idProduct == product.ID).Select(q => q.FilePath).FirstOrDefault();
                shoppingCartDetail.ProductName = product.Name;
                shoppingCartDetail.UnitPrice = product.DiscountFrom > 0 ? (product.StartingPrice - product.DiscountFrom) : product.StartingPrice;
                shoppingCartDetail.Quantity = quantity;
                var total = (product.StartingPrice - product.DiscountFrom) * quantity;
                shoppingCartDetail.Total = total;
                shoppingCartDetail.StatusDel = 1;
                shoppingCartDetail.UpdateDate = DateTime.Now;
                shoppingCartDetail.CreateDate = DateTime.Now;
                shoppingCartDetail.CreateBy = GetUserId();
                shoppingCartDetail.UpdateBy = shoppingCartDetail.CreateBy;
                db.ShoppingCartDetails.Add(shoppingCartDetail);
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Create a StringBuilder to accumulate the error messages
                var errorMessages = new StringBuilder();

                // Iterate over each validation error in the exception
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        // Append the error message to the StringBuilder
                        errorMessages.AppendLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }

                // Log the error messages or handle them as appropriate
                string errorMessage = errorMessages.ToString();
                System.Diagnostics.Debug.WriteLine(errorMessage);

                // Optionally, rethrow the exception or throw a new one with more details
                throw new Exception(errorMessage, ex);
            }
            return "";
        }

    }



}
