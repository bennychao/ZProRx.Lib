using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

namespace ZP.Lib.CoreEx
{
    internal class ZRuntimeRxScheduler : IScheduler
    {
        private readonly static ZRuntimeRxScheduler zRuntimeRx = new ZRuntimeRxScheduler();
        public static ZRuntimeRxScheduler Current => zRuntimeRx;
        private TaskScheduler taskScheduler;

        public ZRuntimeRxScheduler(TaskScheduler taskScheduler = null)
        {
            this.taskScheduler = taskScheduler;
        }

        public DateTimeOffset Now => Scheduler.Now;

        public IDisposable Schedule(Action action)
        {
            var d = new BooleanDisposable();

            new Task(() =>
            {
                if (!d.IsDisposed)
                {
                    action();
                }

            }).Start(taskScheduler ?? TaskScheduler.Current);

            return d;
        }

        public IDisposable Schedule(TimeSpan dueTime, Action action)
        {
            var wait = Scheduler.Normalize(dueTime);

            var d = new BooleanDisposable();

            new Task(() =>
            {
                if (!d.IsDisposed)
                {
                    if (wait.Ticks > 0)
                    {
                        Thread.Sleep(wait);
                    }

                    action();
                }

            }).Start(taskScheduler ?? TaskScheduler.Current);

            return d;
        }
    }
}
