using System;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix;

namespace ZP.Lib.Matrix
{
    [ChannelBoot(ChannelBootFlagEnum.UniCast)]
    public class UniCastPipeline : BaseCastPipeline, IChannelClient, IUniCast
    {
         public string CurClientId { get; set; }
        public UniCastPipeline()
        {
        }

        //implement IChannelClient
        public new  IObservable<ZNull> Post<T>(string action, T data)
        {
            if (CurClientId == "")
                throw new Exception("no support client id");

            var url = GetBaseUrl() + "_unicast_/" + CurClientId + "/" + action;

            return ZPropertySocket.PostPackage(url, data);
        }

        //implement IChannelClient
        public new IObservable<TResult> Send<T, TResult>(string action, T data)
        {
            if (CurClientId == "")
                throw new Exception("UniCast no support curClientId id");

            var url = GetBaseUrl() + "_unicast_/" + CurClientId + "/" + action;

            return ZPropertySocket.SendPackage<T, TResult>(url, data);
        }

        //implement IUniCast
        //public IObservable<Unit> UniCast(string curClientId, string action, object data)
        //{
        //    if (curClientId == "")
        //        throw new Exception("no support curClientId id");

        //    var url = GetBaseUrl() + "_unicast_/" + curClientId + "/" + action;

        //    return ZPropertySocket.Post(url, data);
        //}

        //implement IUniCast
        public IObservable<TResult> UniCast<T, TResult>(string curClientId, string action, T data)
        {
            if (curClientId == "")
                throw new Exception("no support curClientId id");

            var url = GetBaseUrl() + "_unicast_/" + curClientId + "/" + action;

            return ZPropertySocket.SendPackage<T, TResult>(url, data);
        }

        public IObservable<ZNull> UniCast<T>(string curClientId, string action, T data)
        {
            if (curClientId == "")
                throw new Exception("no support curClientId id");

            var url = GetBaseUrl() + "_unicast_/" + curClientId + "/" + action;

            return ZPropertySocket.PostPackage<T>(url, data);
        }

        public IObservable<TResult> UniCastMsg<T, TResult>(string curClientId, T data)
        {
            return UniCast<T, TResult>(curClientId, "msg_raw", data);
        }

        public IObservable<ZNull> UniCastMsg<T>(string curClientId, T data)
        {
            return UniCast<T>(curClientId, "msg_raw", data);
        }
    }
}
