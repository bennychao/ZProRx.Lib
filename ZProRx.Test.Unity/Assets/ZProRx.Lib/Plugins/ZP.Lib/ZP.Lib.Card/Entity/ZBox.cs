using System;
using System.Collections.Generic;
using ZP.Lib;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Card.Entity
{

    public class ZBox : ICard
    {
        private ZProperty<uint> id = new ZProperty<uint>();

        private ZProperty<string> name = new ZProperty<string>();

        private ZRankableProperty<BoxRankItem> ranksMap = new ZRankableProperty<BoxRankItem>();

        private ZProperty<short> rank = new ZProperty<short>();

        private ZRankableProperty<float> expires = new ZRankableProperty<float>();

        private ZProperty<string> cardCode = new ZProperty<string>();

        [PropertyImageRes(ImageResType.LocalRes, "[APP]/Boxs/")]
        private ZProperty<string> image = new ZProperty<string>();

        public uint Id => id.Value;

        public string Name => name.Value;

        public short Rank => rank.Value;

        public float CurExpires => expires.GetRank(Rank);

        public ZCurrency CurEarn => ranksMap.GetRank(Rank).Earn;

        public string CardCode => cardCode.Value;

        //public ZCurrency  => ranksMap.GetRank(Rank).Earn;

        public string Image => image.Value;
        
        public BoxEarn Open()
        {
            //List<uint> commonCards = new List<uint>();
            var ret = ZPropertyMesh.CreateObject<BoxEarn>();

            ret.currency.Value = CurEarn; //*??

            var map = ranksMap.GetRank(Rank);
            var cards = map.RandomCards();

            ret.cards.AddRange(cards);

            return ret;
        }

        public void Upgrade(int rank)
        {
            throw new NotImplementedException();
        }
    }
}
