using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using UniRx;

public class TestBuilder : ZPropertyViewRootBehaviour, IZPropertyViewRoot
{
    public TextAsset BindText;

    //[PropertyAutoFind("CardsPanel")]
    public Transform cardsViewRoot;

    private TestPlayer testPlayer = null;

    private MultiDisposable disposables = new MultiDisposable();
    // Start is called before the first frame update
    void Start()
    {
        testPlayer = ZPropertyMesh.CreateObject<TestPlayer>();

        ZPropertyPrefs.LoadFromStr(testPlayer, BindText.text);

        ZViewBuildTools.BindObject(testPlayer, this.transform);
    }

    public void Bind(object root)
    {
        var onShowCards = ZPropertyMesh.GetEventEx(testPlayer, ".onShowCards");

        onShowCards.OnEventObservable().Subscribe(_ =>
        {
            var view = ZPropertyMesh.CreateObject<TestCardsView>();
            view.Link(testPlayer);

            ZViewBuildTools.BindObject(view, cardsViewRoot);

        }).AddToMultiDisposable(disposables);
    }

    public new void Unbind()
    {
        disposables.Dispose();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
