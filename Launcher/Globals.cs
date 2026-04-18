namespace Library
{
    public static class Globals
    {
        public static string PluginPath(string assemblyName)
        {
            return "Plugins" + "\\" + assemblyName + "\\";
        }
    }
}
