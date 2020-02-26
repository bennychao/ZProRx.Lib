using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Card;
using ZP.Lib.Card.Domain;
using ZP.Lib.Card.Entity;
using ZP.Lib.Server.Test.Entity;

namespace ZP.Lib.Server.Test
{
    public class UnitCards
    {
        [SetUp]
        public void Setup()
        {
            ServerPath.WorkPath = "../../..";
        }

        [Test]
        public void TestFactory()
        {
            ZCardsFactory<Tard>.Instance.Build();

            var cards = ZCardsFactory<Tard>.Instance.GetCards();

            Assert.IsTrue(cards.Count == 4);

            foreach (var c in cards)
            {
                c.Upgrade(1);
                Assert.AreEqual(c.Blood.Value, 8.0f, 0.01f);
            }

            var card = ZCardsFactory<Tard>.Instance.GetCard(1);
            Assert.IsTrue(card != null);

            Assert.AreEqual(card.Blood.Value, 8.0f, 0.01f);

            var cardRep = ZCardsFactory<Tard>.Instance.RepliCard(1);
            Assert.IsTrue(cardRep != null);

            Assert.AreEqual(cardRep.Blood.Value, 8.0f, 0.01f);

            cardRep.Upgrade(2);

            Assert.AreEqual(cardRep.Blood.Value, 10.0f, 0.01f);

            //two Cards not same
            Assert.AreEqual(card.Blood.Value, 8.0f, 0.01f);

            var card2 = ZCardsFactory<Tard>.Instance.GetCard(1);
            Assert.IsTrue(card2 != null);

            Assert.AreEqual(card2.Blood.Value, 8.0f, 0.01f);

            ZCardsFactory<Tard>.Instance.UnBuild();
            card2 = ZCardsFactory<Tard>.Instance.GetCard(1);

            Assert.IsTrue(card2 == null);

            //rebuild
            ZCardsFactory<Tard>.Instance.Build();

            cards = ZCardsFactory<Tard>.Instance.GetCards();

            Assert.IsTrue(cards.Count == 4);

            ZCardsFactory<Tard>.Instance.UnBuild();
        }

        [Test]
        public void TestCardProformance()
        {
            ZCardsFactory<Tard>.Instance.Build();
            var card = ZCardsFactory<Tard>.Instance.GetCard(1);
            card.Upgrade(1);

            for (int i = 0; i < 10000; i++)
            {
                var cardRep = ZCardsFactory<Tard>.Instance.RepliCard(1);
                Assert.IsTrue(cardRep != null);

                Assert.AreEqual(cardRep.Blood.Value, 8.0f, 0.01f);
            }
        }


        [Test]
        public void TestCardConsume()
        {
            ZCardsFactory<Tard>.Instance.Build(tard =>
            {
                //define the Tard type, Material and Currency
                tard.Consumes.Value = ZPropertyMesh.CreateObject<CardConsume<TardTypeEnum, MaterialTypeEnum, CurrencyTypeEnum>>();
            });

            var card = ZCardsFactory<Tard>.Instance.GetCard(1);
            Assert.IsTrue(card != null);

            card.Upgrade(1);

            Assert.AreEqual(card.Blood.Value, 8.0f, 0.01f);

            var camTyped = card.GetCansume(1) as ICansumable<TardTypeEnum, MaterialTypeEnum, CurrencyTypeEnum>;
            Assert.IsTrue(camTyped != null);

            Assert.IsTrue(camTyped.TypedCards.Count == 2);

            Assert.IsTrue(camTyped.TypedCurrency.CurrentType == CurrencyTypeEnum.Corn);

            Assert.IsTrue(camTyped.TypedMaterials[0].CurrentType == MaterialTypeEnum.Oil);
        }
    }
}
