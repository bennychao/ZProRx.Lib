using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Core.Values;
using ZP.Lib.Core.Main;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Net;

namespace ZP.Lib.Matrix
{
    //for one client on chanel to listen the same topic
    public class ChannelListener : PropObjectSingletonWIthId<string, ChannelListener>
    {
        protected Dictionary<string, object> actionMap = new Dictionary<string, object>();

        //connect
        IRawPackageObservable<ZNull> connectObservable = null;

        //connect2
        IRawPackageObservable<ZNull> connect2Observable = null;

        //disconnect
        IRawPackageObservable<ZNull> disconnectObservable = null;

        IObservable<string> statusObservable = null;

        IObservable<ZNull> getinfoObservable = null;

        IObservable<string> getactionsObservable = null;

        IObservable<ISocketPackage> broadCastObservable = null;


        //for syncframe action
        IObservable<ISocketPackage> syncFrameObservable = null;

        IObservable<ZNull> roundStartObservable = null;


        IObservable<ZNull> roundStopObservable = null;

        IObservable<ZNull> roundHandObservable = null;

        //for round custom action 
        IObservable<ISocketPackage> roundCastObservable = null;

        //for systemmgr action
        IObservable<ISocketPackage> unlinkSysObservable = null;


        //string is groupId
        IRawPackageObservable<string> joinObservable = null;


        IObservable<ISocketPackage> uniCastObservable = null;


        IRawPackageObservable<string> UnjoinObservable = null;


        IObservable<ISocketPackage> syncFrameEnumObservable = null;

        IObservable<ISocketPackage> roundCmdObservable = null;


        IObservable<SyncFrameUpdatePackage> syncFrameUpdateObservable = null;

        IObservable<ISocketPackage> multiCastObservable = null;
        public string BaseUrl
        {
            get; set;
        }

        public string ClientId
        {
            get;set;
        }

        public string clientSuffix
        {
            get
            {
                return string.IsNullOrEmpty(ClientId) ? "" : "/" + ClientId;
            }
        }


        public IRawPackageObservable<ZNull> ConnectObservable
        {
            get => connectObservable ?? (connectObservable = ZPropertySocket.ReceiveRawPackageAndResponse<ZNull, ZNull>(BaseUrl + "connect" + clientSuffix));
        }


        public IRawPackageObservable<ZNull> Connect2Observable
        {
            get => connect2Observable ?? (connect2Observable = ZPropertySocket.ReceiveRawPackageAndResponse<ZNull, ZPropertyListHub<string>>(BaseUrl + "connect2" + clientSuffix));
        }


        public IRawPackageObservable<ZNull> DisconnectObservable
        {
            get => disconnectObservable ?? (disconnectObservable = ZPropertySocket.ReceiveRawPackageAndResponse<ZNull, ZNull>(BaseUrl + "disconnect" + clientSuffix));
        }


        public IObservable<string> StatusObservable
        {
            get => statusObservable ?? (statusObservable = ZPropertySocket.ReceivePackageAndResponse<string, ChannelStatusEnum>(BaseUrl + "status" + clientSuffix));
        }



        public IObservable<ZNull> GetinfoObservable
        {
            get => getinfoObservable ?? (getinfoObservable = ZPropertySocket.ReceivePackageAndResponse<ZNull, ZChannelInfo>(BaseUrl + "getinfo" + clientSuffix));
        }



        public IObservable<string> GetactionsObservable
        {
            get => getactionsObservable ?? (getactionsObservable = ZPropertySocket.ReceivePackageAndResponse<string, List<string>>(BaseUrl + "getactions" + clientSuffix));
        }


        public IObservable<ISocketPackage> BroadCastObservable
        {
            get => broadCastObservable ?? (broadCastObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "_broadcast_/#"));
        }

        public IObservable<ISocketPackage> MultiCastObservable
        {
            get => multiCastObservable ?? (multiCastObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "_multicast_/#"));
        }

        public IObservable<ISocketPackage> UniCastObservable
        {
            get => uniCastObservable ?? (uniCastObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "_unicast_/#"));
        }



        public IRawPackageObservable<string> JoinGroupObservable
        {
            get => joinObservable ?? (joinObservable = ZPropertySocket.ReceiveRawPackageAndResponse<string, ZNull>(BaseUrl + "joingroup"));
        }


        public IRawPackageObservable<string> UnJoinGroupObservable
        {
            get => UnjoinObservable ?? (UnjoinObservable = ZPropertySocket.ReceiveRawPackageAndResponse<string, ZNull>(BaseUrl + "leavegroup"));
        }

        public IObservable<ISocketPackage> SyncFrameObservable
        {
            get => syncFrameObservable ?? (syncFrameObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "syncframe/#"));
        }

        public IObservable<ISocketPackage> SyncFrameEnumObservable
        {
            get => syncFrameEnumObservable ?? (syncFrameEnumObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "syncframeenum"));
        }

        public IObservable<SyncFrameUpdatePackage> SyncFrameUpdateObservable
        {
            get => syncFrameUpdateObservable ?? (syncFrameUpdateObservable = ZPropertySocket.ReceivePackage<SyncFrameUpdatePackage>(BaseUrl + "syncframeupdate"));
        }

        //IObservable<ZNull> syncStartObservable = null;
        //public IObservable<ZNull> SyncStartObservable
        //{
        //    get => syncStartObservable ?? (syncStartObservable = ZPropertySocket.ReceiveWithResult<ZNull>(BaseUrl + "syncframestart"));
        //}

        //IObservable<ZNull> syncStopObservable = null;
        //public IObservable<ZNull> SyncStopObservable
        //{
        //    get => syncStopObservable ?? (syncStopObservable = ZPropertySocket.ReceiveWithResult<ZNull>(BaseUrl + "syncframestop"));
        //}

        //IObservable<string> clientConnectedObservable = null;

        //public IObservable<string> ClientConnectedObservable
        //{
        //    get => clientConnectedObservable ?? (clientConnectedObservable = ZPropertySocket.Receive<string>(BaseUrl + "onclientconnected" + clientSuffix));
        //}


        //IObservable<string> clientDisConnectedObservable = null;

        //public IObservable<string> ClientDisConnectedObservable
        //{
        //    get => clientDisConnectedObservable ?? (clientDisConnectedObservable = ZPropertySocket.Receive<string>(BaseUrl + "onclientdisconnected" + clientSuffix));
        //}

        //// --- ---------------     Round Channel

        public IObservable<ZNull> RoundStartObservable
        {
            get => roundStartObservable ?? (roundStartObservable = ZPropertySocket.ReceivePackageAndResponse<ZNull>(BaseUrl + "roundstart"));
        }

        public IObservable<ZNull> RoundStopObservable
        {
            get => roundStopObservable ?? (roundStopObservable = ZPropertySocket.ReceivePackageAndResponse<ZNull>(BaseUrl + "roundstop"));
        }

        public IObservable<ZNull> RoundHandObservable
        {
            get => roundHandObservable ?? (roundHandObservable = ZPropertySocket.ReceivePackageAndResponse<ZNull>(BaseUrl + "round/hand"));
        }

        public IObservable<ISocketPackage> RoundCastObservable
        {
            get => roundCastObservable ?? (roundCastObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "roundcast/#"));
        }


        public IObservable<ISocketPackage> RoundCmdObservable
        {
            get => roundCmdObservable ?? (roundCmdObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "roundcmd" + clientSuffix));
        }

        public IObservable<ISocketPackage> UnlinkSysObservable
        {
            get => unlinkSysObservable ?? (unlinkSysObservable = ZPropertySocket.ReceiveLowRawPackage(BaseUrl + "unlink/"));
        }

        //---------------    for scene manage channel
        //IObservable<ZNull> loadSceneEndObservable = null;
        //public IObservable<ZNull> LoadSceneEndObservable
        //{
        //    get => loadSceneEndObservable ?? (loadSceneEndObservable = ZPropertySocket.Receive<ZNull>(BaseUrl + "loadsceneend"));
        //}

        //IObservable<string> curSceneObservable = null;
        //public IObservable<string> CurSceneObservable
        //{
        //    get => curSceneObservable ?? (curSceneObservable = ZPropertySocket.Receive<string>(BaseUrl + "curscene"));
        //}

        //IObservable<string> loadSceneObservable = null;
        //public IObservable<string> LoadSceneObservable
        //{
        //    get => loadSceneObservable ?? (loadSceneObservable = ZPropertySocket.Receive<string>(BaseUrl + "loadscene" + clientSuffix));
        //}


        //---------------    for template action
        public IRawPackageObservable<IRawDataPref> GetTemplateObservable(string template, Type pType)
        {
            object ret = null;
            if (!actionMap.TryGetValue(template, out ret))
            {
                actionMap[template] = ZPropertySocket.ReceiveRawPackageAndResponse(BaseUrl + template + clientSuffix);    //, pType
            }
            ret = actionMap[template];
            return ret as IRawPackageObservable<IRawDataPref>; 
        }

        public IRawPackageObservable<IRawDataPref> GetTemplateObservable(string template)
        {
            object ret = null;
            if (!actionMap.TryGetValue(template, out ret))
            {
                actionMap[template] = ZPropertySocket.ReceiveRawPackageAndResponse(BaseUrl + template + clientSuffix);
            }
            ret = actionMap[template];
            return ret as IRawPackageObservable<IRawDataPref>;
        }

        public IRawPackageObservable<IRawDataPref> GetTemplateWithNoResultObservable(string template)
        {
            object ret = null;
            if (!actionMap.TryGetValue(template, out ret))
            {
                actionMap[template] = ZPropertySocket.ReceiveRawPackage(BaseUrl + template + clientSuffix);
            }
            ret = actionMap[template];
            return ret as IRawPackageObservable<IRawDataPref>;
        }

        public IRawPackageObservable<IRawDataPref> GetTemplateWithRawResultObservable(string template)
        {
            object ret = null;
            if (!actionMap.TryGetValue(template, out ret))
            {
                actionMap[template] = ZPropertySocket.ReceiveRawPackageAndResponse(BaseUrl + template + clientSuffix);    //, typeof(IRawDataPref)
            }
            ret = actionMap[template];
            return ret as IRawPackageObservable<IRawDataPref>;
        }

        public void ClearTemplate()
        {
            connectObservable = null;

            //connect2
            connect2Observable = null;

            //disconnect
            disconnectObservable = null;

            statusObservable = null;
            
            getinfoObservable = null;
            
            getactionsObservable = null;
            
            broadCastObservable = null;


            //for syncframe action
            
            syncFrameObservable = null;
            
            roundStartObservable = null;
            
            roundStopObservable = null;
            
            roundHandObservable = null;

            //for round custom action 
            
            roundCastObservable = null;

            //for systemmgr action
            
            unlinkSysObservable = null;


            //string is groupId
            joinObservable = null;
            
            uniCastObservable = null;
            
            UnjoinObservable = null;
            
            syncFrameEnumObservable = null;
            
            roundCmdObservable = null;
            
            syncFrameUpdateObservable = null;
            
            multiCastObservable = null;

            actionMap.Clear();
        }
    }
}
