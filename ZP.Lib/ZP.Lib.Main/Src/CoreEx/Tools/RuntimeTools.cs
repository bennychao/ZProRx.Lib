using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZP.Lib.Core.Domain;

namespace ZP.Lib.CoreEx.Tools
{
    public static class MatrixRuntimeTools
    {
        static readonly public string RoomServer = "RoomServer";

        static public string GetRunId()
        {            
            return (TaskScheduler.Current as IZMatrixRuntime)?.RunId;
        }

        static public bool IsInServer(){
            return (TaskScheduler.Current as IZMatrixRuntime)?.IsServer ?? false;
        }

        static public bool IsServerId(string clientId){
            return clientId.Contains("RoomServer");
        }
    }
}
