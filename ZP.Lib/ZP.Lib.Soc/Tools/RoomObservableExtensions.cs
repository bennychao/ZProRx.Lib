using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Soc
{
    public static class RoomObservableExtensions
    {
        public static IObservable<Unit> WaitStoped(this IRoomServer roomServer)
        {
            var server = roomServer as SocRoomServer;

            if (server?.Status == Domain.SocRoomStatusEnum.Idle)
                return Observable.Return(Unit.Default);

            return roomServer.OnStopObservable;
        }
    }
}
