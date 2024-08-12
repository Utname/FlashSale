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

  
}