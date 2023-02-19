using Client.Envir;
using Newtonsoft.Json.Linq;
using System;

namespace Client.Models.Character.Shadow
{
    public class DXShadowBoundary
    {
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }

        public DXShadowBoundary()
        {
            Left = int.MaxValue;
            Top = int.MaxValue;
            Right = int.MinValue;
            Bottom = int.MinValue;
        }

        public void Transform(MirImage image, int drawX, int drawY)
        {
            Left = Math.Min(Left, drawX + image.OffSetX);
            Top = Math.Min(Top, drawY + image.OffSetY);
            Right = Math.Max(Right, image.Width + drawX + image.OffSetX);
            Bottom = Math.Max(Bottom, image.Height + drawY + image.OffSetY);
            
        }
    }
}
