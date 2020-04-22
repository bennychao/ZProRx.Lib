
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc.Entity;

namespace ZP.Lib.Soc
{
    public static class SocBuildingExtensions
    {
        //for custom broadCost Channel
        static public ISocRoomBuilder AddBroadCost<T>(this ISocRoomBuilder socRoomBuilder, T channel)  where T : BaseChannel
        {
           // channelTypes.Add(typeof(T));

            return socRoomBuilder;
        }

        static public ISocRoomBuilder AddRoundServer<T>(this ISocRoomBuilder socRoomBuilder) where T : ServerChannel
        {
            socRoomBuilder.AddChannelType<T>();

            //TODO check boot attribute

            return socRoomBuilder;
        }

        static public ISocRoomBuilder AddSyncFrameServer<T>(this ISocRoomBuilder socRoomBuilder) where T : ServerChannel
        {
            socRoomBuilder.AddChannelType<T>();

            //TODO check boot attribute

            return socRoomBuilder;
        }


        //static public ISocRoomBuilder AddBroadCost(this ISocRoomBuilder socRoomBuilder, string channelName) 
        //{
        //    // channelTypes.Add(typeof(T));
        //    var ch = new BroadCastChannel();

        //    return socRoomBuilder;
        //}
    }
}
