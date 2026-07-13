using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    public sealed class NPCSocketCombineDialog : DXImageControl
    {
        private readonly struct SocketCombineFrame
        {
            public Point Gem1 { get; }
            public Point Gem2 { get; }
            public Point Gem3 { get; }

            public SocketCombineFrame(Point gem1, Point gem2, Point gem3)
            {
                Gem1 = gem1;
                Gem2 = gem2;
                Gem3 = gem3;
            }
        }

        private static readonly SocketCombineFrame[] CombineFrames =
        {
            new SocketCombineFrame(new Point(7, -50), new Point(-44, 26), new Point(57, 26)),
            new SocketCombineFrame(new Point(-5, -49), new Point(-37, 36), new Point(62, 16)),
            new SocketCombineFrame(new Point(-14, -45), new Point(-29, 42), new Point(63, 7)),
            new SocketCombineFrame(new Point(-19, -39), new Point(-20, 45), new Point(61, -2)),
            new SocketCombineFrame(new Point(-23, -31), new Point(-10, 44), new Point(57, -8)),
            new SocketCombineFrame(new Point(-24, -24), new Point(-2, 43), new Point(51, -13)),
            new SocketCombineFrame(new Point(-24, -16), new Point(4, 40), new Point(45, -16)),
            new SocketCombineFrame(new Point(-21, -10), new Point(9, 35), new Point(37, -16)),
            new SocketCombineFrame(new Point(-18, -6), new Point(12, 31), new Point(32, -15)),
            new SocketCombineFrame(new Point(-12, -1), new Point(15, 25), new Point(27, -13)),
            new SocketCombineFrame(new Point(-8, 2), new Point(16, 21), new Point(21, -10)),
            new SocketCombineFrame(new Point(-3, 4), new Point(15, 15), new Point(17, -6)),
            new SocketCombineFrame(new Point(13, -2), new Point(6, 5), new Point(13, 10)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
            new SocketCombineFrame(new Point(10, 5), new Point(10, 5), new Point(10, 5)),
        };

        private static readonly Point InitialGem1Location = new Point(7, -50);
        private static readonly Point InitialGem2Location = new Point(-44, 26);
        private static readonly Point InitialGem3Location = new Point(57, 26);

        public DXLabel TitleLabel;
        public DXButton StartButton, CloseButton, CloseDialogButton;

        public DXItemGrid GemGrid1, GemGrid2, GemGrid3, ResultGrid;
        public DXItemCell GemCell1, GemCell2, GemCell3, ResultCell;

        public DXAnimatedControl CombineAnimation, CombineOverlayAnimation, ResultAnimation;
        public DXAnimatedControl GemLoopAnimation1, GemLoopAnimation2, GemLoopAnimation3;

        private S.NPCSocketCombine _pendingResult;
        private bool _operating, _combineAnimationFinished, _resultAnimationStarted;

        public NPCSocketCombineDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 5701;
            Movable = true;
            Sort = true;
            DropShadow = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft,
            };
            CloseButton.Location = new Point(Size.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.SocketCombineDialogTitle,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Constants.PrimaryColour,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            CombineAnimation = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(70, 95),
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5740,
                FrameCount = 21,
                AnimationDelay = TimeSpan.FromSeconds(2),
                UseOffSet = true,
                Loop = false,
                Animated = false,
                IsControl = false,
                CacheInParent = false,
            };
            CombineAnimation.IndexChanged += CombineAnimation_IndexChanged;
            CombineAnimation.AfterAnimation += CombineAnimation_AfterAnimation;

            GemGrid1 = new DXItemGrid
            {
                Parent = this,
                Location = new Point(CombineAnimation.Location.X + InitialGem1Location.X, CombineAnimation.Location.Y + InitialGem1Location.Y),
                GridSize = new Size(1, 1),
                GridType = GridType.SocketCombine1,
                Linked = true,
                AllowLink = true,
                BackColour = Color.Empty,
                Border = false,
            };
            GemCell1 = GemGrid1.Grid[0];
            GemCell1.FixedBorder = true;
            GemCell1.ShowCountLabel = false;
            GemCell1.LinkChanged += GemCell_LinkChanged;

            GemGrid2 = new DXItemGrid
            {
                Parent = this,
                Location = new Point(CombineAnimation.Location.X + InitialGem2Location.X, CombineAnimation.Location.Y + InitialGem2Location.Y),
                GridSize = new Size(1, 1),
                GridType = GridType.SocketCombine2,
                Linked = true,
                AllowLink = true,
                BackColour = Color.Empty,
                Border = false,
            };
            GemCell2 = GemGrid2.Grid[0];
            GemCell2.FixedBorder = true;
            GemCell2.ShowCountLabel = false;
            GemCell2.LinkChanged += GemCell_LinkChanged;

            GemGrid3 = new DXItemGrid
            {
                Parent = this,
                Location = new Point(CombineAnimation.Location.X + InitialGem3Location.X, CombineAnimation.Location.Y + InitialGem3Location.Y),
                GridSize = new Size(1, 1),
                GridType = GridType.SocketCombine3,
                Linked = true,
                AllowLink = true,
                BackColour = Color.Empty,
                Border = false,
            };
            GemCell3 = GemGrid3.Grid[0];
            GemCell3.FixedBorder = true;
            GemCell3.ShowCountLabel = false;
            GemCell3.LinkChanged += GemCell_LinkChanged;

            ResultGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(77, 213),
                GridSize = new Size(1, 1),
                GridType = GridType.SocketCombineResult,
                Linked = true,
                AllowLink = false,
                BackColour = Color.Empty,
                Border = false,
            };
            ResultCell = ResultGrid.Grid[0];
            ResultCell.FixedBorder = true;
            ResultCell.ShowCountLabel = false;

            GemLoopAnimation1 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(GemGrid1.Location.X - 6, GemGrid1.Location.Y + 3),
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5800,
                FrameCount = 50,
                AnimationDelay = TimeSpan.FromSeconds(5),
                Opacity = 0.1F,
                UseOffSet = true,
                Blend = true,
                Loop = true,
                Animated = false,
                Visible = false,
                IsControl = false,
                CacheInParent = false,
            };

            GemLoopAnimation2 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(GemGrid2.Location.X - 6, GemGrid2.Location.Y + 3),
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5800,
                FrameCount = 50,
                AnimationDelay = TimeSpan.FromSeconds(5),
                Opacity = 0.1F,
                UseOffSet = true,
                Blend = true,
                Loop = true,
                Animated = false,
                Visible = false,
                IsControl = false,
                CacheInParent = false,
            };

            GemLoopAnimation3 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(GemGrid3.Location.X - 6, GemGrid3.Location.Y + 3),
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5800,
                FrameCount = 50,
                AnimationDelay = TimeSpan.FromSeconds(5),
                Opacity = 0.1F,
                UseOffSet = true,
                Blend = true,
                Loop = true,
                Animated = false,
                Visible = false,
                IsControl = false,
                CacheInParent = false,
            };

            CombineOverlayAnimation = new DXAnimatedControl
            {
                Parent = this,
                Location = CombineAnimation.Location,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5710,
                FrameCount = 21,
                AnimationDelay = CombineAnimation.AnimationDelay,
                UseOffSet = true,
                Blend = true,
                Loop = false,
                Animated = false,
                Visible = false,
                IsControl = false,
                CacheInParent = false,
            };
            CombineOverlayAnimation.AfterAnimation += CombineOverlayAnimation_AfterAnimation;

            ResultAnimation = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(70, 215),
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5770,
                FrameCount = 10,
                AnimationDelay = TimeSpan.FromSeconds(1),
                UseOffSet = true,
                Blend = true,
                Loop = false,
                Animated = false,
                Visible = false,
                IsControl = false,
                CacheInParent = false,
            };
            ResultAnimation.AfterAnimation += ResultAnimation_AfterAnimation;

            StartButton = new DXButton
            {
                Parent = this,
                Location = new Point((Size.Width - 140) / 3, 285),
                Size = new Size(70, 24),
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Label = { Text = CEnvir.Language.SocketCombineDialogStartButtonLabel },
            };
            StartButton.MouseClick += (o, e) => Start();

            CloseDialogButton = new DXButton
            {
                Parent = this,
                Location = new Point(Size.Width - (Size.Width - 140) / 3 - 70, 285),
                Size = new Size(70, 24),
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Label = { Text = CEnvir.Language.CommonControlClose },
            };
            CloseDialogButton.MouseClick += (o, e) => Visible = false;
        }

        public DXItemCell GetNextInputCell()
        {
            if (GemCell1.Item == null) return GemCell1;
            if (GemCell2.Item == null) return GemCell2;
            if (GemCell3.Item == null) return GemCell3;

            return null;
        }

        public ItemInfo GetInputInfo()
        {
            return GemCell1.Item?.Info ?? GemCell2.Item?.Info ?? GemCell3.Item?.Info;
        }

        private void Start()
        {
            if (_operating) return;

            ClientUserItem gem1 = GemCell1.Item;
            ClientUserItem gem2 = GemCell2.Item;
            ClientUserItem gem3 = GemCell3.Item;

            if (gem1 == null || gem2 == null || gem3 == null)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.UnableToSocketMissingGems, MessageType.System);
                return;
            }

            if (gem1.Info.ItemType != ItemType.SocketGem || gem2.Info != gem1.Info || gem3.Info != gem1.Info)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.UnableToSocketMismatch, MessageType.System);
                return;
            }

            _operating = true;
            _combineAnimationFinished = false;
            _resultAnimationStarted = false;
            _pendingResult = null;

            StartButton.Enabled = false;
            GemCell1.AllowLink = GemCell2.AllowLink = GemCell3.AllowLink = false;

            CombineAnimation.BaseIndex = 5740;
            CombineAnimation.FrameCount = CombineFrames.Length;
            CombineAnimation.AnimationStart = DateTime.MinValue;
            CombineAnimation.Animated = true;

            CombineOverlayAnimation.AnimationStart = DateTime.MinValue;
            CombineOverlayAnimation.Visible = true;
            CombineOverlayAnimation.Animated = true;

            DXSoundManager.Play(SoundIndex.GemCombine);

            CEnvir.Enqueue(new C.NPCSocketCombine
            {
                Gem1 = GetLink(GemCell1),
                Gem2 = GetLink(GemCell2),
                Gem3 = GetLink(GemCell3),
            });
        }

        private static CellLinkInfo GetLink(DXItemCell cell)
        {
            return new CellLinkInfo
            {
                GridType = cell.Link.GridType,
                Slot = cell.Link.Slot,
                Count = 1,
            };
        }

        public void ProcessResult(S.NPCSocketCombine result)
        {
            if (!result.Accepted)
            {
                CancelOperation(result.Message);
                return;
            }

            _pendingResult = result;
            TryStartResultAnimation();
        }

        private void CombineAnimation_IndexChanged(object sender, EventArgs e)
        {
            if (!_operating) return;

            int frame = CombineAnimation.Index - 5740;
            if (frame < 0 || frame >= CombineFrames.Length) return;

            ApplyFrame(CombineFrames[frame]);
        }

        private void CombineAnimation_AfterAnimation(object sender, EventArgs e)
        {
            CombineAnimation.Animated = false;
            CombineAnimation.BaseIndex = 5740;
            CombineAnimation.FrameCount = 1;
            CombineAnimation.Index = 5740;
            ApplyInitialLocations();

            _combineAnimationFinished = true;
            TryStartResultAnimation();
        }

        private void CombineOverlayAnimation_AfterAnimation(object sender, EventArgs e)
        {
            CombineOverlayAnimation.Animated = false;
            CombineOverlayAnimation.Visible = false;
        }

        private void TryStartResultAnimation()
        {
            if (!_combineAnimationFinished || _pendingResult == null || _resultAnimationStarted) return;

            _resultAnimationStarted = true;
            ApplyResult(_pendingResult);

            if (!_pendingResult.Success)
            {
                string failureMessage = _pendingResult.Message;
                FinishOperation();

                if (!string.IsNullOrEmpty(failureMessage))
                    GameScene.Game.ReceiveChat(failureMessage, MessageType.System);
                return;
            }

            if (!Visible)
            {
                string message = _pendingResult.Message;
                FinishOperation();

                if (!string.IsNullOrEmpty(message))
                    GameScene.Game.ReceiveChat(message, MessageType.System);
                return;
            }

            ResultAnimation.AnimationStart = DateTime.MinValue;
            ResultAnimation.Visible = true;
            ResultAnimation.Animated = true;
        }

        private void ResultAnimation_AfterAnimation(object sender, EventArgs e)
        {
            ResultAnimation.Animated = false;
            ResultAnimation.Visible = false;

            string message = _pendingResult?.Message;
            FinishOperation();

            if (!string.IsNullOrEmpty(message))
                GameScene.Game.ReceiveChat(message, MessageType.System);
        }

        private void ApplyResult(S.NPCSocketCombine result)
        {
            if (result.Success)
            {
                ClearInputLinks();
            }
            else
            {
                HashSet<int> consumedSlots = new HashSet<int>(result.ClearedSlots ?? Enumerable.Empty<int>());

                ClearConsumedInputLink(GemCell1, consumedSlots);
                ClearConsumedInputLink(GemCell2, consumedSlots);
                ClearConsumedInputLink(GemCell3, consumedSlots);
            }

            foreach (int slot in result.ClearedSlots ?? Enumerable.Empty<int>())
            {
                if (slot < 0 || slot >= GameScene.Game.Inventory.Length) continue;

                GameScene.Game.Inventory[slot] = null;
                GameScene.Game.InventoryBox.Grid.Grid[slot].Item = null;
            }

            foreach (ClientUserItem item in result.Items ?? Enumerable.Empty<ClientUserItem>())
            {
                if (item.Slot < 0 || item.Slot >= GameScene.Game.Inventory.Length) continue;

                GameScene.Game.Inventory[item.Slot] = item;
                GameScene.Game.InventoryBox.Grid.Grid[item.Slot].Item = item;
            }

            if (result.ResultSlot >= 0 && result.ResultSlot < GameScene.Game.Inventory.Length)
            {
                DXItemCell inventoryCell = GameScene.Game.InventoryBox.Grid.Grid[result.ResultSlot];
                ResultCell.LinkedCount = 1;
                ResultCell.Link = inventoryCell;
            }
        }

        private static void ClearConsumedInputLink(DXItemCell cell, HashSet<int> consumedSlots)
        {
            if (cell.Link?.GridType != GridType.Inventory || !consumedSlots.Contains(cell.Link.Slot)) return;

            ClearLink(cell);
        }

        private void CancelOperation(string message)
        {
            CombineOverlayAnimation.Animated = false;
            CombineOverlayAnimation.Visible = false;

            CombineAnimation.Animated = false;
            CombineAnimation.BaseIndex = 5740;
            CombineAnimation.FrameCount = 1;
            CombineAnimation.Index = 5740;
            ApplyInitialLocations();

            _operating = false;
            _combineAnimationFinished = false;
            _resultAnimationStarted = false;
            _pendingResult = null;

            StartButton.Enabled = true;
            GemCell1.AllowLink = GemCell2.AllowLink = GemCell3.AllowLink = true;

            if (!string.IsNullOrEmpty(message))
                GameScene.Game.ReceiveChat(message, MessageType.System);
        }

        private void FinishOperation()
        {
            CombineOverlayAnimation.Animated = false;
            CombineOverlayAnimation.Visible = false;

            CombineAnimation.BaseIndex = 5740;
            CombineAnimation.FrameCount = 1;
            CombineAnimation.Index = 5740;
            ApplyInitialLocations();

            _operating = false;
            _combineAnimationFinished = false;
            _resultAnimationStarted = false;
            _pendingResult = null;

            StartButton.Enabled = true;
            GemCell1.AllowLink = GemCell2.AllowLink = GemCell3.AllowLink = true;

            if (!Visible)
                ClearAllLinks();
        }

        private void ApplyFrame(SocketCombineFrame frame)
        {
            GemGrid1.Location = new Point(CombineAnimation.Location.X + frame.Gem1.X, CombineAnimation.Location.Y + frame.Gem1.Y);
            GemGrid2.Location = new Point(CombineAnimation.Location.X + frame.Gem2.X, CombineAnimation.Location.Y + frame.Gem2.Y);
            GemGrid3.Location = new Point(CombineAnimation.Location.X + frame.Gem3.X, CombineAnimation.Location.Y + frame.Gem3.Y);

            GemLoopAnimation1.Location = new Point(GemGrid1.Location.X - 6, GemGrid1.Location.Y + 3);
            GemLoopAnimation2.Location = new Point(GemGrid2.Location.X - 6, GemGrid2.Location.Y + 3);
            GemLoopAnimation3.Location = new Point(GemGrid3.Location.X - 6, GemGrid3.Location.Y + 3);
        }

        private void ApplyInitialLocations()
        {
            ApplyFrame(new SocketCombineFrame(InitialGem1Location, InitialGem2Location, InitialGem3Location));
        }

        private void GemCell_LinkChanged(object sender, EventArgs e)
        {
            SetLoopAnimation(GemLoopAnimation1, GemCell1.Item?.Info);
            SetLoopAnimation(GemLoopAnimation2, GemCell2.Item?.Info);
            SetLoopAnimation(GemLoopAnimation3, GemCell3.Item?.Info);
        }

        private static void SetLoopAnimation(DXAnimatedControl animation, ItemInfo info)
        {
            int index = info?.Shape switch
            {
                1 => 5900,
                2 => 6000,
                4 => 5800,
                _ => 0,
            };

            if (index == 0)
            {
                animation.Animated = false;
                animation.Visible = false;
                return;
            }

            bool restart = animation.BaseIndex != index || !animation.Visible;
            animation.BaseIndex = index;
            animation.Visible = true;
            animation.Animated = true;

            if (restart)
                animation.AnimationStart = CEnvir.Now;
        }

        private void ClearInputLinks()
        {
            ClearLink(GemCell1);
            ClearLink(GemCell2);
            ClearLink(GemCell3);
        }

        private void ClearAllLinks()
        {
            ClearInputLinks();
            ClearLink(ResultCell);
        }

        private static void ClearLink(DXItemCell cell)
        {
            if (cell.Link != null)
                cell.Link.Locked = false;

            cell.Link = null;
            cell.LinkedCount = 0;
            cell.RefreshItem();
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (Visible)
            {
                GameScene.Game.InventoryBox.Visible = true;
                return;
            }

            if (!_operating)
            {
                ClearAllLinks();
                return;
            }

            _combineAnimationFinished = true;
            TryStartResultAnimation();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            _pendingResult = null;

            if (GemCell1 != null)
            {
                GemCell1.LinkChanged -= GemCell_LinkChanged;

                if (!GemCell1.IsDisposed)
                    GemCell1.Dispose();

                GemCell1 = null;
            }

            if (GemCell2 != null)
            {
                GemCell2.LinkChanged -= GemCell_LinkChanged;

                if (!GemCell2.IsDisposed)
                    GemCell2.Dispose();

                GemCell2 = null;
            }

            if (GemCell3 != null)
            {
                GemCell3.LinkChanged -= GemCell_LinkChanged;

                if (!GemCell3.IsDisposed)
                    GemCell3.Dispose();

                GemCell3 = null;
            }

            if (ResultCell != null)
            {
                if (!ResultCell.IsDisposed)
                    ResultCell.Dispose();

                ResultCell = null;
            }

            if (GemGrid1 != null)
            {
                if (!GemGrid1.IsDisposed)
                    GemGrid1.Dispose();

                GemGrid1 = null;
            }

            if (GemGrid2 != null)
            {
                if (!GemGrid2.IsDisposed)
                    GemGrid2.Dispose();

                GemGrid2 = null;
            }

            if (GemGrid3 != null)
            {
                if (!GemGrid3.IsDisposed)
                    GemGrid3.Dispose();

                GemGrid3 = null;
            }

            if (ResultGrid != null)
            {
                if (!ResultGrid.IsDisposed)
                    ResultGrid.Dispose();

                ResultGrid = null;
            }

            if (CombineAnimation != null)
            {
                CombineAnimation.IndexChanged -= CombineAnimation_IndexChanged;
                CombineAnimation.AfterAnimation -= CombineAnimation_AfterAnimation;

                if (!CombineAnimation.IsDisposed)
                    CombineAnimation.Dispose();

                CombineAnimation = null;
            }

            if (CombineOverlayAnimation != null)
            {
                CombineOverlayAnimation.AfterAnimation -= CombineOverlayAnimation_AfterAnimation;

                if (!CombineOverlayAnimation.IsDisposed)
                    CombineOverlayAnimation.Dispose();

                CombineOverlayAnimation = null;
            }

            if (ResultAnimation != null)
            {
                ResultAnimation.AfterAnimation -= ResultAnimation_AfterAnimation;

                if (!ResultAnimation.IsDisposed)
                    ResultAnimation.Dispose();

                ResultAnimation = null;
            }

            if (GemLoopAnimation1 != null)
            {
                if (!GemLoopAnimation1.IsDisposed)
                    GemLoopAnimation1.Dispose();

                GemLoopAnimation1 = null;
            }

            if (GemLoopAnimation2 != null)
            {
                if (!GemLoopAnimation2.IsDisposed)
                    GemLoopAnimation2.Dispose();

                GemLoopAnimation2 = null;
            }

            if (GemLoopAnimation3 != null)
            {
                if (!GemLoopAnimation3.IsDisposed)
                    GemLoopAnimation3.Dispose();

                GemLoopAnimation3 = null;
            }

            if (StartButton != null)
            {
                if (!StartButton.IsDisposed)
                    StartButton.Dispose();

                StartButton = null;
            }

            if (CloseButton != null)
            {
                if (!CloseButton.IsDisposed)
                    CloseButton.Dispose();

                CloseButton = null;
            }

            if (CloseDialogButton != null)
            {
                if (!CloseDialogButton.IsDisposed)
                    CloseDialogButton.Dispose();

                CloseDialogButton = null;
            }

            if (TitleLabel != null)
            {
                if (!TitleLabel.IsDisposed)
                    TitleLabel.Dispose();

                TitleLabel = null;
            }

        }
    }
}
