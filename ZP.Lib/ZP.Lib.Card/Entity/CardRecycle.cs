using System;
using System.Collections.Generic;
using System.Linq;
using ZP.Lib;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Card.Entity
{
    public class CardRecycle : IRecyclable
    {

        private ZPropertyList<ZCodeIdCount> recycleCards = new ZPropertyList<ZCodeIdCount>();

        private ZPropertyList<ZMaterial> recycleMaterials = new ZPropertyList<ZMaterial>();

        private ZProperty<ZCurrency> currency = new ZProperty<ZCurrency>();

        public List<ZCodeIdCount> RecycleCards => recycleCards.ToList();

        public List<ZMaterial> RecycleMaterials => recycleMaterials.ToList();

        public ZCurrency Currency => currency.Value;

        public CardRecycle()
        {
        }
    }

    public class CardRecycle<TCardEnum, TMaterailEnum, TCurrencyEnum> : CardRecycle, IRecyclable<TCardEnum, TMaterailEnum, TCurrencyEnum>
    {
        public List<ZMaterial<TMaterailEnum>> TypedMaterials
            => RecycleMaterials.Select(a => ZMaterial<TMaterailEnum>.Convert(a)).ToList();

        public ZCurrency<TCurrencyEnum> TypedCurrency => ZCurrency<TCurrencyEnum>.Convert(Currency);

        public List<ZCodeIdCount<TCardEnum>> TypedCards
            => RecycleCards.Select(a => ZCodeIdCount<TCardEnum>.Convert(a)).ToList();
    }
}
