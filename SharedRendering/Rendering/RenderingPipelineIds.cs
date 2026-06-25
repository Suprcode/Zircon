using System;

namespace Shared.Rendering
{
    public static class RenderingPipelineIds
    {
        public const string SilkDXD3D11 = "DirectX 11";
        public const string SilkOpenGL = "OpenGL";
        public const string SilkVulkan = "Vulkan";

        public static bool SupportsEnhancedEffects(string pipelineId)
        {
            return string.Equals(pipelineId, SilkDXD3D11, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(pipelineId, SilkOpenGL, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(pipelineId, SilkVulkan, StringComparison.OrdinalIgnoreCase);
        }
    }
}
