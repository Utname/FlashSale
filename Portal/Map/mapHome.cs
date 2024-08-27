using Data;
using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Portal.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Map
{
    public class mapHome : mapCommon
    {
        public FlashSaleEntities db = new FlashSaleEntities();

        public List<CommonModelImage> getListGroupProduct()
        {
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).Select(q => new CommonModelImage
            {
                id = q.ID,
                name = q.TenNhom,
                image = q.Image,
            }).OrderByDescending(d => d.id).ToList();
            return result;
        }

        public List<MenuGroupProductModel> getListMenuPGroupProduct()
        {
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).GroupBy(p => new { p.ID, p.TenNhom }).Select(q => new MenuGroupProductModel
            {
                Id = q.Key.ID,
                Name = q.Key.TenNhom,
                listTypeProduct = db.TypeProducts.Where(d => d.StatusDel == 1).Where(d => d.idProductGroup == q.Key.ID).Select(d => new CommonModel
                {
                    id = d.ID,
                    name = d.Name
                }).OrderByDescending(d => d.id).ToList()
            }).OrderByDescending(q => q.Id).ToList();
            return result;
        }

        public List<BannerHomeModel> getListBanner()
        {
            var result = db.Banners.Where(q => q.StatusDel == 1).Select(q => new BannerHomeModel
            {
                Id = q.ID,
                Name = q.Name,
                Image = q.Image,
                Description = q.Description,
            }).OrderByDescending(d => d.Id).ToList();
            return result;
        }

        public List<HotSaleProductModel> getListHotSaleProduct()
        {
            var result = db.Products.Where(q => q.StatusDel == 1).Where(q => q.EndTime != null).Where(q => q.EndTime > DateTime.Now).Select(q => new HotSaleProductModel
            {
                Id = q.ID.ToString(),
                Name = q.Name,
                AverageRating = q.AverageRating ?? 3,
                TypeProductName = db.TypeProducts.Where(d => d.StatusDel == 1).Where(d => d.ID == q.idType).Select(d => d.Name).FirstOrDefault(),
                ShopName = db.TaiKhoanShops.Where(d => d.StatusDel == 1).Where(d => d.ID == q.idShop).Select(d => d.TenShop).FirstOrDefault(),
                DiscountPercentage = q.DiscountPercentage ?? 0,
                StartingPrice = q.StartingPrice ?? 0,
                EndingPrice = q.EndingPrice ?? 0,
                DiscountFrom = q.DiscountFrom ?? 0,
                DiscountUpTo = q.DiscountUpTo ?? 0,
                StartTime = q.StartTime,
                EndTime = q.EndTime,
                Image = db.ImageProducts.Where(d => d.idProduct == q.ID).Select(d => d.FilePath).FirstOrDefault() ?? "~/Areas/Admin/Content/FileUpload/Images/2024-7-7/design_patten.jpg",
            }).ToList();
            return result;
        }

        public List<GroupProductHomeModel> getListProductHome()
        {
            try {
                var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).GroupBy(group => new { group.ID, group.TenNhom }).Select(group => new GroupProductHomeModel
                {
                    idGroup = group.Key.ID,
                    GroupName = group.Key.TenNhom,
                    listTypeProdctHome = db.TypeProducts.Where(type => type.idProductGroup == group.Key.ID).Where(type => type.StatusDel == 1).GroupBy(type => new { type.ID, type.Name })
                                    .Select(type => new TypeProductHomeModel
                                    {
                                        idType = type.Key.ID,
                                        TypeName = type.Key.Name,
                                        UpdateDate = db.TypeProducts.Where(t => t.ID == type.Key.ID).Select(q => q.UpdateDate).FirstOrDefault(),
                                       
                                    }).OrderByDescending(type => type.UpdateDate).ToList(),
                    listProductHome = db.Products.Where(product => product.idGroup == group.Key.ID).Where(product => 
                         product.idType == db.TypeProducts.Where(type => type.idProductGroup == group.Key.ID).Where(type => type.StatusDel == 1).OrderByDescending(type => type.UpdateDate).Select(q => q.ID).FirstOrDefault()
                    ).Where(product => product.StatusDel == 1).
                                        Select(product => new ProductHomeModel
                                        {
                                            Id = product.ID.ToString(),
                                            Name = product.Name,
                                            AverageRating = product.AverageRating ?? 0,
                                            DiscountPercentage = product.DiscountPercentage ?? 0,
                                            ShopName = db.TaiKhoanShops.Where(d => d.StatusDel == 1).Where(d => d.ID == product.idShop).Select(d => d.TenShop).FirstOrDefault(),
                                            StartingPrice = product.StartingPrice ?? 0,
                                            EndingPrice = product.EndingPrice ?? 0,
                                            DiscountFrom = product.DiscountFrom ?? 0,
                                            DiscountUpTo = product.DiscountUpTo ?? 0,
                                            UpdateDate = product.UpdateDate,
                                            Image = db.ImageProducts.Where(d => d.idProduct == product.ID).Select(d => d.FilePath).FirstOrDefault() ?? "~/Areas/Admin/Content/FileUpload/Images/2024-7-7/design_patten.jpg",
                                        }).OrderByDescending(product => product.UpdateDate).Take(10).ToList(),
                    listAdvertisement = db.Advertisements.Where(ad=>ad.StatusDel == 1).Where(ad=>ad.idGroupProduct == group.Key.ID).Select(ad=> new AdvertisementModel
                    {
                        Id = ad.ID,
                        Name = ad.Name,
                        Image = ad.Image
                    }).ToList()
                }).OrderBy(group => group.GroupName).ToList();
                return result;
            }
            catch(Exception e) {
                return new List<GroupProductHomeModel>();
            }
        }

        public AdvertisementModel getAdvertisementHot()
        {
            var result = db.Advertisements.Where(q => q.Type == 1).Where(q => q.idGroupProduct == null).Where(q=>q.StatusDel == 1).Select(q => new AdvertisementModel
            {
                Id = q.ID,
                Name = q.Name,
                Description = q.Description,
                Image = q.Image
            }).FirstOrDefault();
            return result;
        }

        public List<AdvertisementModel> getAdvertisementBanner()
        {
            var result = db.Advertisements.Where(q => q.Type == 4).Where(q => q.idGroupProduct == null).Where(q => q.StatusDel == 1).Select(q => new AdvertisementModel
            {
                Id = q.ID,
                Name = q.Name,
                Description = q.Description,
                Image = q.Image
            }).ToList();
            return result;
        }
    }
}
