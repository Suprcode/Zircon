using System;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using SlimDX;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    public class FishingCatchDialog : DXImageControl
    {
        #region Properties

        private DateTime UpdateTime;
        private TimeSpan UpdateDelay = TimeSpan.FromMilliseconds(50);

        private const int PointerXStart = 10, PlayerPointerYStart = 65, FishPointerYStart = 82;

        private const int FishBlockSize = 25;
        private const int FishBlocksTotal = 4;
        private int FishMaxTotal => FishBlockSize * FishBlocksTotal;
        private int FishMaxCurrent => FishBlockSize * ThrowQuality;

        //Client Set Values
        private bool Pressed;
        private int PlayerLocation;
        private int FishLocation;

        public bool AutoCast = true;

        private bool FishingStarted;
        private bool FishDirectionRight;

        //Server Set Values
        private int PointsCurrent;

        private int ThrowQuality;
        private int PointsRequired;
        private int MovementSpeed;
        private int RequiredAccuracy;

        #endregion

        public DXImageControl FishFoundBase, FishFoundCircle, FishFoundButton;

        public DXImageControl PlayerPointer;
        public DXImageControl FishPointer;
        public DXControl FishBar;
        public DXControl ProgressBar;

        public DXButton CloseButton;
        public DXCheckBox AutoCastCheckBox;

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            ResetAll();
        }

        public FishingCatchDialog()
        {
            Movable = true;
            Index = 230;
            LibraryFile = LibraryFile.Interface;
            Size = new Size(252, 144);

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 5, 5);
            CloseButton.MouseClick += (o, e) =>
            {
                GameScene.Game.MapControl.FishingState = FishingState.Cancel;
                Visible = false;
            };

            AutoCastCheckBox = new DXCheckBox
            {
                Label = { Text = "Auto Cast:" },
                Parent = this,
                Checked = true,
            };
            AutoCastCheckBox.Location = new Point(252 - AutoCastCheckBox.Size.Width - 16, 47);

            CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out MirLibrary library);

            FishBar = new DXControl
            {
                Parent = this,
                Location = new Point(19, 76),
                Size = library.GetSize(231),
            };
            FishBar.BeforeDraw += (o, e) =>
            {
                if (library == null) return;

                if (GameScene.Game.User.FishingState != FishingState.Cast || !GameScene.Game.User.FishFound)
                {
                    return;
                }

                var count = FishLocation;

                float percent = Math.Min(1, Math.Max(0, count / (float)FishMaxTotal));

                if (percent == 0) return;

                MirImage image = library.CreateImage(231, ImageType.Image);

                PresentTexture(image.Image, this, new Rectangle(FishBar.DisplayArea.X, FishBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, FishBar);
            };

            FishPointer = new DXImageControl
            {
                Index = 234,
                LibraryFile = LibraryFile.Interface,
                Parent = this,
                Location = new Point(PointerXStart, FishPointerYStart)
            };

            ProgressBar = new DXControl
            {
                Parent = this,
                Location = new Point(19, 91),
                Size = library.GetSize(232),
            };
            ProgressBar.BeforeDraw += (o, e) =>
            {
                if (library == null) return;

                if (GameScene.Game.User.FishingState != FishingState.Cast || !GameScene.Game.User.FishFound)
                {
                    return;
                }

                var count = PointsCurrent;

                float percent = Math.Min(1, Math.Max(0, count / (float)PointsRequired));

                if (percent == 0) return;

                MirImage image = library.CreateImage(232, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, this, new Rectangle(ProgressBar.DisplayArea.X, ProgressBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, ProgressBar);
            };

            PlayerPointer = new DXImageControl
            {
                Index = 233,
                LibraryFile = LibraryFile.Interface,
                Parent = this,
                Location = new Point(0, 0)
            };

            int x = 105, y = 102;
            FishFoundBase = new DXImageControl
            {
                Index = 4500,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(x, y),
                Parent = this,
                Size = new Size(44, 42),
                Visible = true
            };

            FishFoundCircle = new DXImageControl
            {
                Index = 4501,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(x, y),
                Parent = this,
                Size = new Size(44, 42),
                Visible = true
            };

            FishFoundButton = new DXButton
            {
                Index = 4510,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(x + 6, y + 6),
                Parent = this,
                Size = new Size(32, 30),
                Visible = false
            };
            FishFoundButton.MouseEnter += (o, e) =>
            {
                FishFoundButton.Index = 4511;
            };
            FishFoundButton.MouseLeave += (o, e) =>
            {
                FishFoundButton.Index = 4510;
                Pressed = false;
            };
            FishFoundButton.MouseDown += (o, e) =>
            {
                Pressed = true;
            };
            FishFoundButton.MouseUp += (o, e) =>
            {
                Pressed = false;
            };
        }

        public bool CaughtFish
        {
            get
            {
                return GameScene.Game.User.FishingState == FishingState.Cast && Math.Abs(FishLocation - PlayerLocation) <= RequiredAccuracy;
            }
        }

        public override void Process()
        {
            base.Process();

            if (!IsVisible) return;

            if (FishingStarted)
            {
                if (GameScene.Game.User.FishingState != FishingState.Cast || !GameScene.Game.User.FishFound)
                {
                    return;
                }
            }

            if (GameScene.Game.User.FishFound)
            {
                FishFoundButton.Visible = true;
            }

            FishingStarted = true;

            float percent = Math.Min(1, Math.Max(0, FishMaxCurrent / (float)FishMaxTotal));

            FishPointer.Location = new Point(PointerXStart + (int)(percent * 216), FishPointerYStart);

            if (UpdateTime < CEnvir.Now)
            {
                UpdateTime = CEnvir.Now.Add(UpdateDelay);

                ProcessFish();
                ProcessUser();
            }
        }

        private void ResetAll()
        {
            if (!FishingStarted)
                return;

            FishingStarted = false;

            PlayerLocation = 0;
            FishLocation = 0;

            FishFoundButton.Visible = false;

            PlayerPointer.Location = new Point(PointerXStart, PlayerPointerYStart);
            FishPointer.Location = new Point(PointerXStart, FishPointerYStart);
        }

        private void ProcessUser()
        {
            if (!FishingStarted)
            {
                return;
            }

            if (Pressed)
            {
                PlayerLocation = Math.Min(PlayerLocation + MovementSpeed, FishMaxCurrent);
            }
            else
            {
                PlayerLocation = Math.Max(PlayerLocation - MovementSpeed, 0);
            }

            float percent = Math.Min(1, Math.Max(0, PlayerLocation / (float)FishMaxTotal));

            PlayerPointer.Location = new Point(PointerXStart + (int)(percent * 215), PlayerPointerYStart);
        }

        private void ProcessFish()
        {
            if (!FishingStarted)
                return;

            var changeDir = CEnvir.Random.Next(0, 7) == 0;

            if (changeDir)
            {
                FishDirectionRight = !FishDirectionRight;
            }

            if (FishDirectionRight)
            {
                FishLocation = Math.Min(FishLocation + MovementSpeed, FishMaxCurrent);
            }
            else
            {
                FishLocation = Math.Max(FishLocation - MovementSpeed, 0);
            }
        }

        public void Update(S.FishingStats p)
        {
            PointsCurrent = p.CurrentPoints;

            PointsRequired = p.RequiredPoints;
            MovementSpeed = p.MovementSpeed;

            if (p.ThrowQuality > -1)
                ThrowQuality = p.ThrowQuality;

            if (p.RequiredAccuracy > -1)
                RequiredAccuracy = p.RequiredAccuracy;

            AutoCastCheckBox.Enabled = p.CanAutoCast;

            if (!AutoCastCheckBox.Enabled)
                AutoCastCheckBox.Checked = false;
        }

        #region TestCode
        //Arrow.BeforeDraw += (o, e) =>
        //{
        //    if (library == null) return;

        //    MirImage image = library.CreateImage(5110, ImageType.Image);

        //    if (image == null) return;

        //    if (timerCount < CEnvir.Now)
        //    {
        //        timerCount = CEnvir.Now.Add(TimeSpan.FromMilliseconds(200));
        //        count += 0.1F;
        //    }

        //    var l = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 90, 0), 1);

        //    //var angle = Math.Atan2(4 * -1, 2) * 180 / Math.PI;

        //    //var degree = (float)((angle + 360) % 360);

        //    //TODO Lerp!

        //    FishActionLabel.Text = l.X + " " + l.Y + " " + l.Z;

        //    library.DrawBlend(5110, 1f, Color.White, 20, 20, count, 1F, ImageType.Image, false, 0);

        //    //PresentTexture(image.Image, this, new Rectangle(Arrow.DisplayArea.X, Arrow.DisplayArea.Y, image.Width, image.Height), Color.White, Arrow);
        //};


        //CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out library);

        //ProgressBar = new DXControl
        //{
        //    Location = new Point(0, 0),
        //    Parent = this,
        //    Modal = true,
        //    Size = library.GetSize(4501),
        //    Visible = true
        //};

        //ProgressBar.BeforeDraw += (o, e) =>
        //{
        //    //MirImage image = library.CreateImage(4501, ImageType.Image);

        //    //if (image == null) return;

        //    //PresentTexture(image.Image, this, new Rectangle(ProgressBar.DisplayArea.X, ProgressBar.DisplayArea.Y, image.Width, image.Height), Color.White, ProgressBar);

        //    //Texture tex = new Texture(DXManager.Device, ProgressBar.Size.Width + 10, ProgressBar.Size.Height + 10, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
        //    //DataRectangle stream = tex.LockRectangle(0, LockFlags.Discard);

        //    //using (Bitmap bitmap = new Bitmap(ProgressBar.Size.Width + 10, ProgressBar.Size.Height + 10, (ProgressBar.Size.Width + 10) * 4, PixelFormat.Format32bppArgb, stream.Data.DataPointer))
        //    //{
        //    //    using (Graphics graphics = Graphics.FromImage(bitmap))
        //    //    {
        //    //        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        //    //        graphics.Clear(Color.Transparent);

        //    //        using (SolidBrush brush = new SolidBrush(Color.Silver))
        //    //        {
        //    //            using (Pen pen = new Pen(brush, 6f))
        //    //            {
        //    //                pen.StartCap = LineCap.Flat;
        //    //                pen.EndCap = LineCap.Flat;

        //    //                graphics.DrawArc(pen, 2, 4, ProgressBar.Size.Width - 9, ProgressBar.Size.Height - 7, -90, 270);
        //    //            }
        //    //        }

        //    //        image.Image.UnlockRectangle(0);
        //    //        graphics.Dispose();
        //    //        stream.Data.Dispose();
        //    //        DXManager.Sprite.Flush();
        //    //        bitmap.Dispose();
        //    //    }
        //    //}
        //    //DXManager.Sprite.Draw(tex, null, Vector3.Zero, new Vector3((float)Location.X + 1, (float)Location.Y - 2, 0.0F), Color.White);
        //    //tex.Dispose();
        //};
        #endregion
    }
}
