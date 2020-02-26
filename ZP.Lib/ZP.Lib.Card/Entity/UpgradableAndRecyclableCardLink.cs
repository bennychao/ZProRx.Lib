using System;
using ZP.Lib;
using ZP.Lib.Card.Domain;

namespace ZP.Lib.Card.Entity
{
    public class UpgradableAndRecyclableCardLink<TCard> : UpgradableCardLink<TCard>
         where TCard : class, ICard
    {

        public IRecyclable CurRankRecycle => (Card as IRecyclableCard)?.GetRecycles(Exp.Value.CurRank);


        public UpgradableAndRecyclableCardLink()
        {
        }
    }
}
