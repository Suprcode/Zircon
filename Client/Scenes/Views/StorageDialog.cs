using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class StorageDialog : DXImageControl
    {
        #region Properties

        private DXTabControl TabControl;
        private DXTab StorageTab, PartsTab;
        public DXLabel TitleLabel;
        public DXTextBox ItemNameTextBox;
        public DXComboBox ItemTypeComboBox;
        public DXButton CloseButton, ClearButton, SortButton;
        public DXVScrollBar StorageScrollBar, PartsScrollBar;

        public DXItemGrid Grid, PartGrid;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                Grid.ClearLinks();

            if (IsVisible)
                BringToFront();

            if (Settings != null)
                Settings.Visible = nValue;
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (CloseButton.Visible)
                    {
                        CloseButton.InvokeMouseClick();
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    break;
            }
        }

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.StorageBox;

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

        #endregion

        public StorageDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 121;
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

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.StorageDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            SortButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 364,
                Parent = this,
                Hint = CEnvir.Language.StorageDialogSortButtonLabel
            };
            SortButton.Location = new Point(DisplayArea.Width - 47, 41);
            SortButton.MouseClick += SortButton_MouseClick;

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(0, 61),
                Size = new Size(410, 420),
                Border = false,
                MarginLeft = 10
            };

            #region FilterPanel

            DXControl filterPanel = new DXControl
            {
                Parent = this,
                Size = new Size(250, 26),
                Location = new Point(0, 0),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Visible = false
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = CEnvir.Language.StorageDialogFilterNameLabel,
            };

            ItemNameTextBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(180, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            };
            ItemNameTextBox.TextBox.TextChanged += (o, e) => ApplyStorageFilter();

            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(ItemNameTextBox.Location.X + ItemNameTextBox.Size.Width + 10, 5),
                Text = CEnvir.Language.StorageDialogFilterItemLabel,
            };

            ItemTypeComboBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
            };
            ItemTypeComboBox.SelectedItemChanged += (o, e) => ApplyStorageFilter();

            new DXListBoxItem
            {
                Parent = ItemTypeComboBox.ListBox,
                Label = { Text = $"All" },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.Reel; i++)
            {
                MemberInfo[] infos = itemType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                new DXListBoxItem
                {
                    Parent = ItemTypeComboBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            ItemTypeComboBox.ListBox.SelectItem(null);

            ClearButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(ItemTypeComboBox.Location.X + ItemTypeComboBox.Size.Width + 17, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.StorageDialogFilterClearButtonLabel }
            };
            ClearButton.MouseClick += (o, e) =>
            {
                ItemTypeComboBox.ListBox.SelectItem(null);
                ItemNameTextBox.TextBox.Text = string.Empty;
            };
            #endregion

            StorageTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.StorageDialogStorageTab } },
                Visible = true,
                BackColour = Color.Empty,
                Border = false,
            };

            PartsTab = new DXTab
            {
                Parent = TabControl,
                Border = false,
                TabButton = { Label = { Text = CEnvir.Language.StorageDialogPartsTab } },
                Visible = false,
                BackColour = Color.Empty
            };

            Grid = new DXItemGrid
            {
                Parent = StorageTab,
                GridSize = new Size(1, 1),
                Location = new Point(19, 11),
                GridType = GridType.Storage,
                ItemGrid = CEnvir.Storage,
                VisibleHeight = 10,
                Border = false,
                GridPadding = 1,
                BackColour = Color.Empty
            };

            Grid.GridSizeChanged += StorageGrid_GridSizeChanged;

            PartGrid = new DXItemGrid
            {
                Parent = PartsTab,
                GridSize = new Size(10, 100),
                Location = new Point(19, 11),
                GridType = GridType.PartsStorage,
                ItemGrid = CEnvir.PartsStorage,
                VisibleHeight = 10,
                Border = false,
                GridPadding = 1,
                BackColour = Color.Empty
            };

            StorageScrollBar = new DXVScrollBar
            {
                Visible = false,
                Parent = StorageTab,
                Location = new Point(Grid.Location.X + PartGrid.Size.Width, Grid.Location.Y),
                Size = new Size(14, 349),
                VisibleSize = 10,
                Change = 1,
            };
            StorageScrollBar.ValueChanged += StorageScrollBar_ValueChanged;

            PartsScrollBar = new DXVScrollBar
            {
                Visible = false,
                Parent = PartsTab,
                Location = new Point(PartGrid.Location.X + PartGrid.Size.Width, PartGrid.Location.Y),
                Size = new Size(14, 349),
                VisibleSize = 10,
                Change = 1,
            };
            PartsScrollBar.ValueChanged += PartsScrollBar_ValueChanged;
            PartsScrollBar.MaxValue = PartGrid.GridSize.Height;

            foreach (DXItemCell cell in Grid.Grid)
                cell.MouseWheel += StorageScrollBar.DoMouseWheel;

            foreach (DXItemCell cell in PartGrid.Grid)
                cell.MouseWheel += PartsScrollBar.DoMouseWheel;
        }

        private void SortButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            DXMessageBox box = new DXMessageBox("Are you sure you want to sort your storage?", "Confirm Sort", DXMessageBoxButtons.YesNo);

            box.YesButton.MouseClick += (o1, e1) =>
            {
                C.ItemSort packet = new C.ItemSort { Grid = StorageTab.Visible ? GridType.Storage : GridType.PartsStorage };

                CEnvir.Enqueue(packet);
            };
        }

        public void RefreshStorage()
        {
            Grid.GridSize = new Size(10, Math.Max(10, (int)Math.Ceiling(GameScene.Game.StorageSize / (float)10)));

            StorageScrollBar.MaxValue = Grid.GridSize.Height;

            ApplyStorageFilter();
        }

        private void StorageGrid_GridSizeChanged(object sender, EventArgs e)
        {
            foreach (DXItemCell cell in Grid.Grid)
                cell.ItemChanged += (o, e1) => FilterCell(cell);

            foreach (DXItemCell cell in Grid.Grid)
                cell.MouseWheel += StorageScrollBar.DoMouseWheel;
        }

        public void ApplyStorageFilter()
        {
            foreach (DXItemCell cell in Grid.Grid)
                FilterCell(cell);

            foreach (DXItemCell cell in PartGrid.Grid)
                FilterPartsCell(cell);
        }

        public void FilterCell(DXItemCell cell)
        {
            if (cell.Slot >= GameScene.Game.StorageSize)
            {
                cell.Enabled = false;
                return;
            }

            if (cell.Item == null && (ItemTypeComboBox.SelectedItem != null || !string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text)))
            {
                cell.Enabled = false;
                return;
            }


            if (ItemTypeComboBox.SelectedItem != null && cell.Item != null && cell.Item.Info.ItemType != (ItemType)ItemTypeComboBox.SelectedItem)
            {
                cell.Enabled = false;
                return;
            }


            if (!string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text) && cell.Item != null && cell.Item.Info.ItemName.IndexOf(ItemNameTextBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
            {
                cell.Enabled = false;
                return;
            }


            cell.Enabled = true;
        }

        public void FilterPartsCell(DXItemCell cell)
        {

            if (cell.Item == null && (ItemTypeComboBox.SelectedItem != null || !string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text)))
            {
                cell.Enabled = false;
                return;
            }


            if (ItemTypeComboBox.SelectedItem != null && cell.Item != null && cell.Item.Info.ItemType != (ItemType)ItemTypeComboBox.SelectedItem)
            {
                cell.Enabled = false;
                return;
            }

            if (cell.Item != null)
            {
                ItemInfo info = Globals.ItemInfoList.Binding.First(x => x.Index == cell.Item.AddedStats[Stat.ItemIndex]);

                if (!string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text) && info.ItemName.IndexOf(ItemNameTextBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    cell.Enabled = false;
                    return;
                }
            }

            if (cell.Item != null && (cell.Item.Info.PartCount <= cell.Item.Count))
                cell.Opacity = 0.8f;


            cell.Enabled = true;
        }

        private void StorageScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Grid.ScrollValue = StorageScrollBar.Value;
        }

        private void PartsScrollBar_ValueChanged(object sender, EventArgs e)
        {
            PartGrid.ScrollValue = PartsScrollBar.Value;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (StorageTab != null)
                {
                    if (!StorageTab.IsDisposed)
                        StorageTab.Dispose();

                    StorageTab = null;
                }

                if (PartsTab != null)
                {
                    if (!PartsTab.IsDisposed)
                        PartsTab.Dispose();

                    PartsTab = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (ItemNameTextBox != null)
                {
                    if (!ItemNameTextBox.IsDisposed)
                        ItemNameTextBox.Dispose();

                    ItemNameTextBox = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (ClearButton != null)
                {
                    if (!ClearButton.IsDisposed)
                        ClearButton.Dispose();

                    ClearButton = null;
                }

                if (SortButton != null)
                {
                    if (!SortButton.IsDisposed)
                        SortButton.Dispose();

                    SortButton = null;
                }

                if (StorageScrollBar != null)
                {
                    if (!StorageScrollBar.IsDisposed)
                        StorageScrollBar.Dispose();

                    StorageScrollBar = null;
                }

                if (PartsScrollBar != null)
                {
                    if (!PartsScrollBar.IsDisposed)
                        PartsScrollBar.Dispose();

                    PartsScrollBar = null;
                }
            }
        }

        #endregion
    }
}
