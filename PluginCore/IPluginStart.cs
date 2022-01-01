using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PluginCore
{
    public interface IPluginStart
    {
        event EventHandler<LogEventArgs> Log;

        string Prefix { get; }

        void SetupMenu(IComponent page);
        void SetupConfig(Dictionary<string, object> configParams);
    }
}
