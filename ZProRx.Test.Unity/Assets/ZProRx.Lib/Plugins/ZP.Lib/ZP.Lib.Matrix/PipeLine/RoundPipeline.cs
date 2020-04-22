using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix
{
    [ChannelBoot(ChannelBootFlagEnum.Round)]
    public class RoundBasePipeline : BasePipeline
    {

    }


    public class RoundPipeline<TCmd> :
        RoundBasePipeline,
        IActionAgentContainer<TCmd>,
        IActionSponsor<TCmd>,
        IChannelClient,
        IRoundPipeline
    {
        private int curRound = 0;

        private string curHandClientId;

        private Dictionary<string, IActionAgent<TCmd>> agents = new Dictionary<string, IActionAgent<TCmd>>();

        private Subject<ZNull> startSubject = new Subject<ZNull>();

        private Subject<ZNull> stopSubject = new Subject<ZNull>();

        private Subject<ZNull> readySubject = new Subject<ZNull>();

        private Subject<CmdPackage<TCmd>> cmdSubject = new Subject<CmdPackage<TCmd>>();

        private Subject<RoundPackage> roundSubject = new Subject<RoundPackage>();

        private Subject<RoundPackage> tickSubject = new Subject<RoundPackage>();

        private Subject<int> onMyHandSubject = new Subject<int>();

        //public attribute
        public int CurRound => curRound;
        
        //public List<string> Clients => clientIds;

        public IObservable<CmdPackage<TCmd>> OnCmdObservable => cmdSubject.ObserveOn(innerScheder);

        public IObservable<RoundPackage> OnRoundObservable => roundSubject.ObserveOn(innerScheder);

        public IObservable<RoundPackage> OnTickObservable => tickSubject.ObserveOn(innerScheder);

        public IObservable<int> OnHandObservable => onMyHandSubject.ObserveOn(innerScheder);

        public IObservable<ZNull> OnReadyObservable => readySubject.ObserveOn(innerScheder);

        public IObservable<ZNull> OnStartObservable => startSubject.ObserveOn(innerScheder);

        public IObservable<ZNull> OnStopObservable => stopSubject.ObserveOn(innerScheder);
        
               

        public IObservable<ZNull> Ready()
        {
            readySubject.OnNext(ZNull.Default);
            return Observable.Return<ZNull>(ZNull.Default);
        }

        public IObservable<ZNull> Start()
        {
            //to Check the Agent
            //must done before Start
            CheckAgents();

            //multi subscribe error
            var obser = Send<ZNull, ZNull>("roundstart").Do(_ => startSubject.OnNext(ZNull.Default)).ObserveOn(innerScheder);
            //[Sync Call] Will Return When All Client Call Start

            return obser;
        }

        //call by client, need all client check thd round is end
        public IObservable<ZNull> Stop()
        {
            var obser = Send<ZNull, ZNull>("roundstop").Do(_ => stopSubject.OnNext(ZNull.Default)).ObserveOn(innerScheder);
            //obser.Subscribe(_ => stopSubject.OnNext(Unit.Default));
            return obser;
        }

        public IObservable<ZNull> Hand()
        {
            var obser = Send<ZNull, ZNull>("round/hand").ObserveOn(innerScheder);
            //obser.Subscribe();
            return obser;

            //Send<ZNull, ZNull>("round/hand").Subscribe();
        }

        public IObservable<ZNull> PostAction<TData>(TCmd cmd, TData data)
        {
            var url = GetBaseUrl() + "roundcmd";

            var cmdPack = ZPropertyMesh.CreateObjectWithParam<CmdPackage<TCmd>>(new object[] { cmd });

            cmdPack.SetData(data);
            cmdPack.Organizer = clientId;

            var ret = ZPropertySocket.PostPackage<CmdPackage<TCmd>>(url, cmdPack).Do(_ => DispatchActions(cmdPack));

            //send to myself
            //ret.Subscribe(_ => {
            //    DispatchActions(cmdPack);
            //});

            return ret;
        }

        public IObservable<ZNull> PostAction(CmdPackage<TCmd> data)
        {
            var url = GetBaseUrl() + "roundcmd";

            var ret = ZPropertySocket.PostPackage<CmdPackage<TCmd>>(url, data).Do(_ => DispatchActions(data));

            return ret;
        }

        public IObservable<TResult> SendCmd<TResult>(CmdPackage<TCmd> data)
        {
            var url = GetBaseUrl() + "roundcmd";

            var ret = ZPropertySocket.SendPackage<CmdPackage<TCmd>, TResult>(url, data).Do(_ => DispatchActions(data));

            //send to myself
            //ret.Subscribe(_ => DispatchActions(data));

            return ret;
        }


        public IObservable<CmdPackage<TCmd>> GetCmdObservable(string clientId)
            => cmdSubject.Where(c => string.Compare(c.Organizer, clientId) == 0);


        static public CmdPackage<TOtherCmd> DecodeCmd<TOtherCmd>(IRawDataPref rawData)
        {
            var cmd = ZPropertyMesh.CreateObject<CmdPackage<TOtherCmd>>();

            ZPropertyPrefs.LoadFromRawData(cmd, rawData);

            return cmd;
        }


        public void RegisterAgent(string clientId, IActionAgent<TCmd> agent)
        {
            if (agents.ContainsKey(clientId))
                return;

            agents[clientId] = agent;
        }

        public void UnRegisterAgent(string clientId)
        {
            agents[clientId] = null;
        }

        //prelisten actions
        [PreListen]
        [Action("onclientconnected")]
        protected ZNull OnClientConnected([FromPackage] string clientId)
        {
            if (TryAddClient(clientId))
            {
                var atype = this.GetType().GetCustomAttribute<AgentTypeAttribute>()?.AgentType;// ?? typeof(RoundPuppetBrain);

                var cid = Convert.ToInt32(clientId);
                if (ZPropertyMesh.IsPropertable(atype))
                {
                    RegisterAgent(clientId,
                   ZPropertyMesh.CreateObjectWithParam(atype, cid) as IActionAgent<TCmd>);
                }
                else if (atype != null)
                {
                    RegisterAgent(clientId,
                        Activator.CreateInstance(atype, new object[] { cid }) as IActionAgent<TCmd>);
                }

                OnConnected.OnNext(clientId);
            }

            return ZNull.Default;
        }


        [PreListen]
        [Action("onclientdisconnected")]
        protected ZNull OnClientDisConnected([FromPackage] string clientId)
        {
            if (TryDelClient(clientId))
            {
                OnDisConnected.OnNext(clientId);

                var atype = this.GetType().GetCustomAttribute<AgentTypeAttribute>()?.AgentType;// ?? typeof(RoundPuppetBrain);

                if (atype != null)
                {
                    UnRegisterAgent(clientId);
                }
            }
            //or throw a exception
            return ZNull.Default;
        }


        //actions
        [Action("oninnerready")]
        protected void OnInnerReady()
        {

#if !ZP_UNITY_CLIENT
            //for server's pipeline to set ready
            Ready();
#endif
        }

        /// <summary>
        /// Return async
        /// </summary>
        /// <param name="package"></param>
        /// <returns>ZNull</returns>
        [Action("ontick")]
        protected ZNull OnTick([FromPackage] RoundPackage package)
        {
            if (curRound != package.CurRound)
            {
                curRound = package.CurRound;
                roundSubject.OnNext(package);
            }

            curHandClientId = package.CurClientId;

            tickSubject.OnNext(package);

            if (string.Compare(clientId, curHandClientId) == 0)
            {
                onMyHandSubject.OnNext(package.CurRound.Value);
            }

            Debug.Log($"RoundPipeline[{clientId}] : curRound[{curRound}] curCoundClient is [{curHandClientId}] ");

            return ZNull.Default;
        }

        [Action("roundcmdclient")]
        protected void OnCommand([FromPackage] CmdPackage<TCmd> rawData)
        {
            cmdSubject.OnNext(rawData);

            DispatchActions(rawData);
        }

        // call by server while round reach max
        [Action("round/onstop")]
        protected ZNull OnStop()
        {
            stopSubject.OnNext(ZNull.Default);
            return ZNull.Default;
        }


        //protected functions
        protected override void OnOpened()
        {
            base.OnOpened();
        }


        protected void CheckAgents()
        {
            var atype = this.GetType().GetCustomAttribute<AgentTypeAttribute>()?.AgentType;// ?? typeof(RoundPuppetBrain);

            foreach (var c in clientIds)
            {
                if (agents.ContainsKey(c))
                    continue;

                var cid = Convert.ToInt32(c);
                if (ZPropertyMesh.IsPropertable(atype))
                {
                    RegisterAgent(c,
                   ZPropertyMesh.CreateObjectWithParam(atype, cid) as IActionAgent<TCmd>);
                }
                else if (atype != null)
                {
                    RegisterAgent(c,
                        Activator.CreateInstance(atype, new object[] { cid }) as IActionAgent<TCmd>);
                }
            }
        }


        /// <summary>
        /// send to self
        /// </summary>
        /// <param name="data"></param>
        private void DispatchActions(CmdPackage<TCmd> data)
        {
            //&& string.Compare(data.Organizer, agent.
            IActionAgent<TCmd> agent;
            if (agents.TryGetValue(data.Organizer, out agent)
                )
                agent.OnAction(data.Cmd, data.RawData);
        }

    }
}
