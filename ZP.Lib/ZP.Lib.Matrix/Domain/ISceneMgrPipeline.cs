using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Matrix.Entity;

namespace ZP.Lib.Matrix.Domain
{
    public interface ISceneMgrPipeline
    {
        //when some Scene loaded 
        IObservable<string> OnLoadedObservable { get; }

        //when some client disconnected
        //IObservable<string> OnDisConnectedObservable { get; }
    }
}
