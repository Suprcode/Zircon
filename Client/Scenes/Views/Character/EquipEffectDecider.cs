using Client.Envir;
using Library;

namespace Client.Scenes.Views.Character
{
    internal class EquipEffectDecider
    {
        public static MirImage GetEffectImageOrNull(ClientUserItem item, MirGender gender)
        {
            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out MirLibrary effectLibrary)) return null;

            MirImage image = ItemExteriorEffectImage(item, gender, effectLibrary);
            if (image != null) return image;
            return PresetItemEffectImage(item, effectLibrary);
        }

        private static MirImage ItemExteriorEffectImage(ClientUserItem item, MirGender gender, MirLibrary effectLibrary)
        {
            int animationIndex = GameScene.Game.MapControl.Animation;
            return item.Info.ExteriorEffect switch
            {
                ExteriorEffect.A_GreenFeatherWings => effectLibrary.CreateImage(2100, ImageType.Image),
                ExteriorEffect.A_RedFeatherWings => effectLibrary.CreateImage(2101, ImageType.Image),
                ExteriorEffect.A_BlueFeatherWings => effectLibrary.CreateImage(2102, ImageType.Image),
                ExteriorEffect.A_WhiteFeatherWings => effectLibrary.CreateImage(2103, ImageType.Image),
                ExteriorEffect.A_AngelicWings => effectLibrary.CreateImage(3000, ImageType.Image),

                ExteriorEffect.A_BlueAura => effectLibrary.CreateImage(MirGender.Male.Equals(gender) ? 602 : 622, ImageType.Image),
                ExteriorEffect.A_FlameAura => effectLibrary.CreateImage(MirGender.Male.Equals(gender) ? 601 : 621, ImageType.Image),
                ExteriorEffect.A_WhiteAura => effectLibrary.CreateImage(MirGender.Male.Equals(gender) ? 600 : 620, ImageType.Image),

                ExteriorEffect.A_SmallYellowWings => effectLibrary.CreateImage(MirGender.Male.Equals(gender) ? 1800 : 1820, ImageType.Image),

                ExteriorEffect.A_PurpleTentacles => effectLibrary.CreateImage(2200 + (animationIndex % 11), ImageType.Image),
                ExteriorEffect.A_LionWings => effectLibrary.CreateImage(2300 + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.A_BlueDragonWings => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 2400 : 2500) + (animationIndex % 14), ImageType.Image),
                ExteriorEffect.A_RedWings2 => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 2600 : 2700) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.A_FlameAura2 => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 1700 : 1720) + (animationIndex % 10), ImageType.Image),
                ExteriorEffect.A_GreenWings => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 400 : 420) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.A_FlameWings => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 300 : 320) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.A_BlueWings => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 200 : 220) + (animationIndex % 15), ImageType.Image),
                ExteriorEffect.A_RedSinWings => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 500 : 520) + (animationIndex % 13), ImageType.Image),
                ExteriorEffect.A_FireDragonWings => effectLibrary.CreateImage((MirGender.Male.Equals(gender) ? 100 : 120) + (animationIndex % 10), ImageType.Image),

                ExteriorEffect.S_WarThurible => effectLibrary.CreateImage(2800 + (animationIndex % 10), ImageType.Image),
                ExteriorEffect.S_PenanceThurible => effectLibrary.CreateImage(2810 + (animationIndex % 10), ImageType.Image),
                ExteriorEffect.S_CensorshipThurible => effectLibrary.CreateImage(2820 + (animationIndex % 10), ImageType.Image),
                ExteriorEffect.S_PetrichorThurible => effectLibrary.CreateImage(2830 + (animationIndex % 10), ImageType.Image),

                ExteriorEffect.W_ChaoticHeavenBlade => effectLibrary.CreateImage(2000 + (animationIndex % 10), ImageType.Image),
                ExteriorEffect.W_JanitorsScimitar => effectLibrary.CreateImage(1900 + (animationIndex % 12), ImageType.Image),
                ExteriorEffect.W_JanitorsDualBlade => effectLibrary.CreateImage(1920 + (animationIndex % 12), ImageType.Image),
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
                //DivineRobe
                //? => effectLibrary.CreateImage(1500, ImageType.Image),
                //? => effectLibrary.CreateImage(1520, ImageType.Image),
                //Other
                _ => null
            };
        }
    }
}
