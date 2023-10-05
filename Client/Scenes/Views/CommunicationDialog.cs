using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class CommunicationDialog : DXImageControl
    {
        #region Properties

        #region RecipientValid

        public bool RecipientValid
        {
            get => _RecipientValid;
            set
            {
                if (_RecipientValid == value) return;

                bool oldValue = _RecipientValid;
                _RecipientValid = value;

                OnRecipientValidChanged(oldValue, value);
            }
        }
        private bool _RecipientValid;
        public event EventHandler<EventArgs> RecipientValidChanged;
        public void OnRecipientValidChanged(bool oValue, bool nValue)
        {
            RecipientValidChanged?.Invoke(this, EventArgs.Empty);
            SendButton.Enabled = CanSend;
        }

        #endregion

        #region GoldValid

        public bool GoldValid
        {
            get => _GoldValid;
            set
            {
                if (_GoldValid == value) return;

                bool oldValue = _GoldValid;
                _GoldValid = value;

                OnGoldValidChanged(oldValue, value);
            }
        }
        private bool _GoldValid;
        public event EventHandler<EventArgs> GoldValidChanged;
        public void OnGoldValidChanged(bool oValue, bool nValue)
        {
            GoldValidChanged?.Invoke(this, EventArgs.Empty);
            SendButton.Enabled = CanSend;
        }
        #endregion
        
        #region SendAttempted

        public bool SendAttempted
        {
            get => _SendAttempted;
            set
            {
                if (_SendAttempted == value) return;

                bool oldValue = _SendAttempted;
                _SendAttempted = value;

                OnSendAttemptedChanged(oldValue, value);
            }
        }
        private bool _SendAttempted;
        public event EventHandler<EventArgs> SendAttemptedChanged;
        public void OnSendAttemptedChanged(bool oValue, bool nValue)
        {
            SendAttemptedChanged?.Invoke(this, EventArgs.Empty);
            SendButton.Enabled = CanSend;
        }
       
        #endregion

        #region ReadMail

        public ClientMailInfo ReadMail
        {
            get => _ReadMail;
            set
            {
                ClientMailInfo oldValue = _ReadMail;
                _ReadMail = value;

                OnReadMailChanged(oldValue, value);
            }
        }
        private ClientMailInfo _ReadMail;
        public event EventHandler<EventArgs> ReadMailChanged;
        public void OnReadMailChanged(ClientMailInfo oValue, ClientMailInfo nValue)
        {
            ReadTab.Visible = ReadMail != null;
            ReadReplyButton.Visible = ReadMail != null;
            ReadDeleteButton.Visible = ReadMail != null;

            if (ReadMail != null)
            {
                SendButton.Visible = false;

                BlockAddButton.Visible = false;
                BlockRemoveButton.Visible = false;

                ReceivedCollectAllButton.Visible = false;
                ReceivedDeleteAll.Visible = false;
                ReceivedNewButton.Visible = false;
            }

            if (ReadMail == null) return;

            ReadSenderBox.TextBox.Text = ReadMail.Sender;
            ReadSubjectBox.TextBox.Text = ReadMail.Subject;
            ReadDateBox.TextBox.Text = ReadMail.Date.ToLongDateString() + " " + ReadMail.Date.ToLongTimeString();
            ReadMessageLabel.Text = ReadMail.Message;

            int height = DXLabel.GetHeight(ReadMessageLabel, ReadMessageLabel.Size.Width).Height;

            ReadMessageLabel.Size = new Size(ReadMessageContainer.Size.Width, height);
            ReadMessageScrollBar.MaxValue = ReadMessageLabel.Size.Height - ReadMessageContainer.Size.Height + 14;

            foreach (DXItemCell cell in ReadGrid.Grid)
                cell.Item = ReadMail.Items.FirstOrDefault(x => x.Slot == cell.Slot);

            ReadMailChanged?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion

        #region OnlineState

        private bool _stateSet = false;

        public OnlineState OnlineState
        {
            get => _OnlineState;
            set
            {
                if (_OnlineState == value && _stateSet) return;

                OnlineState oldValue = _OnlineState;
                _OnlineState = value;
                _stateSet = true;

                OnOnlineStateChanged(oldValue, value);
            }
        }
        private OnlineState _OnlineState;
        public event EventHandler<EventArgs> OnlineStateChanged;
        public void OnOnlineStateChanged(OnlineState oValue, OnlineState nValue)
        {
            OnlineStateChanged?.Invoke(this, EventArgs.Empty);

            FriendOnlineStateBox.ListBox.SelectItem(nValue);
        }

        #endregion

        public bool CanSend => !SendAttempted && RecipientValid && GoldValid;

        private DXButton CloseButton;
        private DXLabel TitleLabel;
        private DXTabControl TabControl;
        private DXImageControl BackgroundImage;
        public DXTab FriendTab, ReceivedTab, SendTab, BlockTab;
        private DXImageControl ReadTab;

        //Friends
        private DXComboBox FriendOnlineStateBox, FriendViewStatusBox;
        private DXListBox FriendListBox;
        private DXButton FriendAddButton, FriendRemoveButton;
        private List<FriendRow> FriendListBoxItems = new List<FriendRow>();
        public List<ClientFriendInfo> FriendList = new List<ClientFriendInfo>();

        //Received
        private DXLabel RecievedCategoryLabel, RecievedTitleLabel, ReceivedDateLabel;
        private DXVScrollBar RecievedScrollBar;
        public CommunicationReceivedRow[] ReceivedRows;
        public List<ClientMailInfo> ReceivedMailList = new List<ClientMailInfo>();
        private DXButton ReceivedCollectAllButton, ReceivedDeleteAll, ReceivedNewButton;

        //Send Mail
        private DXTextBox SendRecipientBox, SendSubjectBox, SendMessageBox;
        private DXVScrollBar SendMessageScrollBar;
        private ClientUserItem[] SendMailItems;
        public DXItemGrid SendGrid;
        private DXNumberBox SendGoldBox;
        private DXButton SendButton;

        //Read Mail
        public DXTextBox ReadSenderBox, ReadSubjectBox, ReadDateBox;
        public DXControl ReadMessageContainer;
        public DXLabel ReadMessageLabel;
        public DXVScrollBar ReadMessageScrollBar;
        public DXButton ReadReplyButton, ReadDeleteButton;
        public DXItemGrid ReadGrid;
        public ClientUserItem[] ReadMailItems;

        //Blocked
        private DXButton BlockAddButton, BlockRemoveButton;
        private DXListBox BlockListBox;
        private List<DXListBoxItem> BlockListBoxItems = new List<DXListBoxItem>();

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
                BringToFront();

            base.OnIsVisibleChanged(oValue, nValue);

            RefreshList();
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

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.CommunicationBox;

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

        #endregion

        public CommunicationDialog()
        {
            Index = 200;
            LibraryFile = LibraryFile.Interface;
            Movable = true;
            Sort = true;

            #region Main

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.CommunicationDialogTitle,
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
                Location = new Point(0, 37),
                Size = new Size(296, 340),
                MarginLeft = 10,
                Border = false
            };

            BackgroundImage = new DXImageControl
            {
                Parent = TabControl,
                Index = 201,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(0, 23),
                Size = new Size(296, 316),
                Visible = true
            };

            FriendTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CommunicationDialogFriendTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            FriendTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 201;

                FriendAddButton.Visible = true;
                FriendRemoveButton.Visible = true;

                SendButton.Visible = false;

                BlockAddButton.Visible = false;
                BlockRemoveButton.Visible = false;

                ReceivedCollectAllButton.Visible = false;
                ReceivedDeleteAll.Visible = false;
                ReceivedNewButton.Visible = false;

                ReadMail = null;

                ClearItemGrid();
            };

            ReceivedTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CommunicationDialogReceivedTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            ReceivedTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 202;

                FriendAddButton.Visible = false;
                FriendRemoveButton.Visible = false;

                SendButton.Visible = false;

                BlockAddButton.Visible = false;
                BlockRemoveButton.Visible = false;

                ReceivedCollectAllButton.Visible = true;
                ReceivedDeleteAll.Visible = true;
                ReceivedNewButton.Visible = true;

                ReadMail = null;

                ClearItemGrid();
            };

            SendTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CommunicationDialogSendTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            SendTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 203;

                FriendAddButton.Visible = false;
                FriendRemoveButton.Visible = false;

                SendRecipientBox.TextBox.Text = string.Empty;
                SendMessageBox.TextBox.Text = string.Empty;
                SendSubjectBox.TextBox.Text = string.Empty;
                SendGoldBox.Value = 0;

                SendButton.Visible = true;

                BlockAddButton.Visible = false;
                BlockRemoveButton.Visible = false;

                ReceivedCollectAllButton.Visible = false;
                ReceivedDeleteAll.Visible = false;
                ReceivedNewButton.Visible = false;

                ReadMail = null;

                ClearItemGrid();
            };

            BlockTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.CommunicationDialogBlockedTabLabel } },
                BackColour = Color.Empty,
                Location = new Point(0, 23)
            };
            BlockTab.TabButton.MouseClick += (o, e) =>
            {
                BackgroundImage.Index = 204;

                FriendAddButton.Visible = false;
                FriendAddButton.Visible = false;
                FriendRemoveButton.Visible = false;

                SendButton.Visible = false;

                BlockAddButton.Visible = true;
                BlockRemoveButton.Visible = true;

                ReceivedCollectAllButton.Visible = false;
                ReceivedDeleteAll.Visible = false;
                ReceivedNewButton.Visible = false;

                ReadMail = null;

                ClearItemGrid();
            };

            ReadTab = new DXImageControl
            {
                Parent = this,
                Visible = false,
                Index = 205,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(0, 60)
            };

            #endregion

            #region Friends

            DXLabel label = new DXLabel
            {
                Parent = FriendTab,
                Text = CEnvir.Language.CommunicationDialogFriendTabStatusLabel
            };
            label.Location = new Point(150 - label.Size.Width, 11);

            FriendOnlineStateBox = new DXComboBox
            {
                Border = false,
                Parent = FriendTab,
                Location = new Point(151, label.Location.Y - 1),
                Size = new Size(122, DXComboBox.DefaultNormalHeight)
            };
            foreach (object val in Enum.GetValues(typeof(OnlineState)))
            {
                new DXListBoxItem
                {
                    Parent = FriendOnlineStateBox.ListBox,
                    Label = { Text = val.ToString() },
                    Item = val
                };
            }
            FriendOnlineStateBox.SelectedItemChanged += (o, e) =>
            {
                var oldState = OnlineState;

                OnlineState = (OnlineState)FriendOnlineStateBox.SelectedItem;

                if (oldState != OnlineState)
                    CEnvir.Enqueue(new C.ChangeOnlineState { State = (OnlineState)OnlineState });
            };

            label = new DXLabel
            {
                Parent = FriendTab,
                Text = CEnvir.Language.CommunicationDialogFriendTabViewStatusLabel
            };
            label.Location = new Point(150 - label.Size.Width, 32);

            FriendViewStatusBox = new DXComboBox
            {
                Border = false,
                Parent = FriendTab,
                Location = new Point(151, label.Location.Y - 1),
                Size = new Size(122, DXComboBox.DefaultNormalHeight),
            };

            new DXListBoxItem
            {
                Parent = FriendViewStatusBox.ListBox,
                Label = { Text = "All" },
                Item = "All"
            };

            foreach (object val in Enum.GetValues(typeof(OnlineState)))
            {
                new DXListBoxItem
                {
                    Parent = FriendViewStatusBox.ListBox,
                    Label = { Text = val.ToString() },
                    Item = val
                };
            }

            FriendViewStatusBox.ListBox.SelectItem("All");

            FriendListBox = new DXListBox
            {
                Parent = FriendTab,
                Border = false,
                Location = new Point(12, 65),
                Size = new Size(268, 238),
                ScrollBar = {
                    Visible = true,
                    BackColour = Color.Empty,
                    Border = false,
                    Parent = FriendTab,
                    Location = new Point(BlockTab.Size.Width - 31, 59),
                    Size = new Size(20, 252),
                    UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                    DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                    PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface }
                }
            };

            FriendAddButton = new DXButton
            {
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommunicationDialogFriendTabFriendAddButtonLabel },
                Visible = true
            };
            FriendAddButton.Location = new Point(43, FriendTab.Location.Y + FriendTab.Size.Height + 43);
            FriendAddButton.MouseClick += (o, e) =>
            {
                DXInputWindow window = new DXInputWindow(CEnvir.Language.CommunicationDialogFriendTabFriendAddButtonConfirmMessage, CEnvir.Language.CommunicationDialogFriendTabFriendAddButtonConfirmCaption)
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
                    CEnvir.Enqueue(new C.FriendAdd { Name = window.Value });
                };
            };

            FriendRemoveButton = new DXButton
            {
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommunicationDialogFriendTabFriendRemoveButtonLabel },
                Visible = true,
                Enabled = false
            };
            FriendRemoveButton.Location = new Point(43 + FriendAddButton.Size.Width + 10, FriendTab.Location.Y + FriendTab.Size.Height + 43);
            FriendRemoveButton.MouseClick += (o, e) =>
            {
                if (FriendListBox.SelectedItem == null) return;

                DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CommunicationDialogFriendTabFriendRemoveButtonConfirmMessage, FriendListBox.SelectedItem.Label.Text), CEnvir.Language.CommunicationDialogFriendTabFriendRemoveButtonConfirmCaption, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.FriendRemove { Index = (int)FriendListBox.SelectedItem.Item });
                };
            };

            FriendListBox.selectedItemChanged += (o, e) =>
            {
                FriendRemoveButton.Enabled = FriendListBox.SelectedItem != null;
            };

            FriendViewStatusBox.SelectedItemChanged += (o, e) =>
            {
                RefreshFriendList();
            };

            #endregion

            #region Received

            RecievedCategoryLabel = new DXLabel
            {
                AutoSize = false,
                Parent = ReceivedTab,
                Size = new Size(50, 20),
                Location = new Point(15, 5),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = CEnvir.Language.CommunicationDialogReceivedTabCategoryLabel
            };

            RecievedTitleLabel = new DXLabel
            {
                AutoSize = false,
                Parent = ReceivedTab,
                Size = new Size(140, 20),
                Location = new Point(65, 5),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = CEnvir.Language.CommunicationDialogReceivedTabTitleLabel
            };

            ReceivedDateLabel = new DXLabel
            {
                AutoSize = false,
                Parent = ReceivedTab,
                Size = new Size(65, 20),
                Location = new Point(200, 5),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = CEnvir.Language.CommunicationDialogReceivedTabDateLabel
            };

            //Rows
            ReceivedRows = new CommunicationReceivedRow[5];
            RecievedScrollBar = new DXVScrollBar
            {
                Parent = ReceivedTab,
                Size = new Size(20, ReceivedTab.Size.Height - 9),
                Location = new Point(ReceivedTab.Size.Width - 31, 3),
                VisibleSize = 5,
                Change = 1,
                Border = false,
                BackColour = Color.Empty,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
            };

            RecievedScrollBar.ValueChanged += ReceivedScrollBar_ValueChanged;
            ReceivedTab.MouseWheel += RecievedScrollBar.DoMouseWheel;

            for (int i = 0; i < 5; i++)
            {
                int index = i;
                ReceivedRows[index] = new CommunicationReceivedRow
                {
                    Parent = ReceivedTab,
                    Location = new Point(18, 43 + (49 * i)),
                    Visible = false,
                    BackColour = Color.Empty
                };
                ReceivedRows[index].MouseClick += (o, e) =>
                {
                    ReadMail = ReceivedRows[index].Mail;

                    if (ReceivedRows[index].Mail.Opened) return;

                    ReceivedRows[index].Mail.Opened = true;
                    ReceivedRows[index].RefreshIcon();
                    UpdateIcon();

                    CEnvir.Enqueue(new C.MailOpened { Index = ReceivedRows[index].Mail.Index });
                };
                ReceivedRows[index].MouseWheel += RecievedScrollBar.DoMouseWheel;
            }

            ReceivedCollectAllButton = new DXButton
            {
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommunicationDialogReceivedTabCollectAllButtonLabel },
                Visible = false
            };
            ReceivedCollectAllButton.Location = new Point(15, ReceivedTab.Location.Y + ReceivedTab.Size.Height + 43);
            ReceivedCollectAllButton.MouseClick += (o, e) =>
            {
                int count = 15;
                foreach (ClientMailInfo mail in ReceivedMailList)
                {
                    if (count <= 0) break;

                    if (mail.Items.Count == 0) continue;

                    if (!mail.Opened)
                    {
                        mail.Opened = true;
                        CEnvir.Enqueue(new C.MailOpened { Index = mail.Index });
                        count--;
                        foreach (CommunicationReceivedRow row in ReceivedRows)
                        {
                            if (row.Mail != mail) continue;
                            row.RefreshIcon();
                            break;
                        }
                    }

                    foreach (ClientUserItem item in mail.Items)
                        CEnvir.Enqueue(new C.MailGetItem { Index = mail.Index, Slot = item.Slot });
                    count--;
                }

                UpdateIcon();
            };

            ReceivedDeleteAll = new DXButton
            {
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommunicationDialogReceivedTabDeleteAllButtonLabel },
                Visible = false
            };
            ReceivedDeleteAll.Location = new Point(15 + ReceivedCollectAllButton.Size.Width + 10, ReceivedTab.Location.Y + ReceivedTab.Size.Height + 43);
            ReceivedDeleteAll.MouseClick += (o, e) =>
            {
                int count = 15;
                foreach (ClientMailInfo mail in ReceivedMailList)
                {
                    if (count <= 0) break;
                    if (mail.Items.Count > 0) continue;

                    CEnvir.Enqueue(new C.MailDelete { Index = mail.Index });
                    count--;
                }
            };

            ReceivedNewButton = new DXButton
            {
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommunicationDialogReceivedTabNewButtonLabel },
                Visible = false
            };
            ReceivedNewButton.Location = new Point(15 + ReceivedCollectAllButton.Size.Width + 10 + ReceivedDeleteAll.Size.Width + 10, ReceivedTab.Location.Y + ReceivedTab.Size.Height + 43);
            ReceivedNewButton.MouseClick += (o, e) =>
            {
                SendTab.TabButton.InvokeMouseClick();
            };

            #endregion

            #region SendMail

            label = new DXLabel
            {
                Parent = SendTab,
                Text = CEnvir.Language.CommunicationDialogSendTabRecipientLabel,
            };
            label.Location = new Point(82 - label.Size.Width, 11);

            SendRecipientBox = new DXTextBox
            {
                Border = false,
                Parent = SendTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Location = new Point(86, label.Location.Y + 1),
                Size = new Size(115, 16),
                MaxLength = Globals.MaxCharacterNameLength
            };
            SendRecipientBox.TextBox.TextChanged += RecipientBox_TextChanged;

            label = new DXLabel
            {
                Parent = SendTab,
                Text = CEnvir.Language.CommunicationDialogSendTabSubjectLabel
            };
            label.Location = new Point(82 - label.Size.Width, 31);

            SendSubjectBox = new DXTextBox
            {
                Border = false,
                Parent = SendTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Location = new Point(86, label.Location.Y),
                Size = new Size(155, 16),
                MaxLength = 30
            };

            SendMessageBox = new DXTextBox
            {
                Border = false,
                BackColour = Color.Black,
                Parent = SendTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                TextBox = { Multiline = true, AcceptsReturn = true, },
                Location = new Point(15, label.Location.Y + 24),
                Size = new Size(SendTab.Size.Width - 55, 185),
                MaxLength = 300
            };
            SendMessageBox.TextBox.TextChanged += (o, e) => UpdateSendMessagePosition();
            SendMessageBox.TextBox.MouseMove += (o, e) => UpdateSendMessagePosition();
            SendMessageBox.TextBox.MouseDown += (o, e) => UpdateSendMessagePosition();
            SendMessageBox.TextBox.MouseUp += (o, e) => UpdateSendMessagePosition();
            SendMessageBox.TextBox.KeyDown += (o, e) => UpdateSendMessagePosition();
            SendMessageBox.TextBox.KeyUp += (o, e) => UpdateSendMessagePosition();
            SendMessageBox.TextBox.KeyPress += (o, e) =>
            {
                if (e.KeyChar == (char)1)
                {
                    SendMessageBox.TextBox.SelectAll();
                    e.Handled = true;
                }

                UpdateSendMessagePosition();
            };

            SendMessageScrollBar = new DXVScrollBar
            {
                Parent = SendTab,
                Size = new Size(20, 198),
                Location = new Point(SendTab.Size.Width - 34, 49),
                VisibleSize = 13,
                Change = 1,
                Border = false,
                BackColour = Color.Empty,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
            };
            SendMessageScrollBar.ValueChanged += (o, e) => SetSendMessageLineIndex(SendMessageScrollBar.Value);

            label = new DXLabel
            {
                Parent = SendTab,
                Text = CEnvir.Language.CommunicationDialogSendTabItemsLabel
            };
            label.Location = new Point(82 - label.Size.Width, 246);

            SendMailItems = new ClientUserItem[5];

            SendGrid = new DXItemGrid
            {
                GridSize = new Size(5, 1),
                Parent = SendTab,
                Location = new Point(82, label.Location.Y),
                Linked = true,
                GridType = GridType.SendMail,
                BackColour = Color.Empty,
                Border = false
            };

            label = new DXLabel
            {
                Parent = SendTab,
                Text = CEnvir.Language.CommunicationDialogSendTabGoldLabel,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
            };
            label.Location = new Point(82 - label.Size.Width, SendGrid.Location.Y + 3 + SendGrid.Size.Height);

            SendGoldBox = new DXNumberBox
            {
                Parent = SendTab,
                Location = new Point(86, label.Location.Y),
                UpButton = { Visible = false },
                DownButton = { Visible = false },
                Border = false,
                Size = new Size(122, 16),
                ValueTextBox = { Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular), Border = false, Location = new Point(1, 1), Size = new Size(100, 16) },
                MaxValue = 2000000000
            };
            SendGoldBox.ValueTextBox.ValueChanged += GoldBox_ValueChanged;

            SendButton = new DXButton
            {
                ButtonType = ButtonType.Default,
                Size = new Size(70, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommunicationDialogSendTabSendButtonLabel },
                Enabled = false,
                Visible = false
            };
            SendButton.Location = new Point((Size.Width - SendButton.Size.Width) / 2, SendTab.Location.Y + SendTab.Size.Height + 43);
            SendButton.MouseClick += (o, e) => SendMail();

            GoldValid = true;

            #endregion

            #region Blocked

            BlockListBox = new DXListBox
            {
                Parent = BlockTab,
                Border = false,
                Location = new Point(12, 65),
                Size = new Size(268, 238),
                ScrollBar = {
                    Visible = true,
                    BackColour = Color.Empty,
                    Border = false,
                    Parent = BlockTab,
                    Location = new Point(BlockTab.Size.Width - 31, 59),
                    Size = new Size(20, 252),
                    UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                    DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                    PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface }
                },
            };

            BlockAddButton = new DXButton
            {
                Label = { Text = CEnvir.Language.CommunicationDialogBlockedTabAddButtonLabel },
                Parent = this,
                Size = new Size(100, DefaultHeight),
                ButtonType = ButtonType.Default,
                Visible = false
            };
            BlockAddButton.MouseClick += (o, e) =>
            {
                DXInputWindow window = new DXInputWindow(CEnvir.Language.CommunicationDialogBlockedTabAddButtonConfirmMessage, CEnvir.Language.CommunicationDialogBlockedTabAddButtonConfirmCaption)
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
                    CEnvir.Enqueue(new C.BlockAdd { Name = window.Value });
                };
            };
            
            BlockRemoveButton = new DXButton
            {
                Label = { Text = CEnvir.Language.CommunicationDialogBlockedTabRemoveButtonLabel },
                Parent = this,
                Size = new Size(100, DefaultHeight),
                ButtonType = ButtonType.Default,
                Enabled = false,
                Visible = false
            };
            BlockRemoveButton.MouseClick += (o, e) =>
            {
                if (BlockListBox.SelectedItem == null) return;

                DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.CommunicationDialogBlockedTabRemoveButtonConfirmMessage, BlockListBox.SelectedItem.Label.Text), CEnvir.Language.CommunicationDialogBlockedTabRemoveButtonConfirmCaption, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.BlockRemove { Index = (int)BlockListBox.SelectedItem.Item });
                };
            };
            BlockAddButton.Location = new Point(43, BlockTab.Location.Y + 56);
            BlockRemoveButton.Location = new Point(43 + BlockAddButton.Size.Width + 10, BlockTab.Location.Y + 56);

            BlockListBox.selectedItemChanged += (o, e) =>
            {
                BlockRemoveButton.Enabled = BlockListBox.SelectedItem != null;
            };

            RefreshBlockList();

            #endregion

            #region ReadMail

            label = new DXLabel
            {
                Parent = ReadTab,
                Text = "Sender:"
            };
            label.Location = new Point(82 - label.Size.Width, 11);

            ReadSenderBox = new DXTextBox
            {
                Border = false,
                Parent = ReadTab,
                ReadOnly = true,
                Editable = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Location = new Point(86, label.Location.Y + 1),
                Size = new Size(115, 16)
            };
            SendRecipientBox.TextBox.TextChanged += RecipientBox_TextChanged;

            label = new DXLabel
            {
                Parent = ReadTab,
                Text = "Subject:"
            };
            label.Location = new Point(82 - label.Size.Width, 30);

            ReadSubjectBox = new DXTextBox
            {
                Border = false,
                Parent = ReadTab,
                ReadOnly = true,
                Editable = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Location = new Point(86, label.Location.Y + 1),
                Size = new Size(155, 16)
            };

            label = new DXLabel
            {
                Parent = ReadTab,
                Text = "Date Sent:"
            };
            label.Location = new Point(82 - label.Size.Width, 49);

            ReadDateBox = new DXTextBox
            {
                Border = false,
                Parent = ReadTab,
                ReadOnly = true,
                Editable = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Location = new Point(86, label.Location.Y + 1),
                Size = new Size(155, 16)
            };

            ReadMessageContainer = new DXControl
            {
                Parent = ReadTab,
                Location = new Point(15, label.Location.Y + 24),
                Size = new Size(ReadTab.Size.Width - 55, 167),
            };

            ReadMessageLabel = new DXLabel
            {
                AutoSize = false,
                BackColour = Color.Black,
                Parent = ReadMessageContainer,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Location = new Point(0, 0),
                Size = new Size(ReadMessageContainer.Size.Width, ReadMessageContainer.Size.Height),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.WordBreak
            };

            ReadMessageScrollBar = new DXVScrollBar
            {
                Parent = ReadTab,
                Size = new Size(20, 178),
                Location = new Point(ReadTab.Size.Width - 34, 68),
                VisibleSize = 12,
                Change = 1,
                MinValue = 0,
                MaxValue = 100,
                Border = false,
                BackColour = Color.Empty,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
            };
            ReadMessageScrollBar.ValueChanged += ReadMessageScrollBar_ValueChanged;
            ReadMessageLabel.MouseWheel += ReadMessageScrollBar.DoMouseWheel;

            label = new DXLabel
            {
                Parent = ReadTab,
                Text = "Items:"
            };
            label.Location = new Point(10, 246);

            ReadMailItems = new ClientUserItem[7];

            ReadGrid = new DXItemGrid
            {
                GridSize = new Size(7, 1),
                Parent = ReadTab,
                Location = new Point(13, 265),
                ItemGrid = ReadMailItems,
                BackColour = Color.Empty,
                Border = false
            };
            foreach (DXItemCell cell in ReadGrid.Grid)
            {
                cell.ReadOnly = true;
                cell.MouseClick += (o, e) =>
                {
                    if (cell.Item == null) return;
                    CEnvir.Enqueue(new C.MailGetItem { Index = ReadMail.Index, Slot = cell.Slot });
                };
            }

            ReadReplyButton = new DXButton
            {
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = "Reply Mail" },
                Visible = false
            };
            ReadReplyButton.Location = new Point(43, ReadTab.Size.Height + 67);
            ReadReplyButton.MouseClick += (o, e) =>
            {
                var sender = ReadMail.Sender;
                var subject = "RE: " + ReadMail.Subject;

                if (subject.Length > SendSubjectBox.TextBox.MaxLength)
                    subject = "";

                SendTab.TabButton.InvokeMouseClick();

                SendRecipientBox.TextBox.Text = sender;
                SendSubjectBox.TextBox.Text = subject;
                SendMessageBox.SetFocus();
            };

            ReadDeleteButton = new DXButton
            {
                Size = new Size(100, DefaultHeight),
                Parent = this,
                Label = { Text = "Delete Mail" },
                Visible = false
            };
            ReadDeleteButton.Location = new Point(43 + ReadReplyButton.Size.Width + 10, ReadTab.Size.Height + 67);
            ReadDeleteButton.MouseClick += (o, e) =>
            {
                if (ReadMail.Items.Count > 0)
                {
                    GameScene.Game.ReceiveChat(CEnvir.Language.CommunicationCannotDeleteMailWithItems, MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.MailDelete { Index = ReadMail.Index });

                ReceivedTab.TabButton.InvokeMouseClick();
            };


            #endregion
        }

        #region Methods

        public void UpdateSendMessagePosition()
        {
            SendMessageScrollBar.MaxValue = SendMessageBox.TextBox.GetLineFromCharIndex(SendMessageBox.TextBox.TextLength) + 1;
            SendMessageScrollBar.Value = GetSendMessageCurrentLine();
        }

        private int GetSendMessageCurrentLine()
        {
            return SendMessage(SendMessageBox.TextBox.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);
        }

        const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        const int EM_LINESCROLL = 0x00B6;

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void SetSendMessageLineIndex(int lineIndex)
        {
            int line = GetSendMessageCurrentLine();
            if (line == lineIndex) return;

            SendMessage(SendMessageBox.TextBox.Handle, EM_LINESCROLL, 0, lineIndex - GetSendMessageCurrentLine());
            SendMessageBox.DisposeTexture();
        }

        private void ReceivedScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void ReadMessageScrollBar_ValueChanged(object sender, EventArgs e)
        {
            int y = -ReadMessageScrollBar.Value;

            ReadMessageLabel.Location = new Point(0, 0 + y);
        }

        public void RefreshList()
        {
            if (ReceivedRows == null) return;

            ReceivedMailList.Sort((x1, x2) =>
            {
                int value = x2.Date.CompareTo(x1.Date);

                if (value == 0)
                    return x1.Index.CompareTo(x2.Index);
                return value;
            });

            RecievedScrollBar.MaxValue = ReceivedMailList.Count;

            for (int i = 0; i < ReceivedRows.Length; i++)
            {
                if (i + RecievedScrollBar.Value >= ReceivedMailList.Count)
                {
                    ReceivedRows[i].Mail = null;
                    continue;
                }

                ReceivedRows[i].Mail = ReceivedMailList[i + RecievedScrollBar.Value];
            }
        }

        public void UpdateIcon()
        {
            GameScene.Game.MainPanel.NewMailIcon.Visible = ReceivedMailList.Any(x => !x.Opened);
        }

        private void GoldBox_ValueChanged(object sender, EventArgs e)
        {
            GoldValid = SendGoldBox.Value >= 0 && SendGoldBox.Value <= MapObject.User.Gold.Amount;

            if (SendGoldBox.Value == 0)
                SendGoldBox.ValueTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                SendGoldBox.ValueTextBox.BorderColour = GoldValid ? Color.Green : Color.Red;
        }

        private void RecipientBox_TextChanged(object sender, EventArgs e)
        {
            RecipientValid = Globals.CharacterReg.IsMatch(SendRecipientBox.TextBox.Text);

            if (string.IsNullOrEmpty(SendRecipientBox.TextBox.Text))
                SendRecipientBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                SendRecipientBox.BorderColour = RecipientValid ? Color.Green : Color.Red;
        }

        public void SendMail()
        {
            SendAttempted = true;

            List<CellLinkInfo> links = new List<CellLinkInfo>();

            foreach (DXItemCell cell in SendGrid.Grid)
            {
                if (cell.Link == null) continue;

                links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = true;
                cell.Link = null;
            }

            CEnvir.Enqueue(new C.MailSend { Links = links, Recipient = SendRecipientBox.TextBox.Text, Subject = SendSubjectBox.TextBox.Text, Message = SendMessageBox.TextBox.Text, Gold = SendGoldBox.Value });

            SendRecipientBox.TextBox.Text = string.Empty;
            SendMessageBox.TextBox.Text = string.Empty;
            SendSubjectBox.TextBox.Text = string.Empty;

            SendGoldBox.Value = 0;
        }

        public void ClearItemGrid()
        {
            foreach (DXItemCell cell in SendGrid.Grid)
            {
                if (cell.Link == null) continue;

                cell.Link.Locked = false;
                cell.Link = null;
            }
        }

        public void RefreshFriendList()
        {
            FriendListBox.SelectedItem = null;

            foreach (FriendRow item in FriendListBoxItems)
                item.Dispose();

            FriendListBoxItems.Clear();

            var filterOption = FriendViewStatusBox.SelectedItem;

            foreach (ClientFriendInfo info in FriendList.OrderBy(x => x.State))
            {
                if (filterOption?.ToString() != "All" && info.State != (OnlineState)filterOption)
                    continue;

                FriendRow row;

                FriendListBoxItems.Add(row = new FriendRow()
                {
                    Parent = FriendListBox,
                    Label = { Text = info.Name },
                    Item = info.Index
                });

                row.OnlineState = info.State;
            }
        }

        public void RefreshBlockList()
        {
            BlockListBox.SelectedItem = null;

            foreach (DXListBoxItem item in BlockListBoxItems)
                item.Dispose();

            BlockListBoxItems.Clear();

            foreach (ClientBlockInfo info in CEnvir.BlockList)
            {
                BlockListBoxItems.Add(new DXListBoxItem
                {
                    Parent = BlockListBox,
                    Label = { Text = info.Name },
                    Item = info.Index
                });
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                RecipientValidChanged = null;
                GoldValidChanged = null;
                SendAttemptedChanged = null;
                ReadMailChanged = null;
                OnlineStateChanged = null;

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (BackgroundImage != null)
                {
                    if (!BackgroundImage.IsDisposed)
                        BackgroundImage.Dispose();

                    BackgroundImage = null;
                }

                if (FriendTab != null)
                {
                    if (!FriendTab.IsDisposed)
                        FriendTab.Dispose();

                    FriendTab = null;
                }

                if (ReceivedTab != null)
                {
                    if (!ReceivedTab.IsDisposed)
                        ReceivedTab.Dispose();

                    ReceivedTab = null;
                }

                if (SendTab != null)
                {
                    if (!SendTab.IsDisposed)
                        SendTab.Dispose();

                    SendTab = null;
                }

                if (BlockTab != null)
                {
                    if (!BlockTab.IsDisposed)
                        BlockTab.Dispose();

                    BlockTab = null;
                }

                if (ReadTab != null)
                {
                    if (!ReadTab.IsDisposed)
                        ReadTab.Dispose();

                    ReadTab = null;
                }

                if (FriendOnlineStateBox != null)
                {
                    if (!FriendOnlineStateBox.IsDisposed)
                        FriendOnlineStateBox.Dispose();

                    FriendOnlineStateBox = null;
                }

                if (FriendViewStatusBox != null)
                {
                    if (!FriendViewStatusBox.IsDisposed)
                        FriendViewStatusBox.Dispose();

                    FriendViewStatusBox = null;
                }

                if (FriendListBox != null)
                {
                    if (!FriendListBox.IsDisposed)
                        FriendListBox.Dispose();

                    FriendListBox = null;
                }

                if (FriendAddButton != null)
                {
                    if (!FriendAddButton.IsDisposed)
                        FriendAddButton.Dispose();

                    FriendAddButton = null;
                }

                if (FriendRemoveButton != null)
                {
                    if (!FriendRemoveButton.IsDisposed)
                        FriendRemoveButton.Dispose();

                    FriendRemoveButton = null;
                }

                foreach (var item in FriendListBoxItems)
                {
                    item.Dispose();
                }

                FriendList = null;
                FriendListBoxItems = null;

                if (RecievedCategoryLabel != null)
                {
                    if (!RecievedCategoryLabel.IsDisposed)
                        RecievedCategoryLabel.Dispose();

                    RecievedCategoryLabel = null;
                }

                if (RecievedTitleLabel != null)
                {
                    if (!RecievedTitleLabel.IsDisposed)
                        RecievedTitleLabel.Dispose();

                    RecievedTitleLabel = null;
                }

                if (ReceivedDateLabel != null)
                {
                    if (!ReceivedDateLabel.IsDisposed)
                        ReceivedDateLabel.Dispose();

                    ReceivedDateLabel = null;
                }

                if (RecievedScrollBar != null)
                {
                    if (!RecievedScrollBar.IsDisposed)
                        RecievedScrollBar.Dispose();

                    RecievedScrollBar = null;
                }

                foreach (var row in ReceivedRows)
                {
                    row.Dispose();
                }

                ReceivedRows = null;
                ReceivedMailList = null;

                if (RecievedScrollBar != null)
                {
                    if (!RecievedScrollBar.IsDisposed)
                        RecievedScrollBar.Dispose();

                    RecievedScrollBar = null;
                }

                if (ReceivedCollectAllButton != null)
                {
                    if (!ReceivedCollectAllButton.IsDisposed)
                        ReceivedCollectAllButton.Dispose();

                    ReceivedCollectAllButton = null;
                }

                if (ReceivedDeleteAll != null)
                {
                    if (!ReceivedDeleteAll.IsDisposed)
                        ReceivedDeleteAll.Dispose();

                    ReceivedDeleteAll = null;
                }

                if (ReceivedNewButton != null)
                {
                    if (!ReceivedNewButton.IsDisposed)
                        ReceivedNewButton.Dispose();

                    ReceivedNewButton = null;
                }

                if (SendRecipientBox != null)
                {
                    if (!SendRecipientBox.IsDisposed)
                        SendRecipientBox.Dispose();

                    SendRecipientBox = null;
                }

                if (SendSubjectBox != null)
                {
                    if (!SendSubjectBox.IsDisposed)
                        SendSubjectBox.Dispose();

                    SendSubjectBox = null;
                }

                if (SendMessageBox != null)
                {
                    if (!SendMessageBox.IsDisposed)
                        SendMessageBox.Dispose();

                    SendMessageBox = null;
                }

                if (SendMessageScrollBar != null)
                {
                    if (!SendMessageScrollBar.IsDisposed)
                        SendMessageScrollBar.Dispose();

                    SendMessageScrollBar = null;
                }

                SendMailItems = null;

                if (SendGrid != null)
                {
                    if (!SendGrid.IsDisposed)
                        SendGrid.Dispose();

                    SendGrid = null;
                }

                if (SendGoldBox != null)
                {
                    if (!SendGoldBox.IsDisposed)
                        SendGoldBox.Dispose();

                    SendGoldBox = null;
                }

                if (SendButton != null)
                {
                    if (!SendButton.IsDisposed)
                        SendButton.Dispose();

                    SendButton = null;
                }

                if (ReadSenderBox != null)
                {
                    if (!ReadSenderBox.IsDisposed)
                        ReadSenderBox.Dispose();

                    ReadSenderBox = null;
                }

                if (ReadSubjectBox != null)
                {
                    if (!ReadSubjectBox.IsDisposed)
                        ReadSubjectBox.Dispose();

                    ReadSubjectBox = null;
                }

                if (ReadDateBox != null)
                {
                    if (!ReadDateBox.IsDisposed)
                        ReadDateBox.Dispose();

                    ReadDateBox = null;
                }
                
                if (ReadMessageContainer != null)
                {
                    if (!ReadMessageContainer.IsDisposed)
                        ReadMessageContainer.Dispose();

                    ReadMessageContainer = null;
                }
                if (ReadMessageLabel != null)
                {
                    if (!ReadMessageLabel.IsDisposed)
                        ReadMessageLabel.Dispose();

                    ReadMessageLabel = null;
                }

                if (ReadMessageScrollBar != null)
                {
                    if (!ReadMessageScrollBar.IsDisposed)
                        ReadMessageScrollBar.Dispose();

                    ReadMessageScrollBar = null;
                }   

                if (ReadReplyButton != null)
                {
                    if (!ReadReplyButton.IsDisposed)
                        ReadReplyButton.Dispose();

                    ReadReplyButton = null;
                }

                if (ReadDeleteButton != null)
                {
                    if (!ReadDeleteButton.IsDisposed)
                        ReadDeleteButton.Dispose();

                    ReadDeleteButton = null;
                }

                if (ReadGrid != null)
                {
                    if (!ReadGrid.IsDisposed)
                        ReadGrid.Dispose();

                    ReadGrid = null;
                }

                ReadMailItems = null;

                if (BlockAddButton != null)
                {
                    if (!BlockAddButton.IsDisposed)
                        BlockAddButton.Dispose();

                    BlockAddButton = null;
                }

                if (BlockRemoveButton != null)
                {
                    if (!BlockRemoveButton.IsDisposed)
                        BlockRemoveButton.Dispose();

                    BlockRemoveButton = null;
                }

                if (BlockListBox != null)
                {
                    if (!BlockListBox.IsDisposed)
                        BlockListBox.Dispose();

                    BlockListBox = null;
                }

                foreach (var item in BlockListBoxItems)
                {
                    if (item != null)
                    {
                        if (!item.IsDisposed)
                            item.Dispose();
                    }
                }

                BlockListBoxItems = null;
            }

        }

        #endregion
    }

    public class FriendRow : DXListBoxItem
    {
        public DXLabel StatusLabel { get; protected set; }

        //Needed as default value still needs setting the first time
        private bool _StateSet = false; 

        #region OnlineState

        public OnlineState OnlineState
        {
            get => _OnlineState;
            set
            {
                if (_OnlineState == value && _StateSet) return;

                OnlineState oldValue = _OnlineState;
                _OnlineState = value;
                _StateSet = true;

                OnOnlineStateChanged(oldValue, value);
            }
        }
        private OnlineState _OnlineState;
        public event EventHandler<EventArgs> OnlineStateChanged;
        public void OnOnlineStateChanged(OnlineState oValue, OnlineState nValue)
        {
            OnlineStateChanged?.Invoke(this, EventArgs.Empty);

            UpdateStateLabel();
        }

        #endregion

        public FriendRow()
        {
            StatusLabel = new DXLabel
            {
                Parent = this,
                Text = "",
                IsControl = false
            };
        }

        #region Methods
        public override void UpdateColours()
        {
            if (Selected)
            {
                Label.ForeColour = Color.White;
                BackColour = Color.FromArgb(128, 64, 64);
            }
            else if (MouseControl == this)
            {
                Label.ForeColour = Color.FromArgb(198, 166, 99);
                BackColour = Color.FromArgb(64, 32, 32);
            }
            else
            {
                Label.ForeColour = Color.FromArgb(198, 166, 99);
                BackColour = Color.Empty;
            }
        }
        
        public void UpdateStateLabel()
        {
            StatusLabel.Text = $"({OnlineState})";
            StatusLabel.Location = new Point(Size.Width - StatusLabel.Size.Width, 0);

            switch (OnlineState)
            {
                case OnlineState.Online:
                    StatusLabel.ForeColour = Color.LimeGreen;
                    break;
                case OnlineState.Away:
                    StatusLabel.ForeColour = Color.Orange;
                    break;
                case OnlineState.Busy:
                    StatusLabel.ForeColour = Color.Red;
                    break;
                case OnlineState.Offline:
                    StatusLabel.ForeColour = Color.Gray;
                    break;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (StatusLabel != null)
                {
                    if (!StatusLabel.IsDisposed)
                        StatusLabel.Dispose();

                    StatusLabel = null;
                }

                OnlineStateChanged = null;
                _StateSet = false;
            }
        }

        #endregion
    }

    public sealed class CommunicationReceivedRow : DXControl
    {
        #region Properties

        #region Mail

        public ClientMailInfo Mail
        {
            get => _Mail;
            set
            {
                ClientMailInfo oldValue = _Mail;
                _Mail = value;

                OnMailChanged(oldValue, value);
            }
        }
        private ClientMailInfo _Mail;
        public event EventHandler<EventArgs> MailChanged;
        public void OnMailChanged(ClientMailInfo oValue, ClientMailInfo nValue)
        {
            Visible = nValue != null;
            if (nValue == null) return;

            SubjectLabel.Text = Mail.Subject;
            SenderLabel.Text = Mail.Sender;
            DateLabel.Text = Mail.Date.ToString("ddd, dd MMM yy");

            RefreshIcon();

            MailChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXImageControl Icon;
        public DXLabel SubjectLabel, SenderLabel, DateLabel;

        #endregion

        public CommunicationReceivedRow()
        {
            Size = new Size(236, 49);
            DrawTexture = true;
            BackColour = Color.Empty;

            Icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3680,
                IsControl = false,
                Location = new Point(6, 7),
            };

            SubjectLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(135, 20),
                Location = new Point(47, 5),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                IsControl = false,
            };

            SenderLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(135, 15),
                Location = new Point(47, 25),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                IsControl = false,
            };

            DateLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(50, 50),
                Location = new Point(185, 0),
                Border = true,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak,
                Parent = this,
                IsControl = false,
            };
        }

        #region Methods
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            BackColour = Color.FromArgb(100, 80, 80, 125);
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            BackColour = Color.Empty;
        }

        public void RefreshIcon()
        {
            //if (Mail.HasItem)
            //    Icon.Index = Mail.Items.Count == 0 ? 231 : 230;
            //else
            //    Icon.Index = Mail.Opened ? 234 : 233;

            Icon.Index = Mail.Opened ? 0 : 3680;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Mail = null;
                MailChanged = null;

                if (Icon != null)
                {
                    if (!Icon.IsDisposed)
                        Icon.Dispose();

                    Icon = null;
                }

                if (SubjectLabel != null)
                {
                    if (!SubjectLabel.IsDisposed)
                        SubjectLabel.Dispose();

                    SubjectLabel = null;
                }

                if (SenderLabel != null)
                {
                    if (!SenderLabel.IsDisposed)
                        SenderLabel.Dispose();

                    SenderLabel = null;
                }

                if (DateLabel != null)
                {
                    if (!DateLabel.IsDisposed)
                        DateLabel.Dispose();

                    DateLabel = null;
                }
            }
        }

        #endregion
    }
}
