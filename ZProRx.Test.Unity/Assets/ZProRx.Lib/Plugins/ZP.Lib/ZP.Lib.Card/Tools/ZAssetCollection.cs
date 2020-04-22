using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.Card.Tools
{
    public class ZAssetCollection : PropObjectSingleton<ZAssetCollection>
    {
        ZCardCollection cardCollection = null;
        ZCurrencyCollection currencyCollection = null;
        ZMaterialCollection materialCollection = null;

        public ZCardCollection CardCollection => cardCollection;

        public ZCurrencyCollection CurrencyCollection => currencyCollection;

        public ZMaterialCollection MaterialCollection => materialCollection;

        public void Bind(object obj)
        {
            //ZPropertyMesh.GetProperties(obj, )
        }
    }
}
