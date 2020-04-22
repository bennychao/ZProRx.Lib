using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix
{
    public static class ChannelObservableExtensions
    {

        public static IObservable<ZNull> WaitClientCounted(this IRoomServer roomServer, int count)
        {
            IReactiveProperty<int> linkedClientCount = new ReactiveProperty<int>(0);

            lock (roomServer)
                linkedClientCount.Value = roomServer.ClientCount;


            return Observable.Create<ZNull>(observer =>
            {
                MultiDisposable disposables = new MultiDisposable();

                roomServer.OnArrivedObservable
                .ObserveOn(ZPRxScheduler.CurrentScheduler)
                .Subscribe(clientId => {
                       //Debug.Log($"BasePipeline  WaitConnected  OnConnectedObservable:{clientId}");
                       linkedClientCount.Value++;
                }).AddTo(disposables);

                roomServer.OnLeaveObservable
                .ObserveOn(ZPRxScheduler.CurrentScheduler)
                .Subscribe(clientId =>
                {
                    linkedClientCount.Value--;
                })
                .AddTo(disposables);

                linkedClientCount.Where(a => a == count)
                .Select(_ => ZNull.Default).Subscribe(observer)
                .AddTo(disposables);

                return disposables;
            });
        }

        public static IObservable<ZNull> WaitClientCounted(this IRoomClient roomClient, int count)
        {
            IReactiveProperty<int> linkedClientCount = new ReactiveProperty<int>(0);

            return Observable.Create<ZNull>(observer =>
               {
                   MultiDisposable disposables = new MultiDisposable();

                   roomClient.OnRunObservable
                   .ObserveOn(ZPRxScheduler.CurrentScheduler)
                   .Subscribe(clientId => {
                       //Debug.Log($"BasePipeline  WaitConnected  OnConnectedObservable:{clientId}");
                       linkedClientCount.Value++;
                   }).AddTo(disposables);

                   roomClient.OnStopObservable
                   .ObserveOn(ZPRxScheduler.CurrentScheduler)
                   .Subscribe(clientId => linkedClientCount.Value--)
                   .AddTo(disposables);
                   
                   linkedClientCount.Where(a => a >= count)
                   .Select(_ => ZNull.Default).Subscribe(observer)
                   .AddTo(disposables);

                   return disposables;
               });
        }

        public static IObservable<ZNull> WaitConnected(this BasePipeline channel, int count)
        {
            IReactiveProperty<int> linkedClientCount = new ReactiveProperty<int>(0);
            int waitCount = count;
            lock (channel)
            {
                waitCount -= channel.ClientCount;
                linkedClientCount.Value = channel.ClientCount;

                channel.OnConnectedObservable.ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(clientId => {
                    //Debug.Log($"BasePipeline  WaitConnected  OnConnectedObservable:{clientId}");
                    linkedClientCount.Value++;
                });
                channel.OnDisConnectedObservable.ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(clientId => linkedClientCount.Value--);
            }

            return linkedClientCount.Where(a => a >= count).Select(_ => ZNull.Default);

            //return Observable.Return(ZNull.Default);
        }

        public static IObservable<ZNull> WaitChannelCounted(this BasePipeline channel, int count)
        {
            IReactiveProperty<int> linkedClientCount = new ReactiveProperty<int>(0);
            int waitCount = count;
            lock (channel)
            {
                waitCount -= channel.ClientCount;
                linkedClientCount.Value = channel.ClientCount;

                channel.OnConnectedObservable.ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(clientId => {
                    //Debug.Log($"BasePipeline  WaitConnected  OnConnectedObservable:{clientId}");
                    linkedClientCount.Value++;
                });
                channel.OnDisConnectedObservable.ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(clientId => linkedClientCount.Value--);
            }

            return linkedClientCount.Where(a => a == count).Select(_ => ZNull.Default);

            //return Observable.Return(ZNull.Default);
        }

        public static IObservable<ZNull> CheckAndWaitStatus(this BaseChannel channel, ChannelStatusEnum status)
        {
            if (channel.Status == status)
            {
                return Observable.Return(ZNull.Default);
            }

            return channel.StatusChanged.Where(s => s == status).Select(_ => ZNull.Default);
            //return Observable.Return(ZNull.Default);
        }

        public static IObservable<ZNull> CheckClientAndWaitStatus(this IChannelClient channel, ChannelStatusEnum status)
        {
            if (channel.Status == status)
            {
                return Observable.Return(ZNull.Default);
            }

            return channel.StatusChanged.Where(s => s == status).Select(_ => ZNull.Default);
            //return Observable.Return(ZNull.Default);
        }

    }
}
