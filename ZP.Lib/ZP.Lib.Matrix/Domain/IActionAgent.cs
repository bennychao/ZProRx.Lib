using System;
using ZP.Lib;

namespace ZP.Lib.Matrix
{
    public interface IActionAgent<TAction>
    {
        void OnAction(TAction action, IRawDataPref rawData);
    }
}
