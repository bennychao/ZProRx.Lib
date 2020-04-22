
using System;
using System.Collections.Generic;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix;

namespace ZP.Lib.Matrix
{
    [ChannelBoot(ChannelBootFlagEnum.BroadCast)]
    public class BroadCastPipeline : BaseCastPipeline, IChannelClient, IBroadCast
    {
        public IObservable<ZNull> BroadCast<T>(string action, T data = default(T))
        {
            var url = GetBaseUrl() + "_broadcast_/" + action;

            var ret = ZPropertySocket.PostPackage(url, data);
            //ret.Subscribe();

            return ret;
        }

        public IObservable<ZNull> BroadCastMsg<T>(T data)
        {
            return BroadCast<T>("msg_raw", data);
        }

        public new IObservable<ZNull> Post<T>(string action, T data = default(T))
        {
            var url = GetBaseUrl() + "_broadcast_/" + action;

            return ZPropertySocket.PostPackage(url, data);
        }

        public new  IObservable<TResult> Send<T, TResult>(string action, T data = default(T))
        {
            //var url = GetBaseUrl() + "_broadcast_/" + action;

            //return ZPropertySocket.SendPackage<T, TResult>(url, data);
            throw new Exception("BroadCast not support Send");
        }

    }
}