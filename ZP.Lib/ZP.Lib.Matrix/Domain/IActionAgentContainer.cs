using System;
using UniRx;

namespace ZP.Lib.Matrix
{
    public interface IActionAgentContainer<TAction>
    {
        void RegisterAgent(string clientId, IActionAgent<TAction> agent);
        void UnRegisterAgent(string clientId);
        //IObservable<Unit> PostAction<TData>(TAction cmd, TData data);
    }


}
