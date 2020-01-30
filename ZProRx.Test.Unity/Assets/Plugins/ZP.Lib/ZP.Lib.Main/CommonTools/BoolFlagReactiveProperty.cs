using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UniRx;
using ZP.Lib.CoreEx.Reactive;

namespace ZP.Lib.Server.CommonTools
{
    public class BoolFlag
    {
        volatile public int flag = 0;
    }

    public class BoolFlagReactiveProperty : ReactiveProperty<BoolFlag>
    {
        public BoolFlagReactiveProperty()
        {
            Value = new BoolFlag();
            Value.flag = 0;
        }

        public BoolFlagReactiveProperty(bool bFlag = false)
        {
            Value = new BoolFlag();
            Value.flag = bFlag ? 1:0;
        }

        public IObservable<bool> ToBoolObservable => this.Select(v => v.flag > 0);

        public bool WinFlag(Action winFunc = null)
        {
            if (0 == Interlocked.Exchange(ref Value.flag, 1))
            {
                //Code to access a resource that is not thread safe would go here.

                if (winFunc != null)
                    winFunc();

                //Release the lock
                //Interlocked.Exchange(ref Value.flag, 0);
                SetValueAndForceNotify(Value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LoseFlag(Action loseFunc = null)
        {
            if (1 == Interlocked.Exchange(ref Value.flag, 0))
            {
                //Code to access a resource that is not thread safe would go here.

                if (loseFunc != null)
                    loseFunc();

                //Release the lock
                //Interlocked.Exchange(ref Value.flag, 0);
                SetValueAndForceNotify(Value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public IObservable<bool> WaitFlag()
        {
            return this.Select(cur => cur.flag > 0).Where(cur => cur).Fetch().Timeout(TimeSpan.FromSeconds(5));
        }
    }
}
