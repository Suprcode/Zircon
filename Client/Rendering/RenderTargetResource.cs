using System;

namespace Client.Rendering
{
    public readonly struct RenderTargetResource
    {
        private RenderTargetResource(RenderTexture texture, RenderSurface surface)
        {
            Texture = texture;
            Surface = surface;
        }

        public RenderTexture Texture { get; }

        public RenderSurface Surface { get; }

        public bool IsValid => Texture.IsValid && Surface.IsValid;

        public static RenderTargetResource From(RenderTexture texture, RenderSurface surface)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (!surface.IsValid)
                throw new ArgumentException("A valid surface handle is required.", nameof(surface));

            return new RenderTargetResource(texture, surface);
        }
    }
}
