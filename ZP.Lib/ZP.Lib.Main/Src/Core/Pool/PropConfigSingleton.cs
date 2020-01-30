using System;
namespace ZP.Lib
{
    public class PropConfigSingleton<T> : IConfig where T : PropConfigSingleton<T>
    {
        protected volatile static T _Instance;

        public static T Instance
        {
            get
            {
                string path = "Jsons/Config/Config.json";
                var attr = ZPropertyAttributeTools.GetTypeAttribute<PropertyConfigPathAttribute>(typeof(T));

                if (attr != null)
                {
                    path = attr.Path;
                }

                if (_Instance == null)
                {
                    lock (typeof(PropConfigSingleton<T>))
                    {
                        if (_Instance == null)
                        {
                            _Instance = ZPropertyMesh.CreateObject<T>();

                            ZPropertyPrefs.LoadFromRes(_Instance, path);

                            ZPropertyConfigs.AddConfig(_Instance as IConfig);
                        }
                    }
                }
                return _Instance;
            }
        }

        public static T CreateConfig()
        {
            return Instance;
        }
    }
}
