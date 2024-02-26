using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.Scenes;
using Library;
using SlimDX.Windows;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace Client
{
    public sealed class TargetForm : RenderForm
    {
        public bool Resizing { get; private set; }

        public TargetForm() : base(Globals.ClientName)
        {
            AutoScaleMode = AutoScaleMode.None;

            AutoScaleDimensions = new SizeF(96F, 96F);

            ClientSize = new Size(1024, 768);
            
            Icon = Properties.Resources.Zircon;
            
            FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedSingle;

            MaximizeBox = false;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            if (GameScene.Game != null)
                GameScene.Game.MapControl.MapButtons = MouseButtons.None;

            CEnvir.Shift = false;
            CEnvir.Alt = false;
            CEnvir.Ctrl = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Config.ClipMouse && Focused)
                Cursor.Clip = RectangleToScreen(ClientRectangle);
            else
                Cursor.Clip = Rectangle.Empty;

            CEnvir.MouseLocation = e.Location;

            try
            {
                DXControl.ActiveScene?.OnMouseMove(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (GameScene.Game != null && e.Button == MouseButtons.Right && (GameScene.Game.SelectedCell != null || GameScene.Game.CurrencyPickedUp != null))
            {
                GameScene.Game.SelectedCell = null;
                GameScene.Game.CurrencyPickedUp = null;
                return;
            }

            try
            {
                DXControl.ActiveScene?.OnMouseDown(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {

            if (GameScene.Game != null)
                GameScene.Game.MapControl.MapButtons &= ~e.Button;

            try
            {
                DXControl.ActiveScene?.OnMouseUp(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnMouseClick(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnMouseClick(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnMouseWheel(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            CEnvir.Shift = e.Shift;
            CEnvir.Alt = e.Alt;
            CEnvir.Ctrl = e.Control;

            try
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    DXManager.ToggleFullScreen();
                    return;
                }
                
                DXControl.ActiveScene?.OnKeyDown(e);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            CEnvir.Shift = e.Shift;
            CEnvir.Alt = e.Alt;
            CEnvir.Ctrl = e.Control;

            if (e.KeyCode == Keys.Pause || e.KeyCode == Keys.PrintScreen)
               CreateScreenShot();

            try
            {
                DXControl.ActiveScene?.OnKeyUp(e);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnKeyPress(e);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (GameScene.Game != null && !GameScene.Game.ExitBox.Exiting)
                {
                    GameScene.Game.ExitBox.Visible = true;
                    e.Cancel = true;
                }
            }
            catch { }
        }

        public void Center()
        {
            CenterToScreen();
        }

        public static void CreateScreenShot()
        {
            Bitmap image = CEnvir.Target.GetImage();

            using (Graphics graphics = Graphics.FromImage(image))
            {
                string text = $"Date: {CEnvir.Now.ToShortDateString()}{Environment.NewLine}";
                text += $"Time: {CEnvir.Now.TimeOfDay:hh\\:mm\\:ss}{Environment.NewLine}";
                if (GameScene.Game != null)
                    text += $"Player: {MapObject.User.Name}{Environment.NewLine}";

                using (Font font = new Font(Config.FontName, CEnvir.FontSize(8F)))
                {
                    graphics.DrawString(text, font, Brushes.Black, 3, 33);
                    graphics.DrawString(text, font, Brushes.Black, 4, 32);
                    graphics.DrawString(text, font, Brushes.Black, 5, 33);
                    graphics.DrawString(text, font, Brushes.Black, 4, 34);
                    graphics.DrawString(text, font, Brushes.White, 4, 33);
                }
            }

            string path = Path.Combine(Application.StartupPath, @"Screenshots\");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            int count = Directory.GetFiles(path, "*.png").Length;
            string fileName = $"Image {count}.png";

            image.Save(Path.Combine(path, fileName), ImageFormat.Png);
            image.Dispose();

            if (GameScene.Game != null)
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.ScreenshotSaved, fileName), MessageType.System);
        }

        #region ScreenCapture

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr handle);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr handle);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr handle, int width, int height);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr handle, IntPtr handle2);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr handle, int destX, int desty, int width, int height,
                                         IntPtr handle2, int sourX, int sourY, int flag);
        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr handle);
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr handle, IntPtr handle2);
        [DllImport("gdi32.dll")]
        public static extern int DeleteObject(IntPtr handle);

        public Bitmap GetImage()
        {
            Point location = PointToClient(Location);

            location = new Point(-location.X, -location.Y);

            Rectangle r = new Rectangle(location, ClientSize);


            IntPtr sourceDc = GetWindowDC(Handle);
            IntPtr destDc = CreateCompatibleDC(sourceDc);

            IntPtr hBmp = CreateCompatibleBitmap(sourceDc, r.Width, r.Height);
            if (hBmp != IntPtr.Zero)
            {
                IntPtr hOldBmp = SelectObject(destDc, hBmp);
                BitBlt(destDc, 0, 0, r.Width, r.Height, sourceDc, r.X, r.Y, 0xCC0020); //0, 0, 13369376);
                SelectObject(destDc, hOldBmp);
                DeleteDC(destDc);
                ReleaseDC(Handle, sourceDc);

                Bitmap bmp = Image.FromHbitmap(hBmp);

                DeleteObject(hBmp);

                return bmp;
            }

            return null;
        }

        #endregion
    }
}
