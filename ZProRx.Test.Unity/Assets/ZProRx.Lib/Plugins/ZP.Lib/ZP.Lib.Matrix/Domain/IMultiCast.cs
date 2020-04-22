using System;
using System.Collections.Generic;
using System.Text;
using UniRx;

namespace ZP.Lib.Matrix.Domain
{
    //from client to server
    public interface IMultiCast
    {
        IObservable<ZNull> JoinGroup(string group);
        IObservable<ZNull> LeaveGroup(string group);

        IObservable<ZNull> MultiCast<T>(string group, string action, T data = default(T));

        IObservable<ZNull> MultiCastMsg<T>(string group, T data);
    }

}
