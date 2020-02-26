using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace ZP.Lib
{
#if ZP_UNIRX
    public class ZTimerProperty : ZProperty<int>
    {
        private IDisposable timer = null;

        private Action startOption = null;
        private Action endOption = null;
        private Action<int> tickOption = null;

        public IObservable<Unit> OnStartObservable =>
            Observable.FromEvent<Action>(h => () => h(), h => startOption += h, h => startOption -= h);

        public IObservable<Unit> OnEndObservable =>
            Observable.FromEvent<Action>(h => () => h(), h => endOption += h, h => endOption -= h);

        public IObservable<int> OnTickObservable =>
            Observable.FromEvent<Action<int>, int>(h => a => h(a), h => tickOption += h, removeHandler: h => tickOption -= h);


        private int period = 1;

        /// <summary>
        /// Start the Timer
        /// </summary>
        /// <param name="deadline">Tick Count, Time run time is deadline * period</param>
        /// <param name="period">Tick Period (unit: second) </param>
        /// <returns></returns>
        public ZTimerProperty Start(int deadline, int period = 1)
        {
            Cancel();

            this.period = period;
            base.Value = deadline;
            timer = UniRx.Observable.Timer(new System.TimeSpan(0, 0, period)).Repeat().Subscribe(_ => {
                innerTick();
            });

            if (startOption != null)
                startOption();

            return this;
        }

        public void Cancel()
        {
            //will start last timer
            if (timer != null)
            {
                //base.Value = 0;
                timer.Dispose();
                timer = null;
            }
        }

        public void Pause()
        {
            Cancel();

        }

        public void Resume()
        {
            timer = UniRx.Observable.Timer(new System.TimeSpan(0, 0, period)).Repeat().Subscribe(_ => {
                innerTick();
            });
        }

        public void Stop()
        {
            Cancel();

            base.Value = 0;
            if (endOption != null)
            {
                endOption();
            }
        }

        public ZTimerProperty End(Action endOp)
        {
            endOption = endOp;

            return this;
        }

        public ZTimerProperty Tick(Action<int> tickOption)
        {
            this.tickOption = tickOption;

            return this;
        }

        private void innerTick()
        {
            //break
            if (timer == null)
                return;

            if (tickOption != null)
            {
                tickOption(base.Value);
            }

            base.Value--;

            if (base.Value == 0)
            {
                timer.Dispose();    //will call inner tick

                if (endOption != null)
                {
                    endOption();
                }

                timer = null;
            }
        }
    }
#endif
}

