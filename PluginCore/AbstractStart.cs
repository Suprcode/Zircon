using Library;
using MirDB;
using System;

namespace PluginCore
{
    public abstract class AbstractStart<T> : IPluginStart where T : class, IPluginType, new()
    {
        public IPluginType Type { get; set; }

        public AbstractStart()
        {
            Type = new T()
            {
                Start = this
            };
        }

        public string Name
        {
            get
            {
                return Namespace.Replace("Plugin.", "");
            }
        }

        public string Namespace
        {
            get
            {
                return (typeof(T)).Namespace;
            }
        }

        public string AssemblyName
        {
            get
            {
                return (typeof(T)).Assembly.GetName().Name;
            }
        }

        public Session Session { get; set; }

        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<ShowViewEventArgs> View;
        public event EventHandler<ShowMapViewerEventArgs> MapViewer;

        public virtual void LogMessage(string message)
        {
            Log?.Invoke(this, new LogEventArgs { Message = message });
        }

        public virtual void ShowView(Type type)
        {
            View?.Invoke(this, new ShowViewEventArgs { View = type });
        }

        public virtual void ShowMapViewer(string mapPath)
        {
            MapViewer?.Invoke(this, new ShowMapViewerEventArgs { MapPath = mapPath });
        }

        public string GetPluginFolder()
        {
            return Globals.PluginPath(AssemblyName);
        }
    }
}