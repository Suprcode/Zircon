using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace PluginCore
{
    public class PluginLoader
    {
        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ShowViewEventArgs> View;
        public event EventHandler<ShowMapViewerEventArgs> MapViewer;

        public List<IPluginStart> Plugins { get; } = new List<IPluginStart>();

        private PluginLoader() { }

        private static PluginLoader instance;
        public static PluginLoader Instance
        {
            get
            {
                instance ??= new PluginLoader();
                return instance;
            }
        }

        public static IPluginStart LoadPlugin(string file)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(file);
                string filename = Path.GetFileNameWithoutExtension(file);

                Type pluginType = assembly.GetType($"{filename}.Start");
                if (pluginType != null && typeof(IPluginStart).IsAssignableFrom(pluginType))
                {
                    var pluginInstance = (IPluginStart)Activator.CreateInstance(pluginType);

                    AttachEventHandlers(pluginInstance);
                    Instance.Plugins.Add(pluginInstance);

                    return pluginInstance;
                }
            }
            catch (Exception ex)
            {
                Instance.LogMessage(ex.ToString());
            }

            return null;
        }

        public static void LoadPlugins(IComponent pluginPage, MirDB.Session session)
        {
            if (Instance == null) return;

            var files = Directory.GetFiles("./", "Plugin.*.dll", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var pluginInstance = LoadPlugin(file);
                if (pluginInstance != null)
                {
                    pluginInstance.Session = session;
                    pluginInstance.Type.SetupMenu(pluginPage);
                }
            }
        }

        private static void AttachEventHandlers(IPluginStart pluginInstance)
        {
            pluginInstance.Log += (o, e) => Instance.Log?.Invoke(o, e);
            pluginInstance.View += (o, e) => Instance.View?.Invoke(o, e);
            pluginInstance.MapViewer += (o, e) => Instance.MapViewer?.Invoke(o, e);
        }

        private void LogMessage(string message)
        {
            Log?.Invoke(this, new LogEventArgs { Message = message });
        }
    }
}
