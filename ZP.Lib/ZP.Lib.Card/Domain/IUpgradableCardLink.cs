using System;
using ZP.Lib;
using ZP.Lib.Card.Entity;

namespace ZP.Lib.Card.Domain
{
    public interface IUpgradableCardLink
    {
        ICansumable CurRankConsume {get;}

        void UpgradeCard();
    }
}
