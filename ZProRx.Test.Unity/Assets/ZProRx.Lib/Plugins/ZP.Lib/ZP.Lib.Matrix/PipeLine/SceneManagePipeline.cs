using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix
{
    [ChannelBoot(ChannelBootFlagEnum.SceneMgr)]
    public class SceneManagePipeline : BasePipeline, IChannelClient, ISceneMgrPipeline
    {
        public string CurSceneName { get; protected set; }

        protected Subject<string> OnLoaded = new Subject<string>();

        //all client and server load Scene end
        public IObservable<string> OnLoadedObservable => OnLoaded.ObserveOn(innerScheder);

        public SceneManagePipeline()
        {

        }

        public async Task<string> GetCurSceneAsync()
        {
            return await Send<ZNull, string>("getcurscene", ZNull.Default).Fetch();
        }

        //load by User
        public async Task LoadSceneAsync(string sceneName)
        {
            await Send<string, ZNull>("loadscenesync", sceneName).Fetch();

            //wait current pipeline to load Scene
            await OnLoadedObservable.Fetch();
        }

        protected override void OnListen()
        {
            base.OnListen();

            //auto connected
            Connect2().Subscribe();
        }

        //call by server
        [Action("loadscene")]
        protected ZNull onLoadScene([FromPackage] string sceneName)
        {
            LoadScene(sceneName);
            Debug.Log("SceneManagePipeline loadscene");

            //Sync to loaded Status
            Send<ZNull, ZNull>("loadsceneend", ZNull.Default).ObserveOn(innerScheder).Subscribe(_=>
            {
                //Debug.Log("SceneManagePipeline loadsceneend return");
                OnLoaded.OnNext(sceneName);
            });

            return ZNull.Default;
        }

        [PreListen]
        [Action("onclientconnected")]
        protected ZNull OnClientConnected([FromPackage] string clientId)
        {
            if (TryAddClient(clientId))
                OnConnected.OnNext(clientId);

            return ZNull.Default;
        }

        [PreListen]
        [Action("onclientdisconnected")]
        protected ZNull OnClientDisConnected([FromPackage] string clientId)
        {
            if (TryDelClient(clientId))
                OnDisConnected.OnNext(clientId);
            //sysClients.Remove(clientId);
            //or throw a exception

            return ZNull.Default;
        }


        virtual protected void LoadScene(string sceneName)
        {
            CurSceneName = sceneName;
            //TODO Load Unity Load Scene
            //SceneManager.LoadScene(sceneName);

#if ZP_UNITY_CLIENT
            //if (RoomMatrixBehaviour.Instance.IsUnityClient)
            ObservableEx.NextFrame().Subscribe(_ =>
                   {
                       SceneManager.LoadScene(sceneName);

                     //  syncRet.Value = true;
                   }) ;
#else

#endif
        }


    }
}
