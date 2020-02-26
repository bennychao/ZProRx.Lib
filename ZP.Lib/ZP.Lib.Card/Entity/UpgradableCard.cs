using System;
using ZP.Lib;

namespace ZP.Lib.Card.Entity
{
    public class UpgradableCard : CommonCard
    {        
        public ZRankableProperty<ICansumable> Consumes = new ZRankableProperty<ICansumable>();

        //public ZRankableProperty<ICansumable> Consumes => consume111s;

        public UpgradableCard()
        {
        }

        public ICansumable GetCansume(int rank)
        {
            return Consumes.GetRank(rank);
        }
    }
}
