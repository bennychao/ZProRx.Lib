using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UniRx;
using ZP.Lib;
using ZP.Lib.Core.Domain;
using ZP.Lib.Server.Test.Entity;
using ZP.Lib.CoreEx;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.CoreEx.Status;

namespace ZP.Lib.Server.Test
{
    public class UnitCoreEx
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestEx()
        {

        }

        static volatile int messageCount = 0;
        [Test]
        public void TestObservable()
        {
            var person = ZPropertyMesh.CreateObject<Person>();
            person.blood.Value = 100;
            person.rank.Value = 2;

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var dispose = person.blood.ValueChangeAsObservable().Subscribe(v =>
            {
                messageCount++;
                Assert.IsTrue(v == 200);
                taskEnd.Value = true;
            });

            person.blood.Value = 200;

            //Thread.Sleep(100);
            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();

            dispose.Dispose();

            person.blood.Value = 200;

            //Thread.Sleep(100);

            Assert.IsTrue(messageCount == 1);

            //Assert.IsFalse(true);

            //Assert.Pass();
        }

        [Test]
        public void TestEvents()
        {
            //select sum count
            var tes = ZPropertyMesh.CreateObject<TEvents>();

            for (int i = 0; i < 10; i++)
            {
                var t = ZPropertyMesh.CreateObject<TEvent>();

                t.Id.Value = i;
                tes.events.Add(t);
            }

            IReactiveProperty<bool> callSync = new ReactiveProperty<bool>(false);

            var mainEvent = ZPropertyMesh.GetEventEx<int>(tes, ".onSelectMain");
            Assert.IsTrue(mainEvent != null);

            var dispMain = mainEvent.OnEventObservable().Subscribe(v =>
            {
                Assert.IsTrue(v == 99);
                callSync.Value = true;
            });

            mainEvent.Invoke(99);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            dispMain.Dispose();

            mainEvent.Invoke(199); //for Test dispose

            var eventList = ZPropertyMesh.GetEventsEx(tes, ".events.*.onEvent");

            Assert.IsTrue(eventList.Count == 10);

            callSync.Value = false;
            var disp = ZPropertyObservable.SubEvents(tes, ".events.*.onEvent").Subscribe(ev =>
            {
                var id = ((ev as IDirectLinkable).Parent as TEvent)?.Id.Value;
                Assert.IsTrue(id == 4);

                callSync.Value = true;
            });


            tes.events[4].onEvent.Invoke();

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            disp.Dispose();

            //reset the call flag
            callSync.Value = false;

            var disp2 = ZPropertyObservable.SubEvents(tes, ".events.*.onEvent").SelectParent<TEvent>().Subscribe(te =>
            {
                var id = te?.Id.Value;
                Assert.IsTrue(id == 5);

                callSync.Value = true;
            });


            tes.events[5].onEvent.Invoke();
            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            disp2.Dispose();
            Thread.Sleep(200);

        }

        [Test]
        public void TestFSM()
        {
            //use for FSM AutoLoad
            ServerPath.WorkPath = "../../..";

            var FsmObj = ZPropertyMesh.CreateObject<TestFsmObj>();

            //FsmObj.Fsm.Value = TestFsm.CreateFSM();
            Assert.IsTrue(FsmObj.Fsm.Value.StatusList.Count == 5);

            Assert.IsTrue(FsmObj.Fsm.Value.Transfers.Count == 5);

            IReactiveProperty<bool> callSync = new ReactiveProperty<bool>(false);

            var dispFsm = FsmObj.Status.Enter.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status1);
                callSync.Value = true;
            });

            var statusProp = FsmObj.Status as IStatusProperty;

            Assert.IsTrue(statusProp != null);
            var dispFsm2 = statusProp.EnterObservable.Subscribe(s =>
            {
                Assert.IsTrue(s == (uint)TestStatus.Status1);
            });

            FsmObj.Status.Start();

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callSync.Value = false;

            Thread.Sleep(200);
            dispFsm2.Dispose();

            dispFsm.Dispose();

            //test for transfer to Status2
            dispFsm = FsmObj.Status.Enter.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status2);
                callSync.Value = true;
            });

            //invode will not work
            FsmObj.OnEvent1.Invoke(2);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callSync.Value = false;

            dispFsm.Dispose();

            //test for transfer to Status3
            dispFsm = FsmObj.Status.Enter.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status3);
                callSync.Value = true;
            });

            var dispFsmLeave = FsmObj.Status.Leave.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status2);
                callSync.Value = true;
            });

            FsmObj.OnEvent2.Invoke(2);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callSync.Value = false;

            dispFsm.Dispose();
            dispFsmLeave.Dispose();

            //test for transfer to Status1 form Status3
            dispFsm = FsmObj.Status.Enter.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status1);
                callSync.Value = true;
            });

            dispFsmLeave = FsmObj.Status.Leave.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status3);
                callSync.Value = true;
            });

            FsmObj.OnEvent4.Invoke(2);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callSync.Value = false;

            dispFsm.Dispose();
            dispFsmLeave.Dispose();

            //test for stop fsm
            dispFsmLeave = FsmObj.Status.Leave.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status1);
                callSync.Value = true;
            });

            FsmObj.Status.Stop();

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            dispFsm.Dispose();
            dispFsmLeave.Dispose();

            Assert.IsTrue(FsmObj.Status.Value == TestStatus.Status1);

            dispFsm = FsmObj.Status.Enter.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status1);
                callSync.Value = true;
            });

            //test for restart
            FsmObj.Status.Start();

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callSync.Value = false;

            dispFsm.Dispose();

            dispFsm = FsmObj.Status.Enter.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus.Status2);
                callSync.Value = true;
            });

            var event1 =  ZPropertyMesh.GetEventEx<int>(FsmObj, ".Fsm.OnEvent1");
            event1.Invoke(2);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callSync.Value = false;

            dispFsm.Dispose();

            //test multi Event And Status
            dispFsm = FsmObj.Status.Enter.Subscribe(s =>
            {
                Assert.IsTrue(s == TestStatus2.Status5);
                callSync.Value = true;
            });

            FsmObj.OnEvent5.Invoke(2);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
        }
    }
}
