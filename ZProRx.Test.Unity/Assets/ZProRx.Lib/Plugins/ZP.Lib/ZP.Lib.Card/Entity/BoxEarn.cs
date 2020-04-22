using System;
using System.Collections.Generic;
using System.Linq;
using ZP.Lib;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Card.Entity
{
    public class BoxEarn
    {
        internal ZPropertyList<ZCodeIdCount> cards = new ZPropertyList<ZCodeIdCount>();

        internal ZProperty<ZCurrency> currency = new ZProperty<ZCurrency>();

        public List<ZCodeIdCount> Cards => cards.ToList();

        public ZCurrency Currency => currency.Value;
        public BoxEarn()
        {
        }
    }
}
