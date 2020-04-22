using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;
using ZP.Lib.Core.Values;
using ZP.Lib.CoreEx.Tools;

namespace ZP.Lib.Server.Test.Entity
{
    public class MainScene2 : MonoBehaviour
    {
        public bool bACallAwake = false;
        public bool bACallStart = false;
        public bool bACallDestroy = false;

        public IReactiveProperty<int> taskCount = new ReactiveProperty<int>(0);

        public MsgsHints Msgs = null;

        private void Awake()
        {
            bACallAwake = true;
        }

        void Start()
        {
            bACallStart = true;

            var root = GameObject.Find("Root")?.transform;

            var msgs = ZPropertyMesh.CreateObject<MsgsHints>();
            Msgs = msgs;


            ZViewBuildTools.BindObject(msgs, root);

            var runId = MatrixRuntimeTools.GetRunId();

            for (int i = 0; i < 10; i++)
            {
                var hint = ZHint.Create("title", "description");

                hint.Transfrom.Position.Value = new Vector3(1, 1, 1);
                hint.Timer = 0.2f * i + 2;
                msgs.HintList.AddTimer(hint);

                var msg = ZMsg.Create("msg", "description");
                msg.Timer = 0.2f * i + 2;
                msgs.MsgList.AddTimer(msg);
            }

            Assert.IsTrue(msgs.HintList.TransNode.childCount == 10);

            Assert.IsTrue(msgs.MsgList.TransNode.childCount == 10);

            var disp = ObservableEx.EveryUpdate().ObserveOn(Scheduler.CurrentThread).Subscribe(_ =>
            {
                runId = MatrixRuntimeTools.GetRunId();
                Assert.IsTrue(string.Compare("TestValuesRoom", runId) == 0);
                //Debug.Log("ObservableEx.NextFrame")
                taskCount.Value++;
            });

        }
    }
}
