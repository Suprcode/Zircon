using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginCore
{
    public class PluginLoader
    {
        public static PluginLoader Loader;

        public event EventHandler<LogEventArgs> Log;

        public List<IPluginStart> Plugins = new List<IPluginStart>();

        /// <summary>
        /// Main entry
        /// </summary>
        private PluginLoader() { }

        public static void Init()
        {
            if (Loader == null)
            {
                Loader = new PluginLoader();
            }
        }

        public static IPluginStart LoadStandalone(string file)
        {
            try
            {
                Assembly dll = Assembly.LoadFrom(file);
                var filename = Path.GetFileNameWithoutExtension(file);

                Type classType = dll.GetType(String.Format("{0}.Start", filename));
                if (classType != null && typeof(IPluginStart).IsAssignableFrom(classType))
                {
                    var pluginStart = (IPluginStart)Activator.CreateInstance(classType);

                    pluginStart.Log += (o, e) =>
                    {
                        Loader.Log?.Invoke(o, e);
                    };

                    Loader.Plugins.Add(pluginStart);

                    return pluginStart;
                }
            }
            catch (Exception ex)
            {
                Loader.LogMessage(ex.ToString());
            }

            return null;
        }

        /// <summary>
        /// Load all plugins in root folder
        /// </summary>
        /// <param name="ribbonPage">Reference to the plugin ribbon tab</param>
        public static void LoadIntegrated(IComponent ribbonPage)
        {
            if (Loader == null) return;

            var files = Directory.GetFiles("./", "Plugin.*.dll", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    Assembly dll = Assembly.LoadFrom(file);
                    var filename = Path.GetFileNameWithoutExtension(file);

                    Type classType = dll.GetType(String.Format("{0}.Start", filename));
                    if (classType != null && typeof(IPluginStart).IsAssignableFrom(classType))
                    {
                        var pluginStart = (IPluginStart)Activator.CreateInstance(classType);

                        pluginStart.Log += (o, e) =>
                        {
                            Loader.Log?.Invoke(o, e);
                        };

                        pluginStart.SetupMenu(ribbonPage);

                        Loader.Plugins.Add(pluginStart);
                    }
                }
                catch (Exception ex)
                {
                    Loader.LogMessage(ex.ToString());
                }
            }
        }

        public IPluginStart FindPlugin(string pluginNamespace)
        {
            return Plugins.FirstOrDefault(x => x.Namespace == pluginNamespace);
        }

        private void LogMessage(string message)
        {
            Log?.Invoke(this, new LogEventArgs { Message = message });
        }
    }
}
