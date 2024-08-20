using Data;
using Data.Entity;
using Data.Helpers.Model;
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
            }).ToList();
            return result;
        }
    }
}
