using Data.Entity;
using Data.Helpers.Common;
using Data.Helpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class FunctionModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }

        public bool CheckAll { get; set; }
        public List<CommonModel> listFunction { get; set; }
        public List<int> SelectedFunctionIds { get; set; } // Add this property to capture selected IDs
    }


    public class FunctionViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<ChucNang> ChucNangs { get; set; }
    }


}