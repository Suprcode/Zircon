using System.Windows.Forms;

namespace PluginCore
{
    public interface IPluginForm : IPluginType
    {
        /// <summary>
        /// Indicates plugin can be loaded as a standalone applicated from the server
        /// </summary>
        bool SupportsStandaloneLoading { get; }

        /// <summary>
        /// Form that gets called when created within the Standalone Plugin Loader
        /// </summary>
        /// <returns></returns>
        Form CreateStandaloneForm();
    }
}