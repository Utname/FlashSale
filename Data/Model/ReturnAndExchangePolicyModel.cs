using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashSale.Areas.Admin.Model
{
    public class ReturnAndExchangePolicyModel
    {
        public ReturnAndExchangePolicyModel()
        {
            db = new ReturnAndExchangePolicy();
        }
        public ReturnAndExchangePolicy db { get; set; }
        public string UpdateByName { get; set; }
        public string RefundFeeView { get; set; }
        public string TypePolicyName { get; set; }

    }

    public class ReturnAndExchangePolicyViewModel
    {
        public string Error { get; set; }
        public string Search { get; set; }
        public int? StatusDel { get; set; }
        public int? Type { get; set; }

        public int Page { get; set; } = 1;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<ReturnAndExchangePolicyModel> ReturnAndExchangePolicy { get; set; }
    }
}