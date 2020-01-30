#if ZP_UNITY_CLIENT
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using ZP.Lib;
using UniRx;
using System;
using System.Text;
using System.Net.Security;
using ZP.Lib.Net;
using ZP.Lib.CoreEx;
using ZP.Lib.CoreEx.Reactive;

internal class TTPClient : TTPSingleton<TTPClient>, IConnectable, IDisposable
{
    private MqttClient mqttClient = null;

    //public Subject<string> RecvEvent = new Subject<string>();
    private List<KeyValuePair<string, Subject<string>>> RecvListeners = new List<KeyValuePair<string, Subject<string>>>();
    private List<KeyValuePair<string, Subject<ISocketPackage>>> RecvWithClientIdListeners = new List<KeyValuePair<string, Subject<ISocketPackage>>>();


    public void Connect()
    {
        var cert = Resources.Load("cacert") as TextAsset;

        mqttClient = new MqttClient(ZPropertySocket.ClientIP, ZPropertySocket.ClientPort, false, new X509Certificate(cert.bytes), 
        new RemoteCertificateValidationCallback
            (
                // 测试服务器未设置公钥证书，返回true即跳过检查，直接通过，否则抛出服务器证书无效Error
                (srvPoint, certificate, chain, errors) => true
            ));

        mqttClient.MqttMsgPublishReceived += msgReceived;
        mqttClient.MqttMsgSubscribed += (object sender, MqttMsgSubscribedEventArgs e) =>
        {
        };

        try
        {
            mqttClient.Connect(ZPropertySocket.ClientID, ZPropertySocket.ClientUser, ZPropertySocket.ClientPassword);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Socket connect error" + e.ToString());
        }

    }

    public void Disconnect()
    {
        if (mqttClient != null && mqttClient.IsConnected)
            mqttClient.Disconnect();

        foreach (var k in RecvListeners)
        {
            k.Value.Dispose();
        }

        RecvListeners.Clear();
    }

    public bool IsConnected()
    {
        return mqttClient != null && mqttClient.IsConnected;
    }


    public IObservable<string> SendMsg(string topic, string msg)
    {
        return Observable.Create<string>(observer => {
            if (mqttClient != null && mqttClient.IsConnected)
            {
                mqttClient.Publish(topic, Encoding.UTF8.GetBytes(msg));

                observer.OnNext("OK");
                observer.OnCompleted();
            }    
            else
            {
                observer.OnError(new Exception("Is not Connected!!"));
            }
            return null;
        });
    }

    public IObservable<string> Subscribe(string topic)
    {
        var ret = new Subject<string>();

        byte[] qos = new byte[1];
        qos[0] = 1;

        string[] msg = new string[1];
        msg[0] = topic;// + "/" + ZPropertySocket.ClientID;

        mqttClient.Subscribe(msg, qos);

        var k = new KeyValuePair<string, Subject<string>>(msg[0], ret);
        RecvListeners.Add(k);
        //mqttClient.Unsubscribe()
        return ret;
    }

    public IObservable<ISocketPackage> SubscribeWithClientId(string topic)
    {
        byte[] qos = new byte[1];
        qos[0] = 1;

        string[] msg = new string[1];
        msg[0] = topic;// + "/" + ZPropertySocket.ClientID;

        mqttClient.Subscribe(msg, qos);

        //var k = new KeyValuePair<string, Subject<string>>(msg[0], ret);
        //RecvListeners.Add(k);
        var ret = new Subject<ISocketPackage>();
        var k = new KeyValuePair<string, Subject<ISocketPackage>>(topic, ret);
        RecvWithClientIdListeners.Add(k);
        //mqttClient.Unsubscribe()
        return ret;
    }

    public void UnSubscribe(string topic)
    {
        //RecvWithClientIdListeners.RemoveAll(a => {
        //    var bFind = string.Compare(a.Key, topic) == 0;
        //    // if (bFind) a.Value.Dispose();
        //    return bFind;
        //});

        RecvListeners.RemoveAll(a => {
            var bFind = string.Compare(a.Key, topic) == 0;
            //  if (bFind) a.Value.Dispose();
            return bFind;
        });
    }


    void msgReceived(object sender, MqttMsgPublishEventArgs e)
    {
        //Debug.Log("服务器返回数据");
        string msg = System.Text.Encoding.Default.GetString(e.Message);
        //Debug.Log(msg + "topic : " + e.Topic);

        foreach (var k in RecvListeners)
        {
            if (MatchTopic(k.Key, e.Topic))
            {
                k.Value.OnNext(msg);
                break;
            }
        }

        foreach (var k in RecvWithClientIdListeners)
        {
            if (MatchTopic(k.Key, e.Topic))
            {
                k.Value.OnNext(new UniClientSocketPackage(e, msg));
                break;
            }
        }
    }

    bool MatchTopic(string key, string topic)
    {
        int index = key.IndexOf("/#");
        if (index > 0)
        {
            var subKey = key.Substring(0, key.Length - 2);
            return topic.Contains(subKey);
        }

        return string.Compare(key, topic, StringComparison.Ordinal) == 0;

    }

    public void Dispose(){
        Disconnect();
    }
}

#endif