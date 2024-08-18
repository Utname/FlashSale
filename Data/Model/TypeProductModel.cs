using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class TypeProductModel
    {
        public TypeProductModel()
        {
            db = new TypeProduct();
        }
        public TypeProduct db { get; set; }
        public string UpdateByName { get; set; }
        public string NameProductGroup { get; set; }

    }

    public class TypeProductViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int? IdGroup { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<TypeProductModel> TypeProduct { get; set; }
    }

}