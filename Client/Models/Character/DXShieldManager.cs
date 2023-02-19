using Client.Envir;
using Client.Models.Character.Shadow;
using Library;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;


namespace Client.Models.Character
{
    public class DXShieldManager
    {
        //Ducky: share this
        private const int FemaleOffSet = 5000;

        #region Shield Librarys
        public Dictionary<int, LibraryFile> ShieldList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Shield1,
            [1] = LibraryFile.M_Shield2,
            [0 + FemaleOffSet] = LibraryFile.WM_Shield1,
            [1 + FemaleOffSet] = LibraryFile.WM_Shield2,

            [100] = LibraryFile.EquipEffect_Part,

            [100 + FemaleOffSet] = LibraryFile.EquipEffect_Part,
        };
        #endregion

        private readonly PlayerObject Player;

        private MirLibrary ShieldLibrary;

        public DXShieldManager(PlayerObject player)
        {
            Player = player;
        }

        //TODO: Ducky: maybe SheidlFrame belongs here too?
        public void DrawShieldAndCalculateShadow(DXShadowBoundary dxShadowBoundary)
        {
            if (Player.ShieldShape < 0 || Player.ShieldShape > 1000) return;

            MirImage image = ShieldLibrary?.GetImage(Player.ShieldFrame);
            if (image == null) return;

            ShieldLibrary.Draw(Player.ShieldFrame, Player.DrawX, Player.DrawY, Color.White, true, 1F, ImageType.Image);
            dxShadowBoundary.Transform(image, Player.DrawX, Player.DrawY);
        }
        
        public void DrawShieldEffect()
        {
            if (!Config.DrawEffects) return;
            if (Player.Horse != HorseType.None) return;

            switch (Player.CurrentAction)
            {
                case MirAction.Die:
                case MirAction.Dead:
                    break;
                default:
                    int ShieldFrame = Player.ShieldFrame;
                    int DrawX = Player.DrawX, DrawY = Player.DrawY;

                    if (Player.ShieldShape >= 1000)
                    {
                        ShieldLibrary.DrawBlend(ShieldFrame + 100, DrawX, DrawY, Color.White, true, 0.8f, ImageType.Image);
                        ShieldLibrary.Draw(ShieldFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);
                    }
                    break;
            }
        }

        public void UpdateLibraries()
        {
            if (Player.ShieldShape < 0) return;
            LibraryFile file = LibraryFile.None;

            int offset = MirGender.Male.Equals(Player.Gender) ? 0 : FemaleOffSet;
            ShieldList.TryGetValue(Player.ShieldShape / 10 + offset, out file);
            CEnvir.LibraryList.TryGetValue(file, out ShieldLibrary);
        }

        public bool IsMouseOver(Point mouse)
        {
            return Player.ShieldShape >= 0 && ShieldLibrary != null &&
                ShieldLibrary.VisiblePixel(Player.ShieldFrame, new Point(mouse.X - Player.DrawX, mouse.Y - Player.DrawY), false, true);
        }
    }
}
