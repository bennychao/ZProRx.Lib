using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.Core.Domain;

namespace ZP.Lib.Core.Relation
{

    class ZPRxScheduler : IScheduler //, IServerRxScheduler, IGetMatrixRuntime
    {
        TaskScheduler taskScheduler;
        public ZPRxScheduler()
        {
            this.taskScheduler = TaskScheduler.Current;
        }

        public ZPRxScheduler(TaskScheduler roomTaskScheduler)
        {
            this.taskScheduler = roomTaskScheduler;
        }


        public TaskScheduler TaScheduler => taskScheduler;

        public DateTimeOffset Now => Scheduler.Now;

        public IZMatrixRuntime Runtime => taskScheduler as IZMatrixRuntime;

        public IDisposable Schedule(Action action)
        {
            var d = new BooleanDisposable();

            new Task(() =>
            {
                if (!d.IsDisposed)
                {
                    action();
                }

            }).Start(taskScheduler);

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

            }).Start(taskScheduler);

            return d;
        }
    }
}
