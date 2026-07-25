using Client.Controls;
using Client.Envir;
using Client.Models;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class HorseTameDialog : DXControl
    {
        private const int LoopBaseIndex = 7600;
        private const int ResultBaseIndex = 7610;
        private const int AngleBaseIndex = 7620;
        private const int ProgressFillIndex = 7630;
        private const int ProgressOutlineIndex = 7631;
        private const int AngleCount = 10;
        private const int AnimationFrameDuration = 200;
        private const int ResultFrameCount = 2;
        private const int MaximumInitialProgress = 50;

        private static readonly TimeSpan LoopDuration = TimeSpan.FromMilliseconds(AnimationFrameDuration * AngleCount);
        private const int ResultDuration = AnimationFrameDuration * ResultFrameCount;

        private uint _targetObjectID;
        private int _progress;
        private int _targetAngle;
        private bool _promptVisible;
        private bool _completed;
        private Size _animationAreaSize;
        private Size _progressSize;
        private Point _animationAnchor;
        private int _healthBarWidth;
        private Timer _promptTimer, _resultTimer;
        private bool _initialized;

        public DXAnimatedControl LassoAnimation;
        public DXImageControl AngleImage;
        public DXControl ProgressContainer;
        public DXImageControl ProgressFill, ProgressOutline;
        public MonsterObject TamingTarget { get; private set; }

        public HorseTameDialog()
        {
            BackColour = Color.Empty;
            Border = false;
            Movable = false;
            PassThrough = true;
            Sort = true;

            _promptTimer = new Timer();
            _promptTimer.Tick += PromptTimer_Tick;

            _resultTimer = new Timer
            {
                Interval = ResultDuration,
            };
            _resultTimer.Tick += ResultTimer_Tick;

            Visible = false;

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary library);

            Rectangle animationBounds = GetImageBounds(library, LoopBaseIndex, AngleBaseIndex + AngleCount - 1);
            _animationAreaSize = animationBounds.Size;
            _animationAnchor = new Point(-animationBounds.Left, -animationBounds.Top);
            _progressSize = library?.GetSize(ProgressOutlineIndex) ?? Size.Empty;

            Size = new Size(
                Math.Max(_animationAreaSize.Width, _progressSize.Width),
                _animationAreaSize.Height + 2 + _progressSize.Height);

            _healthBarWidth = Size.Width;

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out MirLibrary interfaceLibrary))
                _healthBarWidth = interfaceLibrary.GetSize(80).Width;

            LassoAnimation = new DXAnimatedControl
            {
                Parent = this,
                BaseIndex = LoopBaseIndex,
                FrameCount = AngleCount,
                AnimationDelay = LoopDuration,
                LibraryFile = LibraryFile.GameInter,
                Location = _animationAnchor,
                Loop = true,
                Animated = false,
                Visible = false,
                PixelDetect = true,
                UseOffSet = true,
            };
            LassoAnimation.MouseClick += LassoAnimation_MouseClick;

            AngleImage = new DXImageControl
            {
                Parent = this,
                Index = AngleBaseIndex,
                LibraryFile = LibraryFile.GameInter,
                Location = _animationAnchor,
                IsControl = false,
                Visible = false,
                UseOffSet = true,
            };

            Point progressLocation = new Point(
                (Size.Width - _progressSize.Width) / 2,
                _animationAreaSize.Height + 2);

            ProgressContainer = new DXControl
            {
                Parent = this,
                BackColour = Color.Empty,
                Border = false,
                IsControl = false,
                Location = progressLocation,
                Size = new Size(0, _progressSize.Height),
                Visible = false,
            };

            ProgressFill = new DXImageControl
            {
                Parent = ProgressContainer,
                Index = ProgressFillIndex,
                LibraryFile = LibraryFile.GameInter,
                IsControl = false,
            };

            ProgressOutline = new DXImageControl
            {
                Parent = this,
                Index = ProgressOutlineIndex,
                LibraryFile = LibraryFile.GameInter,
                IsControl = false,
                Location = progressLocation,
                Visible = false,
            };

            IsControl = false;
            _initialized = true;
        }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!_initialized)
                return;

            if (nValue)
            {
                UpdatePosition();
                StartNextRound();
            }
            else
            {
                _promptTimer?.Stop();
                _resultTimer?.Stop();
            }
        }

        public override void ResolutionChanged()
        {
            base.ResolutionChanged();

            if (Visible)
                UpdatePosition();
        }

        public void SetTarget(MonsterObject target)
        {
            if (target == null)
            {
                Reset();
                return;
            }

            if (Visible && TamingTarget?.ObjectID == target.ObjectID)
                return;

            TamingTarget = target;
            Begin(target.ObjectID);
        }

        private void Begin(uint targetObjectID)
        {
            _targetObjectID = targetObjectID;
            _progress = new Random(unchecked((int)targetObjectID)).Next(MaximumInitialProgress + 1);
            _completed = false;

            bool wasVisible = Visible;

            IsControl = true;
            LassoAnimation.Visible = true;
            ProgressContainer.Visible = true;
            ProgressOutline.Visible = true;

            UpdateProgress();

            if (wasVisible)
                StartNextRound();
            else
                Visible = true;
        }

        private void StartNextRound()
        {
            _promptTimer.Stop();
            _resultTimer.Stop();

            _promptVisible = false;

            AngleImage.Visible = false;

            LassoAnimation.AnimationStart = CEnvir.Now;
            LassoAnimation.Animated = true;

            _promptTimer.Interval = CEnvir.Random.Next(1000, 5001);
            _promptTimer.Start();
        }

        private void PromptTimer_Tick(object sender, EventArgs e)
        {
            _promptTimer.Stop();

            if (!Visible || _completed)
                return;

            ShowPrompt();
        }

        private void ResultTimer_Tick(object sender, EventArgs e)
        {
            _resultTimer.Stop();

            if (!Visible || _completed)
                return;

            StartNextRound();
        }

        private void ShowPrompt()
        {
            _targetAngle = CEnvir.Random.Next(AngleCount);
            _promptVisible = true;

            AngleImage.Index = AngleBaseIndex + _targetAngle;
            AngleImage.Visible = true;
        }

        private void LassoAnimation_MouseClick(object sender, MouseEventArgs e)
        {
            if (!_promptVisible || _completed || _resultTimer.Enabled)
                return;

            _promptTimer.Stop();

            int clickedAngle = Math.Min(AngleCount - 1, Math.Max(0, LassoAnimation.Index - LoopBaseIndex));
            int change = CEnvir.Random.Next(10, 21);

            if (clickedAngle == _targetAngle)
                _progress = Math.Min(100, _progress + change);
            else
                _progress = Math.Max(0, _progress - change);

            UpdateProgress();

            _promptVisible = false;
            AngleImage.Visible = false;

            LassoAnimation.Animated = false;
            LassoAnimation.Index = ResultBaseIndex + clickedAngle;

            if (_progress < 100)
            {
                _resultTimer.Start();
                return;
            }

            _completed = true;
            CompleteTaming(_targetObjectID);
        }

        private void CompleteTaming(uint objectID)
        {
            CEnvir.Enqueue(new C.TamingSuccess
            {
                ObjectID = objectID,
            });
        }

        private void UpdateProgress()
        {
            int width = (int)(_progressSize.Width * (_progress / 100F));
            ProgressContainer.Size = new Size(width, _progressSize.Height);
        }

        private void UpdatePosition()
        {
            if (MapObject.User == null)
                return;

            Location = new Point(
                MapObject.User.DrawX + (_healthBarWidth - Size.Width) / 2,
                MapObject.User.DrawY - 61 - Size.Height);
        }

        private void Reset()
        {
            if (_targetObjectID == 0 && TamingTarget == null && !Visible)
                return;

            _targetObjectID = 0;
            TamingTarget = null;
            _progress = 0;
            _targetAngle = 0;
            _promptVisible = false;
            _completed = false;

            _promptTimer.Stop();
            _resultTimer.Stop();

            IsControl = false;

            LassoAnimation.Animated = false;
            LassoAnimation.Visible = false;
            AngleImage.Visible = false;
            ProgressContainer.Visible = false;
            ProgressOutline.Visible = false;

            UpdateProgress();
            Visible = false;
        }

        private static Rectangle GetImageBounds(MirLibrary library, int startIndex, int endIndex)
        {
            if (library == null)
                return Rectangle.Empty;

            Rectangle bounds = Rectangle.Empty;

            for (int index = startIndex; index <= endIndex; index++)
            {
                Size imageSize = library.GetSize(index);

                if (imageSize.IsEmpty)
                    continue;

                Rectangle imageBounds = new Rectangle(library.GetOffSet(index), imageSize);
                bounds = bounds.IsEmpty ? imageBounds : Rectangle.Union(bounds, imageBounds);
            }

            return bounds;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            _targetObjectID = 0;
            _progress = 0;
            _targetAngle = 0;
            _promptVisible = false;
            _completed = false;
            _animationAreaSize = Size.Empty;
            _progressSize = Size.Empty;
            _animationAnchor = Point.Empty;
            _healthBarWidth = 0;
            _initialized = false;
            TamingTarget = null;

            if (_promptTimer != null)
            {
                _promptTimer.Stop();
                _promptTimer.Tick -= PromptTimer_Tick;
                _promptTimer.Dispose();
                _promptTimer = null;
            }

            if (_resultTimer != null)
            {
                _resultTimer.Stop();
                _resultTimer.Tick -= ResultTimer_Tick;
                _resultTimer.Dispose();
                _resultTimer = null;
            }

            if (LassoAnimation != null)
            {
                LassoAnimation.MouseClick -= LassoAnimation_MouseClick;

                if (!LassoAnimation.IsDisposed)
                    LassoAnimation.Dispose();

                LassoAnimation = null;
            }

            if (AngleImage != null)
            {
                if (!AngleImage.IsDisposed)
                    AngleImage.Dispose();

                AngleImage = null;
            }

            if (ProgressFill != null)
            {
                if (!ProgressFill.IsDisposed)
                    ProgressFill.Dispose();

                ProgressFill = null;
            }

            if (ProgressContainer != null)
            {
                if (!ProgressContainer.IsDisposed)
                    ProgressContainer.Dispose();

                ProgressContainer = null;
            }

            if (ProgressOutline != null)
            {
                if (!ProgressOutline.IsDisposed)
                    ProgressOutline.Dispose();

                ProgressOutline = null;
            }
        }
    }
}
