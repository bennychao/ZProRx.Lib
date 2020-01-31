using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Net;
using ZP.Lib;
using UniRx;
using ZP.Lib.CoreEx.Reactive;
using UnityEngine;
using System.Threading;
using ZP.Lib.Server.CommonTools;
using ZP.Lib.CoreEx;
using ZP.Lib.Core.Main;
using System.Diagnostics;

namespace ZP.Lib.Server.Test
{
    public class UnitNetAndSocket
    {
        object lockObj = new object();
        [SetUp]
        public void Setup()
        {
            ZPropertySocket.Start();
        }

        public class TestPropData
        {
            public ZProperty<string> name = new ZProperty<string>();
        }

        [Test]
        public void TestBaseSocket()
        {
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            //test raw string 
            ZPropertySocket.ReceiveRaw("topic/rawstr").Subscribe(str => 
             {
                    Assert.IsTrue(string.Compare(str, "TestData") == 0);                   
                    taskEnd.Value = true;
            });

            ZPropertySocket.Post("topic/rawstr", "TestData").Subscribe();

            ZPropertySocket.Send("topic/rawstr", "TestData").Subscribe();

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            //test raw ZP Object
            ZPropertySocket.ReceiveRaw<TestPropData>("topic/rawstr").Subscribe(a1 =>
            {
                Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);
                taskEnd.Value = true;
            });

            ZPropertySocket.Post("topic/rawstr", data).Subscribe();

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            //test package recevie, data is NetPackage<TestPropData>
            ZPropertySocket.ReceivePackage<TestPropData>("topic/response", null).
            Subscribe((TestPropData a1) => {
                //Debug.Log("Get Response " + a1.name.Value.ToString());

                Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);
                taskEnd.Value = true;
            });//, (Exception e) => Debug.Log(e.ToString())
           //);

            Dictionary<string, string> query = new Dictionary<string, string>();
            //query.Add("1", "1122");

            ZPropertySocket.SendPackage<TestPropData, ZNull>("topic/response", data).Subscribe();

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            //ZPropertyNet.Post("http://localhost:5000/api/values", query, data).Subscribe(_ => Debug.Log( "Test"));


        }

        [Test]
        public void TestBaseSocketWithResponse()
        {
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            IReactiveProperty<bool> taskEnd2 = new ReactiveProperty<bool>(false);

            //one topic not support listen two times [BUG]
            var disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData, bool>("topic/response2", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    //Debug.Log("Get Response " + a1.name.Value.ToString());

                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    taskEnd.Value = true;
                    return true;
                });//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, bool>("topic/response2", data)
                .Subscribe(bRet =>
                {
                    taskEnd2.Value = true;
                    Assert.IsTrue(bRet);
                });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            taskEnd2.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            taskEnd.Value = false;
            taskEnd2.Value = false;

            disp.Dispose();

            TestPropData data2 = ZPropertyMesh.CreateObject<TestPropData>();
            data2.name.Value = "SendTwoTimesName";
            ZPropertySocket.SendPackage<TestPropData, bool>("topic/response2", data2)
                .Subscribe(bRet =>
                {
                    taskEnd2.Value = true;
                    Assert.IsTrue(bRet);
                });


            //return ZNull ZNull will not have size , and it can't be omit
            ZPropertySocket.ReceivePackageAndResponse<TestPropData>("topic/response3", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    //Debug.Log("Get Response " + a1.name.Value.ToString());

                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    taskEnd.Value = true;
                    return ZNull.Default; 
                });//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, ZNull>("topic/response3", data)
                .Subscribe(bRet =>
                {
                    taskEnd2.Value = true;
                    //Assert.IsTrue(bRet);
                });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            taskEnd2.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
        }

        [Test]
        public void TestSocketPreformance()
        {
            var callCount = new InterCountReactiveProperty(0);
            ZPropertySocket.ReceiveRaw("topic/rawstr2").Subscribe(str =>
            {
                Assert.IsTrue(string.Compare(str, "TestData") == 0);
                //Interlocked.Increment(ref callCount.Value);
                callCount.Increment();
                //lock(lockObj)
                //    callCount.Value++; //it not thread safe
            });

            for(int i = 0; i < 100; i++)
            {
                ZPropertySocket.Send("topic/rawstr2", "TestData").Subscribe();
            }
            //ZPropertySocket.Post("topic/rawstr", "TestData").Subscribe();


            //callCount.Where(cur => cur.Count >= 100).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callCount.WaitFor(cur => cur >= 100).ToTask().Wait();
        }

        [Test]
        public void TestSocketErrorPackage()
        {
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData>("topic/responseError", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    //throw new Exception(""); 
                    throw new ZNetException(ZNetErrorEnum.ActionError);

                    return ZNull.Default;
                });//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, ZNull>("topic/responseError", data)
            .Subscribe(bRet =>
            {
                //taskEnd.Value = true;
                            //Assert.IsTrue(bRet);
            }, error =>
            {
                //Assert.IsTrue((error as ZNetException<ZNetErrorEnum>).Error == ZNetErrorEnum.ActionError);
                Assert.IsTrue(error.IsError(ZNetErrorEnum.ActionError));
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            taskEnd.Value = false;
            disp.Dispose();            

        }

        enum TestErrorEnum
        {
            BaseError = ZNetErrorEnum.MaxError + 0x100,
            Error1,
            Error2,
            Error3,
            Error4,
        }

        [Test]
        public void TestSocketCustomError()
        {
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData,TestErrorEnum, bool>("topic/responseCustomError", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    //throw new Exception(""); 
                    throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    return true;
                });//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, TestErrorEnum, bool>("topic/responseCustomError", data)
            .Subscribe(bRet =>
            {
                taskEnd.Value = true;
                //Assert.IsTrue(bRet);
            }, error =>
            {
                //Assert.IsTrue((error as ZNetException<ZNetErrorEnum>).Error == ZNetErrorEnum.ActionError);
                Assert.IsTrue(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;
            disp.Dispose();

            //test for common TError
            disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData, bool>("topic/responseCustomError", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    //throw new Exception(""); 
                    throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    return true;
                });

            ZPropertySocket.SendPackage2<TestPropData, bool>("topic/responseCustomError", data)
            .Subscribe(bRet =>
            {
                taskEnd.Value = true;
                //Assert.IsTrue(bRet);
            }, error =>
            {
                //Assert.IsTrue((error as ZNetException<ZNetErrorEnum>).Error == ZNetErrorEnum.ActionError);
                Assert.IsTrue(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
        }

        [Test]
        public void TestRawPackageAndResponse()
        {
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            ZPropertySocket.ReceiveRawPackageAndResponse<TestPropData, TestErrorEnum, bool>("topic/rawPackAndResponse", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1, ISocketPackage rawPack) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    //throw new Exception(""); 
                    throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    return true;
                },
                ()=>
                {
                    Assert.IsTrue(true);
                });//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, TestErrorEnum, bool>("topic/rawPackAndResponse", data)
            .Subscribe(bRet =>
            {
                taskEnd.Value = true;
                //Assert.IsTrue(bRet);
            }, error =>
            {
                //Assert.IsTrue((error as ZNetException<ZNetErrorEnum>).Error == ZNetErrorEnum.ActionError);
                Assert.IsTrue(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            //test for receive RawDataRef and ISocketPackage
            ZPropertySocket.ReceiveRawPackageAndResponse("topic/rawDatarefAndResponse", null).
                Subscribe(             //< TestPropData, bool> support return 
                (IRawDataPref rawData, ISocketPackage rawPack) => {
                    Assert.IsTrue(string.Compare(rawPack.Key, "1000") == 0);

                    //throw new Exception(""); 
                    //throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    //get data from IRawDataRef
                    var rdata = rawData.GetData<TestPropData>();
                    Assert.IsTrue(string.Compare(rdata.name.Value, "testobjectname") == 0);

                    //return result
                    return true;
                });

            ZPropertySocket.SendPackage<TestPropData, TestErrorEnum, bool>("topic/rawDatarefAndResponse", data)
                .Subscribe(bRet =>
                {
                    taskEnd.Value = true;
                    Assert.IsTrue(bRet);
                }, error =>
                {
                    //Assert.IsTrue((error as ZNetException<ZNetErrorEnum>).Error == ZNetErrorEnum.ActionError);
                    Assert.IsTrue(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));
                    taskEnd.Value = true;
                });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
        }

        [Test]
        public void TestIRawPackage()
        {
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            ZPropertySocket.ReceiveLowRawPackage("topic/IrawPack", null).
                Subscribe(             //< TestPropData, bool> support return 
                (ISocketPackage rawPack) => {

                    var obj =  rawPack.Value;

                    var netPack = NetPackage<TestPropData, ZNetErrorEnum>.Parse(rawPack);


                    Assert.IsTrue(string.Compare(netPack.data.name.Value, "testobjectname") == 0);

                    //not return to the client caller
                    //throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    //not support return
                    //return true;

                    taskEnd.Value = true;
                },
                () =>
                {
                    Assert.IsTrue(true);
                });//, (Exception e) => Debug.Log(e.ToString())

            //with no result should use PostPackage
            ZPropertySocket.PostPackage<TestPropData>("topic/IrawPack", data)
            .Subscribe(bRet =>
            {
                //taskEnd.Value = true;
                //Assert.IsTrue(bRet);
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;
        }

        [Conditional("DEBUG")]
        [Test]
        public void TestSocketStartAndConnectHandler()
        {
            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            ZPropertySocket.OnConnected().Subscribe(client => {               
                Assert.IsTrue(string.Compare(client, "testId") == 0);
                taskEnd.Value = true;
            });

            ZPropertySocket.FakeConnect("testId");
#if DEBUG
            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
#endif
            //FakeDisConnect
        }

        [Test]
        public void TestSocketErrorHandler()
        {
            //to Catch the error
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData, TestErrorEnum, bool>("topic/responseErrorHandler", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    //throw new Exception(""); 
                    throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    return true;
                });//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, TestErrorEnum, bool>("topic/responseErrorHandler", data)
               // .Catch<ZException, bool>((e)=> return )
               .CatchIgnore((ZException ex) =>
               {
                   Assert.IsTrue(ex.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));
                   Assert.IsTrue(true);

                   taskEnd.Value = true;
               })
            .Subscribe(bRet =>
            {
                //taskEnd.Value = true;
                //Assert.IsTrue(bRet);
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;
            disp.Dispose();

            //test for common TError
            disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData, bool>("topic/responseErrorHandler2", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    //throw new Exception(""); 
                    throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    return true;
                });

            ZPropertySocket.SendPackage2<TestPropData, bool>("topic/responseErrorHandler2", data)
            .Catch<bool, ZException>((e) => {
                return Observable.Return(true);
            })
            .Subscribe(bRet =>
            {
                taskEnd.Value = true;
                Assert.IsTrue(bRet);
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            //receive error msg

            //socket CutOff 
        }

        [Test]
        public void TestSocketCloseHandler()
        {

            ZPropertySocket.RunningObservable.Where(r => r == true).Fetch().Timeout(TimeSpan.FromSeconds(3)).Wait();

            Assert.IsTrue(ZPropertySocket.IsConnected() == true);

            Thread.Sleep(300);

            Assert.IsTrue(ZPropertySocket.IsConnected() == true);

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);
            IReactiveProperty<bool> taskCompleted = new ReactiveProperty<bool>(false);

            //observable will  recevice completed
            ZPropertySocket.ReceiveRaw("topic/testClose").Subscribe(str =>
            {
                Assert.IsTrue(false);
                //taskEnd.Value = true;
            }, ()=>
            {
                taskCompleted.Value = true;
            });

            //if not have Where it will trigger one time when is already in 1. Where(b =>b == false)
            ZPropertySocket.RunningObservable.Where(b => b == false).Subscribe(bRunning =>
            {
                Assert.IsTrue(true);
                Assert.IsTrue(bRunning == false);

                taskEnd.Value = true;
            });

            ZPropertySocket.Close();

            Observable.WhenAll(
                taskEnd.Where(cur => cur == true).Fetch(),
                taskCompleted.Where(cur => cur == true).Fetch()
                ).
                Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

        }

        [Test]
        public void TestRetry()
        {
        }

        [Test]
        public void TestSocketException()
        {
            //wait for to running
            Thread.Sleep(2000);

            Assert.IsTrue(ZPropertySocket.IsConnected() == true);

            ZPropertySocket.ReceiveRaw("topic/testClose").Subscribe(str =>
            {
                Assert.IsTrue(false);
                //taskEnd.Value = true;
            },
            (error) =>
            {
                Assert.IsTrue(true);
            },
            () =>
            {
                // taskCompleted.Value = true;
                Assert.IsTrue(false);
            });

            Observable.Return(true).Delay(TimeSpan.FromSeconds(10)).Wait();
        }
    }
}
