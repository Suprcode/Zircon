namespace Client.Rendering
{
    public sealed class GraphicsAdapterInfo
    {
        public GraphicsAdapterInfo(string id, string name, string type, ulong dedicatedMemoryMegabytes)
        {
            Id = id;
            Name = name;
            Type = type;
            DedicatedMemoryMegabytes = dedicatedMemoryMegabytes;
        }

        public string Id { get; }
        public string Name { get; }
        public string Type { get; }
        public ulong DedicatedMemoryMegabytes { get; }

        public string DisplayName => DedicatedMemoryMegabytes > 0
            ? $"{Name} ({Type}, {DedicatedMemoryMegabytes} MB)"
            : $"{Name} ({Type})";
    }
}
