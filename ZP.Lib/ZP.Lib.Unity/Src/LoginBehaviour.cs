using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZP.Lib;
using ZP.Lib.Matrix;
using ZP.Lib.Unity;

public class LoginBehaviour<LData> : MonoBehaviour, ILoginItem
{
    //public Transform LoginCanvas;

    //public Transform RegisterCanvas;

    //ZP.Lib.Matrix.
    //ChannelListener channelListener = null;
    public string ClientId;
    public string ClientSecret;
    public string UserId;
    public string Password;

    public LoginSystem<LData> LoginSystem => LoginSystem<LData>.Instance;

    public bool IsLogined => LoginSystem.bLogined.Value;

    //LoginSystem loginSystem = null;

    // Start is called before the first frame update
    protected void Awake()
    {
        //set the clientId
        ZPropertySocket.ClientID = ClientId;
    }

    protected void Start()
    {
        if (IsLogined)
            return;

        if (LoginBuilder.Instance == null)
        {
            BindSystem();
        }

        //loginSystem = ZPropertyMesh.CreateObject<LoginSystem>();
    }

    void BindSystem()
    {
        LoginSystem<LData>.Instance.UserName.Value = UserId;  //"user001";
        LoginSystem<LData>.Instance.UserPassword.Value = Password; // "12345678";

        SceneManager.LoadSceneAsync("Scenes/Login").AsObservable().Subscribe(_=>
        {
            //bind the current's systemlogin UI 

            //ZViewBuildTools.BindObject(this.LoginSystem, LoginCanvas);
            LoginBuilder.Instance.BindLogin(this.LoginSystem);

            LoginSystem.OnToRegister.OnEventObservable().Subscribe(__ =>
            {
                //RegisterCanvas.gameObject.SetActive(true);

                //ZViewBuildTools.BindObject(this.LoginSystem, RegisterCanvas);
                LoginBuilder.Instance.BindRegister(this.LoginSystem);

                //for bind custom register data 

            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
