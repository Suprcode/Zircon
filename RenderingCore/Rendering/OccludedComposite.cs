using System;
using System.Drawing;

namespace Shared.Rendering
{
    public static partial class RenderingPipelineManager
    {
        /// <summary>
        /// Draws an occluded actor with ordinary and screen-blended layers. Both callbacks
        /// must draw the same immutable frame; the occluder callback draws alpha under MASK.
        /// </summary>
        public static void DrawOccludedComposite(Size size, Action drawActor, Action drawOccluders, float opacity)
        {
            if (size.Width <= 0 || size.Height <= 0 || opacity <= 0F) return;
            RenderSurface previousSurface = GetCurrentSurface();
            float previousOpacity = GetOpacity();
            bool previousBlend = IsBlending();
            float previousRate = GetBlendRate();
            BlendMode previousMode = GetBlendMode();
            SpriteShaderEffectRequest? previousEffect = _spriteShaderEffect;
            RenderTargetResource emission = default, white = default, absorption = default;
            Rectangle bounds = new Rectangle(Point.Empty, size);
            try
            {
                emission = RentUICacheTarget(size);
                white = RentUICacheTarget(size);
                absorption = RentUICacheTarget(size);
                DisableSpriteShaderEffect();
                SetOpacity(1F);
                SetBlend(false);

                // Each alpha/screen layer is affine: result = E + T * background.
                // Black captures E; white captures E + T, including rear-layer occlusion.
                SetSurface(emission.Surface);
                Clear(RenderClearFlags.Target, Color.FromArgb(0), 0, 0);
                drawActor();
                SetSurface(white.Surface);
                // Each replay starts from the same state, independent of callback internals.
                DisableSpriteShaderEffect();
                SetOpacity(1F);
                SetBlend(false);
                Clear(RenderClearFlags.Target, Color.White, 0, 0);
                drawActor();
                DisableSpriteShaderEffect();
                SetOpacity(1F);

                // Derive absorption (1 - T) as (1 - white) + emission. Alpha cannot encode
                // this: a red glow transmits green/blue background even with opaque source alpha.
                SetSurface(absorption.Surface);
                Clear(RenderClearFlags.Target, Color.White, 0, 0);
                SetBlend(true, 1F, BlendMode.SUBTRACT_COLOUR);
                DrawPremultipliedTexture(white.Texture, bounds, bounds, Color.White);
                SetBlend(true, 1F, BlendMode.ADD_COLOUR);
                DrawPremultipliedTexture(emission.Texture, bounds, bounds, Color.White);

                // Reuse the white capture for inverse foreground coverage, preserving tile holes.
                SetSurface(white.Surface);
                Clear(RenderClearFlags.Target, Color.White, 0, 0);
                SetBlend(true, 1F, BlendMode.MASK);
                drawOccluders();
                DisableSpriteShaderEffect();
                SetBlend(true, 1F, BlendMode.MASK);
                SetOpacity(1F);
                SetSurface(emission.Surface);
                DrawTexture(white.Texture, bounds, bounds, Color.White);
                SetSurface(absorption.Surface);
                DrawTexture(white.Texture, bounds, bounds, Color.White);

                // Blend the complete actor without converting glow to alpha:
                // world * (1 - opacity * coveredAbsorption) + opacity * coveredEmission.
                SetSurface(previousSurface);
                SetOpacity(Math.Min(1F, opacity));
                SetBlend(true, 1F, BlendMode.ERASE_COLOUR);
                DrawPremultipliedTexture(absorption.Texture, bounds, bounds, Color.White);
                SetBlend(true, 1F, BlendMode.ADD_COLOUR);
                DrawPremultipliedTexture(emission.Texture, bounds, bounds, Color.White);
            }
            finally
            {
                // Finish reads before pooling, and restore state if either callback fails.
                SetSurface(previousSurface);
                FlushSprite();
                SetOpacity(previousOpacity);
                SetBlend(previousBlend, previousRate, previousMode);
                _spriteShaderEffect = previousEffect;
                ReturnUICacheTarget(absorption);
                ReturnUICacheTarget(white);
                ReturnUICacheTarget(emission);
            }
        }
    }
}
