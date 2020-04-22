using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using UniRx;
using ZP.Lib.Soc.Domain;
using ZP.Lib.Soc;
using UnityEngine;
using ZP.Lib.CoreEx;
using ZP.Lib.Net;
using ZP.Lib.Matrix.Test.Entity;

namespace ZProRx.Test.Server.Entity
{
    public class TestSocketRawServer
    {
        static public void RunServer(ISocAppBuilder app)
        {
            //test raw string 
            ZPropertySocket.ReceiveRaw("topic/recv_rawstr").Subscribe(str =>
            {
                Debug.Log("topic/recv_rawstr recv str:" + str);

            }).AddTo(app); //will end with app stop

            //pair api
            //ZPropertySocket.Post("topic/recv_rawstr", "TestData").Subscribe();

            ZPropertySocket.ReceivePackage<TestPack>("topic/recv_pack", null).Subscribe((TestPack a1) => {

                Debug.Log("topic/recv_pack recv package: " + a1.zProperty.Value.ToString());

             }).AddTo(app);//, (Exception e) => Debug.Log(e.ToString())

            //pair api
            //ZPropertySocket.PostPackage<TestPack>("topic/recv_pack", data).Subscribe().AddTo(disposables);

            //one topic not support listen two times [BUG]
            var disp = ZPropertySocket.ReceivePackageAndResponse<TestPack, bool>("topic/recv_pack_and_response", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPack a1) => {
                    Debug.Log("topic/recv_pack_and_response recv package: " + a1.zProperty.Value.ToString());

                    //return the bool result
                    return true;
                }).AddTo(app);//, (Exception e) => Debug.Log(e.ToString())

            //pair api
            //ZPropertySocket.SendPackage<TestPropData, bool>("topic/recv_pack_and_response", data)

            ZPropertySocket.ReceivePackageAndResponse<TestPack, TestErrorEnum, bool>("topic/recv_pack_and_error_response", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPack a1) => {

                    //throw new Exception(""); 
                    throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    return true;
                }).AddTo(app);//, (Exception e) => Debug.Log(e.ToString())

            //pair api
            //ZPropertySocket.SendPackage<TestPropData, TestErrorEnum, bool>("topic/recv_pack_and_error_response", data)
        }
    }
}
