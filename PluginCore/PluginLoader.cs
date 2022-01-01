using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace PluginCore
{
    public class PluginLoader
    {
        private readonly Dictionary<string, object> _configParams = new Dictionary<string, object>();

        public event EventHandler<LogEventArgs> Log;

        /// <summary>
        /// Main entry
        /// </summary>
        /// <param name="config">Reference to the servers config class</param>
        public PluginLoader(Type config)
        {
            InitialiseConfig(config);
        }

        /// <summary>
        /// Load all plugins in root folder
        /// </summary>
        /// <param name="ribbonPage">Reference to the plugin ribbon tab</param>
        public void Load(IComponent ribbonPage)
        {
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
                            Log?.Invoke(o, e);
                        };

                        pluginStart.SetupConfig(_configParams);

                        pluginStart.SetupMenu(ribbonPage);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(ex.ToString());
                }
            }
        }

        private void InitialiseConfig(Type t)
        {
            var props = t.GetProperties();

            foreach (var prop in props)
            {
                _configParams[prop.Name] = prop.GetValue(t, null);
            }
        }

        private void LogMessage(string message)
        {
            Log?.Invoke(this, new LogEventArgs { Message = message });
        }
    }
}
