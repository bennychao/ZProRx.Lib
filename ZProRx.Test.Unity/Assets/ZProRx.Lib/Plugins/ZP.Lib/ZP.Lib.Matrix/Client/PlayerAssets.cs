using System;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using ZP.Lib;
using ZP.Lib.Card.Domain;
using ZP.Lib.Card.Entity;
using ZP.Lib.Card.Tools;
using ZP.Lib.Net;
using UniRx;
using ZP.Lib.CoreEx;
using ZP.Lib.Card;

namespace ZP.Lib.Matrix.Client
{
    public class PlayerAssets : PropObjectSingleton<PlayerAssets>
    {

        public (ZNetErrorEnum error, TCardLink link) ProduceCard<TCard, TCardLink>(uint cardId)
            where TCard : class, ICard
            where TCardLink : BaseCardLink<TCard>
        {
            //ZAssetCollection.Instance.CardCollection
            TCard card = ZCardsFactory<TCard>.Instance.GetCard(cardId);

            var rarity = ZPropertyMesh.GetProperty<int>(card, "Rarity");
            var cost = (rarity ^ 2) * 10;

            var timeCost = (rarity ^ 2) * 10;

            //get link type
            var linkType = ZAssetCollection.Instance.CardCollection.GetLinkType<TCard>();

            Assert.IsNotNull(linkType, "Not card link define");

            //add new card
            var cardLink = ZPropertyMesh.CreateObject<TCardLink>();// BaseCardLink<TCard>;

            cardLink.CardRef.RefID = (int)cardId; //bind

            cardLink.CreateTime.Value = ZDateTime.Now();


            var consumes = (cardLink as UpgradableCardLink<TCard>)?.CurRankConsume;

            //add to task
            ZTask<CardTaskEnum> task = ZTask<CardTaskEnum>.Create(CardTaskEnum.Produce); //GetCardName() + "." + 
            task.SetParam<TCardLink>(cardLink);
            task.Duration.Value = timeCost;

            var error = DoConsumes(consumes);

            var link = ZAssetCollection.Instance.CardCollection.GetCardsProp<TCardLink>();

            link.Add(cardLink);


            (cardLink as UpgradableCardLink<TCard>)?.Tasks.Add(task);

            task.OnEndObservable.Subscribe(_ =>
            {
                //add to link TODO
            });

            return (error, cardLink);
        }

        ZNetErrorEnum DoConsumes(ICansumable consume)
        {
            //check cards
            foreach (var cc in consume.ConsumeCards)
            {
                //cc.Code cc.Count
                var link = ZAssetCollection.Instance.CardCollection.GetCard(cc.Code);
                var curCount = link?.CardCount ?? 0;
                if (curCount < cc.Count)
                {
                    return ZNetErrorEnum.NotEnoughCard;
                }
            }

            //check materiales
            foreach (var cm in consume.ConsumeMaterials)
            {
                var mate = ZAssetCollection.Instance.MaterialCollection.GetMaterial(cm.CodeStr);
                var curCount = mate?.Count ?? 0;

                if (curCount < cm.Count)
                {
                    return ZNetErrorEnum.NotEnoughMaterial;
                }
            }

            var currency = ZAssetCollection.Instance.CurrencyCollection.GetCurrency(consume.Currency.CurrentTypeStr);
            var currCount = currency?.Value ?? 0;

            if (currCount < currency.Value)
            {
                return ZNetErrorEnum.NotEnoughMaterial;
            }



            //consume cards
            foreach (var cc in consume.ConsumeCards)
            {
                var link = ZAssetCollection.Instance.CardCollection.GetCard(cc.Code);
                link.CardCount -= cc.Count;
            }

            foreach (var cm in consume.ConsumeMaterials)
            {
                var mate = ZAssetCollection.Instance.MaterialCollection.GetMaterial(cm.CodeStr);
                mate.Count -= cm.Count;
            }

            currency.Value -= consume.Currency.Value;

            //foreach (var cm in consume.ConsumeMaterials)
            //{
            //    await ZPropertyNet.Post(
            //        MatrixConfig.StockmanMasterServer
            //        + $"/api/v1/stock/materials/{pid}/reduce/{cm.CodeStr}/{cm.Count}",

            //        null);
            //}


            //await ZPropertyNet.Post(
            //    MatrixConfig.MerchantMasterServer
            //    + $"/api/v1/merc/bank/consume/{pid}/{consume.Currency.CurrentTypeStr}/{consume.Currency.Value}",

            //    null);

            return ZNetErrorEnum.NoError;
        }

        ZNetErrorEnum DoRecycles(uint pid, CardRecycle recycle)
        {

            return ZNetErrorEnum.NoError;
        }
    }
}
