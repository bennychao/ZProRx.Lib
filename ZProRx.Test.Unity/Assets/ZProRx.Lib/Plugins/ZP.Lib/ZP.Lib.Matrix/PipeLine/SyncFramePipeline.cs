using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Soc.Pipeline
{

    public class SyncFramePipeline<TAction> :
        SyncFrameBasePipeline,
        IChannelClient,
        IActionAgentContainer<TAction>,
        IActionSponsor<TAction>,
        ISyncFrameChannel

    {

        int curFrame = 0;

        Subject<SyncFrameUpdatePackage> frameSubject = new Subject<SyncFrameUpdatePackage>();

        //Subject<string> onClientConnected = new Subject<string>();
        //Subject<string> onClientDisConnected = new Subject<string>();

        SyncFrameActionPackages<TAction> curFrameActions;

        Subject<SyncFrameActionPackage<TAction>> cmdSubject = new Subject<SyncFrameActionPackage<TAction>>();

        Dictionary<string, IActionAgent<TAction>> agents = new Dictionary<string, IActionAgent<TAction>>();


        public IObservable<SyncFrameUpdatePackage> FrameObservable => frameSubject;

        public IObservable<SyncFrameActionPackage<TAction>> ActionObservable => cmdSubject;

        //public IObservable<string> OnClientConnectedObservable => onClientConnected;
        //public IObservable<string> OnClientDisConnectedObservable => onClientDisConnected;

        public SyncFramePipeline()
        {
        }

        [Action("onframe")]
        protected ZNull OnFrame([FromPackage] SyncFrameUpdatePackage package)
        {
            curFrame = package.CurFrame.Value;

            DispatchActions();

            frameSubject.OnNext(package);

            return ZNull.Default;
        }


        [Action("syncframeenum")]
        protected ZNull OnCommand([FromPackage] SyncFrameActionPackages<TAction> rawData)
        {
            //cmdSubject.OnNext(rawData);
            curFrameActions = rawData;

            //dispacth the actions
            return ZNull.Default;
        }

        [PreListen]
        [Action("onclientconnected")]
        protected void OnClientConnected([FromPackage] string clientId)
        {
            if (TryAddClient(clientId))
            {
                var atype = this.GetType().GetCustomAttribute<AgentTypeAttribute>()?.AgentType;// ?? typeof(SyncFramePuppetBrain);

                if (ZPropertyMesh.IsPropertable(atype))
                {
                    RegisterAgent(clientId,
                        ZPropertyMesh.CreateObjectWithParam(atype, clientId) as IActionAgent<TAction>);
                }
                else if (atype != null)
                {
                    RegisterAgent(clientId,
                        Activator.CreateInstance(atype, new object[] { clientId }) as IActionAgent<TAction>);
                }
                
                OnConnected.OnNext(clientId);
            }
        }

        [PreListen]
        [Action("onclientdisconnected")]
        protected void OnClientDisConnected([FromPackage] string clientId)
        {
            if (TryDelClient(clientId))
            {
                OnDisConnected.OnNext(clientId);

                var atype = this.GetType().GetCustomAttribute<AgentTypeAttribute>()?.AgentType;// ?? typeof(SyncFramePuppetBrain);

                if (atype != null)
                {
                    UnRegisterAgent(clientId);
                }
            }
        }

        public IObservable<ZNull> PostAction<TData>(TAction action, TData data)
        {
            var url = GetBaseUrl() + "syncframeenum";

            var cmdPack = ZPropertyMesh.CreateObject<SyncFrameActionPackage<TAction>>();
            cmdPack.Action = action;
            cmdPack.SetData(data);
            cmdPack.ClientId = clientId;

            return ZPropertySocket.PostPackage<SyncFrameActionPackage<TAction>>(url, cmdPack);
        }

        public IObservable<ZNull> PostAction(SyncFrameActionPackage<TAction> data)
        {
            var url = GetBaseUrl() + "syncframeenum";

            return ZPropertySocket.PostPackage<SyncFrameActionPackage<TAction>>(url, data);
        }

        public IObservable<TResult> SendAction<TResult>(SyncFrameActionPackage<TAction> data)
        {
            var url = GetBaseUrl() + "syncframeenum";

            return ZPropertySocket.SendPackage<SyncFrameActionPackage<TAction>, TResult>(url, data);
        }

        //not used
        public new IObservable<TResult> Send<T, TResult>(string action, T data)
        {
            throw new NotImplementedException();
        }

        public IObservable<ZNull> Start()
        {
            var ret = Send<ZNull, ZNull>("syncframestart");
            ret.Subscribe();
            return ret;
        }

        public IObservable<ZNull> Stop()
        {
            var ret = Send<ZNull, ZNull>("syncframestop");
            ret.Subscribe();
            return ret;
        }

        public void RegisterAgent(string clientId, IActionAgent<TAction> agent)
        {
            agents[clientId] = agent;
        }

        public void UnRegisterAgent(string clientId)
        {
            agents[clientId] = null;
        }

        void DispatchActions()
        {
            IActionAgent<TAction> agent;
            foreach (var a in curFrameActions.Actions)
            {
                if (agents.TryGetValue(a.ClientId, out agent))
                    agent.OnAction(a.Action, a.RawData);
            }
        }
    }//end class
}
