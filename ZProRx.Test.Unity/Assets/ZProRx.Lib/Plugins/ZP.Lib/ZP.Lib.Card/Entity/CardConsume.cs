using System;
using System.Collections.Generic;
using System.Linq;
using ZP.Lib;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Card.Entity
{
    public class CardConsume : ICansumable
    {
        protected ZPropertyList<ZCodeIdCount> consumeCards = new ZPropertyList<ZCodeIdCount>();

        protected ZPropertyList<ZMaterial> consumeMaterials = new ZPropertyList<ZMaterial>();

        protected ZProperty<ZCurrency> currency = new ZProperty<ZCurrency>();

        //implement ICansumable
        public List<ZCodeIdCount> ConsumeCards => consumeCards.ToList();

        public List<ZMaterial> ConsumeMaterials => consumeMaterials.ToList();

        public ZCurrency Currency => currency.Value;

        public CardConsume()
        {
        }
    }

    public class CardConsume<TCardEnum, TMaterailEnum, TCurrencyEnum> : CardConsume, ICansumable<TCardEnum, TMaterailEnum, TCurrencyEnum>
    {
        public List<ZMaterial<TMaterailEnum>> TypedMaterials
            => ConsumeMaterials.Select(a => ZMaterial<TMaterailEnum>.Convert(a)).ToList();

        public ZCurrency<TCurrencyEnum> TypedCurrency => ZCurrency<TCurrencyEnum>.Convert(Currency);

        public List<ZCodeIdCount<TCardEnum>> TypedCards
            => ConsumeCards.Select(a => ZCodeIdCount<TCardEnum>.Convert(a)).ToList();
    }

    //public class ZCardConsume<TCard> :  CardConsume<TCard, MaterialTypeEnum, CurrencyTypeEnum>

}
