

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Core.Domain;
using ZP.Lib.Net;
using ZP.Lib.CoreEx.Tools;

#if ZP_UNITY_CLIENT
using uPLibrary.Networking.M2Mqtt.Messages;
#endif


#if ZP_SERVER
using MQTTnet;
#endif

//same as NetPackage (low level net package)
namespace ZP.Lib.CoreEx.Reactive
{
    public class SocketPackageHub<TData>
    {
       public  ISocketPackage socketPackage;
       public  NetPackage<TData, ZNetErrorEnum> data = new NetPackage<TData, ZNetErrorEnum>();
    }

    public class SocketPackageHub<TData, TErrorEnum>
    {
        public ISocketPackage socketPackage;
        public NetPackage<TData, TErrorEnum> data = new NetPackage<TData, TErrorEnum>();
    }

    public class SocketPackageHub
    {
        public ISocketPackage socketPackage;
        public NetPackage<IRawDataPref, ZNetErrorEnum> data = new NetPackage<IRawDataPref, ZNetErrorEnum>();
    }

    /// <summary>
    /// for 
    /// </summary>
#if ZP_SERVER
    public class ServerSocketPackage : ISocketPackage
    {

        internal MqttApplicationMessageReceivedEventArgs e { get; set; }


        public string Key => e.ClientId;
        public string Value { get; set; }

        public string Topic => e.ApplicationMessage.Topic;
        public ServerSocketPackage(MqttApplicationMessageReceivedEventArgs e, string value)
        {
            this.e = e;
            Value = value;
        }

        internal static ISocketPackage CreateFakePackage(string topic, string value)
        {
            var amsg = new MqttApplicationMessage();
            amsg.Topic = topic;
            var index = topic.LastIndexOf("/");
            //amsg.Payload = 
            var clientId = (TaskScheduler.Current as IZMatrixRuntime)?.RunId ?? ZPropertySocket.ClientID;
                //topic.Substring(index);
            var e = new MqttApplicationMessageReceivedEventArgs(clientId, amsg);

            return new ServerSocketPackage(e, value);

        }

        
    }
#endif
#if ZP_UNITY_CLIENT

    public class UniClientSocketPackage : ISocketPackage
    {

        internal MqttMsgPublishEventArgs e { get; set; }

        //clientId
        public string Key => MatrixRuntimeTools.RoomServer; //ZPropertySocket.ClientID;
        public string Value { get; set; }

        public string Topic => e.Topic;
        public UniClientSocketPackage(MqttMsgPublishEventArgs e, string value)
        {
            this.e = e;
            Value = value;
        }

        internal static ISocketPackage CreateFakePackage(string topic, string value)
        {
            //var amsg = new MqttApplicationMessage();
            //amsg.Topic = topic;
            //var index = topic.LastIndexOf("/");
            ////amsg.Payload = 
            //var clientId = (TaskScheduler.Current as IMatrixRuntime)?.RunId ?? ZPropertySocket.ClientID;
            ////topic.Substring(index);
            //var e = new MqttMsgPublishEventArgs();

            //return new JniClientSocketResponse(e, value);
            throw new Exception("Unity Client Not Support Fake Response");
        }


    }
#endif

}
