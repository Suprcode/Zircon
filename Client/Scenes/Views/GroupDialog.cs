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
        public DXButton CloseButton, AddButton, RemoveButton;
        public DXCheckBox AllowGroupBox;

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
                oValue.ForeColour = Color.FromArgb(198, 166, 99);
                oValue.BackColour = Color.Empty;
            }

            if (nValue != null)
            {
                nValue.ForeColour = Color.White;
                nValue.BackColour = Color.FromArgb(24, 16, 16);
            }

            RemoveButton.Enabled = nValue != null && Members[0].ObjectID == GameScene.Game.User.ObjectID;

            SelectedLabelChanged?.Invoke(this, EventArgs.Empty);
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
                Checked = Config.QuestTrackerVisible,
            };
            AllowGroupBox.Location = new Point(230 - AllowGroupBox.Size.Width, 40);
            AllowGroupBox.CheckedChanged += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.GroupSwitch { Allow = !AllowGroup });
            };

            DXTabControl members = new DXTabControl
            {
                Parent = this,
                Size = new Size(218, 146),
                Location = new Point(11, 60),
            };

            MemberTab = new DXTab
            {
                TabButton =
                {
                    Label =
                    {
                        Text = CEnvir.Language.GroupDialogMemberTabLabel
                    },

                    IsControl = false,
                },
                Parent = members,
                Border = false,
                BackColour = Color.Empty
            };

            AddButton = new DXButton
            {
                Size = new Size(36, 36),
                ButtonType = ButtonType.AddButton,
                Location = new Point(30, 217),
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
                Location = new Point(174, 217),
                Parent = this,
                Enabled = false,
                Hint = CEnvir.Language.GroupDialogRemoveButtonHint
            };
            RemoveButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                CEnvir.Enqueue(new C.GroupRemove { Name = SelectedLabel.Text });
            };

            VisibleChanged += GroupDialog_VisibleChanged;
        }

        private void GroupDialog_VisibleChanged(object sender, EventArgs e)
        {
            
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
                    Location = new Point(10 + 100*(i%2), 5 + 20*(i/2)),
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

                        var map = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == data.MapIndex);

                        if (!GameScene.Game.BigMapBox.TryShowMap(map))
                        {
                            return;
                        }

                        GameScene.Game.BigMapBox.SelectedInfo = map;
                    }
                };

                Labels.Add(label);
            }

            AddButton.Enabled = Members.Count == 0 || Members[0].ObjectID == GameScene.Game.User.ObjectID;

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
}
