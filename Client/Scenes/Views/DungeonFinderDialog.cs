using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.Network.ServerPackets;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class DungeonFinderDialog : DXWindow
    {
        #region Properties

        public DXTabControl TabControl;

        public DXTab DungeonTab, RaidTab;
        public DXTextBox DungeonNameBox;
        public DXComboBox SortBox;
        public DXButton SearchButton;

        public DXVScrollBar DungeonScrollBar;
        public DungeonRow[] DungeonRows;
        public List<InstanceInfo> DungeonSearchResults;

        public DXButton JoinButton;

        #endregion

        #region SelectedStoreRow

        public DungeonRow SelectedDungeonRow
        {
            get => _SelectedDungeonRow;
            set
            {
                if (_SelectedDungeonRow == value) return;

                DungeonRow oldValue = _SelectedDungeonRow;
                _SelectedDungeonRow = value;

                OnSelectedDungeonRowChanged(oldValue, value);
            }
        }
        private DungeonRow _SelectedDungeonRow;
        public event EventHandler<EventArgs> SelectedDungeonRowChanged;
        public void OnSelectedDungeonRowChanged(DungeonRow oValue, DungeonRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            if (nValue?.InstanceInfo == null)
            {
                JoinButton.Visible = false;
            }
            else
            {
                JoinButton.Visible = true;
            }


            SelectedDungeonRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override WindowType Type => WindowType.DungeonFinderBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        public DungeonFinderDialog()
        {
            TitleLabel.Text = "Dungeon Finder";
            SetClientSize(new Size(560, 461));

            TabControl = new DXTabControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
            };

            DungeonTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = "Dungeons" }, Size = new Size(100, TabHeight) },
                Border = true,
            };

            RaidTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = "Raids" }, Size = new Size(100, TabHeight) },
                Border = true,
            };

            DXControl filterPanel = new DXControl
            {
                Parent = DungeonTab,
                Size = new Size(DungeonTab.Size.Width - 20, 26),
                Location = new Point(10, 10),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = "Name:",
            };

            DungeonNameBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(180, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            };
            DungeonNameBox.TextBox.KeyPress += TextBox_KeyPress;



            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(DungeonNameBox.Location.X + DungeonNameBox.Size.Width + 10, 5),
                Text = "Type:",
            };



            //DungeonTypeBox = new DXComboBox
            //{
            //    Parent = filterPanel,
            //    Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            //    Size = new Size(95, DXComboBox.DefaultNormalHeight),
            //    DropDownHeight = 198
            //};


            //new DXListBoxItem
            //{
            //    Parent = DungeonTypeBox.ListBox,
            //    Label = { Text = $"All" },
            //    Item = null
            //};

            //label = new DXLabel
            //{
            //    Parent = filterPanel,
            //    Location = new Point(DungeonTypeBox.Location.X + DungeonTypeBox.Size.Width + 10, 5),
            //    Text = "Sort:",
            //};

            SortBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(100, DXComboBox.DefaultNormalHeight)
            };

            Type storeType = typeof(DungeonFinderSort);

            for (DungeonFinderSort i = DungeonFinderSort.Name; i <= DungeonFinderSort.PlayerCount; i++)
            {
                MemberInfo[] infos = storeType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                new DXListBoxItem
                {
                    Parent = SortBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            SortBox.ListBox.SelectItem(DungeonFinderSort.Name);

            SearchButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(SortBox.Location.X + SortBox.Size.Width + 25, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Search" }
            };
            SearchButton.MouseClick += (o, e) => DungeonSearch();

            //DXButton ClearButton = new DXButton
            //{
            //    Size = new Size(50, SmallButtonHeight),
            //    Location = new Point(SearchButton.Location.X + SearchButton.Size.Width + 40, label.Location.Y - 1),
            //    Parent = filterPanel,
            //    ButtonType = ButtonType.SmallButton,
            //    Label = { Text = "Clear" }
            //};
            //ClearButton.MouseClick += (o, e) =>
            //{
            //    DungeonNameBox.TextBox.Text = "";
            //    //DungeonTypeBox.ListBox.SelectItem(null);
            //    DungeonSearch();
            //};

            DungeonRows = new DungeonRow[9];

            DungeonScrollBar = new DXVScrollBar
            {
                Parent = DungeonTab,
                Location = new Point(533, 47),
                Size = new Size(14, DungeonTab.Size.Height - 59),
                VisibleSize = DungeonRows.Length,
                Change = 3,
            };
            DungeonScrollBar.ValueChanged += DungeonScrollBar_ValueChanged;


            for (int i = 0; i < DungeonRows.Length; i++)
            {
                int index = i;
                DungeonRows[index] = new DungeonRow
                {
                    Parent = DungeonTab,
                    Location = new Point(10, 46 + i * 43),
                };
                DungeonRows[index].MouseClick += (o, e) => { SelectedDungeonRow = DungeonRows[index]; };
                DungeonRows[index].MouseWheel += DungeonScrollBar.DoMouseWheel;
            }

            #region Instance Details


            JoinButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(490, 35),
                Parent = this,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Join Instance" },
                Visible = false
            };
            JoinButton.MouseClick += (o, e) => JoinInstance();

            #endregion
        }

        #region Methods

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (DungeonRows == null) return; //Not Loaded

            if (!Visible)
            {
                return;
            }

            if (DungeonSearchResults == null)
                DungeonSearch();
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (SearchButton.Enabled)
                DungeonSearch();
        }

        private void DungeonScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshDungeonList();
        }

        public void DungeonSearch()
        {
            DungeonSearchResults = new List<InstanceInfo>();

            DungeonScrollBar.MaxValue = 0;


            foreach (DungeonRow row in DungeonRows)
                row.Visible = true;

            //string filter = (string)DungeonTypeBox.SelectedItem;

            DungeonFinderSort sort = (DungeonFinderSort)SortBox.SelectedItem;

            foreach (InstanceInfo info in Globals.InstanceInfoList.Binding)
            {
                if (info == null || !info.ShowOnDungeonFinder) continue;

                //if (filter != null && !info.Filter.Contains(filter)) continue;

                if (!string.IsNullOrEmpty(DungeonNameBox.TextBox.Text) && info.Name.IndexOf(DungeonNameBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0) continue;

                DungeonSearchResults.Add(info);
            }

            switch (sort)
            {
                case DungeonFinderSort.Name:
                    DungeonSearchResults.Sort((x1, x2) => string.Compare(x1.Name, x2.Name, StringComparison.Ordinal));
                    break;
                case DungeonFinderSort.Level:
                    DungeonSearchResults.Sort((x1, x2) => x2.MinPlayerLevel.CompareTo(x1.MinPlayerLevel));
                    break;
                case DungeonFinderSort.PlayerCount:
                    DungeonSearchResults.Sort((x1, x2) => x1.MaxPlayerCount.CompareTo(x2.MaxPlayerCount));
                    break;
            }

            RefreshDungeonList();
        }

        public void RefreshDungeonList()
        {
            if (DungeonSearchResults == null) return;

            DungeonScrollBar.MaxValue = DungeonSearchResults.Count;

            for (int i = 0; i < DungeonRows.Length; i++)
            {
                if (i + DungeonScrollBar.Value >= DungeonSearchResults.Count)
                {
                    DungeonRows[i].Visible = false;
                    continue;
                }

                DungeonRows[i].InstanceInfo = DungeonSearchResults[i + DungeonScrollBar.Value];
            }

        }

        public void JoinInstance()
        {
            if (SelectedDungeonRow == null)
            {
                return;
            }

            if (GameScene.Game.MapControl.InstanceInfo != null)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.DungeonAlreadyInInstance, MessageType.System);
                return;
            }

            var instance = SelectedDungeonRow.InstanceInfo;

            if (instance.ConnectRegion == null)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.DungeonRegionNotConfigured, MessageType.System);
            }

            if (instance.MinPlayerLevel > 0 && MapObject.User.Level < instance.MinPlayerLevel || instance.MaxPlayerLevel > 0 && MapObject.User.Level > instance.MaxPlayerLevel)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.DungeonNotCorrectLevel, MessageType.System);
                return;
            }

            if (instance.Type == InstanceType.Solo)
            {
                CEnvir.Enqueue(new C.JoinInstance { Index = SelectedDungeonRow.InstanceInfo.Index });
            }
            else if (instance.Type == InstanceType.Group)
            {
                if (GameScene.Game.GroupBox.Members.Count == 0)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.DungeonInGroup, MessageType.System);
                    return;
                }

                if (instance.MinPlayerCount > 1 && (GameScene.Game.GroupBox.Members.Count < instance.MinPlayerCount))
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.DungeonNotEnoughPeople, MessageType.System);
                    return;
                }

                if (instance.MaxPlayerCount > 1 && (GameScene.Game.GroupBox.Members.Count > instance.MaxPlayerCount))
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.DungeonTooManyPeople, MessageType.System);
                    return;
                }

                DXMessageBox box = new DXMessageBox("Your group will be teleported to the instance. Are you ready?", "Instance Confirmation", DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.JoinInstance { Index = SelectedDungeonRow.InstanceInfo.Index });
                };

                return;
            }
            else if (instance.Type == InstanceType.Guild)
            {
                if (GameScene.Game.GuildBox.GuildInfo == null)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.DungeonInGuild, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.JoinInstance { Index = SelectedDungeonRow.InstanceInfo.Index });
            }
            else if (instance.Type == InstanceType.Castle)
            {
                if (GameScene.Game.GuildBox.GuildInfo == null || !GameScene.Game.CastleOwners.Any(x => x.Value == GameScene.Game.GuildBox.GuildInfo.GuildName))
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.DungeonInGuild, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.JoinInstance { Index = SelectedDungeonRow.InstanceInfo.Index });
            }
        }


        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

            }
        }

        #endregion
    }


    public sealed class DungeonRow : DXControl
    {
        #region Properties

        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region InstanceInfo

        public InstanceInfo InstanceInfo
        {
            get => _InstanceInfo;
            set
            {
                if (_InstanceInfo == value) return;

                InstanceInfo oldValue = _InstanceInfo;
                _InstanceInfo = value;

                OnInstanceInfoChanged(oldValue, value);
            }
        }
        private InstanceInfo _InstanceInfo;
        public event EventHandler<EventArgs> StoreInfoChanged;
        public void OnInstanceInfoChanged(InstanceInfo oValue, InstanceInfo nValue)
        {
            Visible = true;

            NameLabel.Text = InstanceInfo.Name;
            TypeLabel.Text = InstanceInfo.Type.ToString();
            LevelLabel.Text = $"Level: {GetLevel(InstanceInfo)}";
            CountLabel.Text = $"Player Count: {GetPlayerCount(InstanceInfo)}";
            //FreeSlotLabel.Text = $"Slots: 0 / {InstanceInfo.MaxInstances}";

            if (GameScene.Game.DungeonFinderBox.SelectedDungeonRow == this)
                GameScene.Game.DungeonFinderBox.SelectedDungeonRow = null;

            StoreInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        private string GetLevel(InstanceInfo instance)
        {
            if (instance.MinPlayerLevel == 0 && instance.MaxPlayerLevel == 0)
            {
                return "Any";
            }
            else if (instance.MinPlayerLevel > 0 && instance.MaxPlayerLevel == 0)
            {
                return $"{InstanceInfo.MinPlayerLevel}+";
            }
            else
            {
                return $"{InstanceInfo.MinPlayerLevel} - {InstanceInfo.MaxPlayerLevel}";
            }
        }

        private string GetPlayerCount(InstanceInfo instance)
        {
            if (instance.MaxPlayerCount == 0)
            {
                return "Any";
            }

            return $"{InstanceInfo.MinPlayerCount} - {InstanceInfo.MaxPlayerCount}";
        }

        #endregion

        public DXLabel NameLabel, TypeLabel, LevelLabel, CountLabel;
        public DXButton FavouriteImage;

        #endregion

        public DungeonRow()
        {
            Size = new Size(515, 40);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(20, 12),
                IsControl = false,
            };

            TypeLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(150, 12),
                IsControl = false,
            };

            LevelLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(250, 12),
                IsControl = false,
            };

            CountLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(350, 12),
                IsControl = false,
            };

            //FreeSlotLabel = new DXLabel
            //{
            //    Parent = this,
            //    Location = new Point(350, 12),
            //    IsControl = false,
            //};

            FavouriteImage = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 6570,
                Parent = this,
                Hint = "Favourite (NOT YET ENABLED)",
                Enabled = false,
                Visible = false,
            };
            FavouriteImage.Location = new Point(Size.Width - FavouriteImage.Size.Width - 10, (Size.Height - FavouriteImage.Size.Height) / 2);

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _InstanceInfo = null;
                StoreInfoChanged = null;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (TypeLabel != null)
                {
                    if (!TypeLabel.IsDisposed)
                        TypeLabel.Dispose();

                    TypeLabel = null;
                }

                if (CountLabel != null)
                {
                    if (!CountLabel.IsDisposed)
                        CountLabel.Dispose();

                    CountLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                //if (FreeSlotLabel != null)
                //{
                //    if (!FreeSlotLabel.IsDisposed)
                //        FreeSlotLabel.Dispose();

                //    FreeSlotLabel = null;
                //}

                if (FavouriteImage != null)
                {
                    if (!FavouriteImage.IsDisposed)
                        FavouriteImage.Dispose();

                    FavouriteImage = null;
                }
            }

        }

        #endregion
    }

}
