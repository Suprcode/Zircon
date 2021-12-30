using System.Drawing;
using System.Windows.Forms;
using Client.Envir;
using Client.Scenes;
using Client.Scenes.Views;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Controls
{
    public sealed class DXConfigWindow : DXWindow
    {
        #region Properties
        public static DXConfigWindow ActiveConfig;
        public DXKeyBindWindow KeyBindWindow;

        private DXTabControl TabControl;

        //Grpahics
        public DXTab GraphicsTab;
        public DXCheckBox FullScreenCheckBox, VSyncCheckBox, LimitFPSCheckBox, ClipMouseCheckBox, DebugLabelCheckBox;
        private DXComboBox GameSizeComboBox, LanguageComboBox;

        //Sound
        public DXTab SoundTab;
        private DXNumberBox SystemVolumeBox, MusicVolumeBox, SpellVolumeBox, PlayerVolumeBox, MonsterVolumeBox;
        private DXCheckBox BackgroundSoundBox;

        //Game 
        public DXTab GameTab;
        private DXCheckBox ItemNameCheckBox, MonsterNameCheckBox, PlayerNameCheckBox, UserHealthCheckBox, MonsterHealthCheckBox, DamageNumbersCheckBox, EscapeCloseAllCheckBox, ShiftOpenChatCheckBox, RightClickDeTargetCheckBox, MonsterBoxVisibleCheckBox, LogChatCheckBox, DrawEffectsCheckBox, DrawParticlesCheckBox;
        public DXButton KeyBindButton;

        //Network
        public DXTab NetworkTab;
        private DXCheckBox UseNetworkConfigCheckBox;
        private DXTextBox IPAddressTextBox;
        private DXNumberBox PortBox;

        //Colours
        public DXTab ColourTab;
        public DXColourControl LocalColourBox, GMWhisperInColourBox, WhisperInColourBox, WhisperOutColourBox, GroupColourBox, GuildColourBox, ShoutColourBox, GlobalColourBox, ObserverColourBox, HintColourBox, SystemColourBox, GainsColourBox, AnnouncementColourBox;
        public DXButton ResetColoursButton;


        private DXButton SaveButton, CancelButton;
        public DXButton ExitButton;

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!IsVisible) return;

            FullScreenCheckBox.Checked = Config.FullScreen;
            GameSizeComboBox.ListBox.SelectItem(Config.GameSize);
            VSyncCheckBox.Checked = Config.VSync;
            LimitFPSCheckBox.Checked = Config.LimitFPS;
            ClipMouseCheckBox.Checked = Config.ClipMouse;
            DebugLabelCheckBox.Checked = Config.DebugLabel;
            LanguageComboBox.ListBox.SelectItem(Config.Language);

            BackgroundSoundBox.Checked = Config.SoundInBackground;
            SystemVolumeBox.ValueTextBox.TextBox.Text = Config.SystemVolume.ToString();
            MusicVolumeBox.ValueTextBox.TextBox.Text = Config.MusicVolume.ToString();
            PlayerVolumeBox.ValueTextBox.TextBox.Text = Config.PlayerVolume.ToString();
            MonsterVolumeBox.ValueTextBox.TextBox.Text = Config.MonsterVolume.ToString();
            SpellVolumeBox.ValueTextBox.TextBox.Text = Config.MagicVolume.ToString();
            UseNetworkConfigCheckBox.Checked = Config.UseNetworkConfig;
            IPAddressTextBox.TextBox.Text = Config.IPAddress;
            PortBox.ValueTextBox.TextBox.Text = Config.Port.ToString();

            ItemNameCheckBox.Checked= Config.ShowItemNames;
            MonsterNameCheckBox.Checked = Config.ShowMonsterNames;
            PlayerNameCheckBox.Checked = Config.ShowPlayerNames;
            UserHealthCheckBox.Checked = Config.ShowUserHealth;
            MonsterHealthCheckBox.Checked = Config.ShowMonsterHealth;
            DamageNumbersCheckBox.Checked = Config.ShowDamageNumbers;
            EscapeCloseAllCheckBox.Checked = Config.EscapeCloseAll;
            ShiftOpenChatCheckBox.Checked = Config.ShiftOpenChat;
            RightClickDeTargetCheckBox.Checked = Config.RightClickDeTarget;
            MonsterBoxVisibleCheckBox.Checked = Config.MonsterBoxVisible;
            LogChatCheckBox.Checked = Config.LogChat;
            DrawEffectsCheckBox.Checked = Config.DrawEffects;
            DrawParticlesCheckBox.Checked = Config.DrawParticles;

            LocalColourBox.BackColour = Config.LocalTextColour;
            GMWhisperInColourBox.BackColour = Config.GMWhisperInTextColour;
            WhisperInColourBox.BackColour = Config.WhisperInTextColour;
            WhisperOutColourBox.BackColour = Config.WhisperOutTextColour;
            GroupColourBox.BackColour = Config.GroupTextColour;
            GuildColourBox.BackColour = Config.GuildTextColour;
            ShoutColourBox.BackColour = Config.ShoutTextColour;
            GlobalColourBox.BackColour = Config.GlobalTextColour;
            ObserverColourBox.BackColour = Config.ObserverTextColour;
            HintColourBox.BackColour = Config.HintTextColour;
            SystemColourBox.BackColour = Config.SystemTextColour;
            GainsColourBox.BackColour = Config.GainsTextColour;
            AnnouncementColourBox.BackColour = Config.AnnouncementTextColour;
        }
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            KeyBindWindow.Parent = nValue;
        }

        public override WindowType Type => WindowType.ConfigBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;
        #endregion

        public DXConfigWindow()
        {
            ActiveConfig = this;

            Size = new Size(300, 305);
            TitleLabel.Text = "Configuration";
            HasFooter = true;

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = ClientArea.Location,
                Size = ClientArea.Size,
            };
            GraphicsTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Graphics" } },
            };

            SoundTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Sound" } },
            };

            GameTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Game" } },
            };

            NetworkTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Network" } },
            };

            ColourTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = "Colours" }, Visible = false },
            };


            KeyBindWindow = new DXKeyBindWindow
            {
                Visible =  false
            };

            #region Graphics
            
            FullScreenCheckBox = new DXCheckBox
            {
                Label = { Text = "Full Screen:" },
                Parent = GraphicsTab,
                Checked = Config.FullScreen,
            };
            FullScreenCheckBox.Location = new Point(120 - FullScreenCheckBox.Size.Width, 10);

            DXLabel label = new DXLabel
            {
                Text = "Game Size:",
                Outline = true,
                Parent = GraphicsTab,
            };
            label.Location = new Point(104 - label.Size.Width, 35);

            GameSizeComboBox = new DXComboBox
            {
                Parent = GraphicsTab,
                Location = new Point(104, 35),
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
            };

            foreach (Size resolution in Globals.ValidResolutions)
                new DXListBoxItem
                {
                    Parent = GameSizeComboBox.ListBox,
                    Label = { Text = $"{resolution.Width} x {resolution.Height}" },
                    Item = resolution
                };

            VSyncCheckBox = new DXCheckBox
            {
                Label = { Text = "V-Sync:" },
                Parent = GraphicsTab,
            };
            VSyncCheckBox.Location = new Point(120 - VSyncCheckBox.Size.Width, 60);

            LimitFPSCheckBox = new DXCheckBox
            {
                Label = { Text = "Limit FPS:" },
                Parent = GraphicsTab,
            };
            LimitFPSCheckBox.Location = new Point(120 - LimitFPSCheckBox.Size.Width, 80);

            ClipMouseCheckBox = new DXCheckBox
            {
                Label = { Text = "Clip Mouse:" },
                Parent = GraphicsTab,
            };
            ClipMouseCheckBox.Location = new Point(120 - ClipMouseCheckBox.Size.Width, 100);

            DebugLabelCheckBox = new DXCheckBox
            {
                Label = { Text = "Debug Label:" },
                Parent = GraphicsTab,
            };
            DebugLabelCheckBox.Location = new Point(120 - DebugLabelCheckBox.Size.Width, 120);

            label = new DXLabel
            {
                Text = "Language:",
                Outline = true,
                Parent = GraphicsTab,
            };
            label.Location = new Point(104 - label.Size.Width, 140);

            LanguageComboBox = new DXComboBox
            {
                Parent = GraphicsTab,
                Location = new Point(104, 140),
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
            };

            foreach (string language in Globals.Languages)
                new DXListBoxItem
                {
                    Parent = LanguageComboBox.ListBox,
                    Label = { Text = language },
                    Item = language
                };
            #endregion

            #region Sound

            BackgroundSoundBox = new DXCheckBox
            {
                Label = { Text = "Background Sound:" },
                Parent = SoundTab,
                Checked = Config.SoundInBackground,
            };
            BackgroundSoundBox.Location = new Point(120 - BackgroundSoundBox.Size.Width, 10);

            label = new DXLabel
            {
                Text = "System Volume:",
                Outline = true,
                Parent = SoundTab,
            };
            label.Location = new Point(104 - label.Size.Width, 35);

            SystemVolumeBox = new DXNumberBox
            {
                Parent = SoundTab,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 35)
            };

            label = new DXLabel
            {
                Text = "Music Volume:",
                Outline = true,
                Parent = SoundTab,
            };
            label.Location = new Point(104 - label.Size.Width, 60);

            MusicVolumeBox = new DXNumberBox
            {
                Parent = SoundTab,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 60)
            };

            label = new DXLabel
            {
                Text = "Player Volume:",
                Outline = true,
                Parent = SoundTab,
            };
            label.Location = new Point(104 - label.Size.Width, 85);

            PlayerVolumeBox = new DXNumberBox
            {
                Parent = SoundTab,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 85)
            };
            label = new DXLabel
            {
                Text = "Monster Volume:",
                Outline = true,
                Parent = SoundTab,
            };
            label.Location = new Point(104 - label.Size.Width, 110);

            MonsterVolumeBox = new DXNumberBox
            {
                Parent = SoundTab,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 110)
            };

            label = new DXLabel
            {
                Text = "Magic Volume:",
                Outline = true,
                Parent = SoundTab,
            };
            label.Location = new Point(104 - label.Size.Width, 135);

            SpellVolumeBox = new DXNumberBox
            {
                Parent = SoundTab,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 135)
            };


            #endregion

            #region Game

            ItemNameCheckBox = new DXCheckBox
            {
                Label = { Text = "Item Names:" },
                Parent = GameTab,
            };
            ItemNameCheckBox.Location = new Point(120 - ItemNameCheckBox.Size.Width, 10);

            MonsterNameCheckBox = new DXCheckBox
            {
                Label = { Text = "Monster Names:" },
                Parent = GameTab,
            };
            MonsterNameCheckBox.Location = new Point(120 - MonsterNameCheckBox.Size.Width, 35);

            PlayerNameCheckBox = new DXCheckBox
            {
                Label = { Text = "Player Name:" },
                Parent = GameTab,
            };
            PlayerNameCheckBox.Location = new Point(120 - PlayerNameCheckBox.Size.Width, 60);

            UserHealthCheckBox = new DXCheckBox
            {
                Label = { Text = "User Health:" },
                Parent = GameTab,
            };
            UserHealthCheckBox.Location = new Point(120 - UserHealthCheckBox.Size.Width, 85);

            MonsterHealthCheckBox = new DXCheckBox
            {
                Label = { Text = "Monster Health:" },
                Parent = GameTab,
            };
            MonsterHealthCheckBox.Location = new Point(120 - MonsterHealthCheckBox.Size.Width, 110);

            DamageNumbersCheckBox = new DXCheckBox
            {
                Label = { Text = "Damage Numbers:" },
                Parent = GameTab,
            };
            DamageNumbersCheckBox.Location = new Point(120 - DamageNumbersCheckBox.Size.Width, 135);

            DrawParticlesCheckBox = new DXCheckBox
            {
                Label = { Text = "Draw Particles:" },
                Parent = GameTab,
            };
            DrawParticlesCheckBox.Location = new Point(120 - DrawParticlesCheckBox.Size.Width, 160);

            EscapeCloseAllCheckBox = new DXCheckBox
            {
                Label = { Text = "Escape Close All:" },
                Parent = GameTab,
            };
            EscapeCloseAllCheckBox.Location = new Point(270 - EscapeCloseAllCheckBox.Size.Width, 10);

            ShiftOpenChatCheckBox = new DXCheckBox
            {
                Label = { Text = "Shift + 1  Open Chat:" },
                Parent = GameTab,
                Hint = "If turned on, Pressing Shift + 1 will open chat, if this is off you will use Quick Slot 1"
            };
            ShiftOpenChatCheckBox.Location = new Point(270 - ShiftOpenChatCheckBox.Size.Width, 35);

            RightClickDeTargetCheckBox = new DXCheckBox
            {
                Label = { Text = "Right Click De-Target:" },
                Parent = GameTab,
                Hint = "If turned on, Right clicking to move away will also remove monster target."
            };
            RightClickDeTargetCheckBox.Location = new Point(270 - RightClickDeTargetCheckBox.Size.Width, 60);

            MonsterBoxVisibleCheckBox = new DXCheckBox
            {
                Label = { Text = "Show Monster Info:" },
                Parent = GameTab,
            };
            MonsterBoxVisibleCheckBox.Location = new Point(270 - MonsterBoxVisibleCheckBox.Size.Width, 85);

            LogChatCheckBox = new DXCheckBox
            {
                Label = { Text = "Log Chat:" },
                Parent = GameTab,
            };
            LogChatCheckBox.Location = new Point(270 - LogChatCheckBox.Size.Width, 110);

            DrawEffectsCheckBox = new DXCheckBox
            {
                Label = { Text = "Draw Effects:" },
                Parent = GameTab,
            };
            DrawEffectsCheckBox.Location = new Point(270 - DrawEffectsCheckBox.Size.Width, 135);

            KeyBindButton = new DXButton
            {
                Parent = GameTab,
                Location = new Point(190, 160),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Key Binds" }
            };
            KeyBindButton.MouseClick += (o, e) => KeyBindWindow.Visible = !KeyBindWindow.Visible;
            
            #endregion

            #region Network

            UseNetworkConfigCheckBox = new DXCheckBox
            {
                Label = { Text = "Use Config:" },
                Parent = NetworkTab,
                Checked = Config.FullScreen,
            };
            UseNetworkConfigCheckBox.Location = new Point(120 - UseNetworkConfigCheckBox.Size.Width, 10);

            label = new DXLabel
            {
                Text = "IP Address:",
                Outline = true,
                Parent = NetworkTab,
            };
            label.Location = new Point(104 - label.Size.Width, 35);

            IPAddressTextBox = new DXTextBox
            {
                Location = new Point(104, 35),
                Size = new Size(100, 16),
                Parent = NetworkTab,
            };

            label = new DXLabel
            {
                Text = "Port:",
                Outline = true,
                Parent = NetworkTab,
            };
            label.Location = new Point(104 - label.Size.Width, 60);

            PortBox = new DXNumberBox
            {
                Parent = NetworkTab,
                Change = 100,
                MaxValue = ushort.MaxValue,
                Location = new Point(104, 60)
            };
            #endregion

            #region Colours

            label = new DXLabel
            {
                Text = "Local Chat:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 10);

            LocalColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 10),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "GM Whisper In:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 10);

            GMWhisperInColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 10),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Whisper In:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 35);

            WhisperInColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 35),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Whisper Out:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 35);

            WhisperOutColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 35),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Group Chat:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 60);

            GroupColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 60),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Guild Chat:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 60);

            GuildColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 60),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Shout Chat:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 85);

            ShoutColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 85),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Global Chat:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 85);

            GlobalColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 85),
                Size = new Size(40, label.Size.Height),
            };
            

            label = new DXLabel
            {
                Text = "Observer Chat:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 110);

            ObserverColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 110),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Hint Text:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 110);

            HintColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 110),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "System Text:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 135);

            SystemColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 135),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Gains Text:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 135);

            GainsColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 135),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "Announcements:",
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 160);

            AnnouncementColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 160),
                Size = new Size(40, label.Size.Height),
            };

            ResetColoursButton = new DXButton
            {
                Parent = ColourTab,
                Location = new Point(180, 160),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "Reset All" }
            };
            ResetColoursButton.MouseClick += (o, e) =>
            {
                LocalColourBox.BackColour = Color.White;
                GMWhisperInColourBox.BackColour = Color.Red;
                WhisperInColourBox.BackColour = Color.Cyan;
                WhisperOutColourBox.BackColour = Color.Aquamarine;
                GroupColourBox.BackColour = Color.Plum;
                GuildColourBox.BackColour = Color.LightPink;
                ShoutColourBox.BackColour = Color.Yellow;
                GlobalColourBox.BackColour = Color.Lime;
                ObserverColourBox.BackColour = Color.Silver;
                HintColourBox.BackColour = Color.AntiqueWhite;
                SystemColourBox.BackColour = Color.Red;
                GainsColourBox.BackColour = Color.GreenYellow;
                AnnouncementColourBox.BackColour = Color.DarkBlue;
            };

            #endregion

            SaveButton = new DXButton
            {
                Location = new Point(Size.Width - 190, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Apply" }
            };
            SaveButton.MouseClick += SaveSettings;

            CancelButton = new DXButton
            {
                Location = new Point(Size.Width - 100, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "Cancel" }
            };
            CancelButton.MouseClick += CancelSettings;

            ExitButton = new DXButton
            {
                Location = new Point(Size.Width - 280, Size.Height - 43),
                Size = new Size(60, DefaultHeight),
                Parent = this,
                Label = { Text = "Exit" },
                Visible = false,
            };
            ExitButton.MouseClick += CancelSettings;
        }

        #region Methods
        private void CancelSettings(object o, MouseEventArgs e)
        {
            Visible = false;
        }
        private void SaveSettings(object o, MouseEventArgs e)
        {
            if (Config.FullScreen != FullScreenCheckBox.Checked)
            {
                DXManager.ToggleFullScreen();
            }

            if (GameSizeComboBox.SelectedItem is Size && Config.GameSize != (Size)GameSizeComboBox.SelectedItem)
            {
                Config.GameSize = (Size)GameSizeComboBox.SelectedItem;

                if (ActiveScene is GameScene)
                {
                    ActiveScene.Size = Config.GameSize;
                    DXManager.SetResolution(ActiveScene.Size);
                }
            }

            if (LanguageComboBox.SelectedItem is string && Config.Language != (string)LanguageComboBox.SelectedItem)
            {

                Config.Language = (string) LanguageComboBox.SelectedItem;

                if (CEnvir.Connection != null && CEnvir.Connection.ServerConnected)
                    CEnvir.Enqueue(new C.SelectLanguage { Language = Config.Language });
            }


            if (Config.VSync != VSyncCheckBox.Checked)
            {
                Config.VSync = VSyncCheckBox.Checked;
                DXManager.ResetDevice();
            }

            Config.LimitFPS = LimitFPSCheckBox.Checked;
            Config.ClipMouse = ClipMouseCheckBox.Checked;
            Config.DebugLabel = DebugLabelCheckBox.Checked;

            DebugLabel.IsVisible = Config.DebugLabel;
            PingLabel.IsVisible = Config.DebugLabel;

            if (Config.SoundInBackground != BackgroundSoundBox.Checked)
            {
                Config.SoundInBackground = BackgroundSoundBox.Checked;

                DXSoundManager.UpdateFlags();
            }
            

            bool volumeChanged = false;


            if (Config.SystemVolume != SystemVolumeBox.Value)
            {
                Config.SystemVolume = (int) SystemVolumeBox.Value;
                volumeChanged = true;
            }


            if (Config.MusicVolume != MusicVolumeBox.Value)
            {
                Config.MusicVolume = (int)MusicVolumeBox.Value;
                volumeChanged = true;
            }


            if (Config.PlayerVolume != PlayerVolumeBox.Value)
            {
                Config.PlayerVolume = (int)PlayerVolumeBox.Value;
                volumeChanged = true;
            }

            if (Config.MonsterVolume != MonsterVolumeBox.Value)
            {
                Config.MonsterVolume = (int)MonsterVolumeBox.Value;
                volumeChanged = true;
            }

            if (Config.MagicVolume != SpellVolumeBox.Value)
            {
                Config.MagicVolume = (int)SpellVolumeBox.Value;
                volumeChanged = true;
            }

            Config.ShowItemNames = ItemNameCheckBox.Checked;
            Config.ShowMonsterNames = MonsterNameCheckBox.Checked;
            Config.ShowPlayerNames = PlayerNameCheckBox.Checked;
            Config.ShowUserHealth = UserHealthCheckBox.Checked;
            Config.ShowMonsterHealth = MonsterHealthCheckBox.Checked;
            Config.ShowDamageNumbers = DamageNumbersCheckBox.Checked;

            Config.EscapeCloseAll = EscapeCloseAllCheckBox.Checked;
            Config.ShiftOpenChat = ShiftOpenChatCheckBox.Checked;
            Config.RightClickDeTarget = RightClickDeTargetCheckBox.Checked;
            Config.MonsterBoxVisible = MonsterBoxVisibleCheckBox.Checked;
            Config.LogChat = LogChatCheckBox.Checked;
            Config.DrawEffects = DrawEffectsCheckBox.Checked;
            Config.DrawParticles = DrawParticlesCheckBox.Checked;

            if (volumeChanged)
                DXSoundManager.AdjustVolume();

            Config.UseNetworkConfig = UseNetworkConfigCheckBox.Checked;
            Config.IPAddress = IPAddressTextBox.TextBox.Text;
            Config.Port = (int)PortBox.Value;


            bool coloursChanged = false;

            if (Config.LocalTextColour != LocalColourBox.BackColour)
            {
                Config.LocalTextColour = LocalColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GMWhisperInTextColour != GMWhisperInColourBox.BackColour)
            {
                Config.GMWhisperInTextColour = GMWhisperInColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.WhisperInTextColour != WhisperInColourBox.BackColour)
            {
                Config.WhisperInTextColour = WhisperInColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.WhisperOutTextColour != WhisperOutColourBox.BackColour)
            {
                Config.WhisperOutTextColour = WhisperOutColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.GroupTextColour != GroupColourBox.BackColour)
            {
                Config.GroupTextColour = GroupColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.GuildTextColour != GuildColourBox.BackColour)
            {
                Config.GuildTextColour = GuildColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ShoutTextColour != ShoutColourBox.BackColour)
            {
                Config.ShoutTextColour = ShoutColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GlobalTextColour != GlobalColourBox.BackColour)
            {
                Config.GlobalTextColour = GlobalColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ObserverTextColour != ObserverColourBox.BackColour)
            {
                Config.ObserverTextColour = ObserverColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.HintTextColour != HintColourBox.BackColour)
            {
                Config.HintTextColour = HintColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.SystemTextColour != SystemColourBox.BackColour)
            {
                Config.SystemTextColour = SystemColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GainsTextColour != GainsColourBox.BackColour)
            {
                Config.GainsTextColour = GainsColourBox.BackColour;
                coloursChanged = true;
            }
            if (Config.AnnouncementTextColour != AnnouncementColourBox.BackColour)
            {
                Config.AnnouncementTextColour = AnnouncementColourBox.BackColour;
                coloursChanged = true;
            }

            if (coloursChanged && GameScene.Game != null)
            {
                foreach (ChatTab tab in ChatTab.Tabs)
                    tab.UpdateColours();
            }


        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Visible = false;
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
                if (ActiveConfig == this)
                    ActiveConfig = null;

                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (KeyBindWindow != null)
                {
                    if (!KeyBindWindow.IsDisposed)
                        KeyBindWindow.Dispose();

                    KeyBindWindow = null;
                }

                #region Graphics
                if (GraphicsTab != null)
                {
                    if (!GraphicsTab.IsDisposed)
                        GraphicsTab.Dispose();

                    GraphicsTab = null;
                }

                if (FullScreenCheckBox != null)
                {
                    if (!FullScreenCheckBox.IsDisposed)
                        FullScreenCheckBox.Dispose();

                    FullScreenCheckBox = null;
                }

                if (VSyncCheckBox != null)
                {
                    if (!VSyncCheckBox.IsDisposed)
                        VSyncCheckBox.Dispose();

                    VSyncCheckBox = null;
                }

                if (LimitFPSCheckBox != null)
                {
                    if (!LimitFPSCheckBox.IsDisposed)
                        LimitFPSCheckBox.Dispose();

                    LimitFPSCheckBox = null;
                }

                if (ClipMouseCheckBox != null)
                {
                    if (!ClipMouseCheckBox.IsDisposed)
                        ClipMouseCheckBox.Dispose();

                    ClipMouseCheckBox = null;
                }
                if (DebugLabelCheckBox != null)
                {
                    if (!DebugLabelCheckBox.IsDisposed)
                        DebugLabelCheckBox.Dispose();

                    DebugLabelCheckBox = null;
                }

                if (GameSizeComboBox != null)
                {
                    if (!GameSizeComboBox.IsDisposed)
                        GameSizeComboBox.Dispose();

                    GameSizeComboBox = null;
                }
                if (LanguageComboBox != null)
                {
                    if (!LanguageComboBox.IsDisposed)
                        LanguageComboBox.Dispose();

                    LanguageComboBox = null;
                }
                
                #endregion

                #region Sound
                if (SoundTab != null)
                {
                    if (!SoundTab.IsDisposed)
                        SoundTab.Dispose();

                    SoundTab = null;
                }

                if (SystemVolumeBox != null)
                {
                    if (!SystemVolumeBox.IsDisposed)
                        SystemVolumeBox.Dispose();

                    SystemVolumeBox = null;
                }

                if (MusicVolumeBox != null)
                {
                    if (!MusicVolumeBox.IsDisposed)
                        MusicVolumeBox.Dispose();

                    MusicVolumeBox = null;
                }

                if (PlayerVolumeBox != null)
                {
                    if (!PlayerVolumeBox.IsDisposed)
                        PlayerVolumeBox.Dispose();

                    PlayerVolumeBox = null;
                }

                if (MonsterVolumeBox != null)
                {
                    if (!MonsterVolumeBox.IsDisposed)
                        MonsterVolumeBox.Dispose();

                    MonsterVolumeBox = null;
                }

                if (SpellVolumeBox != null)
                {
                    if (!SpellVolumeBox.IsDisposed)
                        SpellVolumeBox.Dispose();

                    SpellVolumeBox = null;
                }

                if (BackgroundSoundBox != null)
                {
                    if (!BackgroundSoundBox.IsDisposed)
                        BackgroundSoundBox.Dispose();

                    BackgroundSoundBox = null;
                }
                #endregion

                #region Game
                if (GameTab != null)
                {
                    if (!GameTab.IsDisposed)
                        GameTab.Dispose();

                    GameTab = null;
                }

                if (ItemNameCheckBox != null)
                {
                    if (!ItemNameCheckBox.IsDisposed)
                        ItemNameCheckBox.Dispose();

                    ItemNameCheckBox = null;
                }

                if (MonsterNameCheckBox != null)
                {
                    if (!MonsterNameCheckBox.IsDisposed)
                        MonsterNameCheckBox.Dispose();

                    MonsterNameCheckBox = null;
                }

                if (PlayerNameCheckBox != null)
                {
                    if (!PlayerNameCheckBox.IsDisposed)
                        PlayerNameCheckBox.Dispose();

                    PlayerNameCheckBox = null;
                }

                if (UserHealthCheckBox != null)
                {
                    if (!UserHealthCheckBox.IsDisposed)
                        UserHealthCheckBox.Dispose();

                    UserHealthCheckBox = null;
                }

                if (MonsterHealthCheckBox != null)
                {
                    if (!MonsterHealthCheckBox.IsDisposed)
                        MonsterHealthCheckBox.Dispose();

                    MonsterHealthCheckBox = null;
                }

                if (DamageNumbersCheckBox != null)
                {
                    if (!DamageNumbersCheckBox.IsDisposed)
                        DamageNumbersCheckBox.Dispose();

                    DamageNumbersCheckBox = null;
                }

                if (EscapeCloseAllCheckBox != null)
                {
                    if (!EscapeCloseAllCheckBox.IsDisposed)
                        EscapeCloseAllCheckBox.Dispose();

                    EscapeCloseAllCheckBox = null;
                }

                if (ShiftOpenChatCheckBox != null)
                {
                    if (!ShiftOpenChatCheckBox.IsDisposed)
                        ShiftOpenChatCheckBox.Dispose();

                    ShiftOpenChatCheckBox = null;
                }

                if (RightClickDeTargetCheckBox != null)
                {
                    if (!RightClickDeTargetCheckBox.IsDisposed)
                        RightClickDeTargetCheckBox.Dispose();

                    RightClickDeTargetCheckBox = null;
                }
                
                if (MonsterBoxVisibleCheckBox != null)
                {
                    if (!MonsterBoxVisibleCheckBox.IsDisposed)
                        MonsterBoxVisibleCheckBox.Dispose();

                    MonsterBoxVisibleCheckBox = null;
                }

                if (LogChatCheckBox != null)
                {
                    if (!LogChatCheckBox.IsDisposed)
                        LogChatCheckBox.Dispose();

                    LogChatCheckBox = null;
                }
                

                if (KeyBindButton != null)
                {
                    if (!KeyBindButton.IsDisposed)
                        KeyBindButton.Dispose();

                    KeyBindButton = null;
                }
                #endregion

                #region Network
                if (NetworkTab != null)
                {
                    if (!NetworkTab.IsDisposed)
                        NetworkTab.Dispose();

                    NetworkTab = null;
                }
                
                if (UseNetworkConfigCheckBox != null)
                {
                    if (!UseNetworkConfigCheckBox.IsDisposed)
                        UseNetworkConfigCheckBox.Dispose();

                    UseNetworkConfigCheckBox = null;
                }

                if (IPAddressTextBox != null)
                {
                    if (!IPAddressTextBox.IsDisposed)
                        IPAddressTextBox.Dispose();

                    IPAddressTextBox = null;
                }

                if (PortBox != null)
                {
                    if (!PortBox.IsDisposed)
                        PortBox.Dispose();

                    PortBox = null;
                }
                #endregion

                #region Colours
                if (ColourTab != null)
                {
                    if (!ColourTab.IsDisposed)
                        ColourTab.Dispose();

                    ColourTab = null;
                }

                if (LocalColourBox != null)
                {
                    if (!LocalColourBox.IsDisposed)
                        LocalColourBox.Dispose();

                    LocalColourBox = null;
                }

                if (GMWhisperInColourBox != null)
                {
                    if (!GMWhisperInColourBox.IsDisposed)
                        GMWhisperInColourBox.Dispose();

                    GMWhisperInColourBox = null;
                }

                if (WhisperInColourBox != null)
                {
                    if (!WhisperInColourBox.IsDisposed)
                        WhisperInColourBox.Dispose();

                    WhisperInColourBox = null;
                }

                if (WhisperOutColourBox != null)
                {
                    if (!WhisperOutColourBox.IsDisposed)
                        WhisperOutColourBox.Dispose();

                    WhisperOutColourBox = null;
                }

                if (GroupColourBox != null)
                {
                    if (!GroupColourBox.IsDisposed)
                        GroupColourBox.Dispose();

                    GroupColourBox = null;
                }

                if (GuildColourBox != null)
                {
                    if (!GuildColourBox.IsDisposed)
                        GuildColourBox.Dispose();

                    GuildColourBox = null;
                }

                if (ShoutColourBox != null)
                {
                    if (!ShoutColourBox.IsDisposed)
                        ShoutColourBox.Dispose();

                    ShoutColourBox = null;
                }

                if (GlobalColourBox != null)
                {
                    if (!GlobalColourBox.IsDisposed)
                        GlobalColourBox.Dispose();

                    GlobalColourBox = null;
                }

                if (ObserverColourBox != null)
                {
                    if (!ObserverColourBox.IsDisposed)
                        ObserverColourBox.Dispose();

                    ObserverColourBox = null;
                }

                if (HintColourBox != null)
                {
                    if (!HintColourBox.IsDisposed)
                        HintColourBox.Dispose();

                    HintColourBox = null;
                }

                if (SystemColourBox != null)
                {
                    if (!SystemColourBox.IsDisposed)
                        SystemColourBox.Dispose();

                    SystemColourBox = null;
                }

                if (GainsColourBox != null)
                {
                    if (!GainsColourBox.IsDisposed)
                        GainsColourBox.Dispose();

                    GainsColourBox = null;
                }
                #endregion

                if (SaveButton != null)
                {
                    if (!SaveButton.IsDisposed)
                        SaveButton.Dispose();

                    SaveButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }
                
                if (ExitButton != null)
                {
                    if (!ExitButton.IsDisposed)
                        ExitButton.Dispose();

                    ExitButton = null;
                }
            }
        }

        #endregion
    }
}
