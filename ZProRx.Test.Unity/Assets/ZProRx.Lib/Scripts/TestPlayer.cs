using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using UniRx;
using ZP.Lib.Unity.RTComponents;
using ZP.Lib.Unity;

public enum TaskTypeEnum
{
    Timer,
    Kill
}


[AddTriggerComponentClassAttribute(typeof(ZUIHoldableItem), ".OnHold")]
public class TestHoldObject
{
    public ZProperty<bool> bHolding = new ZProperty<bool>();

    public ZEvent<bool> OnHold = new ZEvent<bool>();
}

[AddTriggerComponentClassAttribute(typeof(ZUIClickableItem), ".OnClick")]
public class TestClickObject
{
    public ZProperty<bool> bClick = new ZProperty<bool>();

    public ZEvent OnClick = new ZEvent();
}

public class TestPlayer 
{
    //static panel 
    [PropertyGroup("Main")]
    private ZProperty<string> name = new ZProperty<string>();

    [PropertyImageRes(ImageResType.LocalRes, "ZProApp/Test/")]
    private ZProperty<string> portrait = new ZProperty<string>();

    [PropertyDescription("blood", "bloodDes")]
    private ZProperty<ZDataBar> blood = new ZProperty<ZDataBar>();

    [PropertyDescription("attack", "bloodDes")]
    private ZProperty<ZIntBar> attack = new ZProperty<ZIntBar>();

    //count
    private ZProperty<int> power = new ZProperty<int>();

    //count
    private ZProperty<int> mine = new ZProperty<int>();

    private ZProperty<int> cardCount = new ZProperty<int>();

    //static panel 2
    private ZProperty<ZDateTime> createTime = new ZProperty<ZDateTime>();

    [PropertyDescription("speed", "bloodDes")]
    private ZProperty<ZDataBar> speed = new ZProperty<ZDataBar>();

    [PropertyDescription("rank", "bloodDes")]
    private ZProperty<ZIntBar> rank = new ZProperty<ZIntBar>();

    private ZProperty<ZIntBar> curIntBar = new ZProperty<ZIntBar>();

    private ZProperty<ZIntBar> timerProgress = new ZProperty<ZIntBar>();

    private ZTimerProperty timerProp = new ZTimerProperty();
    
    private ZProperty<int> progress = new ZProperty<int>();

    private ZProperty<float> loadingProgress = new ZProperty<float>();

    private ZTaskProperty<TaskTypeEnum> Task = new ZTaskProperty<TaskTypeEnum>();

    private ZProperty<Vector2> curPosition = new ZProperty<Vector2>();

    private ZProperty<TestHoldObject> holdObj = new ZProperty<TestHoldObject>();

    private ZProperty<TestClickObject> clickObj = new ZProperty<TestClickObject>();

    // input panel
    [PropertyImageRes(ImageResType.LocalRes, "ZProApp/Test/")]
	private ZProperty<GroupTypeEnum> group = new ZProperty<GroupTypeEnum>();

    [PropertyDescription("male", "maleDes")]
    private ZProperty<bool> bMale = new ZProperty<bool>();

    [PropertyDescription("category", "card category Des")]
    public ZProperty<CardCategoryEnum> CardCategory = new ZProperty<CardCategoryEnum>();


    [PropertyLink("cardList")]
    private ZLinkProperty<int> curSelect = new ZLinkProperty<int>();

    [PropertyUIItemRes("ZProApp/Test/TestCardLinkItem", "Root")] //you should create perfab's varient in this folder
    public ZPropertyList<TestCard>  CardList = new ZPropertyList<TestCard>();

    [PropertyDescription("Msg", "messageDes")]
    private ZProperty<string> message = new ZProperty<string>();

    //message List
    private ZMsgList zMsgs = new ZMsgList();

    //events
    private ZEvent onSendMsg = new ZEvent();

    private ZEvent onShowCards = new ZEvent();

    //not property
    private MultiDisposable disposables = new MultiDisposable();

    void OnCreate(){
        //add Cards

        var card = ZPropertyMesh.CreateObject<TestCard>();
        ZPropertyPrefs.LoadFromRes(card, "ZProApp/Test/TestCards/TestCard1"); //.json
        CardList.Add(card);

        card = ZPropertyMesh.CreateObject<TestCard>();
        ZPropertyPrefs.LoadFromRes(card, "ZProApp/Test/TestCards/TestCard2"); //.json
        CardList.Add(card);

        card = ZPropertyMesh.CreateObject<TestCard>();
        ZPropertyPrefs.LoadFromRes(card, "ZProApp/Test/TestCards/TestCard3"); //.json
        CardList.Add(card);

        card = ZPropertyMesh.CreateObject<TestCard>();
        ZPropertyPrefs.LoadFromRes(card, "ZProApp/Test/TestCards/TestCard4"); //.json
        CardList.Add(card);

        holdObj.Value.OnHold.OnEventObservable<bool>().Subscribe(_ =>
        {
            var msg = ZMsg.Create("msg", $"Hold Object {_} !!!");
            msg.Timer = 2;

            zMsgs.AddTimer(msg);

            holdObj.Value.bHolding.ActiveNode(_);
        }).AddTo(disposables);

        clickObj.Value.OnClick.OnEventObservable().Subscribe(_ =>
        {
            var msg = ZMsg.Create("msg", $"Click Object {_} !!!");
            msg.Timer = 2;

            zMsgs.AddTimer(msg);
            
        }).AddTo(disposables);

        cardCount.Count(CardList).AddTo(disposables);

        timerProgress.Select(timerProp).AddTo(disposables);

    }

    void OnBind(Transform node)
    {

        //first to hind the MsgsList
        //zMsgs.ActiveNode(false);
        zMsgs.TransNode.transform.DetachChildren();

        //add Hello Message
        var msg = ZMsg.Create("msg", $"HELLO ZProRx.Lib !!!");
        msg.Timer = 2;

        zMsgs.AddTimer(msg);


        onSendMsg.OnEventObservable().Subscribe(_ =>
        {
            msg = ZMsg.Create("msg", $"Send a Message {message.Value}");
            msg.Timer = 2;

            zMsgs.AddTimer(msg);

            //zMsgs.ActiveNode(true);
        }).AddTo(disposables);

        timerProp.OnEndObservable.Subscribe(_ =>
        {
            msg = ZMsg.Create("msg", $"Timer is End");
            msg.Timer = 2;

            zMsgs.AddTimer(msg);
        });

        timerProp.Start(10); //Start timer

        //onShowCards.OnEventObservable().Subscribe(_ =>
        //{
        //    var view = ZPropertyMesh.CreateObject<TestCardsView>();
        //    view.Link(this);

        //    ZViewBuildTools.BindObject(view)

        //}).AddTo(disposables);

        bMale.ValueChangeAsObservable<bool>().Subscribe(
            _=>
            {
                msg = ZMsg.Create("msg", $"bMale Changed {_}");
                msg.Timer = 2;

                zMsgs.AddTimer(msg);
                Debug.Log($"change the Toggle and Cur Male is {_}");
            }).AddTo(disposables);

        CardCategory.ValueChangeAsObservable<CardCategoryEnum>().Subscribe(
            _=>
            {
                msg = ZMsg.Create("msg", $"Dropdown Changed {_}");
                msg.Timer = 2;

                zMsgs.AddTimer(msg);
                Debug.Log($"change the Dropdown and Cur Card Category is {_}");
            }).AddTo(disposables);

        loadingProgress.Value = 0;

        //init the Task
        Task.Value.OnTickObservable.Subscribe(offset =>
        {
            //Task offset is 0.01s
            loadingProgress.Value += offset / 10;
        }).AddTo(disposables);

        Task.Value.OnEndObservable.Subscribe(_ =>
        {
            
            Task.Value.Reset();
            loadingProgress.Value = 0;
            Task.Value.Start();

            msg = ZMsg.Create("msg", $"Loading Task Reset Changed");
            msg.Timer = 2;

            zMsgs.AddTimer(msg);
            
        }).AddTo(disposables);

        Task.Value.Duration.Value = 10;
        Task.Value.Start();

        holdObj.Value.bHolding.ActiveNode(false);
    }

    void OnUnbind()
    {
        disposables.Dispose();
    }
}
