using System;
using UniRx;

namespace ZP.Lib.Matrix
{

    static public class RoomScheduler
    {
        public static IScheduler MainThread
        {
            get
            {
                return RoomMatrixBehaviour.Instance.RxScheduler;
            }
        }
    }
}