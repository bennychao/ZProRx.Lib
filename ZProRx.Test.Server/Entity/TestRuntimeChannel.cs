using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;
using ZP.Lib.Battle.Domain;
using ZP.Lib.Battle;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix.Test.Entity
{
    [SystemRuntimeAutoFillClient]

    public class TestRuntimeChannel : SystemRuntimeChannel
    {
        public TestRuntimeChannel(IRoomServer roomServer, string clientName, IConfiguration configuration)
            : base(roomServer, clientName, configuration)
        {
            //RequireBattleData("");
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            Debug.Log("TestRuntimeChannel OnOpened");

            //Scene Json in  Assets\Resources\[APP]/\Server\Scene2.json
            //ZServerScene.Instance.Load("Scene2");

            //wait all client linkin
            linkedClientCount.Where(a => a >= clientIds.Count).Subscribe(_ => OnAllClientLinked());
        }

        protected void OnAllClientLinked()
        {

        }

        //Sync Timeout 2 sec
        [SyncResult(2)]
        [Action("syncwithtimeout")]
        protected ZNull SyncCallWithError()
        {
            return ZNull.Default;
        }

    }
}
