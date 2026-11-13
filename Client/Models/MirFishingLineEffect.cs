using Client.Envir;
using Client.Scenes;
using Client.Scenes.Views;
using Library;
using Shared.Rendering;
using System;
using System.Drawing;

namespace Client.Models
{
    public sealed class MirFishingLineEffect : MirEffect
    {
        // Image-local rod tips, measured from Weapon12 frames 27080-27155.
        // Premium rod frames 32080-32155 use the same geometry. Rows follow MirDirection.
        private static readonly Point[][] MaleTips =
        [
            [new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0)],
            [new(68, 0), new(68, 0), new(69, 0), new(69, 0), new(69, 0), new(69, 0)],
            [new(114, 0), new(114, 0), new(114, 0), new(114, 0), new(114, 0), new(115, 0)],
            [new(93, 24), new(93, 24), new(93, 24), new(93, 24), new(93, 24), new(93, 25)],
            [new(20, 54), new(20, 54), new(20, 54), new(20, 54), new(20, 54), new(20, 56)],
            [new(0, 40), new(0, 40), new(0, 40), new(0, 40), new(0, 40), new(0, 41)],
            [new(0, 2), new(0, 2), new(0, 2), new(0, 2), new(0, 2), new(0, 2)],
            [new(1, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0)],
        ];

        // Female rods have different attachment points and sprite offsets.
        private static readonly Point[][] FemaleTips =
        [
            [new(0, 0), new(0, 0), new(1, 0), new(1, 0), new(0, 0), new(0, 0)],
            [new(70, 0), new(70, 0), new(70, 0), new(70, 0), new(70, 0), new(70, 0)],
            [new(109, 0), new(109, 0), new(110, 0), new(110, 0), new(109, 0), new(109, 0)],
            [new(85, 16), new(86, 16), new(86, 16), new(86, 16), new(86, 16), new(86, 16)],
            [new(12, 41), new(12, 41), new(11, 41), new(11, 41), new(11, 41), new(12, 41)],
            [new(0, 25), new(0, 25), new(0, 25), new(0, 25), new(0, 25), new(0, 25)],
            [new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0)],
            [new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0)],
        ];

        // The bobber occupies two visible columns inside its four-pixel-wide image.
        private static readonly int[] IdleTop = [1, 2, 3, 3, 2, 1];
        private static readonly int[] BiteTop = [1, 3, 0, 0, 0, 0];
        private readonly PlayerObject player;
        private MirEffect bobber;
        private readonly LinePoint[] points = new LinePoint[2];

        public MirFishingLineEffect(PlayerObject player, MirEffect bobber)
            : base(0, 1, TimeSpan.FromSeconds(1), LibraryFile.None, 0, 0, Globals.NoneColour)
        {
            this.player = player;
            this.bobber = bobber;
            Target = player;
        }

        public override void Process()
        {
            // Keep one thread for the fishing session, including gaps between bobber effects.
            if (!IsFishing)
            {
                Remove();
                return;
            }

            // Reuse the effect setters' cache invalidation for rod frames and camera motion.
            // The bobber invalidates the same texture when its own frame changes.
            DrawFrame = player.WeaponFrame;
            DrawX = player.DrawX;
            DrawY = player.DrawY;
        }

        public void SetBobber(MirEffect nextBobber)
        {
            // Replace the animation source without removing the thread between waiting cycles.
            bobber = nextBobber;
            GameScene.Game.MapControl.TextureValid = false;
        }

        public override void Remove()
        {
            // Erase the cached thread immediately when fishing ends.
            GameScene.Game.MapControl.TextureValid = false;
            base.Remove();
        }

        private bool IsFishing => !player.Dead && player.CurrentAction == MirAction.Fishing &&
            player.CurrentAnimation == MirAnimation.FishingWait && player.FishingState == FishingState.Cast;

        public override void Draw()
        {
            // The previous bobber's final frame remains the anchor until its replacement arrives.
            if (!IsFishing || CEnvir.Now < bobber.StartTime ||
                !TryGetRodTip(out PointF start) || !TryGetBobberTop(out PointF end)) return;

            points[0] = new LinePoint(start.X, start.Y);
            points[1] = new LinePoint(end.X, end.Y);

            // Keep the thread thin and translucent, including player transparency.
            float previousWidth = RenderingPipelineManager.GetLineWidth();
            try
            {
                RenderingPipelineManager.SetLineWidth(1F);
                RenderingPipelineManager.DrawLine(points, Color.FromArgb((int)(180 * player.Opacity), 210, 210, 195));
            }
            finally
            {
                RenderingPipelineManager.SetLineWidth(previousWidth);
            }
        }

        private bool TryGetRodTip(out PointF tip)
        {
            tip = default;
            if (!player.DrawWeapon || PlayerObject.CostumeShapeHideWeapon.Contains(player.CostumeShape) ||
                player.LibraryWeaponShape is not (115 or 116)) return false;

            // Match the actual library and absolute image index, not just a direction offset.
            LibraryFile file = player.Gender == MirGender.Male ? LibraryFile.M_Weapon12 : LibraryFile.WM_Weapon12;
            if (!CEnvir.LibraryList.TryGetValue(file, out MirLibrary library) || library != player.WeaponLibrary1)
                return false;

            int direction = (int)player.Direction;
            int frame = player.WeaponFrame - (player.LibraryWeaponShape - 110) * 5000 - 2080 - direction * 10;
            if ((uint)direction >= 8 || (uint)frame >= 6) return false;

            MirImage image = library.GetImage(player.WeaponFrame);
            if (image == null) return false;
            Point local = (player.Gender == MirGender.Male ? MaleTips : FemaleTips)[direction][frame];
            if (local.X >= image.Width || local.Y >= image.Height) return false;

            // Player sprites scale about the cell centre after compositing the weapon.
            tip = new PointF(
                player.DrawX + MapObject.CellWidth / 2F + (image.OffSetX + local.X + 0.5F - MapObject.CellWidth / 2F) * player.Scale,
                player.DrawY + MapObject.CellHeight / 2F + (image.OffSetY + local.Y + 0.5F - MapObject.CellHeight / 2F) * player.Scale);
            return true;
        }

        private bool TryGetBobberTop(out PointF top)
        {
            top = default;
            // A replacement may not have processed yet; its first frame is already a valid anchor.
            int frame = bobber.DrawFrame < 0 ? 0 : bobber.DrawFrame - bobber.StartIndex;
            if ((uint)frame >= 6 || bobber.Library == null) return false;

            // Blank bite frames have sentinel offsets far outside the map. Anchor at
            // the submerged tip's water position (the final bite frame) during those frames.
            bool biting = bobber.StartIndex == 1410;
            int imageIndex = biting && frame is >= 2 and <= 4 ? 1415 : bobber.StartIndex + frame;
            MirImage image = bobber.Library.GetImage(imageIndex);
            if (image == null) return false;

            // Match MirEffect's map positioning even after the last bobber has expired,
            // so camera movement during a handoff cannot leave the endpoint behind.
            float x = (bobber.MapTarget.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth
                - MapObject.User.MovingOffSet.X + bobber.AdditionalOffSet.X + MapControl.PixelOffsetX;
            float y = (bobber.MapTarget.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight
                - MapObject.User.MovingOffSet.Y + bobber.AdditionalOffSet.Y + MapControl.PixelOffsetY;
            top = new PointF(x + image.OffSetX + 1F,
                y + image.OffSetY + (biting ? BiteTop[frame] : IdleTop[frame]) + 0.5F);
            return true;
        }
    }
}
