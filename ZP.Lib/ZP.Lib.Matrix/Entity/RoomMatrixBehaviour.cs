using System;
using System.Collections;
using System.Threading.Tasks;
using ZP.Lib.Common;
using UniRx;
using UnityEngine;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib;
using UnityEngine.SceneManagement;

//unity client Room Matrix
//[Conditional("ZP_UNITY_CLIENT")]
public class RoomMatrixBehaviour:  Singleton<RoomMatrixBehaviour>
{

    // public Vector3 Pos = Vector3.zero;
    // public Vector3 Dir = Vector3.up;
    // public Vector3 Scale = Vector3.one;

    //public string AssetPath = "";

    public string AppName = "ZProApp";

    public bool IsServer => false;

    public bool IsUnityClient => true;

    //public bool IsLogined { get; set; }

    static private bool bSingle = false;


    public IChannelCollection ChannelCollection => CurRoom;
    public static RoomServerBehaviour CurRoom => Instance.GetComponent<RoomServerBehaviour>();

    public IScheduler RxScheduler => Scheduler.MainThread;

    private void Awake()
    {
        ServerPath.AppName = AppName;

        //check if is single ??

        //curRoom =
        //check if have logined
        //var loginObj = GameObject.Find("Login");
        //var loginComponent = loginObj?.GetComponent<ILoginItem>();
        //if (loginComponent?.IsLogined == true)
        //{
        //    SceneManager.LoadScene("Scenes/Login");
        //}

        //if (loginObj == null)
        //{
        //    SceneManager.LoadScene("Scenes/Login");
        //}

        if (bSingle)
        {
            GameObject.DestroyImmediate(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);

            bSingle = true;
        }
    }

    void Start()
    {

    } 

    public async Task SwitchRoom<TRoom>(string ip, int port, int roomId) where TRoom: RoomServerBehaviour
    {
        //TODO close current scene
        //LoadScene

        if (CurRoom != null)
        {
            await CurRoom.Disconnect();
            CurRoom.enabled = false;
        }

        var nRoom = GetOrAddComponent<TRoom>();
        nRoom.ServerIP = ip;
        nRoom.ServerPort = port;
        nRoom.VRoomId = roomId;
        nRoom.enabled = true;
        await nRoom.Connect();

        //curRoom = nRoom;
    }

    public void StartSwitchRoomCoroutine<TRoom>(string ip, int port, int roomId) where TRoom : RoomServerBehaviour
    {
        StartCoroutine(SwitchRoomCoroutine<TRoom>(ip, port, roomId));
    }

    public void StartSwitchRoomCoroutine<TRoom>() where TRoom : RoomServerBehaviour
    {
        StartCoroutine(SwitchRoomCoroutine<TRoom>());
    }

    public IEnumerator SwitchRoomCoroutine<TRoom>(string ip, int port, int roomId) where TRoom : RoomServerBehaviour
    {

        if (CurRoom == null)
        {
            yield return CurRoom.Disconnect().ToYieldInstruction();
            CurRoom.enabled = false;

            Debug.LogError("Current Room is Null");
        }

        var nRoom = GetOrAddComponent<TRoom>();
        nRoom.ServerIP = ip;
        nRoom.ServerPort = port;
        nRoom.VRoomId = roomId;
        nRoom.enabled = true;
        yield return nRoom.Connect().ToYieldInstruction();

        yield return null;
    }

    public IEnumerator SwitchRoomCoroutine<TRoom>() where TRoom : RoomServerBehaviour
    {

        if (CurRoom == null)
        {
            yield return CurRoom.Disconnect().ToYieldInstruction();
            CurRoom.enabled = false;

            Debug.LogError("Current Room is Null");
        }

        var nRoom = GetOrAddComponent<TRoom>();
        //nRoom.ServerIP = ip;
        //nRoom.ServerPort = port;
        //nRoom.VRoomId = roomId;
        nRoom.enabled = true;
        yield return nRoom.Connect().ToYieldInstruction();

        yield return null;
    }

    private RoomServerBehaviour GetOrAddComponent<TRoom>() where TRoom : RoomServerBehaviour
    {
       return  gameObject.GetComponent<TRoom>() ?? gameObject.AddComponent<TRoom>() as TRoom;
    }

    public TChannel GetChannel<TChannel>() where TChannel : BaseChannel
    {
        return ChannelCollection?.GetChannel<TChannel>();
    }

    public BaseChannel GetChannel(string channelName)
    {
        return ChannelCollection?.GetChannel(channelName);
    }

    public IChannelClient GetChannelClient(string channelName)
    {
        return ChannelCollection?.GetChannelClient(channelName);
    }

    public IChannelClient GetChannelClient(string channelName, string clientId)
    {
        return ChannelCollection?.GetChannelClient(channelName, clientId);
    }


    //public void Use3T2Axes()
    //{
    //    var a = new N3T2AxesSystem();
    //    a.TRS(Pos, Dir, Scale);
    //    ZWorld.InitAxes(a);
    //    return app;
    //}

    //public void Use3T2Axes(Action<N3T2AxesSystem> configAxesFunc)
    //{
    //    var a = new N3T2AxesSystem();
    //    if (configAxesFunc != null)
    //    {
    //        configAxesFunc(a);
    //    }
    //    ZWorld.InitAxes(a);
    //    return app;
    //}

    //public void UseAxes(IAxesSystem axesSystem)
    //{
    //    ZWorld.InitAxes(axesSystem);
    //    return app;
    //}
}
