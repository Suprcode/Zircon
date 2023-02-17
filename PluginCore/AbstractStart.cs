using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public event EventHandler<LogEventArgs> Log;

        public virtual void LogMessage(string message)
        {
            Log?.Invoke(this, new LogEventArgs { Message = message });
        }
    }
}
