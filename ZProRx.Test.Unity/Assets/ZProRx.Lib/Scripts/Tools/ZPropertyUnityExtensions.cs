using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZP.Lib;
using UniRx;

static public class ZPropertyUnityExtensions
{
    static public void EnableButton(this ZEvent zEvent, bool bEnable)
    {
        zEvent.TransNode.GetComponent<Button>().interactable = bEnable;
    }

    //static public void EnableLink(this ZEvent zEvent, IObservable<bool> observer)
    //{
    //    zEvent.TransNode.GetComponent<Button>().interactable = bEnable;
    //}
}
