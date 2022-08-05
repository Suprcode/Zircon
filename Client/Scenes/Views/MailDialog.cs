using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class MailDialog : DXWindow
    {
        #region Properites

        public DXButton CollectAllButton, DeleteAll, NewButton;

        public MailRow Header;

        public DXVScrollBar ScrollBar;

        public MailRow[] Rows;
        
        public List<ClientMailInfo> MailList = new List<ClientMailInfo>();

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            
            if (!Visible && GameScene.Game.ReadMailBox != null)
                GameScene.Game.ReadMailBox.Visible = false;
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
                RefreshList();
        }

        public override WindowType Type => WindowType.MailBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;
        #endregion

        public MailDialog()
        {
            TitleLabel.Text = "Mail Box";
            HasFooter = true;

            SetClientSize(new Size(350, 350));
            
            //Rows
            Header = new MailRow
            {
                Parent = this,
                Location = ClientArea.Location,
                IsHeader = true,
            };

            Rows = new MailRow[15];
            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(14, ClientArea.Height - 2 - 22),
                Location = new Point(ClientArea.Right - 14, ClientArea.Top + 1 + 22),
                VisibleSize = 15,
                Change = 3,
            };
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;
            MouseWheel += ScrollBar.DoMouseWheel;

            DXControl panel = new DXControl
            {
                Parent = this,
                Location = new Point(ClientArea.Location.X, ClientArea.Location.Y + Header.Size.Height + 2),
                Size = new Size(ClientArea.Width - 16, ClientArea.Size.Height - 22)
            };

            for (int i = 0; i < 15; i++)
            {
                int index = i;
                Rows[index] = new MailRow
                {
                    Parent = panel,
                    Location = new Point(0, 22 * i),
                    Visible = false,
                };
                Rows[index].MouseClick += (o, e) =>
                {
                    GameScene.Game.ReadMailBox.Mail = Rows[index].Mail;

                    if (Rows[index].Mail.Opened) return;

                    Rows[index].Mail.Opened = true;
                    Rows[index].RefreshIcon();
                    GameScene.Game.MailBox.UpdateIcon();

                    CEnvir.Enqueue(new C.MailOpened { Index = Rows[index].Mail.Index });
                };
                Rows[index].MouseWheel += ScrollBar.DoMouseWheel;
            }

            CollectAllButton = new DXButton
            {
                Location = new Point(ClientArea.Right - 100, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Collect Some" }
            };
            CollectAllButton.MouseClick += (o, e) =>
            {
                int count = 15;
                foreach (ClientMailInfo mail in MailList)
                {
                    if (count <= 0) break;

                    if (mail.Items.Count == 0) continue;

                    if (!mail.Opened)
                    {
                        mail.Opened = true;
                        CEnvir.Enqueue(new C.MailOpened { Index = mail.Index });
                        count--;
                        foreach (MailRow row in Rows)
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

                GameScene.Game.MailBox.UpdateIcon();
            };


            DeleteAll = new DXButton
            {
                Location = new Point(ClientArea.Right - 200, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Delete Some" }
            };
            DeleteAll.MouseClick += (o, e) =>
            {
                int count = 15;
                foreach (ClientMailInfo mail in MailList)
                {
                    if (count <= 0) break;
                    if (mail.Items.Count > 0) continue;

                    CEnvir.Enqueue(new C.MailDelete { Index = mail.Index });
                    count--;
                }
            };

            NewButton = new DXButton
            {
                Location = new Point(ClientArea.Right - 300, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "New Mail" }
            };
            NewButton.MouseClick += (o, e) =>
            {
                GameScene.Game.SendMailBox.Visible = true;
                GameScene.Game.SendMailBox.BringToFront();
            };

        }

        #region Methods

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }
        
        public void RefreshList()
        {
            if (Rows == null) return;

            MailList.Sort((x1, x2) =>
            {
                int value = x2.Date.CompareTo(x1.Date);

                if (value == 0)
                    return x1.Index.CompareTo(x2.Index);
                return value;
            });

            ScrollBar.MaxValue = MailList.Count;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (i + ScrollBar.Value >= MailList.Count)
                {
                    Rows[i].Mail = null;
                    continue;
                }
                
                Rows[i].Mail = MailList[i + ScrollBar.Value];
            }

        }


        public void UpdateIcon()
        {
            GameScene.Game.MainPanel.NewMailIcon.Visible = MailList.Any(x => !x.Opened);
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

                if (CollectAllButton != null)
                {
                    if (!CollectAllButton.IsDisposed)
                        CollectAllButton.Dispose();

                    CollectAllButton = null;
                }

                if (DeleteAll != null)
                {
                    if (!DeleteAll.IsDisposed)
                        DeleteAll.Dispose();

                    DeleteAll = null;
                }

                if (NewButton != null)
                {
                    if (!NewButton.IsDisposed)
                        NewButton.Dispose();

                    NewButton = null;
                }

                if (Header != null)
                {
                    if (!Header.IsDisposed)
                        Header.Dispose();

                    Header = null;
                }
                
                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (NewButton != null)
                {
                    if (!NewButton.IsDisposed)
                        NewButton.Dispose();

                    NewButton = null;
                }

                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }
                    }

                    Rows = null;
                }

                MailList.Clear();
                MailList = null;
            }

        }

        #endregion
    }

    public sealed class MailRow : DXControl
    {
        #region Properties

        #region IsHeader

        public bool IsHeader
        {
            get => _IsHeader;
            set
            {
                if (_IsHeader == value) return;

                bool oldValue = _IsHeader;
                _IsHeader = value;

                OnIsHeaderChanged(oldValue, value);
            }
        }
        private bool _IsHeader;
        public event EventHandler<EventArgs> IsHeaderChanged;
        public void OnIsHeaderChanged(bool oValue, bool nValue)
        {
            SubjectLabel.Text = "Subject";
            SenderLabel.Text = "Sender";
            DateLabel.Text = "Date";

            SubjectLabel.ForeColour = Color.FromArgb(198, 166, 99);
            SenderLabel.ForeColour = Color.FromArgb(198, 166, 99);
            DateLabel.ForeColour = Color.FromArgb(198, 166, 99);

            Icon.Visible = false;
            DrawTexture = false;
            IsControl = false;

            IsHeaderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

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
            DateLabel.Text = Mail.Date.ToShortDateString();

            RefreshIcon();

            MailChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXImageControl Icon;
        public DXLabel SubjectLabel, SenderLabel, DateLabel;
        #endregion

        public MailRow()
        {
            Size = new Size(333, 20);
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);


            //230, 231, 233, 234

            Icon = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 234,
                IsControl = false,
                Location = new Point(5,2),
            };

            SubjectLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(130, 20),
                Location = new Point(30,2),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this,
                IsControl = false,
            };

            SenderLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(100, 20),
                Location = new Point(SubjectLabel.Location.X + SubjectLabel.Size.Width, 2),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this,
                IsControl = false,
            };

            DateLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(70, 20),
                Location = new Point(SenderLabel.Location.X + SenderLabel.Size.Width, 2),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this,
                IsControl = false,
            };
        }

        #region Methods
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (IsHeader) return;

            BackColour = Color.FromArgb(80, 80, 125);
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (IsHeader) return;

            BackColour = Color.FromArgb(25, 20, 0);
        }
        public void RefreshIcon()
        {
            if (Mail.HasItem)
                Icon.Index = Mail.Items.Count == 0 ? 231 : 230;
            else
                Icon.Index = Mail.Opened ? 234 : 233;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _IsHeader = false;
                IsHeaderChanged = null;

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

    public sealed class ReadMailDialog : DXWindow
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
            Visible = Mail != null;

            if (Mail == null) return;

            SenderBox.TextBox.Text = Mail.Sender;
            SubjectBox.TextBox.Text = Mail.Subject;
            DateBox.TextBox.Text = Mail.Date.ToLongDateString() + " " + Mail.Date.ToLongTimeString();
            MessageLabel.Text = Mail.Message;

            foreach (DXItemCell cell in Grid.Grid)
                cell.Item = Mail.Items.FirstOrDefault(x => x.Slot == cell.Slot);

            MailChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXTextBox SenderBox, SubjectBox, DateBox;
        public DXLabel MessageLabel;
        public DXButton ReplyButton, DeleteButton;
        public DXItemGrid Grid;

        public ClientUserItem[] MailItems;

        public override WindowType Type => WindowType.ReadMailBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public ReadMailDialog()
        {
            SetClientSize(new Size(255, 340));
            TitleLabel.Text = "Read Mail";

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "Sender:"
            };
            label.Location = new Point(ClientArea.X + 50 - label.Size.Width, ClientArea.Y);

            SenderBox = new DXTextBox
            {
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ReadOnly = true,
                Editable = false,
                Parent = this,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(120, 18)
            };

            ReplyButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(70, SmallButtonHeight),
                Location = new Point(SenderBox.Location.X + SenderBox.Size.Width + 10, label.Location.Y),
                Parent = this,
                Label = { Text = "Reply" }
            };
            ReplyButton.MouseClick += (o, e) =>
            {
                GameScene.Game.SendMailBox.Visible = true;
                GameScene.Game.SendMailBox.BringToFront();
                GameScene.Game.SendMailBox.RecipientBox.TextBox.Text = Mail.Sender;
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "Subject:"
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, SenderBox.Location.Y + 25);

            SubjectBox = new DXTextBox
            {
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ReadOnly = true,
                Editable = false,
                Parent = this,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(195, 18)
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "Date:"
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, SubjectBox.Location.Y + 25);

            DateBox = new DXTextBox
            {
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                ReadOnly = true,
                Editable = false,
                Parent = this,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(195, 18)
            };


            label = new DXLabel
            {
                Parent = this,
                Text = "Message:"
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, DateBox.Location.Y + 25);

            MessageLabel = new DXLabel
            {
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                Location = new Point(ClientArea.X + 5, label.Location.Y + 20),
                Size = new Size(ClientArea.Width - 10, 160),
                AutoSize = false,
            };


            label = new DXLabel
            {
                Parent = this,
                Text = "Items:",
                Location = new Point(ClientArea.X, MessageLabel.Location.Y + 5 + MessageLabel.Size.Height)
            };

            MailItems = new ClientUserItem[7];

            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 1),
                Parent = this,
                Location = new Point(ClientArea.X + 5, label.Location.Y + 20),
                ItemGrid = MailItems,
            };
            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.ReadOnly = true;
                cell.MouseClick += (o, e) =>
                {
                    if (cell.Item == null) return;
                    CEnvir.Enqueue(new C.MailGetItem { Index = Mail.Index, Slot = cell.Slot });
                };
            }

            DeleteButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(70, SmallButtonHeight),
                Location = new Point(ClientArea.Right - 75, Grid.Location.Y + Grid.Size.Height + 5),
                Parent = this,
                Label = { Text = "Delete" }
            };
            DeleteButton.MouseClick += (o, e) =>
            {
                if (Mail.Items.Count > 0)
                {
                    GameScene.Game.ReceiveChat("You cannot delete a mail with items inside", MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.MailDelete { Index = Mail.Index });
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Mail = null;
                MailChanged = null;

                if (SenderBox != null)
                {
                    if (!SenderBox.IsDisposed)
                        SenderBox.Dispose();

                    SenderBox = null;
                }

                if (SubjectBox != null)
                {
                    if (!SubjectBox.IsDisposed)
                        SubjectBox.Dispose();

                    SubjectBox = null;
                }

                if (DateBox != null)
                {
                    if (!DateBox.IsDisposed)
                        DateBox.Dispose();

                    DateBox = null;
                }

                if (MessageLabel != null)
                {
                    if (!MessageLabel.IsDisposed)
                        MessageLabel.Dispose();

                    MessageLabel = null;
                }

                if (ReplyButton != null)
                {
                    if (!ReplyButton.IsDisposed)
                        ReplyButton.Dispose();

                    ReplyButton = null;
                }

                if (DeleteButton != null)
                {
                    if (!DeleteButton.IsDisposed)
                        DeleteButton.Dispose();

                    DeleteButton = null;
                }

                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                MailItems = null;
            }

        }

        #endregion
    }


    public sealed class SendMailDialog : DXWindow
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

        public bool CanSend => !SendAttempted && RecipientValid && GoldValid;

        public DXTextBox RecipientBox, SubjectBox, MessageBox;
        public DXNumberBox GoldBox;
        public DXButton SendButton;
        public DXItemGrid Grid;

        public ClientUserItem[] MailItems;

        public override WindowType Type => WindowType.SendMailBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public SendMailDialog()
        {
            SetClientSize(new Size(255, 320));
            TitleLabel.Text = "New Mail";

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "Recipient:"
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, ClientArea.Y);

            RecipientBox = new DXTextBox
            {
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Parent = this,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(120, 18)
            };
            RecipientBox.TextBox.TextChanged += RecipientBox_TextChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = "Subject:"
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, RecipientBox.Location.Y + 25);

            SubjectBox = new DXTextBox
            {
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Parent = this,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(195, 18),
                MaxLength = 30,
            };
            

            label = new DXLabel
            {
                Parent = this,
                Text = "Message:"
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, SubjectBox.Location.Y + 25);

            MessageBox = new DXTextBox
            {
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                TextBox = { Multiline = true, AcceptsReturn = true, },
                Location = new Point(ClientArea.X + 5, label.Location.Y + 20),
                Size = new Size(ClientArea.Width - 10, 160),
                MaxLength = 300
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "Items:",
                Location = new Point(ClientArea.X + 36, MessageBox.Location.Y + 5 + MessageBox.Size.Height)
            };

            MailItems = new ClientUserItem[6];

            Grid = new DXItemGrid
            {
                GridSize = new Size(6, 1),
                Parent = this,
                Location = new Point(ClientArea.X + 5 + 36, label.Location.Y + 20),
                Linked = true,
                GridType = GridType.SendMail,
            };


            label = new DXLabel
            {
                Parent = this,
                Text = "Gold:"
            };
            label.Location = new Point(ClientArea.X + 42 - label.Size.Width, Grid.Location.Y + 10 + Grid.Size.Height);

            GoldBox = new DXNumberBox
            {
                Parent = this,
                Location = new Point(ClientArea.X + 5 + 36, label.Location.Y),
                UpButton = { Visible = false },
                DownButton = { Visible = false },
                Size = new Size(102, 18),
                ValueTextBox = { Location = new Point(1,1), Size = new Size(100,16)},
                MaxValue = 2000000000
            };
            GoldBox.ValueTextBox.ValueChanged += GoldBox_ValueChanged;

            SendButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(70, SmallButtonHeight),
                Location = new Point(ClientArea.Right - 75, GoldBox.Location.Y),
                Parent = this,
                Label = { Text = "Send" },
                Enabled = false,
            };
            SendButton.MouseClick += (o, e) => Send();


            GoldValid = true;
        }

        #region Methods

        private void GoldBox_ValueChanged(object sender, EventArgs e)
        {
            GoldValid = GoldBox.Value >= 0 && GoldBox.Value <= MapObject.User.Gold.Amount;

            if (GoldBox.Value == 0)
                GoldBox.ValueTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                GoldBox.ValueTextBox.BorderColour = GoldValid ? Color.Green : Color.Red;
        }

        private void RecipientBox_TextChanged(object sender, EventArgs e)
        {
            RecipientValid = Globals.CharacterReg.IsMatch(RecipientBox.TextBox.Text);

            if (string.IsNullOrEmpty(RecipientBox.TextBox.Text))
                RecipientBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                RecipientBox.BorderColour = RecipientValid ? Color.Green : Color.Red;

        }

        public void Send()
        {
            SendAttempted = true;


            List<CellLinkInfo> links = new List<CellLinkInfo>();

            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link == null) continue;

                links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = true;
                cell.Link = null;
            }

            CEnvir.Enqueue(new C.MailSend { Links = links, Recipient = RecipientBox.TextBox.Text, Subject = SubjectBox.TextBox.Text, Message = MessageBox.TextBox.Text, Gold = GoldBox.Value });

            RecipientBox.TextBox.Text = string.Empty;
            MessageBox.TextBox.Text = string.Empty;
            SubjectBox.TextBox.Text = string.Empty;

            GoldBox.Value = 0;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _RecipientValid = false;
                RecipientValidChanged = null;

                _GoldValid = false;
                GoldValidChanged = null;

                _SendAttempted = false;
                SendAttemptedChanged = null;
                
                if (RecipientBox != null)
                {
                    if (!RecipientBox.IsDisposed)
                        RecipientBox.Dispose();

                    RecipientBox = null;
                }

                if (SubjectBox != null)
                {
                    if (!SubjectBox.IsDisposed)
                        SubjectBox.Dispose();

                    SubjectBox = null;
                }

                if (MessageBox != null)
                {
                    if (!MessageBox.IsDisposed)
                        MessageBox.Dispose();

                    MessageBox = null;
                }

                if (GoldBox != null)
                {
                    if (!GoldBox.IsDisposed)
                        GoldBox.Dispose();

                    GoldBox = null;
                }

                if (SendButton != null)
                {
                    if (!SendButton.IsDisposed)
                        SendButton.Dispose();

                    SendButton = null;
                }
                
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                MailItems = null;
            }

        }

        #endregion
    }
}
