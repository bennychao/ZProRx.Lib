using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UniRx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc;
using ZP.Lib.SocClient.Domain;

namespace ZP.Lib.SocClient
{
    public class RoomClientBuilder : ISocClientBuilder
    {


        List<Type> channelTypes = new List<Type>();

        private Subject<IRoomClient> onConnectedSubject = new Subject<IRoomClient>();
        private Subject<IRoomClient> onDisconnectedSubject = new Subject<IRoomClient>();

        public IAppBuilder AppBuilder { get; set; }
        public IObservable<IRoomClient> OnConnected => onConnectedSubject;

        public IObservable<IRoomClient> OnDisConnected => onDisconnectedSubject;

        public IEnumerable<Type> ChannelTypes =>  channelTypes;

        public ISocClientBuilder AddChannelType<T>() where T : BaseChannel
        {
            channelTypes.Add(typeof(T));

            return this;
        }

        public IRoomClient CreateClient(string clientId, int roomId)
        {
            var c =  new SocRoomClient(clientId, AppBuilder, (short)roomId);

            CreateInitChannels(c);

            c.Connect(roomId);

            return c;
        }

        private static bool CheckIocRoomClient(Type chType)
        {
            ConstructorInfo[] methodInfos = chType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            return methodInfos.ToList().FindIndex(a =>
            {
                return a.GetParameters().ToList<ParameterInfo>().FindIndex(p =>
                {
                    return p.ParameterType == typeof(IRoomClient);
                }) >= 0;
            }) >= 0;
        }

        private void CreateInitChannels(SocRoomClient clientSoc)
        {
            foreach (var t in ChannelTypes)
            {
                //default is room suit
                //if (!(t.GetCustomAttribute<ChannelBootAttribute>()?.Support(ChannelBootFlagEnum.RoomSuite) ?? true))
                //{
                //    continue;
                //}

                //type IOC
                if (CheckIocRoomClient(t))
                {
                    //DI the IRoomServer interface to Channel class
                    //var c = Activator.CreateInstance(t, new object[] { clientSoc }) as BaseChannel;

                    var c = ActivatorUtilities.CreateInstance(clientSoc.SubServiceProvider, t, new object[] { clientSoc }) as BaseChannel;

                    // c.BindRoom(room, clientSoc.ClientRxScheduler);
                    clientSoc.AddChannel(c);
                }
                else
                {
                    //var c = Activator.CreateInstance(t) as BaseChannel;
                    var c = ActivatorUtilities.CreateInstance(clientSoc.SubServiceProvider, t) as BaseChannel;
                    // c.BindRoom(room, clientSoc.ClientRxScheduler);
                    clientSoc.AddChannel(c);
                }
            }
        }
    }
}
