using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class QuestDialog : DXImageControl
    {
        #region Properties

        public DXTabControl TabControl;
        public QuestTab AvailableTab, CurrentTab, CompletedTab;
        public MilestoneTab MilestoneTab;
        public MissionTab MissionTab;

        public DXLabel TitleLabel;
        public DXButton CloseButton;

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
        public WindowType Type => WindowType.QuestBox;

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

        public QuestDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 291;
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
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.QuestDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            TabControl = new DXTabControl
            {
                Parent = this,
                Size = new Size(732, 459),
                Location = new Point(0, 21),
                MarginLeft = 18,
            };

            CurrentTab = new QuestTab
            {
                TabButton = { Label = { Text = CEnvir.Language.QuestDialogCurrentTab } },
                Parent = TabControl,
                Border = false,
                ChoiceGrid = { ReadOnly = true },
                AbandonButton = { Visible = true },
                BackColour = Color.Empty,
                Location = new Point(0, 22)
            };
            CurrentTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 291;
            };

            AvailableTab = new QuestTab
            {
                TabButton = { Label = { Text = CEnvir.Language.QuestDialogAvailableTab } },
                Parent = TabControl,
                Border = false,
                ShowTrackerBox = { Visible = false },
                BackColour = Color.Empty,
                Location = new Point(0, 22)
            };
            AvailableTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 291;
            };

            CompletedTab = new QuestTab
            {
                TabButton = { Label = { Text = CEnvir.Language.QuestDialogCompletedTab }, Visible = false },
                Parent = TabControl,
                Border = false,
                ShowTrackerBox = { Visible = false },
                BackColour = Color.Empty,
                Location = new Point(0, 22)
            };
            CompletedTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 291;
            };

            MilestoneTab = new MilestoneTab
            {
                TabButton = { Label = { Text = "Milestones" }, },
                Parent = TabControl,
                Border = false,
                BackColour = Color.Empty,
                Location = new Point(0, 22),
            };
            MilestoneTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 292;
                MilestoneTab.Update();
            };

            MissionTab = new MissionTab
            {
                TabButton = { Label = { Text = "Mission" } },
                Parent = TabControl,
                Border = false,
                BackColour = Color.Empty,
                Location = new Point(0, 22)
            };
            MissionTab.TabButton.MouseClick += (o, e) =>
            {
                Index = 293;
            };
        }

        #region Methods

        public void QuestChanged(ClientUserQuest quest)
        {
            if (AvailableTab.SelectedQuest?.QuestInfo == quest.Quest)
                AvailableTab.UpdateQuestDisplay();

            if (CurrentTab.SelectedQuest?.QuestInfo == quest.Quest)
                CurrentTab.UpdateQuestDisplay();

            if (CompletedTab.SelectedQuest?.QuestInfo == quest.Quest)
                CompletedTab.UpdateQuestDisplay();
        }

        public void CancelQuest(QuestInfo quest)
        {
            bool available = false, current = false, completed = false;

            ClientUserQuest userQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == quest);

            if (userQuest == null) return;

            if (CompletedTab.Quests.Contains(quest))
            {
                CompletedTab.Quests.Remove(quest);
                completed = true;
            }

            if (CurrentTab.Quests.Contains(quest))
            {
                CurrentTab.Quests.Remove(quest);
                current = true;
            }

            if (GameScene.Game.CanAccept(quest))
            {
                if (!AvailableTab.Quests.Contains(quest))
                {
                    AvailableTab.Quests.Add(quest);
                    available = true;
                }
            }

            GameScene.Game.QuestLog.Remove(userQuest);

            if (available)
            {
                AvailableTab.NeedUpdate = true;
                AvailableTab.UpdateQuestDisplay();
            }
            if (current)
            {
                CurrentTab.NeedUpdate = true;
                CurrentTab.UpdateQuestDisplay();
            }
            if (completed)
            {
                CompletedTab.NeedUpdate = true;
                CompletedTab.UpdateQuestDisplay();
            }
        }

        public void PopulateQuests()
        {
            bool available = false, current = false, completed = false;
            foreach (QuestInfo quest in Globals.QuestInfoList.Binding)
            {
                ClientUserQuest userQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == quest);

                if (userQuest == null)
                {
                    if (!GameScene.Game.CanAccept(quest)) continue;

                    if (AvailableTab.Quests.Contains(quest)) continue;

                    AvailableTab.Quests.Add(quest);
                    available = true;
                    continue;
                }


                if (AvailableTab.Quests.Contains(quest))
                {
                    AvailableTab.Quests.Remove(quest);
                    available = true;
                }

                if (userQuest.Completed)
                {
                    if (CompletedTab.Quests.Contains(quest)) continue;

                    CompletedTab.Quests.Add(quest);
                    completed = true;

                    if (!CurrentTab.Quests.Contains(quest)) continue;

                    CurrentTab.Quests.Remove(quest);
                    current = true;

                    continue;
                }

                if (CurrentTab.Quests.Contains(quest)) continue;

                CurrentTab.Quests.Add(quest);
                current = true;
            }


            if (available)
            {
                AvailableTab.NeedUpdate = true;
                AvailableTab.UpdateQuestDisplay();
            }
            if (current)
            {
                CurrentTab.NeedUpdate = true;
                CurrentTab.UpdateQuestDisplay();
            }
            if (completed)
            {
                CompletedTab.NeedUpdate = true;
                CompletedTab.UpdateQuestDisplay();
            }
        }

        public void RefreshMilestones()
        {
            if (!Visible) return;

            if (TabControl.SelectedTab == MilestoneTab)
            {
                MilestoneTab.Update();
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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

                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (AvailableTab != null)
                {
                    if (!AvailableTab.IsDisposed)
                        AvailableTab.Dispose();

                    AvailableTab = null;
                }

                if (CurrentTab != null)
                {
                    if (!CurrentTab.IsDisposed)
                        CurrentTab.Dispose();

                    CurrentTab = null;
                }

                if (CompletedTab != null)
                {
                    if (!CompletedTab.IsDisposed)
                        CompletedTab.Dispose();

                    CompletedTab = null;
                }
            }

        }

        #endregion
    }

    #region Quest

    public sealed class QuestTab : DXTab
    {
        #region Properties

        #region NeedUpdate

        public bool NeedUpdate
        {
            get => _NeedUpdate;
            set
            {
                if (_NeedUpdate == value) return;

                bool oldValue = _NeedUpdate;
                _NeedUpdate = value;

                OnNeedUpdateChanged(oldValue, value);
            }
        }
        private bool _NeedUpdate;
        public event EventHandler<EventArgs> NeedUpdateChanged;
        public void OnNeedUpdateChanged(bool oValue, bool nValue)
        {
            NeedUpdateChanged?.Invoke(this, EventArgs.Empty);

            if (!NeedUpdate) return;

            if (!IsVisible) return;

            UpdateQuestTree();
        }

        #endregion

        #region SelectedQuest

        public QuestTreeEntry SelectedQuest
        {
            get => _SelectedQuest;
            set
            {
                QuestTreeEntry oldValue = _SelectedQuest;
                _SelectedQuest = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private QuestTreeEntry _SelectedQuest;
        public event EventHandler<EventArgs> SelectedQuestChanged;
        public void OnSelectedQuestChanged(QuestTreeEntry oValue, QuestTreeEntry nValue)
        {
            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);

            foreach (DXItemCell cell in RewardGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
                cell.FixedBorder = false;
                cell.Border = false;
                cell.FixedBorderColour = false;
                cell.BorderColour = Color.Lime;
            }

            if (SelectedQuest?.QuestInfo == null)
            {
                QuestLabel.Text = string.Empty;
                TasksLabel.Text = string.Empty;
                DescriptionLabel.Text = string.Empty;

                EndLabel.Text = string.Empty;
                StartLabel.Text = string.Empty;

                AbandonButton.Enabled = false;
                return;
            }

            int standard = 0, choice = 0;

            foreach (QuestReward reward in SelectedQuest.QuestInfo.Rewards)
            {
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                UserItemFlags flags = UserItemFlags.None;
                TimeSpan duration = TimeSpan.FromSeconds(reward.Duration);

                if (reward.Bound)
                    flags |= UserItemFlags.Bound;

                if (duration != TimeSpan.Zero)
                    flags |= UserItemFlags.Expirable;

                ClientUserItem item = new ClientUserItem(reward.Item, reward.Amount)
                {
                    Flags = flags,
                    ExpireTime = duration
                };

                if (reward.Choice)
                {
                    if (choice >= ChoiceGrid.Grid.Length) continue;

                    ChoiceGrid.Grid[choice].Item = item;

                    if (SelectedQuest.UserQuest?.SelectedReward == reward.Index)
                    {
                        ChoiceGrid.Grid[choice].Border = true;
                        ChoiceGrid.Grid[choice].FixedBorder = true;
                        ChoiceGrid.Grid[choice].FixedBorderColour = true;
                        ChoiceGrid.Grid[choice].BorderColour = Color.Lime;
                    }
                    choice++;
                }
                else
                {
                    if (standard >= RewardGrid.Grid.Length) continue;

                    RewardGrid.Grid[standard].Item = item;

                    standard++;
                }
            }

            QuestLabel.Text = SelectedQuest.QuestInfo.QuestName;
            DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, true);
            TasksLabel.Text = GameScene.Game.GetTaskText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            AbandonButton.Enabled = true;

            int height = DXLabel.GetHeight(DescriptionLabel, DescriptionLabel.Size.Width).Height;

            DescriptionLabel.Size = new Size(DescriptionContainer.Size.Width, height);
            DescriptionScrollBar.MaxValue = DescriptionLabel.Size.Height - DescriptionContainer.Size.Height + 14;

            EndLabel.Text = SelectedQuest.QuestInfo.FinishNPC.RegionName;
            StartLabel.Text = SelectedQuest.QuestInfo.StartNPC.RegionName;

            //SelectedQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public List<QuestInfo> Quests = new List<QuestInfo>();

        public DXVScrollBar ScrollBar, DescriptionScrollBar;

        public DXControl DescriptionContainer;
        public DXLabel QuestLabel, TasksLabel, DescriptionLabel, EndLabel, StartLabel;

        public DXButton AbandonButton;

        public DXItemGrid RewardGrid, ChoiceGrid;

        public ClientUserItem[] RewardArray, ChoiceArray;

        public DXCheckBox ShowTrackerBox;

        public bool HasChoice;

        public QuestTree Tree;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (!IsVisible || !NeedUpdate) return;

            UpdateQuestTree();
        }

        #endregion

        public QuestTab()
        {
            int width = 380;

            Tree = new QuestTree
            {
                Parent = this,
                Border = false,
                Location = new Point(10, 8),
                Size = new Size(355, 380),
                ScrollBar = {
                    BackColour = Color.Empty,
                    Border = false,
                    UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                    DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                    PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface }
                }
            };

            Tree.SelectedEntryChanged += (o, e) => SelectedQuest = Tree.SelectedEntry;

            QuestLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(width, 9)
            };

            DXLabel label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabDetailsLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width, 28)
            };

            ShowTrackerBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.QuestTabShowQuestTrackerLabel },
                Parent = this,
                Checked = Config.QuestTrackerVisible,
            };
            ShowTrackerBox.Location = new Point(width + 340 - ShowTrackerBox.Size.Width, 9);
            ShowTrackerBox.CheckedChanged += (o, e) =>
            {
                Config.QuestTrackerVisible = ShowTrackerBox.Checked;
                GameScene.Game.QuestTrackerBox.PopulateQuests();
            };

            DescriptionContainer = new DXControl
            {
                Parent = this,
                Size = new Size(313, 81),
                Location = new Point(width + 3, label.Location.Y + label.Size.Height + 5)
            };

            DescriptionLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(313, 81),
                Border = false,
                ForeColour = Color.White,
                Location = new Point(0, 0),
                Parent = DescriptionContainer,
            };

            DescriptionScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(20, 115),
                Location = new Point(width + 319, 26),
                VisibleSize = 6,
                Change = 1,
                Border = false,
                BackColour = Color.Empty,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
            };
            DescriptionScrollBar.ValueChanged += DescriptionScrollBar_ValueChanged;
            DescriptionLabel.MouseWheel += DescriptionScrollBar.DoMouseWheel;

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabTasksLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, DescriptionContainer.Location.Y + DescriptionContainer.Size.Height + 9),
            };

            TasksLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(332, 61),
                Border = false,
                ForeColour = Color.White,
                Location = new Point(width + 3, label.Location.Y + label.Size.Height + 5),
                Parent = this,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabRewardsLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, TasksLabel.Location.Y + TasksLabel.Size.Height + 9),
            };

            RewardArray = new ClientUserItem[5];
            RewardGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(width + 2, label.Location.Y + label.Size.Height + 5),
                GridSize = new Size(RewardArray.Length, 1),
                ItemGrid = RewardArray,
                ReadOnly = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabChoiceLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(RewardGrid.Location.X + 13 + RewardGrid.Size.Width, TasksLabel.Location.Y + TasksLabel.Size.Height + 9),
            };

            ChoiceArray = new ClientUserItem[4];
            ChoiceGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(RewardGrid.Location.X + 16 + RewardGrid.Size.Width, label.Location.Y + label.Size.Height + 5),
                GridSize = new Size(ChoiceArray.Length, 1),
                ItemGrid = ChoiceArray,
                ReadOnly = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabStartLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            label.Location = new Point(width + 50 - label.Size.Width, ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 10);

            StartLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(label.Location.X + label.Size.Width - 8, label.Location.Y + (label.Size.Height - 12) / 2),
            };
            StartLabel.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo?.StartNPC?.Region?.Map == null) return;

                GameScene.Game.BigMapBox.Visible = true;
                GameScene.Game.BigMapBox.Opacity = 1F;
                GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo.StartNPC.Region.Map;
                GameScene.Game.BigMapBox.PlayLocatorAnim(SelectedQuest.QuestInfo.StartNPC.Index);
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.QuestTabEndLabel,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, label.Location.Y + label.Size.Height),
            };
            label.Location = new Point(width + 50 - label.Size.Width, ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 10 + label.Size.Height);

            EndLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(label.Location.X + label.Size.Width - 8, label.Location.Y + (label.Size.Height - 12) / 2),
            };
            EndLabel.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo?.FinishNPC?.Region?.Map == null) return;

                GameScene.Game.BigMapBox.Visible = true;
                GameScene.Game.BigMapBox.Opacity = 1F;
                GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo.FinishNPC.Region.Map;
                GameScene.Game.BigMapBox.PlayLocatorAnim(SelectedQuest.QuestInfo.FinishNPC.Index);
            };

            AbandonButton = new DXButton
            {
                Parent = this,
                Visible = false,
                Label = { Text = CEnvir.Language.QuestAbandonButtonLabel },
                Location = new Point(640, 398),
                Size = new Size(80, DefaultHeight),
                Enabled = false
            };
            AbandonButton.MouseClick += (o, e) =>
            {
                if (SelectedQuest == null) return;

                DXMessageBox box = new DXMessageBox(CEnvir.Language.QuestAbandonConfirmationMessage, CEnvir.Language.QuestAbandonConfirmationCaption, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.QuestAbandon { Index = SelectedQuest.UserQuest.Index });
                };
            };
        }

        #region Methods

        private void DescriptionScrollBar_ValueChanged(object sender, EventArgs e)
        {
            int y = -DescriptionScrollBar.Value;

            DescriptionLabel.Location = new Point(0, 0 + y);
        }

        public void UpdateQuestTree()
        {
            NeedUpdate = false;

            Tree.TreeList.Clear();

            Quests.Sort((x1, x2) =>
            {
                int res = string.Compare(x1.StartNPC.Region.Map.Description, x2.StartNPC.Region.Map.Description, StringComparison.Ordinal);
                if (res == 0)
                    return string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal);

                return res;
            });
            foreach (QuestInfo quest in Quests)
            {
                MapInfo map = quest?.StartNPC?.Region?.Map;

                if (map == null) continue;

                List<QuestInfo> quests;
                if (!Tree.TreeList.TryGetValue(map, out quests))
                    Tree.TreeList[map] = quests = new List<QuestInfo>();

                quests.Add(quest);
            }

            Tree.ListChanged();
        }

        public void UpdateQuestDisplay()
        {
            if (SelectedQuest == null)
            {
                TasksLabel.Text = string.Empty;
                return;
            }

            TasksLabel.Text = SelectedQuest?.QuestInfo == null ? string.Empty : GameScene.Game.GetTaskText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            if (SelectedQuest != null)
                SelectedQuest.QuestInfo = SelectedQuest.QuestInfo; // Refresh icons
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Quests.Clear();
                Quests = null;

                RewardArray = null;
                ChoiceArray = null;

                HasChoice = false;

                _SelectedQuest = null;
                SelectedQuestChanged = null;

                _NeedUpdate = false;
                NeedUpdateChanged = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (DescriptionScrollBar != null)
                {
                    if (!DescriptionScrollBar.IsDisposed)
                        DescriptionScrollBar.Dispose();

                    DescriptionScrollBar = null;
                }

                if (QuestLabel != null)
                {
                    if (!QuestLabel.IsDisposed)
                        QuestLabel.Dispose();

                    QuestLabel = null;
                }

                if (TasksLabel != null)
                {
                    if (!TasksLabel.IsDisposed)
                        TasksLabel.Dispose();

                    TasksLabel = null;
                }

                if (DescriptionContainer != null)
                {
                    if (!DescriptionContainer.IsDisposed)
                        DescriptionContainer.Dispose();

                    DescriptionContainer = null;
                }

                if (DescriptionLabel != null)
                {
                    if (!DescriptionLabel.IsDisposed)
                        DescriptionLabel.Dispose();

                    DescriptionLabel = null;
                }

                if (EndLabel != null)
                {
                    if (!EndLabel.IsDisposed)
                        EndLabel.Dispose();

                    EndLabel = null;
                }

                if (StartLabel != null)
                {
                    if (!StartLabel.IsDisposed)
                        StartLabel.Dispose();

                    StartLabel = null;
                }

                if (AbandonButton != null)
                {
                    if (!AbandonButton.IsDisposed)
                        AbandonButton.Dispose();

                    AbandonButton = null;
                }

                if (RewardGrid != null)
                {
                    if (!RewardGrid.IsDisposed)
                        RewardGrid.Dispose();

                    RewardGrid = null;
                }

                if (ChoiceGrid != null)
                {
                    if (!ChoiceGrid.IsDisposed)
                        ChoiceGrid.Dispose();

                    ChoiceGrid = null;
                }

                if (ShowTrackerBox != null)
                {
                    if (!ShowTrackerBox.IsDisposed)
                        ShowTrackerBox.Dispose();

                    ShowTrackerBox = null;
                }

                if (Tree != null)
                {
                    if (!Tree.IsDisposed)
                        Tree.Dispose();

                    Tree = null;
                }
            }

        }

        #endregion
    }

    public class QuestTree : DXControl
    {
        #region Properties

        #region SelectedEntry

        public QuestTreeEntry SelectedEntry
        {
            get => _SelectedEntry;
            set
            {
                QuestTreeEntry oldValue = _SelectedEntry;
                _SelectedEntry = value;

                OnSelectedEntryChanged(oldValue, value);
            }
        }
        private QuestTreeEntry _SelectedEntry;
        public event EventHandler<EventArgs> SelectedEntryChanged;
        public virtual void OnSelectedEntryChanged(QuestTreeEntry oValue, QuestTreeEntry nValue)
        {
            SelectedEntryChanged?.Invoke(this, EventArgs.Empty);

            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;
        }

        #endregion

        public Dictionary<MapInfo, List<QuestInfo>> TreeList = new Dictionary<MapInfo, List<QuestInfo>>();

        public DXControl Container;
        public DXVScrollBar ScrollBar;

        public List<DXControl> Lines = new List<DXControl>();

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            ScrollBar.Size = new Size(20, Size.Height);
            ScrollBar.Location = new Point(Size.Width - 20, 0);
            ScrollBar.VisibleSize = Size.Height - 7;

            Container.Size = new Size(Size.Width, Size.Height - 7);
        }

        #endregion

        public QuestTree()
        {
            Container = new DXControl
            {
                Parent = this,
                Location = new Point(0, 2),
            };

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Change = 22,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();

            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods
        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Lines.Count * 22;

            for (int i = 0; i < Lines.Count; i++)
                Lines[i].Location = new Point(Lines[i].Location.X, i * 22 - ScrollBar.Value);
        }

        public void ListChanged()
        {
            QuestInfo selectedQuest = SelectedEntry?.QuestInfo;

            foreach (DXControl control in Lines)
                control.Dispose();

            Lines.Clear();

            _SelectedEntry = null;
            QuestTreeEntry firstEntry = null;

            foreach (KeyValuePair<MapInfo, List<QuestInfo>> pair in TreeList)
            {
                QuestTreeHeader header = new QuestTreeHeader
                {
                    Parent = Container,
                    Location = new Point(1, Lines.Count * 22),
                    Size = new Size(Size.Width - 17, 20),
                    Map = pair.Key
                };
                header.ExpandButton.MouseClick += (o, e) => ListChanged();
                header.MouseWheel += ScrollBar.DoMouseWheel;

                Lines.Add(header);

                if (!pair.Key.Expanded) continue;

                foreach (QuestInfo quest in pair.Value)
                {
                    QuestTreeEntry entry = new QuestTreeEntry
                    {
                        Parent = Container,
                        Location = new Point(1, Lines.Count * 22),
                        Size = new Size(Size.Width - 22, 20),
                        QuestInfo = quest,
                        Selected = quest == selectedQuest,
                    };
                    entry.MouseWheel += ScrollBar.DoMouseWheel;
                    entry.MouseClick += (o, e) =>
                    {
                        SelectedEntry = entry;
                    };

                    if (firstEntry == null)
                        firstEntry = entry;

                    if (entry.Selected)
                        SelectedEntry = entry;

                    entry.TrackBox.CheckedChanged += (o, e) =>
                    {
                        if (entry.UserQuest == null || entry.UserQuest.Track == entry.TrackBox.Checked) return;

                        entry.UserQuest.Track = entry.TrackBox.Checked;

                        CEnvir.Enqueue(new C.QuestTrack { Index = entry.UserQuest.Index, Track = entry.UserQuest.Track });

                        GameScene.Game.QuestTrackerBox.PopulateQuests();
                    };

                    Lines.Add(entry);
                }
            }

            if (SelectedEntry == null)
                SelectedEntry = firstEntry;

            UpdateScrollBar();
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                TreeList.Clear();
                TreeList = null;

                _SelectedEntry = null;
                SelectedEntryChanged = null;

                if (Container != null)
                {
                    if (!Container.IsDisposed)
                        Container.Dispose();

                    Container = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (Lines != null)
                {
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (Lines[i] != null)
                        {
                            if (!Lines[i].IsDisposed)
                                Lines[i].Dispose();

                            Lines[i] = null;
                        }

                    }

                    Lines.Clear();
                    Lines = null;
                }
            }

        }

        #endregion
    }

    public sealed class QuestTreeHeader : DXControl
    {
        #region Properties

        #region Expanded

        public bool Expanded
        {
            get => _Expanded;
            set
            {
                if (_Expanded == value) return;

                bool oldValue = _Expanded;
                _Expanded = value;

                OnExpandedChanged(oldValue, value);
            }
        }
        private bool _Expanded;
        public event EventHandler<EventArgs> ExpandedChanged;
        public void OnExpandedChanged(bool oValue, bool nValue)
        {
            ExpandedChanged?.Invoke(this, EventArgs.Empty);


            ExpandButton.Index = Expanded ? 4871 : 4870;

            Map.Expanded = Expanded;
        }

        #endregion

        #region Map

        public MapInfo Map
        {
            get => _Map;
            set
            {
                if (_Map == value) return;

                MapInfo oldValue = _Map;
                _Map = value;

                OnMapChanged(oldValue, value);
            }
        }
        private MapInfo _Map;
        public event EventHandler<EventArgs> MapChanged;
        public void OnMapChanged(MapInfo oValue, MapInfo nValue)
        {
            Expanded = Map.Expanded;
            MapLabel.Text = Map.PlayerDescription;

            MapChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXButton ExpandButton;
        public DXLabel MapLabel;
        #endregion

        public QuestTreeHeader()
        {
            ExpandButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4870,
                Location = new Point(2, 2)
            };
            ExpandButton.MouseClick += (o, e) => Expanded = !Expanded;

            MapLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(25, 2)
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Expanded = false;
                ExpandedChanged = null;

                _Map = null;
                MapChanged = null;


                if (ExpandButton != null)
                {
                    if (!ExpandButton.IsDisposed)
                        ExpandButton.Dispose();

                    ExpandButton = null;
                }

                if (MapLabel != null)
                {
                    if (!MapLabel.IsDisposed)
                        MapLabel.Dispose();

                    MapLabel = null;
                }
            }

        }

        #endregion
    }

    public sealed class QuestTreeEntry : DXControl
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
            SelectedChanged?.Invoke(this, EventArgs.Empty);
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

        }

        #endregion

        #region QuestInfo

        public QuestInfo QuestInfo
        {
            get => _QuestInfo;
            set
            {
                QuestInfo oldValue = _QuestInfo;
                _QuestInfo = value;

                OnQuestInfoChanged(oldValue, value);
            }
        }
        private QuestInfo _QuestInfo;
        public event EventHandler<EventArgs> QuestInfoChanged;
        public void OnQuestInfoChanged(QuestInfo oValue, QuestInfo nValue)
        {
            UserQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == QuestInfo);

            TrackBox.Visible = false;
            QuestNameLabel.Text = $"[{QuestInfo.QuestType}] {QuestInfo.QuestName}";

            Color colour = Color.White;

            QuestIcon icon = Library.QuestIcon.None;
            QuestType type = QuestType.General;

            if (UserQuest != null)
            {
                type = UserQuest.Quest.QuestType;
                icon = UserQuest.IsComplete ? Library.QuestIcon.Complete : Library.QuestIcon.Incomplete;
            }
            else if (QuestInfo != null)
            {
                type = QuestInfo.QuestType;

                icon = Library.QuestIcon.New;
            }

            int startIndex = 0;

            switch (type)
            {
                case QuestType.General:
                    startIndex = 16;
                    break;
                case QuestType.Daily:
                    startIndex = 76;
                    break;
                case QuestType.Weekly:
                    startIndex = 76;
                    break;
                case QuestType.Repeatable:
                    startIndex = 16;
                    break;
                case QuestType.Story:
                    startIndex = 56;
                    break;
                case QuestType.Account:
                    startIndex = 36;
                    break;
            }

            switch (icon)
            {
                case Library.QuestIcon.New:
                    startIndex += 0;
                    break;
                case Library.QuestIcon.Incomplete:
                    startIndex = 2;
                    break;
                case Library.QuestIcon.Complete:
                    startIndex += 2;
                    break;
            }

            QuestIcon.BaseIndex = startIndex;
            QuestNameLabel.Location = new Point(40, 2);

            //QuestNameLabel.Location = new Point(65, 2);

            TrackBox.Checked = UserQuest != null && UserQuest.Track;

            QuestInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region UserQuest

        public ClientUserQuest UserQuest
        {
            get => _UserQuest;
            set
            {
                ClientUserQuest oldValue = _UserQuest;
                _UserQuest = value;

                OnUserQuestChanged(oldValue, value);
            }
        }
        private ClientUserQuest _UserQuest;
        public event EventHandler<EventArgs> UserQuestChanged;
        public void OnUserQuestChanged(ClientUserQuest oValue, ClientUserQuest nValue)
        {
            UserQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXCheckBox TrackBox;

        public DXAnimatedControl QuestIcon;
        public DXLabel QuestNameLabel;

        #endregion

        public QuestTreeEntry()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);
            QuestIcon = new DXAnimatedControl
            {
                Parent = this,
                Location = new Point(20, 2),
                Loop = true,
                LibraryFile = LibraryFile.QuestIcon,
                BaseIndex = 83,
                FrameCount = 2,
                AnimationDelay = TimeSpan.FromSeconds(1),
                IsControl = false,
            };

            TrackBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(45, 3),
            };


            QuestNameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(65, 2),
                IsControl = false,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _QuestInfo = null;
                QuestInfoChanged = null;

                _UserQuest = null;
                UserQuestChanged = null;


                if (TrackBox != null)
                {
                    if (!TrackBox.IsDisposed)
                        TrackBox.Dispose();

                    TrackBox = null;
                }

                if (QuestIcon != null)
                {
                    if (!QuestIcon.IsDisposed)
                        QuestIcon.Dispose();

                    QuestIcon = null;
                }

                if (QuestNameLabel != null)
                {
                    if (!QuestNameLabel.IsDisposed)
                        QuestNameLabel.Dispose();

                    QuestNameLabel = null;
                }

            }

        }

        #endregion
    }

    #endregion

    #region Milestone

    public sealed class MilestoneTab : DXTab
    {
        public MilestoneMenu Menu;
        public DXLabel ActiveTitle;

        public DXTextBox SearchTextBox;

        public DXCheckBox HideCompleteCheckBox;
        public DXButton ResetTitleButton;

        public List<MilestoneContainer> Items = [];

        private bool Initialized;
        private bool HideComplete;
        private string SearchText;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            CEnvir.Enqueue(new C.MilestoneNotify { Receive = IsVisible });

            base.OnIsVisibleChanged(oValue, nValue);
        }

        #region Selected

        public MilestoneContainer SelectedCategory
        {
            get => _SelectedCategory;
            private set
            {
                MilestoneContainer oldValue = _SelectedCategory;
                _SelectedCategory = value;

                OnSelectedCategoryChanged(oldValue, value);
            }
        }
        private MilestoneContainer _SelectedCategory;
        public event EventHandler<EventArgs> SelectedCategoryChanged;
        public void OnSelectedCategoryChanged(MilestoneContainer oValue, MilestoneContainer nValue)
        {
            oValue?.Visible = false;

            SelectedCategoryChanged?.Invoke(this, EventArgs.Empty);

            nValue?.Update(HideComplete, SearchText);

            nValue?.Visible = true;
        }

        #endregion

        public MilestoneTab()
        {
            Menu = new MilestoneMenu
            {
                Parent = this,
                Location = new Point(13, 40),
            };
            Menu.SelectedChanged += (o, e) =>
            {
                SelectedCategory = Menu.GetAndUpdateSelected(HideComplete, SearchText);
            };

            ActiveTitle = new DXLabel
            {
                Parent = this,
                Size = new Size(165, 18),
                Location = new Point(12, 12),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                ForeColour = Color.White
            };

            SearchTextBox = new DXTextBox
            {
                Parent = this,
                Location = new Point(10, 401),
                Size = new Size(140, 20),
            };
            SearchTextBox.TextBox.TextChanged += (o, e) =>
            {
                SearchText = SearchTextBox.TextBox.Text;

                Update();
            };

            HideCompleteCheckBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(155, 402),
                Label = { Text = "Hide Complete" },
            };
            HideCompleteCheckBox.CheckedChanged += (o, e) =>
            {
                HideComplete = HideCompleteCheckBox.Checked;

                Update();
            };

            ResetTitleButton = new DXButton
            {
                Parent = this,
                Label = { Text = "Reset Title" },
                Location = new Point(640, 398),
                Size = new Size(80, DefaultHeight),
                Enabled = false
            };
            ResetTitleButton.MouseClick += (o, e) =>
            {
                var activeMilestone = GameScene.Game.User.Milestones.First(x => x.Active);
                if (activeMilestone == null) return;
                CEnvir.Enqueue(new C.MilestoneActive { Index = activeMilestone.Index, Active = false });
            };
        }
                
        #region Methods

        private void Add()
        {
            Add("All Milestones", Globals.MilestoneInfoList.Binding);

            var grouped = Globals.MilestoneInfoList.Binding.GroupBy(x => x.Category);

            foreach (var group in grouped.OrderBy(x => x.Key))
            {
                Add(group.Key, group.ToList());
            }
        }

        private void Add(string category, IList<MilestoneInfo> info)
        {
            var page = new MilestoneContainer(info)
            {
                Title = category,
                Parent = this,
                Size = new Size(536, 385),
                Location = new Point(188, 5),
                Visible = false,
            };

            Items.Add(page);

            Menu.Add(page);
        }

        public void Update()
        {
            if (!Initialized)
            {
                Add();
                Initialized = true;
            }

            ResetTitleButton.Enabled = GameScene.Game.User.Milestones.Any(x => x.Active);

            ActiveTitle.Text = GameScene.Game.User.Milestones.FirstOrDefault(x => x.Active)?.Info.Title ?? string.Empty;

            if (SelectedCategory != null)
            {
                SelectedCategory.Update(HideComplete, SearchText);
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

    public sealed class MilestoneMenu : DXControl
    {
        #region Properties

        public DXVScrollBar MenuScrollBar;

        #region Selected

        public DXButton Selected
        {
            get => _Selected;
            private set
            {
                DXButton oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private DXButton _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(DXButton oValue, DXButton nValue)
        {
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            if (oValue != null)
            {
                oValue.Label.ForeColour = Constants.InactiveTabColour;
                oValue.Index = 521;
                oValue.HoverIndex = 521;
            }

            if (nValue != null)
            {
                nValue.Label.ForeColour = Constants.ActiveTabColour;
                nValue.Index = 522;
                nValue.HoverIndex = 522;
            }
        }

        #endregion

        #endregion

        public Dictionary<DXButton, MilestoneContainer> Items = [];

        private const int ButtonHeight = 26;

        public MilestoneMenu()
        {
            Size = new Size(165, 345);

            MenuScrollBar = new DXVScrollBar
            {
                Parent = this,
                BackColour = Color.Empty,
                Location = new Point(145, 0),
                Size = new Size(20, 345),
                MinValue = 0,
                VisibleSize = Size.Height,
                Change = ButtonHeight,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true,
                Border = false,
                Visible = false
            };

            MenuScrollBar.ValueChanged += (o, e) => UpdateLocations(-MenuScrollBar.Value);
        }

        #region Methods

        public void UpdateLocations(int value)
        {
            int y = value + 3;

            foreach (DXControl control in Controls)
            {
                if (control is DXButton)
                {
                    control.Location = new Point(0, y);
                    y += ButtonHeight;
                }
            }
        }

        public void Add(MilestoneContainer page)
        {
            int y = (Items.Count * ButtonHeight) + 3;

            var button = new DXButton
            {
                Index = 521,
                HoverIndex = 522,
                PressedIndex = 522,
                LibraryFile = LibraryFile.GameInter2,
                Size = new Size(164, 24),
                Parent = this,
                Location = new Point(2, y),
                Label = { Text = page.Title, ForeColour = Constants.InactiveTabColour }
            };
            button.MouseClick += (o, e) =>
            {
                Selected = (DXButton)o;
            };

            button.MouseWheel += MenuScrollBar.DoMouseWheel;

            Items.Add(button, page);

            if (Items.Count == 1)
            {
                Selected = button;
            }

            MenuScrollBar.MaxValue = Items.Count * ButtonHeight;
        }

        public MilestoneContainer GetAndUpdateSelected(bool hideComplete, string searchText)
        {
            if (Items.TryGetValue(Selected, out MilestoneContainer page))
            {
                page.Update(hideComplete, searchText);
                return page;
            }

            return null;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (MenuScrollBar != null)
                {
                    if (!MenuScrollBar.IsDisposed)
                        MenuScrollBar.Dispose();

                    MenuScrollBar = null;
                }

                _Selected = null;

                foreach (KeyValuePair<DXButton, MilestoneContainer> pair in Items)
                {
                    if (pair.Value != null)
                    {
                        if (!pair.Value.IsDisposed)
                            pair.Value.Dispose();
                    }

                    if (pair.Key != null)
                    {
                        if (!pair.Key.IsDisposed)
                            pair.Key.Dispose();
                    }
                }

                Items.Clear();
                Items = null;
            }
        }

        #endregion
    }

    public sealed class MilestoneContainer : DXControl
    {
        #region Properties

        public string Title { get; set; }

        public DXVScrollBar ScrollBar;

        #endregion

        public bool HideComplete;

        public Dictionary<MilestoneInfo, MilestoneItem> Items = [];

        public MilestoneContainer(IList<MilestoneInfo> infos)
        {
            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                BackColour = Color.Empty,
                Location = new Point(516, 0),
                Size = new Size(20, 385),
                VisibleSize = 5,
                Change = 1,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = false,
            };

            MouseWheel += ScrollBar.DoMouseWheel;

            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            CreateItems(infos);
        }

        #region Methods

        public void CreateItems(IList<MilestoneInfo> infos)
        {
            int k = 0;

            foreach (var info in infos)
            {
                switch (GameScene.Game.User.Class)
                {
                    case MirClass.Warrior:
                        if ((info.RequiredClass & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((info.RequiredClass & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((info.RequiredClass & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((info.RequiredClass & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                MilestoneItem cell = new MilestoneItem(info)
                {
                    Parent = this,
                    Location = new Point(0, k),
                };
                Items[info] = cell;
                cell.MouseWheel += ScrollBar.DoMouseWheel;

                k += 95;
            }

            ScrollBar.Value = 0;
        }

        public void Update(bool hideComplete, string searchText)
        {
            foreach (DXControl control in Controls)
            {
                if (control is not MilestoneItem item) continue;

                item.Update();
                item.Visible = (!hideComplete || !item.IsComplete) && 
                    (string.IsNullOrEmpty(searchText) || 
                    item.Info.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) || 
                    item.Info.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            UpdateLocations();
        }

        public void UpdateLocations()
        {
            int y = -(ScrollBar.Value * 95) + 4;
            int h = 0;

            int count = 0;

            foreach (DXControl control in Controls)
            {
                if (control is not MilestoneItem item) continue;

                if (!item.Visible) continue;

                count++;

                control.Location = new Point(0, y);
                h += control.Size.Height;
                y += control.Size.Height + 5;
            }

            ScrollBar.MaxValue = count + 1;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                foreach (KeyValuePair<MilestoneInfo, MilestoneItem> pair in Items)
                {
                    if (pair.Value != null)
                    {
                        if (!pair.Value.IsDisposed)
                            pair.Value.Dispose();
                    }
                }

                Items.Clear();
                Items = null;
            }
        }

        #endregion
    }

    public sealed class MilestoneItem : DXControl
    {
        public MilestoneInfo Info;

        public DXCheckBox ActiveCheckbox;

        public DXImageControl BackgroundImage, NoRewardImage;
        public DXLabel CategoryLabel, TitleLabel, DescriptionLabel, DateAchievedLabel, GradeLabel;

        public DXLabel RequirementLabel;

        public DXItemCell ItemCell;

        public bool IsComplete => GameScene.Game.User.Milestones.FirstOrDefault(x => x.Info == Info)?.IsComplete ?? false;

        public MilestoneItem(MilestoneInfo info)
        {
            Size = new Size(516, 90);

            Info = info;

            BackgroundImage = new DXImageControl
            {
                Parent = this,
                Size = Size,
                Index = 510,
                Location = Point.Empty,
                LibraryFile = LibraryFile.GameInter2,
                IsControl = false,
                FixedSize = true
            };

            ActiveCheckbox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(22, 7)
            };
            ActiveCheckbox.CheckedChanged += (o, e) =>
            {
                var userMilestone = GameScene.Game.User.Milestones.FirstOrDefault(x => x.Info == Info);
                if (userMilestone == null) return;

                userMilestone.Active = ActiveCheckbox.Checked;
                CEnvir.Enqueue(new C.MilestoneActive { Index = userMilestone.Index, Active = userMilestone.Active });
            };

            CategoryLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(35, 7),
                ForeColour = Color.White,
                Font = new Font(Config.FontName, CEnvir.FontSize(7F)),
                IsControl = false
            };

            GradeLabel  = new DXLabel
            {
                Parent = this,
                Location = new Point(8, 75),
                Size = new Size (75, 14),
                AutoSize = false,
                IsControl = false,
                ForeColour = Color.White,
                Font = new Font(Config.FontName, CEnvir.FontSize(7F)),
                DrawFormat = TextFormatFlags.HorizontalCenter
            };

            TitleLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(120, 14),
                Size  = new Size(288, 17),
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                IsControl = false,
            };

            DescriptionLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(74, 32),
                Size = new Size(380, 43),
                AutoSize = false,
                ForeColour = Color.White,
                Font = new Font(Config.FontName, CEnvir.FontSize(7F)),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix,
                IsControl = false,
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(26, 33),
                FixedBorder = true,
                Size = new Size(38, 38),
                FixedBorderColour = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = true
            };
            ItemCell.MouseClick += (o, e) =>
            {
                var userMilestone = GameScene.Game.User.Milestones.FirstOrDefault(x => x.Info == Info);

                if (userMilestone == null || !userMilestone.IsComplete) return;

                DXSoundManager.Play(SoundIndex.ItemDefault);
                CEnvir.Enqueue(new C.MilestoneClaim { Index = userMilestone.Index });
            };

            NoRewardImage = new DXImageControl
            {
                Parent = this,
                Location = new Point(26, 33),
                Index = 520,
                LibraryFile = LibraryFile.GameInter2,
                Visible = false
            };

            RequirementLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(75, 70),
                Size = new Size(370, 35),
                AutoSize = false,
                ForeColour = Color.White,
                Font = new Font(Config.FontName, CEnvir.FontSize(7F)),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                IsControl = false,
                Visible = true
            };

            DateAchievedLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(438, 70),
                Size = new Size(70, 15),
                AutoSize = false,
                ForeColour = Color.Goldenrod,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                IsControl = false
            };

            CategoryLabel.Text = Info.Category;
            TitleLabel.Text = Info.Title;
            DescriptionLabel.Text = Info.Description;
            ItemCell.ItemGrid[0] = Info.Reward != null ? new ClientUserItem(Info.Reward, Info.RewardAmount) : null;
            ItemCell.GrayScale = true;
            NoRewardImage.Visible = Info.Reward == null;
            GradeLabel.Text = $"[{Info.Grade}]";
            GradeLabel.OutlineColour = Info.OutlineColour;
        }

        public void Update()
        {
            if (GameScene.Game.User == null) return;

            var userMilestone = GameScene.Game.User.Milestones.FirstOrDefault(x => x.Info == Info);

            var started = userMilestone != null;
            var earned = userMilestone != null && userMilestone.IsComplete;

            Color foreColor = Color.White;

            if (earned)
            {
                BackgroundImage.Index = 510;
                DateAchievedLabel.Text = userMilestone.DateEarned.ToString("yyyy.MM.dd");
                ActiveCheckbox.Enabled = true;
                ItemCell.GrayScale = false;
                ActiveCheckbox.SetSilentState(userMilestone.Active);
                ItemCell.Visible = Info.Reward != null && !userMilestone.Claimed;
                NoRewardImage.Visible = Info.Reward == null || userMilestone.Claimed;
                foreColor = Color.White;

                if (!string.IsNullOrEmpty(Info.Task))
                {
                    RequirementLabel.Text = $"{Info.Task}: Complete";
                }
                else
                {
                    RequirementLabel.Text = "Complete";
                }
            }
            else if (started)
            {
                BackgroundImage.Index = 511;
                DateAchievedLabel.Text = string.Empty;
                ActiveCheckbox.Enabled = false;
                foreColor = Color.LightGray;

                var currentCount = userMilestone.Tasks.Sum(x => x.Count);
                var totalCount = Info.Tasks.Sum(x => x.Amount);

                if (Info.ShowCount && totalCount > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Info.Task))
                    {
                        RequirementLabel.Text = $"{Info.Task}: {currentCount}/{totalCount}";
                    }
                    else
                    {
                        RequirementLabel.Text = $"Tasks: {currentCount}/{totalCount}";
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(Info.Task))
                    {
                        RequirementLabel.Text = Info.Task;
                    }
                    else if (currentCount > 0)
                    {
                        RequirementLabel.Text = "In Progress";
                    }
                    else
                    {
                        RequirementLabel.Text = "Not Started";
                    }
                }
            }

            TitleLabel.ForeColour = foreColor;
            DescriptionLabel.ForeColour = foreColor;
            CategoryLabel.ForeColour = foreColor;
            GradeLabel.ForeColour = foreColor;
            RequirementLabel.ForeColour = foreColor;
        }
    }

    #endregion

    #region Mission

    public sealed class MissionTab : DXTab
    {
        public DXLabel TitleLabel;

        public MissionTab()
        {

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }
            }
        }

        #endregion
    }

    #endregion

    public sealed class MilestoneAchievedDialog : DXControl
    {
        public DXLabel Label, Title;

        public DXImageControl Background, LeftEnd, RightEnd;
        public DXAnimatedControl Animation;

        public MilestoneAchievedDialog()
        {
            Size = new Size(640, 480);
            IsControl = false;

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter2, out var GameInter2Library);

            var backgroundSize = GameInter2Library.GetSize(500);

            Background = new DXImageControl
            {
                Parent = this,
                Index = 294,
                LibraryFile = LibraryFile.Interface,
                Location = new Point((Size.Width - backgroundSize.Width) / 2, (Size.Height - backgroundSize.Height) / 2),
                IsControl = false
            };

            Label = new DXLabel
            {
                Parent = this,
                Size = new Size(150, 25),
                Text = "Title Achieved",
                Font = new Font("MS Sans Serif", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                ForeColour = Color.FromArgb(122, 60, 55),
                Outline = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                IsControl = false
            };
            Label.Location = new Point(((Size.Width - Label.Size.Width) / 2) + 20, ((Size.Height - Label.Size.Height) / 2) - 15);

            LeftEnd = new DXImageControl
            {
                Parent = this,
                Index = 501,
                LibraryFile = LibraryFile.GameInter2,
                IsControl = false
            };

            RightEnd = new DXImageControl
            {
                Parent = this,
                Index = 502,
                LibraryFile = LibraryFile.GameInter2,
                IsControl = false
            };

            Animation = new DXAnimatedControl
            {
                Parent = this,
                Animated = false,
                Loop = false,
                LibraryFile = LibraryFile.GameInter2,
                AnimationDelay = TimeSpan.FromMilliseconds(2000),
                Location = new Point(295, 224),
                BaseIndex = 550,
                FrameCount = 19,
                Blend = true,
                IsControl = false,
                UseOffSet = true
            };
            Animation.AfterAnimation += (o, e) =>
            {
                Visible = false;
            };

            Title = new DXLabel
            {
                Parent = this,
                Size = new Size(100, 25),
                AutoSize = true,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                IsControl = false
            };
        }

        public void Show(MilestoneInfo info)
        {
            Visible = true;
            Title.Text = info.Title;
            Title.OutlineColour = info.OutlineColour;

            Title.Location = new Point(((Size.Width - Title.Size.Width) / 2) + 20, ((Size.Height - Title.Size.Height) / 2) + 10);
            LeftEnd.Location = new Point(Title.Location.X - LeftEnd.Size.Width + 15, Title.Location.Y - 5);
            RightEnd.Location = new Point(Title.Location.X + Title.Size.Width - 15, Title.Location.Y - 5);

            Animation.AnimationStart = DateTime.MinValue;
            Animation.Animated = true;

            DXSoundManager.Play(SoundIndex.QuestComplete);
        }
    }
}