using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Soc;
using ZP.Lib.SocClient.Domain;
using ZP.Lib.SocClient;
using ZP.Lib.CoreEx;
using UniRx;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.SocClient
{
    internal class SocRoomClient : IRoomClient
    {
        private ZRoom zRoom;
        private string clientId;

        private volatile int roomStatus = 0; //not running

        private SocClientTaskScheduler clientTaskScheduler = null;// new SocClientTaskScheduler(10, "RoomServer");

        private SocClientRxScheduler clientRxScheduler = null;

        public IScheduler ClientRxScheduler => clientRxScheduler;

        public ZRoom ZRoom => zRoom;

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        //SocClientChannelListener channelListener;

        private List<BaseChannel> channels = new List<BaseChannel>();
        private Subject<Unit> stopSubject = new Subject<Unit>();
        private Subject<Unit> runSubject = new Subject<Unit>();

        IServiceProvider subServiceProvider;

        public IServiceProvider SubServiceProvider => subServiceProvider;

        public IObservable<Unit> OnRunObservable => runSubject;

        public IObservable<Unit> OnStopObservable => stopSubject;

        public SocRoomClient(string clientId, IAppBuilder appBuilder, short roomId)
        {
            // this.zRoom = zRoom;
            this.clientId = clientId;
            clientTaskScheduler = new SocClientTaskScheduler(10, clientId, roomId);
            clientRxScheduler = new SocClientRxScheduler(clientTaskScheduler);

            subServiceProvider =  appBuilder.BuildSubServiceProvider();
        }

        public void AddChannel(BaseChannel channel)
        {
            channels.Add(channel);
        }

        //connect to room
        public void Connect(int roomId)
        {
            if (Interlocked.Exchange(ref roomStatus, 1) != 0)
            {
                //has run
                return;
            }

            if (!ZPropertySocket.IsConnected())
                ZPropertySocket.Start();

            var request = ZPropertyMesh.CreateObject<SocConnectPackage>();
            request.SetData(clientId, "23312312312");

            foreach (var t in channels)
            {
                request.AddChannel(t.GetType(), roomId);
            }


            RunInClient(() =>
            {
                //send connect room topic to check in
                ZPropertySocket.SendPackage<SocConnectPackage, ZRoom>(MatrixRoomTopicDefines.RoomConnect(roomId), request)
                .Subscribe(room =>
                {
                    Debug.Log("Join Room Id " + roomId.ToString() + " and Client " + clientId);
                    zRoom = room;

                    //reverse thd change direction
                    zRoom.ChangeDir();

                    //add my channels
                    //channels.Select(c => c.GetType()).ToList().ForEach(a => {
                           
                    //        zRoom.AddChannel(a);
                    //    });

                    //init channels
                    BindChannels();

                    Run();
                }, 
                error =>
                {
                    Interlocked.Exchange(ref roomStatus, 0);
                });
            });

        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref roomStatus, 0) != 1)
            {
                //has stop
                return;
            }

            Debug.Log("Request Leave Room clientId =" + clientId);

            ZPropertySocket.PostPackage<string>(MatrixRoomTopicDefines.RoomDisConnect(zRoom.RoomId), clientId).Subscribe(_ =>
            {
                Debug.Log("Leave Room End clientId =" + clientId);

                //ZPropertySocket.Close();

                Stop();
            });
        }


        public bool CheckChannel(string channelName)
        {
            return zRoom?.HasChannelDefined(channelName) ?? false;
        }

        public IChannelClient GetChannelClient(string channelName, string clientId)
        {
            var c = zRoom.Channels.FindValue(a => a.Name.Value.CompareTo(channelName) == 0 && a.IsIn);
            if (c == null)
                return null;

            var cc = new ChannelClient(c, ClientRxScheduler);

            return cc;
        }

        public IChannelClient GetChannelClient(string channelName)
        {
            var c = zRoom.Channels.FindValue(a => a.Name.Value.CompareTo(channelName) == 0 && a.IsIn);
            if (c == null)
                return null;

            var cc = new ChannelClient(c, ClientRxScheduler);

            return cc;
        }

        public BaseChannel GetChannel(string channelName)
        {
            return channels.Find(a => string.Compare(a.ChannelName, channelName, true) == 0);
        }

        //return defined channel in runtime by type
        public TChannel GetChannel<TChannel>() where TChannel : BaseChannel
        {
            return channels.Find(a => typeof(TChannel).IsAssignableFrom(a.GetType())) as TChannel;
        }

        public IEnumerable<TChannel> GetChannels<TChannel>() where TChannel : BaseChannel
        {
            var rets = channels.FindAll(a => typeof(TChannel).IsAssignableFrom(a.GetType()));
            return rets.Select(a => a as TChannel);
        }

        public IChannelClient GetGroupChannelClient(string channelName)
        {
            throw new NotImplementedException();
        }

        public IChannelClient GetUniChannelClient(string channelName)
        {
            throw new NotImplementedException();
        }

        public bool IsRunInClient(int schedulerId)
        {
            return clientTaskScheduler.Id == schedulerId;
        }

        public bool IsRunInClient()
        {
            return clientTaskScheduler.Id == TaskScheduler.Current.Id;
        }

        public Task RunInClient(Action action)
        {
            var task = new Task(() =>
            {
                action();
            });
            task.Start(clientTaskScheduler);
            return task;
        }

        public Task RunInClient(Func<Task> function)
        {
            var task = new Task(async () =>
            {
                await function();
            });
            task.Start(clientTaskScheduler);
            return task;
        }

        //implement IRoomClient
        public void SendPackage<T>(string action, T data)
        {
            throw new NotImplementedException();
        }

        //implement IRoomClient
        public void PostPackage<T>(string action, T data)
        {
            throw new NotImplementedException();
        }

        void BindChannels()
        {
            foreach (var c in channels)
            {
                c.BindRoom(zRoom, ClientRxScheduler);
            }
        }

        void RunChannels()
        {
            foreach (var c in channels)
            {
                c.Listen();
            }
        }

        internal void Run()
        {
            //create new thread to run
            var task = new Task(async () =>
            {
                //to listen
                RunChannels();

                //flag 
                runSubject.OnNext(Unit.Default);

                //wait msg
                await stopSubject.ToTask();

            }, tokenSource.Token);

            task.Start(clientTaskScheduler);
        }

        public void Stop()
        {
            foreach (var c in channels)
            {
                c.Suspend();
            }

            stopSubject.OnNext(Unit.Default);
            stopSubject.OnCompleted();

            tokenSource.Cancel();
        }


        //public void AddChannel(BaseChannel channel)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
