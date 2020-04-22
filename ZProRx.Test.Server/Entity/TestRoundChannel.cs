using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Soc;
using UniRx;

namespace ZP.Lib.Matrix.Test.Entity
{

    public class TestRoundChannel : RoundChannel<TestCmd>
    {
        public TestRoundChannel(IRoomServer roomServer, string clientName, IConfigurationRoot configuration)
            : base(roomServer, clientName, configuration)
        {
        }


        protected override void OnOpened()
        {
            base.OnOpened();

            //use for custom Agent
            //OnConnectedObservable.Subscribe(c =>
            //{
            //    RegisterAgent(c, new TestCmdClientAgent(c));
            //});

        }
    }
}
