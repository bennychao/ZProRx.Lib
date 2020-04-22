using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Card.Entity;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Card
{
    public interface IRecyclable
    {
        List<ZCodeIdCount> RecycleCards { get; }

        List<ZMaterial> RecycleMaterials { get; }

        ZCurrency Currency { get; }
    }

    public interface IRecyclable<TCardEnum, TMaterailEnum, TCurrencyEnum>
    {
        List<ZMaterial<TMaterailEnum>> TypedMaterials { get; }

        ZCurrency<TCurrencyEnum> TypedCurrency { get; }

        List<ZCodeIdCount<TCardEnum>> TypedCards { get; }
    }
}
