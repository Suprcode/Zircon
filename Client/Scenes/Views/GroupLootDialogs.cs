using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Shared.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class GroupBagDialog : DXImageControl
    {
        private const int Columns = 6;
        private const int VisibleRows = 6;

        public DXItemGrid Grid;
        public DXLabel TitleLabel, WeightLabel, ModeTitle, ModeLabel, FilterTitle, FilterLabel;
        public DXButton CloseButton, OptionsButton, ShareButton;
        public DXControl WeightBar;
        public ClientGroupLootInfo Info = new();
        public ClientUserItem[] DisplayItems = new ClientUserItem[Columns * VisibleRows];
        private int _scrollRow;
        private bool _syncingPairLocation;

        public GroupBagDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 246;
            Movable = true;
            Sort = true;
            DropShadow = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft
            };
            CloseButton.Location = new Point(Size.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += CloseButton_MouseClick;

            TitleLabel = new DXWindowTitleLabel
            {
                Text = CEnvir.Language.GroupLootTitle,
                Parent = this,
            };

            Grid = new DXItemGrid
            {
                GridSize = new Size(Columns, VisibleRows),
                Parent = this,
                ItemGrid = DisplayItems,
                GridType = GridType.GroupLoot,
                Location = new Point(20, 39),
                VisibleHeight = VisibleRows,
                GridPadding = 1,
                BackColour = Color.Empty,
                Border = false,
                ReadOnly = true,
                AllowLink = false,
            };
            Grid.MouseWheel += Grid_MouseWheel;
            AttachCellHandlers();

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary library);
            WeightBar = new DXControl { Parent = this, Location = new Point(53, 280), Size = library?.GetSize(360) ?? new Size(196, 8) };
            WeightBar.BeforeDraw += (o, e) =>
            {
                if (library == null || Info.Capacity <= 0 || Info.Weight <= 0) return;
                float percent = Math.Min(1F, Math.Max(0F, Info.Weight / (float)Info.Capacity));
                if (!library.TryGetTexture(360, ImageType.Image, out MirImage image, out var texture, out var sourceRectangle)) return;
                PresentTexture(texture, sourceRectangle, this, new Rectangle(WeightBar.DisplayArea.X, WeightBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, WeightBar);
            };

            WeightLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                AutoSize = false,
                Size = new Size(196, 16),
                Location = new Point(53, 276),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            ModeTitle = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.Goldenrod,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Location = new Point(12, 306),
                Text = CEnvir.Language.GroupLootMode,
                Size = new Size(97, 20),
            };
            Font modeTitleFont = ModeTitle.Font;
            ModeTitle.Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold);
            modeTitleFont.Dispose();

            ModeLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Location = new Point(73, 306),
                Size = new Size(97, 20),
            };

            FilterTitle = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.Goldenrod,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Location = new Point(12, 325),
                Text = CEnvir.Language.GroupLootFilters,
                Size = new Size(97, 20),
            };
            Font filterTitleFont = FilterTitle.Font;
            FilterTitle.Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Bold);
            filterTitleFont.Dispose();

            FilterLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Location = new Point(73, 325),
                Size = new Size(97, 20),
            };

            ShareButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 364,
                Location = new Point(218, 309),
                Hint = CEnvir.Language.GroupLootShare,
            };
            ShareButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GroupLootShare());

            OptionsButton = new DXButton
            {
                Parent = this,
                Size = new Size(36, 36),
                ButtonType = ButtonType.OptionsButton,
                Location = new Point(178, 309),
                Hint = CEnvir.Language.GroupLootSettingsHint,
            };
            OptionsButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.GroupLootEnabled || GameScene.Game.Observer) return;
                _ = new GroupLootSettingsDialog(Info);
            };

            RefreshDisplay();
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (!IsMoving || _syncingPairLocation || GameScene.Game?.GroupBox == null) return;

            Point delta = new Point(nValue.X - oValue.X, nValue.Y - oValue.Y);
            if (delta == Point.Empty) return;

            _syncingPairLocation = true;
            try
            {
                GameScene.Game.GroupBox.MoveFromGroupBag(delta);
            }
            finally
            {
                _syncingPairLocation = false;
            }
        }

        public void Update(ClientGroupLootInfo info)
        {
            Info = info ?? new ClientGroupLootInfo();
            RefreshDisplay();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode != Keys.Escape) return;

            CloseButton.InvokeMouseClick();
            if (!Config.EscapeCloseAll)
                e.Handled = true;
        }

        private void CloseButton_MouseClick(object sender, MouseEventArgs e)
        {
            GameScene.Game.GroupBox.SetGroupBagVisible(false);
        }

        public void RefreshPermissions()
        {
            bool leader = GameScene.Game.GroupBox.Members.Count > 0 && GameScene.Game.GroupBox.Members[0].ObjectID == GameScene.Game.User.ObjectID;
            ShareButton.Enabled = GameScene.Game.GroupLootEnabled && leader && Info.Mode != GroupLootMode.FreeForAll && Info.Items.Count > 0 && !Info.Sharing;
            OptionsButton.Enabled = GameScene.Game.GroupLootEnabled && !GameScene.Game.Observer;
        }

        private void Grid_MouseWheel(object sender, MouseEventArgs e)
        {
            int change = e.Delta / SystemInformation.MouseWheelScrollDelta;
            if (change == 0)
                change = Math.Sign(e.Delta);

            int maximum = Math.Max(0, Grid.GridSize.Height - VisibleRows);
            _scrollRow = Math.Max(0, Math.Min(maximum, _scrollRow - change));
            Grid.ScrollValue = _scrollRow;
        }

        private void AttachCellHandlers()
        {
            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.MouseWheel += Grid_MouseWheel;
                cell.MouseDoubleClick += GridCell_MouseDoubleClick;
            }
        }

        private void GridCell_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || GameScene.Game.Observer || Info.Sharing) return;
            if (!Info.AllowManualTaking && Info.Mode != GroupLootMode.FreeForAll) return;
            if (sender is not DXItemCell cell || cell.Item == null) return;

            CEnvir.Enqueue(new C.GroupLootTake { ItemIndex = cell.Item.Index });
        }

        private void RefreshDisplay()
        {
            int rows = Math.Max(VisibleRows, (Info.Items.Count + Columns - 1) / Columns);

            DisplayItems = new ClientUserItem[rows * Columns];
            for (int i = 0; i < Info.Items.Count; i++)
                DisplayItems[i] = Info.Items[i];

            Grid.ItemGrid = DisplayItems;

            if (Grid.GridSize.Height != rows)
            {
                Grid.GridSize = new Size(Columns, rows);
                AttachCellHandlers();
            }

            _scrollRow = Math.Min(_scrollRow, rows - VisibleRows);
            Grid.ScrollValue = _scrollRow;

            WeightLabel.Text = $"{Info.Weight} of {Info.Capacity}";
            WeightLabel.ForeColour = Info.Weight > Info.Capacity ? Color.Red : Color.White;
            ModeLabel.Text = GetModeText(Info.Mode);
            RefreshFilterDisplay();
            RefreshPermissions();
        }

        private void RefreshFilterDisplay()
        {
            List<string> rarityNames = Enum.GetValues(typeof(Rarity)).Cast<Rarity>()
                .Where(x => Info.Rarities.Contains(x))
                .Select(x => DXCheckedComboBox.GetEnumDisplayText(x))
                .ToList();
            List<string> itemTypeNames = Enum.GetValues(typeof(ItemType)).Cast<ItemType>()
                .Where(x => x != ItemType.Nothing && Info.ItemTypes.Contains(x))
                .Select(x => DXCheckedComboBox.GetEnumDisplayText(x))
                .ToList();

            FilterLabel.Text = string.Format(CEnvir.Language.CommonControlSelectedCount, rarityNames.Count + itemTypeNames.Count);

            List<string> hintSections = new();
            if (rarityNames.Count > 0)
                hintSections.Add(DXCheckedComboBox.BuildHintSection(CEnvir.Language.GroupLootRarities, rarityNames));
            if (itemTypeNames.Count > 0)
                hintSections.Add(DXCheckedComboBox.BuildHintSection(CEnvir.Language.GroupLootItemTypes, itemTypeNames));

            FilterLabel.Hint = hintSections.Count == 0
                ? CEnvir.Language.CommonControlNoneSelected
                : string.Join(Environment.NewLine, hintSections);
        }

        public static string GetModeText(GroupLootMode mode)
        {
            return DXCheckedComboBox.GetEnumDisplayText(mode);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Info = null;
            DisplayItems = null;

            if (Grid != null)
            {
                Grid.MouseWheel -= Grid_MouseWheel;

                if (Grid.Grid != null)
                    foreach (DXItemCell cell in Grid.Grid)
                    {
                        cell.MouseWheel -= Grid_MouseWheel;
                        cell.MouseDoubleClick -= GridCell_MouseDoubleClick;
                    }

                if (!Grid.IsDisposed)
                    Grid.Dispose();

                Grid = null;
            }

            if (WeightLabel != null)
            {
                if (!WeightLabel.IsDisposed)
                    WeightLabel.Dispose();

                WeightLabel = null;
            }

            if (TitleLabel != null)
            {
                if (!TitleLabel.IsDisposed)
                    TitleLabel.Dispose();

                TitleLabel = null;
            }

            if (ModeTitle != null)
            {
                if (!ModeTitle.IsDisposed)
                    ModeTitle.Dispose();

                ModeTitle = null;
            }

            if (ModeLabel != null)
            {
                if (!ModeLabel.IsDisposed)
                    ModeLabel.Dispose();

                ModeLabel = null;
            }

            if (FilterTitle != null)
            {
                if (!FilterTitle.IsDisposed)
                    FilterTitle.Dispose();

                FilterTitle = null;
            }

            if (FilterLabel != null)
            {
                if (!FilterLabel.IsDisposed)
                    FilterLabel.Dispose();

                FilterLabel = null;
            }

            if (CloseButton != null)
            {
                CloseButton.MouseClick -= CloseButton_MouseClick;

                if (!CloseButton.IsDisposed)
                    CloseButton.Dispose();

                CloseButton = null;
            }

            if (ShareButton != null)
            {
                if (!ShareButton.IsDisposed)
                    ShareButton.Dispose();

                ShareButton = null;
            }

            if (OptionsButton != null)
            {
                if (!OptionsButton.IsDisposed)
                    OptionsButton.Dispose();

                OptionsButton = null;
            }

            if (WeightBar != null)
            {
                if (!WeightBar.IsDisposed)
                    WeightBar.Dispose();

                WeightBar = null;
            }
        }
    }

    public sealed class GroupLootSettingsDialog : DXWindow
    {
        public DXComboBox ModeBox;
        public DXCheckedComboBox FilterBox;
        public DXCheckBox BagEnabledBox, NeedRestrictionsBox, AllowManualTakingBox;
        public DXButton SaveButton, CancelButton;
        public readonly List<DXLabel> Labels = new();
        private bool _editable;
        private bool _dirty;
        private bool _loading;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        public GroupLootSettingsDialog(ClientGroupLootInfo info)
        {
            _editable = CanEdit();
            HasFooter = true;
            Modal = true;
            Parent = ActiveScene;
            DropShadow = true;
            MessageBoxList.Add(this);
            TitleLabel.Text = CEnvir.Language.GroupLootSettingsTitle;
            SetClientSize(new Size(300, 132));
            CloseButton.MouseClick += (o, e) => Dispose();

            AddLabel(CEnvir.Language.GroupLootMode, new Point(15, 37));

            ModeBox = new DXComboBox { Parent = this, Location = new Point(100, 38), Size = new Size(180, 16), Enabled = _editable };
            foreach (GroupLootMode mode in Enum.GetValues(typeof(GroupLootMode)))
                _ = new DXListBoxItem { Parent = ModeBox.ListBox, Label = { Text = GroupBagDialog.GetModeText(mode) }, Item = mode };
            ModeBox.ListBox.SelectItem(info.Mode);

            AddLabel(CEnvir.Language.GroupLootFilters, new Point(15, 69));
            FilterBox = new DXCheckedComboBox
            {
                Parent = this,
                Location = new Point(100, 70),
                Size = new Size(180, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 280,
                EmptyText = CEnvir.Language.CommonControlNoneSelected,
                SelectedCountFormat = CEnvir.Language.CommonControlSelectedCount,
                ReadOnly = !_editable,
            };

            DXCheckedComboBoxSection raritySection = FilterBox.AddSection(CEnvir.Language.GroupLootRarities);
            foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
                raritySection.AddEnumItem(rarity, info.Rarities.Contains(rarity));

            DXCheckedComboBoxSection itemTypeSection = FilterBox.AddSection(CEnvir.Language.GroupLootItemTypes);
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                if (type == ItemType.Nothing) continue;
                itemTypeSection.AddEnumItem(type, info.ItemTypes.Contains(type));
            }

            BagEnabledBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(15, 101),
                Label = { Text = CEnvir.Language.GroupLootBagEnabled },
                Checked = info.BagEnabled,
                ReadOnly = !_editable,
            };

            NeedRestrictionsBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(15, 122),
                Label = { Text = CEnvir.Language.GroupLootNeedRestrictions },
                Checked = info.NeedRestrictions,
                ReadOnly = !_editable,
            };

            AllowManualTakingBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(15, 143),
                Label = { Text = CEnvir.Language.GroupLootAllowManualTaking },
                Checked = info.AllowManualTaking,
                ReadOnly = !_editable,
            };

            SaveButton = new DXButton
            {
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(Size.Width / 2 - 90, Size.Height - 43),
                LabelStyle = ButtonLabelStyle.Gold,
                Label = { Text = CEnvir.Language.FilterDialogSaveButtonLabel },
                Enabled = _editable,
            };
            SaveButton.MouseClick += SaveButton_MouseClick;

            CancelButton = new DXButton
            {
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                LabelStyle = ButtonLabelStyle.Gold,
                Label = { Text = CEnvir.Language.CommonControlCancel },
            };
            CancelButton.MouseClick += (o, e) => Dispose();

            ModeBox.SelectedItemChanged += SettingChanged;
            FilterBox.CheckedItemsChanged += SettingChanged;
            BagEnabledBox.CheckedChanged += SettingChanged;
            NeedRestrictionsBox.CheckedChanged += SettingChanged;
            AllowManualTakingBox.CheckedChanged += SettingChanged;

            Location = new Point((SceneLayoutSize.Width - Size.Width) / 2, (SceneLayoutSize.Height - Size.Height) / 2);
        }

        private bool CanEdit()
        {
            if (GameScene.Game.Observer) return false;

            List<ClientPlayerInfo> members = GameScene.Game.GroupBox.Members;
            return members.Count == 0 || members[0].ObjectID == GameScene.Game.User.ObjectID;
        }

        private void SettingChanged(object sender, EventArgs e)
        {
            if (!_loading)
                _dirty = true;
        }

        public void RefreshPermissions()
        {
            _editable = CanEdit();

            if (ModeBox != null) ModeBox.Enabled = _editable;
            if (FilterBox != null) FilterBox.ReadOnly = !_editable;
            if (BagEnabledBox != null) BagEnabledBox.ReadOnly = !_editable;
            if (NeedRestrictionsBox != null) NeedRestrictionsBox.ReadOnly = !_editable;
            if (AllowManualTakingBox != null) AllowManualTakingBox.ReadOnly = !_editable;
            if (SaveButton != null) SaveButton.Enabled = _editable;
        }

        public void Update(ClientGroupLootInfo info)
        {
            bool wasEditable = _editable;
            RefreshPermissions();

            if (!_editable || wasEditable != _editable || !_dirty)
                ApplyInfo(info);
        }

        private void ApplyInfo(ClientGroupLootInfo info)
        {
            if (info == null) return;

            _loading = true;
            try
            {
                ModeBox.ListBox.SelectItem(info.Mode);
                BagEnabledBox.SetSilentState(info.BagEnabled);
                NeedRestrictionsBox.SetSilentState(info.NeedRestrictions);
                AllowManualTakingBox.SetSilentState(info.AllowManualTaking);

                foreach (DXCheckedListBoxItem item in FilterBox.Items)
                {
                    item.Checked = item.Item switch
                    {
                        ItemType type => info.ItemTypes.Contains(type),
                        Rarity rarity => info.Rarities.Contains(rarity),
                        _ => false,
                    };
                }

                _dirty = false;
            }
            finally
            {
                _loading = false;
            }
        }

        private void AddLabel(string text, Point location)
        {
            Labels.Add(new DXLabel { Parent = this, Text = text, Location = location, ForeColour = Color.White });
        }

        private void SaveButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (!_editable) return;
            CEnvir.Enqueue(new C.GroupLootSettings
            {
                Mode = ModeBox.SelectedItem is GroupLootMode mode ? mode : GroupLootMode.Random,
                BagEnabled = BagEnabledBox.Checked,
                NeedRestrictions = NeedRestrictionsBox.Checked,
                AllowManualTaking = AllowManualTakingBox.Checked,
                ItemTypes = FilterBox.CheckedItems.OfType<ItemType>().ToList(),
                Rarities = FilterBox.CheckedItems.OfType<Rarity>().ToList(),
            });
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            MessageBoxList.Remove(this);
            foreach (DXLabel label in Labels)
            {
                if (label == null) continue;

                if (!label.IsDisposed)
                    label.Dispose();
            }
            Labels.Clear();

            if (ModeBox != null)
            {
                ModeBox.SelectedItemChanged -= SettingChanged;
                if (!ModeBox.IsDisposed)
                    ModeBox.Dispose();

                ModeBox = null;
            }

            if (FilterBox != null)
            {
                FilterBox.CheckedItemsChanged -= SettingChanged;
                if (!FilterBox.IsDisposed)
                    FilterBox.Dispose();

                FilterBox = null;
            }

            if (NeedRestrictionsBox != null)
            {
                NeedRestrictionsBox.CheckedChanged -= SettingChanged;
                if (!NeedRestrictionsBox.IsDisposed)
                    NeedRestrictionsBox.Dispose();

                NeedRestrictionsBox = null;
            }

            if (BagEnabledBox != null)
            {
                BagEnabledBox.CheckedChanged -= SettingChanged;
                if (!BagEnabledBox.IsDisposed)
                    BagEnabledBox.Dispose();

                BagEnabledBox = null;
            }

            if (AllowManualTakingBox != null)
            {
                AllowManualTakingBox.CheckedChanged -= SettingChanged;
                if (!AllowManualTakingBox.IsDisposed)
                    AllowManualTakingBox.Dispose();

                AllowManualTakingBox = null;
            }

            if (SaveButton != null)
            {
                SaveButton.MouseClick -= SaveButton_MouseClick;
                if (!SaveButton.IsDisposed)
                    SaveButton.Dispose();

                SaveButton = null;
            }

            if (CancelButton != null)
            {
                if (!CancelButton.IsDisposed)
                    CancelButton.Dispose();

                CancelButton = null;
            }
        }
    }

    public sealed class GroupLootVoteDialog : DXWindow
    {
        public DXItemGrid ItemGrid;
        public DXLabel MessageLabel;
        public DXButton NeedButton, GreedButton, PassButton;
        public ClientUserItem[] ItemArray = new ClientUserItem[1];
        public DateTime Expiry;
        private bool _submitted;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        public GroupLootVoteDialog(ClientUserItem item, TimeSpan duration, bool canNeed)
        {
            Modal = true;
            HasFooter = true;
            Parent = ActiveScene;
            DropShadow = true;
            MessageBoxList.Add(this);
            TitleLabel.Text = CEnvir.Language.GroupLootVoteTitle;
            SetClientSize(new Size(360, 90));
            ItemArray[0] = item;
            Expiry = CEnvir.Now + duration;
            int contentY = ClientArea.Y + (ClientArea.Height - DXItemCell.CellHeight) / 2;

            ItemGrid = new DXItemGrid
            {
                Parent = this,
                GridSize = new Size(1, 1),
                ItemGrid = ItemArray,
                GridType = GridType.GroupLoot,
                Location = new Point(30, contentY),
                ReadOnly = true,
                AllowLink = false,
            };

            MessageLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(80, contentY),
                Size = new Size(250, DXItemCell.CellHeight),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            NeedButton = CreateButton(CEnvir.Language.GroupLootNeed, 45, GroupLootVote.Need);
            NeedButton.Enabled = canNeed;
            GreedButton = CreateButton(CEnvir.Language.GroupLootGreed, 140, GroupLootVote.Greed);
            PassButton = CreateButton(CEnvir.Language.GroupLootPass, 235, GroupLootVote.Pass);
            CloseButton.MouseClick += (o, e) => Submit(GroupLootVote.Pass);
            Location = new Point((SceneLayoutSize.Width - Size.Width) / 2, (SceneLayoutSize.Height - Size.Height) / 2);
        }

        private DXButton CreateButton(string text, int x, GroupLootVote vote)
        {
            DXButton button = new DXButton { Parent = this, Size = new Size(80, DefaultHeight), Location = new Point(x, Size.Height - 43), LabelStyle = ButtonLabelStyle.Gold, Label = { Text = text } };
            button.MouseClick += (o, e) => Submit(vote);
            return button;
        }

        private void Submit(GroupLootVote vote)
        {
            if (_submitted) return;
            _submitted = true;
            CEnvir.Enqueue(new C.GroupLootVote { ItemIndex = ItemArray[0].Index, Vote = vote });
            Dispose();
        }

        public override void Process()
        {
            base.Process();
            int seconds = Math.Max(0, (int)Math.Ceiling((Expiry - CEnvir.Now).TotalSeconds));
            MessageLabel.Text = string.Format(CEnvir.Language.GroupLootVoteTime, ItemArray[0].Info.ItemName, seconds);
            if (seconds == 0) Submit(GroupLootVote.Pass);
        }

        public void CancelWithoutVote()
        {
            _submitted = true;
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            MessageBoxList.Remove(this);
            if (GameScene.Game?.GroupLootVoteBox == this)
                GameScene.Game.GroupLootVoteBox = null;

            ItemArray = null;

            if (ItemGrid != null)
            {
                if (!ItemGrid.IsDisposed)
                    ItemGrid.Dispose();

                ItemGrid = null;
            }

            if (MessageLabel != null)
            {
                if (!MessageLabel.IsDisposed)
                    MessageLabel.Dispose();

                MessageLabel = null;
            }

            if (NeedButton != null)
            {
                if (!NeedButton.IsDisposed)
                    NeedButton.Dispose();

                NeedButton = null;
            }

            if (GreedButton != null)
            {
                if (!GreedButton.IsDisposed)
                    GreedButton.Dispose();

                GreedButton = null;
            }

            if (PassButton != null)
            {
                if (!PassButton.IsDisposed)
                    PassButton.Dispose();

                PassButton = null;
            }
        }
    }
}
