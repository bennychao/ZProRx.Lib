using UnityEngine;
using System.Collections;
using ZP.Lib;
using UnityEngine.UI;
using UniRx;

public class UICardItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    new public bool Bind(IZProperty property)
    {
        //bind with the Color Control
        //nametext = 
        var Shadow = ZViewBuildTools.FindComponentInChildren<Image>(this.transform, "Shadow");

        var colorProp = ZPropertyMesh.GetProperty(property, "TestCard.Color");

        colorProp.ValueChangeAsObservable().Subscribe(v =>
        {
            var col = (Vector3)(v);
            Shadow.color = new Color(col.x, col.y, col.z, 0.4f);
        });

        return base.BindBase(property);
    }

}
