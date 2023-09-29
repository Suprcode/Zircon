using System;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.Properties;
using Client.UserModels;
using Library;
using SlimDX;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    public sealed class FishingDialog : DXImageControl
    {
        #region Properties

        private CharacterDialog CharacterBox;

        public DXButton CloseButton;

        public ClientUserItem[] Equipment
        {
            get
            {
                return GameScene.Game.Equipment;
            }
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        #endregion

        #region Settings

        public WindowSetting Settings;
        public WindowType Type
        {
            get { return WindowType.FishingBox; }
        }

        public void LoadSettings()
        {
            if (Type == WindowType.None || !CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);

            if (Settings != null)
            {
                ApplySettings();
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = Visible;
            Settings.Location = Location;
        }

        public void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            Visible = Settings.Visible;
        }

        #endregion

        public FishingDialog(CharacterDialog characterBox)
        {
            CharacterBox = characterBox;

            LibraryFile = LibraryFile.Interface;
            Index = 220;
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

            DXItemCell cell;

            CharacterBox.Grid[(int)EquipmentSlot.Hook] = cell = new DXItemCell
            {
                Location = new Point(14, 169),
                Parent = this,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Hook,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 221);

            CharacterBox.Grid[(int)EquipmentSlot.Float] = cell = new DXItemCell
            {
                Location = new Point(14, 209),
                Parent = this,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Float,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 222);

            CharacterBox.Grid[(int)EquipmentSlot.Bait] = cell = new DXItemCell
            {
                Location = new Point(54, 209),
                Parent = this,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Bait,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 224);

            CharacterBox.Grid[(int)EquipmentSlot.Finder] = cell = new DXItemCell
            {
                Location = new Point(94, 209),
                Parent = this,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Finder,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 223);

            CharacterBox.Grid[(int)EquipmentSlot.Reel] = cell = new DXItemCell
            {
                Location = new Point(134, 209),
                Parent = this,
                Border = true,
                ItemGrid = Equipment,
                Slot = (int)EquipmentSlot.Reel,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 225);
        }

        #region Methods

        public void Draw(DXItemCell cell, int index, bool backgroundCell = false)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s;
            int x, y;

            s = InterfaceLibrary.GetSize(index);
            x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                CharacterBox = null;
                Settings = null;
            }
        }

        #endregion
    }

    public class FishingCatchDialog : DXImageControl
    {
        #region Properties

        private DateTime UpdateTime;
        private TimeSpan UpdateDelay = TimeSpan.FromMilliseconds(50);

        private const int PointerXStart = 10, PlayerPointerYStart = 65, ThrowDistancePointerY = 82;

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

        //Change this to swap between fish/player being the bar/pointer
        private bool FishAsBar = false;

        #endregion

        public DXImageControl FishFoundBase, FishFoundCircle, FishFoundButton;

        public DXImageControl MovingPointer;
        public DXImageControl CatchBarTexture, ThrowDistancePointer;
        public DXControl CatchBar, CatchInnerBar;
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
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) =>
            {
                GameScene.Game.MapControl.FishingState = FishingState.Cancel;
                Visible = false;
            };

            AutoCastCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.FishingCatchDialogAutoCast },
                Parent = this,
                Checked = true,
            };
            AutoCastCheckBox.Location = new Point(252 - AutoCastCheckBox.Size.Width - 16, 47);

            CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out MirLibrary library);

            var barImage = library.GetSize(231);

            CatchBar = new DXControl
            {
                Parent = this,
                Location = new Point(19, 76),
                Size = library.GetSize(231),
            };
            CatchBar.BeforeDraw += (o, e) =>
            {
                if (GameScene.Game.User.FishingState != FishingState.Cast || !GameScene.Game.User.FishFound)
                {
                    MovingPointer.Visible = false;
                    CatchInnerBar.Visible = false;
                    return;
                }

                MovingPointer.Visible = true;
                CatchInnerBar.Visible = true;

                //var w = (int)(barImage.Width * (FishMaxCurrent / (float)FishMaxTotal));

                CatchBar.Size = new Size(barImage.Width, barImage.Height);
            };

            CatchInnerBar = new DXControl
            {
                Parent = CatchBar,
                Location = new Point(0, 0),
                Size = new Size(1, 1),
                Visible = false
            };
            CatchInnerBar.BeforeDraw += (o, e) =>
            {
                var currentLocation = FishAsBar ? FishLocation : PlayerLocation;

                float left = currentLocation - (float)(RequiredAccuracy);
                float right = currentLocation + (float)(RequiredAccuracy);

                var x = (int)(barImage.Width * (left / (float)FishMaxTotal));
                var w = (int)(barImage.Width * ((RequiredAccuracy * 2) / (float)FishMaxTotal));

                CatchInnerBar.Location = new Point(x, 0);
                CatchInnerBar.Size = new Size(w, barImage.Height);

                CatchBarTexture.Location = new Point(0 - x, 0);
                CatchBarTexture.ImageOpacity = CaughtFish ? 1F : 0.5F;
            };

            CatchBarTexture = new DXImageControl
            {
                Index = 231,
                LibraryFile = LibraryFile.Interface,
                Parent = CatchInnerBar,
                Location = new Point(0, 0)
            };

            ThrowDistancePointer = new DXImageControl
            {
                Index = 234,
                LibraryFile = LibraryFile.Interface,
                Parent = this,
                Location = new Point(PointerXStart, ThrowDistancePointerY),
                Visible = true
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

            MovingPointer = new DXImageControl
            {
                Index = 0,
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

            ThrowDistancePointer.Location = new Point(PointerXStart + (int)(percent * 216), ThrowDistancePointerY);

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

            MovingPointer.Location = new Point(PointerXStart, PlayerPointerYStart);
            ThrowDistancePointer.Location = new Point(PointerXStart, ThrowDistancePointerY);
        }

        private void ProcessUser()
        {
            if (!FishingStarted)
                return;

            if (Pressed)
            {
                PlayerLocation = Math.Min(PlayerLocation + MovementSpeed, FishMaxTotal);
            }
            else
            {
                PlayerLocation = Math.Max(PlayerLocation - MovementSpeed, 0);
            }

            if (FishAsBar)
            {
                float percent = Math.Min(1, Math.Max(0, PlayerLocation / (float)FishMaxTotal));

                MovingPointer.Location = new Point(PointerXStart + (int)(percent * 215), PlayerPointerYStart);
            }
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
                FishLocation = Math.Min(FishLocation + MovementSpeed, FishMaxTotal);
            }
            else
            {
                FishLocation = Math.Max(FishLocation - MovementSpeed, 0);
            }

            if (!FishAsBar)
            {
                float percent = Math.Min(1, Math.Max(0, FishLocation / (float)FishMaxTotal));

                MovingPointer.Location = new Point(PointerXStart + (int)(percent * 215), PlayerPointerYStart);
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

            var movingPointerImage = GetMovingPointerImage();

            MovingPointer.Index = movingPointerImage.Index;
            MovingPointer.LibraryFile = movingPointerImage.File;
        }

        public (int Index, LibraryFile File) GetMovingPointerImage()
        {
            if (FishAsBar)
            {
                return (233, LibraryFile.Interface);
            }
            else
            {
                //Change icon to show as fish?
                return (233, LibraryFile.Interface);
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (FishFoundBase != null)
                {
                    if (!FishFoundBase.IsDisposed)
                        FishFoundBase.Dispose();

                    FishFoundBase = null;
                }

                if (FishFoundCircle != null)
                {
                    if (!FishFoundCircle.IsDisposed)
                        FishFoundCircle.Dispose();

                    FishFoundCircle = null;
                }

                if (FishFoundButton != null)
                {
                    if (!FishFoundButton.IsDisposed)
                        FishFoundButton.Dispose();

                    FishFoundButton = null;
                }

                if (MovingPointer != null)
                {
                    if (!MovingPointer.IsDisposed)
                        MovingPointer.Dispose();

                    MovingPointer = null;
                }

                if (CatchBarTexture != null)
                {
                    if (!CatchBarTexture.IsDisposed)
                        CatchBarTexture.Dispose();

                    CatchBarTexture = null;
                }

                if (ThrowDistancePointer != null)
                {
                    if (!ThrowDistancePointer.IsDisposed)
                        ThrowDistancePointer.Dispose();

                    ThrowDistancePointer = null;
                }

                if (CatchBar != null)
                {
                    if (!CatchBar.IsDisposed)
                        CatchBar.Dispose();

                    CatchBar = null;
                }

                if (CatchInnerBar != null)
                {
                    if (!CatchInnerBar.IsDisposed)
                        CatchInnerBar.Dispose();

                    CatchInnerBar = null;
                }

                if (ProgressBar != null)
                {
                    if (!ProgressBar.IsDisposed)
                        ProgressBar.Dispose();

                    ProgressBar = null;
                }

                if (AutoCastCheckBox != null)
                {
                    if (!AutoCastCheckBox.IsDisposed)
                        AutoCastCheckBox.Dispose();

                    AutoCastCheckBox = null;
                }
            }
}

        #endregion

        #region TestCode

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
