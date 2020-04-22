using System;
namespace ZP.Lib.Net
{
    internal class TTPSingleton<T> where T : TTPSingleton<T>, IConnectable
    {
        protected static T _Instance;

        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = (T)Activator.CreateInstance(typeof(T));
                   // _Instance.Connect();
                }
                return _Instance;
            }
        }
    }
}
