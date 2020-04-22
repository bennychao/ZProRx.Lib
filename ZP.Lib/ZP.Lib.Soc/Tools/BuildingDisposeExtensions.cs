using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib.Soc.Domain;

namespace ZP.Lib.Soc
{
    static public class BuildingDisposeExtensions
    {
        public static IDisposable AddTo<T>(this T disposable, ISocAppBuilder app)
            where T : IDisposable
        {

            CompositeDisposable disposables = new CompositeDisposable();

            disposables.Add(disposable);
            if (app == null || !app.IsRunning)
            {
                disposable.Dispose();
                return disposable;
            }

            app.OnStopObservable.Subscribe(_ => disposable.Dispose()).AddTo(disposables);

            return disposables;
        }

    }
}
