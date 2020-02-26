using System;
using ZP.Lib;
using ZP.Lib.Card.Entity;

namespace ZP.Lib.Card.Domain
{
    public interface IUpgradableCard
    {
        //ZRankableProperty<ICansumable> Consumes { get; }
        ICansumable GetCansume(int rank);
    }
}
