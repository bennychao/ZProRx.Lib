using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZP.Lib;
using ZP.Lib.Card.Domain;

namespace ZP.Lib.Card.Tools
{
    public class ZCardCollection
    {
        List<IZPropertyList> CardLinkPropList = new List<IZPropertyList>();

        public void Bind(object obj)
        {
            var linkList = ZPropertyMesh.GetProperties(obj, typeof(ICardLink)).Select( a=> a as IZPropertyList);

            CardLinkPropList.AddRange(linkList);
        }

        public ICardLink GetCard(Type cardType)
        {
            return CardLinkPropList.Find(p => cardType == (p as IZProperty).GetDefineType())
                ?.PropList.Select(a => a.Value as ICardLink).ToList()?.FirstOrDefault();

            //return default(TCardLink);
        }


        public ICardLink GetCard(string cardTypeStr)
        {
            var linkTypeStr = cardTypeStr + "Link";
            return CardLinkPropList.Find(p => (p as IZProperty).GetDefineType().Name.Contains(linkTypeStr))
                ?.PropList.Select(a => a.Value as ICardLink).ToList()?.FirstOrDefault();

            //return default(TCardLink);
        }

        public List<TCardLink> GetCards<TCardLink>() where TCardLink : class, ICardLink
        {
            return  CardLinkPropList.Find(p => typeof(TCardLink) == (p as IZProperty).GetDefineType())
                ?.PropList.Select(a => a.Value as TCardLink ).ToList();

            //return default(TCardLink);
        }

        public IZPropertyList<TCardLink> GetCardsProp<TCardLink>() where TCardLink : class, ICardLink
        {
            return CardLinkPropList.Find(p => typeof(TCardLink) == (p as IZProperty).GetDefineType()) as IZPropertyList<TCardLink>;

            //return default(TCardLink);
        }

        public int GetCardTypeCount<TCardLink>(uint cardId) where TCardLink : class, ICardLink
        {
            return GetCardsProp<TCardLink>()?.Count ?? 0;
        }

        public int GetCardCount<TCardLink>(uint cardId) where TCardLink : class, ICardLink
        {
            return (GetCardsProp<TCardLink>() as IEnumerable<ICardLink>)?.ToList().Find(a => a.CardId == cardId)?.CardCount ?? 0;
        }

        public void ConsumeCard<TCardLink>(uint cardId, int count) where TCardLink : class, ICardLink
        {
            var link = (GetCardsProp<TCardLink>() as IEnumerable<ICardLink>)?.ToList().Find(a => a.CardId == cardId);
            if (link != null)
            {
                link.CardCount -= count;
            }
        }

        public void AddCard<TCardLink>(uint cardId, int count) where TCardLink : class, ICardLink
        {
            var link = (GetCardsProp<TCardLink>() as IEnumerable<ICardLink>)?.ToList().Find(a => a.CardId == cardId);
            if (link != null)
            {
                link.CardCount += count;
            }
        }

        public Type GetLinkType<TCard>() where TCard : class, ICard
        {
            var cardLinkProp =CardLinkPropList.Select(a => a as IZProperty).Where(a => a.GetDefineType() == typeof(TCard))?.FirstOrDefault();

            return cardLinkProp?.GetDefineType();
        }
        
    }
}
