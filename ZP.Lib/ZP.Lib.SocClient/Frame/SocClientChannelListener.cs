using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib;
using ZP.Lib.Core.Main;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.SocClient
{
    class SocClientChannelListener : PropObjectSingletonWIthId<string, SocClientChannelListener>
    {
        public string BaseUrl
        {
            get; set;
        }

        IObservable<ZRoom> connectObservable = null;

        public IObservable<ZRoom> RoomConnectObservable(int vroomId, SocConnectPackage socConnectPackage)
        {
            return  connectObservable ?? (connectObservable = 
                ZPropertySocket.SendPackage<SocConnectPackage, ZRoom>(BaseUrl + "connect", socConnectPackage));
        }

        IObservable<ZNull> disconnectObservable = null;

        public IObservable<ZNull> RoomDisconnectObservable(int vroomId)
        {
            return disconnectObservable ?? (disconnectObservable =
                ZPropertySocket.Post(BaseUrl + "disconnect"));
        }

    }
}
