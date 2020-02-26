using System;
using System.Collections.Generic;
using ZP.Lib.Card.Entity;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Card
{
    public interface ICansumable{
        List<ZCodeIdCount> ConsumeCards { get; }

        List<ZMaterial> ConsumeMaterials { get; }

        ZCurrency Currency { get; }
    }

    public interface ICansumable<TCardEnum, TMaterailEnum, TCurrencyEnum>
    {
        List<ZMaterial<TMaterailEnum>> TypedMaterials { get; }

        ZCurrency<TCurrencyEnum> TypedCurrency { get; }

        List<ZCodeIdCount<TCardEnum>> TypedCards { get; }
    }
}
