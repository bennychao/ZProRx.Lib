using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZP.Lib;
using ZP.Lib.Soc;
using UniRx;
using ZP.Lib.CoreEx;
using ZP.Lib.Soc.Domain;
using System.Threading;
using ZP.Lib.Soc.Entity;
using System.Linq;
using UnityEngine;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Reflection;

namespace ZP.Lib.Soc
{
    internal class SocRoomServer : IRoomServer
    {
        private ZRoom zRoom;
        private List<string> clientIds = new List<string>();

        private List<BaseChannel> channels = new List<BaseChannel>();

        private RoomTaskScheduler roomTaskScheduler = null;
        private RoomRxScheduler roomRxScheduler = null;


        private ISocAppBuilder socAppBuilder = null;

        private IServiceProvider subServiceProvider = null;

        private CancellationTokenSource tokenSource = null;// new CancellationTokenSource();

        private Subject<string> OnArrivedSubject = new Subject<string>();
        private Subject<string> OnLeaveSubject = new Subject<string>();

        //new RoomTaskScheduler(30, "RoomServer");

        private Subject<Unit> stopSubject = new Subject<Unit>();
        private Subject<Unit> runSubject = new Subject<Unit>();

        private int innerStatus = 0;

        //property
        public IServiceProvider SubServiceProvider => subServiceProvider;

        public IScheduler RoomRxScheduler => roomRxScheduler;

        public int vRoomId => zRoom.RoomId;

        public int ClientCount => (int)clientIds?.Count;

        public IEnumerable<string> Clients => clientIds;


        public IObservable<string> OnArrivedObservable => OnArrivedSubject;

        public IObservable<string> OnLeaveObservable => OnLeaveSubject;


        public IObservable<Unit> OnRunObservable => runSubject;

        public IObservable<Unit> OnStopObservable => stopSubject;

        public RoomStatusEnum RoomStatus => this.zRoom.Status;


        internal SocRoomStatusEnum Status => (SocRoomStatusEnum)innerStatus; // SocRoomStatusEnum.Idle;

        public SocRoomServer(ZRoom zRoom, ISocAppBuilder socAppBuilder)
        {
            this.zRoom = zRoom;
            this.roomTaskScheduler = new RoomTaskScheduler(100, $"RoomServer{zRoom.RoomId}", (short)zRoom.RoomId);
            this.socAppBuilder = socAppBuilder;
            roomRxScheduler = new RoomRxScheduler(roomTaskScheduler);
            subServiceProvider = socAppBuilder.BuildSubServiceProvider();
        }

        public void AddChannel(BaseChannel channel)
        {
            channels.Add(channel);

            //channel.Listen();
        }

        public void AddClientChannel(ZChannel zChannel)
        {
            zChannel.ChangeDir();

            zRoom.AddChannel(zChannel);
            //channel.Listen();
        }

        public void AddClientChannels(List<ZChannel> zChannels)
        {
            foreach (var c in zChannels)
            {
                if (zRoom.HasChannel(c))
                    continue;

                c.ChangeDir();

                zRoom.AddChannel(c);

                //check 
                if (c.IsBroadCast && !CheckChannel(c.Name))         //have not custom channel
                {
                    BroadCastChannel broad = new BroadCastChannel(this, c.Name);
                    var cc = ZPropertyMesh.CloneObject(c) as ZChannel;
                    cc.ChangeDir();

                    zRoom.AddChannel(cc);

                    broad.BindRoom(zRoom, RoomRxScheduler);

                    AddChannel(broad);
                }
                else if (c.IsMultiCast && !CheckChannel(c.Name))         //have not custom channel
                {
                    MultiCastChannel broad = new MultiCastChannel(this, c.Name, socAppBuilder.Configuration);
                    var cc = ZPropertyMesh.CloneObject(c) as ZChannel;
                    cc.ChangeDir();

                    zRoom.AddChannel(cc);

                    broad.BindRoom(zRoom, RoomRxScheduler);

                    AddChannel(broad);
                }

                else if (c.IsUniCast && !CheckChannel(c.Name))         //have not custom channel
                {
                    UniCastChannel broad = new UniCastChannel(this, c.Name);

                    var cc = ZPropertyMesh.CloneObject(c) as ZChannel;
                    cc.ChangeDir();

                    zRoom.AddChannel(cc);

                    broad.BindRoom(zRoom, RoomRxScheduler);

                    AddChannel(broad);
                }

                else if (c.IsRound && !CheckChannel(c.Name))         //only supportcustom channel
                {
                    //RoundChannel broad = 
                    //delete by zb [2019.08.14] can't get type form other assembly
                    //var typeName = c.Name + "Channel";

                    var ctype = GetChannelType(c.Name);

                    var broad = Activator.CreateInstance(ctype,
                        new object[] { this, c.Name.Value, socAppBuilder.Configuration }) as ServerChannel;

                    //if (broad == null)
                    //    broad = new RoundChannel(this, c.Name, socAppBuilder.Configuration);


                    var cc = ZPropertyMesh.CloneObject(c) as ZChannel;
                    cc.ChangeDir();

                    zRoom.AddChannel(cc);

                    broad.BindRoom(zRoom, RoomRxScheduler);

                    AddChannel(broad);
                }

                else if (c.IsSyncFrame && !CheckChannel(c.Name))         //only support custom channel
                {
                    //var typeName = c.Name + "Channel";

                    var ctype = GetChannelType(c.Name);

                    var broad = Activator.CreateInstance(ctype, //Type.GetType(typeName),
                        new object[] { this , c.Name.Value, socAppBuilder.Configuration }) as ServerChannel;

                    var cc = ZPropertyMesh.CloneObject(c) as ZChannel;
                    cc.ChangeDir();

                    zRoom.AddChannel(cc);

                    broad.BindRoom(zRoom, RoomRxScheduler);

                    AddChannel(broad);
                }

                else if (c.IsSceneMgr && !CheckChannel(c.Name))        
                {
                    //var typeName = c.Name + "Channel";

                    var ctype = GetChannelType(c.Name);
                    

                    ServerChannel broad = null;
                    if (ctype != null)
                    {
                        broad = Activator.CreateInstance(ctype, //Type.GetType(typeName),
                            new object[] { this, c.Name.Value, socAppBuilder.Configuration }) as ServerChannel;
                    }
                    else
                    {
                        broad = new SceneManageChannel(this, c.Name.Value, socAppBuilder.Configuration);
                    }


                    var cc = ZPropertyMesh.CloneObject(c) as ZChannel;
                    cc.ChangeDir();

                    zRoom.AddChannel(cc);

                    broad.BindRoom(zRoom, RoomRxScheduler);

                    AddChannel(broad);
                }

                else if (c.IsPipeline && !CheckChannel(c.Name))         //only support custom channel
                {
                    //var typeName = c.Name + "Channel";

                    var ctype = GetChannelType(c.Name);

                    //[TODO] Soc/Devlog.md/BugList 10
                    if (ctype == null)
                    {
                        ctype = this.GetType().GetCustomAttribute<PipelineDefaultServerChannelTypeAttribute>()?.DefaultType;
                    }

                    Assert.IsNotNull(ctype, $"Create Pipeline Error {c.Name} Not Define the same name's ServerChannel to Linked With ClientPipeline");

                    var pipeServer = ActivatorUtilities.CreateInstance(SubServiceProvider, ctype, new object[] { this, c.Name.Value }) as BaseChannel;
                    // var pipeServer = Activator.CreateInstance(ctype, //Type.GetType(typeName),
                    //     new object[] { this, c.Name.Value, socAppBuilder.Configuration }) as ServerChannel;

                    var cc = ZPropertyMesh.CloneObject(c) as ZChannel;
                    cc.ChangeDir();

                    zRoom.AddChannel(cc);

                    pipeServer.BindRoom(zRoom, RoomRxScheduler);

                    AddChannel(pipeServer);
                }
            }

            //channel.Listen();
        }

        public IChannelClient GetChannelClient(string channelName)
        {
            var c = zRoom.Channels.FindValue(a => a.Name.Value.CompareTo(channelName) == 0 && a.IsIn);
            if (c == null)
                return null;

            var cc = new ChannelClient(c, RoomRxScheduler);

            return cc;
        }

        public IChannelClient GetChannelClient(string channelName, string clientId)
        {
            var c = zRoom.Channels.FindValue(a => a.Name.Value.CompareTo(channelName) == 0 && a.IsIn);
            if (c == null)
                return null;

            if (!IsClientInRoom(clientId))
                throw new Exception("Client Not In Room");

            var cc = new SocChannelClient(c, this, clientId);

            return cc;
        }

        public BaseChannel GetChannel(string channelName)
        {
            return channels.Find(a => string.Compare(a.ChannelName, channelName, true) == 0);
        }

        public TChannel GetChannel<TChannel>() where TChannel : BaseChannel
        {
            return channels.Find(a => typeof(TChannel).IsAssignableFrom(a.GetType())) as TChannel;
        }

        public IEnumerable<TChannel> GetChannels<TChannel>() where TChannel : BaseChannel
        {
            var rets = channels.FindAll(a => typeof(TChannel).IsAssignableFrom(a.GetType()));
            return rets.Select(a => a as TChannel);
        }

        //public  ICannelClient GetVisualChannelClient(string channelName)
        //{
        //    var c = zRoom.Channels.FindValue(a => a.Name.Value.CompareTo(channelName) == 0 && a.IsIn);
        //    if (c == null)
        //        return null;

        //    var cc = new LocalChannelClient(c, this);

        //    return cc;
        //}

        public object GetService(Type serviceType)
        {
            return subServiceProvider.GetService(serviceType);
        }

        public T GetService<T>()
        {
            return (T)subServiceProvider.GetService(typeof(T));
        }


        public bool CheckChannel(string channelName)
        {
            return zRoom?.HasChannelDefined(channelName) ?? false;
        }


        IObservable<ZNull> IRoomServer.SendPackage<T>(string action, T data)
        {
            List<IObservable<ZNull>> rets = new List<IObservable<ZNull>>();
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action);

            //var returnClientCount = new ReactiveProperty<int>(0);
            if (Clients.Count() <= 0)
                return Observable.Return<ZNull>(ZNull.Default) ;

            foreach (var c in Clients)
            {
                //send and with result
                var sendObservable = ZPropertySocket.SendPackage<T, ZNull>(url + "/" + c, data);

                rets.Add(sendObservable); 
            }

           return Observable.Merge(rets).Buffer(Clients.Count()).Select(_ => ZNull.Default);

            // TODO When All not work it only return when all observer is stopped
            //return Observable.WhenAll(rets).Select(_=>
            //{
            //   return  ZNull.Default;
            //});

            //return returnClientCount.Where(a => a >= Clients.Count()).Select(_ => ZNull.Default);
        }

        IObservable<ZNull> IRoomServer.SendPackageEx<T>(string excludeClientId, string action, T data)
        {
            List<IObservable<ZNull>> rets = new List<IObservable<ZNull>>();
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action);
            if (Clients.Count() <= 0)
                return Observable.Return<ZNull>(ZNull.Default);

            int sendCount = 0;
            foreach (var c in Clients)
            {
                if (string.Compare(excludeClientId, c, true) == 0)
                    continue;

                sendCount++;
                var sendObservable = ZPropertySocket.SendPackage<T, ZNull>(url + "/" + c, data);
                rets.Add(sendObservable);
            }

            if (sendCount <= 0)
                return Observable.Return<ZNull>(ZNull.Default);

            //return Observable.Concat(rets);
            //return Observable.WhenAll(rets).Select(_ => ZNull.Default);
            return Observable.Merge(rets).Buffer(sendCount).Select(_ => ZNull.Default);
        }

        IObservable<ZNull> IRoomServer.SendPackageTo<T>(string clientId, string action, T data)
        {
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action, clientId);

            return ZPropertySocket.SendPackage<T, ZNull>(url, data);
        }

        IObservable<TResult> IRoomServer.SendPackageTo<T, TResult>(string clientId, string action, T data)
        {
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action, clientId);

            return ZPropertySocket.SendPackage<T, TResult>(url, data);
        }

        void IRoomServer.BroadCastPackage<T>(string action, T data)
        {
            //List<IObservable<ZNull>> rets = new List<IObservable<ZNull>>();
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action);
            if (Clients.Count() <= 0)
                return;
            foreach (var c in Clients)
            {
                ZPropertySocket.PostPackage<T>(url + "/" + c, data).Subscribe();
                //rets.Add(sendObservable);
            }

            //return Observable.Merge(rets);
        }

        void IRoomServer.BroadCastPackageEx<T>(string excludeClientId, string action, T data)
        {
            //List<IObservable<ZNull>> rets = new List<IObservable<ZNull>>();
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action);

            if (Clients.Count() <= 0)
                return;

            foreach (var c in Clients)
            {
                if (string.Compare(excludeClientId, c, true) == 0)
                    continue;

                ZPropertySocket.PostPackage<T>(url + "/" + c, data).Subscribe();
                //rets.Add(sendObservable);
            }

            //return Observable.Merge(rets);
        }

        void IRoomServer.BroadCastPackageTo<T>(string clientId, string action, T data)
        {
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action, clientId);

            ZPropertySocket.PostPackage<T>(url, data).Subscribe();
        }

        void IRoomServer.BroadCastRawData(string action, string data)
        {
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action);

            if (Clients.Count() <= 0)
                return;

            foreach (var c in Clients)
            {
                ZPropertySocket.Post(url + "/" + c, data).Subscribe();
            }
        }

        void IRoomServer.BroadCastRawDataEx(string excludeClientId, string action, string data)
        {
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action);

            if (Clients.Count() <= 0)
                return;

            foreach (var c in Clients)
            {
                if (string.Compare(excludeClientId, c, true) == 0)
                    continue;

                ZPropertySocket.Post(url + "/" + c, data).Subscribe();
            }
        }

        void IRoomServer.BroadCastRawDataTo(string clientId, string action, string data)
        {
            var url = ZPropertySocket.AssembleUrl(zRoom.Url, action, clientId);

            ZPropertySocket.Post(url, data).Subscribe();
        }

        public void AddClient(string clientId)
        {
            lock (this)
                clientIds.Add(clientId);

            //add channel
            OnArrivedSubject.OnNext(clientId);
        }

        public void RemoveClient(string clientId)
        {
            lock (this)
                clientIds.Remove(clientId);

            OnLeaveSubject.OnNext(clientId);
        }



        public bool IsRunInRoom(int schedulerId)
        {
            return roomTaskScheduler.Id == schedulerId;
        }

        public bool IsRunInRoom()
        {
            return roomTaskScheduler.Id == TaskScheduler.Current.Id;
        }

        public bool IsClientInRoom(string clientId)
        {
            return Clients.ToList().Contains(clientId);
        }


        public Task RunInRoom(Action action)
        {
            var task = new Task(() =>
            {
                action();
            });
            task.Start(roomTaskScheduler);
            return task;
        }

        public Task RunInRoom(Func<Task> function)
        {
            var task = new Task(async () =>
            {
                await function();
            });
            task.Start(roomTaskScheduler);
            return task;
        }

        //one room one thread
        internal void Run()
        {
            //1 is running
            if (Interlocked.Exchange(ref innerStatus, (int)SocRoomStatusEnum.Running) != 0)
            {
                return;
            }

            tokenSource = new CancellationTokenSource();
            //create new thread to run
            var task = new Task(async () =>
            {
               //flag
               Debug.LogWarning("Room Run In Task Scheduler");

                foreach (var c in channels)
                {
                    c.Listen();
                }

                runSubject.OnNext(Unit.Default);

                //wait msg
                await stopSubject.Fetch();

                Debug.LogWarning("Room Stop In Task Scheduler");
            }, tokenSource.Token);

            try
            {
                task.Start(roomTaskScheduler);
            }
            catch(Exception ex)
            {
                Debug.Log("Run Error!! " + ex.ToString());
            }


            //Status = SocRoomStatusEnum.Running;
        }

        public void Stop()
        {
            if (Interlocked.Exchange(ref innerStatus, (int)SocRoomStatusEnum.Idle) == 0)
            {
                return;
            }

            Debug.LogWarning("Room Stop Bye!!!");
            foreach (var c in channels)
            {
                c.Suspend();
            }

            stopSubject.OnNext(Unit.Default);
            //stopSubject.OnCompleted();

            tokenSource.Cancel();
            //Status = SocRoomStatusEnum.Idle;

        }

        protected Type GetChannelType(string channelName)
        {
            var builder = socAppBuilder.GetService<ISocRoomBuilder>() as SocBuilding;

            if (builder == null)
                throw new Exception("No Room Builder be Registered");

            //builder.ChannelTypes.
            return builder.FindChannelType(channelName);
        }

    }
}
