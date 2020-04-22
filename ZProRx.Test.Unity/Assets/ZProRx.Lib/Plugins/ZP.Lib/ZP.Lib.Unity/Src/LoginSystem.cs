using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Entity;
using ZP.Matrix.Architect.Domain;

[PropertyUIItemResClass("Views/LoginView")]
//[PropertyAddComponentClassAttribute(typeof(RoomMatrixBehaviour))]
public class LoginSystem<LData> : PropObjectSingleton<LoginSystem<LData>>
{
    //need to bind to the LayoutUI
    [PropertyDescription("Name", "enter your name")]
    public ZProperty<string> UserName = new ZProperty<string>();

    [PropertyDescription("Password", "enter your password")]
    public ZProperty<string> UserPassword = new ZProperty<string>();

    public ZProperty<bool> bLogined = new ZProperty<bool>();

    //no need to bind
    public ZProperty<ZToken> Token = new ZProperty<ZToken>();
    public ZProperty<ZRoom> Room = new ZProperty<ZRoom>();

    public ZEvent OnLogin = new ZEvent();
    public ZEvent OnLogoff = new ZEvent();

    public ZEvent OnRegister = new ZEvent();

    public ZEvent OnToRegister = new ZEvent();

    //default is null
    public object CustomRegisterData { set; get; } = null;

    

    protected Subject<LData> onLogged = new Subject<LData>();
    protected Subject<ZNull> onLogoffed = new Subject<ZNull>();

    //public
    public IObservable<LData> OnLoggedObservable => onLogged;
    public IObservable<ZNull> OnLogoffedObservable => onLogoffed;



    public LoginSystem()
    {
        OnLogin.OnEventObservable().Subscribe(_ =>
        {
            Login();
        });

        OnLogoff.OnEventObservable().Subscribe(_ =>
        {
            Logoff();
        });

        OnRegister.OnEventObservable().Subscribe(_ =>
        {
            Register();
        });
    }

    void OnCreate()
    {
        //[TODO] to check if has the token (second login) 
        //will not show the login UI
    }

    public bool IsFirst()
    {
        return Token.Value?.IsValid() ?? true;
    }

    public void Login()
    {
        var user = UserName.Value;
        var password = UserPassword.Value;

        //ZPropertyNet.Get<ZToken>("http://localhost:7001" + $"/api/v1/matrix/insp/Logins/{user}/{password}", null).Subscribe(token=>
        //{
        //    Token.Value = token;
        //    Debug.Log("Login success!!");

        //    //get the login data
        //    var loginData = token.GetRedirectData<ZPropertyPairPart2<string, ZRoom>>();

        //    Room.Value = loginData.Part2.Value;

        //    var vroomid = Room.Value;
        //    //for test join the roomvroomid
        //    ///api/v1/matrix/arch/Rooms/users/{vroomid} RoomErrorEnum
        //    ///client not to set player with the token
        //    ZPropertyNet.Put<ZRoom>("http://localhost:7001" + $"/api/v1/matrix/arch/Rooms/users/{vroomid}", null);

        //});

        //token will save to httpcontext
        ZPropertyNet.Login<ZPropertyPairPart2<LData, ZRoom>>(
            "http://localhost:7001" + $"/api/v1/matrix/insp/Logins/{user}/{password}", null)
            .Subscribe(async loginData =>
            {
                Debug.Log($"LoginSystem Logined {user}");

                Room.Value = loginData.Part2.Value;

                Dictionary<string, string> query = new Dictionary<string, string>();
                query["playerId"] = "1";

                var vroomId = Room.Value.RoomId;
                //for test join the room vroomId
                ///api/v1/matrix/arch/Rooms/users/{vroomId} RoomErrorEnum
                ///client not to set player with the token
                //var ret = await
                ZPropertyNet.Put<ZRoom, RoomErrorEnum>(
                    "http://localhost:7000" + $"/api/v1/matrix/arch/Rooms/users/{vroomId}", query)
                .Subscribe(
                    room =>
                    {
                        Debug.Log("Get a Token");
                        onLogged.OnNext(loginData.Part1.Value);

                        bLogined.Value = true;

                        //RoomMatrixBehaviour.Instance.StartSwitchRoomCoroutine<TestRoomServer>("localhost", room.Port, room.RoomId);
                        //StartCoroutine()
                        //RoomMatrixBehaviour.Instance.SwitchRoom<TestRoomServer>("localhost", room.Port, room.RoomId);
                        //RoomMatrixBehaviour.Instance
                    });

                Debug.Log("join a Room");
            });
    }

    public void Logoff()
    {
        //var user = Name.Value;
        ZPropertyNet.Delete<ZNull>($"/api/v1/matrix/insp/Logins", null).Subscribe(_=>{

            bLogined.Value = false;

            //check the token
            Debug.Log("Logoff Bye!!");

            onLogoffed.OnNext(ZNull.Default);
        });
    }

    public void Register()
    {
        //have nothing register Data
        ZUserRegisterRequest request = ZPropertyMesh.CreateObjectWithParam<ZUserRegisterRequest>(UserName.Value, UserPassword.Value);

        if (this.CustomRegisterData != null)
            request.SetData(this.CustomRegisterData);

        //request.SetData()       
        ZPropertyNet.Post<ZUserRegisterRequest>($"/api/v1/matrix/insp/Users", null, request).Subscribe(_=>{

            //check the token
            Debug.Log("Register User");

            onLogoffed.OnNext(ZNull.Default);
        });
    }
}
