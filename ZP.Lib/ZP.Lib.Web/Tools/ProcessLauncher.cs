using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ZP.Lib.Web
{
    public class ProcessLogStdoutput
    {
        Logger<ProcessLogStdoutput> stdOutputLog = null;

        public Logger<ProcessLogStdoutput> Log => stdOutputLog;

        public ProcessLogStdoutput(string appName)
        {
            var myfactory = new LoggerFactory();

            myfactory.AddNLog();

            stdOutputLog = new Logger<ProcessLogStdoutput>(myfactory);
        }
    }

    public static class ProcessLauncher
    {


        static public (int id, Process result) Launch(string path, string param, string workfolder = "")
        {

            //var str11 = System.Environment.CurrentDirectory;

            var psi = new ProcessStartInfo(path, param) { 
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            var log = new ProcessLogStdoutput(path);


            log.Log.LogWarning("Launch Path "+ path + " " + param);
            //bin/bash
            //try
            {
                psi.RedirectStandardInput = true;
                //psi.UseShellExecute = true;
                psi.WorkingDirectory = workfolder;
                var proc = Process.Start(psi);

                if (proc == null)
                {
                    log.Log.LogError("Not Find Path");
                    throw new Exception("Not Find Path");
                }
                else
                {

                    //proc.StandardInput.WriteLine("Test");

                    Task.Run(() =>
                   {
                       string retStr = "";
                       using (var sr = proc.StandardOutput)
                       {
                           while (!sr.EndOfStream)
                           {
                               var str = sr.ReadLine();
                               Console.WriteLine(str);
                               log.Log.LogWarning(str);
                               retStr += str;
                           }

                           if (!proc.HasExited)
                           {
                               proc.Kill();
                           }
                       }
                   });
                    log.Log.LogWarning("Start process" + proc.Id.ToString());
                    return (proc.Id, proc);
                }
            }
            //catch (Exception e)
            //{
            //    //Debug.Log()
            //    log.Log.LogError("Start process Error :" + e.Message);
            //    var i = e.Message;
            //    return (-1, null);
            //}

        }

        static public void SendInput(int pid, string str)
        {
            var psi = Process.GetProcessById(pid);

            psi?.StandardInput.WriteLine(str);
        }


        static public void Kill(int pid)
        {
            var psi = Process.GetProcessById(pid);          

             psi?.Kill(true);
             psi.Close();
        }
    }
}
