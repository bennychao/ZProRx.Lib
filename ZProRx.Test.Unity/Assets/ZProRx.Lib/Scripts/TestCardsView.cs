using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using UniRx;
using ZP.Lib.Unity;
using ZP.Lib.Core.Domain;

[PropertyAddComponentClass(typeof(ZUIDialogView))]
public class TestCardsView
{
    private ZProperty<ZIntBar> cardCount = new ZProperty<ZIntBar>();

    [PropertyDescription("category", "card category Des")]
    public ZProperty<CardCategoryEnum> CardCategory = new ZProperty<CardCategoryEnum>();

    [PropertyUIItemRes("ZProApp/Test/TestCardLinkItem", "Root")]
    private ZPropertyRefList<TestCard>  cardList = new ZPropertyRefList<TestCard>();

    private ZEvent onAddCard = new ZEvent();

    private ZEvent onDelCard = new ZEvent();

    //base event
    public ZEvent OnClose = new ZEvent();

    private MultiDisposable disposables = new MultiDisposable();

    private TestCard curSelCard = null;

    private TestPlayer player = null;

    private static int CardIdBase = 5;

    public void Link(TestPlayer player)
    {
        this.player = player;

        cardCount.Value.Max.Count(player.CardList);

        //link the cardlist
        cardList.Where(player.CardList, card => (card.Value as TestCard).Category.Value == CardCategory);

        cardCount.Value.Cur.Count(cardList);

        //var selEvents = ZPropertyMesh.GetEventsEx(this, ".cardList.*.onSelect");
        //foreach(var se in selEvents)
        //{
        //    se.OnEventObservable().Subscribe(ev =>
        //    {

        //    });
        //}


    }

    void OnBind(Transform node)
    {
        onAddCard.OnEventObservable().Subscribe(_ =>
        {
            var card = ZPropertyMesh.CreateObject<TestCard>();
            ZPropertyPrefs.LoadFromRes(card, "ZProApp/Test/TestCards/TestCard1"); //.json
            card.Index = CardIdBase++;
            this.player.CardList.Add(card);

            var onSelEvent = ZPropertyMesh.GetEventEx(card, ".onSelect");
            if (onSelEvent != null)
            {
                onSelEvent.OnEventObservable().Subscribe(__ =>
                {
                    ChangeCurCard(card);
                }).AddTo(disposables);
            }
           
        }).AddTo(disposables);

        onDelCard.OnEventObservable().Subscribe(_ =>
        {
            if (curSelCard != null)
            {
                this.player.CardList.Remove(card => card.Index == curSelCard.Index);

                curSelCard = null;
            }
                
        }).AddTo(disposables);

        var disp = ZPropertyObservable.SubEvents(this, ".cardList.*.onSelect").Subscribe(ev =>
        {
            Debug.Log("Click a Card");
            var curCard = ((ev as IDirectLinkable).Parent as TestCard);
            ChangeCurCard(curCard);

        }).AddTo(disposables);

        CardCategory.ValueChangeAsObservable().Subscribe(cat =>
        {
            cardList.ClearAll();
            cardList.Where(this.player.CardList, card => (card.Value as TestCard).Category.Value == CardCategory);
        }).AddTo(disposables);
    }

    void OnPreUnbind(Transform transform)
    {
        cardList.TransNode?.GetComponent<ZUIPropertyListItem>().ClearRoot();
    }

    void OnUnbind()
    {

        cardList.ClearAll();
        disposables.Dispose();
    }

    void ChangeCurCard(TestCard newCard)
    {
        if (newCard.Index != curSelCard?.Index)
        {
            if (curSelCard != null)
                curSelCard.Color.Value = new Vector3(0, 0, 0);
            curSelCard = newCard;

            curSelCard.Color.Value = new Vector3(1, 0, 0);
        }
    }
}