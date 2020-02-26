using System;
using ZP.Lib;
using ZP.Lib.Server.SQL;
using ZP.Lib.Card.Domain;

namespace ZP.Lib.Card.Entity
{

    [DBIndex("PlayerID")]
    public class BaseCardLink<TCard> : ICardLink, ICardLink<TCard> where TCard : class, ICard
    {
        //[DBIndex]
        public ZPropertyInterfaceRef<ICard> CardRef = new ZPropertyInterfaceRef<ICard>(OnLink);

        public ZProperty<ZDateTime> CreateTime = new ZProperty<ZDateTime>();

        [DBColumnName("Count")]
        public ZProperty<ZIntBar> Count = new ZProperty<ZIntBar>();

        //normal member
        //ICardLink implement
        public uint CardId => (uint)CardRef.RefID;

        public int CardCount
        {
            get => Count.Value;
            set => Count.Value.Cur.Value = value;
        }

        //ICardLink implement
        public TCard Card => CardRef.Value as TCard;

        public BaseCardLink()
        {
        }

        /// <summary>
        /// Ons the link.
        /// </summary>
        /// <returns>The link.</returns>
        /// <param name="cardID">Card identifier.</param>
        private static ICard OnLink(int cardID)
        {
            //create the Card when on server it will not linked
            return ZCardsFactory<TCard>.Instance.GetCard((uint)cardID);
        }

        static public BaseCardLink<TCard> CreateLink(uint cardId)
        {
            var cardLink = ZPropertyMesh.CreateObject<BaseCardLink<TCard>>();

            cardLink.CardRef.RefID = (int)cardId;

            cardLink.CreateTime.Value = ZDateTime.Now();

            return cardLink;
        }
    }
}
