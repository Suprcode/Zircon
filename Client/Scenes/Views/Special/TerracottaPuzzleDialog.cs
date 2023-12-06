using Client.Controls;
using Client.Envir;
using Library;
using SlimDX;
using SlimDX.X3DAudio;
using System;
using System.Drawing;

namespace Client.Scenes.Views
{
    public class TerracottaPuzzleDialog : DXImageControl
    {
        public DXButton CloseButton;
        public DXAnimatedControl TempleImage;

        public TerracottaPuzzleDialog()
        {
            Index = 2300;
            LibraryFile = LibraryFile.GameInter2;
            Movable = true;
            Sort = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TempleImage = new DXAnimatedControl
            {
                BaseIndex = 2360,
                FrameCount = 7,
                Location = new Point(70, 50),
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Animated = false,
                AnimationDelay = TimeSpan.FromMilliseconds(100)
            };

            AfterDraw += TerracottaPuzzleDialog_AfterDraw;
        }

        private void TerracottaPuzzleDialog_AfterDraw(object sender, EventArgs e)
        {
            // Draw an image (replace this with your image drawing logic)
            Library.Draw(2320, this.Location.X + finalPoint.X, this.Location.Y + finalPoint.Y, Color.White, false, 1F, ImageType.Image);

            if (nextUpdate >= CEnvir.Now) return;

            nextUpdate = CEnvir.Now.AddMilliseconds(10);

            // Update the parameter t to move along the curve
            t += speed;

            if (t > 1.0f)
            {
                t = 0.0f; // Reset when reaching the end of the curve
            }

            DrawImageAlongCurve(t);
        }

        private void DrawImageAlongCurve(float t)
        {
            Point[] points = new Point[] { startPoint, controlPoint1, controlPoint2, endPoint, startPoint };

            // Calculate the position on the curve at parameter t
            finalPoint = CalculateBezierPoint(t, points);
        }

        Point finalPoint = Point.Empty;

        private Point CalculateBezierPoint(float t, params Point[] points)
        {
            int n = points.Length - 1;

            // Bernstein basis functions
            Func<int, int, float> B = (i, k) => BinomialCoefficient(n, i) * (float)Math.Pow(t, i) * (float)Math.Pow(1 - t, n - i);

            float x = 0, y = 0;

            for (int i = 0; i <= n; i++)
            {
                x += B(i, n) * points[i].X;
                y += B(i, n) * points[i].Y;
            }

            return new Point((int)x, (int)y);
        }

        private int BinomialCoefficient(int n, int k)
        {
            if (k < 0 || k > n)
                return 0;

            if (k == 0 || k == n)
                return 1;

            int result = 1;
            k = Math.Min(k, n - k);

            for (int i = 0; i < k; ++i)
            {
                result *= (n - i);
                result /= (i + 1);
            }

            return result;
        }

        private DateTime nextUpdate;

        private float t = 0.0f; // Parameter for moving along the curve
        private float speed = 0.01f; // Speed of movement along the curve

        private Point startPoint = new Point(-50, 0);
        private Point controlPoint1 = new Point(0, 500);
        private Point controlPoint2 = new Point(500, 500);
        private Point endPoint = new Point(500, 0);
    }
}
