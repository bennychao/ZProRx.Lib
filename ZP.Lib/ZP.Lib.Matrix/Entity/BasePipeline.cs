using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Core.Values;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix.Entity
{
    //implement IChannelClient
    public class BasePipeline : BaseChannel, IDisposable
    {
        private object channelLock = new object();
        private ZChannel channel;

        private int pipelineConnected = 0;

        //for single and himself
        protected string clientId { get; set; }

        public int ClientCount => clientIds.Count();

        public List<string> Clients => clientIds;



        public BasePipeline()
        {
        }

        /// <summary>
        /// for client to connect to pipeline's server
        /// </summary>
        /// <returns></returns>
        public IObservable<ZNull> Connect()
        {
            if (Interlocked.Exchange(ref pipelineConnected, 1) != 0)
            {
                //has run
                return Observable.Return<ZNull>(ZNull.Default);
            }

            // link to Server Channel
            lock (channelLock)
            {
                if (channel != null)
                {
                    return Observable.Return<ZNull>(ZNull.Default);
                }
                channel = zRoom.Channels.FindValue(a => string.Compare(a.Name.Value, ChannelName, StringComparison.Ordinal) == 0 && a.IsIn);
            }

            if (channel == null)
            {
                Interlocked.Exchange(ref pipelineConnected, 0);
                throw new Exception("Server have define this pipeline");
            }

            var ret = SendWithResult("connect");
            
            //var cid = selfId;
            return ret.ObserveOn(innerScheder).LinkedSubscribe( _=> Open(selfId));

            //return ret;
        }

        //for client to connect to server
        public IObservable<ZNull> Connect2()
        {
            if (Interlocked.Exchange(ref pipelineConnected, 1) != 0)
            {
                //has run
                return Observable.Return<ZNull>(ZNull.Default);
            }

            // link to Server Channel
            lock (channelLock)
            {
                if (channel != null)
                {
                    return Observable.Return<ZNull>(ZNull.Default);
                }
                channel = zRoom.Channels.FindValue(a => string.Compare(a.Name.Value, ChannelName, StringComparison.Ordinal) == 0 && a.IsIn);
            }
           

            if (channel == null)
                throw new Exception("Server have define this pipeline");

            return Observable.Create<ZNull>(observer =>
            {
                var ret = Send<ZNull, ZPropertyListHub<string>>("connect2", ZNull.Default);
                //var cid = selfId;
                return ret.ObserveOn(innerScheder).Subscribe(clientIds => {

                    // lock(this) for client to open the channel
                    clientId = selfId;
                    Open(selfId);
                    OnConnected.OnNext(selfId);

                    //check for agent, will contain self
                    foreach (var c in clientIds.Node)
                    {
                        //Debug.Log($"BasePipeline Client {c} Connect2  {selfId} ");
                        //if (string.Compare(c, selfId) == 0)
                        //    continue;
                        
                        if (TryAddClient(c))
                        {
                            OnConnected.OnNext(c);
                        }
                    }

                    observer.OnNext(ZNull.Default);

                },
                error =>
                {
                    Interlocked.Exchange(ref pipelineConnected, 0);
                    observer.OnError(error);
                }, 

                ()=>
                {
                    observer.OnCompleted();
                }
                );
            });
            //return ret.Select(_=> ZNull.Default);
        }

        //for client to call
        public void Disconnect()
        {
            if (Interlocked.Exchange(ref pipelineConnected, 0) != 1)
            {
                //has run
                return;
            }

            lock (channelLock)
            {
                if (channel != null){
                     SendWithResult("disconnect").ObserveOn(innerScheder).Subscribe(_ => {
                         Close();
                         channel = null;

                         //self Disconnected
                         OnDisConnected.OnNext(selfId);
                     });
                }
                //whill set in close
               // this.channel.Status.Value = ChannelStatusEnum.Closed;
            }
        }


        public List<string> GetActions()
        {
            // TODO
            throw new NotImplementedException();
        }

        public IObservable<ZChannelInfo> GetInfo()
        {
            //merge Current Info
            return Observable.Create<ZChannelInfo>(observer =>
           {
               return Send<ZNull, ZChannelInfo>("getinfo", ZNull.Default).Subscribe(serverInfo =>
               {
                   var clientInfo = GetChannelInfo();
                   foreach (var ac in serverInfo.Actions)
                   {
                       ac.IsInServer.Value = true;
                       clientInfo.Actions.Add(ac);
                   }
                   observer.OnNext(clientInfo);
               });
           });
        }

        protected new string GetBaseUrl()
        {
            return zChannel.Topic + "/";//.Url + "/Channel/" + channelName + "/";
        }

        protected IObservable<Unit> Send(string action, string data)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.Post(url, data);
        }

        protected IObservable<ZNull> SendWithResult(string action)
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.SendPackage<ZNull, ZNull>(url, ZNull.Default).ObserveOn(innerScheder);
        }

        public IObservable<Unit> Post<T>(string action, T data = default(T))
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.PostPackage<T>(url, data).ObserveOn(innerScheder);
        }

        public IObservable<TResult> Send<T, TResult>(string action, T data = default(T))
        {
            var url = GetBaseUrl() + action;

            return ZPropertySocket.SendPackage2<T, TResult>(url, data).ObserveOn(innerScheder);
        }

        //public new void Dispose(){
        //    Disconnect();
        //    base.Dispose();
        //}

        protected override void OnDispose()
        {
            Disconnect();
            base.OnDispose();
        }
    }
}
