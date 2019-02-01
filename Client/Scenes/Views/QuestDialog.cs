using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class QuestDialog : DXWindow
    {
        #region Properties

        public DXTabControl TabControl;
        public QuestTab AvailableTab, CurrentTab, CompletedTab;

        public override WindowType Type => WindowType.QuestBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;
        #endregion

        public QuestDialog()
        {
            TitleLabel.Text = "Quest Log";

            SetClientSize(new Size(558, 380));

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = ClientArea.Location,
                Size = ClientArea.Size,
            };

            CurrentTab = new QuestTab
            {
                TabButton = { Label = { Text = "Current" } },
                Parent = TabControl,
                Border = true,
                ChoiceGrid = { ReadOnly = true }
            };

            AvailableTab = new QuestTab
            {
                TabButton = { Label = { Text = "Available" } },
                Parent = TabControl,
                Border = true,
                ShowTrackerBox = { Visible = false }
            };


            CompletedTab = new QuestTab
            {
                TabButton = { Label = { Text = "Completed" } },
                Parent = TabControl,
                Border = true,
                ShowTrackerBox = { Visible = false }
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
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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
                TasksLabel.Text = string.Empty;
                DescriptionLabel.Text = string.Empty;
                
                EndLabel.Text = string.Empty;
                StartLabel.Text = string.Empty;
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

            DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, true);
            TasksLabel.Text = GameScene.Game.GetTaskText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            EndLabel.Text = SelectedQuest.QuestInfo.FinishNPC.RegionName;
            StartLabel.Text = SelectedQuest.QuestInfo.StartNPC.RegionName;
            
            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public List<QuestInfo> Quests = new List<QuestInfo>();

        public DXVScrollBar ScrollBar;

        public DXLabel TasksLabel, DescriptionLabel, EndLabel, StartLabel;

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
        

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (Tree == null) return;

            Tree.Size = new Size(240, Size.Height - 10);

        }
        #endregion

        public QuestTab()
        {
            int width = 250;

            Tree = new QuestTree
            {
                Parent = this,
                Location = new Point(5, 5)
            };
            Tree.SelectedEntryChanged += (o, e) => SelectedQuest = Tree.SelectedEntry;


            DXLabel label = new DXLabel
            {
                Text = "Details",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width, 4)
            };

            ShowTrackerBox = new DXCheckBox
            {
                Label = { Text = "Show Quest Tracker" },
                Parent = this,
            };
            ShowTrackerBox.Location = new Point(width + 303 - ShowTrackerBox.Size.Width, 7);
            ShowTrackerBox.CheckedChanged += (o, e) =>
            {
                Config.QuestTrackerVisible = ShowTrackerBox.Checked;
                GameScene.Game.QuestTrackerBox.PopulateQuests();
            };


            DescriptionLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(300 - 4, 80),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                Location = new Point(width + 3, label.Location.Y + label.Size.Height + 5),
                Parent = this,
            };

            label = new DXLabel
            {
                Text = "Tasks",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, DescriptionLabel.Location.Y + DescriptionLabel.Size.Height + 5),
            };


            TasksLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(300 - 4, 80),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                Location = new Point(width + 3, label.Location.Y + label.Size.Height + 5),
                Parent = this,
            };

            label = new DXLabel
            {
                Text = "Rewards",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(width + 0, TasksLabel.Location.Y + TasksLabel.Size.Height + 5),
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
                Text = "Choice",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(RewardGrid.Location.X + 13 + RewardGrid.Size.Width, TasksLabel.Location.Y + TasksLabel.Size.Height + 5),
            };

            ChoiceArray = new ClientUserItem[3];
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
                Text = "Start:",
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
            };

            label = new DXLabel
            {
                Text = "End:",
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
                Location = new Point(label.Location.X + label.Size.Width - 8, label.Location.Y + (label.Size.Height - 12)/2),
            };
            EndLabel.MouseClick += (o, e) =>
            {
                if (SelectedQuest?.QuestInfo?.FinishNPC?.Region?.Map == null) return;

                GameScene.Game.BigMapBox.Visible = true;
                GameScene.Game.BigMapBox.Opacity = 1F;
                GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo.FinishNPC.Region.Map;
            };

        }

        #region Methods

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

                if (TasksLabel != null)
                {
                    if (!TasksLabel.IsDisposed)
                        TasksLabel.Dispose();

                    TasksLabel = null;
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

        private DXVScrollBar ScrollBar;

        public List<DXControl> Lines = new List<DXControl>();
        
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            ScrollBar.Size = new Size(14, Size.Height);
            ScrollBar.Location = new Point(Size.Width - 14, 0);
            ScrollBar.VisibleSize = Size.Height;
        }
        #endregion
        
        public QuestTree()
        {
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

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
                Lines[i].Location = new Point(Lines[i].Location.X, i*22 - ScrollBar.Value);
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
                    Parent = this,
                    Location = new Point(1, Lines.Count*22),
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
                        Parent = this,
                        Location = new Point(1, Lines.Count*22),
                        Size = new Size(Size.Width - 17, 20),
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
                        if (entry.UserQuest.Track == entry.TrackBox.Checked) return;

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
            MapLabel.Text = Map.Description;

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
            QuestNameLabel.Text = QuestInfo.QuestName;

            if (UserQuest == null)
            {
                QuestIcon.BaseIndex = 83; //Available
                QuestNameLabel.Location = new Point(40, 2);
            }
            else if (UserQuest.Completed)
            {
                QuestIcon.BaseIndex = 91; //Current
                QuestNameLabel.Location = new Point(40, 2);
            }
            else if (!UserQuest.IsComplete)
            {
                QuestIcon.BaseIndex = 85; //Completed
                TrackBox.Visible = true;
                QuestNameLabel.Location = new Point(65, 2);
            }
            else
            {
                QuestIcon.BaseIndex = 93; //Current
                TrackBox.Visible = true;
                QuestNameLabel.Location = new Point(65, 2);
            }

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
                LibraryFile = LibraryFile.Interface,
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

}