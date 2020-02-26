using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Card;
using ZP.Lib.Card.Domain;
using ZP.Lib.Card.Entity;

namespace ZP.Lib.Server.Test.Entity
{
    //test Card Class
    public class Tard : UpgradableAndRecyclableCard, IUpgradableCard, IRecyclableCard, ICard
    {
        public ZRankableProperty<float> Attack = new ZRankableProperty<float>();

        public ZRankableProperty<float> Blood = new ZRankableProperty<float>();

        [PropertyImageRes(ImageResType.LocalRes, "[APP]/Images/")]
        public ZProperty<string> Image = new ZProperty<string>();
    }



    public class TardLink : UpgradableAndRecyclableCardLink<Tard> , IUpgradableCardLink, IRecyclableCardLink
    {

    }

    public class Tard2 : UpgradableAndRecyclableCard, IUpgradableCard, IRecyclableCard, ICard
    {
        public ZRankableProperty<float> Attack = new ZRankableProperty<float>();

        public ZRankableProperty<float> Blood = new ZRankableProperty<float>();

        [PropertyImageRes(ImageResType.LocalRes, "[APP]/Images/")]
        public ZProperty<string> Image = new ZProperty<string>();
    }

    public class TardLink2 : UpgradableAndRecyclableCardLink<Tard2>, IUpgradableCardLink, IRecyclableCardLink
    {

    }

    public enum TardTypeEnum
    {
        Tard,
        Tard2
    }
}
