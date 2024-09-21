using Data.Entity;
using Data.Helpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Portal.Model
{
    public class ModalViewReLoadModel
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SubClose { get; set; }
        public string SubDetails { get; set; }


    }

    public class ModalViewModel : ModalViewReLoadModel
    {
        public string RedirectFunctionClose { get; set; }
        public string RedirectFunctionDetails { get; set; }

    }
}