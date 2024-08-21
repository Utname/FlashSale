using Data;
using Data.Entity;
using Data.Helpers.Model;
using FlashSale.Areas.Portal.Model;
using System;
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
                id=q.ID,
                name = q.TenNhom,
                image = q.Image,
            }).OrderByDescending(d => d.id).ToList();
            return result;
        }

        public List<MenuGroupProductModel> getListMenuPGroupProduct()
        {
            var result = db.NhomSanPhams.Where(q => q.StatusDel == 1).GroupBy(p => new {p.ID,p.TenNhom}).Select(q => new MenuGroupProductModel
            {
                Id = q.Key.ID,
                Name = q.Key.TenNhom,
                listTypeProduct = db.TypeProducts.Where(d=>d.StatusDel == 1).Where(d=>d.idProductGroup == q.Key.ID).Select(d=> new CommonModel { 
                    id = d.ID,
                    name = d.Name
                }).OrderByDescending(d=>d.id).ToList()
            }).OrderByDescending(q=>q.Id).ToList();
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

    }
}
