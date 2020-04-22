using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.Core.Domain;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.SocClient
{
    internal class SocClientRxScheduler : IScheduler, IGetMatrixRuntime
    {
        SocClientTaskScheduler clientTaskScheduler;
        public IZMatrixRuntime Runtime => clientTaskScheduler;

        public SocClientRxScheduler(SocClientTaskScheduler roomTaskScheduler)
        {
            this.clientTaskScheduler = roomTaskScheduler;
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

            }).Start(clientTaskScheduler);

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

            }).Start(clientTaskScheduler);

            return d;
        }
    }
}
