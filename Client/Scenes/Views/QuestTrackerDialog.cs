using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class QuestTrackerDialog : DXWindow
    {
        #region Properties
        public List<DXLabel> Lines = new List<DXLabel>();

        private DXVScrollBar ScrollBar;
        public DXControl TextPanel;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (ScrollBar == null || TextPanel == null) return;

            ScrollBar.Size = new Size(14, Size.Height);
            ScrollBar.Location = new Point(Size.Width - 14, 0);
            ScrollBar.VisibleSize = Size.Height;
            ScrollBar.HideWhenNoScroll = true;

            TextPanel.Location = new Point(0, ResizeBuffer);
            TextPanel.Size = new Size(Size.Width - ScrollBar.Size.Width - 1 - ResizeBuffer , Size.Height - ResizeBuffer * 2);

            ScrollBar.VisibleSize = TextPanel.Size.Height;
            ScrollBar.Location = new Point(Size.Width - ScrollBar.Size.Width - ResizeBuffer, ResizeBuffer);
            ScrollBar.Size = new Size(14, Size.Height - ResizeBuffer * 2);
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            this.Opacity = 0.3F;
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            this.Opacity = 0.0F;
        }


        public override WindowType Type => WindowType.QuestTrackerBox;
        public override bool CustomSize => true;
        public override bool AutomaticVisibility => true;
        #endregion

        public QuestTrackerDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0.0F;
            AllowResize = true;

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Change = 15,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();

            TextPanel = new DXControl
            {
                Parent = this,
                PassThrough = true,
                Location = new Point(ResizeBuffer, ResizeBuffer),
                Size = new Size(Size.Width - ScrollBar.Size.Width - 1 - ResizeBuffer * 2, Size.Height - ResizeBuffer * 2),
            };

            Size = new Size(250, 100);
            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods

        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Lines.Count * 15;

            for (int i = 0; i < Lines.Count; i++)
                Lines[i].Location = new Point(Lines[i].Location.X, i * 15 - ScrollBar.Value);
        }

        public void PopulateQuests()
        {
            foreach (DXLabel line in Lines)
                line.Dispose();

            Lines.Clear();

            if (!Config.QuestTrackerVisible)
            {
                Visible = false;
                return;
            }

            foreach (QuestInfo quest in GameScene.Game.QuestBox.CurrentTab.Quests)
            {
                ClientUserQuest userQuest = GameScene.Game.QuestLog.First(x => x.Quest == quest);

                if (!userQuest.Track) continue;

                DXLabel label = new DXLabel
                {
                    Text = quest.QuestName,
                    Parent = TextPanel,
                    Outline = true,
                    OutlineColour = Color.Black,
                    IsControl = false,
                    Location = new Point(15, Lines.Count * 15)
                };
                

                DXAnimatedControl QuestIcon = new DXAnimatedControl
                {
                    Parent = TextPanel,
                    Location = new Point(2, Lines.Count * 15),
                    Loop = true,
                    LibraryFile = LibraryFile.QuestIcon,
                    BaseIndex = 83,
                    FrameCount = 2,
                    AnimationDelay = TimeSpan.FromSeconds(1),
                    IsControl = false,
                };
                label.Disposing += (o, e) =>
                {
                    QuestIcon.Dispose();
                };

                label.LocationChanged += (o, e) =>
                {
                    QuestIcon.Location = new Point(QuestIcon.Location.X, label.Location.Y);
                };

                var type = userQuest.Quest.QuestType;
                var icon = userQuest.IsComplete ? Library.QuestIcon.Complete : Library.QuestIcon.Incomplete;
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


                if (userQuest.IsComplete)
                {
                    label.Text += " (Complete)";
                }

                Lines.Add(label);

                if (userQuest.IsComplete)
                {
                    DXLabel label1 = new DXLabel
                    {
                        Text = $"Goto {quest.FinishNPC.NPCName} in {quest.FinishNPC.RegionName}",
                        Parent = TextPanel,
                        ForeColour = Color.White,
                        Outline = true,
                        OutlineColour = Color.Black,
                        IsControl = false,
                        Location = new Point(25, Lines.Count * 15)
                    };

                    Lines.Add(label1);
                }
                else
                {
                    foreach (QuestTask task in quest.Tasks)
                    {
                        ClientUserQuestTask userTask = userQuest.Tasks.FirstOrDefault(x => x.Task == task);

                        if (userTask != null && userTask.Completed) continue;

                        DXLabel label1 = new DXLabel
                        {
                            Text = GameScene.Game.GetTaskText(task, userQuest),
                            Parent = TextPanel,
                            ForeColour = Color.White,
                            Outline = true,
                            OutlineColour = Color.Black,
                            IsControl = false,
                            Location = new Point(25, Lines.Count * 15)
                        };

                        Lines.Add(label1);
                    }
                }
            }
            

            Visible = Lines.Count > 0;
            UpdateScrollBar();
        }
        #endregion
        
        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (TextPanel != null)
                {
                    if (!TextPanel.IsDisposed)
                        TextPanel.Dispose();

                    TextPanel = null;
                }
            }

        }

        #endregion
    }
}
