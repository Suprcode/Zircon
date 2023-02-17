using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using DevExpress.XtraTab;

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

        /// <summary>
        /// Namespace of your plugin
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Name of your plugin to uniquely differentiate it from others. Usually the same as your library name <see cref="Plugin.{Name}.dll"/>
        /// </summary>
        string Name { get; }
    }
}
