using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace ZP.Lib.Core.Pool
{
    public class SinglenodeItem<T> where T : class
    {
        public T Instance = null;

        public TaskScheduler curScheduler = null;

        public IScheduler rxScheduler = null;
    }
}
