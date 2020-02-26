using System;
using System.Collections.Generic;
using ZP.Lib;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Card.Entity
{
    public class BoxRankItem //: ZRange<int>
    {
        private ZPropertyList<ZCodeIdCount> cards = new ZPropertyList<ZCodeIdCount>();
        
        private ZProperty<short> rarity = new ZProperty<short>();
        private ZProperty<float> probability = new ZProperty<float>();

        //private ZProperty<string> cardCode = new ZProperty<string>();

        private ZProperty<ZCurrency> earn = new ZProperty<ZCurrency>();

        //private static ICard OnLink(int cardID)
        //{
        //    //create the Card when on server it will not linked
        //    //TODO
        //    return null; //ZCardsFactory<TCard>.Instance.GetCard((uint)cardID);

        //}

        //public string CardCode => cardCode.Value;

        public ZCurrency Earn => earn.Value;

            //TODO
        public List<ZCodeIdCount> RandomCards()
        {
            List<ZCodeIdCount> rets = new List<ZCodeIdCount>();

            foreach (var cc in cards)
            {
                var c = ZPropertyMesh.CreateObject<ZCodeIdCount>();
                c = ZPropertyMesh.CloneObject(cc) as ZCodeIdCount;
                c.Count.Value = (int)RandomMgr.Next(0, cc.Count.Value);
                rets.Add(c);
            }

            return rets;
        }
    }
}
