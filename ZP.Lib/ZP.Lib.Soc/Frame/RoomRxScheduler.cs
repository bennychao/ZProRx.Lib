using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.Core.Domain;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Soc
{
    class RoomRxScheduler : IScheduler, IServerRxScheduler, IGetMatrixRuntime
    {
        RoomTaskScheduler roomTaskScheduler;

        public RoomRxScheduler(RoomTaskScheduler roomTaskScheduler)
        {
            this.roomTaskScheduler = roomTaskScheduler;
        }

        public TaskScheduler TaScheduler => roomTaskScheduler;

        public DateTimeOffset Now => Scheduler.Now;

        public IZMatrixRuntime Runtime => roomTaskScheduler;

        public IDisposable Schedule(Action action)
        {
            var d = new BooleanDisposable();

            new Task(() =>
            {
                if (!d.IsDisposed)
                {
                    action();
                }

            }).Start(roomTaskScheduler);

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

            }).Start(roomTaskScheduler);

            return d;
        }
    }
}
