using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using SlimDX;
using SlimDX.Direct3D9;

namespace Client.Scenes.Views
{
    public class FishingDialog
    {
    }

    public class FishingCatchDialog : DXImageControl
    {
        public DXControl ProgressBar;
        public DXImageControl State;

        public FishingCatchDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 4500;
            Movable = false;
            Sort = false;
            Size = new Size(44, 42);

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary library);

            ProgressBar = new DXControl
            {
                Location = new Point(0, 0),
                Parent = this,
                Modal = true,
                Size = library.GetSize(4501),
            };
            ProgressBar.BeforeDraw += (o, e) =>
            {
                MirImage image = library.CreateImage(4501, ImageType.Image);

                if (image == null) return;

                //PresentTexture(image.Image, this, new Rectangle(ProgressBar.DisplayArea.X, ProgressBar.DisplayArea.Y, image.Width, image.Height), Color.White, ProgressBar);

                Texture tex = new Texture(DXManager.Device, ProgressBar.Size.Width + 10, ProgressBar.Size.Height + 10, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                DataRectangle stream = tex.LockRectangle(0, LockFlags.Discard);

                using (Bitmap bitmap = new Bitmap(ProgressBar.Size.Width + 10, ProgressBar.Size.Height + 10, (ProgressBar.Size.Width + 10) * 4, PixelFormat.Format32bppArgb, stream.Data.DataPointer))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.Clear(Color.Transparent);

                        using (SolidBrush brush = new SolidBrush(Color.Silver))
                        {
                            using (Pen pen = new Pen(brush, 6f))
                            {
                                pen.StartCap = LineCap.Flat;
                                pen.EndCap = LineCap.Flat;

                                graphics.DrawArc(pen, 2, 4, ProgressBar.Size.Width - 9, ProgressBar.Size.Height - 7, -90, 270);
                            }
                        }
                        image.Image.UnlockRectangle(0);
                        graphics.Dispose();
                        stream.Data.Dispose();
                        DXManager.Sprite.Flush();
                        bitmap.Dispose();
                    }
                }
                DXManager.Sprite.Draw(tex, null, Vector3.Zero, new Vector3((float)Location.X + 1, (float)Location.Y - 2, 0.0F), Color.White);
                tex.Dispose();
            };

            State = new DXButton
            {
                Index = 4510,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(6, 6),
                Parent = this,
                Size = new Size(32, 30),
                Visible = true
            };
            State.MouseEnter += (o, e) =>
            {
                State.Index = 4511;
            };
            State.MouseLeave += (o, e) =>
            {
                State.Index = 4510;
            };

        }
    }
}
