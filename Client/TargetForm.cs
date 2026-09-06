using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.Scenes;
using Library;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace Client
{
    public sealed class TargetForm : Form
    {
        public bool Resizing { get; private set; }

        public float WindowScale
        {
            get
            {
                if (Config.FullScreen || Config.Borderless)
                    return 1F;

                return Config.WindowScalePercent is >= 100 and <= 300
                    ? Config.WindowScalePercent / 100F
                    : Math.Max(1F, DeviceDpi / 96F);
            }
        }

        public float UIScale => DXControl.ActiveScene is GameScene
            ? Math.Clamp(Config.UIScalePercent / 100F, 1F, 3F)
            : 1F;

        public float TextRasterScale => WindowScale * UIScale;

        public TargetForm()
        {
            Text = Globals.ClientName;
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;

            AutoScaleDimensions = new SizeF(96F, 96F);

            ClientSize = new Size(1024, 768);

            Icon = Properties.Resources.Zircon;

            FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedSingle;

            MaximizeBox = false;
        }

        public void SetLogicalClientSize(Size size)
        {
            ClientSize = new Size(
                Math.Max(1, (int)Math.Round(size.Width * WindowScale)),
                Math.Max(1, (int)Math.Round(size.Height * WindowScale)));
        }

        public void ApplyWindowScale()
        {
            Size logicalSize = DXControl.ActiveScene?.Size ?? Config.GameSize;
            RenderingPipelineManager.SetResolution(logicalSize);
            Program.InvalidateUiRenderCaches();
            Invalidate();
        }

        public void ApplyUIScale(float previousScale)
        {
            if (DXControl.ActiveScene is GameScene game)
                game.UIScaleChanged(previousScale);

            Program.InvalidateUiRenderCaches();
            Invalidate();
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);

            if (!Config.FullScreen && !Config.Borderless)
                SetLogicalClientSize(DXControl.ActiveScene?.Size ?? Config.GameSize);

            Program.InvalidateUiRenderCaches();
        }

        public static float GetMonitorScale(Screen screen)
        {
            if (Config.FullScreen || Config.Borderless)
                return 1F;

            if (Config.WindowScalePercent is >= 100 and <= 300)
                return Config.WindowScalePercent / 100F;
            if (screen == null)
                return 1F;

            TargetForm target = CEnvir.Target;
            if (target?.IsHandleCreated == true &&
                string.Equals(Screen.FromControl(target).DeviceName, screen.DeviceName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    uint windowDpi = GetDpiForWindow(target.Handle);
                    if (windowDpi > 0)
                        return windowDpi / 96F;
                }
                catch (EntryPointNotFoundException)
                {
                }
            }

            try
            {
                Point centre = new Point(screen.Bounds.Left + screen.Bounds.Width / 2, screen.Bounds.Top + screen.Bounds.Height / 2);
                IntPtr monitor = MonitorFromPoint(centre, 2);

                if (monitor != IntPtr.Zero && GetDpiForMonitor(monitor, 0, out uint dpiX, out _) == 0 && dpiX > 0)
                    return dpiX / 96F;
            }
            catch (EntryPointNotFoundException)
            {
            }
            catch (DllNotFoundException)
            {
            }

            return 1F;
        }

        private MouseEventArgs ToLogicalMouseEventArgs(MouseEventArgs e)
        {
            Size logicalSize = DXControl.ActiveScene?.Size ?? Config.GameSize;
            float scaleX = ClientSize.Width > 0 ? logicalSize.Width / (float)ClientSize.Width : 1F / WindowScale;
            float scaleY = ClientSize.Height > 0 ? logicalSize.Height / (float)ClientSize.Height : 1F / WindowScale;

            return new MouseEventArgs(
                e.Button,
                e.Clicks,
                (int)Math.Floor(e.X * scaleX),
                (int)Math.Floor(e.Y * scaleY),
                e.Delta);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.F10) == Keys.F10)
            {
                return true;
            }

            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return true;
            }

            return base.IsInputKey(keyData);
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

            e = ToLogicalMouseEventArgs(e);
            if (DXControl.ActiveScene is GameScene game)
                e = game.ToUIMouseEventArgs(e);
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
                e = ToLogicalMouseEventArgs(e);
                if (DXControl.ActiveScene is GameScene game)
                    e = game.ToUIMouseEventArgs(e);
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
                e = ToLogicalMouseEventArgs(e);
                if (DXControl.ActiveScene is GameScene game)
                    e = game.ToUIMouseEventArgs(e);
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
                e = ToLogicalMouseEventArgs(e);
                if (DXControl.ActiveScene is GameScene game)
                    e = game.ToUIMouseEventArgs(e);
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
                e = ToLogicalMouseEventArgs(e);
                if (DXControl.ActiveScene is GameScene game)
                    e = game.ToUIMouseEventArgs(e);
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
                e = ToLogicalMouseEventArgs(e);
                if (DXControl.ActiveScene is GameScene game)
                    e = game.ToUIMouseEventArgs(e);
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
                    RenderingPipelineManager.ToggleFullScreen();
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

                using (Font font = new Font(Config.FontName, CEnvir.FontSize(8F) * CEnvir.Target.TextRasterScale))
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
        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromPoint(Point point, uint flags);
        [DllImport("shcore.dll")]
        static extern int GetDpiForMonitor(IntPtr monitor, int dpiType, out uint dpiX, out uint dpiY);
        [DllImport("user32.dll")]
        static extern uint GetDpiForWindow(IntPtr window);
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
