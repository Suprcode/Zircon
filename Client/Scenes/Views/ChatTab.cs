using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class ChatTab : DXTab
    {
        #region Properties
        public static List<ChatTab> Tabs = new List<ChatTab>();

        public ChatTabPageSetting Settings;

        public DXImageControl AlertIcon;

        public ChatOptionsPanel Panel;
        public DXControl TextPanel;

        public DXVScrollBar ScrollBar;

        public List<DXLabel> ItemLabels = new List<DXLabel>();
        public List<ChatHistory> History = new List<ChatHistory>();

        #region HideChat

        public bool HideChat
        {
            get => _HideChat;
            set
            {
                if (_HideChat == value) return;

                bool oldValue = _HideChat;
                _HideChat = value;

                OnHideChatChanged(oldValue, value);
            }
        }
        private bool _HideChat;
        public event EventHandler<EventArgs> HideChatChanged;
        public void OnHideChatChanged(bool oValue, bool nValue)
        {
            HideChatChanged?.Invoke(this, EventArgs.Empty);

            if (!HideChat)
            {
                chatFade = 1F;

                foreach (var item in History)
                {
                    item.Label.Opacity = chatFade;
                }

                foreach (var label in ItemLabels)
                {
                    label.Opacity = chatFade;
                }
            }
        }

        private float chatFade = 1F;
        private DateTime nextFadeCheck;
        private DateTime nextCleanUpCheck;

        #endregion

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

            if (Panel == null || CurrentTabControl == null || CurrentTabControl.SelectedTab != this || Panel.TransparentCheckBox == null) return;

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
                VisibleSize = Size.Height
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

        public override void Process()
        {
            base.Process();

            FadeOutChatHistory();
            CleanUpChat();
        }

        //TODO - Make more efficient
        private void FadeOutChatHistory()
        {
            if (!Panel.FadeOutCheckBox.Checked && chatFade == 1F)
            {
                return;
            }

            if (HideChat && nextFadeCheck < CEnvir.Now && chatFade > 0F)
            {
                chatFade -= 0.2F;

                chatFade = Math.Max(0F, chatFade);

                foreach (var item in History)
                {
                    item.Label.Opacity = chatFade;
                }

                foreach (var label in ItemLabels)
                {
                    label.Opacity = chatFade;
                }

                nextFadeCheck = CEnvir.Now.AddMilliseconds(100);
                return;
            }

            bool hideChat = false;

            DXTabControl tab = TabButton.Parent as DXTabControl;

            if (tab.SelectedTab == this && !GameScene.Game.ChatOptionsBox.Visible &&
                GameScene.Game.ChatTextBox.TextBox != DXTextBox.ActiveTextBox && 
                Panel.FadeOutCheckBox.Checked && Panel.TransparentCheckBox.Checked)
            {
                var newest = History.LastOrDefault();

                if (newest != null && newest.SentDate < CEnvir.Now.AddSeconds(-10))
                {
                    hideChat = true;
                }
            }

            HideChat = hideChat;
        }

        private void CleanUpChat()
        {
            if (nextCleanUpCheck < CEnvir.Now)
            {
                if (Panel.CleanUpCheckBox.Checked)
                {
                    bool chatCleaned = false;
                    for (int i = History.Count - 1; i >= 0; i--)
                    {
                        var history = History[i];

                        TimeSpan timeDifference = CEnvir.Now - History[i].SentDate;

                        if (timeDifference > TimeSpan.FromSeconds(5) && history.Action == MessageAction.None)
                        {
                            history.Dispose();

                            if (History.Contains(history))
                                History.RemoveAt(i);

                            chatCleaned = true;
                        }
                    }

                    if (chatCleaned)
                    {
                        UpdateItems();
                    }
                }

                nextCleanUpCheck = DateTime.UtcNow.AddSeconds(1);
            }
        }

        public void ResizeChat()
        {
            if (!IsResizing)
            {
                foreach (DXLabel label in History.Select(x => x.Label))
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

        public void UpdateItems(bool updated = true)
        {
            if (Panel == null) return;

            if (Panel.ReverseListCheckBox.Checked)
            {
                if (updated)
                {
                    int y = Size.Height - 20 + ScrollBar.Value;

                    for (int i = 0; i < History.Count; i++)
                    {
                        var label = History[i].Label;
                        y -= label.Size.Height;
                        label.Location = new Point(0, y);
                    }
                }

                ScrollBar.Value = Math.Max(ScrollBar.MinValue, Math.Min(ScrollBar.MaxValue - ScrollBar.VisibleSize, ScrollBar.MaxValue));
            }
            else
            {
                if (updated)
                {
                    int y = -ScrollBar.Value;

                    for (int i = 0; i < History.Count; i++)
                    {
                        var label = History[i].Label;
                        label.Location = new Point(0, y);
                        y += label.Size.Height;
                    }
                }
            }

            if (updated)
            {
                ProcessLinkedItems();
            }
        }

        public void UpdateScrollBar()
        {
            ScrollBar.VisibleSize = TextPanel.Size.Height;

            int height = 0;

            foreach (DXLabel control in History.Select(x => x.Label))
                height += control.Size.Height;

            ScrollBar.MaxValue = height;
        }

        public void ReceiveChat(string message, MessageType type, List<ClientUserItem> linkedItems)
        {
            if (Panel == null) return;

            switch (type)
            {
                case MessageType.Announcement:
                    break;
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
                case MessageType.Debug:
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

            History.Add(new ChatHistory { Message = message, Label = label, LinkedItems = linkedItems, SentDate = CEnvir.Now });

            while (History.Count > 250)
            {
                History[0].Dispose();
                History.RemoveAt(0);
            }

            AlertIcon.Visible = !IsVisible && Panel.AlertCheckBox.Checked;

            bool update = ScrollBar.Value >= ScrollBar.MaxValue - ScrollBar.VisibleSize;

            UpdateScrollBar();

            if (update)
            {
                ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
            }

            UpdateItems(update);
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

                        History.RemoveAll(x => x.Label == label);
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

            History.Add(new ChatHistory { Message = label.Text, Label = label, SentDate = CEnvir.Now, Action = action });

            while (History.Count > 250)
            {
                History[0].Dispose();
                History.RemoveAt(0);
            }

            AlertIcon.Visible = !IsVisible && Panel.AlertCheckBox.Checked;

            bool update = ScrollBar.Value >= ScrollBar.MaxValue - ScrollBar.VisibleSize;

            UpdateScrollBar();

            if (update)
            {
                ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
            }

            UpdateItems(update);
        }
        public void UpdateColours()
        {
            foreach (DXLabel label in History.Select(x => x.Label))
                UpdateColours(label);
        }
        private void UpdateColours(DXLabel label)
        {
            switch ((MessageType)label.Tag)
            {
                case MessageType.Normal:
                    label.BackColour = GetBackColour(Config.LocalTextBackColour);
                    label.ForeColour = Config.LocalTextForeColour;
                    break;
                case MessageType.Shout:
                    label.BackColour = GetBackColour(Config.ShoutTextBackColour);
                    label.ForeColour = Config.ShoutTextForeColour;
                    break;
                case MessageType.Group:
                    label.BackColour = GetBackColour(Config.GroupTextBackColour);
                    label.ForeColour = Config.GroupTextForeColour;
                    break;
                case MessageType.Global:
                    label.BackColour = GetBackColour(Config.GlobalTextBackColour);
                    label.ForeColour = Config.GlobalTextForeColour;
                    break;
                case MessageType.Hint:
                    label.BackColour = GetBackColour(Config.HintTextBackColour);
                    label.ForeColour = Config.HintTextForeColour;
                    break;
                case MessageType.System:
                    label.BackColour = GetBackColour(Config.SystemTextBackColour);
                    label.ForeColour = Config.SystemTextForeColour;
                    break;
                case MessageType.Announcement:
                    label.BackColour = GetBackColour(Config.AnnouncementTextBackColour);
                    label.ForeColour = Config.AnnouncementTextForeColour;
                    break;
                case MessageType.WhisperIn:
                    label.BackColour = GetBackColour(Config.WhisperInTextBackColour);
                    label.ForeColour = Config.WhisperInTextForeColour;
                    break;
                case MessageType.GMWhisperIn:
                    label.BackColour = GetBackColour(Config.GMWhisperInTextBackColour);
                    label.ForeColour = Config.GMWhisperInTextForeColour;
                    break;
                case MessageType.WhisperOut:
                    label.BackColour = GetBackColour(Config.WhisperOutTextBackColour);
                    label.ForeColour = Config.WhisperOutTextForeColour;
                    break;
                case MessageType.Combat:
                    label.BackColour = GetBackColour(Config.GainsTextBackColour);
                    label.ForeColour = Config.GainsTextForeColour;
                    break;
                case MessageType.ObserverChat:
                    label.BackColour = GetBackColour(Config.ObserverTextBackColour);
                    label.ForeColour = Config.ObserverTextForeColour;
                    break;
                case MessageType.Guild:
                    label.BackColour = GetBackColour(Config.GuildTextBackColour);
                    label.ForeColour = Config.GuildTextForeColour;
                    break;
                case MessageType.Debug:
                    label.BackColour = Color.SkyBlue;
                    label.ForeColour = Color.White;
                    break;
            }
        }

        private Color GetBackColour(Color color)
        {
            if (Panel?.TransparentCheckBox.Checked == true && color == Color.FromArgb(0, 0, 0, 0))
            {
                return Color.FromArgb(100, 0, 0, 0);
            }

            return color;
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

                foreach (DXLabel label in History.Select(x => x.Label))
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

                foreach (DXLabel label in History.Select(x => x.Label))
                    UpdateColours(label);
            }
        }

        public void ProcessLinkedItems()
        {
            foreach (DXLabel label in ItemLabels)
            {
                if (!label.IsDisposed)
                    label.Dispose();
            }
            ItemLabels.Clear();

            foreach (ChatHistory history in History)
                ProcessText(history);
        }

        public void ProcessText(ChatHistory history)
        {
            DXLabel label = history.Label;
            List<ClientUserItem> items = history.LinkedItems;
            string message = history.Message;

            label.Text = Globals.LinkedItemRegex.Replace(message, @" [${Text}] ");
            message = Globals.LinkedItemRegex.Replace(message, @" [${Text}:${ID}] ");

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            message = regex.Replace(message, " ");
            label.Text = regex.Replace(label.Text, " ");
            //label.AutoSize = true;

            Size size = DXLabel.GetHeight(label, TextPanel.Size.Width);
            label.Size = new Size(size.Width, size.Height);

            MatchCollection matches = Globals.LinkedItemRegex.Matches(message);
            List<CharacterRange> ranges = new List<CharacterRange>();

            int offset = 1;
            foreach (Match match in matches)
            {
                ranges.Add(new CharacterRange(match.Groups["Text"].Index - offset, match.Groups["Text"].Length + 2));
                offset += 1 + match.Groups["ID"].Length;
            }

            for (int i = 0; i < ranges.Count; i++)
            {
                if (!int.TryParse(matches[i].Groups["ID"].Value, out int index)) continue;
                ClientUserItem item = items.FirstOrDefault(e => e.Index == index);
                if (item == null) continue;

                List<ButtonInfo> buttons = NPCDialog.GetWordRegionsNew(DXManager.Graphics, label.Text, label.Font, label.DrawFormat, label.Size.Width, ranges[i].First, ranges[i].Length);

                List<DXLabel> labels = new List<DXLabel>();

                foreach (ButtonInfo info in buttons)
                {
                    labels.Add(new DXLabel
                    {
                        AutoSize = false,
                        Parent = label,
                        ForeColour = Color.Yellow,
                        Location = info.Region.Location,
                        DrawFormat = label.DrawFormat,
                        Text = label.Text.Substring(info.Index, info.Length),
                        Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Underline),
                        Size = info.Region.Size,
                        Outline = false,
                        Sound = SoundIndex.ButtonC,
                    });
                }

                foreach (DXLabel newlabel in labels)
                {
                    newlabel.MouseEnter += (o, e) =>
                    {
                        if (chatFade == 0F) return;

                        GameScene.Game.MouseItem = item;
                        foreach (DXLabel l in labels)
                            l.ForeColour = Color.Red;
                    };

                    newlabel.MouseLeave += (o, e) =>
                    {
                        GameScene.Game.MouseItem = null;
                        foreach (DXLabel l in labels)
                            l.ForeColour = Color.Yellow;
                    };

                    newlabel.MouseWheel += ScrollBar.DoMouseWheel;

                    ItemLabels.Add(newlabel);
                }
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
                        History[i].Dispose();

                    History.Clear();
                    History = null;
                }

                if (AlertIcon != null)
                {
                    if (!AlertIcon.IsDisposed)
                        AlertIcon.Dispose();

                    AlertIcon = null;
                }

                if (ItemLabels != null)
                {
                    for (int i = 0; i < ItemLabels.Count; i++)
                    {
                        if (ItemLabels[i] == null) continue;
                        if (!ItemLabels[i].IsDisposed)
                            ItemLabels[i].Dispose();

                        ItemLabels[i] = null;
                    }

                    ItemLabels.Clear();
                    ItemLabels = null;
                }
            }

        }

        #endregion
    }

    public class ChatHistory
    {
        public string Message;
        public DXLabel Label;
        public List<ClientUserItem> LinkedItems;
        public DateTime SentDate;

        public MessageAction Action;

        public void Dispose()
        {
            if (Label != null && !Label.IsDisposed)
                Label.Dispose();

            if (LinkedItems == null) return;
            LinkedItems.Clear();
            LinkedItems = null;
        }
    }
}
