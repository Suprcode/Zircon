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
    public sealed class HorseTameDialog : DXImageControl
    {
        private const int LoopBaseIndex = 7640;
        private const int ResultBaseIndex = 7650;
        private const int AngleBaseIndex = 7660;
        private const int BackgroundIndex = 285;
        private const int ProgressFillIndex = 7689;
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
        private Size _progressSize;
        private int _healthBarWidth;
        private Timer _promptTimer, _resultTimer;
        private bool _initialized;

        public DXAnimatedControl LassoAnimation;
        public DXImageControl AngleImage;
        public DXControl ProgressContainer;
        public DXImageControl ProgressFill;
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

            LibraryFile = LibraryFile.Interface;
            Index = BackgroundIndex;
            PixelDetect = false;

            // Position the GameInter sprites' shared origin inside the Interface window.
            Point animationAnchor = new Point(83, 63);
            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary gameInterLibrary);
            _progressSize = gameInterLibrary?.GetSize(ProgressFillIndex) ?? Size.Empty;
            _healthBarWidth = Library?.GetSize(80).Width ?? Size.Width;

            new DXWindowTitleLabel
            {
                Parent = this,
                Text = CEnvir.Language.HorseTameDialogTitle,
            };

            LassoAnimation = new DXAnimatedControl
            {
                Parent = this,
                BaseIndex = LoopBaseIndex,
                FrameCount = AngleCount,
                AnimationDelay = LoopDuration,
                LibraryFile = LibraryFile.GameInter,
                Location = animationAnchor,
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
                Location = animationAnchor,
                IsControl = false,
                Visible = false,
                UseOffSet = true,
            };

            ProgressContainer = new DXControl
            {
                Parent = this,
                BackColour = Color.Empty,
                Border = false,
                IsControl = false,
                Location = new Point(3, 148),
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

            UpdateProgress();
            Visible = false;
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
            _progressSize = Size.Empty;
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
        }
    }
}
