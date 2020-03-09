using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZP.Lib;
using UniRx;

using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using ZP.Lib.Net;
using LitJson;
using ZP.Lib.Common;
using ZP.Lib.Core.Values;
using ZP.Lib.Matrix.Domain;

using ZP.Lib.Core.Domain;
using ZP.Lib.Matrix;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Matrix.Entity
{

    public class BaseChannel : IDisposable
    {
        private bool bSupportClientSuit = false;

        protected IScheduler scheduler = null;

        protected ChannelListener channelListener = null;

        protected ZRoom zRoom = null;

        protected ZChannel zChannel = null;
        protected List<string> clientIds = new List<string>();

            // => clientIds?.FirstOrDefault(); not safe 

        //not thread safe 
        protected string selfId => (TaskScheduler.Current as IZMatrixRuntime)?.RunId ?? ZPropertySocket.ClientID;

        protected MultiDisposable disposables = new MultiDisposable();
        protected MultiDisposable defaultDisposables = new MultiDisposable();

        //for Client connected
        protected Subject<string> OnConnected = new Subject<string>();
        protected Subject<string> OnDisConnected = new Subject<string>();

        protected IScheduler innerScheder => scheduler ?? Scheduler.CurrentThread;


        //public
        public int RoomId => zRoom.RoomId;

        public bool IsSupportClientSuit => bSupportClientSuit;

        public bool IsServerChannel => (innerScheder as IServerRxScheduler) != null;


        public ChannelStatusEnum Status => zChannel.Status;

        public IObservable<ChannelStatusEnum> StatusChanged => zChannel.Status.ValueChangeAsObservable<ChannelStatusEnum>();

        public IObservable<string> OnConnectedObservable => OnConnected;
        public IObservable<string> OnDisConnectedObservable => OnDisConnected;

        //support the pipeline
        public string ChannelName
        {
            get
            {
                var typeName = this.GetType().Name;
                // var startindex = typeName.IndexOf(this.GetType().Name);
                return GetChannelName(typeName);
            }
        }
        //public ZRoom Room => zRoom;

        public BaseChannel()
        {
            //zRoom = room;

            var a = this.GetType().GetCustomAttribute<ChannelBootAttribute>();
            bSupportClientSuit = a?.Support(ChannelBootFlagEnum.ClientSuite) ?? false;
        }

        //can run on different scheduler
        virtual public void BindRoom(ZRoom room, IScheduler scheduler = null)
        {
            zRoom = room;
            this.scheduler = scheduler;

            var selfId = (scheduler as IGetMatrixRuntime)?.Runtime?.RunId ?? ZPropertySocket.ClientID;
            var clientId = !IsServerChannel ? selfId ?? "" : "";

            //check attribute
            channelListener = ChannelListener.GetInstance(zRoom.RoomId.ToString() + $"/{clientId}/" + ChannelName);
            channelListener.BaseUrl = GetBaseUrl();

            channelListener.ClientId = clientId;
            //var ch = room

            //find channel
            zChannel = zRoom.FindChannel(ChannelName);
        }

        //for suite Channel
        public void BuildClient(string clientId)
        {
            if (!bSupportClientSuit)
                throw new Exception("Build client with room channel, Check ChannelBootAttribute settings");

            this.AddClient(clientId);
        }

        protected void AddClient(string cid)
        {
            if (string.IsNullOrEmpty(cid))
                return;

            var f = clientIds.FindIndex(a => string.Compare(a, cid, true) == 0);
            if (f < 0)
                clientIds.Add(cid);
            else if(string.Compare(cid, selfId) == 0)
            {
                //do nothing
            }
            else 
                throw new ZNetException(ZNetErrorEnum.ClientAlreadyExists);
        }

        protected bool TryAddClient(string cid)
        {
            if (string.IsNullOrEmpty(cid))
                return false;

            var f = clientIds.FindIndex(a => string.Compare(a, cid, true) == 0);
            if (f < 0)
            {
                clientIds.Add(cid);
                return true;
            }

            return false;

        }

        protected bool TryDelClient(string cid)
        {
            if (string.IsNullOrEmpty(cid))
                return false;

            var f = clientIds.FindIndex(a => string.Compare(a, cid, true) == 0);
            if (f >= 0)
            {
                clientIds.Remove(cid);
                return true;
            }

            return false;
        }

        public void DisBuildClient(string clientId)
        {
            if (!bSupportClientSuit)
                throw new Exception("Build client with room channel, Check ChannelBootAttribute settings");

            this.clientIds.Remove(clientId);
        }

        public static string GetChannelName(string typeName)
        {
            var chindex = typeName.IndexOf("Channel", StringComparison.Ordinal);

            var pipleindex = typeName.IndexOf("Pipeline", StringComparison.Ordinal);
            return chindex > 0 ? typeName.Substring(0, typeName.Length - 7) :

                pipleindex > 0 ? typeName.Substring(0, typeName.Length - 8) : typeName;
        }

        protected bool isCurrentClient(string id) => !bSupportClientSuit || string.Compare(id, this.selfId) == 0;


        public void Listen()
        {
            UpdateChannelStatus(ChannelStatusEnum.Listen);

            //.CheckClient(a => isCurrentClient(a))
            IDisposable lisopen = null;
            lisopen = channelListener.ConnectObservable.ResponseOn(innerScheder) //ZPropertySocket.Receive<string>(GetBaseUrl() + "connect")
                .Subscribe((_, resp) =>
                {
                    Debug.Log("[" + ChannelName + "]  A Client Connected " + resp.Key + "(clientId)");

                    lock (this)
                    {
                        Open(resp.Key);
                    }

                    OnConnected.OnNext(resp.Key);
                    return ZNull.Default;
                });

            defaultDisposables.Add(lisopen);

            //use for return clients 
            lisopen = channelListener.Connect2Observable.ResponseOn(innerScheder) //ZPropertySocket.Receive<string>(GetBaseUrl() + "connect2")
                .Subscribe((_, resp) =>
                {
                    Debug.Log("[" + ChannelName + "]  A Client Connected " + resp.Key + "(clientId) With Connect2");

                    lock (this)
                        Open(resp.Key);

                    //Debug.Log("[" + ChannelName + "]  A Client Connected " + resp.Key + "(clientId) With Connect2 OnConnected");
                    OnConnected.OnNext(resp.Key);
                    return ZPropertyListHub<string>.CreateList(clientIds);
                });

            defaultDisposables.Add(lisopen);

            IDisposable lisclose = null;
            lisclose = channelListener.DisconnectObservable.ResponseOn(innerScheder) //ZPropertySocket.Receive<string>(GetBaseUrl() + "disconnect")
                .Subscribe((_, resp) =>
                {
                    lock (this)
                        Close(resp.Key);

                    Debug.Log("[" + ChannelName + "]  A Client DisConnected " + resp.Key);
                    OnDisConnected.OnNext(resp.Key);
                    return ZNull.Default;
                }
            );

            defaultDisposables.Add(lisclose);

            var lis = channelListener.StatusObservable.ResponseOn(innerScheder)// ZPropertySocket.ReceiveWithResult<string, ChannelStatusEnum>(GetBaseUrl() + "status")
                 .Subscribe(_ =>
                 {
                     //will call ZPropertySocket.Send<bool>()
                     return Status;
                 });

            defaultDisposables.Add(lis);

            //get channel info
            lis = channelListener.GetinfoObservable.ResponseOn(innerScheder)//ZPropertySocket.ReceiveWithResult<string, ZChannel>(GetBaseUrl() + "getinfo")
                .Subscribe(_ =>
                {
                    return GetChannelInfo();
                });

            defaultDisposables.Add(lis);


            lis = channelListener.GetactionsObservable.ResponseOn(innerScheder) //ZPropertySocket.ReceiveWithResult<string, List<string>>(GetBaseUrl() + "getactions")
                .Subscribe(_ =>
                {
                    //will call ZPropertySocket.Send<bool>()
                    return "";
                });

            defaultDisposables.Add(lis);

            //call pre listen method, it will listen when channel Listen status
            MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var m in methodInfos)
            {
                var a = m.GetCustomAttribute<ActionAttribute>();
                if (a == null)
                    continue;

                if (m.GetCustomAttribute<PreListenAttribute>() == null)
                    continue;

                ListenMethod(m, a);
            }

            OnListen();
        }

        public void Suspend()
        {
            UpdateChannelStatus(ChannelStatusEnum.Disabled);

            clientIds.Clear();

            //throw new NotImplementedException();
            defaultDisposables.Dispose();
            disposables.Dispose();

            channelListener.ClearTemplate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cid"></param>
        public void Open(string cid = "")
        {
            Debug.Log("[" + ChannelName + "] Channel Open clientID =" + cid);

            //[Room]/Channel/ChannelName/Action
            AddClient(cid);

            if (Status == ChannelStatusEnum.Opened)
                return ;


            //init action
            MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var m in methodInfos)
            {
                var a = m.GetCustomAttribute<ActionAttribute>();
                if (a == null)
                    continue;

                if (m.GetCustomAttribute<PreListenAttribute>() != null)
                    continue;

                ListenMethod(m, a);
            }

            if (Status != ChannelStatusEnum.Opened)
                OnOpened();

            UpdateChannelStatus(ChannelStatusEnum.Opened);
        }


        public void Close(string cid = "")
        {
            clientIds.Remove(cid);

            //DisBuildClient(cid);

            if (clientIds.Count > 0 && !string.IsNullOrEmpty(cid))
                return;

            //client count is 0 or cid = "" call by Disconnect
            if (Status != ChannelStatusEnum.Closed)
                OnCloseed();

            UpdateChannelStatus(ChannelStatusEnum.Closed);
                       
            disposables.Dispose();

            channelListener.ClearTemplate();
        }

        public void Dispose()
        {
            OnDispose();
            //throw new NotImplementedException();
            defaultDisposables.Dispose();
            disposables.Dispose();

            channelListener.ClearTemplate();
        }


        private void ListenMethod(MethodInfo m, ActionAttribute a)
        {
            var pParamType = GetBodyParamType(m);

            bool bIsRequireResponseParam = IsHasSocketResponseParam(m);

            //check is RawData
            if (ZPropertyMesh.IsRawDataRef(pParamType))
            {
                //CheckClient(c => isCurrentClient(c)).
                var sockObserver = channelListener.GetTemplateWithRawResultObservable(a.template).ResponseOn(innerScheder);
                //check client id in Where
                if (m.ReturnType != typeof(void))
                {
                    DoActionWithRawResult(m, sockObserver, pParamType);
                }
                else
                    disposables.Add(sockObserver.ResponceFilter(m, this).Subscribe((o, resp) =>
                    {
                        var ret = bIsRequireResponseParam ?
                        m.Invoke(this, new object[] { o as IRawDataPref, resp })
                        :
                        m.Invoke(this, new object[] { o as IRawDataPref });
                        //return ret;
                    }));
            }
            else if (pParamType != null)
            {
                //TODO record

                //check client id in Where
                if (m.ReturnType != typeof(void))
                {
                    //.CheckClient(c => isCurrentClient(c))
                    var sockObserver = channelListener.GetTemplateObservable(a.template, pParamType).ResponseOn(innerScheder);
                    // ZPropertySocket.ReceiveWithResult(GetBaseUrl() + a.template, pType);

                    DoActionWithResult(m, sockObserver, pParamType);
                }
                else
                {
                    var sockObserver = channelListener.GetTemplateWithNoResultObservable(a.template).ResponseOn(innerScheder);

                    disposables.Add(sockObserver.ResponceFilter(m, this).Subscribe((rawData, resp) =>
                    {
                        var id = resp.Key;
                        if (ZPropertyMesh.IsValueType(pParamType))
                        {
                            if (bIsRequireResponseParam)
                                CallFuncWithValueParam(m, rawData, pParamType, resp);
                            else
                                CallFuncWithValueParam(m, rawData, pParamType);
                        }
                        else
                        {
                            var o = ZPropertyMesh.CreateObject(pParamType);
                            ZPropertyPrefs.LoadFromRawData(o, rawData);

                            var b = bIsRequireResponseParam ?
                            m.Invoke(this, ZNull.IsNull(o) ? new object[] { resp } : new object[] { o, resp })
                            :
                            m.Invoke(this, ZNull.IsNull(o) ? null : new object[] { o });
                        }
                    }));
                }

            }
            else
            {
                //TODO record

                //check client id in Where
                if (m.ReturnType != typeof(void))
                {
                    //no param
                    // .CheckClient(c => isCurrentClient(c))
                    var sockObserver = channelListener.GetTemplateObservable(a.template).ResponseOn(innerScheder);

                    // ZPropertySocket.ReceiveWithResult(GetBaseUrl() + a.template, pType);
                    DoActionWithResult(m, sockObserver, typeof(ZNull));
                }
                else
                {
                    var sockObserver = channelListener.GetTemplateWithNoResultObservable(a.template).ResponseOn(innerScheder);
                    //.Where(o => true)
                    disposables.Add(sockObserver.ResponceFilter(m, this).Subscribe((_, resp) =>
                    {

                        var ret = bIsRequireResponseParam ? m.Invoke(this, new object[] { resp })
                        : m.Invoke(this, null);

                        //return ret;
                    }));
                }
            }
        }

        protected ZChannelInfo GetChannelInfo()
        {
            var ret = ZPropertyMesh.CreateObject<ZChannelInfo>();

            MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var m in methodInfos)
            {
                var a = m.GetCustomAttribute<ActionAttribute>();
                if (a == null)
                    continue;

                var pParamType = GetBodyParamType(m);

                var actionInfo = ZPropertyMesh.CreateObject<ZChannelAction>();

                actionInfo.Name.Value = m.Name;
                actionInfo.PackageParamType.Value = pParamType?.Name;
                actionInfo.ReturnType.Value = m.ReturnType.Name;

                ret.Actions.Add(actionInfo);
            }

            ret.ChannelRef.Value = this.zChannel;

            return ret;
        }


        //private functions
        private void CallFuncWithValueParam(MethodInfo m, IRawDataPref data, Type pType)
        {
            var json = data.RawData as JsonData;
            if (json.IsInt)
                m.Invoke(this, new object[] { (int)(json) });
            else if (json.IsString)
                m.Invoke(this, new object[] { (string)(json) });
            else if (json.IsDouble)
                m.Invoke(this, new object[] { (float)(double)(json) });
            else if (json.IsBoolean)
                m.Invoke(this, new object[] { (bool)(json) });
        }

        private void CallFuncWithValueParam(MethodInfo m, IRawDataPref data, Type pType, ISocketPackage resp)
        {
            var json = data.RawData as JsonData;
            if (json.IsInt)
                m.Invoke(this, new object[] { resp, (int)(json) });
            else if (json.IsString)
                m.Invoke(this, new object[] { resp, (string)(json) });
            else if (json.IsDouble)
                m.Invoke(this, new object[] { resp, (float)(double)(json) });
            else if (json.IsBoolean)
                m.Invoke(this, new object[] { resp, (bool)(json) });
        }

        private object CallFuncWithValueParamAndResult(MethodInfo m, IRawDataPref data, Type pType)
        {
            object ret = null;
            var json = data.RawData as JsonData;
            if (json.IsInt)
                ret = m.Invoke(this, new object[] { (int)(json) });
            else if (json.IsString)
                ret = m.Invoke(this, new object[] { (string)(json) });
            else if (json.IsDouble)
                ret = m.Invoke(this, new object[] { (float)(double)(json) });
            else if (json.IsBoolean)
                ret = m.Invoke(this, new object[] { (bool)(json) });

            return ret;
        }

        private object CallFuncWithValueParamAndResult(MethodInfo m, IRawDataPref data, Type pType, ISocketPackage resp)
        {
            object ret = null;
            var json = data.RawData as JsonData;
            if (json.IsInt)
                ret = m.Invoke(this, new object[] { resp, (int)(json) });
            else if (json.IsString)
                ret = m.Invoke(this, new object[] { resp, (string)(json) });
            else if (json.IsDouble)
                ret = m.Invoke(this, new object[] { resp, (float)(double)(json) });
            else if (json.IsBoolean)
                ret = m.Invoke(this, new object[] { resp, (bool)(json) });

            return ret;
        }

        //can only set inner
        protected void UpdateChannelStatus(ChannelStatusEnum status)
        {
            //zRoom.FindChannel()
            zChannel.Status.Value = status;
        }

        private void DoActionWithResult(MethodInfo m, IRawPackageObservable<IRawDataPref> sockObserver, Type bodyType)
        {
            IDisposable disposable = null;
            Dictionary<string, object> curResult = new Dictionary<string, object>();

            bool bIsRequireResponseParam = IsHasSocketResponseParam(m);

            ObjectLock objLock = new ObjectLock();

            IReactiveProperty<object> syncRet = new ReactiveProperty<object>(null);

            //do action main
            disposable = sockObserver.Subscribe(async (rawData, resp) =>
            {
                //var id = (disposable as IGetSocketResponse)?.SocketResponse.Key;
                var id = resp.Key;
                object ret = null;


                //return now
                var a = m.GetCustomAttribute<SyncResultAttribute>();
                if (a == null)
                {
                    if (ZPropertyMesh.IsValueType(bodyType))
                    {
                        if (bIsRequireResponseParam)
                            ret = CallFuncWithValueParamAndResult(m, rawData, bodyType, resp);
                        else
                            ret = CallFuncWithValueParamAndResult(m, rawData, bodyType);
                    }
                    else
                    {
                        var o = ZPropertyMesh.CreateObject(bodyType);
                        ZPropertyPrefs.LoadFromRawData(o, rawData);

                        try
                        {
                            ret = bIsRequireResponseParam ?

                                m.Invoke(this, ZNull.IsNull(o) ? new object[] { resp } : new object[] { o, resp })
                                :
                                m.Invoke(this, ZNull.IsNull(o) ? null : new object[] { o });
                        }
                        catch (TargetInvocationException e)
                        {
                            throw e.InnerException;
                        }

                    }
                    return ret;
                }

                //Debug.Log($"{m.Name} DoActionWithResult Enter Sync ClientId is " + id + " ClientIdsCount = " + clientIds.Count);
                //curResult.ContainsKey(id)
                lock (curResult)
                {
                    curResult[id] = true;
                }

                var timeoutObservable = new TimeoutObservable(a.Timeout);

                var runTask = Task.Run(() =>
                {
                    while (curResult.Keys.Count < clientIds.Count)
                    {
                        //Debug.Log($"{m.Name} DoActionWithResult Check  Sync Keys ClientId is " + id + " KeysCount = " + curResult.Keys.Count);
                        Thread.Sleep(500);
                    }

                });

                //wait for all client Call the sync function
                await Task.WhenAny(runTask, timeoutObservable.ToTask());

                if (timeoutObservable.IsTimeout)
                    throw new ZNetException(ZNetErrorEnum.Timeout);

                //Debug.Log($"{m.Name} DoActionWithResult Sync The Call ClientId is " + id);
                //curResult.Clear();

                //call the sync function, only one can Call the target function
                //but need to return to all caller, so lock can only support one return

                //using (var mu = new Mutex(true, this.ChannelName + m.Name, out bRun))
                {
                    if (objLock.GetLock())
                    {
                        //Debug.Log($"{m.Name} DoActionWithResult Get the Lock to Call Function " + id);
                        if (ZPropertyMesh.IsValueType(bodyType))
                        {
                            if (bIsRequireResponseParam)
                                ret = CallFuncWithValueParamAndResult(m, rawData, bodyType, resp);
                            else
                                ret = CallFuncWithValueParamAndResult(m, rawData, bodyType);

                            syncRet.Value = ret;
                        }
                        else
                        {
                            var o = ZPropertyMesh.CreateObject(bodyType);
                            ZPropertyPrefs.LoadFromRawData(o, rawData);
                            ret = m.Invoke(this, ZNull.IsNull(o) ? null : new object[] { o });

                            syncRet.Value = ret;
                        }
                    }
                }

                await syncRet.Where(obj => obj != null).Fetch();

                Debug.Log($"{m.Name} DoActionWithResult Sync the Result ClientId is " + id);
                return syncRet.Value;
                //return ZPropertyNet.OkResult(ret);
            });

            disposables.Add(disposable);
        }

        private void DoActionWithRawResult(MethodInfo m, IRawPackageObservable<IRawDataPref> sockObserver, Type bodyType)
        {
            IDisposable disposable = null;
            Dictionary<string, object> curResult = new Dictionary<string, object>();

            bool bIsRequireResponseParam = IsHasSocketResponseParam(m);

            ObjectLock objLock = new ObjectLock();
            IReactiveProperty<object> syncRet = new ReactiveProperty<object>(null);

            //run
            disposable = sockObserver.Subscribe(async (rawData, resp) =>
            {
                //var id = (disposable as IGetSocketResponse)?.SocketResponse.Key;
                var id = resp.Key;
                //var o = ZPropertyMesh.CreateObject(bodyType);
                //ZPropertyPrefs.LoadFromRawData(o, rawData);
                var o = rawData;

                var a = m.GetCustomAttribute<SyncResultAttribute>();
                if (a == null)
                {
                    return bIsRequireResponseParam ?
                    m.Invoke(this, ZNull.IsNull(o) ? new object[] { resp } : new object[] { o as IRawDataPref, resp })
                    :
                    m.Invoke(this, ZNull.IsNull(o) ? null : new object[] { o as IRawDataPref });
                    //return ret;
                }

                //curResult.ContainsKey(id)
                curResult[id] = true;

                var timeoutObservable = new TimeoutObservable(a.Timeout);

                var runTask = Task.Run(() =>
                 {
                     while (curResult.Keys.Count != clientIds.Count)
                         Thread.Sleep(100);
                 });

                await Task.WhenAny(runTask, timeoutObservable.ToTask());

                if (timeoutObservable.IsTimeout)
                    throw new ZNetException(ZNetErrorEnum.Timeout);

                //curResult.Clear();

                //call the method
                if (objLock.GetLock())
                {
                    var ret = bIsRequireResponseParam ?
                    m.Invoke(this, ZNull.IsNull(o) ? new object[] { resp } : new object[] { o as IRawDataPref, resp })
                    :
                    m.Invoke(this, ZNull.IsNull(o) ? null : new object[] { o as IRawDataPref });

                    syncRet.Value = ret;
                }

                await syncRet.Where(obj => obj != null).Fetch();

                Debug.Log($"{m.Name} DoActionWithRawResult Sync the Result ClientId is " + id);
                return syncRet.Value;
                //return ZPropertyNet.OkResult(ret);
            });

            disposables.Add(disposable);
        }

        //       protected Task WaitAllClient()
        //       {
        //           return Task.Run(() =>
        //           {
        //               while (curResult.Keys.Count != clientIds.Count)
        //                   Thread.Sleep(100);
        //           }
        //);
        //       }

        protected Type GetBodyParamType(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().ToList().Find(a =>
            a.GetCustomAttribute<FromPackageAttribute>() != null)?.ParameterType;
        }

        protected bool IsHasSocketResponseParam(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().ToList().Find(a =>
                a.ParameterType == typeof(ISocketPackage)) != null;
        }

        protected virtual void OnListen()
        {

        }

        protected virtual void OnOpened()
        {

        }

        protected virtual void OnCloseed()
        {

        }

        protected virtual void OnDispose()
        {

        }

        protected string GetBaseUrl()
        {
            //return "matrix/hall/" + RoomlId.ToString() + "/Channel/" + ChannelName + "/";
            return zRoom.ChannelBaseUrl + "/" + ChannelName + "/";
        }

        protected string GetBaseUrlPrefix()
        {
            return zRoom.ChannelBaseUrl + "/";
        }

        protected bool IsSupportAttribute<TAttr>() where TAttr : Attribute
        {
            return this.GetType().GetCustomAttribute<TAttr>() != null;
        }

    }
}
