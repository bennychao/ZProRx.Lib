using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZP.Lib.Card.Domain;
using ZP.Lib;


namespace ZP.Lib.Card
{
    public class CardGroup
    {
        public ZProperty<int> GroupID = new ZProperty<int>();
        public ZPropertyRefList<ICard> CardRefs = new ZPropertyRefList<ICard>();
    }

    /// <summary>
    /// Cards factory.
    /// </summary>
    public sealed class ZCardsFactory<TCard>  : PropObjectSingleton<ZCardsFactory<TCard>>  where TCard : ICard
    {
        /// <summary>
        /// Card group.
        /// </summary>

        private ZPropertyList<CardGroup> Groups = new ZPropertyList<CardGroup>();

        internal ZPropertyInterfaceRef<ICard>.RefBindEvent OnBind;

        private List<int> activeGroups = new List<int>();

        private TextAsset CardsConfigText;
        private TextAsset[] CardAssets;



        //protected CardsFactory()
        //{

        //}

        public ZCardsFactory()
        {
            //Build();
        }


        public void Build(Action<TCard> buildAction = null)
        {
            var attr = typeof(TCard).GetCustomAttribute<CardAssetConfigAttribute>();

            //TODO temp before add the resources mgr
#if !ZP_UNITY_CLIENT
            var configPath = string.IsNullOrEmpty(attr?.ConfigPath) ?
                $"Config/{typeof(TCard).Name}s.json" :        //default asset path
                attr.AssetPath;
#else
            var configPath = string.IsNullOrEmpty(attr?.ConfigPath) ?
                $"Config/{typeof(TCard).Name}s" :        //default asset path
                attr.AssetPath;
#endif

            if (!string.IsNullOrEmpty(configPath))
                CardsConfigText = Resources.Load<TextAsset>($"{ServerPath.AppName}/Jsons/" + configPath);

            //ZCardsFactory<TCard>.Instance,
            ZPropertyPrefs.LoadFromStr(this, CardsConfigText?.text);

            var assetPath = string.IsNullOrEmpty(attr?.AssetPath) ? 
                $"{ServerPath.AppName}/Jsons/{typeof(TCard).Name}s" :        //default asset path
                attr.AssetPath;

            CardAssets = Resources.LoadAll<TextAsset>(assetPath);

            foreach (var g in Groups)
            {
                g.CardRefs.OnBindProp = OnCreateCard;
            }

            //to bind card object
            this.OnBind = (id) =>
            {
                var card = ZPropertyMesh.CreateObject<TCard>();

                //to build the card
                if (buildAction != null)
                {
                    buildAction(card);
                }

                //Card file mgr
                ZPropertyPrefs.LoadFromStr(card, GetCardStr(id));


                return card;
            };


            //bind 
            foreach (var g in Groups)
            {
                g.CardRefs.BindRefs();
            }
        }


        public void OnCreate()
        {
            //Build();
        }

        /// <summary>
        /// Ons the create card.
        /// </summary>
        /// <returns>The create card.</returns>
        /// <param name="id">Identifier.</param>
        static ICard OnCreateCard(int id)
        {
            //TODO
            //check group id
            return Instance.OnBind(id);
        }

        /// <summary>
        /// Release the cards
        /// </summary>
        public void UnBuild()
        {
            foreach (var g in Groups)
            {
                g.CardRefs.ClearAll();
            }

            Groups.ClearAll();

            activeGroups.Clear();
        }

        /// <summary>
        /// Gets the card string.
        /// </summary>
        /// <returns>The card string.</returns>
        /// <param name="cardID">Card I.</param>
        string GetCardStr(int cardID)
        {
            string cardName = typeof(TCard).Name + cardID.ToString();

            var card = CardAssets.ToList().Find(a => a.name.CompareTo(cardName) == 0);
            if (card != null)
                return card.text;

            return "";
        }

        /// <summary>
        /// Actives the group.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        public void ActiveGroup(int[] a)
        {
            activeGroups.AddRange( a.ToList());
        }

        /// <summary>
        /// Actives the group.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        public void ActiveGroup(int a)
        {
            activeGroups.Add(a);
        }

        /// <summary>
        /// Gets the card.
        /// </summary>
        /// <returns>The card.</returns>
        /// <param name="id">Identifier.</param>
        public TCard GetCard(uint id)
        {
            ICard ret = null;
            foreach (var g in Groups)
            {
                ret = g.CardRefs.FindValue(a => a == id);
                if (ret != null)
                    break;
            }
            return (TCard)ret;
        }

        public List<TCard> GetCards()
        {
            List<TCard> ret = new List<TCard>();
            foreach (var g in Groups)
            {
                ret.AddRange( g.CardRefs.ToList().Select(c => (TCard)c));
            }
            return ret;
        }

        public List<TCard> GetCards(uint groupId)
        {
            List<TCard> ret = new List<TCard>();
            foreach (var g in Groups)
            {
                if (g.GroupID == groupId)
                {
                    ret.AddRange(g.CardRefs.ToList().Select(c => (TCard)c));
                    break;
                }
            }
            return ret;
        }


        public TCard GetCard(int id)
        {
            ICard ret = null;
            foreach (var g in Groups)
            {
                ret = g.CardRefs.FindValue(a => a == id);
                if (ret != null)
                    break;
            }
            return (TCard)ret;
        }

        public TCard RepliCard(int id)
        {
            ICard ret = null;
            foreach (var g in Groups)
            {
                var retProp = g.CardRefs.FindProperty(a => a == id);
                if (retProp != null)
                {
                    //ret = ZPropertyMgr.CreateObject(retProp.GetDefineType()) as ICard;

                    //send copy msg after clone
                    ret = ZPropertyMesh.CloneObject(retProp.Value) as ICard;
                    break;
                }
            }

            return (TCard)ret;
        }
    }
}

