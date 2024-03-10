using System;

namespace PluginCore
{
    public interface IPluginStart
    {
        /// <summary>
        /// Type of plugin and how it interacts with the server
        /// </summary>
        IPluginType Type { get; set; }

        /// <summary>
        /// Event handler to send log messages to the current console 
        /// </summary>
        event EventHandler<LogEventArgs> Log;
        void LogMessage(string message);

        /// <summary>
        /// Event handler to show a view in the main window
        /// </summary>
        event EventHandler<ShowViewEventArgs> View;
        void ShowView(Type type);

        /// <summary>
        /// Event handler to open the map viewer with a specific map
        /// </summary>
        event EventHandler<ShowMapViewerEventArgs> MapViewer;
        void ShowMapViewer(string mapName);

        /// <summary>
        /// Namespace of your plugin
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Name of your plugin to uniquely differentiate it from others. Usually the same as your library name <see cref="Plugin.{Name}.dll"/>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Main Database session of the server. Only set on integrated plugins.
        /// </summary>
        MirDB.Session Session { get; set; }
    }
}
