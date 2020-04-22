using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using ZP.Lib.CoreEx;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.Server.Test.Entity;

namespace ZP.Lib.Server.Test
{
    internal  class UnitScene
    {
        private TestTaskScheduler taskScheduler;
        [SetUp]
        public void Setup()
        {
            this.taskScheduler = new TestTaskScheduler(100, $"TestRoom", 99);
        }

        [Test]
        public async Task TestLoadScene()
        {
            ServerPath.WorkPath = "../../..";

            var task = new Task(() =>
            {
                ZServerScene.Instance.Load("Scene3");
                var runId = MatrixRuntimeTools.GetRunId();


                var mainScene = GameObject.FindObjectOfType<MainScene>();

                Assert.IsTrue(mainScene.bACallAwake == true);
                Assert.IsTrue(mainScene.bACallStart == true);

                //await Observable.ReturnUnit().Delay(TimeSpan.FromSeconds(2));
               // Thread.Sleep(2000);
                mainScene.taskCount.Where(cur => cur > 20).Fetch().Timeout(TimeSpan.FromSeconds(5)).Wait();

                Assert.IsTrue(mainScene.taskCount.Value > 10);

                Assert.IsTrue(mainScene.bACallNextFrame == true);

                var sub = mainScene.gameObject.GetComponentInChildren<Transform>();

                //test for close
                ZServerScene.Instance.Close();

                var bindTarget = GameObject.Find("main"); //self

               

                Assert.IsTrue(bindTarget == null);
                Assert.IsTrue(mainScene.bACallDestroy == true);

            });

            task.Start(this.taskScheduler);

            await task;


        }

        [Test]
        public void TestPrefabs()
        {
            //[TODO]
            //ZServerScene.Instance.Load("Scene2");
            Assert.IsTrue(true);
        }


        [Test]
        public void TestBindEvents()
        {
            //[TODO]
            //ZServerScene.Instance.Load("Scene2");
            Assert.IsTrue(true);
        }

        [Test]
        public void TestMatrixAndMath()
        {
            var pos = new System.Numerics.Vector3(5.0f, 0.0f, 0.0f);
            System.Numerics.Matrix4x4 t = System.Numerics.Matrix4x4.CreateTranslation(pos);

            System.Numerics.Matrix4x4 r = System.Numerics.Matrix4x4.CreateFromQuaternion(new System.Numerics.Quaternion(0.0f, 0.0f, 0.0f, 1.0f));

            System.Numerics.Matrix4x4 s = System.Numerics.Matrix4x4.CreateScale(new System.Numerics.Vector3(1.0f, 1.0f, 1.0f));

            var m = System.Numerics.Matrix4x4.Multiply(System.Numerics.Matrix4x4.Multiply(s, r), t); //must this order

            var tpos = System.Numerics.Vector3.Transform(pos, m);

            var euler = Quat2Euler.QuaternionToEuler(new Quaternion(0.0f, 0.9537f, 0.0f, 0.3007f));

            Assert.AreEqual(euler.y, 145.0f, 0.01f);

            euler = Quat2Euler.QuaternionToEuler(new Quaternion(0.4435054f, 0.770011f, -0.2308743f, 0.3963371f));

            Assert.AreEqual(euler.x, 45.0f, 0.01f);
            Assert.AreEqual(euler.y, 145.0f, 0.01f);
            Assert.AreEqual(euler.z, 45.0f, 0.01f);
            //ZServerScene.Instance.Load("Scene2");            

            //euler (0, -60, 0)
            euler = Quat2Euler.QuaternionToEuler(new Quaternion(0.0f, -0.5f, 0f, 0.8660254f));

            Assert.AreEqual(euler.x, 0.0f, 0.01f);
            Assert.AreEqual(euler.y, -60.0f, 0.01f);
            Assert.AreEqual(euler.z, 0.0f, 0.01f);

            //euler (0, 60, 0)
            euler = Quat2Euler.QuaternionToEuler(new Quaternion(0.0f, 0.5f, 0f, 0.8660254f));

            Assert.AreEqual(euler.x, 0.0f, 0.01f);
            Assert.AreEqual(euler.y, 60.0f, 0.01f);
            Assert.AreEqual(euler.z, 0.0f, 0.01f);

            //euler (0, 180, 0)
            euler = Quat2Euler.QuaternionToEuler(new Quaternion(0.0f, 1.0f, 0f, 0.0f));

            Assert.AreEqual(euler.x, 0.0f, 0.01f);
            Assert.AreEqual(euler.y, 180.0f, 0.01f);
            Assert.AreEqual(euler.z, 0.0f, 0.01f);

            euler = Quat2Euler.QuaternionToEuler(new Quaternion(0.0f, 0.0f, 1.0f, 0.0f));

            Assert.AreEqual(euler.x, 0.0f, 0.01f);
            Assert.AreEqual(euler.y, 0.0f, 0.01f);
            Assert.AreEqual(euler.z, 180.0f, 0.01f);

            euler = Quat2Euler.QuaternionToEuler(new Quaternion(1.0f, 0.0f, 0.0f, 0.0f));

            //Bug Unity is (180, 0, 0)
            Assert.AreEqual(euler.x, 0.0f, 0.01f);
            Assert.AreEqual(euler.y, 180.0f, 0.01f);
            Assert.AreEqual(euler.z, 180.0f, 0.01f);

            var q = ZTransformEx.EulerToQuat(new Vector3(0, 180, 0));

            Assert.AreEqual(q.x, 0.0f, 0.01f);
            Assert.AreEqual(q.y, 1.0f, 0.01f);
            Assert.AreEqual(q.z, 0.0f, 0.01f);
            Assert.AreEqual(q.w, 0.0f, 0.01f);

            q = ZTransformEx.EulerToQuat(new Vector3(0, 180, 180));

            Assert.AreEqual(q.x, 1.0f, 0.01f);
            Assert.AreEqual(q.y, 0.0f, 0.01f);
            Assert.AreEqual(q.z, 0.0f, 0.01f);
            Assert.AreEqual(q.w, 0.0f, 0.01f);

            q = ZTransformEx.EulerToQuat(new Vector3(0, 60, 0));

            //0.0f, 0.5f, 0f, 0.8660254f
            Assert.AreEqual(q.x, 0.0f, 0.01f);
            Assert.AreEqual(q.y, 0.5f, 0.01f);
            Assert.AreEqual(q.z, 0.0f, 0.01f);
            Assert.AreEqual(q.w, 0.8660254f, 0.01f);
        }
    }
}
