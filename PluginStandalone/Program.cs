using PluginCore;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace PluginStandalone
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                string pluginFilename = ConfigurationManager.AppSettings["Plugin"];

                if (string.IsNullOrWhiteSpace(pluginFilename))
                {
                    throw new Exception("Plugin filename must be referenced in the application config under the 'Plugin' key.");
                }

                PluginLoader.Instance.Log += Loader_Log;

                var plugin = PluginLoader.LoadPlugin(pluginFilename);

                if (plugin == null)
                {
                    throw new Exception($"Failed to load {pluginFilename}.");
                }

                if (plugin.Type is not IPluginForm form || !form.SupportsStandaloneLoading)
                {
                    throw new Exception($"{pluginFilename} does not support being loaded as a standalone application.");
                }

                Console.WriteLine($"Loading {pluginFilename}...");
                Application.Run(form.CreateStandaloneForm());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        private static void Loader_Log(object sender, LogEventArgs e)
        {
            Console.WriteLine(e.Message);

            //TODO - Save to log file
        }
    }
}
