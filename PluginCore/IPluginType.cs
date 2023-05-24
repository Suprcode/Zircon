﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCore
{
    public interface IPluginType
    {
        IPluginStart Start { get; set; }

        //TODO Determine if plugin is for the server/client
        //bool ServerPlugin { get; }

        /// <summary>
        /// Initialise any menu items to appear under the plugin tab
        /// </summary>
        /// <param name="page"></param>
        void SetupMenu(IComponent page);
    }
}
