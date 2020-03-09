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

        IObservable<Unit> MultiCast<T>(string group, string action, T data = default(T));

        IObservable<Unit> MultiCastMsg<T>(string group, T data);
    }

}
