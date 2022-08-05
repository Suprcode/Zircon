using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Linq;
using Library.SystemModels;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class StorageDialog : DXWindow
    {
        #region Properties
        public DXItemGrid Grid, PartGrid;
        
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.StorageBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion
        private DXTabControl TabControl;
        private DXTab StorageTab, PartsTab;
        public DXTextBox ItemNameTextBox;
        public DXComboBox ItemTypeComboBox;
        public DXButton ClearButton;
        public DXVScrollBar StorageScrollBar, PartsScrollBar;
        public ClientUserItem[] GuildStorage = new ClientUserItem[1000];

        public StorageDialog()
        {
            TitleLabel.Text = "Storage";

            SetClientSize(new Size(483, 411));

            DXControl filterPanel = new DXControl
            {
                Parent = this,
                Size = new Size(ClientArea.Width, 26),
                Location = new Point(ClientArea.Location.X, ClientArea.Location.Y),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(filterPanel.Location.X, filterPanel.Location.Y + filterPanel.Size.Height),
                Size = new Size(ClientArea.Width, 390),
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = "Name:",
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
                Text = "Item:",
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

            for (ItemType i = ItemType.Nothing; i <= ItemType.ItemPart; i++)
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
                Label = { Text = "Clear" }
            };
            ClearButton.MouseClick += (o, e) =>
            {
                ItemTypeComboBox.ListBox.SelectItem(null);
                ItemNameTextBox.TextBox.Text = string.Empty;
            };

            StorageTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Storage" } },
                Visible = true,
            };

            PartsTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Parts" } },
                Visible = false,
            };

            Grid = new DXItemGrid
            {
                Parent = StorageTab,
                GridSize = new Size(1, 1),
                Location = new Point(5, 10),
                GridType = GridType.Storage,
                ItemGrid = CEnvir.Storage,
                VisibleHeight = 10,
            };

            Grid.GridSizeChanged += StorageGrid_GridSizeChanged;

            PartGrid = new DXItemGrid
            {
                Parent = PartsTab,
                GridSize = new Size(13, 76),
                Location = new Point(5, 10),
                GridType = GridType.PartsStorage,
                ItemGrid = CEnvir.PartsStorage,
                VisibleHeight = 10,
            };

            StorageScrollBar = new DXVScrollBar
            {
                Parent = StorageTab,
                Location = new Point(Grid.Location.X + PartGrid.Size.Width, Grid.Location.Y),
                Size = new Size(14, 349),
                VisibleSize = 10,
                Change = 1,
            };
            StorageScrollBar.ValueChanged += StorageScrollBar_ValueChanged;

            PartsScrollBar = new DXVScrollBar
            {
                Parent = PartsTab,
                Location = new Point(PartGrid.Location.X+PartGrid.Size.Width, PartGrid.Location.Y),
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

        public void RefreshStorage()
        {
            Grid.GridSize = new Size(13, Math.Max(10, (int)Math.Ceiling(GameScene.Game.StorageSize / (float)13)));

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
            }

        }

        #endregion
    }
}
