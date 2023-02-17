using Client.Envir;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Scenes.Views.Character
{
    internal class ArmourEffectDecider
    {
        public static MirImage GetArmourEffectImageOrNull(ClientUserItem Item, MirGender Gender)
        {
            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out MirLibrary effectLibrary)) return null;

            MirImage image = ItemExteriorEffectImage(Item, Gender, effectLibrary);
            if (image != null) return image;
            return PresetItemEffectImage(Item, effectLibrary);
        }

        private static MirImage ItemExteriorEffectImage(ClientUserItem Item, MirGender Gender, MirLibrary effectLibrary)
        {
            int animationIndex = GameScene.Game.MapControl.Animation;
            return Item.Info.ExteriorEffect switch
            {
                ExteriorEffect.GreenFeatherWings => effectLibrary.CreateImage(2100, ImageType.Image),
                ExteriorEffect.RedFeatherWings => effectLibrary.CreateImage(2101, ImageType.Image),
                ExteriorEffect.BlueFeatherWings => effectLibrary.CreateImage(2102, ImageType.Image),
                ExteriorEffect.WhiteFeatherWings => effectLibrary.CreateImage(2103, ImageType.Image),
                ExteriorEffect.AngelicWings => effectLibrary.CreateImage(3000, ImageType.Image),

                ExteriorEffect.BlueAura => effectLibrary.CreateImage(MirGender.Male.Equals(Gender) ? 602 : 622, ImageType.Image),
                ExteriorEffect.FlameAura => effectLibrary.CreateImage(MirGender.Male.Equals(Gender) ? 601 : 621, ImageType.Image),
                ExteriorEffect.WhiteAura => effectLibrary.CreateImage(MirGender.Male.Equals(Gender) ? 600 : 620, ImageType.Image),
                ExteriorEffect.SmallYellowWings => effectLibrary.CreateImage(MirGender.Male.Equals(Gender) ? 1800 : 1820, ImageType.Image),

                ExteriorEffect.PurpleTentacles => effectLibrary.CreateImage(2200 + (animationIndex % 11), ImageType.Image),
                ExteriorEffect.LionWings => effectLibrary.CreateImage(2300 + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.BlueDragonWings => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 2400 : 2500) + (animationIndex % 14), ImageType.Image),
                ExteriorEffect.RedWings2 => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 2600 : 2700) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.FlameAura2 => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 1700 : 1720) + (animationIndex % 10), ImageType.Image),
                ExteriorEffect.GreenWings => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 400 : 420) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.FlameWings => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 300 : 320) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.BlueWings => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 200 : 220) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.RedSinWings => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 500 : 520) + (animationIndex % 13), ImageType.Image),
                ExteriorEffect.FireDragonWings => effectLibrary.CreateImage((MirGender.Male.Equals(Gender) ? 100 : 120) + (animationIndex % 10), ImageType.Image),
                _ => null
            };
        }

        private static MirImage PresetItemEffectImage(ClientUserItem item, MirLibrary effectLibrary)
        {
            
            return item.Info.Image switch
            {
                //LightArmour
                942 => effectLibrary.CreateImage(700, ImageType.Image),
                952 => effectLibrary.CreateImage(720, ImageType.Image),
                //WyvernArmour
                961 => effectLibrary.CreateImage(1600, ImageType.Image),
                971 => effectLibrary.CreateImage(1620, ImageType.Image),
                //38WarArmour
                982 => effectLibrary.CreateImage(800, ImageType.Image),
                992 => effectLibrary.CreateImage(820, ImageType.Image),
                //44WarRobe
                983 => effectLibrary.CreateImage(1200, ImageType.Image),
                993 => effectLibrary.CreateImage(1220, ImageType.Image),
                //GhostArmour
                984 => effectLibrary.CreateImage(1100, ImageType.Image),
                994 => effectLibrary.CreateImage(1120, ImageType.Image),
                //38WizRobe
                1022 => effectLibrary.CreateImage(900, ImageType.Image),
                1032 => effectLibrary.CreateImage(920, ImageType.Image),
                //44WizRobe
                1023 => effectLibrary.CreateImage(1300, ImageType.Image),
                1033 => effectLibrary.CreateImage(1320, ImageType.Image),
                //38TaoRobe
                1002 => effectLibrary.CreateImage(1000, ImageType.Image),
                1012 => effectLibrary.CreateImage(1020, ImageType.Image),
                //44TaoRobe
                1003 => effectLibrary.CreateImage(1400, ImageType.Image),
                1013 => effectLibrary.CreateImage(1420, ImageType.Image),
                //Other
                _ => null
            };
        }
    }
}
