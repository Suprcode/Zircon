using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class ChatTextBox : DXWindow
    {
        #region Properties

        #region Mode

        public ChatMode Mode
        {
            get { return _Mode; }
            set
            {
                if (_Mode == value) return;

                ChatMode oldValue = _Mode;
                _Mode = value;

                OnModeChanged(oldValue, value);
            }
        }
        private ChatMode _Mode;
        public event EventHandler<EventArgs> ModeChanged;
        public void OnModeChanged(ChatMode oValue, ChatMode nValue)
        {
            ModeChanged?.Invoke(this, EventArgs.Empty);

            if (ChatModeButton != null)
                ChatModeButton.Label.Text = Mode.ToString();
        }

        #endregion

        public string LastPM;

        public DXTextBox TextBox;
        public DXButton OptionsButton;
        public DXButton ChatModeButton;
        private readonly List<int> LinkedItemIndexes = new List<int>();
        private readonly List<string> AutoCompleteSuggestions = new List<string>();
        private string SuggestedCompletion;
        private int AutoCompleteSuggestionIndex = -1;

        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            if (GameScene.Game.MainPanel == null) return;

            SetDefaultLocation();
        }
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (TextBox == null || ChatModeButton == null || OptionsButton == null) return;

            ChatModeButton.Location = new Point(ClientArea.Location.X, ClientArea.Y - 1);
            TextBox.Size = new Size(ClientArea.Width - ChatModeButton.Size.Width - 10 - OptionsButton.Size.Width, 100);
            TextBox.Location = new Point(ClientArea.Location.X + ChatModeButton.Size.Width + 5, ClientArea.Y);
            OptionsButton.Location = new Point(ClientArea.Location.X + TextBox.Size.Width + ChatModeButton.Size.Width + 10, ClientArea.Y - 1);
        }

        public override WindowType Type => WindowType.ChatTextBox;
        public override bool CustomSize => true;
        public override bool AutomaticVisibility => true;

        #endregion

        public ChatTextBox()
        {
            Size = new Size(400, 25);

            Opacity = 0.6F;

            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            CloseButton.Visible = false;

            AllowResize = true;
            CanResizeHeight = false;

            ChatModeButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = Mode.ToString() },
                Parent = this,
            };
            ChatModeButton.MouseClick += (o, e) => Mode = (ChatMode)(((int)(Mode) + 1) % 7);

            OptionsButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(50, SmallButtonHeight),
                Label = { Text = CEnvir.Language.ChatTextBoxOptionsButtonLabel },
                Parent = this,
            };
            OptionsButton.MouseClick += (o, e) =>
            {
                GameScene.Game.ChatOptionsBox.Visible = !GameScene.Game.ChatOptionsBox.Visible;
            };

            TextBox = new DXTextBox
            {
                Size = new Size(350, 100),
                Parent = this,
                MaxLength = Globals.MaxChatLength,
                Opacity = 0.35f,
            };
            TextBox.TextBox.KeyPress += TextBox_KeyPress;
            TextBox.TextBox.KeyDown += TextBox_KeyDown;
            TextBox.TextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            TextBox.TextBox.TextChanged += TextBox_TextChanged;

            SetDefaultSize();

            ChatModeButton.Location = new Point(ClientArea.Location.X, ClientArea.Y - 1);
            TextBox.Location = new Point(ClientArea.Location.X + ChatModeButton.Size.Width + 5, ClientArea.Y);
            OptionsButton.Location = new Point(ClientArea.Location.X + TextBox.Size.Width + ChatModeButton.Size.Width + 10, ClientArea.Y - 1);
        }

        public void SetDefaultSize()
        {
            SetClientSize(new Size(TextBox.Size.Width + ChatModeButton.Size.Width + 15 + OptionsButton.Size.Width, TextBox.Size.Height));
        }

        public void SetDefaultLocation()
        {
            Location = new Point(GameScene.Game.MainPanel.Location.X, (GameScene.Game.MainPanel.Location.Y - Size.Height));
        }

        #region Methods
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateAutoCompleteSuggestion();
        }

        private void TextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab && !string.IsNullOrEmpty(SuggestedCompletion))
                e.IsInputKey = true;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(SuggestedCompletion) || AutoCompleteSuggestions.Count == 0 ||
                TextBox.TextBox.SelectionLength != 0 || TextBox.TextBox.SelectionStart != TextBox.TextBox.TextLength) return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    AutoCompleteSuggestionIndex--;
                    if (AutoCompleteSuggestionIndex < 0)
                        AutoCompleteSuggestionIndex = AutoCompleteSuggestions.Count - 1;
                    ShowCurrentAutoCompleteSuggestion();
                    break;
                case Keys.Down:
                    AutoCompleteSuggestionIndex++;
                    if (AutoCompleteSuggestionIndex >= AutoCompleteSuggestions.Count)
                        AutoCompleteSuggestionIndex = 0;
                    ShowCurrentAutoCompleteSuggestion();
                    break;
                case Keys.Tab:
                    TextBox.TextBox.Text = SuggestedCompletion;
                    TextBox.TextBox.SelectionStart = TextBox.TextBox.TextLength;
                    TextBox.TextBox.SelectionLength = 0;
                    break;
                default:
                    return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void UpdateAutoCompleteSuggestion()
        {
            SuggestedCompletion = null;
            AutoCompleteSuggestions.Clear();
            AutoCompleteSuggestionIndex = -1;
            TextBox.TextBox.SetSuggestion(string.Empty);

            string text = TextBox.TextBox.Text;
            if (string.IsNullOrEmpty(text) || TextBox.TextBox.SelectionLength != 0 ||
                TextBox.TextBox.SelectionStart != TextBox.TextBox.TextLength) return;

            GetCommandSuggestions(text, AutoCompleteSuggestions);
            AutoCompleteSuggestions.RemoveAll(suggestion => suggestion.Length <= text.Length || suggestion.Length > TextBox.MaxLength);
            if (AutoCompleteSuggestions.Count == 0) return;

            AutoCompleteSuggestionIndex = 0;
            ShowCurrentAutoCompleteSuggestion();
        }

        private void ShowCurrentAutoCompleteSuggestion()
        {
            SuggestedCompletion = AutoCompleteSuggestions[AutoCompleteSuggestionIndex];
            TextBox.TextBox.SetSuggestion(SuggestedCompletion.Substring(TextBox.TextBox.TextLength));
        }

        private static void GetCommandSuggestions(string text, List<string> suggestions)
        {
            const string makePrefix = "@MAKE ";
            if (text.StartsWith(makePrefix, StringComparison.OrdinalIgnoreCase))
            {
                FindDatabaseMatches(text, makePrefix.Length, true, suggestions);
                return;
            }

            const string monsterPrefix = "@MONSTER ";
            if (text.StartsWith(monsterPrefix, StringComparison.OrdinalIgnoreCase))
                FindDatabaseMatches(text, monsterPrefix.Length, false, suggestions);
        }

        private static void FindDatabaseMatches(string text, int valueStart, bool itemSearch, List<string> suggestions)
        {
            string search = text.Substring(valueStart);
            if (string.IsNullOrWhiteSpace(search) || search.IndexOf(' ') >= 0) return;

            List<string> matches = new List<string>();

            if (itemSearch)
            {
                if (Globals.ItemInfoList == null) return;

                foreach (var info in Globals.ItemInfoList.Binding)
                    AddDatabaseMatch(info.ItemName, search, matches);
            }
            else
            {
                if (Globals.MonsterInfoList == null) return;

                foreach (var info in Globals.MonsterInfoList.Binding)
                    AddDatabaseMatch(info.MonsterName, search, matches);
            }

            matches.Sort((x, y) =>
            {
                int result = x.Length.CompareTo(y.Length);
                return result != 0 ? result : string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            });

            foreach (string match in matches)
                suggestions.Add(text + match.Substring(search.Length));
        }

        private static void AddDatabaseMatch(string databaseName, string search, List<string> matches)
        {
            if (string.IsNullOrWhiteSpace(databaseName)) return;

            string commandName = databaseName.Replace(" ", string.Empty);
            if (!commandName.StartsWith(search, StringComparison.OrdinalIgnoreCase) ||
                commandName.Length == search.Length) return;

            foreach (string match in matches)
                if (string.Equals(match, commandName, StringComparison.OrdinalIgnoreCase)) return;

            matches.Add(commandName);
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(TextBox.TextBox.Text))
                    {
                        CEnvir.Enqueue(new C.Chat
                        {
                            Text = TextBox.TextBox.Text,
                            LinkedItemIndexes = new List<int>(LinkedItemIndexes),
                        });

                        if (TextBox.TextBox.Text[0] == '/')
                        {
                            string[] parts = TextBox.TextBox.Text.Split(' ');

                            if (parts.Length > 0) LastPM = parts[0];
                        }
                    }

                    DXTextBox.ActiveTextBox = null;
                    TextBox.TextBox.Text = string.Empty;
                    LinkedItemIndexes.Clear();

                    ToggleVisibility(e, true);
                    break;
                case (char)Keys.Escape:
                    e.Handled = true;
                    DXTextBox.ActiveTextBox = null;
                    TextBox.TextBox.Text = string.Empty;
                    LinkedItemIndexes.Clear();

                    ToggleVisibility(e, false);
                    break;
            }
        }

        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            switch (e.KeyChar)
            {
                case '@':
                    TextBox.SetFocus();
                    TextBox.TextBox.Text = @"@";
                    TextBox.Visible = true;
                    TextBox.TextBox.SelectionLength = 0;
                    TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                    e.Handled = true;
                    break;
                case '!':
                    if (!Config.ShiftOpenChat) return;
                    TextBox.SetFocus();
                    TextBox.TextBox.Text = @"!";
                    TextBox.Visible = true;
                    TextBox.TextBox.SelectionLength = 0;
                    TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                    e.Handled = true;
                    break;
                case ' ':
                case (char)Keys.Enter:
                    OpenChat();
                    e.Handled = true;
                    break;
                case '/':
                    TextBox.SetFocus();
                    if (string.IsNullOrEmpty(LastPM))
                        TextBox.TextBox.Text = "/";
                    else
                        TextBox.TextBox.Text = LastPM + " ";
                    TextBox.Visible = true;
                    TextBox.TextBox.SelectionLength = 0;
                    TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }

        public void ToggleVisibility(KeyPressEventArgs e, bool hide)
        {
            if (Config.HideChatBar)
            {
                if (Visible)
                {
                    if (hide)
                    {
                        if (ChatTab.Tabs.Count > 0 && ChatTab.Tabs[0].Panel.TransparentCheckBox.Checked == true)
                        {
                            Visible = false;
                        }
                    }
                }
                else
                {
                    if (!hide)
                    {
                        Visible = true;

                        OnKeyPress(e);
                    }
                }
            }
        }

        public void OpenChat()
        {
            if (string.IsNullOrEmpty(TextBox.TextBox.Text))
                switch (Mode)
                {
                    case ChatMode.Shout:
                        TextBox.TextBox.Text = @"!";
                        break;
                    case ChatMode.Whisper:
                        if (!string.IsNullOrWhiteSpace(LastPM))
                            TextBox.TextBox.Text = LastPM + " ";
                        break;
                    case ChatMode.Group:
                        TextBox.TextBox.Text = @"!!";
                        break;
                    case ChatMode.Guild:
                        TextBox.TextBox.Text = @"!~";
                        break;
                    case ChatMode.Global:
                        TextBox.TextBox.Text = @"!@";
                        break;
                    case ChatMode.Observer:
                        TextBox.TextBox.Text = @"#";
                        break;
                }

            TextBox.SetFocus();
            TextBox.TextBox.SelectionLength = 0;
            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
        }
        public void StartPM(string name)
        {
            TextBox.TextBox.Text = $"/{name} ";
            LinkedItemIndexes.Clear();
            TextBox.SetFocus();
            TextBox.TextBox.SelectionLength = 0;
            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
        }

        public void LinkItem(ClientUserItem item)
        {
            if (item == null || LinkedItemIndexes.Count >= Globals.MaxChatItemLinks) return;

            TextBox.TextBox.Text += $"[{item.Info.ItemName}]";
            LinkedItemIndexes.Add(item.Index);
            TextBox.SetFocus();
            TextBox.TextBox.SelectionLength = 0;
            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Mode = 0;
                ModeChanged = null;

                LastPM = null;
                SuggestedCompletion = null;
                AutoCompleteSuggestions.Clear();
                AutoCompleteSuggestionIndex = -1;
                LinkedItemIndexes.Clear();

                if (TextBox != null)
                {
                    TextBox.TextBox.KeyPress -= TextBox_KeyPress;
                    TextBox.TextBox.KeyDown -= TextBox_KeyDown;
                    TextBox.TextBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                    TextBox.TextBox.TextChanged -= TextBox_TextChanged;

                    if (!TextBox.IsDisposed)
                        TextBox.Dispose();

                    TextBox = null;
                }

                if (OptionsButton != null)
                {
                    if (!OptionsButton.IsDisposed)
                        OptionsButton.Dispose();

                    OptionsButton = null;
                }

                if (ChatModeButton != null)
                {
                    if (!ChatModeButton.IsDisposed)
                        ChatModeButton.Dispose();

                    ChatModeButton = null;
                }
            }

        }

        #endregion
    }

    public enum ChatMode
    {
        Local,
        Whisper,
        Group,
        Guild,
        Shout,
        Global,
        Observer,
    }

    public class Message
    {
        public string Text { get; set; }
        public DateTime ReceivedTime { get; set; }
        public MessageType Type { get; set; }
    }
}
