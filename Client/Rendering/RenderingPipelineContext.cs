using System;
using System.Windows.Forms;

namespace Client.Rendering
{
    public sealed class RenderingPipelineContext
    {
        public RenderingPipelineContext(Form renderTarget)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
        }

        public Form RenderTarget { get; }
    }
}
