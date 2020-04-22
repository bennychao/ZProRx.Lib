using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ZP.Lib;
using ZP.Lib.Net;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix.Entity
{
    sealed public class ZRoom : IIndexable
    {
        public static string RoomTag = "matrix/hall/";

        private ZProperty<int> roomId = new ZProperty<int>();

        private ZProperty<ZEndPoint> endPoint = new ZProperty<ZEndPoint>();

        private ZProperty<string> mainTopic = new ZProperty<string>();

        private ZProperty<int> regionId = new ZProperty<int>();

        private ZProperty<string> token = new ZProperty<string>();

        private ZPropertyList<ZChannel> channels = new ZPropertyList<ZChannel>();

        private ZProperty<RoomStatusEnum> status = new ZProperty<RoomStatusEnum>();

        private ZPropertyList<ZRoomRole> clients = new ZPropertyList<ZRoomRole>();

        private ZProperty<string> checkInData = new ZProperty<string>();

        public int Index {
            set => roomId.Value = value;
            get => roomId.Value;
        }


        public int RoomId {
            get => roomId.Value;
            set => roomId.Value = value;
        }

        public string Host

        {
            get => endPoint.Value.IP;
            set => endPoint.Value.IP = value;
        }

        public int Port
        {
            get => endPoint.Value.Port;
            set => endPoint.Value.Port = value;
        }

        public RoomStatusEnum Status
        {
            get => status.Value;
            set => status.Value = value;
        }

        public string Token
        {
            get => token.Value;
            set => token.Value = value;
        }

        public string CheckinData
        {
            get => checkInData.Value;
            set => checkInData.Value = value;
        }

        public string Url => RoomTag + RoomId.ToString();// MainTopic.Value;

        public string ChannelBaseUrl => Url + "/Channel";


        public ZPropertyList<ZChannel> Channels => channels;

        public ZPropertyRefList<string> Clients = new ZPropertyRefList<string>();

        void OnLoad()
        {
            Clients.Select(clients, a => a.ClientId.ToString());
        }

        public bool IsInService()
        {
            return Status == RoomStatusEnum.Ordered || Status == RoomStatusEnum.Occupied;
        }

        public void AddChannel(string name, ChannelDirectionEnum dir = ChannelDirectionEnum.Out)
        {
            var c = ZPropertyMesh.CreateObject<ZChannel>();
            c.Name.Value = name;
            c.Dir.Value = dir;
            c.Topic.Value = Url + "/Channel/" + name;


            channels.Add(c);
        }

        public void AddChannel(Type t, ChannelDirectionEnum dir = ChannelDirectionEnum.Out)
        {
            //var chindex = t.Name.IndexOf("Channel");
            var cname = BaseChannel.GetChannelName(t.Name);

            var c = ZPropertyMesh.CreateObject<ZChannel>();
            c.Name.Value = cname;
            c.Dir.Value = dir;
            c.Topic.Value = Url + "/Channel/" + cname;

            //get bootflag
            c.BootFlag.Value  = t.GetCustomAttribute<ChannelBootAttribute>()?.BootFlag ?? ChannelBootFlagEnum.Normal;

            channels.Add(c);
        }

        public void AddChannel(ZChannel zChannel)
        {
            channels.Add(zChannel);
        }

        //same name add same direction
        public bool HasChannel(ZChannel zChannel)
        {
            return channels.FindIndex(a => 
            string.Compare(a.Topic, zChannel.Topic, true) == 0 &&
            a.IsIn == zChannel.IsIn
            ) >= 0;
        }

        public bool HasChannelDefined(string name)
        {
            return FindChannel(name) != null;
        }

        public ZChannel FindChannel(string chName)
        {
            return channels.FindValue(a => string.Compare(a.Name, chName, true) == 0 && !a.IsIn ) ;
        }



        public int GetRoleId(string cid)
        {
            //clients
           return clients.FindValue(a => string.Compare(a.ClientId, cid, true) == 0)?.RoleId ?? -1;
        }

        public void ChangeDir()
        {
            foreach (var c in channels)
            {
                c.ChangeDir();
            }
        }

        public void AddClient(List<ZRoomRole> clients)
        {
            this.clients.AddRange(clients);
        }

        public void CopyClients(ZRoom room)
        {
            clients.AddRange(room.clients);
        }

        public void Update(ZRoom zRoom)
        {
            clients.ClearAll();

            clients.AddRange(zRoom.clients);
            status.Value = zRoom.status;

            channels.ClearAll();
            channels.AddRange(zRoom.channels);
        }

    }
}
