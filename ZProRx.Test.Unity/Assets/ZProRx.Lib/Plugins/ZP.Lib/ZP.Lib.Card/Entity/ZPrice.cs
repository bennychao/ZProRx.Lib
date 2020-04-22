using System;
using System.Linq;
using ZP.Lib;

namespace ZP.Lib.Card.Entity
{
    public class ZPrice
    {
        private ZPropertyList<ZCurrency> costs = new ZPropertyList<ZCurrency>();

        private ZPropertyList<float> discount = new ZPropertyList<float>();

        public ZPrice()
        {
        }

        public float Cost(string currencyType)
        {
            var index = costs.FindIndex(a => string.Compare(a.CurrentTypeStr, currencyType, true) == 0);
             return costs.ToList()[index].Value * discount.ToList()[index];
        }

        public float Origin(string currencyType)
        {
            var index = costs.FindIndex(a => string.Compare(a.CurrentTypeStr, currencyType, true) == 0);
            return costs.ToList()[index].Value;
        }
    }
}
