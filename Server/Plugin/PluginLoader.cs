using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Server
{
    public static class PluginLoader
    {
        private static readonly string _pluginFolder = "./";
        private static readonly string _className = "PluginEntry";

        private static readonly Dictionary<string, object> _configParams = new Dictionary<string, object>();

        public static void Load(RibbonPage page)
        {
            InitConfig();

            var files = Directory.GetFiles(_pluginFolder, "Plugin.*.dll", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    Assembly dll = Assembly.LoadFrom(file);
                    var filename = Path.GetFileNameWithoutExtension(file);

                    Type classType = dll.GetType(String.Format("{0}.{1}", filename, _className));
                    if (classType != null)
                    {
                        Activator.CreateInstance(classType, page, _configParams);
                    }
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.ToString());
                }
            }
        }

        private static void InitConfig()
        {
            var props = typeof(Config).GetProperties();

            foreach (var prop in props)
            {
                _configParams[prop.Name] = prop.GetValue(typeof(Config), null);
            }
        }
    }
}
