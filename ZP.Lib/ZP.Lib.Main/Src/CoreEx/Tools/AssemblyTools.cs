using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ZP.Lib.CoreEx.Tools
{
    public static class AssemblyTools
    {
        static public Assembly LoadFrom( string path)
        {
            try
            {
                return Assembly.LoadFrom(
#if ZP_UNITY_CLIENT && DEBUG
               "Library\\ScriptAssemblies\\" + path
#else
                path
#endif
                );
            }
            catch 
            {
                Debug.Log("load Assembly dll error " + path);
            }

            return null;
        }
    }
}
