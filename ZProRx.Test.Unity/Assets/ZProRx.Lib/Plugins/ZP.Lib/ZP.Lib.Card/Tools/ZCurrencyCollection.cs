using System;
using System.Linq;
using ZP.Lib;
using ZP.Lib.Card.Entity;

namespace ZP.Lib.Card.Tools
{
    public class ZCurrencyCollection
    {
        IZPropertyList<ZCurrency> propertyList = null;

        public ZCurrencyCollection()
        {

        }

        public void Bind(object obj)
        {
            propertyList = ZPropertyMesh.GetProperties(obj, typeof(ZCurrency))
                .Select(a => a as IZPropertyList<ZCurrency>)
                ?.FirstOrDefault();

            
        }

        public ZCurrency GetCurrency(string currTypeStr)
        {
            //var linkTypeStr = cardTypeStr + "Link";
            return propertyList.PropList.Find(p => p.GetDefineType().Name.Contains(currTypeStr))?.Value as ZCurrency;

            //return default(TCardLink);
        }
    }
}
