using System;
using ZP.Lib.Core.Main;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc;

namespace ZP.Lib.Soc
{
    public class RoomMatrixBehaviour : PropBehaviourSingletonWithTaskScheduler<RoomMatrixBehaviour>
    {
        //private MatrixRuntimeTypeEnum matrixRuntimeTypeEnum = 
        private IChannelCollection channelCollection = null;
        public IChannelCollection ChannelCollection => channelCollection;

        //must run ih a roomserver's task scheduler
        private void Start()
        {
            var socBuilder = SocApp.Instance.GetService<ISocRoomBuilder>() as SocBuilding;

            channelCollection = socBuilder.GetCurServer();

        }

        public TChannel GetChannel<TChannel>() where TChannel : BaseChannel
        {
            return ChannelCollection.GetChannel<TChannel>();
        }

        

        public BaseChannel GetChannel(string channelName)
        {
            return ChannelCollection.GetChannel(channelName);
        }

        public bool IsServer => true;

        public bool IsUnityClient => false;
    }
}
