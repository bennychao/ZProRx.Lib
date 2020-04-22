using System;
using ZP.Lib;
using ZP.Lib.Card.Domain;

namespace ZP.Lib.Card.Entity
{
    public class UpgradableCardLink<TCard> : BaseCardLink<TCard>
        where TCard : class, ICard
    {
        [PropertyGroup("Statistics")]
        //[PropertyDescription("Exp:", "飞机经验值")]
        public ZExpProperty Exp = new ZExpProperty();

        //repair / produce
        public ZTaskPropertyList<CardTaskEnum> Tasks = new ZTaskPropertyList<CardTaskEnum>();
        //private ZRankableProperty<CardConsume> consumes = new ZRankableProperty<CardConsume>();

        public int MaxRank => Exp.Value.MaxRank;

        public ICansumable CurRankConsume => (Card as IUpgradableCard)?.GetCansume(Exp.Value.CurRank);
            //(Card as IUpgradableCard)?. Consumes.GetRank(Exp.Value.CurRank);

        public void UpgradeCard()
        {
            var curRank = Exp.Value.CurRank.Value;

            if (curRank < Exp.Value.MaxRank)
            {

                Card.Upgrade(curRank + 1);
            }
        }
    }
}
