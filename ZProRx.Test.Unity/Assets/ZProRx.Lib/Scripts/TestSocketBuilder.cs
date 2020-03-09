using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using ZProRx.Lib.Unity;
using UniRx;
using System;
using UnityEngine.UI;

public class TestSocketBuilder : MonoBehaviour
{
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 5050;
    public string ClientID = "9577";
    // Start is called before the first frame update
    void Start()
    {
        ZPropertySocket.ClientIP = ServerIP;
        ZPropertySocket.ClientPort = ServerPort;
        ZPropertySocket.ClientID = ClientID;

        var system = ZPropertyMesh.CreateObject<TestSocketSystem>();

        ZViewBuildTools.BindObject(system, this.transform);

        system.OnStartSock.OnEventObservable().Subscribe(_ =>
        {
            Observable.Return(ZNull.Default).Delay(TimeSpan.FromSeconds(1)).Subscribe(__=>
            {
                var text = system.OnStartSock.TransNode?.GetComponentInChildren<Text>();//?.text;
                if (text != null)
                    text.text = ZPropertySocket.IsConnected() ? "Close" : "Start";
            });
        });

        //stop Socket link to server
        //ZPropertySocket.Start();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
