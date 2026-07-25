namespace LibraryEditor
{
    public sealed class LibrarySaveOptions
    {
        public bool BuildAtlasMetadata { get; set; } = true;
        public bool BuildShadowAtlasMetadata { get; set; }
        public bool BuildOverlayAtlasMetadata { get; set; }
        public bool StorePngSourceImages { get; set; }
        public int AtlasGroupImageCount { get; set; }
        public int AtlasPageSize { get; set; } = 2048;
        public ZlRuntimeTexturePreference IndividualRuntimePreference { get; set; } = ZlRuntimeTexturePreference.Bgra32;
        public ZlRuntimeTexturePreference AtlasRuntimePreference { get; set; } = ZlRuntimeTexturePreference.Bc7;
        public ZlContainerCompression ContainerCompression { get; set; } = ZlContainerCompression.DeflateBest;
    }
}
