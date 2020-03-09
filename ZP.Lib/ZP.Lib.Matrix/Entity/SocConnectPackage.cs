using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix.Entity
{
    sealed public class SocConnectPackage
    {
        private ZProperty<string> token = new ZProperty<string>();
        private ZProperty<string> clientId = new ZProperty<string>();

        private ZPropertyList<ZChannel> zChannels = new ZPropertyList<ZChannel>();


        public string Token => token.Value;
        public string ClientId => clientId.Value;

        public List<ZChannel> Channels =>( zChannels as IZPropertyList<ZChannel>)?.ConvertToArray().ToList();


        public void SetData(string clientId, string token)
        {
            this.token.Value = token;
            this.clientId.Value = clientId;
        }

        public ZChannel AddChannel(string name)
        {
            var c = ZPropertyMesh.CreateObject<ZChannel>();
            c.Name.Value = name;
            c.Dir.Value = Domain.ChannelDirectionEnum.Out;
            //c.Topic.Value = Url + "/Channel/" + name;

            zChannels.Add(c);

            return c;
        }

        public ZChannel AddChannel(Type t, int roomId)
        {
            var c = ZPropertyMesh.CreateObject<ZChannel>();
            c.Name.Value =  BaseChannel.GetChannelName(t.Name);

            c.Dir.Value = Domain.ChannelDirectionEnum.Out;

            c.Topic.Value = ZRoom.RoomTag + + roomId + "/Channel/" + c.Name.Value;

            c.BootFlag.Value = t.GetCustomAttribute<ChannelBootAttribute>()?.BootFlag ?? ChannelBootFlagEnum.Normal;

            zChannels.Add(c);

            return c;
        }
    }
}
