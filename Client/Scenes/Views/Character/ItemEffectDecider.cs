using Client.Envir;
using Library;
using System;

namespace Client.Scenes.Views.Character
{
    internal class ItemEffectDecider
    {
        public static MirImage GetItemEffectImageOrNull(ItemType type, int shape, out int x, out int y)
        {
            x = 0; y = 0;

            LibraryFile libraryFile;

            switch (type)
            {
                case ItemType.DarkStone:
                    libraryFile = LibraryFile.GameInter;
                    break;
                default:
                    return null;
            }

            if (!CEnvir.LibraryList.TryGetValue(libraryFile, out MirLibrary effectLibrary)) return null;

            MirImage image = ItemImage(type, shape, effectLibrary, out x, out y);

            return image;
        }

        private static MirImage ItemImage(ItemType type, int shape, MirLibrary effectLibrary, out int x, out int y)
        {
            int animationIndex = GameScene.Game.MapControl.Animation;

            int startIndex = 0;
            int animationCount = 0;
            x = 0; y = 0;

            switch (type)
            {
                case ItemType.DarkStone:
                    switch (shape)
                    {
                        case 1: //Burning
                            startIndex = 2020;
                            animationCount = 10;
                            x = -5;
                            y = 20;
                            break;
                        case 2: //Frozen
                            startIndex = 2030;
                            animationCount = 10;
                            x = -5;
                            y = 20;
                            break;
                        case 3: //Shocking
                            startIndex = 2040;
                            animationCount = 10;
                            x = -5;
                            y = 20;
                            break;
                        case 4: //Gusting
                            startIndex = 2050;
                            animationCount = 10;
                            x = -5;
                            y = 20;
                            break;
                    }
                    break;
            }

            if (startIndex == 0)
                return null;

            var animationOffset = (animationIndex % animationCount);

            return effectLibrary.CreateImage(startIndex + animationOffset, ImageType.Image);
        }
    }
}
