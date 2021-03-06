﻿using System;
using System.Collections.Generic;

#if ZP_SERVER
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UniRx;
using ZP.Lib;
using ZP.Lib.CoreEx.Domain;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Net
{
    //user for server to send httpwebrequest, Call by ZPropertyNet
    internal class HTTPClient : TTPSingleton<HTTPClient>, IConnectable, INetHttpEngine
    {
        static private float TimeOut = 10;

        //private List<KeyValuePair<string, Subject<string>>> RecvListeners = new List<KeyValuePair<string, Subject<string>>>();

        public HttpClient Client { get; set; }

        public HTTPClient()
        {
            //[TODO]
            //ObservableMap<string> map = new ObservableMap<string>();
            //map.CreateObserver(url)            
        }


        public void Connect()
        {
            //do nothing
        }

        public void Disconnect()
        {
            //do nothing
        }

        public IObservable<string> Get(string url, Dictionary<string, string> headers = null)
        {
            return Observable.Create<string>(observer =>
            {
                var ret = observer.ToCancellable();

                Task.Run(async () => { await httpGet(url, ret, headers); }, ret.Token);

                return ret;
            });
        }

        public IObservable<string> Post(string url, string data, Dictionary<string, string> headers = null)
        {
            return Observable.Create<string>(observer =>
            {
                var ret = observer.ToCancellable();

                Task.Run(async () => { await httpPost(url, data, ret, headers); }, ret.Token);

                return ret;
            });
        }

        public IObservable<string> Post(string url, object data, Dictionary<string, string> headers = null)
        {
            return Observable.Create<string>(observer =>
            {
                var ret = observer.ToCancellable();

                Task.Run(async () => { await httpPost(url, data, ret, headers); }, ret.Token);

                return ret;
            });
        }

        public IObservable<string> Put(string url, object data, Dictionary<string, string> headers = null)
        {
            return Observable.Create<string>(observer =>
            {
                var ret = observer.ToCancellable();

                Task.Run(async () => { await httpPut(url, data, ret, headers); }, ret.Token);

                return ret;
            });
        }

        public IObservable<string> Delete(string url, Dictionary<string, string> headers = null)
        {
            return Observable.Create<string>(observer =>
            {
                var ret = observer.ToCancellable();

                Task.Run(async () => { await httpDelete(url, ret, headers); }, ret.Token);

                return ret;
            });
        }


        // /-------------------------------- inner functions ---------------------------------/ //
        static async Task httpGet(string url, ICancellableObserver<string>  k, Dictionary<string, string> headers)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(TimeOut);


                if (headers != null)
                {
                    //client.DefaultRequestHeaders = new System.Net.Http.Headers.HttpRequestHeaders();
                    foreach (var s in headers)
                    {
                        if (client.DefaultRequestHeaders.Contains(s.Key))
                        {
                            client.DefaultRequestHeaders.Remove(s.Key);
                        }

                        client.DefaultRequestHeaders.Add(s.Key, s.Value);
                    }
                }

                client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT heade

                try
                {
                    var result = await client.GetAsync(url, k.Token);
                    //Console.WriteLine(result.StatusCode);

                    if (!result.IsSuccessStatusCode)
                    {
                        k.OnError(new ZNetHttpException(result.StatusCode));
                        return;
                    }

                    string resultStr = await result.Content.ReadAsStringAsync();

                    k.OnNext(resultStr);
                    Console.WriteLine(resultStr);
                }
                catch (Exception e)
                {
                    k.OnError(e);
                }
            }

        }

        static async Task httpPost(string url, string data, ICancellableObserver<string> k, Dictionary<string, string> headers)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);


            // Request headers
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<key>");//Face API key

            client.Timeout = TimeSpan.FromSeconds(TimeOut);


            var uri = url + queryString;

            HttpResponseMessage response;

            // Request body (byte)
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            try
            {
                using (var content = new ByteArrayContent(byteData))
                {
                    if (headers != null)
                    {
                        foreach (var s in headers)
                        {
                            content.Headers.Add(s.Key, s.Value);
                        }
                    }

                    response = await client.PostAsync(uri, content, k.Token);
                }

                if (!response.IsSuccessStatusCode)
                {
                    k.OnError(new ZNetHttpException(response.StatusCode));
                    return;
                }

                //response result
                string result = await response.Content.ReadAsStringAsync();

                k.OnNext(result);

            }
            catch (Exception e)
            {
                k.OnError(e);
            }

            //Console.WriteLine("response:" + result);
        }

        static async Task httpPut(string url, string data, ICancellableObserver<string> k, Dictionary<string, string> headers)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.Timeout = TimeSpan.FromSeconds(TimeOut);

            // Request headers
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<key>");//Face API key

            var uri = url + queryString;

            HttpResponseMessage response;

            // Request body (byte)
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            try
            {
                using (var content = new ByteArrayContent(byteData))
                {
                    if (headers != null)
                    {
                        foreach (var s in headers)
                        {
                            content.Headers.Add(s.Key, s.Value);
                        }
                    }

                    response = await client.PutAsync(uri, content, k.Token);
                }
                if (!response.IsSuccessStatusCode)
                {
                    k.OnError(new ZNetHttpException(response.StatusCode));
                    return;
                }
                //response result
                string result = await response.Content.ReadAsStringAsync();

                k.OnNext(result);

            }
            catch (Exception e)
            {
                k.OnError(e);
            }

            //Console.WriteLine("response:" + result);
        }

        static async Task httpDelete(string url, ICancellableObserver<string> k, Dictionary<string, string> headers)
        {
            using (var client = new HttpClient())
            {

                client.Timeout = TimeSpan.FromSeconds(TimeOut);

                if (headers != null)
                {
                    foreach (var s in headers)
                    {
                       if (client.DefaultRequestHeaders.Contains(s.Key))
                       {
                           client.DefaultRequestHeaders.Remove(s.Key);
                       }

                       client.DefaultRequestHeaders.Add(s.Key, s.Value);
                    }

                    client.DefaultRequestHeaders
                          .Accept
                          .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT heade
                }

                try
                {
                    var result = await client.DeleteAsync(url, k.Token);
                    //Console.WriteLine(result.StatusCode);
                    if (!result.IsSuccessStatusCode)
                    {
                        k.OnError(new ZNetHttpException(result.StatusCode));
                        return;
                    }

                    string resultStr = await result.Content.ReadAsStringAsync();

                    k.OnNext(resultStr);
                    //Console.WriteLine(resultStr);
                }
                catch (Exception e)
                {
                    k.OnError(e);
                }

            }
        }


        static Task httpPost(string url, object data, ICancellableObserver<string> k, Dictionary<string, string> headers)
        {
            var str = ZPropertyPrefs.ConvertToStr(data);
            return httpPost(url, str, k, headers);
        }

        static Task httpPut(string url, object data, ICancellableObserver<string> k, Dictionary<string, string> headers)
        {
            var str = ZPropertyPrefs.ConvertToStr(data);
            return httpPut(url, str, k, headers);
        }
    }
}
#endif