using System;
namespace ZP.Lib.Server.Tools
{
    static public class ZPAppTools<TPrograme>
    {
        static public string AssemblePath
        {
            get {
                var projName = typeof(TPrograme).Assembly.GetName().Name;
                return projName;
            }
        }

        static public string AssembleShortName
        {
            get
            {
                var projName = typeof(TPrograme).Assembly.GetName().Name;
                var index = projName.LastIndexOf('.') + 1;
                var shortName = projName.Substring(index, projName.Length - index);
                return shortName;
            }
        }
    }

    public class ZPAppTools
    {
        Type appType;
        public string AssemblePath
        {
            get
            {
                var projName = appType.Assembly.GetName().Name;
                return projName;
            }
        }

        public string AssembleShortName
        {
            get
            {
                var projName = appType.Assembly.GetName().Name;
                var index = projName.LastIndexOf('.') + 1;
                var shortName = projName.Substring(index, projName.Length - index);
                return shortName;
            }
        }

        public ZPAppTools(Type type)
        {
            appType = type;
        }
    }
}
