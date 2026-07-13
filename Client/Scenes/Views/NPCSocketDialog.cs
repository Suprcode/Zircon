using Client.Controls;
using Client.Envir;
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
    public sealed class NPCSocketDialog : DXImageControl
    {
        private const int SocketCount = 3;

        public DXLabel TitleLabel;

        public DXItemGrid TargetGrid, GemGrid;
        public DXItemCell TargetCell, GemCell;
        public DXItemCell SocketCell1, SocketCell2, SocketCell3;

        public DXAnimatedControl GemLoopAnimation;
        public DXAnimatedControl SocketLoopAnimation1, SocketLoopAnimation2, SocketLoopAnimation3;
        public DXAnimatedControl SocketingAnimation1, SocketingAnimation2, SocketingAnimation3;

        public DXButton StartButton, CloseButton, CloseDialogButton;

        private S.NPCSocketItem _pendingResult;
        private int _pendingAnimationCount;
        private DXItemCell _targetLink;
        private DXItemCell _gemLink;

        public NPCSocketDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 5700;
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
                Text = CEnvir.Language.SocketDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Constants.PrimaryColour,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);
            TargetGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(8, 38),
                GridSize = new Size(1, 1),
                GridType = GridType.SocketTarget,
                Linked = true,
                AllowLink = true,
                BackColour = Color.Empty,
                Border = false,
            };
            TargetGrid.Size = new Size(170, 150);
            TargetCell = TargetGrid.Grid[0];
            TargetCell.Size = TargetGrid.Size;
            TargetCell.Hidden = true;
            TargetCell.ShowCountLabel = false;
            TargetCell.LinkChanged += LinkedCell_LinkChanged;

            GemGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(136, 45),
                GridSize = new Size(1, 1),
                GridType = GridType.SocketGem,
                Linked = true,
                AllowLink = true,
                BackColour = Color.Empty,
                Border = false,
            };
            GemCell = GemGrid.Grid[0];
            GemCell.ShowCountLabel = false;
            GemCell.FixedBorder = true;
            GemCell.LinkChanged += LinkedCell_LinkChanged;

            BeforeChildrenDraw += DrawTargetItem;

            SocketCell1 = new DXItemCell
            {
                Parent = this,
                Location = new Point(21, 211),
                AllowLink = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = false
            };
            SocketCell2 = new DXItemCell
            {
                Parent = this,
                Location = new Point(74, 211),
                AllowLink = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = false
            };
            SocketCell3 = new DXItemCell
            {
                Parent = this,
                Location = new Point(128, 211),
                AllowLink = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = false
            };

            SocketLoopAnimation1 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(15, 214),
                IsControl = false,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5800,
                FrameCount = 50,
                Blend = true,
                AnimationDelay = TimeSpan.FromSeconds(5),
                Opacity = 0.1F,
                UseOffSet = true,
                Loop = true,
                Animated = true,
                Visible = false,
                CacheInParent = false,
            };
            SocketLoopAnimation2 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(68, 214),
                IsControl = false,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5800,
                FrameCount = 50,
                Blend = true,
                AnimationDelay = TimeSpan.FromSeconds(5),
                Opacity = 0.1F,
                UseOffSet = true,
                Loop = true,
                Animated = true,
                Visible = false,
                CacheInParent = false,
            };
            SocketLoopAnimation3 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(122, 214),
                IsControl = false,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5800,
                FrameCount = 50,
                Blend = true,
                AnimationDelay = TimeSpan.FromSeconds(5),
                Opacity = 0.1F,
                UseOffSet = true,
                Loop = true,
                Animated = true,
                Visible = false,
                CacheInParent = false,
            };

            GemLoopAnimation = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(130, 48),
                IsControl = false,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5800,
                FrameCount = 50,
                Blend = true,
                AnimationDelay = TimeSpan.FromSeconds(5),
                Opacity = 0.1F,
                UseOffSet = true,
                Loop = true,
                Animated = true,
                Visible = false,
                CacheInParent = false,
            };

            SocketingAnimation1 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(15, 214),
                IsControl = false,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5770,
                FrameCount = 9,
                Blend = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                UseOffSet = true,
                Loop = false,
                Animated = false,
                Visible = false,
                CacheInParent = false,
            };
            SocketingAnimation1.AfterAnimation += SocketingAnimation_AfterAnimation;

            SocketingAnimation2 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(68, 214),
                IsControl = false,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5770,
                FrameCount = 9,
                Blend = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                UseOffSet = true,
                Loop = false,
                Animated = false,
                Visible = false,
                CacheInParent = false,
            };
            SocketingAnimation2.AfterAnimation += SocketingAnimation_AfterAnimation;

            SocketingAnimation3 = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(122, 214),
                IsControl = false,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 5770,
                FrameCount = 9,
                Blend = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                UseOffSet = true,
                Loop = false,
                Animated = false,
                Visible = false,
                CacheInParent = false,
            };
            SocketingAnimation3.AfterAnimation += SocketingAnimation_AfterAnimation;

            StartButton = new DXButton
            {
                Parent = this,
                Location = new Point((Size.Width - 140) / 3, 280),
                Size = new Size(70, 24),
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Label = { Text = CEnvir.Language.SocketDialogStartButtonLabel },
            };
            StartButton.MouseClick += (o, e) => Start();

            CloseDialogButton = new DXButton
            {
                Parent = this,
                Location = new Point(Size.Width - (Size.Width - 140) / 3 - 70, 280),
                Size = new Size(70, 24),
                ButtonType = ButtonType.Default,
                LabelStyle = ButtonLabelStyle.Gold,
                Label = { Text = CEnvir.Language.CommonControlClose },
            };
            CloseDialogButton.MouseClick += (o, e) => Visible = false;
        }

        private void SocketingAnimation_AfterAnimation(object sender, EventArgs e)
        {
            DXAnimatedControl animation = (DXAnimatedControl)sender;
            animation.Animated = false;
            animation.Visible = false;

            if (_pendingAnimationCount > 0)
                _pendingAnimationCount--;

            if (_pendingAnimationCount != 0) return;

            CompleteOperation();
        }

        private void DrawTargetItem(object sender, EventArgs e)
        {
            ClientUserItem item = TargetCell?.Item;
            if (item == null || !CEnvir.LibraryList.TryGetValue(LibraryFile.Inventory, out MirLibrary library)) return;

            int imageIndex = item.Info.Image;
            Size imageSize = library.GetSize(imageIndex);
            Point location = new Point(
                TargetCell.DisplayArea.X + (TargetCell.Size.Width - imageSize.Width) / 2,
                TargetCell.DisplayArea.Y + (TargetCell.Size.Height - imageSize.Height) / 2);

            library.Draw(imageIndex, location.X, location.Y, Color.White, false, 1F, ImageType.Image);
            if (item.Colour != Color.Empty)
                library.Draw(imageIndex, location.X, location.Y, item.Colour, false, 1F, ImageType.Overlay);
        }

        private void LinkedCell_LinkChanged(object sender, EventArgs e)
        {
            if (sender == TargetCell)
            {
                if (_targetLink != null && _targetLink != TargetCell.Link)
                    GemCell.Link = null;

                _targetLink = TargetCell.Link;
            }
            else if (sender == GemCell)
            {
                if (_gemLink != null)
                    _gemLink.ItemChanged -= GemLink_ItemChanged;

                _gemLink = GemCell.Link;

                if (_gemLink != null)
                    _gemLink.ItemChanged += GemLink_ItemChanged;
            }

            RefreshSockets();
        }

        private void GemLink_ItemChanged(object sender, EventArgs e)
        {
            SetLoopAnimation(GemLoopAnimation, GemCell.Item?.Info);
        }

        public bool CanUseGem(ClientUserItem gem)
        {
            if (gem?.Info == null || gem.Info.ItemType != ItemType.SocketGem)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.UnableToSocketInvalidGem, MessageType.System);
                return false;
            }

            ClientUserItem target = TargetCell.Item;
            if (target == null)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToSocketNoTarget, gem.Info.ItemName), MessageType.System);
                return false;
            }

            if (target.Info.ItemType != ItemType.Weapon && target.Info.ItemType != ItemType.Armour)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToSocketIncorrectTarget, gem.Info.ItemName), MessageType.System);
                return false;
            }

            int shape = gem.Info.Shape;
            if (shape != 0 && shape != 1 && shape != 2 && shape != 4)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.UnableToSocketInvalidGem, MessageType.System);
                return false;
            }

            if ((shape == 0 || shape == 1 || shape == 2) && gem.Info.Rarity != target.Info.Rarity)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToSocketIncorrectRarity, gem.Info.ItemName), MessageType.System);
                return false;
            }

            if (shape == 0 && target.Sockets.Count >= SocketCount)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToSocketNoMoreSockets, gem.Info.ItemName), MessageType.System);
                return false;
            }

            if ((shape == 1 && target.Info.ItemType != ItemType.Weapon) ||
                (shape == 2 && target.Info.ItemType != ItemType.Armour))
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToSocketIncorrectTarget, gem.Info.ItemName), MessageType.System);
                return false;
            }

            if ((shape == 1 || shape == 2) &&
                target.Sockets.All(x => x.Gem != null) &&
                target.Sockets.All(x => x.Gem?.InfoIndex != gem.Info.Index))
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.UnableToSocketNoAvailableSocket, MessageType.System);
                return false;
            }

            return true;
        }

        private void Start()
        {
            if (!StartButton.Enabled) return;
            if (TargetCell.Item == null || GemCell.Item == null)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.UnableToSocketMissingItems, MessageType.System);
                return;
            }
            if (!CanUseGem(GemCell.Item)) return;

            StartButton.Enabled = false;
            TargetCell.AllowLink = GemCell.AllowLink = false;
            _pendingResult = null;

            DXSoundManager.Play(SoundIndex.GemStart);

            CEnvir.Enqueue(new C.NPCSocketItem
            {
                Target = new CellLinkInfo { GridType = TargetCell.Link.GridType, Slot = TargetCell.Link.Slot, Count = 1 },
                Gem = new CellLinkInfo { GridType = GemCell.Link.GridType, Slot = GemCell.Link.Slot, Count = 1 },
            });
        }

        public void ProcessResult(S.NPCSocketItem result)
        {
            if (result.Item == null)
            {
                if (!string.IsNullOrEmpty(result.Message))
                    GameScene.Game.ReceiveChat(result.Message, MessageType.System);

                EndOperation();
                return;
            }

            _pendingResult = result;
            _pendingAnimationCount = 0;

            if (!Visible)
            {
                CompleteOperation();
                return;
            }

            if (result.GemShape == 4)
            {
                IReadOnlyList<ClientUserItemSocket> currentSockets = TargetCell.Link?.Item?.Sockets;
                currentSockets ??= Array.Empty<ClientUserItemSocket>();

                if (currentSockets.FirstOrDefault(x => x.Slot == 0)?.Gem != null)
                    StartSocketingAnimation(0);
                if (currentSockets.FirstOrDefault(x => x.Slot == 1)?.Gem != null)
                    StartSocketingAnimation(1);
                if (currentSockets.FirstOrDefault(x => x.Slot == 2)?.Gem != null)
                    StartSocketingAnimation(2);
            }
            else if (result.GemShape == 0)
            {
                int slot = result.SocketSlot;
                if (slot < 0 || slot >= SocketCount)
                    slot = Math.Min(TargetCell.Link?.Item?.Sockets?.Count ?? 0, SocketCount - 1);

                StartSocketingAnimation(slot);
            }
            else if (result.SocketSlot >= 0 && result.SocketSlot < SocketCount)
            {
                StartSocketingAnimation(result.SocketSlot);
            }

            if (_pendingAnimationCount == 0)
                CompleteOperation();
        }

        private void StartSocketingAnimation(int slot)
        {
            DXAnimatedControl animation = slot switch
            {
                0 => SocketingAnimation1,
                1 => SocketingAnimation2,
                2 => SocketingAnimation3,
                _ => null,
            };

            if (animation == null) return;

            animation.AnimationStart = DateTime.MinValue;
            animation.Visible = true;
            animation.Animated = true;
            _pendingAnimationCount++;
        }

        private void CompleteOperation()
        {
            if (_pendingResult == null) return;

            if (_pendingResult.Item != null && GemCell.Link != null)
            {
                DXItemCell gemSource = GemCell.Link;
                gemSource.Locked = false;
                GemCell.Link = null;
                GemCell.LinkedCount = 0;
                gemSource.RefreshItem();
                GemCell.RefreshItem();
            }

            if (_pendingResult.Item != null && _pendingResult.GridType == GridType.Inventory && _pendingResult.Slot >= 0 && _pendingResult.Slot < GameScene.Game.Inventory.Length)
            {
                GameScene.Game.Inventory[_pendingResult.Slot] = _pendingResult.Item;
                GameScene.Game.InventoryBox.Grid.Grid[_pendingResult.Slot].Item = _pendingResult.Item;
                if (TargetCell.Link?.GridType == GridType.Inventory && TargetCell.Link.Slot == _pendingResult.Slot)
                    TargetCell.Link.Item = _pendingResult.Item;
            }

            RefreshSockets();
            if (!string.IsNullOrEmpty(_pendingResult.Message))
                GameScene.Game.ReceiveChat(_pendingResult.Message, MessageType.System);

            EndOperation();
        }

        private void EndOperation()
        {
            StartButton.Enabled = true;
            TargetCell.AllowLink = GemCell.AllowLink = true;
            _pendingResult = null;

            if (Visible) return;

            TargetCell.Link = null;
            GemCell.Link = null;
        }

        private void RefreshSockets()
        {
            ClientUserItem item = TargetCell.Link?.Item;
            IReadOnlyList<ClientUserItemSocket> sockets = item?.Sockets;
            sockets ??= Array.Empty<ClientUserItemSocket>();

            RefreshSocketCell(SocketCell1, SocketLoopAnimation1, sockets, 0);
            RefreshSocketCell(SocketCell2, SocketLoopAnimation2, sockets, 1);
            RefreshSocketCell(SocketCell3, SocketLoopAnimation3, sockets, 2);

            SetLoopAnimation(GemLoopAnimation, GemCell?.Item?.Info);
        }

        private static void RefreshSocketCell(DXItemCell cell, DXAnimatedControl animation, IReadOnlyList<ClientUserItemSocket> sockets, int slot)
        {
            ClientUserItemSocket socket = sockets.FirstOrDefault(x => x.Slot == slot);
            ClientUserItem gem = socket?.Gem;

            cell.Item = gem;
            cell.Enabled = socket != null;
            SetLoopAnimation(animation, gem?.Info);
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

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                GameScene.Game.InventoryBox.Visible = true;
                return;
            }

            if (_pendingResult != null)
            {
                StopSocketingAnimations();
                CompleteOperation();
                return;
            }

            if (!StartButton.Enabled) return;

            TargetCell.Link = null;
            GemCell.Link = null;
        }

        private void StopSocketingAnimations()
        {
            SocketingAnimation1.Animated = false;
            SocketingAnimation1.Visible = false;
            SocketingAnimation2.Animated = false;
            SocketingAnimation2.Visible = false;
            SocketingAnimation3.Animated = false;
            SocketingAnimation3.Visible = false;
            _pendingAnimationCount = 0;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            _pendingResult = null;
            _targetLink = null;

            BeforeChildrenDraw -= DrawTargetItem;

            if (_gemLink != null)
            {
                _gemLink.ItemChanged -= GemLink_ItemChanged;
                _gemLink = null;
            }

            if (TargetCell != null)
            {
                TargetCell.LinkChanged -= LinkedCell_LinkChanged;

                if (!TargetCell.IsDisposed)
                    TargetCell.Dispose();

                TargetCell = null;
            }

            if (GemCell != null)
            {
                GemCell.LinkChanged -= LinkedCell_LinkChanged;

                if (!GemCell.IsDisposed)
                    GemCell.Dispose();

                GemCell = null;
            }

            if (TargetGrid != null)
            {
                if (!TargetGrid.IsDisposed)
                    TargetGrid.Dispose();

                TargetGrid = null;
            }

            if (GemGrid != null)
            {
                if (!GemGrid.IsDisposed)
                    GemGrid.Dispose();

                GemGrid = null;
            }

            if (SocketCell1 != null)
            {
                if (!SocketCell1.IsDisposed)
                    SocketCell1.Dispose();

                SocketCell1 = null;
            }

            if (SocketCell2 != null)
            {
                if (!SocketCell2.IsDisposed)
                    SocketCell2.Dispose();

                SocketCell2 = null;
            }

            if (SocketCell3 != null)
            {
                if (!SocketCell3.IsDisposed)
                    SocketCell3.Dispose();

                SocketCell3 = null;
            }

            if (GemLoopAnimation != null)
            {
                if (!GemLoopAnimation.IsDisposed)
                    GemLoopAnimation.Dispose();

                GemLoopAnimation = null;
            }

            if (SocketLoopAnimation1 != null)
            {
                if (!SocketLoopAnimation1.IsDisposed)
                    SocketLoopAnimation1.Dispose();

                SocketLoopAnimation1 = null;
            }

            if (SocketLoopAnimation2 != null)
            {
                if (!SocketLoopAnimation2.IsDisposed)
                    SocketLoopAnimation2.Dispose();

                SocketLoopAnimation2 = null;
            }

            if (SocketLoopAnimation3 != null)
            {
                if (!SocketLoopAnimation3.IsDisposed)
                    SocketLoopAnimation3.Dispose();

                SocketLoopAnimation3 = null;
            }

            if (SocketingAnimation1 != null)
            {
                SocketingAnimation1.AfterAnimation -= SocketingAnimation_AfterAnimation;

                if (!SocketingAnimation1.IsDisposed)
                    SocketingAnimation1.Dispose();

                SocketingAnimation1 = null;
            }

            if (SocketingAnimation2 != null)
            {
                SocketingAnimation2.AfterAnimation -= SocketingAnimation_AfterAnimation;

                if (!SocketingAnimation2.IsDisposed)
                    SocketingAnimation2.Dispose();

                SocketingAnimation2 = null;
            }

            if (SocketingAnimation3 != null)
            {
                SocketingAnimation3.AfterAnimation -= SocketingAnimation_AfterAnimation;

                if (!SocketingAnimation3.IsDisposed)
                    SocketingAnimation3.Dispose();

                SocketingAnimation3 = null;
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
