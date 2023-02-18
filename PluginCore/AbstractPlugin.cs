using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCore
{
    public abstract class AbstractPlugin : IPluginType
    {
        public IPluginStart Start { get; set; }

        public abstract void SetupMenu(IComponent page);
    }
}
