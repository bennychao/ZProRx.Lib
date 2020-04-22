using System;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix;

namespace ZP.Lib.Matrix
{
    [ChannelBoot(ChannelBootFlagEnum.MultiCast)]
    public class MultiCastPipeline : BaseCastPipeline, IChannelClient, IMultiCast
    {
        private string curGroup = "";

        public bool IsInGroup => !string.IsNullOrEmpty(curGroup);

        public MultiCastPipeline()
        {
        }

        public IObservable<ZNull> JoinGroup(string group)
        {
            var url = GetBaseUrl() + "joingroup";
            var ret = ZPropertySocket.SendPackage<string, ZNull>(url, group).ObserveOn(innerScheder);
            //ret.Subscribe(
            //_ => {//do nothing
            //}
            ////, error => {
            ////    throw error;
            ////}
            //);
            curGroup = group;

            return ret;
        }

        public IObservable<ZNull> LeaveGroup(string group)
        {
            var url = GetBaseUrl() + "leavegroup";
            var ret =ZPropertySocket.SendPackage<string, ZNull>(url, group).ObserveOn(innerScheder);
            //ret.Subscribe(
            //    _ => {//do nothing
            //    }
            //    , error => {
            //        throw error;
            //    }
            //);

            curGroup = "";

            return ret;
        }

        public IObservable<ZNull> MultiCast<T>(string group, string action, T data = default)
        {
            var url = GetBaseUrl() + "_multicast_/" + group + "/" + action;

            var ret = ZPropertySocket.PostPackage(url, data);
            //ret.Subscribe();

            return ret;
        }

        public IObservable<ZNull> MultiCastMsg<T>(string group, T data)
        {
            return MultiCast<T>(group, "msg_raw", data);
        }

        //multicast
        public new IObservable<ZNull> Post<T>(string action, T data)
        {
            if (curGroup == "")
                throw new Exception($"MultiCast no support group id [{curGroup}]");

            var url = GetBaseUrl() + "_multicast_/" + curGroup + "/" + action;

            return ZPropertySocket.PostPackage(url, data);
        }

        //public IObservable<Unit> Post(string group, string action, object data)
        //{
        //    if (curGroup == "")
        //        throw new Exception("no support group id");

        //    var url = GetBaseUrl() + "_multicast_/" + group + "/" + action;

        //    return ZPropertySocket.Post(url, data);
        //}

        public new IObservable<TResult> Send<T, TResult>(string action, T data)
        {
            throw new Exception("BroadCast not support Send");
            //if (curGroup == "")
            //    throw new Exception("no support group id");

            //var url = GetBaseUrl() + "_multicast_/" + curGroup + "/" + action;

            //return ZPropertySocket.SendPackage<T, TResult>(url, data);
        }


        //public IObservable<TResult> Send<T, TResult>(string group, string action, T data)
        //{
        //    throw new Exception("BroadCast not support Send");
        //    //if (curGroup == "")
        //    //    throw new Exception("no support group id");

        //    //var url = GetBaseUrl() + "_multicast_/" + group + "/" + action;

        //    //return ZPropertySocket.SendPackage<T, TResult>(url, data);
        //}
    }
}
