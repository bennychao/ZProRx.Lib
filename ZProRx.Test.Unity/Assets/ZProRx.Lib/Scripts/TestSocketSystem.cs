using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZP.Lib;
using UniRx;
using ZP.Lib.Matrix.Test.Entity;
using ZP.Lib.Net;

namespace ZProRx.Lib.Unity
{
    public class TestSocketSystem
    {
        public ZProperty<string> AppName = new ZProperty<string>();

        public ZEvent OnStartSock = new ZEvent();

        public ZEvent OnPostRawStr = new ZEvent();
        public ZEvent OnPostPackage = new ZEvent();
        public ZEvent OnSendPackage = new ZEvent();
        public ZEvent OnSendPackageWithError = new ZEvent();

        //message List
        private ZMsgList zMsgs = new ZMsgList();


        public void OnBind(Transform node)
        {

            OnStartSock.OnEventObservable().Subscribe(_ =>
            {
                if (ZPropertySocket.IsConnected())
                    ZPropertySocket.Close();
                else
                    ZPropertySocket.Start();
            });

            OnPostRawStr.OnEventObservable().Subscribe(_ =>
            {
                ZPropertySocket.Post("topic/recv_rawstr", "TestData").Catch((Exception e )=>
                {
                    var msg = ZMsg.Create("msg", $"OnSendPackageWithError return {e.Message} !!!");
                    msg.Timer = 2;

                    zMsgs.AddTimer(msg);

                    return Observable.Empty<Unit>();
                }
                    ).Subscribe();
            });

            OnPostPackage.OnEventObservable().Subscribe(_ =>
            {
                var pack = ZPropertyMesh.CreateObject<TestPack>();
                pack.zProperty.Value = 900;
                ZPropertySocket.PostPackage<TestPack>("topic/recv_pack", pack).Subscribe();

            });

            OnSendPackage.OnEventObservable().Subscribe(_ =>
            {
                var pack = ZPropertyMesh.CreateObject<TestPack>();
                pack.zProperty.Value = 900;

                ZPropertySocket.SendPackage<TestPack, bool>("topic/recv_pack_and_response", pack).Subscribe(ret=>
                {
                    var msg = ZMsg.Create("msg", $"OnSendPackage return {ret} !!!");
                    msg.Timer = 2;

                    zMsgs.AddTimer(msg);
                });
            });

            OnSendPackageWithError.OnEventObservable().Subscribe(_ =>
            {
                var pack = ZPropertyMesh.CreateObject<TestPack>();
                pack.zProperty.Value = 900;

                ZPropertySocket.SendPackage2<TestPack, bool>("topic/recv_pack_and_error_response", pack).Subscribe(bRet =>
                {
                    //Assert.IsTrue(bRet);
                }, error =>
                {
                    if (error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1))
                    {
                        var msg = ZMsg.Create("msg", $"OnSendPackageWithError return {TestErrorEnum.Error1} !!!");
                        msg.Timer = 2;

                        zMsgs.AddTimer(msg);
                    }
                });
            });
        }
    }
}
