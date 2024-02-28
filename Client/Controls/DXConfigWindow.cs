using System.Drawing;
using System.Windows.Forms;
using Client.Envir;
using Client.Envir.Translations;
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
        public DXCheckBox FullScreenCheckBox, VSyncCheckBox, LimitFPSCheckBox, ClipMouseCheckBox, DebugLabelCheckBox, SmoothMoveCheckBox;
        private DXComboBox GameSizeComboBox, LanguageComboBox;

        //Sound
        public DXTab SoundTab;
        private DXNumberBox SystemVolumeBox, MusicVolumeBox, SpellVolumeBox, PlayerVolumeBox, MonsterVolumeBox;
        private DXCheckBox BackgroundSoundBox;

        //Game 
        public DXTab GameTab;
        private DXCheckBox ItemNameCheckBox, MonsterNameCheckBox, PlayerNameCheckBox, UserHealthCheckBox, MonsterHealthCheckBox, DamageNumbersCheckBox, 
            EscapeCloseAllCheckBox, ShiftOpenChatCheckBox, RightClickDeTargetCheckBox, MonsterBoxVisibleCheckBox, LogChatCheckBox, DrawEffectsCheckBox, 
            DrawParticlesCheckBox, DrawWeatherCheckBox;
        public DXCheckBox DisplayHelmetCheckBox, HideChatBarCheckBox;

        public DXButton KeyBindButton;

        //Network
        public DXTab NetworkTab;
        private DXCheckBox UseNetworkConfigCheckBox;
        private DXTextBox IPAddressTextBox;
        private DXNumberBox PortBox;

        //Colours
        public DXTab ColourTab;
        public DXColourControl LocalForeColourBox, GMWhisperInForeColourBox, WhisperInForeColourBox, WhisperOutForeColourBox, GroupForeColourBox, GuildForeColourBox, ShoutForeColourBox, GlobalForeColourBox, ObserverForeColourBox, HintForeColourBox, SystemForeColourBox, GainsForeColourBox, AnnouncementForeColourBox;
        public DXColourControl LocalBackColourBox, GMWhisperInBackColourBox, WhisperInBackColourBox, WhisperOutBackColourBox, GroupBackColourBox, GuildBackColourBox, ShoutBackColourBox, GlobalBackColourBox, ObserverBackColourBox, HintBackColourBox, SystemBackColourBox, GainsBackColourBox, AnnouncementBackColourBox;
        public DXButton ResetColoursButton;

        private DXButton SaveButton, CancelButton;

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!IsVisible) return;

            FullScreenCheckBox.Checked = Config.FullScreen;
            GameSizeComboBox.ListBox.SelectItem(Config.GameSize);
            VSyncCheckBox.Checked = Config.VSync;
            LimitFPSCheckBox.Checked = Config.LimitFPS;
            SmoothMoveCheckBox.Checked = Config.SmoothMove;
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
            DrawWeatherCheckBox.Checked = Config.DrawWeather;
            HideChatBarCheckBox.Checked = Config.HideChatBar;

            LocalForeColourBox.BackColour = Config.LocalTextForeColour;
            GMWhisperInForeColourBox.BackColour = Config.GMWhisperInTextForeColour;
            WhisperInForeColourBox.BackColour = Config.WhisperInTextForeColour;
            WhisperOutForeColourBox.BackColour = Config.WhisperOutTextForeColour;
            GroupForeColourBox.BackColour = Config.GroupTextForeColour;
            GuildForeColourBox.BackColour = Config.GuildTextForeColour;
            ShoutForeColourBox.BackColour = Config.ShoutTextForeColour;
            GlobalForeColourBox.BackColour = Config.GlobalTextForeColour;
            ObserverForeColourBox.BackColour = Config.ObserverTextForeColour;
            HintForeColourBox.BackColour = Config.HintTextForeColour;
            SystemForeColourBox.BackColour = Config.SystemTextForeColour;
            GainsForeColourBox.BackColour = Config.GainsTextForeColour;
            AnnouncementForeColourBox.BackColour = Config.AnnouncementTextForeColour;

            LocalBackColourBox.BackColour = Config.LocalTextBackColour;
            GMWhisperInBackColourBox.BackColour = Config.GMWhisperInTextBackColour;
            WhisperInBackColourBox.BackColour = Config.WhisperInTextBackColour;
            WhisperOutBackColourBox.BackColour = Config.WhisperOutTextBackColour;
            GroupBackColourBox.BackColour = Config.GroupTextBackColour;
            GuildBackColourBox.BackColour = Config.GuildTextBackColour;
            ShoutBackColourBox.BackColour = Config.ShoutTextBackColour;
            GlobalBackColourBox.BackColour = Config.GlobalTextBackColour;
            ObserverBackColourBox.BackColour = Config.ObserverTextBackColour;
            HintBackColourBox.BackColour = Config.HintTextBackColour;
            SystemBackColourBox.BackColour = Config.SystemTextBackColour;
            GainsBackColourBox.BackColour = Config.GainsTextBackColour;
            AnnouncementBackColourBox.BackColour = Config.AnnouncementTextBackColour;
        }
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            KeyBindWindow.Parent = nValue;
        }

        public override WindowType Type => WindowType.ConfigBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public DXConfigWindow()
        {
            ActiveConfig = this;

            Size = new Size(300, 355);
            TitleLabel.Text = CEnvir.Language.CommonControlConfigWindowTitle;
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
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabLabel } },
            };

            SoundTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowSoundTabLabel } },
            };

            GameTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabLabel } },
            };

            NetworkTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowNetworkTabLabel } },
            };

            ColourTab = new DXTab
            {
                Parent = TabControl,
                Border = true,
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowColoursTabLabel }, Visible = false },
            };


            KeyBindWindow = new DXKeyBindWindow
            {
                Visible =  false
            };

            #region Graphics
            
            FullScreenCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabFullScreenLabel },
                Parent = GraphicsTab,
                Checked = Config.FullScreen,
            };
            FullScreenCheckBox.Location = new Point(120 - FullScreenCheckBox.Size.Width, 10);

            DXLabel label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabGameSizeLabel,
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

            foreach (Size resolution in DXManager.ValidResolutions)
                new DXListBoxItem
                {
                    Parent = GameSizeComboBox.ListBox,
                    Label = { Text = $"{resolution.Width} x {resolution.Height}" },
                    Item = resolution
                };

            VSyncCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabVSyncLabel },
                Parent = GraphicsTab,
            };
            VSyncCheckBox.Location = new Point(120 - VSyncCheckBox.Size.Width, 60);

            LimitFPSCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabLimitFPSLabel },
                Parent = GraphicsTab,
            };
            LimitFPSCheckBox.Location = new Point(120 - LimitFPSCheckBox.Size.Width, 80);

            SmoothMoveCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabSmoothMoveLabel },
                Parent = GraphicsTab,
            };
            SmoothMoveCheckBox.Location = new Point(120 - SmoothMoveCheckBox.Size.Width, 100);

            ClipMouseCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabClipMouseLabel },
                Parent = GraphicsTab,
            };
            ClipMouseCheckBox.Location = new Point(120 - ClipMouseCheckBox.Size.Width, 120);

            DebugLabelCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabDebugLabelLabel },
                Parent = GraphicsTab,
            };
            DebugLabelCheckBox.Location = new Point(120 - DebugLabelCheckBox.Size.Width, 140);

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabLanguageLabel,
                Outline = true,
                Parent = GraphicsTab,
            };
            label.Location = new Point(104 - label.Size.Width, 160);

            LanguageComboBox = new DXComboBox
            {
                Parent = GraphicsTab,
                Location = new Point(104, 160),
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
                Label = { Text = CEnvir.Language.CommonControlConfigWindowSoundTabBackgroundSoundLabel },
                Parent = SoundTab,
                Checked = Config.SoundInBackground,
            };
            BackgroundSoundBox.Location = new Point(120 - BackgroundSoundBox.Size.Width, 10);

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowSoundTabSystemVolumeLabel,
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
                Text = CEnvir.Language.CommonControlConfigWindowSoundTabMusicVolumeLabel,
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
                Text = CEnvir.Language.CommonControlConfigWindowSoundTabPlayerVolumeLabel,
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
                Text = CEnvir.Language.CommonControlConfigWindowSoundTabMonsterVolumeLabel,
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
                Text = CEnvir.Language.CommonControlConfigWindowSoundTabMagicVolumeLabel,
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
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabItemNameLabel },
                Parent = GameTab,
            };
            ItemNameCheckBox.Location = new Point(120 - ItemNameCheckBox.Size.Width, 10);

            MonsterNameCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabMonsterNameLabel },
                Parent = GameTab,
            };
            MonsterNameCheckBox.Location = new Point(120 - MonsterNameCheckBox.Size.Width, 35);

            PlayerNameCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabPlayerNameLabel },
                Parent = GameTab,
            };
            PlayerNameCheckBox.Location = new Point(120 - PlayerNameCheckBox.Size.Width, 60);

            UserHealthCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabUserHealthLabel },
                Parent = GameTab,
            };
            UserHealthCheckBox.Location = new Point(120 - UserHealthCheckBox.Size.Width, 85);

            MonsterHealthCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabMonsterHealthLabel },
                Parent = GameTab,
            };
            MonsterHealthCheckBox.Location = new Point(120 - MonsterHealthCheckBox.Size.Width, 110);

            DamageNumbersCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabDamageNumbersLabel },
                Parent = GameTab,
            };
            DamageNumbersCheckBox.Location = new Point(120 - DamageNumbersCheckBox.Size.Width, 135);

            DrawParticlesCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabDrawParticlesLabel },
                Parent = GameTab,
            };
            DrawParticlesCheckBox.Location = new Point(120 - DrawParticlesCheckBox.Size.Width, 160);

            DisplayHelmetCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabDisplayHelmetLabel },
                Parent = GameTab,
            };
            DisplayHelmetCheckBox.Location = new Point(120 - DisplayHelmetCheckBox.Size.Width, 185);
            DisplayHelmetCheckBox.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.HelmetToggle { HideHelmet = DisplayHelmetCheckBox.Checked });
            };

            HideChatBarCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabHideChatBarLabel },
                Parent = GameTab,
                Hint = "Hide chat bar when not active"
            };
            HideChatBarCheckBox.Location = new Point(120 - HideChatBarCheckBox.Size.Width, 210);
            HideChatBarCheckBox.MouseClick += (o, e) =>
            {
                if (HideChatBarCheckBox.Checked)
                {
                    GameScene.Game.ChatTextBox.Visible = true;
                }
            };

            EscapeCloseAllCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabEscapeCloseAllLabel },
                Parent = GameTab,
            };
            EscapeCloseAllCheckBox.Location = new Point(270 - EscapeCloseAllCheckBox.Size.Width, 10);

            ShiftOpenChatCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabShiftOpenChatLabel },
                Parent = GameTab,
                Hint = CEnvir.Language.CommonControlConfigWindowGameTabShiftOpenChatHint
            };
            ShiftOpenChatCheckBox.Location = new Point(270 - ShiftOpenChatCheckBox.Size.Width, 35);

            RightClickDeTargetCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabRightClickDeTargetLabel },
                Parent = GameTab,
                Hint = CEnvir.Language.CommonControlConfigWindowGameTabRightClickDeTargetHint
            };
            RightClickDeTargetCheckBox.Location = new Point(270 - RightClickDeTargetCheckBox.Size.Width, 60);

            MonsterBoxVisibleCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabMonsterBoxVisibleLabel },
                Parent = GameTab,
            };
            MonsterBoxVisibleCheckBox.Location = new Point(270 - MonsterBoxVisibleCheckBox.Size.Width, 85);

            LogChatCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabLogChatLabel },
                Parent = GameTab,
            };
            LogChatCheckBox.Location = new Point(270 - LogChatCheckBox.Size.Width, 110);

            DrawEffectsCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabDrawEffectsLabel },
                Parent = GameTab,
            };
            DrawEffectsCheckBox.Location = new Point(270 - DrawEffectsCheckBox.Size.Width, 135);

            DrawWeatherCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabDrawWeatherLabel },
                Parent = GameTab,
            };
            DrawWeatherCheckBox.Location = new Point(270 - DrawWeatherCheckBox.Size.Width, 160);

            KeyBindButton = new DXButton
            {
                Parent = GameTab,
                Location = new Point(190, 185),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabKeyBindButtonLabel }
            };
            KeyBindButton.MouseClick += (o, e) => KeyBindWindow.Visible = !KeyBindWindow.Visible;
            
            #endregion

            #region Network

            UseNetworkConfigCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowNetworkTabUseNetworkConfigLabel },
                Parent = NetworkTab,
                Checked = Config.FullScreen,
            };
            UseNetworkConfigCheckBox.Location = new Point(120 - UseNetworkConfigCheckBox.Size.Width, 10);

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowNetworkTabUseIPAddressLabel,
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
                Text = CEnvir.Language.CommonControlConfigWindowNetworkTabUsePortLabel,
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
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabLocalChatLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 10);

            LocalForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 10),
                Size = new Size(20, label.Size.Height),
            };

            LocalBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(110, 10),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabGMWhisperInLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 10);

            GMWhisperInForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 10),
                Size = new Size(20, label.Size.Height),
            };

            GMWhisperInBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(240, 10),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabWhisperInLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 35);

            WhisperInForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 35),
                Size = new Size(20, label.Size.Height),
            };

            WhisperInBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(110, 35),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabWhisperOutLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 35);

            WhisperOutForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 35),
                Size = new Size(20, label.Size.Height),
            };

            WhisperOutBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(240, 35),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabGroupChatLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 60);

            GroupForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 60),
                Size = new Size(20, label.Size.Height),
            };

            GroupBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(110, 60),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabGuildChatLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 60);

            GuildForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 60),
                Size = new Size(20, label.Size.Height),
            };

            GuildBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(240, 60),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabShoutChatLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 85);

            ShoutForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 85),
                Size = new Size(20, label.Size.Height),
            };

            ShoutBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(110, 85),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabGlobalChatLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 85);

            GlobalForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 85),
                Size = new Size(20, label.Size.Height),
            };

            GlobalBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(240, 85),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabObserverChatLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 110);

            ObserverForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 110),
                Size = new Size(20, label.Size.Height),
            };

            ObserverBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(110, 110),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabHintTextLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 110);

            HintForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 110),
                Size = new Size(20, label.Size.Height),
            };

            HintBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(240, 110),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabSystemTextLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 135);

            SystemForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 135),
                Size = new Size(20, label.Size.Height),
            };

            SystemBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(110, 135),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabGainsTextLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(220 - label.Size.Width, 135);

            GainsForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(220, 135),
                Size = new Size(20, label.Size.Height),
            };

            GainsBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(240, 135),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            label = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowColoursTabAnnouncementsLabel,
                Outline = true,
                Parent = ColourTab,
            };
            label.Location = new Point(90 - label.Size.Width, 160);

            AnnouncementForeColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(90, 160),
                Size = new Size(20, label.Size.Height),
            };

            AnnouncementBackColourBox = new DXColourControl
            {
                Parent = ColourTab,
                Location = new Point(110, 160),
                Size = new Size(20, label.Size.Height),
                AllowNoColour = true,
            };

            ResetColoursButton = new DXButton
            {
                Parent = ColourTab,
                Location = new Point(180, 160),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.CommonControlConfigWindowColoursTabResetColoursButtonLabel }
            };
            ResetColoursButton.MouseClick += (o, e) =>
            {
                LocalForeColourBox.BackColour = Color.White;
                GMWhisperInForeColourBox.BackColour = Color.Red;
                WhisperInForeColourBox.BackColour = Color.Cyan;
                WhisperOutForeColourBox.BackColour = Color.Aquamarine;
                GroupForeColourBox.BackColour = Color.Plum;
                GuildForeColourBox.BackColour = Color.LightPink;
                ShoutForeColourBox.BackColour = Color.Yellow;
                GlobalForeColourBox.BackColour = Color.Lime;
                ObserverForeColourBox.BackColour = Color.Silver;
                HintForeColourBox.BackColour = Color.AntiqueWhite;
                SystemForeColourBox.BackColour = Color.Red;
                GainsForeColourBox.BackColour = Color.GreenYellow;
                AnnouncementForeColourBox.BackColour = Color.DarkBlue;

                LocalBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                GMWhisperInBackColourBox.BackColour = Color.FromArgb(255, 255, 255, 255);
                WhisperInBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                WhisperOutBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                GroupBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                GuildBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                ShoutBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                GlobalBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                ObserverBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                HintBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                SystemBackColourBox.BackColour = Color.FromArgb(255, 255, 255, 255);
                GainsBackColourBox.BackColour = Color.FromArgb(0, 0, 0, 0);
                AnnouncementBackColourBox.BackColour = Color.FromArgb(255, 255, 255, 255);
            };

            #endregion

            SaveButton = new DXButton
            {
                Location = new Point(Size.Width - 190, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlApply }
            };
            SaveButton.MouseClick += SaveSettings;

            CancelButton = new DXButton
            {
                Location = new Point(Size.Width - 100, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlCancel }
            };
            CancelButton.MouseClick += CancelSettings;
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

                CEnvir.LoadLanguage();

                if (CEnvir.Connection != null && CEnvir.Connection.ServerConnected)
                    CEnvir.Enqueue(new C.SelectLanguage { Language = Config.Language });
            }


            if (Config.VSync != VSyncCheckBox.Checked)
            {
                Config.VSync = VSyncCheckBox.Checked;
                DXManager.ResetDevice();
            }

            Config.LimitFPS = LimitFPSCheckBox.Checked;
            Config.SmoothMove = SmoothMoveCheckBox.Checked;
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
            Config.DrawWeather = DrawWeatherCheckBox.Checked;
            Config.HideChatBar = HideChatBarCheckBox.Checked;

            if (volumeChanged)
                DXSoundManager.AdjustVolume();

            Config.UseNetworkConfig = UseNetworkConfigCheckBox.Checked;
            Config.IPAddress = IPAddressTextBox.TextBox.Text;
            Config.Port = (int)PortBox.Value;


            bool coloursChanged = false;

            //Fore Colours

            if (Config.LocalTextForeColour != LocalForeColourBox.BackColour)
            {
                Config.LocalTextForeColour = LocalForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GMWhisperInTextForeColour != GMWhisperInForeColourBox.BackColour)
            {
                Config.GMWhisperInTextForeColour = GMWhisperInForeColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.WhisperInTextForeColour != WhisperInForeColourBox.BackColour)
            {
                Config.WhisperInTextForeColour = WhisperInForeColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.WhisperOutTextForeColour != WhisperOutForeColourBox.BackColour)
            {
                Config.WhisperOutTextForeColour = WhisperOutForeColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.GroupTextForeColour != GroupForeColourBox.BackColour)
            {
                Config.GroupTextForeColour = GroupForeColourBox.BackColour;
                coloursChanged = true;
            }
            
            if (Config.GuildTextForeColour != GuildForeColourBox.BackColour)
            {
                Config.GuildTextForeColour = GuildForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ShoutTextForeColour != ShoutForeColourBox.BackColour)
            {
                Config.ShoutTextForeColour = ShoutForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GlobalTextForeColour != GlobalForeColourBox.BackColour)
            {
                Config.GlobalTextForeColour = GlobalForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ObserverTextForeColour != ObserverForeColourBox.BackColour)
            {
                Config.ObserverTextForeColour = ObserverForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.HintTextForeColour != HintForeColourBox.BackColour)
            {
                Config.HintTextForeColour = HintForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.SystemTextForeColour != SystemForeColourBox.BackColour)
            {
                Config.SystemTextForeColour = SystemForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GainsTextForeColour != GainsForeColourBox.BackColour)
            {
                Config.GainsTextForeColour = GainsForeColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.AnnouncementTextForeColour != AnnouncementForeColourBox.BackColour)
            {
                Config.AnnouncementTextForeColour = AnnouncementForeColourBox.BackColour;
                coloursChanged = true;
            }

            //Back Colours

            if (Config.LocalTextBackColour != LocalBackColourBox.BackColour)
            {
                Config.LocalTextBackColour = LocalBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GMWhisperInTextBackColour != GMWhisperInBackColourBox.BackColour)
            {
                Config.GMWhisperInTextBackColour = GMWhisperInBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.WhisperInTextBackColour != WhisperInBackColourBox.BackColour)
            {
                Config.WhisperInTextBackColour = WhisperInBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.WhisperOutTextBackColour != WhisperOutBackColourBox.BackColour)
            {
                Config.WhisperOutTextBackColour = WhisperOutBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GroupTextBackColour != GroupBackColourBox.BackColour)
            {
                Config.GroupTextBackColour = GroupBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GuildTextBackColour != GuildBackColourBox.BackColour)
            {
                Config.GuildTextBackColour = GuildBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ShoutTextBackColour != ShoutBackColourBox.BackColour)
            {
                Config.ShoutTextBackColour = ShoutBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GlobalTextBackColour != GlobalBackColourBox.BackColour)
            {
                Config.GlobalTextBackColour = GlobalBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ObserverTextBackColour != ObserverBackColourBox.BackColour)
            {
                Config.ObserverTextBackColour = ObserverBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.HintTextBackColour != HintBackColourBox.BackColour)
            {
                Config.HintTextBackColour = HintBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.SystemTextBackColour != SystemBackColourBox.BackColour)
            {
                Config.SystemTextBackColour = SystemBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GainsTextBackColour != GainsBackColourBox.BackColour)
            {
                Config.GainsTextBackColour = GainsBackColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.AnnouncementTextBackColour != AnnouncementBackColourBox.BackColour)
            {
                Config.AnnouncementTextBackColour = AnnouncementBackColourBox.BackColour;
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
                if (SmoothMoveCheckBox != null)
                {
                    if (!SmoothMoveCheckBox.IsDisposed)
                        SmoothMoveCheckBox.Dispose();

                    SmoothMoveCheckBox = null;
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


                if (DrawParticlesCheckBox != null)
                {
                    if (!DrawParticlesCheckBox.IsDisposed)
                        DrawParticlesCheckBox.Dispose();

                    DrawParticlesCheckBox = null;
                }


                if (DisplayHelmetCheckBox != null)
                {
                    if (!DisplayHelmetCheckBox.IsDisposed)
                        DisplayHelmetCheckBox.Dispose();

                    DisplayHelmetCheckBox = null;
                }


                if (HideChatBarCheckBox != null)
                {
                    if (!HideChatBarCheckBox.IsDisposed)
                        HideChatBarCheckBox.Dispose();

                    HideChatBarCheckBox = null;
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

                if (LocalForeColourBox != null)
                {
                    if (!LocalForeColourBox.IsDisposed)
                        LocalForeColourBox.Dispose();

                    LocalForeColourBox = null;
                }

                if (GMWhisperInForeColourBox != null)
                {
                    if (!GMWhisperInForeColourBox.IsDisposed)
                        GMWhisperInForeColourBox.Dispose();

                    GMWhisperInForeColourBox = null;
                }

                if (WhisperInForeColourBox != null)
                {
                    if (!WhisperInForeColourBox.IsDisposed)
                        WhisperInForeColourBox.Dispose();

                    WhisperInForeColourBox = null;
                }

                if (WhisperOutForeColourBox != null)
                {
                    if (!WhisperOutForeColourBox.IsDisposed)
                        WhisperOutForeColourBox.Dispose();

                    WhisperOutForeColourBox = null;
                }

                if (GroupForeColourBox != null)
                {
                    if (!GroupForeColourBox.IsDisposed)
                        GroupForeColourBox.Dispose();

                    GroupForeColourBox = null;
                }

                if (GuildForeColourBox != null)
                {
                    if (!GuildForeColourBox.IsDisposed)
                        GuildForeColourBox.Dispose();

                    GuildForeColourBox = null;
                }

                if (ShoutForeColourBox != null)
                {
                    if (!ShoutForeColourBox.IsDisposed)
                        ShoutForeColourBox.Dispose();

                    ShoutForeColourBox = null;
                }

                if (GlobalForeColourBox != null)
                {
                    if (!GlobalForeColourBox.IsDisposed)
                        GlobalForeColourBox.Dispose();

                    GlobalForeColourBox = null;
                }

                if (ObserverForeColourBox != null)
                {
                    if (!ObserverForeColourBox.IsDisposed)
                        ObserverForeColourBox.Dispose();

                    ObserverForeColourBox = null;
                }

                if (HintForeColourBox != null)
                {
                    if (!HintForeColourBox.IsDisposed)
                        HintForeColourBox.Dispose();

                    HintForeColourBox = null;
                }

                if (SystemForeColourBox != null)
                {
                    if (!SystemForeColourBox.IsDisposed)
                        SystemForeColourBox.Dispose();

                    SystemForeColourBox = null;
                }

                if (GainsForeColourBox != null)
                {
                    if (!GainsForeColourBox.IsDisposed)
                        GainsForeColourBox.Dispose();

                    GainsForeColourBox = null;
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
            }
        }

        #endregion
    }
}
