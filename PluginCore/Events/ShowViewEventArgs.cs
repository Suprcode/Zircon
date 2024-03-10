using System;

namespace PluginCore
{
    public class ShowViewEventArgs : EventArgs
    {
        public Type View { get; set; }
    }
}
