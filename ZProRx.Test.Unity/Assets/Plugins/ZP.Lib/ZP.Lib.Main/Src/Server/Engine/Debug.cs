using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.Server.Domain;
using ZP.Lib.Server.Engine;

#if ZP_SERVER

namespace UnityEngine
{

    static public class Debug
    {

        static List<ILogTargeter> targeters = new List<ILogTargeter>();



        static Debug()
        {
            targeters.Add(new ConsoleLogTargeter());
        }

        static public void AddTargeter(ILogTargeter targeter)
        {
            targeters.Add(targeter);
        }

        static public void LogError(string str)
        {
            //Console.WriteLine("Error:" + str);
            targeters.ForEach(t => t.Write("Error:" + str));
            throw new Exception(str);
        }

        [Conditional("DEBUG")]
        static public void Log(string str)
        {
            var runId = MatrixRuntimeTools.GetRunId();
            var header = ">> [" + System.DateTime.Now.ToString() + System.DateTime.Now.Millisecond +  "] ";
            header += runId != null ? "[RunTime:" + runId + "] " : "";
            //Console.WriteLine(header +  str);
            targeters.ForEach(t => t.Write(header + str));
        }

        static public void LogWarning(string str)
        {
            //Console.WriteLine("Warning:" + str);

            targeters.ForEach(t => t.Write("Warning:" + str));
        }

        static public void DrawLine(Vector3 p1, Vector3 p2, Color color)
        {
            //do noting
        }

        static public void DrawLine(Vector3 p1, Vector3 p2)
        {
            //do noting
            DrawLine(p1, p2, Color.red);
        }
        static public void DrawRay(Vector3 p1, Vector3 p2, Color color)
        {
            //do noting
        }

    }
}

#endif