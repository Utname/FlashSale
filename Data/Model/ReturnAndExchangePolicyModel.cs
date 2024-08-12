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
}