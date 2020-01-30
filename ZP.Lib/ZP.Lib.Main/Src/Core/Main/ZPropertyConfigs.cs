using System;
using System.Collections.Generic;

namespace ZP.Lib
{
    public static class ZPropertyConfigs
    {
        private volatile static List<IConfig> configs = new List<IConfig>();

        public static void AddConfig(IConfig config)
        {
            configs.Add(config);
        }

        public static T GetConfig<T>() where T : PropConfigSingleton<T>
        {
            var c =  configs.Find(o => typeof(T).IsAssignableFrom(o.GetType()));
            if (c == null)
                return PropConfigSingleton<T>.CreateConfig();

            else
                return (T)c;
        }
    }
}
