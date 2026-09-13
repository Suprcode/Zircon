using Client.Envir;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models
{
    // Loot has a fixed ten-frame loop, no combat callbacks, and a constant light.
    public sealed class LootEffect : MirEffect
    {
        private const int AnimationFrameCount = 10;
        private const int FrameMilliseconds = 100;
        private const int LightRadius = 60;
        private static readonly TimeSpan[] delays = CreateDelays();
        private static readonly Dictionary<Color, Color[]> colours = new Dictionary<Color, Color[]>();
        private MirImage[] boundsImages;
        private Rectangle animationBounds;
        private bool boundsReady;

        private static TimeSpan[] CreateDelays()
        {
            var result = new TimeSpan[AnimationFrameCount];
            Array.Fill(result, TimeSpan.FromMilliseconds(FrameMilliseconds));
            return result;
        }
        private static Color[] GetColours(Color colour)
        {
            if (!colours.TryGetValue(colour, out Color[] result))
            {
                result = new Color[AnimationFrameCount];
                Array.Fill(result, colour);
                colours.Add(colour, result);
            }
            return result;
        }
        public LootEffect(ItemObject target, int index, Color colour)
            : base(index, AnimationFrameCount, LibraryFile.ProgUse, LightRadius, LightRadius)
        {
            // Loot never mutates these arrays; ordinary MirEffect instances own their data.
            Delays = delays;
            LightColours = GetColours(colour);
            Target = target;
            Loop = true;
            Blend = true;
            BlendRate = 0.5F;
        }

        internal bool Representative => Target is ItemObject item && item.IsPileRepresentative;
        public override float FrameLight => Representative && CEnvir.Now >= StartTime ? LightRadius : 0;

        public override void Process()
        {
            if (Target == null) return;
            DrawX = Target.DrawX + AdditionalOffSet.X;
            DrawY = Target.DrawY + AdditionalOffSet.Y;
            if (!Config.DrawEffects || !SpriteVisible()) return;
            long elapsed = Math.Max(0, (CEnvir.Now - StartTime).Ticks);
            FrameIndex = (int)(elapsed / (TimeSpan.TicksPerMillisecond * FrameMilliseconds) % AnimationFrameCount);
            DrawFrame = StartIndex + FrameIndex;
        }

        private bool SpriteVisible()
        {
            if (!Representative || Library == null) return false;
            if (!boundsReady || boundsImages != Library.Images)
            {
                // Use the entire loop's bounds; retry if the library is still loading.
                animationBounds = Rectangle.Empty;
                boundsReady = true;
                for (int i = 0; i < FrameCount; i++)
                {
                    Point offset = UseOffSet ? Library.GetOffSet(StartIndex + i) : Point.Empty;
                    Size size = Library.GetSize(StartIndex + i);
                    if (size.IsEmpty)
                    {
                        boundsReady = false;
                        continue;
                    }
                    Rectangle frame = new Rectangle(offset, size);
                    animationBounds = animationBounds.IsEmpty ? frame : Rectangle.Union(animationBounds, frame);
                }
                boundsImages = Library.Images;
            }
            Rectangle bounds = animationBounds;
            bounds.Offset(DrawX, DrawY);
            return ItemObject.IntersectsViewport(bounds);
        }

        public override void Draw()
        {
            if (SpriteVisible()) base.Draw();
        }
    }
}
