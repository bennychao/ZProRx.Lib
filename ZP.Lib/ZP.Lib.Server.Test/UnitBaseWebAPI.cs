
using UniRx;
using ZP.Lib.CoreEx;
using ZP.Matrix.Test.Tools;
using ZP.Lib.Net;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using ZP.Lib;
using NUnit.Framework;
using System.Threading.Tasks;
using ZProRx.Test.Web.Entity;
using System;
using System.Net;
using ZP.Lib.NetCore;
using Microsoft.AspNetCore.Hosting;

namespace ZP.Lib.Server.WebTest
{
    public class UnitBaseWebAPI
    {
        public HttpClient Client { get; }

        public UnitBaseWebAPI()
        {
            var server = new TestServer(ZPWebHost.CreateDefaultBuilder(null)
                //.UseNacos<Startup>() //UseTestNacos ??
                .UseUrls("http://*:6008")
                .UseStartup<ZProRx.Matrix.Web.Startup>());

            Client = server.CreateClient();

            ZPropertyNet.ConfigEngine(new TestHttpClient(server));
        }

        [Test]
        public async Task TestAPIAsync()
        {
            var id = 1;
            var response = await Client.GetAsync($"/api/v1/TestWeb/{id}");

            Assert.IsTrue(HttpStatusCode.OK == response.StatusCode);

            response = await Client.GetAsync($"/api/v1/TestWeb/{id}");

            Assert.IsTrue(HttpStatusCode.OK ==  response.StatusCode);

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var url ="http://" + Client.BaseAddress.Host + ":" + Client.BaseAddress.Port;

            //Result is TestData
            ZPropertyNet.Get<TestData>(
                //config.ArchitectMasterServer +
                url +
                $"/api/v1/TestWeb/{id}",
                null).Subscribe(recvData =>
                {
                    taskEnd.Value = true;

                    Assert.True(recvData.testNum.Value == 100);
                });

            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
            taskEnd.Value = false;
                       
            var data = ZPropertyMesh.CreateObject<TestData>();
            data.testNum.Value = 300;


            var retData = await ZPropertyNet.Get<TestData>(url + $"/api/v1/TestWeb/{id}").Fetch();
            Assert.True(retData.testNum.Value == 100);


            retData = ZPropertyNet.Get<TestData>(url + $"/api/v1/TestWeb/{id}")
                .Catch<TestData, Exception>(error =>
                {
                    data.testNum.Value = 200;

                    Assert.True(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));

                    return Observable.Return<TestData>(data);
                })
                .FetchResult();

            Assert.True(data.testNum.Value == 300);

            //not need Result
            ZPropertyNet.Post(url + $"/api/v1/TestWeb/{id}", null, data).Subscribe(_ => {});


            retData = await ZPropertyNet.Put<TestData>(url + $"/api/v1/TestWeb/{id}", null, data).Fetch();
            Assert.True(retData.testNum.Value == 100);

            //no return can't get a result
            //await ZPropertyNet.Delete(url + $"/api/v1/TestWeb/{id}", null).Fetch();

            ZPropertyNet.Delete(url + $"/api/v1/TestWeb/{id}", null).Subscribe();
        }

        [Test]
        public void TestAPIWithError()
        {
            var id = 1;

            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var url = "http://" + Client.BaseAddress.Host + ":" + Client.BaseAddress.Port;

            var data = ZPropertyMesh.CreateObject<TestData>();
            data.testNum.Value = 300;

            //not need Result
            ZPropertyNet.Post(url + $"/api/v1/test/web/TestError/{id}", null, data).Subscribe(_ => { },
                error =>
                {
                    //can not read the error
                    taskEnd.Value = true;
                });


            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;

            ZPropertyNet.Post<ZNull, TestErrorEnum>(url + $"/api/v1/test/web/TestError/{id}", null, data).Subscribe(_ => { },
                error =>
                {
                    Assert.True(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));

                    taskEnd.Value = true;
                });


            taskEnd.Where(cur => cur == true).WaitAFetch(2);
            taskEnd.Value = false;


            var retData = ZPropertyNet.Get<TestData, TestErrorEnum>(url + $"/api/v1/test/web/TestError/{id}")
                .Catch<TestData, Exception>(error =>
                {
                    data.testNum.Value = 200;

                    Assert.True(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));

                    return Observable.Return<TestData>(data);
                })
                .FetchResult();

            Assert.True(data.testNum.Value == 200);

            retData = ZPropertyNet.Put<TestData, TestErrorEnum>(url + $"/api/v1/test/web/TestError/{id}", null, data)
                .Catch<TestData, Exception>(error =>
                {
                    data.testNum.Value = 200;

                    Assert.True(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));

                    return Observable.Return<TestData>(data);
                })
                .FetchResult();

            Assert.True(data.testNum.Value == 200);

            ZPropertyNet.Delete<ZNull, TestErrorEnum>(url + $"/api/v1/test/web/TestError/{id}")
                .Catch<ZNull, Exception>(error =>
                {
                    data.testNum.Value = 200;

                    Assert.True(error.IsMultiError<TestErrorEnum>(TestErrorEnum.Error1));

                    return Observable.Return<ZNull>(ZNull.Default);
                })
                .FetchResult();


        }

        [Test]
        public void TestModel()
        {

        }
    }
}
