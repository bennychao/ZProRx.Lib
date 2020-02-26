using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Core.Values;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.CoreEx;
using ZP.Lib.Server.Test.Entity;
using System.Threading;

namespace ZP.Lib.Server.Test
{
    public class UnitValues
    {
        private TestTaskScheduler taskScheduler;

        [SetUp]
        public void Setup()
        {
            this.taskScheduler = new TestTaskScheduler(100, $"TestValuesRoom", 991);
        }

        [Test]
        public void TestDateTime()
        {
            var date = ZPropertyMesh.CreateObject<ZDateTime>();

            ZPropertyPrefs.LoadFromStr(date, "{\"ZDateTime.date\":\"2018/09/12\", \"ZDateTime.time\":\"12:09:22\"}");

            DateTime dt;

            DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();

            dtFormat.ShortDatePattern = "yyyy/MM/dd hh:mm:ss";

            dt = Convert.ToDateTime("2018/09/12 12:09:22", dtFormat);

            Assert.IsTrue(date.ToDate().CompareTo(dt) == 0);

            var  date2 = ZDateTime.Create("2018/09/12 12:09:22");

            var dur = date2.Duration(date);

            Assert.IsTrue(dur.Seconds == 0);
        }

        [Test]
        public void TestPair()
        {
            var pair = ZPropertyPairPart2<int, string>.Create(100, "Test");
            Assert.IsTrue(pair.Part1.Value == 100);

            Assert.IsTrue(string.Compare(pair.Part2.Value, "Test") == 0);

            pair.SetPart1("Data1", 99);

            var strData = ZPropertyPrefs.ConvertToStr(pair);

            var pair2 = ZPropertyMesh.CreateObject< ZPropertyPairPart2<int, string>>();

            //part name will not save to json str
            ZPropertyPrefs.LoadFromStr(pair2, strData);

            Assert.IsTrue(pair2.Part1.Value == 99);

            Assert.IsTrue(string.Compare(pair2.Part2.Value, "Test") == 0);


        }

        class HubData
        {
            public int Data { set; get; }
            public HubData(int iData)
            {
                Data = iData;
            }
        }
        [Test]
        public void TestHub()
        {
            var person = ZPropertyMesh.CreateObject<Person>();
            person.blood.Value = 100;
            person.rank.Value = 2;

            Assert.IsTrue(person.rank.Value == 2);
            Assert.IsTrue(person.blood.Value == 100);


            Weapon sword = person.weapon.Value;
            sword.power.Value = 991.0f;

            var weaponHub = ZDataHub<ZProperty<Weapon>>.Create(person.weapon);

            Assert.AreEqual(weaponHub.Node.Value.power.Value, 991.0f, 0.001f);

            var whub = ZPropertyHub<Weapon>.CreateHub(100.5f);

            Assert.AreEqual(whub.Node.Value.power.Value, 100.5f, 0.001f);

            Assert.AreEqual(whub.Value.power.Value, 100.5f, 0.001f);
        }

        [Test]
        public async Task TestMsgAndHints()
        {
            ServerPath.WorkPath = "../../..";

            var task = new Task(() =>
            {
                ZServerScene.Instance.Load("Scene4");
                var runId = MatrixRuntimeTools.GetRunId();

                var mainScene = GameObject.FindObjectOfType<MainScene2>();

                Assert.IsTrue(mainScene.bACallAwake == true);
                Assert.IsTrue(mainScene.bACallStart == true);

                mainScene.taskCount.Where(cur => cur > 60).Fetch().Timeout(TimeSpan.FromSeconds(10)).Wait();

                //await Observable.ReturnUnit().Delay(TimeSpan.FromSeconds(2));
               // Thread.Sleep(6000);

                Assert.IsTrue(mainScene.Msgs.HintList.TransNode.childCount == 0);

                Assert.IsTrue(mainScene.Msgs.MsgList.TransNode.childCount == 0);

                ZViewBuildTools.UnBindObject(mainScene.Msgs);
            });

            task.Start(this.taskScheduler);

            await task;
        }

        [Test]
        public async Task TestTask()
        {

        }
    }
}
