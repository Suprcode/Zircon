// Headless scene/library substitutes for exercising production effect timing and ownership.
using System.Drawing;

namespace Library
{
    public enum LibraryFile { ProgUse }
    public enum MirDirection { Up }
    public enum ImageType { Image }
    public sealed class MirImage { }
    public sealed class MirLibrary
    {
        public MirImage[] Images = new MirImage[130];
        public int SizeReads;
        public Size GetSize(int index) { SizeReads++; return new Size(16, 16); }
        public Point GetOffSet(int index) => Point.Empty;
        public void DrawBlend(int frame, int x, int y, Color colour, bool offset, float rate, ImageType type) { }
        public void Draw(int frame, int x, int y, Color colour, bool offset, float opacity, ImageType type) { }
    }
}

namespace Client.Envir
{
    public static class CEnvir
    {
        public static DateTime Now = new DateTime(2026, 9, 12);
        public static readonly Dictionary<Library.LibraryFile, Library.MirLibrary> LibraryList = new();
    }
}

namespace Client.Scenes.Views
{
    public sealed class MapControl
    {
        public bool TextureValid;
        public static int PixelOffsetX, PixelOffsetY;
        public readonly List<Client.Models.MirEffect> Effects = new();
    }
}

namespace Client.Scenes
{
    public sealed class GameScene
    {
        public static readonly GameScene Game = new();
        public readonly Views.MapControl MapControl = new();
    }
}

namespace Client.Models.Particles
{
    public sealed class ParticleEmitter { }
}

namespace Client.Models
{
    public class MapObject
    {
        public static readonly MapObject User = new();
        public const int CellWidth = 48, CellHeight = 32;
        public static int OffSetX, OffSetY;
        public int DrawX, DrawY;
        public Point CurrentLocation, MovingOffSet;
        public readonly List<MirEffect> Effects = new();
    }

    public sealed class ItemObject : MapObject
    {
        public bool IsPileRepresentative = true;
        public static bool IntersectsViewport(Rectangle bounds) => bounds.IntersectsWith(new Rectangle(0, 0, 800, 600));
    }
}
