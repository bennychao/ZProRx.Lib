using System;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix
{
    public class BaseCastPipeline : BasePipeline
    {
        protected Subject<string> onStrMsg = new Subject<string>();

        protected Subject<IRawDataPref> onRawMsg = new Subject<IRawDataPref>();

        //Observable.Return<TData>(default(TData));
        public BaseCastPipeline()
        {
        }

        [PreListen]
        [Action("onclientconnected")]
        protected ZNull OnClientConnected([FromPackage] string clientId)
        {
            //Debug.Log("BaseCastPipeline OnClientConnected clientId=" + clientId);

            if (string.Compare(clientId, selfId) == 0)
                return ZNull.Default;

            if (TryAddClient(clientId))
                OnConnected.OnNext(clientId);
            //AddClient(clientId);
            return ZNull.Default;
        }

        [PreListen]
        [Action("onclientdisconnected")]
        protected ZNull OnClientDisConnected([FromPackage] string clientId)
        {
            if (TryDelClient(clientId))
                OnDisConnected.OnNext(clientId);

            //this.clientIds.Remove(clientId);
            //or throw a exception

            return ZNull.Default;
        }

        protected virtual void OnMsg(string strData)
        {

        }

        protected virtual void OnRawMsg(IRawDataPref rawData)
        {

        }

        [Action("msg")]
        public void MsgFunc([FromPackage]string strData)
        {
            OnMsg(strData);
            onStrMsg.OnNext(strData);
        }

        [Action("msg_raw")]
        public void MsgFuncWithRawData([FromPackage]IRawDataPref rawData)
        {
            OnRawMsg(rawData);
            onRawMsg.OnNext(rawData);
        }

        public IObservable<TData> MsgObservable<TData>()
        {
            return onStrMsg.Select(str =>
            {
                var data = ZPropertyMesh.CreateObject<TData>();
                ZPropertyPrefs.LoadFromStr(data, str);
                return data;
            });

        }

        public IObservable<TData> RawMsgObservable<TData>()
        {
            return onRawMsg.Select(raw =>
            {
                var data = ZPropertyMesh.CreateObject<TData>();
                ZPropertyPrefs.LoadFromRawData(data, raw);
                return data;
            });

        }


    }
}
