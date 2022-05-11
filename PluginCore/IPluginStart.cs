using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace PluginCore
{
    public interface IPluginStart
    {
        event EventHandler<LogEventArgs> Log;

        bool SupportsStandaloneLoading { get; }

        string Namespace { get; }
        string Prefix { get; }

        void SetupMenu(IComponent page);

        Form CreateStandaloneForm();
    }
}
