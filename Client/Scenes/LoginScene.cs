using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Scenes.Views;
using Client.UserModels;
using Library;
using SlimDX.Direct3D9;
using C = Library.Network.ClientPackets;
using Font = System.Drawing.Font;

//Cleaned
namespace Client.Scenes
{
    public class LoginScene : DXScene
    {
        #region Properties

        #region Loaded

        public bool Loaded
        {
            get { return _Loaded; }
            set
            {
                if (_Loaded == value) return;

                bool oldValue = _Loaded;
                _Loaded = value;

                OnLoadedChanged(oldValue, value);
            }
        }
        private bool _Loaded;
        public event EventHandler<EventArgs> LoadedChanged;
        public void OnLoadedChanged(bool oValue, bool nValue)
        {
            foreach (DXWindow window in DXWindow.Windows)
                window.LoadSettings();

            LoadedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ConnectionAttempt

        public int ConnectionAttempt
        {
            get => _ConnectionAttempt;
            set
            {
                if (_ConnectionAttempt == value) return;

                int oldValue = _ConnectionAttempt;
                _ConnectionAttempt = value;

                OnConnectionAttemptChanged(oldValue, value);
            }
        }
        private int _ConnectionAttempt;
        public event EventHandler<EventArgs> ConnectionAttemptChanged;
        public virtual void OnConnectionAttemptChanged(int oValue, int nValue)
        {
            ConnectionAttemptChanged?.Invoke(this, EventArgs.Empty);

            string message = string.Format(CEnvir.Language.LoginConnectionAttemptMessage, ConnectionAttempt);

            if (ConnectionBox == null)
            {
                ConnectionBox = new DXMessageBox(message, CEnvir.Language.LoginConnectionAttemptCaption, DXMessageBoxButtons.Cancel);
                ConnectionBox.Disposing += (o, e1) => ConnectionBox = null;
                ConnectionBox.CancelButton.MouseClick += (o, e1) => CEnvir.Target.Close();
                ConnectionBox.CloseButton.Visible = false;

                ConnectionBox.Modal = false;

                LoginBox.Visible = false;
            }
            else
                ConnectionBox.Label.Text = message;
        }

        #endregion

        public DXMessageBox ConnectionBox;
        public DXConfigWindow ConfigBox;
        public LoginDialog LoginBox;
        public NewAccountDialog AccountBox;
        public ChangePasswordDialog ChangeBox;
        public RequestResetPasswordDialog RequestPasswordBox;
        public ResetPasswordDialog ResetBox;
        public ActivationDialog ActivationBox;
        public RequestActivationKeyDialog RequestActivationBox;
        public RankingDialog RankingBox;

        public DXImageControl Logo, LogoBackground;

        private TcpClient ConnectingClient;
        private DateTime ConnectionTime;

        #endregion

        public LoginScene(Size size) : base(size)
        {
            DXImageControl background = new DXImageControl
            {
                Index = 20,
                LibraryFile = LibraryFile.Interface1c,
                Parent = this,
            };

            DXAnimatedControl control = new DXAnimatedControl
            {
                BaseIndex = 2200,
                LibraryFile = LibraryFile.Interface1c,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(10),
                FrameCount = 100,
                Parent = background,
                Loop = false,
                UseOffSet = true,
            };
            control.BeforeDraw += (o, e) =>
            {
                DXManager.Device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
            };
            control.AfterDraw += (o, e) =>
            {
                DXManager.Device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
            };

            control = new DXAnimatedControl
            {
                BaseIndex = 2400,
                LibraryFile = LibraryFile.Interface1c,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(5),
                FrameCount = 30,
                Parent = background,
                UseOffSet = true,
            };
            control.BeforeDraw += (o, e) =>
            {
                DXManager.Device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.None);
            };
            control.AfterDraw += (o, e) =>
            {
                DXManager.Device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
            };

            new DXAnimatedControl
            {
                BaseIndex = 2300,
                LibraryFile = LibraryFile.Interface1c,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(10),
                FrameCount = 30,
                Parent = background,
                Blend = true,
            };


            new DXAnimatedControl
            {
                BaseIndex = 2500,
                LibraryFile = LibraryFile.Interface1c,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(8),
                FrameCount = 30,
                Parent = background,
                UseOffSet = true,
            };

            LogoBackground = new DXImageControl
            {
                Index = 23,
                LibraryFile = LibraryFile.Interface1c,
                Parent = this
            };

            Logo = new DXImageControl
            {
                Index = 22,
                LibraryFile = LibraryFile.Interface1c,
                Parent = LogoBackground,
                Blend = true,
                BlendMode = BlendMode.HIGHLIGHT,
                FixedSize = true,
                Size = new Size(564, 300)
            };

            LogoBackground.Location = new Point((Size.Width - LogoBackground.Size.Width) / 2, 25);
            Logo.Location = new Point(((Size.Width - Logo.Size.Width) / 2) - 270, -27);

            ConfigBox = new DXConfigWindow
            {
                Parent = this,
                Visible = false
            };
            ConfigBox.Location = new Point((Size.Width - ConfigBox.Size.Width) / 2, (Size.Height - ConfigBox.Size.Height) / 2);

            LoginBox = new LoginDialog
            {
                Parent = this,
            };
            LoginBox.Location = new Point((Size.Width - LoginBox.Size.Width) / 2, Size.Height - LoginBox.Size.Height - 20);

            RankingBox = new RankingDialog
            {
                Parent = this,
                Visible = false,
                ObservableBox = { Visible = false }
            };
            RankingBox.Location = new Point((Size.Width - RankingBox.Size.Width) / 2, (Size.Height - RankingBox.Size.Height) / 2);


            AccountBox = new NewAccountDialog
            {
                Parent = this,
            };
            AccountBox.Location = new Point((Size.Width - AccountBox.Size.Width) / 2, (Size.Height - AccountBox.Size.Height) / 2);


            ChangeBox = new ChangePasswordDialog
            {
                Parent = this,
            };
            ChangeBox.Location = new Point((Size.Width - ChangeBox.Size.Width) / 2, (Size.Height - ChangeBox.Size.Height) / 2);


            RequestPasswordBox = new RequestResetPasswordDialog
            {
                Parent = this,
            };
            RequestPasswordBox.Location = new Point((Size.Width - RequestPasswordBox.Size.Width) / 2, (Size.Height - RequestPasswordBox.Size.Height) / 2);

            ResetBox = new ResetPasswordDialog
            {
                Parent = this,
            };
            ResetBox.Location = new Point((Size.Width - ResetBox.Size.Width) / 2, (Size.Height - ResetBox.Size.Height) / 2);

            ActivationBox = new ActivationDialog
            {
                Parent = this,
            };
            ActivationBox.Location = new Point((Size.Width - ActivationBox.Size.Width) / 2, (Size.Height - ActivationBox.Size.Height) / 2);

            RequestActivationBox = new RequestActivationKeyDialog
            {
                Parent = this,
            };
            RequestActivationBox.Location = new Point((Size.Width - RequestActivationBox.Size.Width) / 2, (Size.Height - RequestActivationBox.Size.Height) / 2);



            DXSoundManager.Play(SoundIndex.LoginScene2);
        }

        #region Methods

        public override void Process()
        {
            base.Process();

            Loaded = CEnvir.Loaded;

            if (!CEnvir.Loaded)
            {
                string message = CEnvir.Language.LoginLoadingMessage;

                if (ConnectionBox == null)
                {
                    ConnectionBox = new DXMessageBox(message, CEnvir.Language.LoginLoadingCaption, DXMessageBoxButtons.Cancel);

                    ConnectionBox.Disposing += (o, e1) => ConnectionBox = null;
                    ConnectionBox.CancelButton.MouseClick += (o, e1) => CEnvir.Target.Close();
                    ConnectionBox.CloseButton.Visible = false;
                    ConnectionBox.Modal = false;

                    LoginBox.Visible = false;
                }
                else
                    ConnectionBox.Label.Text = message;
            }

            if (CEnvir.WrongVersion)
            {
                if (ConnectingClient != null)
                {
                    ConnectingClient.Close();
                    ConnectingClient = null;
                }

                if (ConnectionBox == null) return;

                if (!ConnectionBox.IsDisposed)
                    ConnectionBox.Dispose();

                ConnectionBox = null;
                return;
            }

            if (CEnvir.Connection != null && CEnvir.Connection.ServerConnected)
            {
                if (CEnvir.Loaded && !LoginBox.Visible && !AccountBox.Visible && !ChangeBox.Visible && !ResetBox.Visible)
                {
                    if (ConnectionBox != null && !ConnectionBox.IsDisposed)
                        ConnectionBox.Dispose();

                    LoginBox.Visible = true;
                }
                return;
            }

            if (CEnvir.Now < ConnectionTime) return;

            ConnectingClient?.Close();
            ConnectingClient = new TcpClient();
            if (Config.UseNetworkConfig)
                ConnectingClient.BeginConnect(Config.IPAddress, Config.Port, Connecting, ConnectingClient);
            else
                ConnectingClient.BeginConnect(Config.DefaultIPAddress, Config.DefaultPort, Connecting, ConnectingClient);

            ConnectionTime = CEnvir.Now.AddSeconds(5);
            ConnectionAttempt++;
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled) return;
            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.ConfigWindow:
                        ConfigBox.Visible = !ConfigBox.Visible;
                        break;
                    case KeyBindAction.RankingWindow:
                        RankingBox.Visible = !RankingBox.Visible && CEnvir.Connection != null;
                        break;
                    default:
                        continue;
                }

                e.Handled = true;
            }
        }

        private void Connecting(IAsyncResult result)
        {
            try
            {
                TcpClient client = (TcpClient)result.AsyncState;
                client.EndConnect(result);

                if (!client.Connected) return;

                if (client != ConnectingClient)
                {
                    ConnectingClient = null;
                    client.Close();
                    return;
                }

                ConnectionTime = CEnvir.Now.AddSeconds(5); //Add 5 more seconds to timeout for delayed HandShake
                ConnectingClient = null;

                CEnvir.Connection = new CConnection(client);
            }
            catch { }
        }

        public void LoadDatabase()
        {
            CEnvir.LoadDatabase();
        }

        public void Disconnected()
        {
            _ConnectionAttempt = 0;
            ConnectionTime = DateTime.MinValue;
            ConnectingClient = null;


            if (LoginBox != null) LoginBox.LoginAttempted = false;
            if (AccountBox != null) AccountBox.CreateAttempted = false;
            if (ChangeBox != null) ChangeBox.ChangeAttempted = false;
        }

        public void ShowActivationBox(DXControl window)
        {
            window.Visible = false;
            ActivationBox.Visible = true;
            ActivationBox.PreviousWindow = window;
            ActivationBox.BringToFront();
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ConnectionBox != null)
                {
                    if (!ConnectionBox.IsDisposed)
                        ConnectionBox.Dispose();

                    ConnectionBox = null;
                }

                if (ConfigBox != null)
                {
                    if (!ConfigBox.IsDisposed)
                        ConfigBox.Dispose();

                    ConfigBox = null;
                }

                if (LoginBox != null)
                {
                    if (!LoginBox.IsDisposed)
                        LoginBox.Dispose();

                    LoginBox = null;
                }

                if (AccountBox != null)
                {
                    if (!AccountBox.IsDisposed)
                        AccountBox.Dispose();

                    AccountBox = null;
                }

                if (ChangeBox != null)
                {
                    if (!ChangeBox.IsDisposed)
                        ChangeBox.Dispose();
                    ChangeBox = null;
                }

                if (RequestPasswordBox != null)
                {
                    if (!RequestPasswordBox.IsDisposed)
                        RequestPasswordBox.Dispose();

                    RequestPasswordBox = null;
                }

                if (ResetBox != null)
                {
                    if (!ResetBox.IsDisposed)
                        ResetBox.Dispose();

                    ResetBox = null;
                }

                if (ActivationBox != null)
                {
                    if (!ActivationBox.IsDisposed)
                        ActivationBox.Dispose();

                    ActivationBox = null;
                }

                if (RequestActivationBox != null)
                {
                    if (!RequestActivationBox.IsDisposed)
                        RequestActivationBox.Dispose();

                    RequestActivationBox = null;
                }

                if (RankingBox != null)
                {
                    if (!RankingBox.IsDisposed)
                        RankingBox.Dispose();

                    RankingBox = null;
                }

                if (Logo != null)
                {
                    if (!Logo.IsDisposed)
                        Logo.Dispose();

                    Logo = null;
                }

                if (LogoBackground != null)
                {
                    if (!LogoBackground.IsDisposed)
                        LogoBackground.Dispose();

                    LogoBackground = null;
                }

                ConnectingClient = null;
                _ConnectionAttempt = 0;
                ConnectionTime = DateTime.MinValue;

            }
        }
        #endregion

        public sealed class LoginDialog : DXImageControl
        {
            #region Properties

            #region EMailValid

            public bool EMailValid
            {
                get => _EMailValid;
                set
                {
                    if (_EMailValid == value) return;

                    bool oldValue = _EMailValid;
                    _EMailValid = value;

                    OnEMailValidChanged(oldValue, value);
                }
            }
            private bool _EMailValid;
            public event EventHandler<EventArgs> EMailValidChanged;
            public void OnEMailValidChanged(bool oValue, bool nValue)
            {
                EMailValidChanged?.Invoke(this, EventArgs.Empty);

                LoginButton.Enabled = CanLogin;
            }

            #endregion

            #region PasswordValid

            public bool PasswordValid
            {
                get => _PasswordValid;
                set
                {
                    if (_PasswordValid == value) return;

                    bool oldValue = _PasswordValid;
                    _PasswordValid = value;

                    OnPasswordValidChanged(oldValue, value);
                }
            }
            private bool _PasswordValid;
            public event EventHandler<EventArgs> PasswordValidChanged;
            public void OnPasswordValidChanged(bool oValue, bool nValue)
            {
                PasswordValidChanged?.Invoke(this, EventArgs.Empty);

                LoginButton.Enabled = CanLogin;
            }

            #endregion

            #region LoginAttempted

            public bool LoginAttempted
            {
                get => _LoginAttempted;
                set
                {
                    if (_LoginAttempted == value) return;

                    bool oldValue = _LoginAttempted;
                    _LoginAttempted = value;

                    OnLoginAttemptedChanged(oldValue, value);
                }
            }
            private bool _LoginAttempted;
            public event EventHandler<EventArgs> LoginAttemptedChanged;
            public void OnLoginAttemptedChanged(bool oValue, bool nValue)
            {
                LoginAttemptedChanged?.Invoke(this, EventArgs.Empty);

                LoginButton.Enabled = CanLogin;
            }

            #endregion

            public bool CanLogin => EMailValid && PasswordValid && !LoginAttempted;

            public DXTextBox EMailTextBox, PasswordTextBox;
            public DXLabel EMailHelpLabel, PasswordHelpLabel;
            public DXCheckBox RememberCheckBox;

            public DXButton LoginButton, ExitButton, OptionButton, RankingButton, NewAccountButton, ChangePasswordButton;
            public DXLabel TitleLabel, ForgotPasswordLabel;

            public override void OnParentChanged(DXControl oValue, DXControl nValue)
            {
                base.OnParentChanged(oValue, nValue);

                if (Parent == null) return;

                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
            }
            public override void OnIsVisibleChanged(bool oValue, bool nValue)
            {
                base.OnIsVisibleChanged(oValue, nValue);

                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                if (!IsVisible)
                    scene.RankingBox.Visible = false;
            }

            public WindowType Type => WindowType.None;

            #endregion

            public LoginDialog()
            {
                LibraryFile = LibraryFile.Interface;
                Index = 151;
                Movable = false;
                Sort = false;

                TitleLabel = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.LoginDialogTitle,
                    ForeColour = Color.FromArgb(214, 190, 148)
                };
                TitleLabel.Location = new Point(280, 38);

                EMailTextBox = new DXTextBox
                {
                    Parent = this,
                    Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                    Location = new Point(70, 65),
                    Size = new Size(170, 14),
                    Border = false,
                    BackColour = Color.FromArgb(16, 8, 8),
                };
                EMailTextBox.SetFocus();
                EMailTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
                EMailTextBox.TextBox.GotFocus += (o, e) => EMailHelpLabel.Visible = true;
                EMailTextBox.TextBox.LostFocus += (o, e) => EMailHelpLabel.Visible = false;
                EMailTextBox.TextBox.KeyPress += TextBox_KeyPress;

                PasswordTextBox = new DXTextBox
                {
                    Parent = this,
                    Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                    Location = new Point(357, 65),
                    Size = new Size(170, 14),
                    Border = false,
                    BackColour = Color.FromArgb(16, 8, 8),
                    Password = true,
                };
                PasswordTextBox.TextBox.TextChanged += PasswordTextBox_TextChanged;
                PasswordTextBox.TextBox.GotFocus += (o, e) => PasswordHelpLabel.Visible = true;
                PasswordTextBox.TextBox.LostFocus += (o, e) => PasswordHelpLabel.Visible = false;
                PasswordTextBox.TextBox.KeyPress += TextBox_KeyPress;

                EMailHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.LoginDialogEMailHelpHint, Globals.MaxEMailLength),
                };
                EMailHelpLabel.Location = new Point(EMailTextBox.Location.X + EMailTextBox.Size.Width + 2, EMailTextBox.Location.Y - 2);

                PasswordHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.LoginDialogPasswordHelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                PasswordHelpLabel.Location = new Point(PasswordTextBox.Location.X + PasswordTextBox.Size.Width + 2, PasswordTextBox.Location.Y - 2);

                LoginButton = new DXButton
                {
                    Parent = this,
                    Location = new Point(550, 60),
                    Size = new Size(100, DefaultHeight),
                    Label = { Text = CEnvir.Language.LoginDialogLoginButtonLabel },
                    Enabled = false,
                };
                LoginButton.MouseClick += (o, e) => Login();

                ExitButton = new DXButton
                {
                    Parent = this,
                    Location = new Point(660, 60),
                    Size = new Size(100, DefaultHeight),
                    Label = { Text = CEnvir.Language.LoginDialogExitButtonLabel },
                    Enabled = true,
                };
                ExitButton.MouseClick += (o, e) => CEnvir.Target.Close();

                RankingButton = new DXButton
                {
                    Parent = this,
                    LibraryFile = LibraryFile.Interface,
                    Index = 153,
                    Location = new Point(20, 0),
                    Size = new Size(68, 32),
                    Label = { Text = CEnvir.Language.LoginDialogRankingButtonLabel, ForeColour = Color.FromArgb(255, 227, 165) },
                };
                RankingButton.MouseClick += RankingButton_MouseClick;

                OptionButton = new DXButton
                {
                    Parent = this,
                    LibraryFile = LibraryFile.Interface,
                    Index = 153,
                    Location = new Point(RankingButton.Location.X + RankingButton.Size.Width + 5, 0),
                    Size = new Size(68, 32),
                    Label = { Text = CEnvir.Language.LoginDialogOptionButtonLabel, ForeColour = Color.FromArgb(255, 227, 165) },
                };
                OptionButton.MouseClick += OptionButton_MouseClick;
                
                NewAccountButton = new DXButton
                {
                    Parent = this,
                    LibraryFile = LibraryFile.Interface,
                    Index = 152,
                    Location = new Point(485, 0),
                    Size = new Size(136, 32),
                    Label = { Text = CEnvir.Language.LoginDialogNewAccountButtonLabel, ForeColour = Color.FromArgb(255, 227, 165) },
                };
                NewAccountButton.MouseClick += NewAccountButton_MouseClick;

                ChangePasswordButton = new DXButton
                {
                    Parent = this,
                    LibraryFile = LibraryFile.Interface,
                    Index = 152,
                    Location = new Point(625, 0),
                    Size = new Size(136, 32),
                    Label = { Text = CEnvir.Language.LoginDialogChangePasswordButtonLabel, ForeColour = Color.FromArgb(255, 227, 165) }
                };
                ChangePasswordButton.MouseClick += ChangePasswordButton_MouseClick;

                RememberCheckBox = new DXCheckBox
                {
                    Label = { Text = CEnvir.Language.LoginDialogRememberCheckBoxLabel },
                    Parent = this,
                    Checked = Config.RememberDetails,
                };
                RememberCheckBox.Location = new Point(NewAccountButton.Location.X + 5, 38);
                RememberCheckBox.CheckedChanged += (o, e) => Config.RememberDetails = RememberCheckBox.Checked;

                ForgotPasswordLabel = new DXLabel()
                {
                    Parent = this,
                    Text = CEnvir.Language.LoginDialogForgotPasswordLabel,
                    Sound = SoundIndex.ButtonC,
                };
                ForgotPasswordLabel.MouseEnter += (o, e) => ForgotPasswordLabel.ForeColour = Color.White;
                ForgotPasswordLabel.MouseLeave += (o, e) => ForgotPasswordLabel.ForeColour = Color.FromArgb(198, 166, 99);
                ForgotPasswordLabel.Location = new Point(ChangePasswordButton.Location.X + 15, 38);
                ForgotPasswordLabel.MouseClick += ForgotPasswordLabel_MouseClick;

                if (Config.RememberDetails)
                {
                    EMailTextBox.TextBox.Text = Config.RememberedEMail;
                    PasswordTextBox.TextBox.Text = Config.RememberedPassword;
                }
            }

            private void RankingButton_MouseClick(object sender, MouseEventArgs e)
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                scene.RankingBox.Visible = !scene.RankingBox.Visible;
            }
            private void OptionButton_MouseClick(object sender, MouseEventArgs e)
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                scene.ConfigBox.Visible = !scene.ConfigBox.Visible;
            }

            #region Methods

            public void Login()
            {
                LoginAttempted = true;

                C.Login packet = new C.Login
                {
                    EMailAddress = EMailTextBox.TextBox.Text,
                    Password = PasswordTextBox.TextBox.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (!EMailValid)
                {
                    EMailTextBox.SetFocus();
                    return;
                }
                if (!PasswordValid)
                {
                    PasswordTextBox.SetFocus();
                    return;
                }

                if (LoginButton.IsEnabled)
                    Login();
            }

            private void EMailTextBox_TextChanged(object sender, EventArgs e)
            {
                EMailValid = !string.IsNullOrEmpty(EMailTextBox.TextBox.Text) && EMailTextBox.TextBox.Text.Length >= 3;

                if (string.IsNullOrEmpty(EMailTextBox.TextBox.Text))
                    EMailTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    EMailTextBox.BorderColour = EMailValid ? Color.Green : Color.Red;
            }
            private void PasswordTextBox_TextChanged(object sender, EventArgs e)
            {
                PasswordValid = !string.IsNullOrEmpty(PasswordTextBox.TextBox.Text) && Globals.PasswordRegex.IsMatch(PasswordTextBox.TextBox.Text);

                if (string.IsNullOrEmpty(PasswordTextBox.TextBox.Text))
                    PasswordTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    PasswordTextBox.BorderColour = PasswordValid ? Color.Green : Color.Red;
            }

            private void ChangePasswordButton_MouseClick(object sender, MouseEventArgs e)
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.ChangeBox.Visible = true;
                scene.ChangeBox.EMailTextBox.SetFocus();
            }
            private void ForgotPasswordLabel_MouseClick(object sender, MouseEventArgs e)
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.RequestPasswordBox.Visible = true;
                scene.RequestPasswordBox.EMailTextBox.SetFocus();
            }
            private void NewAccountButton_MouseClick(object sender, MouseEventArgs e)
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.AccountBox.Visible = true;
                scene.AccountBox.EMailTextBox.SetFocus();
            }


            #endregion

            #region IDisposable

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (EMailTextBox != null)
                    {
                        if (!EMailTextBox.IsDisposed)
                            EMailTextBox.Dispose();
                        EMailTextBox = null;
                    }

                    if (PasswordTextBox != null)
                    {
                        if (!PasswordTextBox.IsDisposed)
                            PasswordTextBox.Dispose();
                        PasswordTextBox = null;
                    }

                    if (EMailHelpLabel != null)
                    {
                        if (!EMailHelpLabel.IsDisposed)
                            EMailHelpLabel.Dispose();

                        EMailHelpLabel = null;
                    }

                    if (PasswordHelpLabel != null)
                    {
                        if (!PasswordHelpLabel.IsDisposed)
                            PasswordHelpLabel.Dispose();

                        PasswordHelpLabel = null;
                    }

                    if (RememberCheckBox != null)
                    {
                        if (!RememberCheckBox.IsDisposed)
                            RememberCheckBox.Dispose();
                        RememberCheckBox = null;
                    }

                    if (LoginButton != null)
                    {
                        if (!LoginButton.IsDisposed)
                            LoginButton.Dispose();
                        LoginButton = null;
                    }

                    if (ExitButton != null)
                    {
                        if (!ExitButton.IsDisposed)
                            ExitButton.Dispose();
                        ExitButton = null;
                    }

                    if (RankingButton != null)
                    {
                        if (!RankingButton.IsDisposed)
                            RankingButton.Dispose();
                        RankingButton = null;
                    }

                    if (OptionButton != null)
                    {
                        if (!OptionButton.IsDisposed)
                            OptionButton.Dispose();
                        OptionButton = null;
                    }

                    if (NewAccountButton != null)
                    {
                        if (!NewAccountButton.IsDisposed)
                            NewAccountButton.Dispose();
                        NewAccountButton = null;
                    }

                    if (ChangePasswordButton != null)
                    {
                        if (!ChangePasswordButton.IsDisposed)
                            ChangePasswordButton.Dispose();
                        ChangePasswordButton = null;
                    }

                    if (ForgotPasswordLabel != null)
                    {
                        if (!ForgotPasswordLabel.IsDisposed)
                            ForgotPasswordLabel.Dispose();
                        ForgotPasswordLabel = null;
                    }

                    _EMailValid = false;
                    _PasswordValid = false;
                    _LoginAttempted = false;
                }
            }

            #endregion
        }

        public sealed class NewAccountDialog : DXWindow
        {
            #region Properties

            #region EMailValid

            public bool EMailValid
            {
                get => _EMailValid;
                set
                {
                    if (_EMailValid == value) return;

                    bool oldValue = _EMailValid;
                    _EMailValid = value;

                    OnEMailValidChanged(oldValue, value);
                }
            }
            private bool _EMailValid;
            public event EventHandler<EventArgs> EMailValidChanged;
            public void OnEMailValidChanged(bool oValue, bool nValue)
            {
                EMailValidChanged?.Invoke(this, EventArgs.Empty);

                CreateButton.Enabled = CanCreate;
            }

            #endregion

            #region Password1Valid

            public bool Password1Valid
            {
                get => _Password1Valid;
                set
                {
                    if (_Password1Valid == value) return;

                    bool oldValue = _Password1Valid;
                    _Password1Valid = value;

                    OnPassword1ValidChanged(oldValue, value);
                }
            }
            private bool _Password1Valid;
            public event EventHandler<EventArgs> Password1ValidChanged;
            public void OnPassword1ValidChanged(bool oValue, bool nValue)
            {
                Password1ValidChanged?.Invoke(this, EventArgs.Empty);

                CreateButton.Enabled = CanCreate;
            }

            #endregion

            #region Password2Valid

            public bool Password2Valid
            {
                get => _Password2Valid;
                set
                {
                    if (_Password2Valid == value) return;

                    bool oldValue = _Password2Valid;
                    _Password2Valid = value;

                    OnPassword2ValidChanged(oldValue, value);
                }
            }
            private bool _Password2Valid;
            public event EventHandler<EventArgs> Password2ValidChanged;
            public void OnPassword2ValidChanged(bool oValue, bool nValue)
            {
                Password2ValidChanged?.Invoke(this, EventArgs.Empty);

                CreateButton.Enabled = CanCreate;
            }

            #endregion

            #region RealNameValid

            public bool RealNameValid
            {
                get => _RealNameValid;
                set
                {
                    if (_RealNameValid == value) return;

                    bool oldValue = _RealNameValid;
                    _RealNameValid = value;

                    OnRealNameValidChanged(oldValue, value);
                }
            }
            private bool _RealNameValid;
            public event EventHandler<EventArgs> RealNameValidChanged;
            public void OnRealNameValidChanged(bool oValue, bool nValue)
            {
                RealNameValidChanged?.Invoke(this, EventArgs.Empty);
                CreateButton.Enabled = CanCreate;
            }

            #endregion

            #region BirthDateValid

            public bool BirthDateValid
            {
                get => _BirthDateValid;
                set
                {
                    if (_BirthDateValid == value) return;

                    bool oldValue = _BirthDateValid;
                    _BirthDateValid = value;

                    OnBirthDateValidChanged(oldValue, value);
                }
            }
            private bool _BirthDateValid;
            public event EventHandler<EventArgs> BirthDateValidChanged;
            public void OnBirthDateValidChanged(bool oValue, bool nValue)
            {
                BirthDateValidChanged?.Invoke(this, EventArgs.Empty);
                CreateButton.Enabled = CanCreate;
            }

            #endregion

            #region ReferralValid

            public bool ReferralValid
            {
                get => _ReferralValid;
                set
                {
                    if (_ReferralValid == value) return;

                    bool oldValue = _ReferralValid;
                    _ReferralValid = value;

                    OnReferralValidChanged(oldValue, value);
                }
            }
            private bool _ReferralValid;
            public event EventHandler<EventArgs> ReferralValidChanged;
            public void OnReferralValidChanged(bool oValue, bool nValue)
            {
                ReferralValidChanged?.Invoke(this, EventArgs.Empty);
                CreateButton.Enabled = CanCreate;
            }

            #endregion

            #region CreateAttempted

            public bool CreateAttempted
            {
                get => _CreateAttempted;
                set
                {
                    if (_CreateAttempted == value) return;

                    bool oldValue = _CreateAttempted;
                    _CreateAttempted = value;

                    OnCreateAttemptedChanged(oldValue, value);
                }
            }
            private bool _CreateAttempted;
            public event EventHandler<EventArgs> CreateAttemptedChanged;
            public void OnCreateAttemptedChanged(bool oValue, bool nValue)
            {
                CreateAttemptedChanged?.Invoke(this, EventArgs.Empty);
                CreateButton.Enabled = CanCreate;
            }

            #endregion

            public bool CanCreate => EMailValid && Password1Valid && Password2Valid && RealNameValid && BirthDateValid && ReferralValid && !CreateAttempted;

            public DXButton CreateButton, CancelButton;
            public DXTextBox EMailTextBox, Password1TextBox, Password2TextBox, RealNameTextBox, BirthDateTextBox, ReferralTextBox;
            public DXLabel EMailHelpLabel, Password1HelpLabel, Password2HelpLabel, RealNameHelpLabel, BirthDateHelpLabel, ReferralHelpLabel;

            public override void OnParentChanged(DXControl oValue, DXControl nValue)
            {
                base.OnParentChanged(oValue, nValue);

                if (Parent == null) return;

                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
            }

            public override WindowType Type => WindowType.None;
            public override bool CustomSize => false;
            public override bool AutomaticVisibility => false;
            #endregion

            public NewAccountDialog()
            {
                Size = new Size(300, 255);
                TitleLabel.Text = CEnvir.Language.NewAccountDialogTitle;
                HasFooter = true;
                Visible = false;

                CancelButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.CommonControlCancel },
                    Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CancelButton.MouseClick += (o, e) => Close();
                CloseButton.MouseClick += (o, e) => Close();

                CreateButton = new DXButton
                {
                    Enabled = false,
                    Parent = this,
                    Label = { Text = CEnvir.Language.NewAccountDialogCreateButtonLabel },
                    Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CreateButton.MouseClick += (o, e) => Create();


                EMailTextBox = new DXTextBox
                {
                    Location = new Point(85, 45),
                    MaxLength = Globals.MaxEMailLength,
                    Parent = this,
                    Size = new Size(180, 20),
                };
                EMailTextBox.SetFocus();
                EMailTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
                EMailTextBox.TextBox.GotFocus += (o, e) => EMailHelpLabel.Visible = true;
                EMailTextBox.TextBox.LostFocus += (o, e) => EMailHelpLabel.Visible = false;
                EMailTextBox.TextBox.KeyPress += TextBox_KeyPress;

                Password1TextBox = new DXTextBox
                {
                    Location = new Point(85, 70),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 20),
                };
                Password1TextBox.TextBox.TextChanged += Password1TextBox_TextChanged;
                Password1TextBox.TextBox.GotFocus += (o, e) => Password1HelpLabel.Visible = true;
                Password1TextBox.TextBox.LostFocus += (o, e) => Password1HelpLabel.Visible = false;
                Password1TextBox.TextBox.KeyPress += TextBox_KeyPress;

                Password2TextBox = new DXTextBox
                {
                    Location = new Point(85, 95),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 20),
                };
                Password2TextBox.TextBox.TextChanged += Password2TextBox_TextChanged;
                Password2TextBox.TextBox.GotFocus += (o, e) => Password2HelpLabel.Visible = true;
                Password2TextBox.TextBox.LostFocus += (o, e) => Password2HelpLabel.Visible = false;
                Password2TextBox.TextBox.KeyPress += TextBox_KeyPress;

                RealNameTextBox = new DXTextBox
                {
                    Location = new Point(85, 120),
                    MaxLength = Globals.MaxRealNameLength,
                    Parent = this,
                    Size = new Size(136, 20),
                };
                RealNameTextBox.TextBox.TextChanged += RealNameTextBox_TextChanged;
                RealNameTextBox.TextBox.GotFocus += (o, e) => RealNameHelpLabel.Visible = true;
                RealNameTextBox.TextBox.LostFocus += (o, e) => RealNameHelpLabel.Visible = false;
                RealNameTextBox.TextBox.KeyPress += TextBox_KeyPress;


                BirthDateTextBox = new DXTextBox
                {
                    Location = new Point(85, 145),
                    MaxLength = 10,
                    Parent = this,
                    Size = new Size(136, 20),
                };
                BirthDateTextBox.TextBox.TextChanged += BirthDateTextBox_TextChanged;
                BirthDateTextBox.TextBox.GotFocus += (o, e) => BirthDateHelpLabel.Visible = true;
                BirthDateTextBox.TextBox.LostFocus += (o, e) => BirthDateHelpLabel.Visible = false;
                BirthDateTextBox.TextBox.KeyPress += TextBox_KeyPress;


                ReferralTextBox = new DXTextBox
                {
                    Location = new Point(85, 170),
                    MaxLength = Globals.MaxEMailLength,
                    Parent = this,
                    Size = new Size(180, 20),
                };
                ReferralTextBox.TextBox.TextChanged += ReferralTextBox_TextChanged;
                ReferralTextBox.TextBox.GotFocus += (o, e) => ReferralHelpLabel.Visible = true;
                ReferralTextBox.TextBox.LostFocus += (o, e) => ReferralHelpLabel.Visible = false;
                ReferralTextBox.TextBox.KeyPress += TextBox_KeyPress;

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.NewAccountDialogEMailLabel,
                };
                label.Location = new Point(EMailTextBox.Location.X - label.Size.Width - 5, (EMailTextBox.Size.Height - label.Size.Height) / 2 + EMailTextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.NewAccountDialogPassword1Label,
                };
                label.Location = new Point(Password1TextBox.Location.X - label.Size.Width - 5, (Password1TextBox.Size.Height - label.Size.Height) / 2 + Password1TextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.NewAccountDialogPassword2Label,
                };
                label.Location = new Point(Password2TextBox.Location.X - label.Size.Width - 5, (Password2TextBox.Size.Height - label.Size.Height) / 2 + Password2TextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.NewAccountDialogRealNameLabel,
                };
                label.Location = new Point(RealNameTextBox.Location.X - label.Size.Width - 5, (RealNameTextBox.Size.Height - label.Size.Height) / 2 + RealNameTextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.NewAccountDialogBirthDateLabel,
                };
                label.Location = new Point(BirthDateTextBox.Location.X - label.Size.Width - 5, (BirthDateTextBox.Size.Height - label.Size.Height) / 2 + BirthDateTextBox.Location.Y);


                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.NewAccountDialogReferralLabel,
                };
                label.Location = new Point(ReferralTextBox.Location.X - label.Size.Width - 5, (ReferralTextBox.Size.Height - label.Size.Height) / 2 + ReferralTextBox.Location.Y);


                EMailHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.NewAccountDialogEMailHelpHint, Globals.MaxEMailLength),
                };
                EMailHelpLabel.Location = new Point(EMailTextBox.Location.X + EMailTextBox.Size.Width + 2, (EMailTextBox.Size.Height - EMailHelpLabel.Size.Height) / 2 + EMailTextBox.Location.Y);

                Password1HelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.NewAccountDialogPassword1HelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                Password1HelpLabel.Location = new Point(Password1TextBox.Location.X + Password1TextBox.Size.Width + 2, (Password1TextBox.Size.Height - Password1HelpLabel.Size.Height) / 2 + Password1TextBox.Location.Y);

                Password2HelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.NewAccountDialogPassword2HelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                Password2HelpLabel.Location = new Point(Password2TextBox.Location.X + Password2TextBox.Size.Width + 2, (Password2TextBox.Size.Height - Password2HelpLabel.Size.Height) / 2 + Password2TextBox.Location.Y);


                RealNameHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.NewAccountDialogRealNameHelpHint, Globals.MinRealNameLength, Globals.MaxRealNameLength, Globals.RealNameRequired),
                };
                RealNameHelpLabel.Location = new Point(RealNameTextBox.Location.X + RealNameTextBox.Size.Width + 2, (RealNameTextBox.Size.Height - RealNameHelpLabel.Size.Height) / 2 + RealNameTextBox.Location.Y);

                BirthDateHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.NewAccountDialogBirthDateHelpHint, Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper(), Globals.BirthDateRequired),
                };
                BirthDateHelpLabel.Location = new Point(BirthDateTextBox.Location.X + BirthDateTextBox.Size.Width + 2, (BirthDateTextBox.Size.Height - BirthDateHelpLabel.Size.Height) / 2 + BirthDateTextBox.Location.Y);

                ReferralHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.NewAccountDialogReferralHelpHint, Globals.MaxEMailLength),
                };
                ReferralHelpLabel.Location = new Point(ReferralTextBox.Location.X + ReferralTextBox.Size.Width + 2, (ReferralTextBox.Size.Height - ReferralHelpLabel.Size.Height) / 2 + ReferralTextBox.Location.Y);


                RealNameValid = !Globals.RealNameRequired;
                BirthDateValid = !Globals.BirthDateRequired;
                ReferralValid = true;
            }

            #region Methods

            private void Create()
            {
                CreateAttempted = true;

                DateTime birthDate;
                DateTime.TryParse(BirthDateTextBox.TextBox.Text, out birthDate);

                C.NewAccount packet = new C.NewAccount
                {
                    EMailAddress = EMailTextBox.TextBox.Text,
                    Password = Password1TextBox.TextBox.Text,
                    RealName = RealNameTextBox.TextBox.Text,
                    BirthDate = birthDate,
                    Referral = ReferralTextBox.TextBox.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            public void Clear()
            {
                EMailTextBox.TextBox.Text = string.Empty;
                Password1TextBox.TextBox.Text = string.Empty;
                Password2TextBox.TextBox.Text = string.Empty;
                RealNameTextBox.TextBox.Text = string.Empty;
                BirthDateTextBox.TextBox.Text = string.Empty;

                Close();
            }
            private void Close()
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.LoginBox.Visible = true;
                scene.LoginBox.EMailTextBox.SetFocus();
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (!EMailValid)
                {
                    EMailTextBox.SetFocus();
                    return;
                }
                if (!Password1Valid)
                {
                    Password1TextBox.SetFocus();
                    return;
                }

                if (!Password2Valid)
                {
                    Password2TextBox.SetFocus();
                    return;
                }
                if (!RealNameValid)
                {
                    RealNameTextBox.SetFocus();
                    return;
                }
                if (!BirthDateValid)
                {
                    BirthDateTextBox.SetFocus();
                    return;
                }

                if (CreateButton.IsEnabled)
                    Create();
            }
            private void EMailTextBox_TextChanged(object sender, EventArgs e)
            {
                EMailValid = Globals.EMailRegex.IsMatch(EMailTextBox.TextBox.Text) && EMailTextBox.TextBox.Text.Length <= Globals.MaxEMailLength;

                if (string.IsNullOrEmpty(EMailTextBox.TextBox.Text))
                    EMailTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    EMailTextBox.BorderColour = EMailValid ? Color.Green : Color.Red;

            }
            private void Password1TextBox_TextChanged(object sender, EventArgs e)
            {
                Password1Valid = !string.IsNullOrEmpty(Password1TextBox.TextBox.Text) && Globals.PasswordRegex.IsMatch(Password1TextBox.TextBox.Text);
                Password2Valid = Password1Valid && Password1TextBox.TextBox.Text == Password2TextBox.TextBox.Text;

                if (string.IsNullOrEmpty(Password1TextBox.TextBox.Text))
                    Password1TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    Password1TextBox.BorderColour = Password1Valid ? Color.Green : Color.Red;

                if (string.IsNullOrEmpty(Password2TextBox.TextBox.Text))
                    Password2TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    Password2TextBox.BorderColour = Password2Valid ? Color.Green : Color.Red;

            }
            private void Password2TextBox_TextChanged(object sender, EventArgs e)
            {
                Password2Valid = Password1Valid && Password1TextBox.TextBox.Text == Password2TextBox.TextBox.Text;

                if (string.IsNullOrEmpty(Password2TextBox.TextBox.Text))
                    Password2TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    Password2TextBox.BorderColour = Password2Valid ? Color.Green : Color.Red;

            }
            private void RealNameTextBox_TextChanged(object sender, EventArgs e)
            {
                RealNameValid = (!Globals.RealNameRequired && string.IsNullOrEmpty(RealNameTextBox.TextBox.Text)) ||
                                (RealNameTextBox.TextBox.Text.Length >= Globals.MinRealNameLength && RealNameTextBox.TextBox.Text.Length <= Globals.MaxRealNameLength);

                if (string.IsNullOrEmpty(RealNameTextBox.TextBox.Text))
                    RealNameTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    RealNameTextBox.BorderColour = RealNameValid ? Color.Green : Color.Red;

            }
            private void BirthDateTextBox_TextChanged(object sender, EventArgs e)
            {
                DateTime temp;
                BirthDateValid = (!Globals.BirthDateRequired && string.IsNullOrEmpty(BirthDateTextBox.TextBox.Text)) || DateTime.TryParse(BirthDateTextBox.TextBox.Text, out temp);

                if (!Globals.BirthDateRequired && string.IsNullOrEmpty(BirthDateTextBox.TextBox.Text))
                    BirthDateTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    BirthDateTextBox.BorderColour = BirthDateValid ? Color.Green : Color.Red;

            }
            private void ReferralTextBox_TextChanged(object sender, EventArgs e)
            {
                ReferralValid = string.IsNullOrEmpty(ReferralTextBox.TextBox.Text) || (Globals.EMailRegex.IsMatch(ReferralTextBox.TextBox.Text) && ReferralTextBox.TextBox.Text.Length <= Globals.MaxEMailLength);

                if (string.IsNullOrEmpty(ReferralTextBox.TextBox.Text))
                    ReferralTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    ReferralTextBox.BorderColour = ReferralValid ? Color.Green : Color.Red;

            }
            #endregion

            #region IDisposable
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (CreateButton != null)
                    {
                        if (!CreateButton.IsDisposed)
                            CreateButton.Dispose();

                        CreateButton = null;
                    }

                    if (CancelButton != null)
                    {
                        if (!CancelButton.IsDisposed)
                            CancelButton.Dispose();

                        CancelButton = null;
                    }

                    if (EMailTextBox != null)
                    {
                        if (!EMailTextBox.IsDisposed)
                            EMailTextBox.Dispose();

                        EMailTextBox = null;
                    }

                    if (Password1TextBox != null)
                    {
                        if (!Password1TextBox.IsDisposed)
                            Password1TextBox.Dispose();

                        Password1TextBox = null;
                    }

                    if (Password2TextBox != null)
                    {
                        if (!Password2TextBox.IsDisposed)
                            Password2TextBox.Dispose();

                        Password2TextBox = null;
                    }

                    if (RealNameTextBox != null)
                    {
                        if (!RealNameTextBox.IsDisposed)
                            RealNameTextBox.Dispose();

                        RealNameTextBox = null;
                    }

                    if (BirthDateTextBox != null)
                    {
                        if (!BirthDateTextBox.IsDisposed)
                            BirthDateTextBox.Dispose();

                        BirthDateTextBox = null;
                    }

                    if (ReferralTextBox != null)
                    {
                        if (!ReferralTextBox.IsDisposed)
                            ReferralTextBox.Dispose();

                        ReferralTextBox = null;
                    }

                    if (EMailHelpLabel != null)
                    {
                        if (!EMailHelpLabel.IsDisposed)
                            EMailHelpLabel.Dispose();

                        EMailHelpLabel = null;
                    }

                    if (Password1HelpLabel != null)
                    {
                        if (!Password1HelpLabel.IsDisposed)
                            Password1HelpLabel.Dispose();

                        Password1HelpLabel = null;
                    }

                    if (Password2HelpLabel != null)
                    {
                        if (!Password2HelpLabel.IsDisposed)
                            Password2HelpLabel.Dispose();

                        Password2HelpLabel = null;
                    }

                    if (RealNameHelpLabel != null)
                    {
                        if (!RealNameHelpLabel.IsDisposed)
                            RealNameHelpLabel.Dispose();

                        RealNameHelpLabel = null;
                    }

                    if (BirthDateHelpLabel != null)
                    {
                        if (!BirthDateHelpLabel.IsDisposed)
                            BirthDateHelpLabel.Dispose();

                        BirthDateHelpLabel = null;
                    }

                    if (ReferralHelpLabel != null)
                    {
                        if (!ReferralHelpLabel.IsDisposed)
                            ReferralHelpLabel.Dispose();

                        ReferralHelpLabel = null;
                    }

                    _EMailValid = false;
                    _Password1Valid = false;
                    _Password2Valid = false;
                    _RealNameValid = false;
                    _BirthDateValid = false;
                    _ReferralValid = false;
                    _CreateAttempted = false;
                }
            }
            #endregion
        }

        public sealed class ChangePasswordDialog : DXWindow
        {
            #region Properties

            #region EMailValid

            public bool EMailValid
            {
                get => _EMailValid;
                set
                {
                    if (_EMailValid == value) return;

                    bool oldValue = _EMailValid;
                    _EMailValid = value;

                    OnEMailValidChanged(oldValue, value);
                }
            }
            private bool _EMailValid;
            public event EventHandler<EventArgs> EMailValidChanged;
            public void OnEMailValidChanged(bool oValue, bool nValue)
            {
                EMailValidChanged?.Invoke(this, EventArgs.Empty);
                ChangeButton.Enabled = CanChange;
            }

            #endregion

            #region CurrentPasswordValid

            public bool CurrentPasswordValid
            {
                get => _CurrentPasswordValid;
                set
                {
                    if (_CurrentPasswordValid == value) return;

                    bool oldValue = _CurrentPasswordValid;
                    _CurrentPasswordValid = value;

                    OnCurrentPasswordValidChanged(oldValue, value);
                }
            }
            private bool _CurrentPasswordValid;
            public event EventHandler<EventArgs> CurrentPasswordValidChanged;
            public void OnCurrentPasswordValidChanged(bool oValue, bool nValue)
            {
                CurrentPasswordValidChanged?.Invoke(this, EventArgs.Empty);
                ChangeButton.Enabled = CanChange;
            }

            #endregion

            #region NewPassword1Valid

            public bool NewPassword1Valid
            {
                get => _NewPassword1Valid;
                set
                {
                    if (_NewPassword1Valid == value) return;

                    bool oldValue = _NewPassword1Valid;
                    _NewPassword1Valid = value;

                    OnNewPassword1ValidChanged(oldValue, value);
                }
            }
            private bool _NewPassword1Valid;
            public event EventHandler<EventArgs> NewPassword1ValidChanged;
            public void OnNewPassword1ValidChanged(bool oValue, bool nValue)
            {
                NewPassword1ValidChanged?.Invoke(this, EventArgs.Empty);
                ChangeButton.Enabled = CanChange;
            }

            #endregion

            #region NewPassword2Valid

            public bool NewPassword2Valid
            {
                get => _NewPassword2Valid;
                set
                {
                    if (_NewPassword2Valid == value) return;

                    bool oldValue = _NewPassword2Valid;
                    _NewPassword2Valid = value;

                    OnNewPassword2ValidChanged(oldValue, value);
                }
            }
            private bool _NewPassword2Valid;
            public event EventHandler<EventArgs> NewPassword2ValidChanged;
            public void OnNewPassword2ValidChanged(bool oValue, bool nValue)
            {
                NewPassword2ValidChanged?.Invoke(this, EventArgs.Empty);
                ChangeButton.Enabled = CanChange;
            }

            #endregion

            #region ChangeAttempted

            public bool ChangeAttempted
            {
                get => _ChangeAttempted;
                set
                {
                    if (_ChangeAttempted == value) return;

                    bool oldValue = _ChangeAttempted;
                    _ChangeAttempted = value;

                    OnChangeAttemptedChanged(oldValue, value);
                }
            }
            private bool _ChangeAttempted;
            public event EventHandler<EventArgs> ChangeAttemptedChanged;
            public void OnChangeAttemptedChanged(bool oValue, bool nValue)
            {
                ChangeAttemptedChanged?.Invoke(this, EventArgs.Empty);
                ChangeButton.Enabled = CanChange;
            }

            #endregion

            public bool CanChange => EMailValid && CurrentPasswordValid && NewPassword1Valid && NewPassword2Valid && !ChangeAttempted;

            public DXButton ChangeButton, CancelButton;

            public DXTextBox EMailTextBox, CurrentPasswordTextBox, NewPassword1TextBox, NewPassword2TextBox;
            public DXLabel EMailHelpLabel, CurrentPasswordHelpLabel, NewPassword1HelpLabel, NewPassword2HelpLabel;

            public override void OnParentChanged(DXControl oValue, DXControl nValue)
            {
                base.OnParentChanged(oValue, nValue);

                if (Parent == null) return;

                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
            }

            public override WindowType Type => WindowType.None;
            public override bool CustomSize => false;
            public override bool AutomaticVisibility => false;

            #endregion

            public ChangePasswordDialog()
            {
                Size = new Size(330, 205);
                TitleLabel.Text = CEnvir.Language.ChangePasswordTitle;
                HasFooter = true;
                Visible = false;

                CancelButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.CommonControlCancel },
                    Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CancelButton.MouseClick += (o, e) => Close();
                CloseButton.MouseClick += (o, e) => Close();

                ChangeButton = new DXButton
                {
                    Enabled = false,
                    Parent = this,
                    Label = { Text = CEnvir.Language.ChangePasswordChangeButtonLabel },
                    Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                ChangeButton.MouseClick += (o, e) => Change();


                EMailTextBox = new DXTextBox
                {
                    Location = new Point(105, 45),
                    MaxLength = Globals.MaxEMailLength,
                    Parent = this,
                    Size = new Size(190, 20),
                };
                EMailTextBox.SetFocus();
                EMailTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
                EMailTextBox.TextBox.GotFocus += (o, e) => EMailHelpLabel.Visible = true;
                EMailTextBox.TextBox.LostFocus += (o, e) => EMailHelpLabel.Visible = false;
                EMailTextBox.TextBox.KeyPress += TextBox_KeyPress;

                CurrentPasswordTextBox = new DXTextBox
                {
                    Location = new Point(105, 70),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 20),
                };
                CurrentPasswordTextBox.TextBox.TextChanged += CurrentPasswordTextBox_TextChanged;
                CurrentPasswordTextBox.TextBox.GotFocus += (o, e) => CurrentPasswordHelpLabel.Visible = true;
                CurrentPasswordTextBox.TextBox.LostFocus += (o, e) => CurrentPasswordHelpLabel.Visible = false;
                CurrentPasswordTextBox.TextBox.KeyPress += TextBox_KeyPress;

                NewPassword1TextBox = new DXTextBox
                {
                    Location = new Point(105, 95),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 20),
                };
                NewPassword1TextBox.TextBox.TextChanged += NewPassword1TextBox_TextChanged;
                NewPassword1TextBox.TextBox.GotFocus += (o, e) => NewPassword1HelpLabel.Visible = true;
                NewPassword1TextBox.TextBox.LostFocus += (o, e) => NewPassword1HelpLabel.Visible = false;
                NewPassword1TextBox.TextBox.KeyPress += TextBox_KeyPress;

                NewPassword2TextBox = new DXTextBox
                {
                    Location = new Point(105, 120),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 20),
                };
                NewPassword2TextBox.TextBox.TextChanged += NewPassword2TextBox_TextChanged;
                NewPassword2TextBox.TextBox.GotFocus += (o, e) => NewPassword2HelpLabel.Visible = true;
                NewPassword2TextBox.TextBox.LostFocus += (o, e) => NewPassword2HelpLabel.Visible = false;
                NewPassword2TextBox.TextBox.KeyPress += TextBox_KeyPress;

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ChangePasswordEMailLabel,
                };
                label.Location = new Point(EMailTextBox.Location.X - label.Size.Width - 5, (EMailTextBox.Size.Height - label.Size.Height) / 2 + EMailTextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ChangePasswordCurrentPasswordLabel,
                };
                label.Location = new Point(CurrentPasswordTextBox.Location.X - label.Size.Width - 5, (CurrentPasswordTextBox.Size.Height - label.Size.Height) / 2 + CurrentPasswordTextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ChangePasswordNewPassword1Label,
                };
                label.Location = new Point(NewPassword1TextBox.Location.X - label.Size.Width - 5, (NewPassword1TextBox.Size.Height - label.Size.Height) / 2 + NewPassword1TextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ChangePasswordNewPassword2Label,
                };
                label.Location = new Point(NewPassword2TextBox.Location.X - label.Size.Width - 5, (NewPassword2TextBox.Size.Height - label.Size.Height) / 2 + NewPassword2TextBox.Location.Y);


                EMailHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.ChangePasswordEMailHelpHint, Globals.MaxEMailLength),
                };
                EMailHelpLabel.Location = new Point(EMailTextBox.Location.X + EMailTextBox.Size.Width + 2, (EMailTextBox.Size.Height - EMailHelpLabel.Size.Height) / 2 + EMailTextBox.Location.Y);

                CurrentPasswordHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.ChangePasswordCurrentPasswordHelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                CurrentPasswordHelpLabel.Location = new Point(CurrentPasswordTextBox.Location.X + CurrentPasswordTextBox.Size.Width + 2, (CurrentPasswordTextBox.Size.Height - CurrentPasswordHelpLabel.Size.Height) / 2 + CurrentPasswordTextBox.Location.Y);

                NewPassword1HelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.ChangePasswordNewPassword1HelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                NewPassword1HelpLabel.Location = new Point(NewPassword1TextBox.Location.X + NewPassword1TextBox.Size.Width + 2, (NewPassword1TextBox.Size.Height - NewPassword1HelpLabel.Size.Height) / 2 + NewPassword1TextBox.Location.Y);

                NewPassword2HelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.ChangePasswordNewPassword2HelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                NewPassword2HelpLabel.Location = new Point(NewPassword2TextBox.Location.X + NewPassword2TextBox.Size.Width + 2, (NewPassword2TextBox.Size.Height - NewPassword2HelpLabel.Size.Height) / 2 + NewPassword2TextBox.Location.Y);

            }

            #region Methods
            public void Change()
            {
                ChangeAttempted = true;

                C.ChangePassword packet = new C.ChangePassword
                {
                    EMailAddress = EMailTextBox.TextBox.Text,
                    CurrentPassword = CurrentPasswordTextBox.TextBox.Text,
                    NewPassword = NewPassword1TextBox.TextBox.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            public void Clear()
            {
                EMailTextBox.TextBox.Text = string.Empty;
                CurrentPasswordTextBox.TextBox.Text = string.Empty;
                NewPassword1TextBox.TextBox.Text = string.Empty;
                NewPassword2TextBox.TextBox.Text = string.Empty;

                Close();
            }
            private void Close()
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.LoginBox.Visible = true;
                scene.LoginBox.EMailTextBox.SetFocus();
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (!EMailValid)
                {
                    EMailTextBox.SetFocus();
                    return;
                }
                if (!CurrentPasswordValid)
                {
                    CurrentPasswordTextBox.SetFocus();
                    return;
                }

                if (!NewPassword1Valid)
                {
                    NewPassword1TextBox.SetFocus();
                    return;
                }
                if (!NewPassword2Valid)
                {
                    NewPassword2TextBox.SetFocus();
                    return;
                }

                if (ChangeButton.IsEnabled)
                    Change();
            }
            private void EMailTextBox_TextChanged(object sender, EventArgs e)
            {
                EMailValid = Globals.EMailRegex.IsMatch(EMailTextBox.TextBox.Text) && EMailTextBox.TextBox.Text.Length <= Globals.MaxEMailLength;

                if (string.IsNullOrEmpty(EMailTextBox.TextBox.Text))
                    EMailTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    EMailTextBox.BorderColour = EMailValid ? Color.Green : Color.Red;

            }
            private void CurrentPasswordTextBox_TextChanged(object sender, EventArgs e)
            {
                CurrentPasswordValid = Globals.PasswordRegex.IsMatch(CurrentPasswordTextBox.TextBox.Text);

                if (string.IsNullOrEmpty(CurrentPasswordTextBox.TextBox.Text))
                    CurrentPasswordTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    CurrentPasswordTextBox.BorderColour = CurrentPasswordValid ? Color.Green : Color.Red;
            }
            private void NewPassword1TextBox_TextChanged(object sender, EventArgs e)
            {
                NewPassword1Valid = !string.IsNullOrEmpty(NewPassword1TextBox.TextBox.Text) && Globals.PasswordRegex.IsMatch(NewPassword1TextBox.TextBox.Text);
                NewPassword2Valid = NewPassword1Valid && NewPassword1TextBox.TextBox.Text == NewPassword2TextBox.TextBox.Text;

                if (string.IsNullOrEmpty(NewPassword1TextBox.TextBox.Text))
                    NewPassword1TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    NewPassword1TextBox.BorderColour = NewPassword1Valid ? Color.Green : Color.Red;

                if (string.IsNullOrEmpty(NewPassword2TextBox.TextBox.Text))
                    NewPassword2TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    NewPassword2TextBox.BorderColour = NewPassword2Valid ? Color.Green : Color.Red;

            }
            private void NewPassword2TextBox_TextChanged(object sender, EventArgs e)
            {
                NewPassword2Valid = NewPassword1Valid && NewPassword1TextBox.TextBox.Text == NewPassword2TextBox.TextBox.Text;

                if (string.IsNullOrEmpty(NewPassword2TextBox.TextBox.Text))
                    NewPassword2TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    NewPassword2TextBox.BorderColour = NewPassword2Valid ? Color.Green : Color.Red;

            }
            #endregion

            #region IDisposable
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (ChangeButton != null)
                    {
                        if (!ChangeButton.IsDisposed)
                            ChangeButton.Dispose();

                        ChangeButton = null;
                    }

                    if (CancelButton != null)
                    {
                        if (!CancelButton.IsDisposed)
                            CancelButton.Dispose();

                        CancelButton = null;
                    }

                    if (EMailTextBox != null)
                    {
                        if (!EMailTextBox.IsDisposed)
                            EMailTextBox.Dispose();

                        EMailTextBox = null;
                    }

                    if (CurrentPasswordTextBox != null)
                    {
                        if (!CurrentPasswordTextBox.IsDisposed)
                            CurrentPasswordTextBox.Dispose();

                        CurrentPasswordTextBox = null;
                    }

                    if (NewPassword1TextBox != null)
                    {
                        if (!NewPassword1TextBox.IsDisposed)
                            NewPassword1TextBox.Dispose();

                        NewPassword1TextBox = null;
                    }

                    if (NewPassword2TextBox != null)
                    {
                        if (!NewPassword2TextBox.IsDisposed)
                            NewPassword2TextBox.Dispose();

                        NewPassword2TextBox = null;
                    }

                    if (EMailHelpLabel != null)
                    {
                        if (!EMailHelpLabel.IsDisposed)
                            EMailHelpLabel.Dispose();

                        EMailHelpLabel = null;
                    }

                    if (CurrentPasswordHelpLabel != null)
                    {
                        if (!CurrentPasswordHelpLabel.IsDisposed)
                            CurrentPasswordHelpLabel.Dispose();

                        CurrentPasswordHelpLabel = null;
                    }

                    if (NewPassword1HelpLabel != null)
                    {
                        if (!NewPassword1HelpLabel.IsDisposed)
                            NewPassword1HelpLabel.Dispose();

                        NewPassword1HelpLabel = null;
                    }

                    if (NewPassword2HelpLabel != null)
                    {
                        if (!NewPassword2HelpLabel.IsDisposed)
                            NewPassword2HelpLabel.Dispose();

                        NewPassword2HelpLabel = null;
                    }

                    _EMailValid = false;
                    _CurrentPasswordValid = false;
                    _NewPassword1Valid = false;
                    _NewPassword2Valid = false;
                    _ChangeAttempted = false;
                }
            }
            #endregion
        }

        public sealed class RequestResetPasswordDialog : DXWindow
        {
            #region Properties

            #region EMailValid

            public bool EMailValid
            {
                get => _EMailValid;
                set
                {
                    if (_EMailValid == value) return;

                    bool oldValue = _EMailValid;
                    _EMailValid = value;

                    OnEMailValidChanged(oldValue, value);
                }
            }
            private bool _EMailValid;
            public event EventHandler<EventArgs> EMailValidChanged;
            public void OnEMailValidChanged(bool oValue, bool nValue)
            {
                EMailValidChanged?.Invoke(this, EventArgs.Empty);
                RequestButton.Enabled = CanReset;
            }

            #endregion

            #region RequestAttempted

            public bool RequestAttempted
            {
                get => _RequestAttempted;
                set
                {
                    if (_RequestAttempted == value) return;

                    bool oldValue = _RequestAttempted;
                    _RequestAttempted = value;

                    OnRequestAttemptedChanged(oldValue, value);
                }
            }
            private bool _RequestAttempted;
            public event EventHandler<EventArgs> RequestAttemptedChanged;
            public void OnRequestAttemptedChanged(bool oValue, bool nValue)
            {
                RequestAttemptedChanged?.Invoke(this, EventArgs.Empty);
                RequestButton.Enabled = CanReset;
            }

            #endregion

            public bool CanReset => EMailValid && !RequestAttempted;

            public DXButton RequestButton, CancelButton;

            public DXTextBox EMailTextBox;
            public DXLabel EMailHelpLabel, HaveKeyLabel;

            public override void OnParentChanged(DXControl oValue, DXControl nValue)
            {
                base.OnParentChanged(oValue, nValue);

                if (Parent == null) return;

                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
            }

            public override WindowType Type => WindowType.None;
            public override bool CustomSize => false;
            public override bool AutomaticVisibility => false;

            #endregion

            public RequestResetPasswordDialog()
            {
                Size = new Size(330, 150);
                TitleLabel.Text = CEnvir.Language.RequestResetPasswordTitle;
                HasFooter = true;
                Visible = false;

                CancelButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.CommonControlCancel },
                    Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CancelButton.MouseClick += (o, e) => Close();
                CloseButton.MouseClick += (o, e) => Close();

                RequestButton = new DXButton
                {
                    Enabled = false,
                    Parent = this,
                    Label = { Text = CEnvir.Language.RequestResetPasswordRequestButtonLabel },
                    Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                RequestButton.MouseClick += (o, e) => Request();


                EMailTextBox = new DXTextBox
                {
                    Location = new Point(105, 45),
                    MaxLength = Globals.MaxEMailLength,
                    Parent = this,
                    Size = new Size(190, 20),
                };
                EMailTextBox.SetFocus();
                EMailTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
                EMailTextBox.TextBox.GotFocus += (o, e) => EMailHelpLabel.Visible = true;
                EMailTextBox.TextBox.LostFocus += (o, e) => EMailHelpLabel.Visible = false;
                EMailTextBox.TextBox.KeyPress += TextBox_KeyPress;

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.RequestResetPasswordEMailLabel,
                };
                label.Location = new Point(EMailTextBox.Location.X - label.Size.Width - 5, (EMailTextBox.Size.Height - label.Size.Height) / 2 + EMailTextBox.Location.Y);

                EMailHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.RequestResetPasswordEMailHelpHint, Globals.MaxEMailLength),
                };
                EMailHelpLabel.Location = new Point(EMailTextBox.Location.X + EMailTextBox.Size.Width + 2, (EMailTextBox.Size.Height - EMailHelpLabel.Size.Height) / 2 + EMailTextBox.Location.Y);

                HaveKeyLabel = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.RequestResetPasswordHaveKeyLabel,
                };
                HaveKeyLabel.MouseEnter += (o, e) => HaveKeyLabel.ForeColour = Color.White;
                HaveKeyLabel.MouseLeave += (o, e) => HaveKeyLabel.ForeColour = Color.FromArgb(198, 166, 99);
                HaveKeyLabel.MouseClick += HaveKeyLabel_MouseClick;
                HaveKeyLabel.Location = new Point(EMailTextBox.Location.X + (EMailTextBox.Size.Width - HaveKeyLabel.Size.Width) / 2, 70);
            }

            #region Methods

            public void Request()
            {
                RequestAttempted = true;

                C.RequestPasswordReset packet = new C.RequestPasswordReset
                {
                    EMailAddress = EMailTextBox.TextBox.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            public void Clear()
            {
                EMailTextBox.TextBox.Text = string.Empty;

                Close();
            }
            private void Close()
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.LoginBox.Visible = true;
                scene.LoginBox.EMailTextBox.SetFocus();
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (!EMailValid)
                {
                    EMailTextBox.SetFocus();
                    return;
                }

                if (RequestButton.IsEnabled)
                    Request();
            }
            private void EMailTextBox_TextChanged(object sender, EventArgs e)
            {
                EMailValid = Globals.EMailRegex.IsMatch(EMailTextBox.TextBox.Text) && EMailTextBox.TextBox.Text.Length <= Globals.MaxEMailLength;

                if (string.IsNullOrEmpty(EMailTextBox.TextBox.Text))
                    EMailTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    EMailTextBox.BorderColour = EMailValid ? Color.Green : Color.Red;

            }
            private void HaveKeyLabel_MouseClick(object sender, MouseEventArgs e)
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.ResetBox.Visible = true;
                scene.ResetBox.ResetKeyTextBox.SetFocus();
            }
            #endregion

            #region IDisposable
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (RequestButton != null)
                    {
                        if (!RequestButton.IsDisposed)
                            RequestButton.Dispose();

                        RequestButton = null;
                    }

                    if (CancelButton != null)
                    {
                        if (!CancelButton.IsDisposed)
                            CancelButton.Dispose();

                        CancelButton = null;
                    }

                    if (EMailTextBox != null)
                    {
                        if (!EMailTextBox.IsDisposed)
                            EMailTextBox.Dispose();

                        EMailTextBox = null;
                    }

                    if (EMailHelpLabel != null)
                    {
                        if (!EMailHelpLabel.IsDisposed)
                            EMailHelpLabel.Dispose();

                        EMailHelpLabel = null;
                    }

                    if (HaveKeyLabel != null)
                    {
                        if (!HaveKeyLabel.IsDisposed)
                            HaveKeyLabel.Dispose();

                        HaveKeyLabel = null;
                    }

                    _EMailValid = false;
                    _RequestAttempted = false;
                }
            }
            #endregion
        }

        public sealed class ResetPasswordDialog : DXWindow
        {
            #region Properties

            #region ResetKeyValid

            public bool ResetKeyValid
            {
                get => _ResetKeyValid;
                set
                {
                    if (_ResetKeyValid == value) return;

                    bool oldValue = _ResetKeyValid;
                    _ResetKeyValid = value;

                    OnResetKeyValidChanged(oldValue, value);
                }
            }
            private bool _ResetKeyValid;
            public event EventHandler<EventArgs> ResetKeyValidChanged;
            public void OnResetKeyValidChanged(bool oValue, bool nValue)
            {
                ResetKeyValidChanged?.Invoke(this, EventArgs.Empty);
                ResetButton.Enabled = CanReset;
            }

            #endregion

            #region NewPassword1Valid

            public bool NewPassword1Valid
            {
                get => _NewPassword1Valid;
                set
                {
                    if (_NewPassword1Valid == value) return;

                    bool oldValue = _NewPassword1Valid;
                    _NewPassword1Valid = value;

                    OnNewPassword1ValidChanged(oldValue, value);
                }
            }
            private bool _NewPassword1Valid;
            public event EventHandler<EventArgs> NewPassword1ValidChanged;
            public void OnNewPassword1ValidChanged(bool oValue, bool nValue)
            {
                NewPassword1ValidChanged?.Invoke(this, EventArgs.Empty);
                ResetButton.Enabled = CanReset;
            }

            #endregion

            #region NewPassword2Valid

            public bool NewPassword2Valid
            {
                get => _NewPassword2Valid;
                set
                {
                    if (_NewPassword2Valid == value) return;

                    bool oldValue = _NewPassword2Valid;
                    _NewPassword2Valid = value;

                    OnNewPassword2ValidChanged(oldValue, value);
                }
            }
            private bool _NewPassword2Valid;
            public event EventHandler<EventArgs> NewPassword2ValidChanged;
            public void OnNewPassword2ValidChanged(bool oValue, bool nValue)
            {
                NewPassword2ValidChanged?.Invoke(this, EventArgs.Empty);
                ResetButton.Enabled = CanReset;
            }

            #endregion

            #region ResetAttempted

            public bool ResetAttempted
            {
                get => _ResetAttempted;
                set
                {
                    if (_ResetAttempted == value) return;

                    bool oldValue = _ResetAttempted;
                    _ResetAttempted = value;

                    OnResetAttemptedChanged(oldValue, value);
                }
            }
            private bool _ResetAttempted;
            public event EventHandler<EventArgs> ResetAttemptedChanged;
            public void OnResetAttemptedChanged(bool oValue, bool nValue)
            {
                ResetAttemptedChanged?.Invoke(this, EventArgs.Empty);
                ResetButton.Enabled = CanReset;
            }

            #endregion

            public bool CanReset => ResetKeyValid && NewPassword1Valid && NewPassword2Valid && !ResetAttempted;

            public DXButton ResetButton, CancelButton;

            public DXTextBox ResetKeyTextBox, NewPassword1TextBox, NewPassword2TextBox;
            public DXLabel ResetHelpLabel, NewPassword1HelpLabel, NewPassword2HelpLabel;

            public override void OnParentChanged(DXControl oValue, DXControl nValue)
            {
                base.OnParentChanged(oValue, nValue);

                if (Parent == null) return;

                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
            }

            public override WindowType Type => WindowType.None;
            public override bool CustomSize => false;
            public override bool AutomaticVisibility => false;

            #endregion

            public ResetPasswordDialog()
            {
                Size = new Size(330, 180);
                TitleLabel.Text = CEnvir.Language.ResetPasswordTitle;
                HasFooter = true;
                Visible = false;

                CancelButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.CommonControlCancel },
                    Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CancelButton.MouseClick += (o, e) => Close();
                CloseButton.MouseClick += (o, e) => Close();

                ResetButton = new DXButton
                {
                    Enabled = false,
                    Parent = this,
                    Label = { Text = CEnvir.Language.ResetPasswordResetButtonLabel },
                    Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                ResetButton.MouseClick += (o, e) => Reset();


                ResetKeyTextBox = new DXTextBox
                {
                    Location = new Point(105, 45),
                    MaxLength = Globals.MaxEMailLength,
                    Parent = this,
                    Size = new Size(190, 20),
                };
                ResetKeyTextBox.SetFocus();
                ResetKeyTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
                ResetKeyTextBox.TextBox.GotFocus += (o, e) => ResetHelpLabel.Visible = true;
                ResetKeyTextBox.TextBox.LostFocus += (o, e) => ResetHelpLabel.Visible = false;
                ResetKeyTextBox.TextBox.KeyPress += TextBox_KeyPress;

                NewPassword1TextBox = new DXTextBox
                {
                    Location = new Point(105, 70),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 20),
                };
                NewPassword1TextBox.TextBox.TextChanged += NewPassword1TextBox_TextChanged;
                NewPassword1TextBox.TextBox.GotFocus += (o, e) => NewPassword1HelpLabel.Visible = true;
                NewPassword1TextBox.TextBox.LostFocus += (o, e) => NewPassword1HelpLabel.Visible = false;
                NewPassword1TextBox.TextBox.KeyPress += TextBox_KeyPress;

                NewPassword2TextBox = new DXTextBox
                {
                    Location = new Point(105, 95),
                    MaxLength = Globals.MaxPasswordLength,
                    Parent = this,
                    Password = true,
                    Size = new Size(136, 20),
                };
                NewPassword2TextBox.TextBox.TextChanged += NewPassword2TextBox_TextChanged;
                NewPassword2TextBox.TextBox.GotFocus += (o, e) => NewPassword2HelpLabel.Visible = true;
                NewPassword2TextBox.TextBox.LostFocus += (o, e) => NewPassword2HelpLabel.Visible = false;
                NewPassword2TextBox.TextBox.KeyPress += TextBox_KeyPress;

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ResetPasswordResetKeyLabel,
                };
                label.Location = new Point(ResetKeyTextBox.Location.X - label.Size.Width - 5, (ResetKeyTextBox.Size.Height - label.Size.Height) / 2 + ResetKeyTextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ResetPasswordNewPassword1Label,
                };
                label.Location = new Point(NewPassword1TextBox.Location.X - label.Size.Width - 5, (NewPassword1TextBox.Size.Height - label.Size.Height) / 2 + NewPassword1TextBox.Location.Y);

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ResetPasswordNewPassword2Label,
                };
                label.Location = new Point(NewPassword2TextBox.Location.X - label.Size.Width - 5, (NewPassword2TextBox.Size.Height - label.Size.Height) / 2 + NewPassword2TextBox.Location.Y);


                ResetHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = CEnvir.Language.ResetPasswordResetKeyHelpHint,
                };
                ResetHelpLabel.Location = new Point(ResetKeyTextBox.Location.X + ResetKeyTextBox.Size.Width + 2, (ResetKeyTextBox.Size.Height - ResetHelpLabel.Size.Height) / 2 + ResetKeyTextBox.Location.Y);

                NewPassword1HelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.ResetPasswordNewPassword1HelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                NewPassword1HelpLabel.Location = new Point(NewPassword1TextBox.Location.X + NewPassword1TextBox.Size.Width + 2, (NewPassword1TextBox.Size.Height - NewPassword1HelpLabel.Size.Height) / 2 + NewPassword1TextBox.Location.Y);

                NewPassword2HelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.ResetPasswordNewPassword2HelpHint, Globals.MinPasswordLength, Globals.MaxPasswordLength),
                };
                NewPassword2HelpLabel.Location = new Point(NewPassword2TextBox.Location.X + NewPassword2TextBox.Size.Width + 2, (NewPassword2TextBox.Size.Height - NewPassword2HelpLabel.Size.Height) / 2 + NewPassword2TextBox.Location.Y);

            }

            #region Methods
            public void Reset()
            {
                ResetAttempted = true;

                C.ResetPassword packet = new C.ResetPassword
                {
                    ResetKey = ResetKeyTextBox.TextBox.Text,
                    NewPassword = NewPassword1TextBox.TextBox.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            public void Clear()
            {
                ResetKeyTextBox.TextBox.Text = string.Empty;
                NewPassword1TextBox.TextBox.Text = string.Empty;
                NewPassword2TextBox.TextBox.Text = string.Empty;

                Close();
            }
            private void Close()
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.RequestPasswordBox.Visible = true;
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (!ResetKeyValid)
                {
                    ResetKeyTextBox.SetFocus();
                    return;
                }

                if (!NewPassword1Valid)
                {
                    NewPassword1TextBox.SetFocus();
                    return;
                }
                if (!NewPassword2Valid)
                {
                    NewPassword2TextBox.SetFocus();
                    return;
                }

                if (ResetButton.IsEnabled)
                    Reset();
            }
            private void EMailTextBox_TextChanged(object sender, EventArgs e)
            {
                ResetKeyValid = !string.IsNullOrEmpty(ResetKeyTextBox.TextBox.Text);

                if (string.IsNullOrEmpty(ResetKeyTextBox.TextBox.Text))
                    ResetKeyTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    ResetKeyTextBox.BorderColour = ResetKeyValid ? Color.Green : Color.Red;

            }
            private void NewPassword1TextBox_TextChanged(object sender, EventArgs e)
            {
                NewPassword1Valid = !string.IsNullOrEmpty(NewPassword1TextBox.TextBox.Text) && Globals.PasswordRegex.IsMatch(NewPassword1TextBox.TextBox.Text);
                NewPassword2Valid = NewPassword1Valid && NewPassword1TextBox.TextBox.Text == NewPassword2TextBox.TextBox.Text;

                if (string.IsNullOrEmpty(NewPassword1TextBox.TextBox.Text))
                    NewPassword1TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    NewPassword1TextBox.BorderColour = NewPassword1Valid ? Color.Green : Color.Red;

            }
            private void NewPassword2TextBox_TextChanged(object sender, EventArgs e)
            {
                NewPassword2Valid = NewPassword1Valid && NewPassword1TextBox.TextBox.Text == NewPassword2TextBox.TextBox.Text;

                if (string.IsNullOrEmpty(NewPassword2TextBox.TextBox.Text))
                    NewPassword2TextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    NewPassword2TextBox.BorderColour = NewPassword2Valid ? Color.Green : Color.Red;

            }
            #endregion

            #region IDisposable
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (ResetButton != null)
                    {
                        if (!ResetButton.IsDisposed)
                            ResetButton.Dispose();

                        ResetButton = null;
                    }

                    if (CancelButton != null)
                    {
                        if (!CancelButton.IsDisposed)
                            CancelButton.Dispose();

                        CancelButton = null;
                    }

                    if (ResetKeyTextBox != null)
                    {
                        if (!ResetKeyTextBox.IsDisposed)
                            ResetKeyTextBox.Dispose();

                        ResetKeyTextBox = null;
                    }

                    if (NewPassword1TextBox != null)
                    {
                        if (!NewPassword1TextBox.IsDisposed)
                            NewPassword1TextBox.Dispose();

                        NewPassword1TextBox = null;
                    }

                    if (NewPassword2TextBox != null)
                    {
                        if (!NewPassword2TextBox.IsDisposed)
                            NewPassword2TextBox.Dispose();

                        NewPassword2TextBox = null;
                    }

                    if (ResetHelpLabel != null)
                    {
                        if (!ResetHelpLabel.IsDisposed)
                            ResetHelpLabel.Dispose();

                        ResetHelpLabel = null;
                    }

                    if (NewPassword1HelpLabel != null)
                    {
                        if (!NewPassword1HelpLabel.IsDisposed)
                            NewPassword1HelpLabel.Dispose();

                        NewPassword1HelpLabel = null;
                    }

                    if (NewPassword2HelpLabel != null)
                    {
                        if (!NewPassword2HelpLabel.IsDisposed)
                            NewPassword2HelpLabel.Dispose();

                        NewPassword2HelpLabel = null;
                    }

                    _ResetKeyValid = false;
                    _NewPassword1Valid = false;
                    _NewPassword2Valid = false;
                    _ResetAttempted = false;
                }
            }
            #endregion
        }

        public sealed class ActivationDialog : DXWindow
        {
            #region Properties
            #region ActivationKeyValid

            public bool ActivationKeyValid
            {
                get => _ActivationKeyValid;
                set
                {
                    if (_ActivationKeyValid == value) return;

                    bool oldValue = _ActivationKeyValid;
                    _ActivationKeyValid = value;

                    OnActivationKeyValidChanged(oldValue, value);
                }
            }
            private bool _ActivationKeyValid;
            public event EventHandler<EventArgs> ActivationKeyValidChanged;
            public void OnActivationKeyValidChanged(bool oValue, bool nValue)
            {
                ActivationKeyValidChanged?.Invoke(this, EventArgs.Empty);
                ActivateButton.Enabled = CanActivate;
            }

            #endregion

            #region ActivationAttempted

            public bool ActivationAttempted
            {
                get => _ActivationAttempted;
                set
                {
                    if (_ActivationAttempted == value) return;

                    bool oldValue = _ActivationAttempted;
                    _ActivationAttempted = value;

                    OnActivationAttemptedChanged(oldValue, value);
                }
            }
            private bool _ActivationAttempted;
            public event EventHandler<EventArgs> ActivationAttemptedChanged;
            public void OnActivationAttemptedChanged(bool oValue, bool nValue)
            {
                ActivationAttemptedChanged?.Invoke(this, EventArgs.Empty);
                ActivateButton.Enabled = CanActivate;
            }

            #endregion

            public bool CanActivate => ActivationKeyValid && !ActivationAttempted;

            public DXControl PreviousWindow;
            public DXButton ActivateButton, CancelButton;

            public DXTextBox ActivationKeyTextBox;
            public DXLabel ActivationHelpLabel, ResendLabel;

            public override void OnParentChanged(DXControl oValue, DXControl nValue)
            {
                base.OnParentChanged(oValue, nValue);

                if (Parent == null) return;

                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
            }

            public override WindowType Type => WindowType.None;
            public override bool CustomSize => false;
            public override bool AutomaticVisibility => false;

            #endregion

            public ActivationDialog()
            {
                Size = new Size(330, 155);
                TitleLabel.Text = CEnvir.Language.ActivationTitle;
                HasFooter = true;
                Visible = false;

                CancelButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.CommonControlCancel },
                    Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CancelButton.MouseClick += (o, e) => Close();
                CloseButton.MouseClick += (o, e) => Close();

                ActivateButton = new DXButton
                {
                    Enabled = false,
                    Parent = this,
                    Label = { Text = CEnvir.Language.ActivationActivateButtonLabel },
                    Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                ActivateButton.MouseClick += (o, e) => Activate();


                ActivationKeyTextBox = new DXTextBox
                {
                    Location = new Point(105, 45),
                    MaxLength = 30,
                    Parent = this,
                    Size = new Size(190, 20),
                };
                ActivationKeyTextBox.SetFocus();
                ActivationKeyTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
                ActivationKeyTextBox.TextBox.GotFocus += (o, e) => ActivationHelpLabel.Visible = true;
                ActivationKeyTextBox.TextBox.LostFocus += (o, e) => ActivationHelpLabel.Visible = false;
                ActivationKeyTextBox.TextBox.KeyPress += TextBox_KeyPress;

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ActivationActivationKeyLabel,
                };
                label.Location = new Point(ActivationKeyTextBox.Location.X - label.Size.Width - 5, (ActivationKeyTextBox.Size.Height - label.Size.Height) / 2 + ActivationKeyTextBox.Location.Y);

                ActivationHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = CEnvir.Language.ActivationActivationKeyHelpHint,
                };
                ActivationHelpLabel.Location = new Point(ActivationKeyTextBox.Location.X + ActivationKeyTextBox.Size.Width + 2, (ActivationKeyTextBox.Size.Height - ActivationHelpLabel.Size.Height) / 2 + ActivationKeyTextBox.Location.Y);

                ResendLabel = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.ActivationResendLabel,
                };
                ResendLabel.MouseEnter += (o, e) => ResendLabel.ForeColour = Color.White;
                ResendLabel.MouseLeave += (o, e) => ResendLabel.ForeColour = Color.FromArgb(198, 166, 99);
                ResendLabel.MouseClick += ResendLabel_MouseClick;
                ResendLabel.Location = new Point(ActivationKeyTextBox.Location.X + (ActivationKeyTextBox.Size.Width - ResendLabel.Size.Width) / 2, 70);
            }

            #region Methods
            public void Activate()
            {
                ActivationAttempted = true;

                C.Activation packet = new C.Activation
                {
                    ActivationKey = ActivationKeyTextBox.TextBox.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            public void Clear()
            {
                ActivationKeyTextBox.TextBox.Text = string.Empty;
                Close();
            }
            private void Close()
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                PreviousWindow.Visible = true;
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (!ActivationKeyValid)
                {
                    ActivationKeyTextBox.SetFocus();
                    return;
                }

                if (ActivateButton.IsEnabled)
                    Activate();
            }
            private void EMailTextBox_TextChanged(object sender, EventArgs e)
            {
                ActivationKeyValid = !string.IsNullOrEmpty(ActivationKeyTextBox.TextBox.Text);

                if (string.IsNullOrEmpty(ActivationKeyTextBox.TextBox.Text))
                    ActivationKeyTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    ActivationKeyTextBox.BorderColour = ActivationKeyValid ? Color.Green : Color.Red;

            }
            private void ResendLabel_MouseClick(object sender, MouseEventArgs e)
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.RequestActivationBox.Visible = true;
                scene.RequestActivationBox.EMailTextBox.SetFocus();
            }
            #endregion

            #region IDisposable
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    PreviousWindow = null;

                    if (ActivateButton != null)
                    {
                        if (!ActivateButton.IsDisposed)
                            ActivateButton.Dispose();

                        ActivateButton = null;
                    }

                    if (CancelButton != null)
                    {
                        if (!CancelButton.IsDisposed)
                            CancelButton.Dispose();

                        CancelButton = null;
                    }

                    if (ActivationKeyTextBox != null)
                    {
                        if (!ActivationKeyTextBox.IsDisposed)
                            ActivationKeyTextBox.Dispose();

                        ActivationKeyTextBox = null;
                    }

                    if (ActivationHelpLabel != null)
                    {
                        if (!ActivationHelpLabel.IsDisposed)
                            ActivationHelpLabel.Dispose();

                        ActivationHelpLabel = null;
                    }

                    if (ResendLabel != null)
                    {
                        if (!ResendLabel.IsDisposed)
                            ResendLabel.Dispose();

                        ResendLabel = null;
                    }

                    _ActivationKeyValid = false;
                    _ActivationAttempted = false;
                }
            }
            #endregion
        }

        public sealed class RequestActivationKeyDialog : DXWindow
        {
            #region Properties

            #region EMailValid

            public bool EMailValid
            {
                get => _EMailValid;
                set
                {
                    if (_EMailValid == value) return;

                    bool oldValue = _EMailValid;
                    _EMailValid = value;

                    OnEMailValidChanged(oldValue, value);
                }
            }
            private bool _EMailValid;
            public event EventHandler<EventArgs> EMailValidChanged;
            public void OnEMailValidChanged(bool oValue, bool nValue)
            {
                EMailValidChanged?.Invoke(this, EventArgs.Empty);

                RequestButton.Enabled = CanRequest;
            }

            #endregion

            #region RequestAttempted

            public bool RequestAttempted
            {
                get => _RequestAttempted;
                set
                {
                    if (_RequestAttempted == value) return;

                    bool oldValue = _RequestAttempted;
                    _RequestAttempted = value;

                    OnRequestAttemptedChanged(oldValue, value);
                }
            }
            private bool _RequestAttempted;
            public event EventHandler<EventArgs> RequestAttemptedChanged;
            public void OnRequestAttemptedChanged(bool oValue, bool nValue)
            {
                RequestAttemptedChanged?.Invoke(this, EventArgs.Empty);
                RequestButton.Enabled = CanRequest;
            }

            #endregion

            public bool CanRequest => EMailValid && !RequestAttempted;

            public DXButton RequestButton, CancelButton;

            public DXTextBox EMailTextBox;
            public DXLabel EMailHelpLabel;

            public override void OnParentChanged(DXControl oValue, DXControl nValue)
            {
                base.OnParentChanged(oValue, nValue);

                if (Parent == null) return;

                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
            }

            public override WindowType Type => WindowType.None;
            public override bool CustomSize => false;
            public override bool AutomaticVisibility => false;
            #endregion

            public RequestActivationKeyDialog()
            {
                Size = new Size(330, 130);
                TitleLabel.Text = CEnvir.Language.RequestActivationKeyTitle;
                HasFooter = true;
                Visible = false;

                CancelButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.CommonControlCancel },
                    Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CancelButton.MouseClick += (o, e) => Close();
                CloseButton.MouseClick += (o, e) => Close();

                RequestButton = new DXButton
                {
                    Enabled = false,
                    Parent = this,
                    Label = { Text = CEnvir.Language.RequestActivationKeyRequestButtonLabel },
                    Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                RequestButton.MouseClick += (o, e) => Request();


                EMailTextBox = new DXTextBox
                {
                    BorderColour = Color.Red,
                    Location = new Point(105, 45),
                    MaxLength = Globals.MaxEMailLength,
                    Parent = this,
                    Size = new Size(190, 20),
                };
                EMailTextBox.SetFocus();
                EMailTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
                EMailTextBox.TextBox.GotFocus += (o, e) => EMailHelpLabel.Visible = true;
                EMailTextBox.TextBox.LostFocus += (o, e) => EMailHelpLabel.Visible = false;
                EMailTextBox.TextBox.KeyPress += TextBox_KeyPress;

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.RequestActivationKeyEMailLabel,
                };
                label.Location = new Point(EMailTextBox.Location.X - label.Size.Width - 5, (EMailTextBox.Size.Height - label.Size.Height) / 2 + EMailTextBox.Location.Y);

                EMailHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.RequestActivationKeyEMailHelpHint, Globals.MaxEMailLength),
                };
                EMailHelpLabel.Location = new Point(EMailTextBox.Location.X + EMailTextBox.Size.Width + 2, (EMailTextBox.Size.Height - EMailHelpLabel.Size.Height) / 2 + EMailTextBox.Location.Y);
            }

            #region Methods
            public void Request()
            {
                RequestAttempted = true;

                C.RequestActivationKey packet = new C.RequestActivationKey
                {
                    EMailAddress = EMailTextBox.TextBox.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            public void Clear()
            {
                EMailTextBox.TextBox.Text = string.Empty;

                Close();
            }
            private void Close()
            {
                LoginScene scene = ActiveScene as LoginScene;

                if (scene == null) return;

                Visible = false;
                scene.ActivationBox.Visible = true;
            }

            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (!EMailValid)
                {
                    EMailTextBox.SetFocus();
                    return;
                }

                if (RequestButton.IsEnabled)
                    Request();
            }
            private void EMailTextBox_TextChanged(object sender, EventArgs e)
            {
                EMailValid = Globals.EMailRegex.IsMatch(EMailTextBox.TextBox.Text) && EMailTextBox.TextBox.Text.Length <= Globals.MaxEMailLength;

                if (string.IsNullOrEmpty(EMailTextBox.TextBox.Text))
                    EMailTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    EMailTextBox.BorderColour = EMailValid ? Color.Green : Color.Red;

            }
            #endregion

            #region IDisposable
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (RequestButton != null)
                    {
                        if (!RequestButton.IsDisposed)
                            RequestButton.Dispose();

                        RequestButton = null;
                    }

                    if (CancelButton != null)
                    {
                        if (!CancelButton.IsDisposed)
                            CancelButton.Dispose();

                        CancelButton = null;
                    }

                    if (EMailTextBox != null)
                    {
                        if (!EMailTextBox.IsDisposed)
                            EMailTextBox.Dispose();

                        EMailTextBox = null;
                    }

                    if (EMailHelpLabel != null)
                    {
                        if (!EMailHelpLabel.IsDisposed)
                            EMailHelpLabel.Dispose();

                        EMailHelpLabel = null;
                    }

                    _EMailValid = false;
                    _RequestAttempted = false;
                }
            }
            #endregion
        }
    }
}

