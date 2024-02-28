using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class GroupDialog : DXImageControl
    {
        #region Properties

        public DXLabel TitleLabel;
        public DXButton CloseButton, AddButton, RemoveButton, LFGButton, OptionsButton;
        public DXCheckBox AllowGroupBox;

        public DXLabel LFGNameLabel, LFGStatusLabel;
        public DXVScrollBar LFGScrollBar;
        private DateTime LFGRequestDelay;

        public GroupLFGRow[] LFGRows;

        #region AllowGroup

        public bool AllowGroup
        {
            get => _AllowGroup;
            set
            {
                if (_AllowGroup == value) return;

                bool oldValue = _AllowGroup;
                _AllowGroup = value;

                OnAllowGroupChanged(oldValue, value);
            }
        }
        private bool _AllowGroup;
        public event EventHandler<EventArgs> AllowGroupChanged;
        public void OnAllowGroupChanged(bool oValue, bool nValue)
        {
            AllowGroupChanged?.Invoke(this, EventArgs.Empty);

            if (AllowGroup)
            {
                AllowGroupBox.Label.Text = CEnvir.Language.GroupDialogAllowGroupButtonAllowingHint;
            }
            else
            {
                AllowGroupBox.Label.Text = CEnvir.Language.GroupDialogAllowGroupButtonNotAllowingHint;
            }

            AllowGroupBox.SetSilentState(AllowGroup);
            AllowGroupBox.Location = new Point(230 - AllowGroupBox.Size.Width, 40);
        }

        #endregion

        public DXTab MemberTab;

        public List<ClientPlayerInfo> Members = new List<ClientPlayerInfo>();

        public List<DXLabel> Labels = new List<DXLabel>();

        #region SelectedLabel

        public DXLabel SelectedLabel
        {
            get => _SelectedLabel;
            set
            {
                if (_SelectedLabel == value) return;

                DXLabel oldValue = _SelectedLabel;
                _SelectedLabel = value;

                OnSelectedLabelChanged(oldValue, value);
            }
        }
        private DXLabel _SelectedLabel;
        public event EventHandler<EventArgs> SelectedLabelChanged;
        public void OnSelectedLabelChanged(DXLabel oValue, DXLabel nValue)
        {
            if (oValue != null)
            {
                oValue.ForeColour = Color.White;
                oValue.BackColour = Color.FromArgb(24, 16, 16);
            }

            if (nValue != null)
            {
                nValue.ForeColour = Color.LimeGreen;
                nValue.BackColour = Color.Empty;
            }

            RemoveButton.Enabled = nValue != null && Members[0].ObjectID == GameScene.Game.User.ObjectID;

            SelectedLabelChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        #region SelectedLFG

        public GroupLFGRow SelectedLFG
        {
            get => _SelectedLFG;
            set
            {
                if (_SelectedLFG == value) return;

                GroupLFGRow oldValue = _SelectedLFG;
                _SelectedLFG = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private GroupLFGRow _SelectedLFG;
        public event EventHandler<EventArgs> SelectedLFGChanged;
        public void OnSelectedQuestChanged(GroupLFGRow oValue, GroupLFGRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (SelectedLFG != null)
            {
                SelectedLFG.Selected = true;
            }

            SelectedLFGChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
                BringToFront();

            if (Settings != null)
                Settings.Visible = nValue;

            base.OnIsVisibleChanged(oValue, nValue);
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

        #endregion

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.GroupBox;

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

        public GroupDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 240;
            Movable = true;
            Sort = true;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.GroupDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(240 - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            AllowGroupBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.GroupDialogAllowGroupButtonNotAllowingHint },
                Parent = this,
                Checked = false,
            };
            AllowGroupBox.Location = new Point(230 - AllowGroupBox.Size.Width, 40);
            AllowGroupBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.GroupSwitch { Allow = !AllowGroup });
            };

            DXTabControl tab = new DXTabControl
            {
                Parent = this,
                Size = new Size(214, 148),
                Location = new Point(13, 60)
            };

            MemberTab = new DXTab
            {
                TabButton =
                {
                    Label =
                    {
                        Text = CEnvir.Language.GroupDialogMemberTabLabel
                    },
                    Visible = false
                },
                Parent = tab,
                Border = false,
                BackColour = Color.Empty
            };
            MemberTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 240;
            };

            AddButton = new DXButton
            {
                Size = new Size(36, 36),
                ButtonType = ButtonType.AddButton,
                Location = new Point(35, 217),
                Parent = this,
                Hint = CEnvir.Language.GroupDialogAddButtonHint
            };
            AddButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.GroupMemberLimit, MessageType.System);
                    return;
                }

                if (Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.GroupNotLeader, MessageType.System);
                    return;
                }

                DXInputWindow window = new DXInputWindow(CEnvir.Language.GroupDialogAddButtonConfirmMessage, CEnvir.Language.GroupDialogAddButtonConfirmCaption)
                {
                    ConfirmButton = { Enabled = false },
                    Modal = true
                };
                window.ValueTextBox.TextBox.TextChanged += (o1, e1) =>
                {
                    window.ConfirmButton.Enabled = Globals.CharacterReg.IsMatch(window.ValueTextBox.TextBox.Text);
                };
                window.ConfirmButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GroupInvite { Name = window.Value });
                };
            };

            RemoveButton = new DXButton
            {
                Size = new Size(36, 36),
                ButtonType = ButtonType.RemoveButton,
                Location = new Point(81, 217),
                Parent = this,
                Enabled = false,
                Hint = CEnvir.Language.GroupDialogRemoveButtonHint
            };
            RemoveButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.GroupRemove { Name = SelectedLabel.Text });
            };

            LFGButton = new DXButton
            {
                Size = new Size(36, 36),
                ButtonType = ButtonType.LFGButton,
                Location = new Point(127, 217),
                Parent = this,
                Hint = CEnvir.Language.GroupDialogCreateLFGButtonHint
            };
            LFGButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                var lfg = GameScene.Game.GroupBox.LFGList.FirstOrDefault(x => x.LeaderName == GameScene.Game.User.Name);

                GroupLFGInputWindow window = new GroupLFGInputWindow()
                {
                    EnableButton = { Enabled = false },
                    Modal = true
                };
                window.NameTextBox.TextBox.TextChanged += (o1, e1) =>
                {
                    var length = window.NameTextBox.TextBox.Text.Length;

                    window.EnableButton.Enabled = length >= 2 && length <= 16;
                };
                window.EnableButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GroupLFGUpdate { Name = window.NameValue, MaxCount = window.CountValue, Type = window.TypeValue, Enabled = true });
                };
                window.DisableButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GroupLFGUpdate { Name = window.NameValue, MaxCount = window.CountValue, Type = window.TypeValue, Enabled = false });
                };

                if (lfg != null)
                {
                    window.EnableButton.Label.Text = "Update";

                    window.NameTextBox.TextBox.Text = lfg.GroupName;
                    window.CountNumberBox.Value = lfg.MaxCount;
                    window.TypeComboBox.ListBox.SelectItem(lfg.GroupType);
                    window.DisableButton.Enabled = true;
                }
                else
                {
                    window.EnableButton.Label.Text = "Enable";
                    window.DisableButton.Enabled = false;
                }
            };

            OptionsButton = new DXButton
            {
                Size = new Size(36, 36),
                ButtonType = ButtonType.OptionsButton,
                Location = new Point(173, 217),
                Parent = this,
                Enabled = false,
                Hint = "Settings"
            };

            VisibleChanged += GroupDialog_VisibleChanged;

            LFGNameLabel = new DXLabel
            {
                Text = "Group Name",
                Parent = this,
                IsControl = false,
                Size = new Size(101, 20),
                Location = new Point(12, 272),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.FromArgb(198, 166, 99)
            };

            LFGStatusLabel = new DXLabel
            {
                Text = "Status",
                Parent = this,
                IsControl = false,
                Size = new Size(95, 20),
                Location = new Point(114, 272),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                ForeColour = Color.FromArgb(198, 166, 99)
            };

            LFGScrollBar = new DXVScrollBar
            {
                Parent = this,
                VisibleSize = 5,
                Change = 1,
                Size = new Size(24, 140),
                Location = new Point(210, 268),
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface }
            };

            LFGScrollBar.ValueChanged += LFGScrollBar_ValueChanged;

            LFGRows = new GroupLFGRow[5];
            for (int i = 0; i < 5; i++)
            {
                int index = i;
                LFGRows[index] = new GroupLFGRow
                {
                    Parent = this,
                    Location = new Point(13, 293 + (i * 21)),
                    Size = new Size(194, 19),
                    Visible = false
                };
                LFGRows[index].MouseWheel += LFGScrollBar.DoMouseWheel;
                LFGRows[index].MouseClick += LFG_MouseClick;
            }

            UpdateList(new List<ClientGroup>());
        }

        private void LFG_MouseClick(object sender, MouseEventArgs e)
        {
            var row = (GroupLFGRow)sender;

            if (row == null || row.Info == null) return;

            if (!row.Info.Enabled) return;

            if (!AllowGroup)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.GroupDialogNotAllowingGroupMessage, MessageType.System);
                return;
            }

            if (row.Info.MaxCount == row.Info.CurrentCount)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.GroupMemberLimit, MessageType.System);
                return;
            }

            if (row.Info.LeaderName == GameScene.Game.User.Name)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.GroupSelf, MessageType.System);
                return;
            }

            if (Members.Count > 0)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.GroupNotLeader, MessageType.System);
                return;
            }

            if (LFGRequestDelay > CEnvir.Now)
            {
                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GroupLFGRequestDelay, (LFGRequestDelay - CEnvir.Now).Seconds), MessageType.System);
                return;  
            }

            LFGRequestDelay = CEnvir.Now.AddSeconds(30);

            CEnvir.Enqueue(new C.GroupRequest { Name = row.Info.LeaderName });
            GameScene.Game.ReceiveChat(CEnvir.Language.GroupLFGRequestSent, MessageType.System);
        }

        private void RefreshList()
        {
            if (LFGRows == null) return;

            var list = LFGList.Where(x => x.Enabled).OrderBy(x => x.GroupName).ToList();

            LFGScrollBar.MaxValue = list.Count;

            for (int i = 0; i < LFGRows.Length; i++)
            {
                var index = i + LFGScrollBar.Value;

                if (index >= list.Count)
                {
                    LFGRows[index].Info = null;
                    LFGRows[index].RefreshStatus();
                    LFGRows[index].Visible = false;
                    continue;
                }

                LFGRows[i].Info = list[index];
                LFGRows[index].RefreshStatus();
                LFGRows[index].Visible = true;
            }
        }

        public List<ClientGroup> LFGList = new List<ClientGroup>();

        public void UpdateList(List<ClientGroup> lfgList = null)
        {
            if (lfgList != null)
            {
                LFGList = lfgList;
            }

            RefreshList();
        }

        public void UpdateItem(ClientGroup group)
        {
            var lfg = LFGList.FirstOrDefault(x => x.LeaderName == group.LeaderName);

            if (lfg != null)
            {
                LFGList.Remove(lfg);
            }

            LFGList.Add(group);

            RefreshList();
        }

        private void LFGScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void GroupDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                UpdateList();
            }

            CEnvir.Enqueue(new C.GroupNotify { Receive = Visible });
        }

        #region Methods
        public void UpdateMembers()
        {
            SelectedLabel = null;

            foreach (DXLabel label in Labels)
                label.Dispose();

            Labels.Clear();

            for (int i = 0; i < Members.Count; i++)
            {
                ClientPlayerInfo member = Members[i];

                DXLabel label = new DXLabel
                {
                    Parent = MemberTab,
                    Location = new Point(10 + 100 * (i % 2), 5 + 20 * (i / 2)),
                    Text = member.Name,
                    ForeColour = Color.White
                };
                label.MouseClick += (o, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                        SelectedLabel = label;
                    else if (e.Button == MouseButtons.Right)
                    {
                        GameScene.Game.BigMapBox.Visible = true;
                        GameScene.Game.BigMapBox.Opacity = 1F;

                        if (!GameScene.Game.DataDictionary.TryGetValue(member.ObjectID, out ClientObjectData data)) return;

                        GameScene.Game.BigMapBox.SelectedInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == data.MapIndex);
                    }
                };

                Labels.Add(label);
            }

            AddButton.Enabled = Members.Count == 0 || Members[0].ObjectID == GameScene.Game.User.ObjectID;
            LFGButton.Enabled = Members.Count == 0 || Members[0].ObjectID == GameScene.Game.User.ObjectID;
            GameScene.Game.GroupHealthBox.UpdateMembers();
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _AllowGroup = false;
                AllowGroupChanged = null;

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (AllowGroupBox != null)
                {
                    if (!AllowGroupBox.IsDisposed)
                        AllowGroupBox.Dispose();

                    AllowGroupBox = null;
                }

                if (AddButton != null)
                {
                    if (!AddButton.IsDisposed)
                        AddButton.Dispose();

                    AddButton = null;
                }

                if (RemoveButton != null)
                {
                    if (!RemoveButton.IsDisposed)
                        RemoveButton.Dispose();

                    RemoveButton = null;
                }

                if (MemberTab != null)
                {
                    if (!MemberTab.IsDisposed)
                        MemberTab.Dispose();

                    MemberTab = null;
                }

                for (int i = 0; i < Labels.Count; i++)
                {
                    if (Labels[i] != null)
                    {
                        if (!Labels[i].IsDisposed)
                            Labels[i].Dispose();

                        Labels[i] = null;
                    }
                }
                Labels.Clear();
                Labels = null;

                if (_SelectedLabel != null)
                {
                    if (!_SelectedLabel.IsDisposed)
                        _SelectedLabel.Dispose();

                    _SelectedLabel = null;
                }

                SelectedLabelChanged = null;

                Members.Clear();
                Members = null;
            }

        }

        #endregion
    }

    public sealed class GroupHealthDialog : DXWindow
    {
        public List<DXLabel> Labels = new();
        public List<DXControl> HealthBars = new();

        public GroupHealthDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0.0F;
            AllowResize = false;
            Movable = false;
            Border = true;
            IsControl = false;

            Size = new Size(150, 500);
        }

        public override WindowType Type => WindowType.None;

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => true;

        public void UpdateMembers()
        {
            foreach (DXLabel label in Labels)
                label.Dispose();

            foreach (DXControl healthBar in HealthBars)
                healthBar.Dispose();

            Labels.Clear();

            HealthBars.Clear();

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out MirLibrary barLibrary);

            int index = 0;

            for (int i = 0; i < GameScene.Game.GroupBox.Members.Count; i++)
            {
                ClientPlayerInfo member = GameScene.Game.GroupBox.Members[i];

                if (member.Name == GameScene.Game.User.Name) continue;

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Location = new Point(15, 10 + 30 * index),
                    Text = member.Name,
                    ForeColour = Color.White,
                    Tag = member.ObjectID
                };

                Labels.Add(label);

                var healthBar = new DXControl
                {
                    Parent = this,
                    Location = new Point(15, 30 + 30 * index),
                    Size = barLibrary.GetSize(52),
                    Tag = label
                };
                healthBar.BeforeDraw += (o, e) =>
                {
                    if (barLibrary == null) return;

                    var nameLabel = (DXLabel)((DXControl)o).Tag;
                    var objectID = (uint)nameLabel.Tag;

                    if (!GameScene.Game.DataDictionary.TryGetValue(objectID, out ClientObjectData data)) return;

                    MirImage backImage = barLibrary.CreateImage(316, ImageType.Image);

                    PresentTexture(backImage.Image, this, new Rectangle(healthBar.DisplayArea.X, healthBar.DisplayArea.Y, (int)backImage.Width, backImage.Height), Color.White, healthBar);

                    if (data.Health <= 0)
                    {
                        nameLabel.ForeColour = Color.IndianRed;
                        return;
                    }

                    nameLabel.ForeColour = Color.White;

                    float percent = Math.Min(1, Math.Max(0, data.Health / (float)data.MaxHealth));

                    if (percent == 0) return;

                    MirImage image = barLibrary.CreateImage(315, ImageType.Image);

                    if (image == null) return;

                    PresentTexture(image.Image, this, new Rectangle(healthBar.DisplayArea.X, healthBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, healthBar);
                };

                HealthBars.Add(healthBar);

                index++;
            }
        }
    }

    public sealed class GroupLFGRow : DXControl
    {
        public DXLabel NameLabel;
        public DXLabel StatusLabel, TypeLabel;

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
        private event EventHandler<EventArgs> SelectedChanged;
        private void OnSelectedChanged(bool oValue, bool nValue)
        {
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            if (Selected)
            {
                NameLabel.BackColour = Color.FromArgb(100, 100, 100, 100);
                StatusLabel.BackColour = Color.FromArgb(100, 100, 100, 100);
                TypeLabel.BackColour = Color.FromArgb(100, 100, 100, 100);
            }
            else
            {
                NameLabel.BackColour = Color.Empty;
                StatusLabel.BackColour = Color.Empty;
                TypeLabel.BackColour = Color.Empty;
            }
        }

        #endregion

        #region Info

        public ClientGroup Info
        {
            get => _Info;
            set
            {
                if (_Info == value) return;

                ClientGroup oldValue = _Info;
                _Info = value;

                OnInfoChanged(oldValue, value);
            }
        }
        private ClientGroup _Info;
        private event EventHandler<EventArgs> InfoChanged;
        private void OnInfoChanged(ClientGroup oValue, ClientGroup nValue)
        {
            InfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public GroupLFGRow()
        {
            NameLabel = new DXLabel
            {
                Text = "",
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(0, 0),
                Size = new Size(100, 20),
                AutoSize = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.WordEllipsis,
                IsControl = false
            };

            StatusLabel = new DXLabel
            {
                Text = "",
                Parent = this,
                ForeColour = Color.Lime,
                Location = new Point(101, 0),
                Size = new Size(50, 20),
                AutoSize = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false
            };

            TypeLabel = new DXLabel
            {
                Text = "",
                Parent = this,
                ForeColour = Color.Lime,
                Location = new Point(151, 0),
                Size = new Size(42, 20),
                AutoSize = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false
            };

            MouseEnter += GroupLFGRow_MouseEnter;
            MouseLeave += GroupLFGRow_MouseLeave;
        }

        private void GroupLFGRow_MouseLeave(object sender, EventArgs e)
        {
            Selected = false;
        }

        private void GroupLFGRow_MouseEnter(object sender, EventArgs e)
        {
            Selected = true;
        }

        public void RefreshStatus()
        {
            if (Info == null)
            {
                NameLabel.Text = "";
                StatusLabel.Text = "";
                TypeLabel.Text = "";
            }
            else
            {
                NameLabel.Text = Info.GroupName;
                StatusLabel.Text = $"[{Info.CurrentCount:D2}/{Info.MaxCount:D2}]";
                TypeLabel.Text = Info.GroupType;
                Enabled = true;

                if (GameScene.Game.GroupBox.Members.Count > 1)
                {
                    NameLabel.ForeColour = Color.DarkGray;
                    StatusLabel.ForeColour = Color.DarkGray;
                    TypeLabel.ForeColour = Color.DarkGray;
                }
                else
                {
                    NameLabel.ForeColour = Color.White;
                    StatusLabel.ForeColour = Color.Wheat;
                    TypeLabel.ForeColour = Info.GroupType == "PvE" ? Color.Lime : Color.Red;
                }
            }
        }
    }

    public sealed class GroupLFGInputWindow : DXWindow
    {
        #region Properites
        public DXLabel Label;

        public DXButton EnableButton, DisableButton, CancelButton;
        public DXTextBox NameTextBox;
        public DXNumberBox CountNumberBox;
        public DXComboBox TypeComboBox;

        //Don't Dispose
        public string NameValue;
        public int CountValue = 4;
        public string TypeValue = "PvE";

        public override WindowType Type => WindowType.InputWindow;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public GroupLFGInputWindow()
        {
            HasFooter = true;

            TitleLabel.Text = "Looking For Group";

            Parent = ActiveScene;
            MessageBoxList.Add(this);

            Label = new DXLabel
            {
                AutoSize = false,
                Location = new Point(10, 35),
                Parent = this,
                Text = "Enter the name, size and type of your desired group. Group notifications will last for 1 hour or until you disable it.",
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.HorizontalCenter
            };
            Label.Size = new Size(300, DXLabel.GetHeight(Label, 300).Height);

            NameTextBox = new DXTextBox
            {
                Parent = this,
                Size = new Size(200, 20),
                Location = new Point(60, 45 + Label.Size.Height),
                MaxLength = 16,
                KeepFocus = true,
            };
            NameTextBox.SetFocus();
            NameTextBox.TextBox.TextChanged += TextBox_TextChanged;
            NameTextBox.TextBox.KeyPress += (o, e) => OnKeyPress(e);

            TypeComboBox = new DXComboBox
            {
                Parent = this,
                Size = new Size(100, 16),
                Location = new Point(60, 72 + Label.Size.Height)
            };
            TypeComboBox.SelectedItemChanged += TypeComboBox_SelectedItemChanged;

            _ = new DXListBoxItem
            {
                Parent = TypeComboBox.ListBox,
                Label = { Text = "PvE" },
                Item = "PvE"
            };
            _ = new DXListBoxItem
            {
                Parent = TypeComboBox.ListBox,
                Label = { Text = "PvP" },
                Item = "PvP"
            };
            TypeComboBox.ListBox.SelectItem(TypeValue);

            CountNumberBox = new DXNumberBox
            {
                Parent = this,
                Size = new Size(90, 20),
                Location = new Point(65 + TypeComboBox.Size.Width + 10, 70 + Label.Size.Height),
                MinValue = 2,
                MaxValue = Globals.GroupLimit,
                Value = CountValue,
                Change = 1
            };
            CountNumberBox.ValueTextBox.ValueChanged += ValueTextBox_ValueChanged;

            SetClientSize(new Size(300, 60 + Label.Size.Height));
            Label.Location = ClientArea.Location;

            EnableButton = new DXButton
            {
                Location = new Point((Size.Width) / 3 - 80 - 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Enable" },
            };
            EnableButton.MouseClick += (o, e) => Dispose();

            DisableButton = new DXButton
            {
                Location = new Point((Size.Width) / 3 + 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Disable" }
            };
            DisableButton.MouseClick += (o, e) =>
            {
                Dispose();
            };

            CancelButton = new DXButton
            {
                Location = new Point((Size.Width / 3) * 2 + 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlCancel }
            };
            CancelButton.MouseClick += (o, e) => Dispose();

            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);
        }

        #region Methods
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            e.Handled = true;
        }
        public override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            e.Handled = true;
        }
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            switch (e.KeyChar)
            {
                case (char)Keys.Escape:
                    if (CancelButton != null && !CancelButton.IsDisposed)
                        CancelButton.InvokeMouseClick();
                    break;
                case (char)Keys.Enter:
                    if (EnableButton != null && !EnableButton.IsDisposed)
                        EnableButton.InvokeMouseClick();
                    break;
                default: return;
            }
            e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            NameValue = NameTextBox.TextBox.Text;
        }

        private void ValueTextBox_ValueChanged(object sender, EventArgs e)
        {
            CountValue = (int)CountNumberBox.Value;
        }

        private void TypeComboBox_SelectedItemChanged(object sender, EventArgs e)
        {
            TypeValue = TypeComboBox.SelectedItem.ToString();
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();
                    Label = null;
                }
            }
        }

        #endregion
    }
}