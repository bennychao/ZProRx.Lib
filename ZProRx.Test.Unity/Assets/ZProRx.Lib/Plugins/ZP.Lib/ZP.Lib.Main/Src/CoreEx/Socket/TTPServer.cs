#if ZP_SERVER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Protocol;
using MQTTnet.Server;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Main.CommonTools;
//using ClientResponse = System.Collections.Generic.KeyValuePair<string, string>;

namespace ZP.Lib.Net
{
    internal class TTPServer : TTPSingleton<TTPServer>, IConnectable, IDisposable
    {
        private static IMqttServer mqttServer = null;

        private static List<string> connectedClientId = new List<string>();
        private List<KeyValuePair<string, Subject<string>>> RecvListeners = new List<KeyValuePair<string, Subject<string>>>();
        private List<KeyValuePair<string, Subject<ISocketPackage>>> RecvWithClientIdListeners = new List<KeyValuePair<string, Subject<ISocketPackage>>>();

        private List<Subject<string>> ConnectListeners = new List<Subject<string>>();

        private List<Subject<string>> DisConnectListeners = new List<Subject<string>>();

        private BoolFlagReactiveProperty runningFlag = new BoolFlagReactiveProperty(false);

        public bool IsRunning => runningFlag.Value.flag > 0;

        public IObservable<bool> RunningObservable => runningFlag.ToBoolObservable;

        //private Subject<string> connectListener = new Subject<string>();

        public TTPServer()
        {
        }

        public IObservable<string> OnConnected()
        {
            var ret = new Subject<string>();

            ConnectListeners.Add(ret);

            ////server need not to Subscribe
            return ret;

            //return connectListener;
        }

        public IObservable<string> OnDisConnected()
        {
            var ret = new Subject<string>();

            DisConnectListeners.Add(ret);

            ////server need not to Subscribe
            return ret;
        }


        public void Connect()
        {
            if (mqttServer != null)
                return;
            //throw new NotImplementedException();
            Task.Run(async () => { await StartMqttServer(); });

            MqttNetGlobalLogger.LogMessagePublished += MqttNetTrace_TraceMessagePublished;
        }

        public void Disconnect()
        {
            SubjectComplete();

            Task.Run(async () => { await EndMqttServer_2_7_5(); });
            Console.WriteLine("TTPServer End");
        }

        public IObservable<string> Subscribe(string topic)
        {
            var listen = RecvListeners.Find(a => string.Compare(topic, a.Key) == 0);

            Subject<string> ret = listen.Value;
            
            if (listen.Value == null)
            {
                ret = new Subject<string>();
                var k = new KeyValuePair<string, Subject<string>>(topic, ret);
                lock (RecvListeners)
                {
                    RecvListeners.Add(k);
                }
            }


            //server need not to Subscribe
            return ret;
        }

        //can return the clientId
        public IObservable<ISocketPackage> SubscribeWithClientId(string topic)
        {
            var listen = RecvWithClientIdListeners.Find(a => string.Compare(topic, a.Key) == 0);

            Subject<ISocketPackage> ret = listen.Value;

            if (listen.Value == null)
            {
                ret = new Subject<ISocketPackage>();
                var k = new KeyValuePair<string, Subject<ISocketPackage>>(topic, ret);
                lock (RecvWithClientIdListeners)
                {
                    RecvWithClientIdListeners.Add(k);
                }
            }


            //server need not to Subscribe
            return ret;
        }

        public void UnSubscribe(string topic)
        {
            lock (RecvWithClientIdListeners)
            {
                RecvWithClientIdListeners.RemoveAll(a => {
                    var bFind = string.Compare(a.Key, topic) == 0 && !a.Value.HasObservers;
                    // if (bFind) a.Value.Dispose();
                    return bFind;
                });
            }

            lock (RecvListeners)
            {
                RecvListeners.RemoveAll(a => {
                    var bFind = string.Compare(a.Key, topic) == 0 && !a.Value.HasObservers;
                  //  if (bFind) a.Value.Dispose();
                    return bFind;
                });
            }

        }

        /// <summary>
        /// Support Retry 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public IObservable<string> SendMsg(string topic, string msg)
        {
            //create in safe scheduler suit
            var fakeResp = ServerSocketPackage.CreateFakePackage(topic, msg);

            return Observable.Create<string>(observer => {

                bool bLocal = SendFakeMsg(topic, msg) || SendFakeMsgWithId(topic, fakeResp);

                if (!bLocal)
                {
                    var cancelObserver = observer.ToCancellable();

                    Task.Run(async () => {
                        if (cancelObserver.Token.IsCancellationRequested)
                        {
                            cancelObserver.OnError(new Exception("Canceled"));
                            cancelObserver.OnCompleted();
                        }
                        else
                        {
                            await PublishAsync(topic, msg);

                            cancelObserver.OnNext("OK");
                            cancelObserver.OnCompleted();
                        }

                    }, cancelObserver.Token);

                    return cancelObserver;
                }
                else
                {
                    observer.OnNext("OK");
                    observer.OnCompleted();
                }

                // not support the Cancel, only to return Disposable.Empty
                // return subscribe(observer) ?? Disposable.Empty; [UniRx Code]
                return Disposable.Empty;
            });

            // Subject<string> ret = new Subject<string>();

            // //check if topic is in local
            // bool bLocal = SendFakeMsg(topic, msg) || SendFakeMsgWithId(topic, msg);

            // if (!bLocal)
            // {
            //     Task.Run(async () => {
            //         await PublishAsync(topic, msg);
            //         ret.OnNext("OK");
            //     });
            // }
            // else
            // {
            //     return Observable.Return<string>("OK");
            // }

            // return ret;
        }


        [Conditional("DEBUG")]
        public void FakeConnect(string clientId)
        {
            connectedClientId.Add(clientId);

            //ConnectListeners.Find((obj) => obj.)

            foreach (var c in ConnectListeners)
            {
                c.OnNext(clientId);
            }
        }

        [Conditional("DEBUG")]
        public void FakeDisConnect(string clientId)
        {
            connectedClientId.Remove(clientId);

            foreach (var c in DisConnectListeners)
            {
                c.OnNext(clientId);
            }
        }

        //[Conditional("DEBUG")]
        public bool SendFakeMsg(string topic, string msg)
        {
            var listen = RecvListeners.Find(a => MatchTopic(a.Key, topic));

            //Task.Run()
            Observable.ReturnUnit().Delay(TimeSpan.FromSeconds(0.1)).Subscribe(_ =>
            {
                listen.Value?.OnNext(msg);
            });

          
            return listen.Value != null;
        }


        //only support send from server to local client
       // [Conditional("DEBUG")]
        public bool SendFakeMsgWithId(string topic, string msg)
        {
            var listen = RecvWithClientIdListeners.Find(a => MatchTopic(a.Key, topic));

            var fakeResp = ServerSocketPackage.CreateFakePackage(topic, msg);

            Observable.ReturnUnit().Delay(TimeSpan.FromSeconds(0.1)).Subscribe(_ =>
            {
                listen.Value?.OnNext(fakeResp);
            });

//            listen.Value?.OnNext(ServerSocketResponse.CreateFakeResponse(topic, msg));

            return listen.Value != null;
        }

        public bool SendFakeMsgWithId(string topic, ISocketPackage fakeResp)
        {
            var listen = RecvWithClientIdListeners.Find(a => MatchTopic(a.Key, topic));

            Observable.ReturnUnit().Delay(TimeSpan.FromSeconds(0.1)).Subscribe(_ =>
            {
                listen.Value?.OnNext(fakeResp);
            });

            //            listen.Value?.OnNext(ServerSocketResponse.CreateFakeResponse(topic, msg));

            return listen.Value != null;
        }

        private async Task StartMqttServer()
        {
            if (mqttServer == null)
            {
                // Configure MQTT server.
                var optionsBuilder = new MqttServerOptionsBuilder()
                    .WithConnectionBacklog(100)
                    .WithDefaultEndpointPort(ZPropertySocket.ClientPort)
                    .WithConnectionValidator(ValidatingMqttClients())
                    ;

                // Start a MQTT server.
                mqttServer = new MqttFactory().CreateMqttServer();
                mqttServer.ApplicationMessageReceived += MqttServer_ApplicationMessageReceived;
                mqttServer.ClientConnected += MqttServer_ClientConnected;
                mqttServer.ClientDisconnected += MqttServer_ClientDisconnected;

                mqttServer.ClientSubscribedTopic += MqttServer_ClientSubscribedTopic;
                mqttServer.ClientUnsubscribedTopic += MqttServer_ClientUnSubscribedTopic;

                mqttServer.Started += (__, ___) => this.runningFlag.WinFlag();

                mqttServer.Stopped += (__, ___) => this.runningFlag.LoseFlag();

                await Task.Run(async () => { await mqttServer.StartAsync(optionsBuilder.Build()); });
                //mqttServer.StartAsync(optionsBuilder.Build());
                Console.WriteLine("TTPServer Start!!");
            }
        }

        void MqttServer_ClientSubscribedTopic1(object sender, MqttClientSubscribedTopicEventArgs e)
        {
        }

        private void SubjectComplete()
        {
            lock (RecvWithClientIdListeners)
            {
                //foreach (var s in RecvWithClientIdListeners)
                //{
                //    s.Value.OnCompleted();
                //}

                var list = RecvWithClientIdListeners.ToList().Select(s => s.Value).ToList();
                if (list.Count > 0)
                    list.ForEach(s => s.OnCompleted());

                RecvWithClientIdListeners.Clear();
            }

            lock (RecvListeners)
            {
                //foreach (var s in RecvListeners)
                //{
                //    s.Value.OnCompleted();
                //}

                //will create new List 
                var list = RecvListeners.ToList().Select(s => s.Value).ToList();
                if (list.Count > 0)
                    list.ForEach(s => s.OnCompleted());

                RecvListeners.Clear();
            }


            lock (ConnectListeners)
            {
                //foreach (var s in ConnectListeners)
                //{
                //    s.OnCompleted();
                //}

                ConnectListeners.ToList()?.ForEach(s => s.OnCompleted());
                ConnectListeners.Clear();
            }
                       
            lock (DisConnectListeners)
            {
                //foreach (var s in DisConnectListeners)
                //{
                //    s.OnCompleted();
                //}
                DisConnectListeners.ToList().ForEach(s => s.OnCompleted());
                DisConnectListeners.Clear();
            }
        }

        private static async Task EndMqttServer_2_7_5()
        {
            if (mqttServer != null)
            {
                await mqttServer.StopAsync();

                mqttServer = null;
            }
            else
            {
                //Console.WriteLine("mqttserver=null");
            }
        }

        private static Action<MqttConnectionValidatorContext> ValidatingMqttClients()
        {
            // Setup client validator.    
            var options =new MqttServerOptions();
            options.ConnectionValidator = c =>
            {
                // TODO check the room token

                Dictionary<string, string> c_u = new Dictionary<string, string>();
                c_u.Add("9527", "username001");
                c_u.Add("client002", "username002");
                Dictionary<string, string> u_psw = new Dictionary<string, string>();
                u_psw.Add("username001", "psw001");
                u_psw.Add("username002", "psw002");

                if (c_u.ContainsKey(c.ClientId) && c_u[c.ClientId] == c.Username)
                {
                    if (u_psw.ContainsKey(c.Username) && u_psw[c.Username] == c.Password)
                    {
                        c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
                    }
                    else
                    {
                        c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                    }
                }
                else
                {
                    c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedIdentifierRejected;
                }

                //for TODO
                 c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
            };
            return options.ConnectionValidator;
        }
        
        private static void Usingcertificate(ref MqttServerOptions options)
        {
            var certificate = new X509Certificate(@"C:\certs\test\test.cer", "");
            options.TlsEndpointOptions.Certificate = certificate.Export(X509ContentType.Cert);
            var aes = new System.Security.Cryptography.AesManaged();

        }


        private static async Task PublishAsync(string topic, string msg)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msg)
                .WithAtMostOnceQoS()
                .WithRetainFlag(false)
                .Build();
            await mqttServer.PublishAsync(message);
        }

        private void MqttServer_ClientUnSubscribedTopic(object sender, MqttClientUnsubscribedTopicEventArgs e)
        {
            Console.WriteLine($"客户端[{e.ClientId}]订阅消{e.TopicFilter}");
            //connectedClientId.Add(e.ClientId);
        }

        private void MqttServer_ClientSubscribedTopic(object sender, MqttClientSubscribedTopicEventArgs e)
        {
            Console.WriteLine($"客户端[{e.ClientId}]订阅消{e.TopicFilter.Topic}");
            //mqttServer.s
        }

        private void MqttServer_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            UnityEngine.Debug.LogWarning($"MqttServer_Client   Connected ClientId:[{e.ClientId}]");
            connectedClientId.Add(e.ClientId);

            //ConnectListeners.Find((obj) => obj.)

            foreach (var c in ConnectListeners) {
                c.OnNext(e.ClientId);
            }
        }

        private void MqttServer_ClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
           UnityEngine.Debug.LogWarning($"MqttServer_Client  Disconnected ClientId:[{e.ClientId}]");
            connectedClientId.Remove(e.ClientId);

            foreach (var c in DisConnectListeners)
            {
                c.OnNext(e.ClientId);
            }
        }

        private void MqttServer_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string recv = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"客户端[{e.ClientId}]>>");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {recv}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine();
            //if (e.ApplicationMessage.Topic == "slave/json")
            //{
            //    JsonData(recv);
            //}

            List<Subject<string>> targets = new List<Subject<string>>();

            lock(RecvListeners)
            {
                //dispatch the msg
                foreach (var k in RecvListeners)
                {
                    if (MatchTopic(k.Key, e.ApplicationMessage.Topic))
                    {
                        targets.Add(k.Value);
                        //k.Value.OnNext(recv);
                        break;
                    }
                }
            }


            //dispatch
            foreach(var t in targets)
            {
                t.OnNext(recv);
            }

            List<Subject<ISocketPackage>> targetsWitchIds = new List<Subject<ISocketPackage>>();

            //lock it 
            lock(RecvWithClientIdListeners)
            {
                foreach (var k in RecvWithClientIdListeners)
                {
                    if (MatchTopic(k.Key, e.ApplicationMessage.Topic))
                    {
                        targetsWitchIds.Add(k.Value);
                        k.Value.OnNext(new ServerSocketPackage(e, recv));
                        break;
                    }
                }
            }


            //foreach (var t in targetsWitchIds)
            //{
            //    t.OnNext(new ServerSocketPackage(e, recv));
            //}
        }

        private static void MqttNetTrace_TraceMessagePublished(object sender, MqttNetLogMessagePublishedEventArgs e)
        {
            var trace = $">> [{e.TraceMessage.Timestamp:O}] [{e.TraceMessage.ThreadId}] [{e.TraceMessage.Source}] [{e.TraceMessage.Level}]: {e.TraceMessage.Message}";
            if (e.TraceMessage.Exception != null)
            {
                trace += Environment.NewLine + e.TraceMessage.Exception.ToString();
            }

            Console.WriteLine(trace);
        }


        bool MatchTopic(string key, string topic)
        {
            int index = key.IndexOf("/#");
            if (index > 0)
            {
                var subKey = key.Substring(0, key.Length - 2);
                return topic.ToLower().Contains(subKey.ToLower());
            }

            return string.Compare(key, topic, StringComparison.CurrentCultureIgnoreCase) == 0;

        }


        public void Dispose()
        {
            Disconnect();
        }

        [Conditional("DEBUG")]
        public void CheckRecvListenerCount()
        {
            if (RecvListeners.Count() != 0) throw new Exception("RecvListeners is not Clear");
            if (RecvWithClientIdListeners.Count() != 0) throw new Exception("RecvListeners is not Clear");
        }

        [Conditional("DEBUG")]
        public void CheckRecvListenerCount(int recvCount, int recvWithIdCount)
        {
            if (RecvListeners.Count() != recvCount) throw new Exception("RecvListeners is not Clear");
            if (RecvWithClientIdListeners.Count() != recvWithIdCount) throw new Exception("RecvListeners is not Clear");
        }
    }
}
#endif