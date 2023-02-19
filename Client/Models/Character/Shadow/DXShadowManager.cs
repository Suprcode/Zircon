using Client.Envir;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;


namespace Client.Models.Character.Shadow
{
    public class DXShadowManager
    {
        private readonly PlayerObject Player;

        public DXShadowManager(PlayerObject player)
        {
            Player = player;
        }

        public void DrawShadow(DXShadowBoundary dxShadowBoundary)
        {
            MirImage image = Player.BodyLibrary?.GetImage(Player.ArmourFrame);

            if (image == null) return;

            int w = (Player.DrawX + image.OffSetX) - dxShadowBoundary.Left;
            int h = (Player.DrawY + image.OffSetY) - dxShadowBoundary.Top;

            Matrix m = Matrix.Scaling(1F, 0.5f, 0);

            m.M21 = -0.50F;
            DXManager.Sprite.Transform = m * Matrix.Translation(Player.DrawX + image.ShadowOffSetX - w + (image.Height) / 2 + h / 2, Player.DrawY + image.ShadowOffSetY - h / 2, 0);

            DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);

            float oldOpacity = DXManager.Opacity;
            if (oldOpacity != 0.5F) DXManager.SetOpacity(0.5F);
            DXManager.Sprite.Draw(DXManager.ScratchTexture, Rectangle.FromLTRB(dxShadowBoundary.Left, dxShadowBoundary.Top, dxShadowBoundary.Right, dxShadowBoundary.Bottom), Vector3.Zero, Vector3.Zero, Color.Black);

            DXManager.Sprite.Transform = Matrix.Identity;
            DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

            if (0.5F != oldOpacity) DXManager.SetOpacity(oldOpacity);
        }
    }
}
