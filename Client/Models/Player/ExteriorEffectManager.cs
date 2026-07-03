using Client.Envir;
using Client.Scenes;
using Library;
using System.Drawing;

namespace Client.Models.Player
{
    public class ExteriorEffectManager
    {
        public static void DrawExteriorEffects(PlayerObject player, bool behind)
        {
            if (!Config.DrawEffects)
            {
                return;
            }

            switch (player.CurrentAction)
            {
                case MirAction.Standing:
                case MirAction.Moving:
                case MirAction.Pushed:
                case MirAction.Attack:
                case MirAction.RangeAttack:
                case MirAction.Spell:
                case MirAction.Harvest:
                case MirAction.Struck:
                case MirAction.Die:
                case MirAction.Dead:
                    break;
                default:
                    return;
            }

            if (player.CostumeShape < 0)
            {
                DrawExteriorEffect(player, player.ArmourEffect, behind);
            }

            DrawExteriorEffect(player, player.EmblemEffect, behind);

            if (!PlayerObject.CostumeShapeHideWeapon.Contains(player.CostumeShape))
            {
                DrawExteriorEffect(player, player.WeaponEffect, behind);
                DrawExteriorEffect(player, player.ShieldEffect, behind);
            }
        }

        private static bool DrawExteriorEffectBehind(MirDirection direction, ExteriorEffect effect)
        {
            // always draw behind
            switch (effect)
            {
                case ExteriorEffect.E_BlueEyeRing:
                case ExteriorEffect.E_RedEyeRing:
                case ExteriorEffect.E_GreenSpiralRing:
                    return true;
                default:
                    break;
            }

            // never draw behind
            switch (effect)
            {
                case ExteriorEffect.A_BlueAura:
                case ExteriorEffect.A_FlameAura:
                case ExteriorEffect.A_WhiteAura:
                    return false;
                default:
                    break;
            }

            // right hand
            switch (effect)
            {
                case ExteriorEffect.W_ChaoticHeavenBlade:
                case ExteriorEffect.W_JanitorsScimitar:
                case ExteriorEffect.W_JanitorsDualBlade:
                    {
                        switch (direction)
                        {
                            case MirDirection.Up:
                            case MirDirection.UpLeft:
                            case MirDirection.Left:
                            case MirDirection.DownLeft:
                                return true;
                        }
                        return false;
                    }
            }

            // left hand
            switch (effect)
            {
                case ExteriorEffect.S_WarThurible:
                case ExteriorEffect.S_PenanceThurible:
                case ExteriorEffect.S_CensorshipThurible:
                case ExteriorEffect.S_PetrichorThurible:
                    {
                        switch (direction)
                        {
                            case MirDirection.UpRight:
                            case MirDirection.Right:
                            case MirDirection.DownRight:
                                return true;
                        }
                        return false;
                    }
            }

            switch (direction)
            {
                case MirDirection.Up:
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    return false;
                case MirDirection.DownRight:
                case MirDirection.Down:
                case MirDirection.DownLeft:
                    return true;
                default:
                    break;
            }

            return false;
        }

        private static bool DrawExteriorEffectInFront(MirDirection direction, ExteriorEffect effect)
        {
            // always draw in front
            switch (effect)
            {
                case ExteriorEffect.A_BlueAura:
                case ExteriorEffect.A_FlameAura:
                case ExteriorEffect.A_WhiteAura:
                    return true;
                default:
                    break;
            }
            ;

            // never draw in front
            switch (effect)
            {
                case ExteriorEffect.E_BlueEyeRing:
                case ExteriorEffect.E_RedEyeRing:
                case ExteriorEffect.E_GreenSpiralRing:
                    return false;
                default:
                    break;
            }

            // right hand
            switch (effect)
            {
                case ExteriorEffect.W_ChaoticHeavenBlade:
                case ExteriorEffect.W_JanitorsScimitar:
                case ExteriorEffect.W_JanitorsDualBlade:
                    {
                        switch (direction)
                        {
                            case MirDirection.UpRight:
                            case MirDirection.Right:
                            case MirDirection.DownRight:
                            case MirDirection.Down:
                                return true;
                        }
                        return false;
                    }
            }

            // left hand
            switch (effect)
            {
                case ExteriorEffect.S_WarThurible:
                case ExteriorEffect.S_PenanceThurible:
                case ExteriorEffect.S_CensorshipThurible:
                case ExteriorEffect.S_PetrichorThurible:
                    {
                        switch (direction)
                        {
                            case MirDirection.Up:
                            case MirDirection.Down:
                            case MirDirection.DownLeft:
                            case MirDirection.Left:
                            case MirDirection.UpLeft:
                                return true;
                        }
                        return false;
                    }
            }

            switch (direction)
            {
                case MirDirection.Up:
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    return true;
                case MirDirection.DownRight:
                case MirDirection.Down:
                case MirDirection.DownLeft:
                    return false;
                default:
                    break;
            }

            return false;
        }

        private static void DrawExteriorEffect(PlayerObject player, ExteriorEffect effect, bool behind)
        {
            if (behind)
            {
                if (!DrawExteriorEffectBehind(player.Direction, effect))
                    return;
            }
            else
            {
                if (!DrawExteriorEffectInFront(player.Direction, effect))
                    return;
            }

            MirDirection direction = player.Direction;
            int drawX = DetermineDrawX(player, effect, direction),
                drawY = DetermineDrawY(player, effect, direction);

            if (effect >= ExteriorEffect.E_RedEyeRing && player.Horse == HorseType.None) //MonMagicEx26 - Emblem
            {
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.MonMagicEx26, out MirLibrary library)) return;
                switch (effect)
                {
                    case ExteriorEffect.E_RedEyeRing:
                        DrawBlend(player, library, 90 + GameScene.Game.MapControl.Animation / 2 % 24, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        DrawBlend(player, library, 140 + GameScene.Game.MapControl.Animation / 2 % 28, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.E_BlueEyeRing:
                        DrawBlend(player, library, 220 + GameScene.Game.MapControl.Animation / 2 % 25, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        DrawBlend(player, library, 180 + GameScene.Game.MapControl.Animation / 2 % 28, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.E_GreenSpiralRing:
                        DrawBlend(player, library, 330 + GameScene.Game.MapControl.Animation / 2 % 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        DrawBlend(player, library, 270 + GameScene.Game.MapControl.Animation / 2 % 28, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.E_Fireworks:
                        DrawBlend(player, library, 360 + GameScene.Game.MapControl.Animation / 2 % 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                }
            }
            else if (effect >= ExteriorEffect.A_RedWings2)
            {
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_FullEx3, out MirLibrary library)) return;
                switch (effect)
                {
                    case ExteriorEffect.A_RedWings2:
                        DrawBlend(player, library, DetermineIndex(0, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                }
            }
            else if (effect >= ExteriorEffect.A_BlueDragonWings)
            {
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_FullEx2, out MirLibrary library)) return;
                switch (effect)
                {
                    case ExteriorEffect.A_BlueDragonWings:
                        DrawBlend(player, library, DetermineIndex(0, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                }
            }
            else if (effect >= ExteriorEffect.A_LionWings)
            {
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_FullEx1, out MirLibrary library)) return;
                switch (effect)
                {
                    case ExteriorEffect.A_LionWings:
                        DrawBlend(player, library, DetermineIndex(0, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_AngelicWings:
                        DrawBlend(player, library, DetermineIndex(10000, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                }
            }
            else if (effect >= ExteriorEffect.A_FireDragonWings) //EquipEffect_Full
            {
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out MirLibrary library)) return;
                switch (effect)
                {
                    case ExteriorEffect.A_FireDragonWings:
                        DrawBlend(player, library, DetermineIndex(0, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_SmallYellowWings:
                        DrawBlend(player, library, DetermineIndex(10000, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_GreenFeatherWings:
                        DrawBlend(player, library, DetermineIndex(50000, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_RedFeatherWings:
                        DrawBlend(player, library, DetermineIndex(60000, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_BlueFeatherWings:
                        DrawBlend(player, library, DetermineIndex(70000, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_WhiteFeatherWings:
                        DrawBlend(player, library, DetermineIndex(80000, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_PurpleTentacles:
                        DrawBlend(player, library, DetermineIndex(90000, player), drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;

                    case ExteriorEffect.W_ChaoticHeavenBlade:
                        DrawBlend(player, library, 40000 + 5000 * (byte)player.Gender + player.DrawFrame, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.W_JanitorsScimitar:
                        DrawBlend(player, library, 20000 + 5000 * (byte)player.Gender + player.DrawFrame, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.W_JanitorsDualBlade: //TODO - cannot find correct images
                        DrawBlend(player, library, 20000 + 5000 * (byte)player.Gender + player.DrawFrame, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                }
            }
            else //EquipEffect_Part
            {
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out MirLibrary library)) return;

                switch (effect)
                {
                    case ExteriorEffect.A_WhiteAura:
                        DrawBlend(player, library, 800 + GameScene.Game.MapControl.Animation / 2 % 13, drawX, drawY, Color.White, useOffSet: true, 0.7f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_FlameAura:
                        DrawBlend(player, library, 820 + GameScene.Game.MapControl.Animation / 2 % 13, drawX, drawY, Color.White, useOffSet: true, 0.7f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_BlueAura:
                        DrawBlend(player, library, 840 + GameScene.Game.MapControl.Animation / 2 % 13, drawX, drawY, Color.White, useOffSet: true, 0.7f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_GreenWings:
                        DrawBlend(player, library, 400 + GameScene.Game.MapControl.Animation / 2 % 15 + (int)direction * 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_FlameWings:
                        DrawBlend(player, library, 200 + GameScene.Game.MapControl.Animation / 2 % 15 + (int)direction * 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_BlueWings:
                        DrawBlend(player, library, GameScene.Game.MapControl.Animation / 2 % 15 + (int)direction * 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_RedSinWings:
                        DrawBlend(player, library, 600 + GameScene.Game.MapControl.Animation / 2 % 13 + (int)direction * 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    //case 9177 or 9178: //??? i'm missing something from EquipEffect-Part.Zl?
                    //    DrawBlend(player, library, 4874 + GameScene.Game.MapControl.Animation / 2 % 20, DrawX, DrawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                    //    DrawBlend(player, library, 4898 + GameScene.Game.MapControl.Animation / 2 % 20, DrawX, DrawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                    //    break;
                    case ExteriorEffect.A_PurpleTentacles2:
                        DrawBlend(player, library, 4454 + GameScene.Game.MapControl.Animation / 2 % 4 + (int)direction * 9, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_DiamondFireWings:
                        DrawBlend(player, library, 4566 + GameScene.Game.MapControl.Animation / 2 % 4 + (int)direction * 9, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_PhoenixWings:
                        DrawBlend(player, library, 4062 + GameScene.Game.MapControl.Animation / 2 % 8 + (int)direction * 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_IceKingWings:
                        DrawBlend(player, library, 4258 + GameScene.Game.MapControl.Animation / 2 % 8 + (int)direction * 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.A_BlueButterflyWings:
                        DrawBlend(player, library, 4678 + GameScene.Game.MapControl.Animation / 2 % 8 + (int)direction * 20, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;

                    case ExteriorEffect.S_WarThurible:
                        Draw(player, library, 900 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image);
                        DrawBlend(player, library, 1000 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.S_PenanceThurible:
                        Draw(player, library, 1100 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image);
                        DrawBlend(player, library, 1200 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.S_CensorshipThurible:
                        Draw(player, library, 1300 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image);
                        DrawBlend(player, library, 1400 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                    case ExteriorEffect.S_PetrichorThurible:
                        Draw(player, library, 1500 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image);
                        DrawBlend(player, library, 1600 + GameScene.Game.MapControl.Animation % 4 + (int)direction * 10, drawX, drawY, Color.White, useOffSet: true, 1f, ImageType.Image, 0);
                        break;
                }
            }
        }

        private static void DrawBlend(PlayerObject player, MirLibrary library, int index, float drawX, float drawY, Color colour,
            bool useOffSet, float opacity, ImageType imageType, byte shadow)
        {
            MirImage image = library.GetImage(index);
            if (image == null) return;

            PointF location = player.GetScaledLibraryDrawLocation(image, imageType, drawX, drawY);
            library.DrawBlendScaled(index, player.Scale, player.Scale, colour, location.X, location.Y, 0F, opacity,
                imageType, useOffSet, shadow);
        }

        private static void Draw(PlayerObject player, MirLibrary library, int index, float drawX, float drawY, Color colour,
            bool useOffSet, float opacity, ImageType imageType)
        {
            MirImage image = library.GetImage(index);
            if (image == null) return;

            PointF location = player.GetScaledLibraryDrawLocation(image, imageType, drawX, drawY);
            library.Draw(index, location.X, location.Y, colour, useOffSet, opacity, imageType, player.Scale);
        }

        private static int DetermineIndex(int indexStart, PlayerObject player)
        {
            int animationSpeed = 2;
            MirDirection direction = player.Direction;
            MirAnimation animation = player.CurrentAnimation;

            switch (animation)
            {
                case MirAnimation.Walking:
                    return indexStart + 80 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.Running:
                    return indexStart + 160 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.Pushed:
                    return indexStart + 240 + GameScene.Game.MapControl.Animation / animationSpeed % 1 + (int)direction * 10;
                //case MirAnimation.Pushed2:
                //    return indexStart + 300 + GameScene.Game.MapControl.Animation / speedMultiplier % 1 + (int)direction * 10;
                case MirAnimation.Stance:
                    return indexStart + 400 + GameScene.Game.MapControl.Animation / animationSpeed % 3 + (int)direction * 10;
                case MirAnimation.Harvest:
                    return indexStart + 480 + GameScene.Game.MapControl.Animation / animationSpeed % 2 + (int)direction * 10;
                case MirAnimation.Combat1:
                    return indexStart + 560 + GameScene.Game.MapControl.Animation / animationSpeed % 5 + (int)direction * 10;
                case MirAnimation.Combat2:
                    return indexStart + 640 + GameScene.Game.MapControl.Animation / animationSpeed % 5 + (int)direction * 10;
                case MirAnimation.Combat3:
                    return indexStart + 722 + GameScene.Game.MapControl.Animation / animationSpeed % 4 + (int)direction * 10;
                case MirAnimation.Combat4:
                    return indexStart + 800 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.Combat5:
                    return indexStart + 880 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Combat6:
                    return indexStart + 960 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Combat7:
                    return indexStart + 1040 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Combat8:
                    return indexStart + 1120 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.Combat9:
                    return indexStart + 1200 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Combat10:
                    return indexStart + 1280 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Combat11:
                    return indexStart + 1360 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Combat12:
                    return indexStart + 1440 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Combat13:
                    return indexStart + 1520 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.Combat14:
                    return indexStart + 1600 + GameScene.Game.MapControl.Animation / animationSpeed % 8 + (int)direction * 10;
                case MirAnimation.Combat15:
                    return indexStart + 400 + GameScene.Game.MapControl.Animation / animationSpeed % 3 + (int)direction * 10;
                case MirAnimation.DragonRepulseStart:
                    return indexStart + 1600 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.DragonRepulseMiddle:
                    return indexStart + 1605 + GameScene.Game.MapControl.Animation / animationSpeed % 1 + (int)direction * 10;
                case MirAnimation.DragonRepulseEnd:
                    return indexStart + 1606 + GameScene.Game.MapControl.Animation / animationSpeed % 2 + (int)direction * 10;
                case MirAnimation.Struck:
                    return indexStart + 1840 + GameScene.Game.MapControl.Animation / animationSpeed % 3 + (int)direction * 10;
                case MirAnimation.Die:
                    return indexStart + 1920 + GameScene.Game.MapControl.Animation / animationSpeed % 10 + (int)direction * 10;
                case MirAnimation.Dead:
                    return indexStart + 1929 + GameScene.Game.MapControl.Animation / animationSpeed % 1 + (int)direction * 10;
                case MirAnimation.HorseStanding:
                    return indexStart + 2240 + GameScene.Game.MapControl.Animation / animationSpeed % 4 + (int)direction * 10;
                case MirAnimation.HorseWalking:
                    return indexStart + 2320 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.HorseRunning:
                    return indexStart + 2400 + GameScene.Game.MapControl.Animation / animationSpeed % 6 + (int)direction * 10;
                case MirAnimation.HorseStruck:
                    return indexStart + 2480 + GameScene.Game.MapControl.Animation / animationSpeed % 3 + (int)direction * 10;
                default:
                    return indexStart + GameScene.Game.MapControl.Animation / animationSpeed % 4 + (int)direction * 10;
            }
        }

        private static int DetermineDrawX(PlayerObject player, ExteriorEffect effect, MirDirection direction)
        {
            int initialDrawX = player.DrawX;

            if (player.Horse == HorseType.None)
            {
                return initialDrawX;
            }

            switch (effect)
            {
                case ExteriorEffect.A_WhiteAura:
                case ExteriorEffect.A_BlueAura:
                case ExteriorEffect.A_FlameAura:
                    switch (direction)
                    {
                        //case MirDirection.Up:
                        case MirDirection.UpRight:
                        case MirDirection.Right:
                        case MirDirection.DownRight:
                            return initialDrawX + 7;
                        //case MirDirection.Down:
                        case MirDirection.DownLeft:
                        case MirDirection.Left:
                        case MirDirection.UpLeft:
                            return initialDrawX - 8;
                    }
                    break;
                case ExteriorEffect.A_GreenWings:
                case ExteriorEffect.A_BlueWings:
                case ExteriorEffect.A_FlameWings:
                case ExteriorEffect.A_RedSinWings:
                case ExteriorEffect.A_DiamondFireWings:
                case ExteriorEffect.A_PurpleTentacles2:
                case ExteriorEffect.A_PhoenixWings:
                case ExteriorEffect.A_IceKingWings:
                    switch (direction)
                    {
                        //case MirDirection.Up:
                        case MirDirection.UpRight:
                        case MirDirection.Right:
                        case MirDirection.DownRight:
                            return initialDrawX + 7 + (player.CurrentAnimation == MirAnimation.HorseWalking ? 4 : player.CurrentAnimation == MirAnimation.HorseRunning ? 8 : 0);
                        //case MirDirection.Down:
                        case MirDirection.DownLeft:
                            return initialDrawX - 5 - (player.CurrentAnimation == MirAnimation.HorseWalking ? 4 : player.CurrentAnimation == MirAnimation.HorseRunning ? 8 : 0);
                        case MirDirection.Left:
                        case MirDirection.UpLeft:
                            return initialDrawX - 8 - (player.CurrentAnimation == MirAnimation.HorseWalking ? 4 : player.CurrentAnimation == MirAnimation.HorseRunning ? 8 : 0);
                    }
                    break;
            }

            return initialDrawX;
        }

        private static int DetermineDrawY(PlayerObject player, ExteriorEffect effect, MirDirection direction)
        {
            int initialDrawY = player.DrawY;

            if (player.Horse == HorseType.None)
            {
                return initialDrawY;
            }

            switch (effect)
            {
                case ExteriorEffect.A_WhiteAura:
                case ExteriorEffect.A_BlueAura:
                case ExteriorEffect.A_FlameAura:
                case ExteriorEffect.A_FlameAura2:
                    return initialDrawY - 25;
                case ExteriorEffect.A_PhoenixWings:
                case ExteriorEffect.A_IceKingWings:
                case ExteriorEffect.A_DiamondFireWings:
                case ExteriorEffect.A_PurpleTentacles2:
                    switch (direction)
                    {
                        case MirDirection.Down:
                        case MirDirection.DownLeft:
                            return initialDrawY - 15;
                        default:
                            return initialDrawY - 25;
                    }
                case ExteriorEffect.A_GreenWings:
                case ExteriorEffect.A_BlueWings:
                case ExteriorEffect.A_FlameWings:
                case ExteriorEffect.A_RedSinWings:
                    switch (direction)
                    {
                        case MirDirection.DownLeft:
                        case MirDirection.DownRight:
                        case MirDirection.Down:
                            return initialDrawY - 16;
                        default:
                            return initialDrawY - 30;
                    }
            }

            return initialDrawY;
        }
    }
}
