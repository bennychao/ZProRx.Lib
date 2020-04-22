using System;
using System.Collections.Generic;
using ZP.Lib;
using UniRx;
using ZP.Lib.Soc;
using ZP.Lib.Soc.Domain;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using Microsoft.Extensions.DependencyInjection;
using ZP.Matrix.Architect.Domain;
using NUnit.Framework;
using UnityEngine;
using ZP.Lib.Core.Domain;
using System.Threading;

namespace ZP.Lib.Soc
{
    public class SocBuilding : ISocRoomBuilder
    {
        private IDisposable idleDeadlineCheckDisp = null;

        private List<Type> channelTypes = new List<Type>();

        private List<IRoomServer> roomServers = new List<IRoomServer>();

        private Subject<IRoomServer> onConnectedSubject = new Subject<IRoomServer>();
        private Subject<IRoomServer> onDisconnectedSubject = new Subject<IRoomServer>();

        private ZPropertyRefList<ZRoom> rooms = new ZPropertyRefList<ZRoom>();

        private SemaphoreSlim lockConnect = new SemaphoreSlim(1);


        protected MultiDisposable disposables = new MultiDisposable();

        public int Port { get; set; }

        public string UnitTypeStr { get; set; }

        public int ClientCount { get; set; }


        //public IConfigurationRoot Configuration { get; set; }
        public IMatrixConfig MatrixConfiguration { get; set; }

        public IEnumerable<Type> ChannelTypes
        {
            get => channelTypes;
        }

        public bool IsInBuilding
        {
            get
            {
                var schId = TaskScheduler.Current.Id;
                return roomServers.FindIndex(r => r.IsRunInRoom(schId)) >= 0;
            }
        }


        public IObservable<IRoomServer> OnConnected => onConnectedSubject;

        public IObservable<IRoomServer> OnDisConnected => onDisconnectedSubject;


        public ISocRoomBuilder AddChannelType<T>() where T : BaseChannel
        {
            channelTypes.Add(typeof(T));

            return this;
        }

        public Type FindChannelType(string name)
        {
            return channelTypes.Find(a => string.Compare(name, BaseChannel.GetChannelName(a.Name), true) == 0);
        }



        public IRoomServer AddRoom(ZRoom room)
        {
            var rooserv = new SocRoomServer(room, SocApp.Instance);

            roomServers.Add(rooserv);

            rooms.Add(room);

            //init channel
            InitChannelName(room);

            //init channels
            CreateInitChannels(room, rooserv);

            return rooserv;
        }

        public IRoomServer FindRoom(uint roomId)
        {
            return roomServers.Find(r => r.vRoomId == roomId);
        }

        public void Build()
        {
            //create tht tcp link
            ZPropertySocket.ClientPort = Port;

            //client id
            var disp = ZPropertySocket.OnConnected().Subscribe(a =>
            {

            });

            disposables.Add(disp);

            disp = ZPropertySocket.OnDisConnected().Subscribe(a =>
            {
            });

            disposables.Add(disp);

            var baseTopicBase = "matrix/" + UnitTypeStr + "/";

            foreach (var r in rooms)
            {
                var curR = r;
                //matrix/hall/connect/{vroomid}
                disp = ZPropertySocket.ReceivePackageAndResponse<SocConnectPackage, ZRoom>(
                    baseTopicBase + "connect/" + r.RoomId.ToString()
                    , null).Subscribe<SocConnectPackage, ZRoom>(async response =>
                    {
                         await ConnectRoomAsync(curR, response);

                        //return room info to client
                        return ZPropertySocket.OkResult(curR);
                    });

                disposables.Add(disp);

                //matrix/hall/disconnect/{vroomid}
                disp = ZPropertySocket.ReceivePackage<string>(
                      baseTopicBase + "disconnect/" + r.RoomId.ToString()
                      , null).Subscribe(clientId =>
                      {                         
                          DisconnectRoomAsync(curR, clientId);
                      });

                disposables.Add(disp);
            }

            ZPropertySocket.Start();
        }

        public void Unbuild()
        {
            disposables.Dispose();
            //for proce to kill

            rooms.ClearAll();

            roomServers.Clear();

            ZPropertySocket.Close();
        }

        internal async Task ConnectRoomAsync(ZRoom r, SocConnectPackage response)
        {
            if (!lockConnect.Wait(30000))
            {
                //time out
                Debug.LogWarning("ConnectRoomAsync wait mutex time out ");
                return;
            }

            Debug.LogWarning("ConnectRoomAsync clientid is " + response.ClientId);
            var server = getServer(r);

            //TODO Threadsafe
            if (!r.IsInService())
            {
                ZRoom roomData = r;

                if (!MatrixAppBuilderExtensions.bPrivateClub)
                {
                    roomData = await ZPropertyNet.Post<ZRoom>(
                          this.MatrixConfiguration.ArchitectMasterServer + "/api/v1/matrix/arch/rooms/services/" + r.RoomId,
                          null)
                        //.Catch((Exception ex) =>
                        //  {
                        //      Debug.LogError("Not Have the ArchitectMaster ");
                        //      return null;
                        //  })
                        .Retry(3)
                        .Fetch();

                    //return the clientList
                    //r.Clients = //
                    r.CopyClients(roomData);
                    r.CheckinData = roomData.CheckinData;    //use for custom room param ex. use different  Scene
                }

                r.Status = RoomStatusEnum.Occupied;
                Debug.LogWarning("ConnectRoomAsync Occupied clientid is " + response.ClientId);
                //server.AddChannel(broadcast/round/runtime) and save to zroom
                server.AddClientChannels(response.Channels); //willchange the channel list(foreach)

                //run the room service
                server.Run();

                onConnectedSubject.OnNext(server);
            }

            Assert.IsTrue(server != null && server.Status == SocRoomStatusEnum.Running, "Error: Server not running");
                       
            //[TODO]  check the user token
            //await ZPropertyNet.Get<string>(
            //      this.MatrixConfiguration.AuthorizationServer + "/check/token/" + r.RoomId + "/" + response.Token);
            //    .Subscribe(userid =>
            //    {
            //        //get userid
            //        //var userId = response.ClientId;
            //        server.AddClient(response.ClientId);

            //        //add new client bind new channel
            //        CreateClientChannel(r, server, response.ClientId);
            //    }
            //);

            //check client's channels


            //get userid
            //var userId = response.ClientId;
            server.AddClient(response.ClientId);

            //add new client bind new channel
            CreateSuiteChannel(r, server, response.ClientId);

            //reset the deadline idle check
            idleDeadlineCheckDisp?.Dispose();

            //update room info
            if (!MatrixAppBuilderExtensions.bPrivateClub)
            {
                ZPropertyNet.Put<ZRoom, RoomErrorEnum>(
                      this.MatrixConfiguration.ArchitectMasterServer + "/api/v1/matrix/arch/rooms/services/status",
                      null, r).Subscribe();
            }

            lockConnect.Release();
        }

        internal void DisconnectRoomAsync(ZRoom r, string clientId)
        {
            if (!lockConnect.Wait(30000))
            {
                //time out
                Debug.LogWarning("DisconnectRoomAsync wait mutex time out ");
                return;
            }

            var server = getServer(r);

            server.RemoveClient(clientId);

            Debug.Log("DisconnectRoomAsync clientId= " + clientId);

            if (r.IsInService() && server.ClientCount <= 0)
            {               

                Dictionary<string, string> param = new Dictionary<string, string>();
                param["workder"] = MatrixConfiguration.ServerName;

                if (!MatrixAppBuilderExtensions.bPrivateClub)
                {
                    ZPropertyNet.Delete(
                         this.MatrixConfiguration.ArchitectMasterServer + "/api/v1/matrix/arch/rooms/services/" + r.RoomId,
                         null).Subscribe(_ =>
                         {
                             StopRoomServer(r, server);
                         });
                }
                else
                {
                    StopRoomServer(r, server);
                }

                CheckIdleDeadline();
            }

            lockConnect.Release();
        }

        public IRoomServer GetCurServer()
        {
            string selfId = (TaskScheduler.Current as IZMatrixRuntime)?.RunId;

            return roomServers.Find(a
                => string.Compare(
                    (a.RoomRxScheduler as IGetMatrixRuntime)?.Runtime.RunId,
                    selfId) == 0) as SocRoomServer;
        }

        //private functions
        private void InitChannelName(ZRoom room)
        {
            foreach (var t in ChannelTypes)
            {
                if (t.GetCustomAttribute<ChannelBootAttribute>()?.Support(
                    ChannelBootFlagEnum.ManualCreate) ?? false)
                {
                    continue;
                }

                room.AddChannel(t);
            }
        }

        private void CreateInitChannels(ZRoom room, SocRoomServer rooserv)
        {
            foreach (var t in ChannelTypes)
            {
                //default is room suit
                if (t.GetCustomAttribute<ChannelBootAttribute>()?.Support(
                    ChannelBootFlagEnum.ClientSuite |
                    ChannelBootFlagEnum.ManualCreate) ?? false)
                {
                    continue;
                }

                //type IOC
                if (CheckIocRoomServer(t))
                {
                    //DI the IRoomServer interface to Channel class
                    //var c = Activator.CreateInstance(t, new object[] { rooserv }) as BaseChannel;

                    var c = ActivatorUtilities.CreateInstance(rooserv.SubServiceProvider, t, new object[] { rooserv }) as BaseChannel;

                    c.BindRoom(room, rooserv.RoomRxScheduler);
                    rooserv.AddChannel(c);
                }
                else
                {
                    //ActivatorUtilities TODO
                    //var c = Activator.CreateInstance(t) as BaseChannel;

                    var c = ActivatorUtilities.CreateInstance(rooserv.SubServiceProvider, t) as BaseChannel;

                    c.BindRoom(room, rooserv.RoomRxScheduler);
                    rooserv.AddChannel(c);
                }

            }
        }

        private void CreateSuiteChannel(ZRoom room, SocRoomServer rooserv, string cid)
        {
            foreach (var t in ChannelTypes)
            {
                //default is room suit
                if (!(t.GetCustomAttribute<ChannelBootAttribute>()?.Support(ChannelBootFlagEnum.ClientSuite) ?? true))
                {
                    continue;
                }

                //type IOC
                if (CheckIocRoomServer(t))
                {
                    //DI the IRoomServer interface to Channel class
                    //var c = Activator.CreateInstance(t, new object[] { rooserv }) as BaseChannel;
                    var c = ActivatorUtilities.CreateInstance(rooserv.SubServiceProvider, t, new object[] { rooserv }) as BaseChannel;

                    c.BindRoom(room, rooserv.RoomRxScheduler);
                    c.BuildClient(cid);

                    rooserv.AddChannel(c);
                    c.Listen();
                }
                else
                {
                    //var c = Activator.CreateInstance(t) as BaseChannel;

                    var c = ActivatorUtilities.CreateInstance(rooserv.SubServiceProvider, t) as BaseChannel;

                    c.BindRoom(room, rooserv.RoomRxScheduler);

                    c.BuildClient(cid);

                    rooserv.AddChannel(c);
                    c.Listen();
                }

            }
        }

        private static bool CheckIocRoomServer(Type chType)
        {
            ConstructorInfo[] methodInfos = chType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            return methodInfos.ToList().FindIndex(a =>
            {
                return a.GetParameters().ToList<ParameterInfo>().FindIndex(p =>
                {
                    return p.ParameterType == typeof(IRoomServer);
                }) >= 0;
            }) >= 0;
        }

        private void StopRoomServer(ZRoom r, SocRoomServer server)
        {
            r.Status = RoomStatusEnum.Unused;
            onDisconnectedSubject.OnNext(server);

            server.Stop();
        }

        //TODO
        private void CheckIdleDeadline()
        {
            var runningCount = roomServers.Select(a => a as SocRoomServer).Count(a => a.Status == SocRoomStatusEnum.Running);
            if (runningCount <= 0)
            {
                idleDeadlineCheckDisp = Observable.Timer(TimeSpan.FromSeconds(MatrixConfiguration.IdleDeadline)).Subscribe(_ =>
                {
                    // stop App;
                    SocApp.Instance.Stop();
                });
            }
        }

        private SocRoomServer getServer(ZRoom zr)
        {
            return roomServers.Find(a => a.vRoomId == zr.RoomId) as SocRoomServer;
        }
    }
}
