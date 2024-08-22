using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Admin.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{

    public class mapProduct : mapCommon
    {

        public ProductViewModel getAllList(ProductViewModel model)
        {
            model.StatusDel = model.StatusDel ?? 1;
            model.IdGroup = model.IdGroup ?? -1;
            model.Search = model.Search == null ? "" : model.Search.ToLower();
            model.PageSize = 10; // Kích thước trang
            int skip = (model.Page - 1) * model.PageSize;
            var resultNew = new List<ProductModel>();
            var result = db.Products.Where(q => q.StatusDel == model.StatusDel)
              .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search))
              .Where(q => q.idGroup == model.IdGroup || model.IdGroup == -1)
              .Select(q => new ProductModel
              {
                  db = q,
                  GroupProductName = db.NhomSanPhams.Where(d => d.ID == q.idGroup).Where(d => d.StatusDel == 1).Select(d => d.TenNhom).FirstOrDefault(),
                  TypeProductName = db.TypeProducts.Where(d => d.ID == q.idType).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                  ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                  ShippingMethodName = q.ShippingMethod == 1 ? "Miễn phí" : "Có phí",
                  RefundFee = db.ReturnAndExchangePolicies.Where(d => d.ID == q.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).Select(d => d.RefundFee).FirstOrDefault() ?? 0,
                  WanrrantyName = db.Warranties.Where(d => d.ID == q.idWanrranty).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                  ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                  UpdateByName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.Username).FirstOrDefault()
              }).OrderByDescending(q => q.db.UpdateDate);
            if (model.TypeAction == 1)
            {
                var data = result.Skip(skip).Take(model.PageSize).ToList();
                resultNew = data;
            }
            else
            {
                var data = result.ToList();
                resultNew = data;
            }


            model.TotalCount = db.Products.Where(q => q.StatusDel == model.StatusDel)
                .Where(q => q.Name.ToLower().Contains(model.Search) || String.IsNullOrEmpty(model.Search))
                .Where(q => q.idGroup == model.IdGroup || model.IdGroup == -1).Count();



            model.CurrentPage = model.Page;
            resultNew.ForEach(q =>
            {
                var typeReturnAndChangePolice = "";
                if (!String.IsNullOrEmpty(q.db.idProductClassification))
                {
                    var listClassficationName = db.ProductClassifications.Where(d => q.db.idProductClassification.Contains(d.ID + "")).Where(d => d.StatusDel == 1).Select(d => d.Name).ToList();
                    q.ProductClassificationName = String.Join(",", listClassficationName);
                }
                var returnAndExchangePolicie = db.ReturnAndExchangePolicies.Where(d => d.ID == q.db.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).FirstOrDefault();
                if (returnAndExchangePolicie.Type == 1)
                {
                    typeReturnAndChangePolice = "Đổi trả miễn phí";
                }
                else
                {
                    typeReturnAndChangePolice = "Đổi trả có phí - ";
                    q.ReturnAndExchangePolicyFee = returnAndExchangePolicie.RefundFee;
                }
                q.ReturnAndExchangePolicyName = returnAndExchangePolicie.Name + typeReturnAndChangePolice;
            });
            model.Products = resultNew;
            return model;
        }

        public List<CommonModel> getListUse()
        {
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).Select(q => new CommonModel
            {
                id = q.ID,
                name = q.TenNhom
            }).ToList();
            return result;
        }

        public void SaveImageInfo(ImageProduct image)
        {
            // Lưu thông tin hình ảnh vào cơ sở dữ liệu
            image.NguoiCapNhat = GetUserId();
            image.NgayCapNhat = DateTime.Now;
            db.ImageProducts.Add(image);
            db.SaveChanges();
        }

        public void UpdateImageInfo(List<ImageProduct> updatedImages)
        {
            var idProduct = updatedImages.Select(q => q.idProduct).FirstOrDefault();
            var listIdImage = updatedImages.Where(q => q.ID != 0).Select(q => q.ID);

            if (listIdImage.Count() > 0)
            {
                var listImageToRomove = db.ImageProducts.Where(q => !listIdImage.Contains(q.ID)).ToList();
                db.ImageProducts.RemoveRange(listImageToRomove);
                db.SaveChanges();
            }

            // Cập nhật thông tin hình ảnh vào cơ sở dữ liệu
            foreach (var image in updatedImages)
            {

                var existingImage = db.ImageProducts.Find(image.ID);
                if (existingImage != null)
                {
                    existingImage.FileName = image.FileName;
                    existingImage.FileExtension = image.FileExtension;
                    db.SaveChanges();
                }
                else
                {
                    SaveImageInfo(image);
                }

            }
        }

        public List<CommonModel> getListShippingMethod()
        {
            var result = new List<CommonModel>()
            {
                new CommonModel(){id = 1,name="Miên phí ship"},
                new CommonModel(){id = 2,name="Có phí"}
            };
            return result;
        }

        public int insert(ProductModel model)
        {
            model.db.StartingPrice = String.IsNullOrEmpty(model.StartingPriceView) ? 0 : FormatDecimalView(model.StartingPriceView) ;
            model.db.DiscountPercentage = String.IsNullOrEmpty(model.DiscountPercentageView) ? 0 : FormatIntView(model.DiscountPercentageView) ;
            model.db.EndingPrice = String.IsNullOrEmpty(model.EndingPriceView) ? 0 : FormatDecimalView(model.EndingPriceView) ;
            model.db.DiscountFrom = String.IsNullOrEmpty(model.DiscountFromView) ? 0 : FormatDecimalView(model.DiscountFromView) ;
            model.db.DiscountUpTo = String.IsNullOrEmpty(model.DiscountUpToView) ? 0 : FormatDecimalView(model.DiscountUpToView) ;
            model.db.ShippingFee = String.IsNullOrEmpty(model.ShippingFeeView) ? 0 : FormatDecimalView(model.ShippingFeeView) ;
            model.db.Quantity = String.IsNullOrEmpty(model.QuantityView) ? 0 : FormatIntView(model.QuantityView);
            model.db.RemainingQuantity = model.db.Quantity;

            model.db.ID = Guid.NewGuid();
            model.db.CreateDate = DateTime.Now;
            model.db.CreateBy = GetUserId();
            model.db.UpdateBy = GetUserId();
            model.db.StatusDel = 1;
            model.db.UpdateDate = DateTime.Now;
            db.Products.Add(model.db);
            db.SaveChanges();
            return 1;
        }


        public void insertExcel(Product model)
        {
            db.Products.Add(model);
            db.SaveChanges();
        }

        public int edit(ProductModel model)
        {
            model.db.StartingPrice = String.IsNullOrEmpty(model.StartingPriceView) ? 0 : FormatDecimalView(model.StartingPriceView);
            model.db.DiscountPercentage = String.IsNullOrEmpty(model.DiscountPercentageView) ? 0 : FormatIntView(model.DiscountPercentageView);
            model.db.EndingPrice = String.IsNullOrEmpty(model.EndingPriceView) ? 0 : FormatDecimalView(model.EndingPriceView) ;
            model.db.DiscountFrom = String.IsNullOrEmpty(model.DiscountFromView) ? 0 : FormatDecimalView(model.DiscountFromView);
            model.db.DiscountUpTo = String.IsNullOrEmpty(model.DiscountUpToView) ? 0 : FormatDecimalView(model.DiscountUpToView) ;
            model.db.ShippingFee = String.IsNullOrEmpty(model.ShippingFeeView) ? 0 : FormatDecimalView(model.ShippingFeeView);
            model.db.Quantity = String.IsNullOrEmpty(model.QuantityView) ? 0 : FormatIntView(model.QuantityView) ;
            model.db.UpdateDate = DateTime.Now;
            model.db.UpdateBy = GetUserId();
            var item = db.Products.Where(q => q.ID == model.db.ID).SingleOrDefault();
            if (item != null)
            {
                item.Name = model.db.Name;
                item.idType = model.db.idType;
                item.idGroup = model.db.idGroup;
                item.StartingPrice = model.db.StartingPrice;
                item.EndingPrice = model.db.EndingPrice;
                item.DiscountPercentage = model.db.DiscountPercentage;
                item.DiscountFrom = model.db.DiscountFrom;
                item.DiscountUpTo = model.db.DiscountUpTo;
                item.idPolicy = model.db.idPolicy;
                item.ShippingMethod = model.db.ShippingMethod;
                item.ShippingFee = model.db.ShippingFee;
                item.Description = model.db.Description;
                item.Quantity = model.db.Quantity;
                item.RemainingQuantity = model.db.Quantity;
                item.SendFrom = model.db.SendFrom;
                item.idProductCategory = model.db.idProductCategory;
                item.idProductClassification = model.db.idProductClassification;
                item.idReturnAndExchangePolicy = model.db.idReturnAndExchangePolicy;
                item.idShop = model.db.idShop;
                item.StartTime = model.db.StartTime;
                item.EndTime = model.db.EndTime;
                item.UpdateBy = GetUserId();
                item.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }



        public int editExcel(ProductModel model)
        {
            var item = db.Products.Where(q => q.ID == model.db.ID).SingleOrDefault();
            if (item != null)
            {
                item.Name = model.db.Name;
                item.idType = model.db.idType;
                item.idGroup = model.db.idGroup;
                item.StartingPrice = model.db.StartingPrice;
                item.EndingPrice = model.db.EndingPrice;
                item.DiscountPercentage = model.db.DiscountPercentage;
                item.DiscountFrom = model.db.DiscountFrom;
                item.DiscountUpTo = model.db.DiscountUpTo;
                item.idPolicy = model.db.idPolicy;
                item.ShippingMethod = model.db.ShippingMethod;
                item.ShippingFee = model.db.ShippingFee;
                item.Description = model.db.Description;
                item.Quantity = model.db.Quantity;
                item.RemainingQuantity = model.db.Quantity;
                item.SendFrom = model.db.SendFrom;
                item.idProductCategory = model.db.idProductCategory;
                item.idProductClassification = model.db.idProductClassification;
                item.idReturnAndExchangePolicy = model.db.idReturnAndExchangePolicy;
                item.idShop = model.db.idShop;
                item.StartTime = model.db.StartTime;
                item.EndTime = model.db.EndTime;
                item.UpdateBy = GetUserId();
                item.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public ProductModel details(string id)
        {
            var result = db.Products.Where(q => q.ID.ToString() == id).Select(q => new ProductModel
            {
                db = q,
                GroupProductName = db.NhomSanPhams.Where(d => d.ID == q.idGroup).Where(d => d.StatusDel == 1).Select(d => d.TenNhom).FirstOrDefault(),
                TypeProductName = db.TypeProducts.Where(d => d.ID == q.idType).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                // ProductClassificationName = db.ProductClassifications.Where(d => d.ID == q.idProductClassification).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ShippingMethodName = q.ShippingMethod == 1 ? "Miễn phí" : "Có phí",
                ReturnAndExchangePolicyName = db.ReturnAndExchangePolicies.Where(d => d.ID == q.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                WanrrantyName = db.Warranties.Where(d => d.ID == q.idWanrranty).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.Username).FirstOrDefault()

            }).SingleOrDefault();
            result.StartingPriceView = result.db.StartingPrice == null ? "0" : FormatDecimalViewString((decimal)result.db.StartingPrice);
            result.EndingPriceView = result.db.EndingPrice == null ? "0" : FormatDecimalViewString((decimal)result.db.EndingPrice);
            result.DiscountFromView = result.db.DiscountFrom == null ? "0" : FormatDecimalViewString((decimal)result.db.DiscountFrom);
            result.DiscountUpToView = result.db.DiscountUpTo == null ? "0" : result.db.StartingPrice == null ? "0" : FormatDecimalViewString((decimal)result.db.DiscountUpTo);
            result.ShippingFeeView = result.db.ShippingFee == null ? "0" : FormatDecimalViewString((decimal)result.db.ShippingFee);
            result.QuantityView = result.db.Quantity == null ? "0" : FormatDecimalViewString((decimal)result.db.Quantity);
            result.DiscountPercentageView = result.db.DiscountPercentage == null ? "0" : FormatDecimalViewString((decimal)result.db.DiscountPercentage);

            //result.RemainingQuantityView = ((decimal)result.db.RemainingQuantity).ToString("#,##0");
            return result;
        }

        public int updateStatusDel(string id, int statusDel)
        {
            var item = db.Products.Where(q => q.ID.ToString() == id).SingleOrDefault();
            if (item != null)
            {
                item.StatusDel = statusDel;
                item.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
