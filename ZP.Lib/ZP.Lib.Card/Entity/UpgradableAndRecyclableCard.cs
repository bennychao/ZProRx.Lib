using System;
using ZP.Lib;
using ZP.Lib.Card;
using ZP.Lib.Card.Domain;

namespace ZP.Lib.Card.Entity
{
    
    public class UpgradableAndRecyclableCard : UpgradableCard
    {
        public ZRankableProperty<IRecyclable> Recycles = new ZRankableProperty<IRecyclable>();
        //public ZRankableProperty<IRecyclable> Recycles => recycles;

        public UpgradableAndRecyclableCard()
        {

        }

        public IRecyclable GetRecycles(int rank)
        {
            return Recycles.GetRank(rank);
        }
    }
}
