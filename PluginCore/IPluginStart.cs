using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PluginCore
{
    public interface IPluginStart
    {
        event EventHandler<LogEventArgs> Log;

        string Namespace { get; }
        string Prefix { get; }

        void SetupMenu(IComponent page);
    }
}
