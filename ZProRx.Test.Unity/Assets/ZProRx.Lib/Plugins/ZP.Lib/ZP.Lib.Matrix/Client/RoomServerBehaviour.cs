#if ZP_UNITY_CLIENT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using UniRx;
using ZP.Lib.Matrix;
using System;
using ZP.Lib.Common;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using System.Threading.Tasks;
using System.Linq;

//TODO only support client
//[Conditional("ZP_UNITY_CLIENT")]
public class RoomServerBehaviour :MonoBehaviour, IChannelCollection, IRoomClient//Singleton<RoomServerBehaviour> //MonoBehaviour
{
    private Subject<Unit> stopSubject = new Subject<Unit>();
    private Subject<Unit> runSubject = new Subject<Unit>();

    private List<BaseChannel> channels = new List<BaseChannel>();

    private ZRoom zRoom;
    
    //for debug
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 5050;
    public int VRoomId = 1;

    public ZRoom ZRoom => zRoom;

    public IObservable<Unit> OnRunObservable => runSubject;

    public IObservable<Unit> OnStopObservable => stopSubject;

    // Start is called before the first frame update

    void Start()
    {
        //for test
        //zRoom = ZPropertyMesh.CreateObject<ZRoom>();
        //zRoom.Host = ServerIP;
        //zRoom.Port = ServerPort;
        //zRoom.RoomId = 1;
        //init channels
        //AddChannel(new TestClientChannel());
        //Connect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IObservable<Unit> Connect()
    {
        ZPropertySocket.ClientIP = ServerIP;
        ZPropertySocket.ClientPort = ServerPort;


        if (!ZPropertySocket.IsConnected())
            ZPropertySocket.Start();

        var request = ZPropertyMesh.CreateObject<SocConnectPackage>();
        request.SetData(ZPropertySocket.ClientID, "23312312312");

        foreach (var t in channels)
        {
            var tName = t.GetType().Name;
            var chindex = tName.IndexOf("Channel");
            var cname = chindex > 0 ? tName.Substring(0, tName.Length - 7) : tName;
            //room.AddChannel(cname);

            request.AddChannel(cname);
        }

        //request.ClientId = "33123";
        //send connect room topic to check in
        ZPropertySocket.SendPackage<SocConnectPackage, ZRoom>(MatrixRoomTopicDefines.RoomConnect(VRoomId), request)
            .Subscribe(room =>
        {
            Debug.Log("Join Room ");
            zRoom = room;

            //reverse thd change direction
            zRoom.ChangeDir();

            RunChannels();

            runSubject.OnNext(Unit.Default);
            runSubject.OnCompleted();
        });

        return runSubject;
    }

    public IObservable<Unit> Disconnect()
    {
        ZPropertySocket.PostPackage<string>(MatrixRoomTopicDefines.RoomDisConnect(VRoomId), ZPropertySocket.ClientID).Subscribe(_ =>
        {
            Debug.Log("Leave Room");

            //ZPropertySocket.Close();

            stopSubject.OnNext(Unit.Default);
            stopSubject.OnCompleted();

		});

        return stopSubject;
    }

    public void AddChannel(BaseChannel channel)
    {
        channels.Add(channel);

        //channel.Listen();
        //channel.BindRoom(zRoom);
    }

    public void AddPipeline(BasePipeline channel)
    {
        channels.Add(channel);

        //channel.Listen();
        //channel.BindRoom(zRoom);
    }

    public IChannelClient GetChannelClient(string channelName)
    {
        var c = zRoom.Channels.FindValue(a => a.Name.Value.CompareTo(channelName) == 0 && a.IsIn);
        if (c == null)
            return null;

        var cc = new ChannelClient(c, null);

        return cc;
    }

    public IChannelClient GetChannelClient(string channelName, string clientId)
    {
        var c = zRoom.Channels.FindValue(a => a.Name.Value.CompareTo(channelName) == 0 && a.IsIn);
        if (c == null)
            return null;

        var cc = new ChannelClient(c, null);

        return cc;
    }


    public void SendPackage<T>(string action, T data)
    {
        var url = zRoom.Url + "/" + action;

        ZPropertySocket.SendPackage<T, ZNull>(url, data);
    }

    public void PostPackage<T>(string action, T data)
    {
        var url = zRoom.Url + "/" + action;

        ZPropertySocket.Post(url, data);
    }

    void RunChannels()
    {
        foreach (var c in channels)
        {
            c.BindRoom(zRoom);
            c.Listen();
        }
    }

    public bool CheckChannel(string channelName)
    {
        return zRoom?.HasChannelDefined(channelName) ?? false;
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

    public BaseChannel GetChannel(string channelName)
    {
        //return channels.Find(a => Type.GetType(channelName)?.IsAssignableFrom(a.GetType()) ?? false) as BaseChannel;
        return channels.Find(a => string.Compare(a.ChannelName, channelName, true) == 0 || channelName.Contains(a.ChannelName));
    }

    public void Connect(int roomId)
    {
        //get room's port
        Connect().Subscribe();
    }

    void IRoomClient.Disconnect()
    {
        Disconnect().Subscribe();
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
        //throw new NotImplementedException();
        return true;
    }

    public Task RunInClient(Action action)
    {
        //throw new NotImplementedException();
        var task = new Task(action);
        task.Start();
        return task;
    }

    public Task RunInClient(Func<Task> function)
    {
        var task = new Task(async () => await function());
        task.Start();
        return task;
    }
}


#endif