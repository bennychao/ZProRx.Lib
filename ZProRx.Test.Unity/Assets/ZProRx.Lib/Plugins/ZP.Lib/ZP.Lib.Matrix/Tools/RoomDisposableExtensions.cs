using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix
{
    public static  class RoomDisposableExtensions
    {
        public static IDisposable AddTo<T>(this T disposable, BaseChannel channel)
    where T : IDisposable
        {

            CompositeDisposable disposables = new CompositeDisposable();

            disposables.Add(disposable);
            if (channel == null)
            {
                disposable.Dispose();
                return disposable;
            }

            channel.StatusChanged.Where(s => s == Domain.ChannelStatusEnum.Closed).Subscribe(_ => disposable.Dispose()).AddTo(disposables);

            return disposables;
        }
    }
}
