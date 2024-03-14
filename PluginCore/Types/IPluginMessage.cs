namespace PluginCore
{
    public interface IPluginMessage : IPluginType
    {
        /// <summary>
        /// Receive messages that need processing
        /// </summary>
        /// <param name="value"></param>
        void ReceiveMessage(object value);
    }
}
