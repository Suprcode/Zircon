using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class ChatTab : DXTab
    {
        #region Properties
        public static List<ChatTab> Tabs = new List<ChatTab>();


        public DXImageControl AlertIcon;

        public ChatOptionsPanel Panel;
        public DXControl TextPanel;

        public DXVScrollBar ScrollBar;

        public List<DXLabel> History = new List<DXLabel>();

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            // DrawTexture = true;
            if (ScrollBar == null || TextPanel == null) return;

            TextPanel.Location = new Point(ResizeBuffer, ResizeBuffer);
            TextPanel.Size = new Size(Size.Width - ScrollBar.Size.Width - 1 - ResizeBuffer*2, Size.Height - ResizeBuffer*2);

            ScrollBar.VisibleSize = TextPanel.Size.Height;
            ScrollBar.Location = new Point(Size.Width - ScrollBar.Size.Width - ResizeBuffer, ResizeBuffer);
            ScrollBar.Size = new Size(14, Size.Height - ResizeBuffer*2);

            if (!IsResizing)
                ResizeChat();
            
        }
        public override void OnIsResizingChanged(bool oValue, bool nValue)
        {
            ResizeChat();


            base.OnIsResizingChanged(oValue, nValue);
        }
        public override void OnSelectedChanged(bool oValue, bool nValue)
        {
            base.OnSelectedChanged(oValue, nValue);

            if (Panel == null || CurrentTabControl == null || CurrentTabControl.SelectedTab != this) return;

            float opacity = Panel.TransparentCheckBox.Checked ? 0.5F : 1F;

            foreach (DXButton button in CurrentTabControl.TabButtons)
                button.Opacity = opacity;
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible && AlertIcon != null)
                AlertIcon.Visible = false;
        }

        #endregion

        public ChatTab()
        {
            Opacity = 0.5F;
            DrawOtherBorder = true;


            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(14, Size.Height),
                VisibleSize = Size.Height,
            };
            ScrollBar.Location = new Point(Size.Width - ScrollBar.Size.Width - ResizeBuffer , 0);
            ScrollBar.ValueChanged += (o, e) => UpdateItems();


            TextPanel = new DXControl
            {
                Parent = this,
                PassThrough = true,
                Location = new Point(ResizeBuffer, ResizeBuffer),
                Size = new Size(Size.Width - ScrollBar.Size.Width - 1 - ResizeBuffer * 2, Size.Height - ResizeBuffer * 2),
            };

            AlertIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 240,
                Parent = TabButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };

            MouseWheel += ScrollBar.DoMouseWheel;
            Tabs.Add(this);
        }

        #region Methods

        public void ResizeChat()
        {
            if (!IsResizing)
            {
                foreach (DXLabel label in History)
                {
                    if (label.Size.Width == TextPanel.Size.Width) continue;

                    Size size = DXLabel.GetHeight(label, TextPanel.Size.Width);
                    label.Size = new Size(size.Width, size.Height);

                    //label.Size = new Size(TextPanel.Size.Width, DXLabel.GetHeight(label, TextPanel.Size.Width).Height);
                }

                UpdateItems();
                UpdateScrollBar();
            }
        }
        public void UpdateItems()
        {
            int y = -ScrollBar.Value;

            foreach (DXLabel control in History)
            {
                control.Location = new Point(0, y);
                y += control.Size.Height;
            }

        }

        public void UpdateScrollBar()
        {
            ScrollBar.VisibleSize = TextPanel.Size.Height;

            int height = 0;

            foreach (DXLabel control in History)
                height += control.Size.Height;

            ScrollBar.MaxValue = height;
        }

        public void ReceiveChat(string message, MessageType type)
        {
            if (Panel == null) return;

            switch (type)
            {
                case MessageType.Normal:
                    if (!Panel.LocalCheckBox.Checked) return;
                    break;
                case MessageType.Shout:
                    if (!Panel.ShoutCheckBox.Checked) return;
                    break;
                case MessageType.Global:
                    if (!Panel.GlobalCheckBox.Checked) return;
                    break;
                case MessageType.Group:
                    if (!Panel.GroupCheckBox.Checked) return;
                    break;
                case MessageType.Hint:
                    if (!Panel.HintCheckBox.Checked) return;
                    break;
                case MessageType.System:
                    if (!Panel.SystemCheckBox.Checked) return;
                    break;
                case MessageType.WhisperIn:
                case MessageType.WhisperOut:
                    if (!Panel.WhisperCheckBox.Checked) return;
                    break;
                case MessageType.Combat:
                    if (!Panel.GainsCheckBox.Checked) return;
                    break;
                case MessageType.ObserverChat:
                    if (!Panel.ObserverCheckBox.Checked) return;
                    break;
                case MessageType.Guild:
                    if (!Panel.GuildCheckBox.Checked) return;
                    break;
            }


            DXLabel label = new DXLabel
            {
                AutoSize = false,
                Text = message,
                Outline = false,
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,
                Parent = TextPanel,
            };
            label.MouseWheel += ScrollBar.DoMouseWheel;
            label.Tag = type;

            switch (type)
            {
                case MessageType.Normal:
                case MessageType.Shout:
                case MessageType.Global:
                case MessageType.WhisperIn:
                case MessageType.WhisperOut:
                case MessageType.Group:
                case MessageType.ObserverChat:
                case MessageType.Guild:
                    label.MouseUp += (o, e) =>
                    {
                        string[] parts = label.Text.Split(':', ' ');
                        if (parts.Length == 0) return;

                        GameScene.Game.ChatTextBox.StartPM(Regex.Replace(parts[0], "[^A-Za-z0-9]", ""));
                    };
                    break;
            }

            UpdateColours(label);

            Size size = DXLabel.GetHeight(label, TextPanel.Size.Width);
            label.Size = new Size(size.Width, size.Height);

            History.Add(label);

            while (History.Count > 250)
            {
                DXLabel oldLabel = History[0];
                History.Remove(oldLabel);
                oldLabel.Dispose();
            }

            AlertIcon.Visible = !IsVisible && Panel.AlertCheckBox.Checked;

            bool update = ScrollBar.Value >= ScrollBar.MaxValue - ScrollBar.VisibleSize;

            UpdateScrollBar();

            if (update)
            {
                ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
            }
            else UpdateItems();
        }
        public void ReceiveChat(MessageAction action, params object[] args)
        {
            DXLabel label;
            switch (action)
            {
                case MessageAction.Revive:
                    label = new DXLabel
                    {
                        AutoSize = false,
                        Text = "You have died, Click here to revive in town.",
                        Outline = false,
                        DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,
                        Parent = TextPanel,
                    };
                    label.MouseClick += (o, e) =>
                    {
                        CEnvir.Enqueue(new C.TownRevive());
                        label.Dispose();
                    };
                    label.MouseWheel += ScrollBar.DoMouseWheel;
                    label.Disposing += (o, e) =>
                    {
                        if (IsDisposed) return;

                        History.Remove(label);
                        UpdateScrollBar();
                        ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
                    };
                    label.Tag = MessageType.Announcement;
                    break;
                default:
                    return;
            }

            UpdateColours(label);

            Size size = DXLabel.GetHeight(label, TextPanel.Size.Width);
            label.Size = new Size(size.Width, size.Height);

            History.Add(label);

            while (History.Count > 250)
            {
                DXLabel oldLabel = History[0];
                History.Remove(oldLabel);
                oldLabel.Dispose();
            }

            AlertIcon.Visible = !IsVisible && Panel.AlertCheckBox.Checked;

            bool update = ScrollBar.Value >= ScrollBar.MaxValue - ScrollBar.VisibleSize;

            UpdateScrollBar();


            if (update)
            {
                ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
            }
            else UpdateItems();
        }
        public void UpdateColours()
        {
            foreach (DXLabel label in History)
                UpdateColours(label);
        }
        private void UpdateColours(DXLabel label)
        {
            Color empty = Panel?.TransparentCheckBox.Checked == true ? Color.FromArgb(100, 0, 0, 0) : Color.Empty;

            switch ((MessageType)label.Tag)
            {
                case MessageType.Normal:
                    label.BackColour = empty;
                    label.ForeColour = Config.LocalTextColour;
                    break;
                case MessageType.Shout:
                    label.BackColour = empty;
                    label.ForeColour = Config.ShoutTextColour;
                    break;
                case MessageType.Group:
                    label.BackColour = empty;
                    label.ForeColour = Config.GroupTextColour;
                    break;
                case MessageType.Global:
                    label.BackColour = empty;
                    label.ForeColour = Config.GlobalTextColour;
                    break;
                case MessageType.Hint:
                    label.BackColour = empty;
                    label.ForeColour = Config.HintTextColour;
                    break;
                case MessageType.System:
                    label.BackColour = Color.FromArgb(200, 255, 255, 255);
                    label.ForeColour = Config.SystemTextColour;
                    break;
                case MessageType.Announcement:
                    label.BackColour = Color.FromArgb(200, 255, 255, 255);
                    label.ForeColour = Config.AnnouncementTextColour;
                    break;
                case MessageType.WhisperIn:
                    label.BackColour = empty;
                    label.ForeColour = Config.WhisperInTextColour;
                    break;
                case MessageType.GMWhisperIn:
                    label.BackColour = Color.FromArgb(200, 255, 255, 255);
                    label.ForeColour = Config.GMWhisperInTextColour;
                    break;
                case MessageType.WhisperOut:
                    label.BackColour = empty;
                    label.ForeColour = Config.WhisperOutTextColour;
                    break;
                case MessageType.Combat:
                    label.BackColour = empty;
                    label.ForeColour = Config.GainsTextColour;
                    break;
                case MessageType.ObserverChat:
                    label.BackColour = empty;
                    label.ForeColour = Config.ObserverTextColour;
                    break;
                case MessageType.Guild:
                    label.BackColour = empty;
                    label.ForeColour = Config.GuildTextColour;
                    break;
            }


        }
        public void TransparencyChanged()
        {
            if (Panel.TransparentCheckBox.Checked)
            {
                ScrollBar.Visible = false;
                DrawTexture = false;
                DrawOtherBorder = false;
                AllowResize = false;

                if (CurrentTabControl.SelectedTab == this)
                    foreach (DXButton button in CurrentTabControl.TabButtons)
                        button.Opacity = 0.5f;


                foreach (DXLabel label in History)
                    UpdateColours(label);
            }
            else
            {
                ScrollBar.Visible = true;
                DrawTexture = true;
                AllowResize = true;
                DrawOtherBorder = true;

                if (CurrentTabControl.SelectedTab == this)
                    foreach (DXButton button in CurrentTabControl.TabButtons)
                        button.Opacity = 1f;
                
                foreach (DXLabel label in History)
                    UpdateColours(label);
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Tabs.Remove(this);

                if (Panel != null)
                {
                    if (!Panel.IsDisposed)
                        Panel.Dispose();

                    Panel = null;
                }

                if (TextPanel != null)
                {
                    if (!TextPanel.IsDisposed)
                        TextPanel.Dispose();

                    TextPanel = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (History != null)
                {
                    for (int i = 0; i < History.Count; i++)
                    {
                        if (History[i] != null)
                        {
                            if (!History[i].IsDisposed)
                                    History[i].Dispose();

                            History[i] = null;
                        }
                    }
                    
                    History.Clear();
                    History = null;
                }

                if (AlertIcon != null)
                {
                    if (!AlertIcon.IsDisposed)
                        AlertIcon.Dispose();

                    AlertIcon = null;
                }
            }

        }

        #endregion
    }
}
