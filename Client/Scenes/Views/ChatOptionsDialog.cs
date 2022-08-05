using System.Drawing;
using Client.Controls;
using Client.UserModels;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class ChatOptionsDialog : DXWindow
    {
        #region Properties
        public DXListBox ListBox;

        public override WindowType Type => WindowType.ChatOptionsBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        public ChatOptionsDialog()
        {
            TitleLabel.Text = "Chat Options";
            HasFooter = true;

            SetClientSize(new Size(350, 200));

            ListBox = new DXListBox
            {
                Location = ClientArea.Location,
                Size = new Size(120, ClientArea.Height - SmallButtonHeight - 5),
                Parent = this,
            };


            DXButton button = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Add" },
                Parent = this,
                Size = new Size(50, SmallButtonHeight),
            };
            button.Location = new Point((ListBox.DisplayArea.Right - button.Size.Width), ListBox.DisplayArea.Bottom + 5);

            button.MouseClick += (o, e) => AddNewTab(null);

            button = new DXButton
            {
                ButtonType = ButtonType.Default,
                Label = { Text = "Reset All" },
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(ClientArea.Right - 80 - 10, Size.Height - 43),
            };
            button.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox("Are you sure you want to reset ALL chat windows", "Chat Reset", DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    for (int i = ChatTab.Tabs.Count - 1; i >= 0; i--)
                        ChatTab.Tabs[i].Panel.RemoveButton.InvokeMouseClick();

                    CreateDefaultWindows();
                };
            };

            button = new DXButton
            {
                ButtonType = ButtonType.Default,
                Label = { Text = "Save All" },
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(ClientArea.X, Size.Height - 43),
            };
            button.MouseClick += (o, e) =>
            {
                // DXMessageBox box = new DXMessageBox("Are you sure you want to reset ALL chat windows", "Chat Reset", DXMessageBoxButtons.YesNo);

                GameScene.Game.SaveChatTabs();
                GameScene.Game.ReceiveChat("Chat Layout Saved", MessageType.Announcement);
            };

            button = new DXButton
            {
                ButtonType = ButtonType.Default,
                Label = { Text = "Reload All" },
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(ClientArea.X + 85, Size.Height - 43),
            };
            button.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox("Are you sure you want to reload ALL chat windows", "Chat reload", DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    GameScene.Game.LoadChatTabs();
                };
            };
        }

        #region Methods

        public ChatTab AddNewTab(ChatTabPageSetting settings)
        {
            ChatOptionsPanel panel;
            DXListBoxItem item = new DXListBoxItem
            {
                Parent = ListBox,

                Item = panel = new ChatOptionsPanel
                {
                    Parent = this,
                    Visible = false,
                    Location = new Point(ListBox.Location.X + ListBox.Size.Width + 5, ListBox.Location.Y),
                    LocalCheckBox = { Checked = true },
                    WhisperCheckBox = { Checked = true },
                    GroupCheckBox = { Checked = true },
                    GuildCheckBox = { Checked = true },
                    ShoutCheckBox = { Checked = true },
                    GlobalCheckBox = { Checked = true },
                    ObserverCheckBox = { Checked = true },
                    HintCheckBox = { Checked = true },
                    SystemCheckBox = { Checked = true },
                    GainsCheckBox = { Checked = true },
                    AlertCheckBox = { Checked = true },
                },
            };
            item.SelectedChanged += (o, e) => panel.Visible = item.Selected;

            DXTabControl tabControl = new DXTabControl
            {
            //    PassThrough = false,
                Size = new Size(200, 200),
                Parent = GameScene.Game,
                Movable = true,
            };

            ChatTab tab = new ChatTab
            {
                Parent = tabControl,
                Panel =  panel,
                Opacity = 0.5F,
                AllowResize = true,
                TabButton =
                {
                    Movable = true, AllowDragOut = true,
                    Label = { Text = $"Window {ListBox.Controls.Count - 1}" }
                }
            };

            tab.Settings = settings;

            tabControl.MouseWheel += (o, e1) => (tabControl.SelectedTab as ChatTab)?.ScrollBar.DoMouseWheel(o, e1);

            panel.TransparentCheckBox.CheckedChanged += (o, e1) => tab.TransparencyChanged();
            panel.AlertCheckBox.CheckedChanged += (o, e1) => tab.AlertIcon.Visible = false;

            panel.Size = new Size(ClientArea.Width - panel.Location.X, ClientArea.Height);

            panel.TextChanged += (o1, e1) =>
            {
                item.Label.Text = panel.Text;
                tab.TabButton.Label.Text = panel.Text;
            };


            panel.Text = $"Window {ListBox.Controls.Count - 1}";
            
            panel.RemoveButton.MouseClick += (o1, e1) =>
            {
                DXListBoxItem nextItem = null;
                bool found = false;

                foreach (DXControl control in ListBox.Controls)
                {
                    if (!(control is DXListBoxItem)) continue;

                    if (control == item)
                    {
                        found = true;
                        continue;
                    }

                    nextItem = control as DXListBoxItem;
                    
                    if (found) break;
                }
                ListBox.SelectedItem = nextItem;

                item.Dispose();
                panel.Dispose();
                ListBox.UpdateItems();


                tabControl = tab.Parent as DXTabControl;
                tab.Dispose();

                if (tabControl?.Controls.Count == 0)
                    tabControl.Dispose();
            };

            ListBox.SelectedItem = item;

            return tab;
        }

        public void CreateDefaultWindows()
        {
            GameScene.Game.ChatTextBox.Dispose();

            GameScene.Game.ChatTextBox = new ChatTextBox
            {
                Parent = GameScene.Game,
            };

            ChatTab tab = AddNewTab(null);
            
            tab.CurrentTabControl.Size = new Size(GameScene.Game.ChatTextBox.Size.Width, 150);
            tab.CurrentTabControl.Location = new Point(GameScene.Game.ChatTextBox.Location.X, GameScene.Game.ChatTextBox.Location.Y - 150);

            tab.Panel.Text = $"Chat {ListBox.Controls.Count - 1}";
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ListBox != null)
                {
                    if (!ListBox.IsDisposed)
                        ListBox.Dispose();

                    ListBox = null;
                }

            }

        }

        #endregion
    }
    
    public sealed class ChatOptionsPanel : DXControl
    {
        #region Properties

        public DXTextBox NameTextBox;
        public DXButton RemoveButton;
        
        public DXCheckBox TransparentCheckBox, AlertCheckBox;
        public DXCheckBox LocalCheckBox, WhisperCheckBox;
        public DXCheckBox GroupCheckBox, GuildCheckBox;
        public DXCheckBox ShoutCheckBox, GlobalCheckBox;
        public DXCheckBox ObserverCheckBox;
        public DXCheckBox SystemCheckBox, GainsCheckBox;
        public DXCheckBox HintCheckBox;


        public override void OnTextChanged(string oValue, string nValue)
        {
            base.OnTextChanged(oValue, nValue);

            if (NameTextBox != null)
                NameTextBox.TextBox.Text = Text;
        }

        #endregion

        public ChatOptionsPanel()
        {
            DXLabel label = new DXLabel
            {
                Text = "Chat Name:",
                Outline = true,
                Parent = this,
            };
            label.Location = new Point(74 - label.Size.Width, 1);

            NameTextBox = new DXTextBox
            {
                Location = new Point(74, 1),
                Size = new Size(80, 20),
                Parent = this,
            };
            NameTextBox.TextBox.TextChanged += (o, e) => Text = NameTextBox.TextBox.Text;

            TransparentCheckBox = new DXCheckBox
            {
                Label = { Text = "Transparent:" },
                Parent = this,
                Checked = false,
            };
            TransparentCheckBox.Location = new Point(100 - TransparentCheckBox.Size.Width, 40);

            AlertCheckBox = new DXCheckBox
            {
                Label = { Text = "Show Alert:" },
                Parent = this,
                Checked = false,
            };
            AlertCheckBox.Location = new Point(216 - AlertCheckBox.Size.Width, 40);


            LocalCheckBox = new DXCheckBox
            {
                Label = { Text = "Local Chat:" },
                Parent = this,
                Checked = false,
            };
            LocalCheckBox.Location = new Point(100 - LocalCheckBox.Size.Width, 80);

            WhisperCheckBox = new DXCheckBox
            {
                Label = { Text = "Whisper Chat:" },
                Parent = this,
                Checked = false,
            };
            WhisperCheckBox.Location = new Point(216 - WhisperCheckBox.Size.Width, 80);

            GroupCheckBox = new DXCheckBox
            {
                Label = { Text = "Group Chat:" },
                Parent = this,
                Checked = false,
            };
            GroupCheckBox.Location = new Point(100 - GroupCheckBox.Size.Width, 105);

            GuildCheckBox = new DXCheckBox
            {
                Label = { Text = "Guild Chat:" },
                Parent = this,
                Checked = false,
            };
            GuildCheckBox.Location = new Point(216 - GuildCheckBox.Size.Width, 105);

            ShoutCheckBox = new DXCheckBox
            {
                Label = { Text = "Shout Chat:" },
                Parent = this,
                Checked = false,
            };
            ShoutCheckBox.Location = new Point(100 - ShoutCheckBox.Size.Width, 130);

            GlobalCheckBox = new DXCheckBox
            {
                Label = { Text = "Global Chat:" },
                Parent = this,
                Checked = false,
            };
            GlobalCheckBox.Location = new Point(216 - GlobalCheckBox.Size.Width, 130);

            ObserverCheckBox = new DXCheckBox
            {
                Label = { Text = "Observer Chat:" },
                Parent = this,
                Checked = false,
            };
            ObserverCheckBox.Location = new Point(100 - ObserverCheckBox.Size.Width, 155);

            HintCheckBox = new DXCheckBox
            {
                Label = { Text = "Hint Text:" },
                Parent = this,
                Checked = false,
            };
            HintCheckBox.Location = new Point(216 - HintCheckBox.Size.Width, 155);

            SystemCheckBox = new DXCheckBox
            {
                Label = { Text = "System Text:" },
                Parent = this,
                Checked = false,
            };
            SystemCheckBox.Location = new Point(100 - SystemCheckBox.Size.Width, 180);

            GainsCheckBox = new DXCheckBox
            {
                Label = { Text = "Gains Text:" },
                Parent = this,
                Checked = false,
            };
            GainsCheckBox.Location = new Point(216 - GainsCheckBox.Size.Width, 180);

            RemoveButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Remove" },
                Parent = this,
                Size = new Size(50, SmallButtonHeight),
                Location = new Point(NameTextBox.DisplayArea.Right + 10, 0),
            };


        }
        
        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (NameTextBox != null)
                {
                    if (!NameTextBox.IsDisposed)
                        NameTextBox.Dispose();

                    NameTextBox = null;
                }

                if (RemoveButton != null)
                {
                    if (!RemoveButton.IsDisposed)
                        RemoveButton.Dispose();

                    RemoveButton = null;
                }

                if (TransparentCheckBox != null)
                {
                    if (!TransparentCheckBox.IsDisposed)
                        TransparentCheckBox.Dispose();

                    TransparentCheckBox = null;
                }

                if (AlertCheckBox != null)
                {
                    if (!AlertCheckBox.IsDisposed)
                        AlertCheckBox.Dispose();

                    AlertCheckBox = null;
                }

                if (LocalCheckBox != null)
                {
                    if (!LocalCheckBox.IsDisposed)
                        LocalCheckBox.Dispose();

                    LocalCheckBox = null;
                }

                if (WhisperCheckBox != null)
                {
                    if (!WhisperCheckBox.IsDisposed)
                        WhisperCheckBox.Dispose();

                    WhisperCheckBox = null;
                }

                if (GroupCheckBox != null)
                {
                    if (!GroupCheckBox.IsDisposed)
                        GroupCheckBox.Dispose();

                    GroupCheckBox = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }

                if (ShoutCheckBox != null)
                {
                    if (!ShoutCheckBox.IsDisposed)
                        ShoutCheckBox.Dispose();

                    ShoutCheckBox = null;
                }

                if (GlobalCheckBox != null)
                {
                    if (!GlobalCheckBox.IsDisposed)
                        GlobalCheckBox.Dispose();

                    GlobalCheckBox = null;
                }

                if (ObserverCheckBox != null)
                {
                    if (!ObserverCheckBox.IsDisposed)
                        ObserverCheckBox.Dispose();

                    ObserverCheckBox = null;
                }

                if (SystemCheckBox != null)
                {
                    if (!SystemCheckBox.IsDisposed)
                        SystemCheckBox.Dispose();

                    SystemCheckBox = null;
                }

                if (GainsCheckBox != null)
                {
                    if (!GainsCheckBox.IsDisposed)
                        GainsCheckBox.Dispose();

                    GainsCheckBox = null;
                }

                if (HintCheckBox != null)
                {
                    if (!HintCheckBox.IsDisposed)
                        HintCheckBox.Dispose();

                    HintCheckBox = null;
                }
            }

        }

        #endregion
    }
}
