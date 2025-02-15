using System.ComponentModel;

namespace PluginCore
{
    public abstract class AbstractPlugin : IPluginType
    {
        public IPluginStart Start { get; set; }

        public abstract void SetupMenu(IComponent pluginPage);
    }
}
