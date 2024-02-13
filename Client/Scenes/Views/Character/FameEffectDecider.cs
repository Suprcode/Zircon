using Client.Envir;
using Library;
using System;

namespace Client.Scenes.Views.Character
{
    internal class FameEffectDecider
    {
        public static MirImage GetFameEffectImageOrNull(int shape, out int x, out int y)
        {
            x = 0; y = 0;

            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary effectLibrary)) return null;

            MirImage image = FameImage(shape, effectLibrary, out x, out y);

            return image;
        }

        private static MirImage FameImage(int shape, MirLibrary effectLibrary, out int x, out int y)
        {
            int animationIndex = GameScene.Game.MapControl.Animation;

            int startIndex = 0;
            int animationCount = 0;
            x = 0; y = 0;

            switch (shape)
            {
                case 1:
                    startIndex = 1870;
                    animationCount = 10;
                    break;
                case 2:
                    startIndex = 1890;
                    animationCount = 10;
                    break;
                case 3:
                    startIndex = 1910;
                    animationCount = 11;
                    break;
                case 4:
                    startIndex = 1930;
                    animationCount = 10;
                    break;
                case 5:
                    startIndex = 1950;
                    animationCount = 10;
                    break;
                case 6:
                    startIndex = 1970;
                    animationCount = 10;
                    x = -11; y = -10;
                    break;
                case 7:
                    startIndex = 1990;
                    animationCount = 12;
                    x = -17; y = -15;
                    break;
                case 8:
                    startIndex = 2270;
                    animationCount = 18;
                    x = -7; y = -5;
                    break;
                case 9:
                    startIndex = 2250;
                    animationCount = 18;
                    x = -7; y = -5;
                    break;
                default:
                    break;
            }

            if (startIndex == 0)
                return null;

            var animationOffset = (animationIndex % animationCount);

            return effectLibrary.CreateImage(startIndex + animationOffset, ImageType.Image);
        }
    }
}
