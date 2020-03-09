using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib;
using ZP.Lib.Matrix;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix.Domain
{
    public interface IRoomServer : IChannelCollection
    {
        int vRoomId { get; }

        //runtime
        IScheduler RoomRxScheduler { get; }

        IEnumerable<string> Clients { get; }

        //client count
        int ClientCount { get; }

        RoomStatusEnum RoomStatus { get; }

        IObservable<string> OnArrivedObservable { get; }

        IObservable<string> OnLeaveObservable { get; }

        IObservable<Unit> OnRunObservable { get; }

        IObservable<Unit> OnStopObservable { get; }

        object GetService(Type serviceType);
        T GetService<T>();

        //send pack
        IObservable<ZNull> SendPackage<T>(string action, T data);

        IObservable<ZNull> SendPackageEx<T>(string excludeClientId, string action, T data);

        IObservable<ZNull> SendPackageTo<T>(string clientId, string action, T data);

        IObservable<TResult> SendPackageTo<T, TResult>(string clientId, string action, T data);

        void BroadCastPackage<T>(string action, T data);

        void BroadCastPackageEx<T>(string excludeClientId, string action, T data);

        void BroadCastPackageTo<T>(string clientId, string action, T data);

        //post raw data
        void BroadCastRawData(string action, string data);

        void BroadCastRawDataTo(string clientId, string action, string data);

        void BroadCastRawDataEx(string excludeClientId, string action, string data);

        //client 
        void AddClient(string clientId);
        void RemoveClient(string clientId);


        bool IsClientInRoom(string clientId);

        bool IsRunInRoom(int schedulerId);

        bool IsRunInRoom();


        Task RunInRoom(Action action);
        Task RunInRoom(Func<Task> function);



        //void Run()
    }
}
