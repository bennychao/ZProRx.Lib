#if ZP_UNITY_CLIENT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace ZP.Lib.CoreEx
{
    //UnityWebRequest Observables for UniRx 's WWW Rx Substitute
    static internal class UnityWebRequestObservable
    {

        public static IObservable<string> Get(string url, Dictionary<string, string> headers = null)
        {
            // convert coroutine to IObservable
            return Observable.FromCoroutine<string>((observer, cancellationToken) => InnerGet(observer, cancellationToken, url, headers));
        }

        public static IObservable<string> Put(string url, string data, Dictionary<string, string> headers = null)
        {
            // convert coroutine to IObservable
            return Observable.FromCoroutine<string>((observer, cancellationToken) => InnerPut(observer, cancellationToken, url, data, headers));
        }

        public static IObservable<string> Post(string url, string data, Dictionary<string, string> headers = null)
        {
            // convert coroutine to IObservable
            return Observable.FromCoroutine<string>((observer, cancellationToken) => InnerPost(observer, cancellationToken, url, data, headers));
        }

        public static IObservable<string> Delete(string url, Dictionary<string, string> headers = null)
        {
            // convert coroutine to IObservable
            return Observable.FromCoroutine<string>((observer, cancellationToken) => InnerDelete(observer, cancellationToken, url, headers));
        }

        static IEnumerator InnerGet(IObserver<string> observer, CancellationToken cancellationToken, string url, Dictionary<string, string> headers = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            //while (cancellationToken.IsCancellationRequested)
            //{
            //    yield return null;
            //}

            InitHeaders(webRequest, headers);

            yield return webRequest.SendWebRequest();
            //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
                observer.OnError(new Exception(webRequest.error));
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                observer.OnNext(webRequest.downloadHandler.text);
                observer.OnCompleted();
            }
        }


        static IEnumerator InnerPut(
            IObserver<string> observer,
            CancellationToken cancellationToken,
            string url,
            string data,
            Dictionary<string, string> headers = null)
        {

            var sendData = data;
            if (string.IsNullOrEmpty(sendData))
                sendData = " ";

            byte[] byteArray = System.Text.Encoding.Default.GetBytes(sendData);

            UnityWebRequest webRequest = UnityWebRequest.Put(url, byteArray);
            //webRequest.SetRequestHeader()

            //while (cancellationToken.IsCancellationRequested)
            //{
            //    yield return null;
            //}

            InitHeaders(webRequest, headers);

            yield return webRequest.SendWebRequest();
            //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
                observer.OnError(new Exception(webRequest.error));
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                observer.OnNext(webRequest.downloadHandler.text);
                observer.OnCompleted();
            }
        }


        static IEnumerator InnerPost(
            IObserver<string> observer,
            CancellationToken cancellationToken,
            string url,
            string data,
            Dictionary<string, string> headers = null)
        {

            //byte[] byteArray = System.Text.Encoding.Default.GetBytes(data);

            UnityWebRequest webRequest = UnityWebRequest.Post(url, data);

            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    yield return null;
            //}

            InitHeaders(webRequest, headers);

            yield return webRequest.SendWebRequest();
            //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
                observer.OnError(new Exception(webRequest.error));
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                observer.OnNext(webRequest.downloadHandler.text);
                observer.OnCompleted();
            }
        }

        static IEnumerator InnerDelete(
            IObserver<string> observer,
            CancellationToken cancellationToken,
            string url,
            Dictionary<string, string> headers = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Delete(url);

            //while (cancellationToken.IsCancellationRequested)
            //{
            //    yield return null;
            //}

            InitHeaders(webRequest, headers);

            yield return webRequest.SendWebRequest();
            //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
                observer.OnError(new Exception(webRequest.error));
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                observer.OnNext(webRequest.downloadHandler.text);
                observer.OnCompleted();
            }
        }

        static void InitHeaders(UnityWebRequest webRequest, Dictionary<string, string> headers)
        {
            foreach (var v in headers)
            {
                webRequest.SetRequestHeader(v.Key, v.Value);
            }
        }
    }
}

#endif