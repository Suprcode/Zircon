using System;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class AutoPotionDialog : DXWindow
    {
        #region Properties
        public ClientAutoPotionLink[] Links;

        public AutoPotionRow[] Rows;
        public DXVScrollBar ScrollBar;

        public override WindowType Type => WindowType.AutoPotionBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        public AutoPotionDialog()
        {
            TitleLabel.Text = CEnvir.Language.AutoPotionTitle;
            HasFooter = true;

            SetClientSize(new Size(280, 398));

            Links = new ClientAutoPotionLink[Globals.MaxAutoPotionCount];
            Rows = new AutoPotionRow[Globals.MaxAutoPotionCount];


            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(14, ClientArea.Height - 2),
                Location = new Point(ClientArea.Right - 14, ClientArea.Top + 1),
                VisibleSize = ClientArea.Height,
                MaxValue = Rows.Length * 50 - 2

            };
            DXControl panel = new DXControl
            {
                Parent = this,
                Size = new Size(ClientArea.Size.Width - 16, ClientArea.Size.Height),
                Location = ClientArea.Location,
            };
            panel.MouseWheel += ScrollBar.DoMouseWheel;

            for (int i = 0; i < Links.Length; i++)
            {
                AutoPotionRow row;
                Rows[i] = row = new AutoPotionRow
                {
                    Parent = panel,
                    Location = new Point(1, 1 + 50 * i),
                    Index = i,
                };
                row.MouseWheel += ScrollBar.DoMouseWheel;
            }
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();
        }

        #region Methods
        public void UpdateLocations()
        {
            int y = -ScrollBar.Value;

            foreach (AutoPotionRow row in Rows)
                row.Location = new Point(1, 1 + 50 * row.Index + y);
        }

        public bool Updating;

        public void UpdateLinks()
        {
            Updating = true;
            foreach (ClientAutoPotionLink link in Links)
            {
                if (link == null || link.Slot < 0 || link.Slot >= Rows.Length) continue;

                Rows[link.Slot].ItemCell.QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == link.LinkInfoIndex);
                Rows[link.Slot].HealthTargetBox.Value = link.Health;
                Rows[link.Slot].ManaTargetBox.Value = link.Mana;
                Rows[link.Slot].EnabledCheckBox.Checked = link.Enabled;
            }
            Updating = false;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Links != null)
                {
                    for (int i = 0; i < Links.Length; i++)
                        Links[i] = null;

                    Links = null;
                }

                for (int i = 0; i < Rows.Length; i++)
                {
                    if (Rows[i] == null) continue;

                    if (!Rows[i].IsDisposed)
                        Rows[i].Dispose();

                    Rows[i] = null;
                }

                Rows = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }
            }

        }

        #endregion
    }

    public sealed class AutoPotionRow : DXControl
    {
        #region Properties

        #region UseItem

        public ItemInfo UseItem
        {
            get => _UseItem;
            set
            {
                if (_UseItem == value) return;

                ItemInfo oldValue = _UseItem;
                _UseItem = value;

                OnUseItemChanged(oldValue, value);
            }
        }
        private ItemInfo _UseItem;
        public event EventHandler<EventArgs> UseItemChanged;
        public void OnUseItemChanged(ItemInfo oValue, ItemInfo nValue)
        {
            UseItemChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Index

        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value) return;

                int oldValue = _Index;
                _Index = value;

                OnIndexChanged(oldValue, value);
            }
        }
        private int _Index;
        public event EventHandler<EventArgs> IndexChanged;
        public void OnIndexChanged(int oValue, int nValue)
        {
            IndexChanged?.Invoke(this, EventArgs.Empty);
            IndexLabel.Text = (Index + 1).ToString();
            ItemCell.Slot = Index;

            UpButton.Enabled = Index > 0;
            DownButton.Enabled = Index < 7;
        }

        #endregion

        public DXLabel IndexLabel, HealthLabel, ManaLabel;
        public DXItemCell ItemCell;
        public DXNumberBox HealthTargetBox, ManaTargetBox;
        public DXCheckBox EnabledCheckBox;
        public DXButton UpButton, DownButton;

        #endregion

        public AutoPotionRow()
        {
            Size = new Size(260, 46);

            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            UpButton = new DXButton
            {
                Index = 44,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(5, 5),
                Parent = this,
                Enabled = false,
            };
            UpButton.MouseClick += (o, e) =>
            {
                GameScene.Game.AutoPotionBox.Updating = true;

                int hp = (int)HealthTargetBox.Value;
                int mp = (int)ManaTargetBox.Value;
                bool enabled = EnabledCheckBox.Checked;
                ItemInfo info = ItemCell.QuickInfo;

                ItemCell.QuickInfo = GameScene.Game.AutoPotionBox.Rows[Index - 1].ItemCell.QuickInfo;
                HealthTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index - 1].HealthTargetBox.Value;
                ManaTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index - 1].ManaTargetBox.Value;
                EnabledCheckBox.Checked = GameScene.Game.AutoPotionBox.Rows[Index - 1].EnabledCheckBox.Checked;

                GameScene.Game.AutoPotionBox.Rows[Index - 1].ItemCell.QuickInfo = info;
                GameScene.Game.AutoPotionBox.Rows[Index - 1].HealthTargetBox.Value = hp;
                GameScene.Game.AutoPotionBox.Rows[Index - 1].ManaTargetBox.Value = mp;
                GameScene.Game.AutoPotionBox.Rows[Index - 1].EnabledCheckBox.Checked = enabled;

                GameScene.Game.AutoPotionBox.Updating = false;

                SendUpdate();
                GameScene.Game.AutoPotionBox.Rows[Index - 1].SendUpdate();
            };

            DownButton = new DXButton
            {
                Index = 46,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(5, 29),
                Parent = this,
            };
            DownButton.MouseClick += (o, e) =>
            {
                GameScene.Game.AutoPotionBox.Updating = true;

                int hp = (int)HealthTargetBox.Value;
                int mp = (int)ManaTargetBox.Value;
                bool enabled = EnabledCheckBox.Checked;
                ItemInfo info = ItemCell.QuickInfo;

                ItemCell.QuickInfo = GameScene.Game.AutoPotionBox.Rows[Index + 1].ItemCell.QuickInfo;
                HealthTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index + 1].HealthTargetBox.Value;
                ManaTargetBox.Value = GameScene.Game.AutoPotionBox.Rows[Index + 1].ManaTargetBox.Value;
                EnabledCheckBox.Checked = GameScene.Game.AutoPotionBox.Rows[Index + 1].EnabledCheckBox.Checked;

                GameScene.Game.AutoPotionBox.Rows[Index + 1].ItemCell.QuickInfo = info;
                GameScene.Game.AutoPotionBox.Rows[Index + 1].HealthTargetBox.Value = hp;
                GameScene.Game.AutoPotionBox.Rows[Index + 1].ManaTargetBox.Value = mp;
                GameScene.Game.AutoPotionBox.Rows[Index + 1].EnabledCheckBox.Checked = enabled;

                GameScene.Game.AutoPotionBox.Updating = false;

                SendUpdate();
                GameScene.Game.AutoPotionBox.Rows[Index + 1].SendUpdate();
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(20, 5),
                AllowLink = true,
                FixedBorder = true,
                Border = true,
                GridType = GridType.AutoPotion,
            };

            IndexLabel = new DXLabel
            {
                Parent = ItemCell,
                Text = (Index + 1).ToString(),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Italic),
                IsControl = false,
                Location = new Point(-2, -1)
            };


            HealthTargetBox = new DXNumberBox
            {
                Parent = this,
                Location = new Point(105, 5),
                Size = new Size(80, 20),
                ValueTextBox = { Size = new Size(40, 18) },
                MaxValue = 50000,
                MinValue = 0,
                UpButton = { Location = new Point(63, 1) }
            };
            HealthTargetBox.ValueTextBox.ValueChanged += (o, e) => SendUpdate();

            ManaTargetBox = new DXNumberBox
            {
                Parent = this,
                Location = new Point(105, 25),
                Size = new Size(80, 20),
                ValueTextBox = { Size = new Size(40, 18) },
                MaxValue = 50000,
                MinValue = 0,
                UpButton = { Location = new Point(63, 1) }
            };
            ManaTargetBox.ValueTextBox.ValueChanged += (o, e) => SendUpdate();

            HealthLabel = new DXLabel
            {
                Parent = this,
                IsControl = false,
                Text = CEnvir.Language.CommonStatusHealth + ":"
            };
            HealthLabel.Location = new Point(HealthTargetBox.Location.X - HealthLabel.Size.Width, HealthTargetBox.Location.Y + (HealthTargetBox.Size.Height - HealthLabel.Size.Height) / 2);


            ManaLabel = new DXLabel
            {
                Parent = this,
                IsControl = false,
                Text = CEnvir.Language.CommonStatusMana + ":"
            };
            ManaLabel.Location = new Point(ManaTargetBox.Location.X - ManaLabel.Size.Width, ManaTargetBox.Location.Y + (ManaTargetBox.Size.Height - ManaLabel.Size.Height) / 2);

            EnabledCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.AutoPotionEnabledLabel },
                Parent = this,
            };
            EnabledCheckBox.CheckedChanged += (o, e) => SendUpdate();

            EnabledCheckBox.Location = new Point(Size.Width - EnabledCheckBox.Size.Width - 5, 5);
        }

        #region Methods

        public void SendUpdate()
        {
            if (GameScene.Game.Observer) return;

            if (GameScene.Game.AutoPotionBox.Updating) return;

            CEnvir.Enqueue(new C.AutoPotionLinkChanged { Slot = Index, LinkIndex = ItemCell.Item?.Info.Index ?? -1, Enabled = EnabledCheckBox.Checked, Health = (int)HealthTargetBox.Value, Mana = (int)ManaTargetBox.Value });
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _UseItem = null;
                UseItemChanged = null;

                _Index = 0;
                IndexChanged = null;

                if (IndexLabel != null)
                {
                    if (!IndexLabel.IsDisposed)
                        IndexLabel.Dispose();

                    IndexLabel = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
                }

                if (ManaLabel != null)
                {
                    if (!ManaLabel.IsDisposed)
                        ManaLabel.Dispose();

                    ManaLabel = null;
                }

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (HealthTargetBox != null)
                {
                    if (!HealthTargetBox.IsDisposed)
                        HealthTargetBox.Dispose();

                    HealthTargetBox = null;
                }

                if (ManaTargetBox != null)
                {
                    if (!ManaTargetBox.IsDisposed)
                        ManaTargetBox.Dispose();

                    ManaTargetBox = null;
                }

                if (EnabledCheckBox != null)
                {
                    if (!EnabledCheckBox.IsDisposed)
                        EnabledCheckBox.Dispose();

                    EnabledCheckBox = null;
                }

                if (UpButton != null)
                {
                    if (!UpButton.IsDisposed)
                        UpButton.Dispose();

                    UpButton = null;
                }

                if (DownButton != null)
                {
                    if (!DownButton.IsDisposed)
                        DownButton.Dispose();

                    DownButton = null;
                }

            }

        }

        #endregion
    }

}
