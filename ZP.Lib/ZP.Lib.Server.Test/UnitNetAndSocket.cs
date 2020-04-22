using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Net;
using ZP.Lib;
using UniRx;
using ZP.Lib.CoreEx;
using UnityEngine;
using System.Threading;
using ZP.Lib.Main.CommonTools;
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
            MultiDisposable disposables = new MultiDisposable();
            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            //test raw string 
            ZPropertySocket.ReceiveRaw("topic/rawstr").Subscribe(str => 
             {
                    Assert.IsTrue(string.Compare(str, "TestData") == 0);                   
                    taskEnd.Value = true;
            }).AddTo(disposables);

            ZPropertySocket.Post("topic/rawstr", "TestData").Subscribe();

            ZPropertySocket.Send("topic/rawstr", "TestData").Subscribe().AddTo(disposables);

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            //test raw ZP Object
            ZPropertySocket.ReceiveRaw<TestPropData>("topic/rawstr").Subscribe(a1 =>
            {
                Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);
                taskEnd.Value = true;
            }).AddTo(disposables);

            ZPropertySocket.Post("topic/rawstr", data).Subscribe();

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            //test package recevie, data is NetPackage<TestPropData>
            ZPropertySocket.ReceivePackage<TestPropData>("topic/response", null).
            Subscribe((TestPropData a1) => {
                //Debug.Log("Get Response " + a1.name.Value.ToString());

                Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);
                taskEnd.Value = true;
            }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())
           //);

            Dictionary<string, string> query = new Dictionary<string, string>();
            //query.Add("1", "1122");

            //add disposable to dispose the result listener, there is noresult so you need to dispose to release the result listen
            //you need use PostPackage
            ZPropertySocket.SendPackage<TestPropData, ZNull>("topic/response", data).Subscribe().AddTo(disposables);

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            //ZPropertyNet.Post("http://localhost:5000/api/values", query, data).Subscribe(_ => Debug.Log( "Test"));

            disposables.Dispose();

            Thread.Sleep(1000);

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestBaseSocketWithResponse()
        {
            MultiDisposable disposables = new MultiDisposable();

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
                }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, bool>("topic/response2", data)
                .Subscribe(bRet =>
                {
                    taskEnd2.Value = true;
                    Assert.IsTrue(bRet);
                });//.AddTo(disposables);

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
                }).AddTo(disposables);


            //return ZNull ZNull will not have size , and it can't be omit
            ZPropertySocket.ReceivePackageAndResponse<TestPropData>("topic/response3", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    //Debug.Log("Get Response " + a1.name.Value.ToString());

                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    taskEnd.Value = true;
                    return ZNull.Default; 
                }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())

            ZPropertySocket.SendPackage<TestPropData, ZNull>("topic/response3", data)
                .Subscribe(bRet =>
                {
                    taskEnd2.Value = true;
                    //Assert.IsTrue(bRet);
                });//.AddTo(disposables);

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            taskEnd2.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            disposables.Dispose();

            
#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestSocketPreformance()
        {
            MultiDisposable disposables = new MultiDisposable();

            var callCount = new InterCountReactiveProperty(0);
            ZPropertySocket.ReceiveRaw("topic/rawstr2").Subscribe(str =>
            {
                Assert.IsTrue(string.Compare(str, "TestData") == 0);
                //Interlocked.Increment(ref callCount.Value);
                callCount.Increment();
                //lock(lockObj)
                //    callCount.Value++; //it not thread safe
            }).AddTo(disposables);

            for(int i = 0; i < 100; i++)
            {
                ZPropertySocket.Post("topic/rawstr2", "TestData").Subscribe();
            }
            //ZPropertySocket.Post("topic/rawstr", "TestData").Subscribe();


            //callCount.Where(cur => cur.Count >= 100).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callCount.WaitFor(cur => cur >= 100).ToTask().Wait();

            disposables.Dispose();
#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestSocketErrorPackage()
        {
            MultiDisposable disposables = new MultiDisposable();

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
                }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())

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

            disposables.Dispose();
#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
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
            MultiDisposable disposables = new MultiDisposable();

            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData,TestErrorEnum, bool>("topic/responseCustomError", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);

                    //throw new Exception(""); 
                    throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

                    return true;  //must have return, or else Sender will not recevie the result even the Error Exception
                }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())

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
                }).AddTo(disposables);

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
            taskEnd.Value = false;

            //Test SendPackage2 with no Error
            disp = ZPropertySocket.ReceivePackageAndResponse<TestPropData, bool>("topic/responseSend2WithNoError", null).
                Subscribe(             //< TestPropData, bool> support return 
                (TestPropData a1) => {
                    Assert.IsTrue(string.Compare(a1.name.Value, "testobjectname") == 0);
                    return true;
                }).AddTo(disposables);

            ZPropertySocket.SendPackage2<TestPropData, bool>("topic/responseSend2WithNoError", data)
            .Subscribe(bRet =>
            {
                taskEnd.Value = true;
                //Assert.IsTrue(bRet);
            }, error =>
            {
                //Assert.IsTrue((error as ZNetException<ZNetErrorEnum>).Error == ZNetErrorEnum.ActionError);
                Assert.IsTrue(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));
                Assert.IsTrue(false);

                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            disposables.Dispose();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestRawPackageAndResponse()
        {
            MultiDisposable disposables = new MultiDisposable();

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
                }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())

            var disp = ZPropertySocket.SendPackage<TestPropData, TestErrorEnum, bool>("topic/rawPackAndResponse", data)
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
                }).AddTo(disposables);

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

            disposables.Dispose();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestIRawPackage()
        {
            MultiDisposable disposables = new MultiDisposable();

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
                }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())

            //with no result should use PostPackage
            ZPropertySocket.PostPackage<TestPropData>("topic/IrawPack", data)
            .Subscribe(bRet =>
            {
                //taskEnd.Value = true;
                //Assert.IsTrue(bRet);
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
            taskEnd.Value = false;

            disposables.Dispose();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Conditional("DEBUG")]
        [Test]
        public void TestSocketStartAndConnectHandler()
        {
            MultiDisposable disposables = new MultiDisposable();

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            ZPropertySocket.OnConnected().Subscribe(client => {               
                Assert.IsTrue(string.Compare(client, "testId") == 0);
                taskEnd.Value = true;
            }).AddTo(disposables);

            ZPropertySocket.FakeConnect("testId");
#if DEBUG
            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();
#endif
            //FakeDisConnect

            disposables.Dispose();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestSocketErrorHandler()
        {
            MultiDisposable disposables = new MultiDisposable();

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
                }).AddTo(disposables);//, (Exception e) => Debug.Log(e.ToString())

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
                }).AddTo(disposables);

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

            disposables.Dispose();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestSocketCloseHandler()
        {
            MultiDisposable disposables = new MultiDisposable();

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
            }).AddTo(disposables);

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
            disposables.Dispose();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }

        [Test]
        public void TestRetry()
        {
        }

        [Test]
        public void TestMultiSubscribe()
        {
            MultiDisposable disposables = new MultiDisposable();

            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            var count = new InterCountReactiveProperty(0);

            //test raw string 
            var propObser = ZPropertySocket.ReceivePackage<TestPropData>("topic/multisubscribe");

            propObser.Subscribe(str =>
            {
                count.Increment();
            }).AddTo(disposables);

            propObser.Subscribe(str =>
            {
                count.Increment();
            }).AddTo(disposables);

            ZPropertySocket.PostPackage<TestPropData>("topic/multisubscribe", data).Subscribe();

            ZPropertySocket.PostPackage<TestPropData>("topic/multisubscribe", data).Subscribe();

            count.Where(cur => cur.Count == 4).WaitAFetch(2);

            disposables.Dispose();
        }

        [Test]
        public void TestMultiReceive()
        {
            Subject<bool> subject = new Subject<bool>();

            var dis = subject.Subscribe();
            dis.Dispose();

            TestPropData data = ZPropertyMesh.CreateObject<TestPropData>();
            data.name.Value = "testobjectname";

            var count = new InterCountReactiveProperty(0);

            //test raw string 
            var propObser = ZPropertySocket.ReceivePackage<TestPropData>("topic/multisubscribe");

            var disp = propObser.Subscribe(str =>
            {
                count.Increment();
            });

            var propObser2 = ZPropertySocket.ReceivePackage<TestPropData>("topic/multisubscribe");

            var dis2 = propObser2.Subscribe(str =>
            {
                count.Increment();
            });

            ZPropertySocket.PostPackage<TestPropData>("topic/multisubscribe", data).Subscribe();

            count.Where(cur => cur.Count == 2).WaitAFetch(2);

            disp.Dispose();
            dis2.Dispose();


            ZPropertySocket.PostPackage<TestPropData>("topic/multisubscribe", data).Subscribe();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif


        }

        [Test]
        public void TestSocketException()
        {
            MultiDisposable disposables = new MultiDisposable();

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
            }).AddTo(disposables);

            Observable.Return(true).Delay(TimeSpan.FromSeconds(10)).Wait();

            disposables.Dispose();

#if DEBUG
            ZPropertySocket.CheckRecvListenerCount();
#endif
        }
    }
}
