using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZP.Lib;
using UniRx;
using ZP.Lib.Matrix.Test.Entity;
using ZP.Lib.CoreEx;

namespace ZProRx.Lib.Test
{
    public class TestWebSystem
    {
        public ZProperty<string> AppName = new ZProperty<string>();

        public ZEvent OnGet = new ZEvent();
        public ZEvent OnPut = new ZEvent();
        public ZEvent OnPost = new ZEvent();
        public ZEvent OnDelete = new ZEvent();

        //message List
        private ZMsgList zMsgs = new ZMsgList();

        private string curUrl = "";

        public TestWebSystem(string url)
        {
            this.curUrl = url;
        }

        public void OnBind(Transform node)
        {
            zMsgs.TransNode.transform.DetachChildren();

            OnGet.OnEventObservable().Subscribe(_ =>
            {
                int id = 1;
                ZPropertyNet.Get<TestData>(curUrl + $"/api/v1/TestWeb/{id}").Subscribe(retData =>
                {
                    var msg = ZMsg.Create("msg", $"OnGet return {retData.testNum.Value} !!!");
                    msg.Timer = 2;

                    zMsgs.AddTimer(msg);
                });
            });

            OnPut.OnEventObservable().Subscribe(_ =>
            {
                var data = ZPropertyMesh.CreateObject<TestData>();
                data.testNum.Value = 300;

                int id = 1;

                 ZPropertyNet.Put<TestData>(curUrl + $"/api/v1/TestWeb/{id}", null, data).Subscribe(retData =>
                {
                    var msg = ZMsg.Create("msg", $"OnPut return {retData.testNum.Value} !!!");
                    msg.Timer = 2;

                    zMsgs.AddTimer(msg);
                });
            });

            OnPost.OnEventObservable().Subscribe(_ =>
            {
                var data = ZPropertyMesh.CreateObject<TestData>();
                data.testNum.Value = 300;

                int id = 1;

                ZPropertyNet.Post(curUrl + $"/api/v1/TestWeb/{id}", null, data).Subscribe(__ => {

                    var msg = ZMsg.Create("msg", $"OnPost data {data.testNum.Value} !!!");
                    msg.Timer = 2;

                    zMsgs.AddTimer(msg);

                });

            });

            OnDelete.OnEventObservable().Subscribe(_ =>
            {
                int id = 1;

                ZPropertyNet.Delete(curUrl + $"/api/v1/TestWeb/{id}", null).Subscribe();

                var msg = ZMsg.Create("msg", $"OnDelete !!!");
                msg.Timer = 2;

                zMsgs.AddTimer(msg);
            });

        }//end OnBind
    }
}
