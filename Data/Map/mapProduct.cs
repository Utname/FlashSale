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
        public List<ProductModel> getAllList(string search, int statusDel,int? idGroup)
        {
            var result = db.Products.Where(q => q.StatusDel == statusDel)
                .Where(q => q.Name.ToLower().Contains(search) || String.IsNullOrEmpty(search))
                .Where(q=>q.idGroup == idGroup || idGroup == -1)
                .Select(q=> new ProductModel
                {
                    db = q,
                    GroupProductName = db.NhomSanPhams.Where(d=>d.ID == q.idGroup).Where(d=>d.StatusDel == 1).Select(d=>d.TenNhom).FirstOrDefault(),
                    TypeProductName = db.TypeProducts.Where(d => d.ID == q.idType).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                    ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                    ProductClassificationName = db.ProductClassifications.Where(d => d.ID == q.idProductClassification).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                    ShippingMethodName = q.ShippingMethod == 1 ? "Miễn phí" : "Có phí",
                    ReturnAndExchangePolicyName = db.ReturnAndExchangePolicies.Where(d => d.ID == q.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                    RefundFee = db.ReturnAndExchangePolicies.Where(d => d.ID == q.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).Select(d => d.RefundFee).FirstOrDefault()??0,
                    WanrrantyName = db.Warranties.Where(d => d.ID == q.idWanrranty).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                    ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                    UpdateByName = db.TaiKhoanShops.Where(d=>d.ID == q.idShop).Select(d=>d.Username).FirstOrDefault()
                }).OrderByDescending(q => q.db.UpdateDate).ToList();
            return result;
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
            image.NguoiCapNhat =GetUserId();
            image.NgayCapNhat = DateTime.Now;
            db.ImageProducts.Add(image);
            db.SaveChanges();
        }

        public void UpdateImageInfo(List<ImageProduct> updatedImages)
        {
            var idProduct = updatedImages.Select(q => q.idProduct).FirstOrDefault();
            var listIdImage = updatedImages.Where(q=>q.ID != 0).Select(q=>q.ID);

            if(listIdImage.Count() > 0)
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

        public int insert(ProductModel model)
        {
            model.db.StartingPrice = decimal.Parse(model.StartingPriceView.Replace(",", ""));
            model.db.EndingPrice = decimal.Parse(model.EndingPriceView.Replace(",", ""));
            model.db.DiscountFrom = decimal.Parse(model.DiscountFromView.Replace(",", ""));
            model.db.DiscountUpTo = decimal.Parse(model.DiscountUpToView.Replace(",", ""));
            model.db.ShippingFee = decimal.Parse(model.ShippingFeeView.Replace(",", ""));
            model.db.Quantity = int.Parse(model.QuantityView.Replace(",", ""));
            model.db.RemainingQuantity = int.Parse(model.RemainingQuantityView.Replace(",", ""));

            model.db.ID = Guid.NewGuid();
            model.db.CreateDate = DateTime.Now;
            model.db.CreateBy = GetUserId();
            model.db.UpdateBy =GetUserId();
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
            model.db.StartingPrice = decimal.Parse(model.StartingPriceView.Replace(",", ""));
            model.db.EndingPrice = decimal.Parse(model.EndingPriceView.Replace(",", ""));
            model.db.DiscountFrom = decimal.Parse(model.DiscountFromView.Replace(",", ""));
            model.db.DiscountUpTo = decimal.Parse(model.DiscountUpToView.Replace(",", ""));
            model.db.ShippingFee = decimal.Parse(model.ShippingFeeView.Replace(",", ""));
            model.db.Quantity = int.Parse(model.QuantityView.Replace(",", ""));
            model.db.RemainingQuantity = int.Parse(model.RemainingQuantityView.Replace(",", ""));
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
                item.RemainingQuantity = model.db.RemainingQuantity;
                item.SendFrom = model.db.SendFrom;
                item.idProductCategory = model.db.idProductCategory;
                item.idProductClassification = model.db.idProductClassification;
                item.idReturnAndExchangePolicy = model.db.idReturnAndExchangePolicy;
                item.idShop = model.db.idShop;
                item.StartTime = model.db.StartTime;
                item.EndTime = model.db.EndTime;
                item.UpdateBy =GetUserId();
                item.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public ProductModel details(string id)
        {
            var result = db.Products.Where(q => q.ID.ToString() == id).Select(q=> new ProductModel { 
                db = q,
                GroupProductName = db.NhomSanPhams.Where(d => d.ID == q.idGroup).Where(d => d.StatusDel == 1).Select(d => d.TenNhom).FirstOrDefault(),
                TypeProductName = db.TypeProducts.Where(d => d.ID == q.idType).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ProductCategoryName = db.ProductCategories.Where(d => d.ID == q.idProductCategory).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ProductClassificationName = db.ProductClassifications.Where(d => d.ID == q.idProductClassification).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ShippingMethodName = q.ShippingMethod == 1 ? "Miễn phí" : "Có phí",
                ReturnAndExchangePolicyName = db.ReturnAndExchangePolicies.Where(d => d.ID == q.idReturnAndExchangePolicy).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                WanrrantyName = db.Warranties.Where(d => d.ID == q.idWanrranty).Where(d => d.StatusDel == 1).Select(d => d.Name).FirstOrDefault(),
                ShopName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Where(d => d.StatusDel == 1).Select(d => d.TenShop).FirstOrDefault(),
                UpdateByName = db.TaiKhoanShops.Where(d => d.ID == q.idShop).Select(d => d.Username).FirstOrDefault()

            }).SingleOrDefault();
            result.StartingPriceView = ((decimal)result.db.StartingPrice).ToString("#,##0");
            result.EndingPriceView = ((decimal)result.db.EndingPrice).ToString("#,##0");
            result.DiscountFromView = ((decimal)result.db.DiscountFrom).ToString("#,##0");
            result.DiscountUpToView = ((decimal)result.db.DiscountUpTo).ToString("#,##0");
            result.ShippingFeeView = ((decimal)result.db.ShippingFee).ToString("#,##0");
            result.QuantityView = ((decimal)result.db.Quantity).ToString("#,##0");
            result.RemainingQuantityView = ((decimal)result.db.RemainingQuantity).ToString("#,##0");
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
