using System;

namespace Client.Rendering
{
    public static class RenderingPipelineIds
    {
        public const string SharpDXD3D9 = "DirectX 9";
        public const string SharpDXD3D11 = "DirectX 11";
        public const string SilkOpenGL = "OpenGL";
        public const string SilkVulkan = "Vulkan";

        public static bool SupportsEnhancedEffects(string pipelineId)
        {
            return string.Equals(pipelineId, SharpDXD3D11, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(pipelineId, SilkOpenGL, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(pipelineId, SilkVulkan, StringComparison.OrdinalIgnoreCase);
        }
    }
}
