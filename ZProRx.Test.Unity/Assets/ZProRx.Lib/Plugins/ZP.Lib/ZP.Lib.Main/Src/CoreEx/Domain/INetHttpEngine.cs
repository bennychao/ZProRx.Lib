using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Net
{
    public interface INetHttpEngine
    {
        IObservable<string> Delete(string url, Dictionary<string, string> headers = null);

        IObservable<string> Get(string url, Dictionary<string, string> headers = null);

        IObservable<string> Post(string url, object data, Dictionary<string, string> headers = null);

        IObservable<string> Post(string url, string data, Dictionary<string, string> headers = null);

        IObservable<string> Put(string url, object data, Dictionary<string, string> headers = null);
    }
}
