using System;
using System.Threading.Tasks;

namespace ZP.Lib.Matrix.Domain
{
    public interface IServerRxScheduler
    {
        TaskScheduler TaScheduler { get; }
    }
}
