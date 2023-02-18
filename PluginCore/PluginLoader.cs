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

                        pluginStart.Type.SetupMenu(ribbonPage);

                        Loader.Plugins.Add(pluginStart);
                    }
                }
                catch (Exception ex)
                {
                    Loader.LogMessage(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Send data to all plugins
        /// </summary>
        /// <param name="value"></param>
        public void SendMessage(object value)
        {
            var names = Plugins.Select(x => x.Name).ToArray();

            Send(names, value);
        }

        /// <summary>
        /// Send data to multiple plugins
        /// </summary>
        /// <param name="names"></param>
        /// <param name="value"></param>
        public void Send(string[] names, object value)
        {
            if (names == null || names.Length == 0) return;

            foreach (var name in names)
            {
                Send(name, value);
            }
        }

        /// <summary>
        /// Send data to specific plugin
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Send(string name, object value)
        {
            foreach (var plugin in Loader.Plugins)
            {
                try
                {
                    if (!(plugin.Type is IPluginMessage message)) continue;

                    if (string.IsNullOrEmpty(name)) continue;

                    if (string.Compare(name, plugin.Name, StringComparison.OrdinalIgnoreCase) != 0) continue;

                    message.ReceiveMessage(value);
                }
                catch (NotImplementedException)
                {

                }
                catch (NotSupportedException)
                {

                }
                catch (Exception ex)
                {
                    LogMessage(ex.Message);
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
