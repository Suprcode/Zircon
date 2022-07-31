using Client.Envir;
using Library;
using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class MirFishingFloat : MirEffect
    {
        public Point PlayerTarget;

        public MirLibrary WeaponLibrary;
        public int WeaponFrame;

        public MirFishingFloat(int startIndex, int frameCount, TimeSpan frameDelay, LibraryFile file, int startLight, int endLight, Color lightColour) : base(startIndex, frameCount, frameDelay, file, startLight, endLight, lightColour)
        {
        }

        public override void Draw()
        {
            base.Draw();

            DrawLine();
        }

        private void DrawLine()
        {
            var floatOffSet = new Point(-1, 4);

            MirImage floatImage = Library.Images[DrawFrame];

            if (floatImage != null && floatImage.Width > 1)
                floatOffSet.Offset((floatImage.Width / 2) + floatImage.OffSetX, (floatImage.Height / 2) + floatImage.OffSetY);
            else
                floatOffSet.Offset(25, 8);

            var rodOffSet = new Point(0, 0);

            MirImage weaponImage = WeaponLibrary.Images[WeaponFrame];
            
            if (weaponImage != null)
            {
                int x = 0, y = 0;
                switch (Direction)
                {
                    case MirDirection.Up:
                        rodOffSet.Offset(-24, 105);
                        x = 1; y = -1;
                        break;
                    case MirDirection.UpRight:
                        rodOffSet.Offset(-5, 89);
                        x = 1; y = -1;
                        break;
                    case MirDirection.Right:
                        rodOffSet.Offset(-6, -38);
                        x = 1; y = 1;
                        break;
                    case MirDirection.DownRight:
                        rodOffSet.Offset(-4, -3);
                        x = 1; y = 1;
                        break;
                    case MirDirection.Down:
                        rodOffSet.Offset(45, -3);
                        x = -1; y = 1;
                        break;
                    case MirDirection.DownLeft:
                        rodOffSet.Offset(73, -2);
                        x = -1; y = 1;
                        break;
                    case MirDirection.Left:
                        rodOffSet.Offset(117, 22);
                        x = -1; y = -1;
                        break;
                    case MirDirection.UpLeft:
                        rodOffSet.Offset(97, 73);
                        x = -1; y = -1;
                        break;
                }

                rodOffSet.Offset(weaponImage.Width * x, weaponImage.Height * y);
                rodOffSet.Offset(weaponImage.OffSetX, weaponImage.OffSetY);
            }

            var floatX = (MapTarget.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X + floatOffSet.X;
            var floatY = (MapTarget.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y + floatOffSet.Y;

            var rodX = (PlayerTarget.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X + rodOffSet.X;
            var rodY = (PlayerTarget.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y + rodOffSet.Y;

            var width = Math.Abs(floatX - rodX);
            var height = Math.Abs(floatY - rodY);

            var minX = Math.Min(floatX, rodX);
            var minY = Math.Min(floatY, rodY);

            Texture tex = new(DXManager.Device, width, height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            DataRectangle stream = tex.LockRectangle(0, LockFlags.Discard);

            using (Bitmap bitmap = new (width, height, width * 4, PixelFormat.Format32bppArgb, stream.Data.DataPointer))
            {
                using Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);

                using (SolidBrush brush = new(Color.FromArgb(100, 255, 255, 255)))
                {
                    using Pen pen = new(brush, 0.1F);
                    graphics.DrawLine(pen, floatX - minX, floatY - minY, rodX - minX, rodY - minY);
                }

                tex.UnlockRectangle(0);
                graphics.Dispose();
                stream.Data.Dispose();
                bitmap.Dispose();
            }

            DXManager.Sprite.Draw(tex, null, Vector3.Zero, new Vector3((float)minX, (float)minY, 0.0F), Color.White);
            tex.Dispose();
        }
    }
}
