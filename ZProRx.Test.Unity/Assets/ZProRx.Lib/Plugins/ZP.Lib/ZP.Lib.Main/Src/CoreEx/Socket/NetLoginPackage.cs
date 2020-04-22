using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib;

namespace ZP.Lib.Net
{
    //not used
    [Obsolete("not used")]
    internal class NetLoginPackage<T>
    {
        private ZProperty<string> accessToken = new ZProperty<string>();
        private ZProperty<float> expiresIn = new ZProperty<float>();

        private ZProperty<string> refreshUrl = new ZProperty<string>();

        private ZProperty<T> data = new ZProperty<T>();

        public string Token => accessToken.Value;

        public T Data => data.Value;

        public float ExpiresIn => expiresIn.Value;

        public IDisposable ConfigRefresh()
        {
            //ZPropertyNet.ResetToken();

            return Observable.Timer(TimeSpan.FromSeconds(expiresIn)).Subscribe(_ => ZPropertyNet.Login<T>(refreshUrl.Value) );
        }
        
    }
}
