using Client.Envir;
using Client.Rendering;
using Client.Scenes;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Controls
{
    public sealed class DXConfigWindow : DXImageControl
    {
        public static DXConfigWindow ActiveConfig;

        public DXKeyBindWindow KeyBindWindow;

        private DXTabControl TabControl;
        public DXConfigTab GraphicsTab, SoundTab, GameTab, NetworkTab, UITab;

        //Graphics
        public DXCheckBox FullScreenCheckBox, VSyncCheckBox, LimitFPSCheckBox, ClipMouseCheckBox, DebugLabelCheckBox, SmoothMoveCheckBox;
        private DXComboBox GameSizeComboBox, LanguageComboBox, RenderingPipelineComboBox;

        //Sound
        public DXCheckBox BackgroundSoundBox;
        public DXSoundBar SoundMusicBar, SoundSystemBar, SoundPlayerBar, SoundMonsterBar, SoundMagicBar;

        //Game
        private DXCheckBox ItemNameCheckBox, MonsterNameCheckBox, PlayerNameCheckBox, UserHealthCheckBox, MonsterHealthCheckBox, DamageNumbersCheckBox,
            EscapeCloseAllCheckBox, ShiftOpenChatCheckBox, RightClickDeTargetCheckBox, MonsterBoxVisibleCheckBox, LogChatCheckBox, DrawEffectsCheckBox,
            DrawParticlesCheckBox, DrawWeatherCheckBox, ShowTargetOutlineCheckBox;
        public DXCheckBox DisplayHelmetCheckBox, HideChatBarCheckBox;
        public DXButton KeyBindButton;

        //Network
        private DXCheckBox UseNetworkConfigCheckBox;
        private DXTextBox IPAddressTextBox;
        private DXNumberBox PortBox;

        //Chat
        public DXColourControlPair LocalColourBox, GMWhisperInColourBox, WhisperInColourBox, WhisperOutColourBox, GroupColourBox, GuildColourBox, ShoutColourBox, GlobalColourBox, ObserverColourBox, HintColourBox, SystemColourBox, GainsColourBox, AnnouncementColourBox;
        public DXButton ResetColoursButton;

        private DXButton CloseButton;
        private DXLabel TitleLabel;

        #region Properties

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!IsVisible) return;

            FullScreenCheckBox.Enabled = ActiveScene is GameScene;
            GameSizeComboBox.Enabled = ActiveScene is GameScene;
            RenderingPipelineComboBox.Enabled = ActiveScene is GameScene;

            FullScreenCheckBox.Checked = Config.FullScreen;
            GameSizeComboBox.ListBox.SelectItem(Config.GameSize);
            VSyncCheckBox.Checked = Config.VSync;
            LimitFPSCheckBox.Checked = Config.LimitFPS;
            SmoothMoveCheckBox.Checked = Config.SmoothMove;
            ClipMouseCheckBox.Checked = Config.ClipMouse;
            DebugLabelCheckBox.Checked = Config.DebugLabel;
            LanguageComboBox.ListBox.SelectItem(Config.Language);
            RenderingPipelineComboBox.ListBox.SelectItem(Config.RenderingPipeline);

            BackgroundSoundBox.Checked = Config.SoundInBackground;
            SoundSystemBar.Value = Config.SystemVolume;
            SoundSystemBar.Muted = Config.SystemVolumeMuted;
            SoundMusicBar.Value = Config.MusicVolume;
            SoundMusicBar.Muted = Config.MusicVolumeMuted;
            SoundPlayerBar.Value = Config.PlayerVolume;
            SoundPlayerBar.Muted = Config.PlayerVolumeMuted;
            SoundMonsterBar.Value = Config.MonsterVolume;
            SoundMonsterBar.Muted = Config.MonsterVolumeMuted;
            SoundMagicBar.Value = Config.MagicVolume;
            SoundMagicBar.Muted = Config.MagicVolumeMuted;

            UseNetworkConfigCheckBox.Checked = Config.UseNetworkConfig;
            IPAddressTextBox.TextBox.Text = Config.IPAddress;
            PortBox.ValueTextBox.TextBox.Text = Config.Port.ToString();

            ItemNameCheckBox.Checked = Config.ShowItemNames;
            MonsterNameCheckBox.Checked = Config.ShowMonsterNames;
            PlayerNameCheckBox.Checked = Config.ShowPlayerNames;
            UserHealthCheckBox.Checked = Config.ShowUserHealth;
            MonsterHealthCheckBox.Checked = Config.ShowMonsterHealth;
            DamageNumbersCheckBox.Checked = Config.ShowDamageNumbers;
            DrawParticlesCheckBox.Checked = Config.DrawParticles;
            HideChatBarCheckBox.Checked = Config.HideChatBar;

            EscapeCloseAllCheckBox.Checked = Config.EscapeCloseAll;
            ShiftOpenChatCheckBox.Checked = Config.ShiftOpenChat;
            RightClickDeTargetCheckBox.Checked = Config.RightClickDeTarget;
            MonsterBoxVisibleCheckBox.Checked = Config.MonsterBoxVisible;
            LogChatCheckBox.Checked = Config.LogChat;
            DrawEffectsCheckBox.Checked = Config.DrawEffects;
            DrawWeatherCheckBox.Checked = Config.DrawWeather;
            ShowTargetOutlineCheckBox.Checked = Config.ShowTargetOutline;

            LocalColourBox.ForeColourControl.BackColour = Config.LocalTextForeColour;
            GMWhisperInColourBox.ForeColourControl.BackColour = Config.GMWhisperInTextForeColour;
            WhisperInColourBox.ForeColourControl.BackColour = Config.WhisperInTextForeColour;
            WhisperOutColourBox.ForeColourControl.BackColour = Config.WhisperOutTextForeColour;
            GroupColourBox.ForeColourControl.BackColour = Config.GroupTextForeColour;
            GuildColourBox.ForeColourControl.BackColour = Config.GuildTextForeColour;
            ShoutColourBox.ForeColourControl.BackColour = Config.ShoutTextForeColour;
            GlobalColourBox.ForeColourControl.BackColour = Config.GlobalTextForeColour;
            ObserverColourBox.ForeColourControl.BackColour = Config.ObserverTextForeColour;
            HintColourBox.ForeColourControl.BackColour = Config.HintTextForeColour;
            SystemColourBox.ForeColourControl.BackColour = Config.SystemTextForeColour;
            GainsColourBox.ForeColourControl.BackColour = Config.GainsTextForeColour;
            AnnouncementColourBox.ForeColourControl.BackColour = Config.AnnouncementTextForeColour;

            LocalColourBox.BackColourControl.BackColour = Config.LocalTextBackColour;
            GMWhisperInColourBox.BackColourControl.BackColour = Config.GMWhisperInTextBackColour;
            WhisperInColourBox.BackColourControl.BackColour = Config.WhisperInTextBackColour;
            WhisperOutColourBox.BackColourControl.BackColour = Config.WhisperOutTextBackColour;
            GroupColourBox.BackColourControl.BackColour = Config.GroupTextBackColour;
            GuildColourBox.BackColourControl.BackColour = Config.GuildTextBackColour;
            ShoutColourBox.BackColourControl.BackColour = Config.ShoutTextBackColour;
            GlobalColourBox.BackColourControl.BackColour = Config.GlobalTextBackColour;
            ObserverColourBox.BackColourControl.BackColour = Config.ObserverTextBackColour;
            HintColourBox.BackColourControl.BackColour = Config.HintTextBackColour;
            SystemColourBox.BackColourControl.BackColour = Config.SystemTextBackColour;
            GainsColourBox.BackColourControl.BackColour = Config.GainsTextBackColour;
            AnnouncementColourBox.BackColourControl.BackColour = Config.AnnouncementTextBackColour;
        }
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            KeyBindWindow.Parent = nValue;
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.ConfigBox;

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

        public DXConfigWindow()
        {
            ActiveConfig = this;

            Index = 282;
            LibraryFile = LibraryFile.Interface;
            Movable = true;
            Sort = true;
            DropShadow = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.CommonControlConfigWindowTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);
            
            KeyBindWindow = new DXKeyBindWindow
            {
                Visible = false
            };

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(0, 37),
                Size = new Size(357, 365),
                Border = false,
                MarginLeft = 10,
            };

            GraphicsTab = new DXConfigTab
            {
                Parent = TabControl,
                BackColour = Color.Empty,
                Location = new Point(8, 25),
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabLabel } },
            };

            SoundTab = new DXConfigTab
            {
                Parent = TabControl,
                Border = false,
                BackColour = Color.Empty,
                Location = new Point(8, 25),
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowSoundTabLabel } },
            };

            GameTab = new DXConfigTab
            {
                Parent = TabControl,
                Border = false,
                BackColour = Color.Empty,
                Location = new Point(8, 25),
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabLabel } },
            };

            NetworkTab = new DXConfigTab
            {
                Parent = TabControl,
                Border = false,
                BackColour = Color.Empty,
                Location = new Point(8, 25),
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowNetworkTabLabel } },
            };

            UITab = new DXConfigTab
            {
                Parent = TabControl,
                Border = false,
                BackColour = Color.Empty,
                Location = new Point(8, 25),
                TabButton = { Label = { Text = CEnvir.Language.CommonControlConfigWindowUITabLabel } },
            };

            const int checkboxPadding = 0;

            #region Graphics

            #region Display

            DXConfigSection displayGraphicsSection = new("Display")
            {
                Columns = 1,
                Parent = GraphicsTab,
                Location = new Point(0, 0)
            };
            GraphicsTab.AddSection(displayGraphicsSection);

            FullScreenCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabFullScreenLabel },
                LabelBoxPadding = checkboxPadding,
                Enabled = false
            };
            FullScreenCheckBox.CheckedChanged += (o, e) =>
            {
                if (Config.FullScreen != FullScreenCheckBox.Checked)
                {
                    RenderingPipelineManager.ToggleFullScreen();

                    if (!Config.FullScreen)
                    {
                        CEnvir.Target.ClientSize = Config.GameSize;
                        CEnvir.Target.Center();
                    }
                }
            };

            displayGraphicsSection.AddControl("", FullScreenCheckBox);

            RenderingPipelineComboBox = new DXComboBox
            {
                Size = new Size(122, DXComboBox.DefaultNormalHeight),
                Border = false,
                Background = { Visible = true },
                Enabled = false
            };
            RenderingPipelineComboBox.SelectedItemChanged += (o, e) =>
            {
                var renderingPipeline = RenderingPipelineManager.SupportsMultiplePipelines
                       ? RenderingPipelineComboBox.SelectedItem as string
                       : RenderingPipelineManager.DefaultPipelineIdentifier;

                RenderingPipelineManager.SwitchPipeline(renderingPipeline);
                Config.RenderingPipeline = renderingPipeline;
            };

            foreach (string pipelineId in RenderingPipelineManager.AvailablePipelineIds.OrderBy(x => x))
            {
                new DXListBoxItem
                {
                    Parent = RenderingPipelineComboBox.ListBox,
                    Label = { Text = pipelineId },
                    Item = pipelineId
                };
            }
            displayGraphicsSection.AddControl(CEnvir.Language.CommonControlConfigWindowGraphicsTabRenderingPipelineLabel, RenderingPipelineComboBox);

            GameSizeComboBox = new DXComboBox
            {
                Size = new Size(122, DXComboBox.DefaultNormalHeight),
                Border = false,
                Background = { Visible = true },
                Enabled = false
            };
            GameSizeComboBox.SelectedItemChanged += (o, e) => 
            {
                Config.GameSize = (Size)GameSizeComboBox.SelectedItem;

                if (ActiveScene is GameScene)
                {
                    ActiveScene.Size = Config.GameSize;
                    RenderingPipelineManager.SetResolution(Config.GameSize);
                }
            };

            IReadOnlyList<Size> supportedResolutions = RenderingPipelineManager.GetSupportedResolutions();

            foreach (Size resolution in supportedResolutions)
                new DXListBoxItem
                {
                    Parent = GameSizeComboBox.ListBox,
                    Label = { Text = $"{resolution.Width} x {resolution.Height}" },
                    Item = resolution
                };

            displayGraphicsSection.AddControl(CEnvir.Language.CommonControlConfigWindowGraphicsTabGameSizeLabel, GameSizeComboBox);

            VSyncCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabVSyncLabel },
                LabelBoxPadding = checkboxPadding
            };
            VSyncCheckBox.CheckedChanged += (o, e) =>
            {
                Config.VSync = VSyncCheckBox.Checked;
                RenderingPipelineManager.ResetDevice();
            };

            displayGraphicsSection.AddControl("", VSyncCheckBox);

            LimitFPSCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabLimitFPSLabel },
                LabelBoxPadding = checkboxPadding
            };
            LimitFPSCheckBox.CheckedChanged += (o, e) => Config.LimitFPS = LimitFPSCheckBox.Checked;

            displayGraphicsSection.AddControl("", LimitFPSCheckBox);

            #endregion

            #region Usability

            DXConfigSection displayUsabilitySection = new("Usability")
            {
                Columns = 1,
                Parent = GraphicsTab,
                Location = new Point(0, displayGraphicsSection.Size.Height )
            };
            GraphicsTab.AddSection(displayUsabilitySection);

            SmoothMoveCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabSmoothMoveLabel },
                LabelBoxPadding = checkboxPadding
            };
            SmoothMoveCheckBox.CheckedChanged += (o, e) => Config.SmoothMove = SmoothMoveCheckBox.Checked;
            displayUsabilitySection.AddControl("", SmoothMoveCheckBox);

            ClipMouseCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabClipMouseLabel },
                LabelBoxPadding = checkboxPadding
            };
            ClipMouseCheckBox.CheckedChanged += (o, e) => Config.ClipMouse = ClipMouseCheckBox.Checked;
            displayUsabilitySection.AddControl("", ClipMouseCheckBox);

            DebugLabelCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabDebugLabelLabel },
                LabelBoxPadding = checkboxPadding
            };
            DebugLabelCheckBox.CheckedChanged += (o, e) => Config.DebugLabel = DebugLabelCheckBox.Checked;
            displayUsabilitySection.AddControl("", DebugLabelCheckBox);

            LanguageComboBox = new DXComboBox
            {
                Size = new Size(122, DXComboBox.DefaultNormalHeight),
                Border = false,
                Background = { Visible = true }
            };
            LanguageComboBox.SelectedItemChanged += (o, e) =>
            {
                Config.Language = (string)LanguageComboBox.SelectedItem;

                CEnvir.LoadLanguage();

                if (CEnvir.Connection != null && CEnvir.Connection.ServerConnected)
                    CEnvir.Enqueue(new C.SelectLanguage { Language = Config.Language });
            };

            foreach (string language in Globals.Languages)
                new DXListBoxItem
                {
                    Parent = LanguageComboBox.ListBox,
                    Label = { Text = language },
                    Item = language
                };
            displayUsabilitySection.AddControl(CEnvir.Language.CommonControlConfigWindowGraphicsTabLanguageLabel, LanguageComboBox);

            #endregion

            #region Effects

            DXConfigSection displayEffectsSection = new("Effects")
            {
                Columns = 2,
                Parent = GraphicsTab,
                Location = new Point(0, displayGraphicsSection.Size.Height + displayUsabilitySection.Size.Height)
            };
            GraphicsTab.AddSection(displayEffectsSection);

            DrawParticlesCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabDrawParticlesLabel },
            };
            DrawParticlesCheckBox.CheckedChanged += (o, e) => Config.DrawParticles = DrawParticlesCheckBox.Checked;
            displayEffectsSection.AddControl("", DrawParticlesCheckBox);

            DrawEffectsCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabDrawEffectsLabel },
            };
            DrawEffectsCheckBox.CheckedChanged += (o, e) => Config.DrawEffects = DrawEffectsCheckBox.Checked;
            displayEffectsSection.AddControl("", DrawEffectsCheckBox);

            DrawWeatherCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabDrawWeatherLabel },
            };
            DrawWeatherCheckBox.CheckedChanged += (o, e) => Config.DrawWeather = DrawWeatherCheckBox.Checked;
            displayEffectsSection.AddControl("", DrawWeatherCheckBox);

            DisplayHelmetCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGraphicsTabDisplayHelmetLabel },
            };
            DisplayHelmetCheckBox.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.HelmetToggle { HideHelmet = DisplayHelmetCheckBox.Checked });
            };
            displayEffectsSection.AddControl("", DisplayHelmetCheckBox);

            #endregion

            #endregion

            #region Sound

            #region Options

            DXConfigSection soundSettingsSection = new DXConfigSection("Settings")
            {
                Columns = 1,
                Parent = SoundTab,
                Location = new Point(0, 0)
            };
            SoundTab.AddSection(soundSettingsSection);

            BackgroundSoundBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowSoundTabBackgroundSoundLabel },
                LabelBoxPadding = checkboxPadding,
                Checked = Config.SoundInBackground,
            };
            BackgroundSoundBox.CheckedChanged += (o, e) =>
            {
                Config.SoundInBackground = BackgroundSoundBox.Checked;
                DXSoundManager.UpdateFlags();
            };
            soundSettingsSection.AddControl("", BackgroundSoundBox);

            #endregion

            #region Volume

            DXConfigSection soundVolumeSection = new DXConfigSection("Volume")
            {
                Columns = 1,
                Parent = SoundTab,
            };
            SoundTab.AddSection(soundVolumeSection);

            SoundMusicBar = new DXSoundBar
            {
                Parent = soundVolumeSection
            };
            SoundMusicBar.ValueChanged += (o, e) =>
            {
                Config.MusicVolume = SoundMusicBar.Value;
                DXSoundManager.AdjustVolume();
            };
            SoundMusicBar.MutedChanged += (o, e) =>
            {
                Config.MusicVolumeMuted = SoundMusicBar.Muted;
                DXSoundManager.AdjustVolume();
            };
            soundVolumeSection.AddControl(CEnvir.Language.CommonControlConfigWindowSoundTabMusicVolumeLabel, SoundMusicBar);
    
            SoundSystemBar = new DXSoundBar
            {
                Parent = soundVolumeSection
            };
            SoundSystemBar.ValueChanged += (o, e) =>
            {
                Config.SystemVolume = SoundSystemBar.Value;
                DXSoundManager.AdjustVolume();
            };
            SoundSystemBar.MutedChanged += (o, e) =>
            {
                Config.SystemVolumeMuted = SoundSystemBar.Muted;
                DXSoundManager.AdjustVolume();
            };
            soundVolumeSection.AddControl(CEnvir.Language.CommonControlConfigWindowSoundTabSystemVolumeLabel, SoundSystemBar);
            
            SoundPlayerBar = new();
            SoundPlayerBar.ValueChanged += (o, e) =>
            {
                Config.PlayerVolume = SoundPlayerBar.Value;
                DXSoundManager.AdjustVolume();
            };
            SoundPlayerBar.MutedChanged += (o, e) =>
            {
                Config.PlayerVolumeMuted = SoundPlayerBar.Muted;
                DXSoundManager.AdjustVolume();
            };
            soundVolumeSection.AddControl(CEnvir.Language.CommonControlConfigWindowSoundTabPlayerVolumeLabel, SoundPlayerBar);
            
            SoundMonsterBar = new();
            SoundMonsterBar.ValueChanged += (o, e) =>
            {
                Config.MonsterVolume = SoundMonsterBar.Value;
                DXSoundManager.AdjustVolume();
            };
            SoundMonsterBar.MutedChanged += (o, e) =>
            {
                Config.MonsterVolumeMuted = SoundMonsterBar.Muted;
                DXSoundManager.AdjustVolume();
            };
            soundVolumeSection.AddControl(CEnvir.Language.CommonControlConfigWindowSoundTabMonsterVolumeLabel, SoundMonsterBar);
            
            SoundMagicBar = new();
            SoundMagicBar.ValueChanged += (o, e) =>
            {
                Config.MagicVolume = SoundMagicBar.Value;
                DXSoundManager.AdjustVolume();
            };
            SoundMagicBar.MutedChanged += (o, e) =>
            {
                Config.MagicVolumeMuted = SoundMagicBar.Muted;
                DXSoundManager.AdjustVolume();
            };
            soundVolumeSection.AddControl(CEnvir.Language.CommonControlConfigWindowSoundTabMagicVolumeLabel, SoundMagicBar);

            #endregion

            #endregion

            #region Game

            DXConfigSection gameSettingsSection = new("Settings")
            {
                Columns = 2,
                Parent = GameTab,
            };
            GameTab.AddSection(gameSettingsSection);

            ItemNameCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabItemNameLabel },
            };
            ItemNameCheckBox.CheckedChanged += (o, e) => Config.ShowItemNames = ItemNameCheckBox.Checked;
            gameSettingsSection.AddControl("", ItemNameCheckBox);

            MonsterNameCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabMonsterNameLabel },
            };
            MonsterNameCheckBox.CheckedChanged += (o, e) => Config.ShowMonsterNames = MonsterNameCheckBox.Checked;
            gameSettingsSection.AddControl("", MonsterNameCheckBox);

            PlayerNameCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabPlayerNameLabel },
            };
            PlayerNameCheckBox.CheckedChanged += (o, e) => Config.ShowPlayerNames = PlayerNameCheckBox.Checked;
            gameSettingsSection.AddControl("", PlayerNameCheckBox);

            UserHealthCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabUserHealthLabel },
            };
            UserHealthCheckBox.CheckedChanged += (o, e) => Config.ShowUserHealth = UserHealthCheckBox.Checked;
            gameSettingsSection.AddControl("", UserHealthCheckBox);

            MonsterHealthCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabMonsterHealthLabel },
            };
            MonsterHealthCheckBox.CheckedChanged += (o, e) => Config.ShowMonsterHealth = MonsterHealthCheckBox.Checked;
            gameSettingsSection.AddControl("", MonsterHealthCheckBox);

            DamageNumbersCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabDamageNumbersLabel },
            };
            DamageNumbersCheckBox.CheckedChanged += (o, e) => Config.ShowDamageNumbers = DamageNumbersCheckBox.Checked;
            gameSettingsSection.AddControl("", DamageNumbersCheckBox);

            RightClickDeTargetCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabRightClickDeTargetLabel },
                Hint = CEnvir.Language.CommonControlConfigWindowGameTabRightClickDeTargetHint
            };
            RightClickDeTargetCheckBox.CheckedChanged += (o, e) => Config.RightClickDeTarget = RightClickDeTargetCheckBox.Checked;
            gameSettingsSection.AddControl("", RightClickDeTargetCheckBox);

            MonsterBoxVisibleCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabMonsterBoxVisibleLabel },
            };
            MonsterBoxVisibleCheckBox.CheckedChanged += (o, e) => Config.MonsterBoxVisible = MonsterBoxVisibleCheckBox.Checked;
            gameSettingsSection.AddControl("", MonsterBoxVisibleCheckBox);

            ShowTargetOutlineCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowGameTabShowTargetOutlineLabel },
            };
            ShowTargetOutlineCheckBox.CheckedChanged += (o, e) => Config.ShowTargetOutline = ShowTargetOutlineCheckBox.Checked;
            gameSettingsSection.AddControl("", ShowTargetOutlineCheckBox);

            #endregion

            #region Network

            DXConfigSection networkNetworkSection = new("Network")
            {
                Columns = 1,
                Parent = NetworkTab,
            };
            NetworkTab.AddSection(networkNetworkSection);

            UseNetworkConfigCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowNetworkTabUseNetworkConfigLabel },
                Parent = NetworkTab,
                Checked = Config.FullScreen,
            };
            networkNetworkSection.AddControl("", UseNetworkConfigCheckBox);

            IPAddressTextBox = new DXTextBox
            {
                Location = new Point(104, 35),
                Size = new Size(100, 16),
                Parent = NetworkTab,
            };
            networkNetworkSection.AddControl(CEnvir.Language.CommonControlConfigWindowNetworkTabUseIPAddressLabel, IPAddressTextBox);

            PortBox = new DXNumberBox
            {
                Parent = NetworkTab,
                Change = 100,
                MaxValue = ushort.MaxValue,
                Location = new Point(104, 60)
            };

            networkNetworkSection.AddControl(CEnvir.Language.CommonControlConfigWindowNetworkTabUsePortLabel, PortBox);

            #endregion

            #region UI

            #region Settings

            DXConfigSection uiSettingsSection = new("Settings")
            {
                Columns = 2,
                Parent = UITab,
            };
            UITab.AddSection(uiSettingsSection);

            HideChatBarCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowUITabHideChatBarLabel },
                Hint = "Hide chat bar when not active"
            };
            HideChatBarCheckBox.MouseClick += (o, e) =>
            {
                if (HideChatBarCheckBox.Checked)
                {
                    GameScene.Game.ChatTextBox.Visible = true;
                }
            };
            HideChatBarCheckBox.CheckedChanged += (o, e) => Config.HideChatBar = HideChatBarCheckBox.Checked;
            uiSettingsSection.AddControl("", HideChatBarCheckBox);

            ShiftOpenChatCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowUITabShiftOpenChatLabel },
                Hint = CEnvir.Language.CommonControlConfigWindowUITabShiftOpenChatHint
            };
            ShiftOpenChatCheckBox.CheckedChanged += (o, e) => Config.ShiftOpenChat = ShiftOpenChatCheckBox.Checked;
            uiSettingsSection.AddControl("", ShiftOpenChatCheckBox);

            EscapeCloseAllCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowUITabEscapeCloseAllLabel },
            };
            EscapeCloseAllCheckBox.CheckedChanged += (o, e) => Config.EscapeCloseAll = EscapeCloseAllCheckBox.Checked;
            uiSettingsSection.AddControl("", EscapeCloseAllCheckBox);

            LogChatCheckBox = new DXCheckBox
            {
                Label = { Text = CEnvir.Language.CommonControlConfigWindowUITabLogChatLabel },
            };
            LogChatCheckBox.CheckedChanged += (o, e) => Config.LogChat = LogChatCheckBox.Checked;
            uiSettingsSection.AddControl("", LogChatCheckBox);

            KeyBindButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.CommonControlConfigWindowUITabKeyBindButtonLabel }
            };
            KeyBindButton.MouseClick += (o, e) => KeyBindWindow.Visible = !KeyBindWindow.Visible;
            uiSettingsSection.AddControl("", KeyBindButton);

            #endregion

            #region Colours

            DXConfigSection uiColoursSection = new("Colours")
            {
                Columns = 2,
                Parent = UITab,
            };
            UITab.AddSection(uiColoursSection);

            LocalColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabLocalChatLabel, LocalColourBox);

            GMWhisperInColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabGMWhisperInLabel, GMWhisperInColourBox);

            WhisperInColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabWhisperInLabel, WhisperInColourBox);

            WhisperOutColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabWhisperOutLabel, WhisperOutColourBox);

            GroupColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabGroupChatLabel, GroupColourBox);

            GuildColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabGuildChatLabel, GuildColourBox);

            ShoutColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabShoutChatLabel, ShoutColourBox);

            GlobalColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabGlobalChatLabel, GlobalColourBox);

            ObserverColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabObserverChatLabel, ObserverColourBox);

            HintColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabHintTextLabel, HintColourBox);

            SystemColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabSystemTextLabel, SystemColourBox);

            GainsColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabGainsTextLabel, GainsColourBox);

            AnnouncementColourBox = new DXColourControlPair();
            uiColoursSection.AddControl(CEnvir.Language.CommonControlConfigWindowColoursTabAnnouncementsLabel, AnnouncementColourBox);

            ResetColoursButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.CommonControlConfigWindowColoursTabResetColoursButtonLabel }
            };
            ResetColoursButton.MouseClick += (o, e) =>
            {
                LocalColourBox.ForeColourControl.BackColour = Color.White;
                GMWhisperInColourBox.ForeColourControl.BackColour = Color.Red;
                WhisperInColourBox.ForeColourControl.BackColour = Color.Cyan;
                WhisperOutColourBox.ForeColourControl.BackColour = Color.Aquamarine;
                GroupColourBox.ForeColourControl.BackColour = Color.Plum;
                GuildColourBox.ForeColourControl.BackColour = Color.LightPink;
                ShoutColourBox.ForeColourControl.BackColour = Color.Yellow;
                GlobalColourBox.ForeColourControl.BackColour = Color.Lime;
                ObserverColourBox.ForeColourControl.BackColour = Color.Silver;
                HintColourBox.ForeColourControl.BackColour = Color.AntiqueWhite;
                SystemColourBox.ForeColourControl.BackColour = Color.Red;
                GainsColourBox.ForeColourControl.BackColour = Color.GreenYellow;
                AnnouncementColourBox.ForeColourControl.BackColour = Color.DarkBlue;

                LocalColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                GMWhisperInColourBox.BackColourControl.BackColour = Color.FromArgb(255, 255, 255, 255);
                WhisperInColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                WhisperOutColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                GroupColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                GuildColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                ShoutColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                GlobalColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                ObserverColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                HintColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                SystemColourBox.BackColourControl.BackColour = Color.FromArgb(255, 255, 255, 255);
                GainsColourBox.BackColourControl.BackColour = Color.FromArgb(0, 0, 0, 0);
                AnnouncementColourBox.BackColourControl.BackColour = Color.FromArgb(255, 255, 255, 255);
            };
            uiColoursSection.AddControl("", ResetColoursButton);

            #endregion

            #endregion
        }

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

                if (SoundSystemBar != null)
                {
                    if (!SoundSystemBar.IsDisposed)
                        SoundSystemBar.Dispose();

                    SoundSystemBar = null;
                }

                if (SoundMusicBar != null)
                {
                    if (!SoundMusicBar.IsDisposed)
                        SoundMusicBar.Dispose();

                    SoundMusicBar = null;
                }

                if (SoundPlayerBar != null)
                {
                    if (!SoundPlayerBar.IsDisposed)
                        SoundPlayerBar.Dispose();

                    SoundPlayerBar = null;
                }

                if (SoundMonsterBar != null)
                {
                    if (!SoundMonsterBar.IsDisposed)
                        SoundMonsterBar.Dispose();

                    SoundMonsterBar = null;
                }

                if (SoundMagicBar != null)
                {
                    if (!SoundMagicBar.IsDisposed)
                        SoundMagicBar.Dispose();

                    SoundMagicBar = null;
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

                if (DrawParticlesCheckBox != null)
                {
                    if (!DrawParticlesCheckBox.IsDisposed)
                        DrawParticlesCheckBox.Dispose();

                    DrawParticlesCheckBox = null;
                }

                if (DrawWeatherCheckBox != null)
                {
                    if (!DrawWeatherCheckBox.IsDisposed)
                        DrawWeatherCheckBox.Dispose();

                    DrawWeatherCheckBox = null;
                }

                if (ShowTargetOutlineCheckBox != null)
                {
                    if (!ShowTargetOutlineCheckBox.IsDisposed)
                        ShowTargetOutlineCheckBox.Dispose();
                    ShowTargetOutlineCheckBox = null;
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

                if (UITab != null)
                {
                    if (!UITab.IsDisposed)
                        UITab.Dispose();

                    UITab = null;
                }

                if (ResetColoursButton != null)
                {
                    if (!ResetColoursButton.IsDisposed)
                        ResetColoursButton.Dispose();

                    ResetColoursButton = null;
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
            }
        }

        #endregion
    }

    public sealed class DXConfigTab : DXTab
    {
        public int ScrollOffsetY;

        public List<DXConfigSection> Sections = [];

        public DXConfigTab()
        {
            PassThrough = false;
        }

        public void AddSection(DXConfigSection newSection)
        {
            Sections.Add(newSection);

            int y = 0;

            foreach (var section in Sections)
            {
                section.Location = new Point(0, y);

                y += section.Size.Height;
            }
        }

        public override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            var changed = e.Delta / SystemInformation.MouseWheelScrollDelta * 10;

            var tempValue = ScrollOffsetY + changed;

            var totalHeight = Sections.Sum(x => x.Size.Height) + 10;

            var currentBottom = -tempValue + totalHeight;

            if (currentBottom <= Size.Height || tempValue < 0)
            {
                return;
            }

            ScrollOffsetY = tempValue;

            int y = -ScrollOffsetY;

            foreach (DXControl control in Controls)
            {
                if (control is not DXConfigSection) continue;

                control.Location = new Point(0, y);
                y += control.Size.Height;
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (var section in Sections)
                {
                    if (section != null)
                    {
                        if (!section.IsDisposed)
                            section.Dispose();
                    }
                }
                Sections.Clear();
            }
        }

        #endregion
    }

    public sealed class DXConfigSection : DXControl
    {
        public DXLabel TitleLabel;
        public int Columns;

        public DXImageControl HeaderImage, FooterImage;
        public List<DXImageControl> BodyImages = new();

        public List<ConfigControl> ConfigControls = new();

        public class ConfigControl
        {
            public DXLabel Label { get; set; }
            public DXControl Control { get; set; }
        }

        public DXConfigSection(string title)
        {
            Size = new Size(348, 30);
            PassThrough = true;

            HeaderImage = new DXImageControl
            {
                Index = 4750,
                LibraryFile = LibraryFile.GameInter,
                Parent = this,
                IsControl = false,
                PassThrough = true,
                Location = new Point(0, 0)
            };

            for (int i = 0; i < 50; i++)
            {
                BodyImages.Add(new DXImageControl
                {
                    Index = 4751,
                    LibraryFile = LibraryFile.GameInter,
                    Parent = this,
                    IsControl = false,
                    PassThrough = true,
                    Visible = false
                });
            }

            FooterImage = new DXImageControl
            {
                Index = 4752,
                LibraryFile = LibraryFile.GameInter,
                Parent = this,
                IsControl = false,
                PassThrough = true,
                Location = new Point(0, HeaderImage.Size.Height)
            };

            TitleLabel = new DXLabel
            {
                Text = title,
                Parent = this,
                Size = new Size(Size.Width, 20),
                PassThrough = true,
                AutoSize = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
        }

        public void AddControl(string label, DXControl control)
        {
            control.Parent = this;

            if (control is DXCheckBox cb)
            {
                cb.Label.ForeColour = Color.FromArgb(169, 124, 67);
                cb.Label.Outline = true;
            }

            ConfigControls.Add(new ConfigControl
            {
                Label = new DXLabel
                {
                    Text = label,
                    ForeColour = Color.FromArgb(169, 124, 67),
                    Outline = true,
                    Parent = this,
                },
                Control = control
            });

            UpdateControlLocations();
        }

        public void UpdateControlLocations()
        {
            const int sectionWidth = 348;
            const int headerHeight = 25;
            const int controlHeight = 20;
            const int footerHeight = 5;

            LayoutImages();

            int y = headerHeight;

            if (Columns == 1)
            {
                LayoutSingleColumnControls(ref y, controlHeight);
            }
            else if (Columns == 2)
            {
                LayoutTwoColumnControls(ref y, controlHeight);
            }

            Size = new Size(sectionWidth, y + footerHeight);
        }

        private void LayoutImages()
        {
            var imageY = HeaderImage.Size.Height;

            int rowCount = (int)Math.Ceiling(ConfigControls.Count / (double)Columns);
            int bodyCount = rowCount * 5;

            HeaderImage.Location = new Point(0, 0);

            for (int i = 0; i < bodyCount; i++)
            {
                if (BodyImages.Count < bodyCount)
                    break;

                BodyImages[i].Visible = true;
                BodyImages[i].Location = new Point(0, imageY);

                imageY += BodyImages[i].Size.Height;
            }

            FooterImage.Location = new Point(0, imageY);
        }

        private void LayoutSingleColumnControls(ref int y, int controlHeight)
        {
            foreach (var control in ConfigControls)
            {
                GetSingleColumnAlignment(control.Control, out int labelAlignX, out int controlAlignX);

                control.Label.Location = new Point(DisplayArea.Right - labelAlignX - control.Label.Size.Width, y);

                control.Control.Location = new Point(DisplayArea.Right - controlAlignX - control.Control.Size.Width, y);

                y += controlHeight;
            }
        }

        private void LayoutTwoColumnControls(ref int y, int controlHeight)
        {
            int labelAlignX = 175;
            int controlAlignX = 25;

            int rowItems = 0;
            const int colOffset = 175;

            foreach (var control in ConfigControls)
            {
                if (control.Control is DXColourControlPair)
                {
                    labelAlignX = 70;
                }

                bool isLeftColumn = rowItems == 0;
                int columnOffset = isLeftColumn ? colOffset : 0;

                control.Label.Location = new Point(DisplayArea.Right - labelAlignX - columnOffset - control.Label.Size.Width, y);

                control.Control.Location = new Point(DisplayArea.Right - controlAlignX - columnOffset - control.Control.Size.Width, y);

                rowItems++;

                if (rowItems == 2)
                {
                    y += controlHeight;
                    rowItems = 0;
                }
            }

            // Handle last partial row
            if (rowItems > 0)
            {
                y += controlHeight;
            }
        }

        private void GetSingleColumnAlignment(DXControl control, out int labelAlignX, out int controlAlignX)
        {
            // Default alignment
            labelAlignX = 230;
            controlAlignX = 100;

            if (control is DXSoundBar)
            {
                labelAlignX = 250;
                controlAlignX = 70;
            }
            else if (control is DXCheckBox)
            {
                labelAlignX = 280;
                controlAlignX = 100;
            }
            else if (control is DXButton)
            {
                labelAlignX = 0;
                controlAlignX = (Size.Width - control.Size.Width) / 2;
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (HeaderImage != null)
                {
                    if (!HeaderImage.IsDisposed)
                        HeaderImage.Dispose();

                    HeaderImage = null;
                }

                foreach (var bodyImage in BodyImages)
                {
                    if (bodyImage != null)
                    {
                        if (!bodyImage.IsDisposed)
                            bodyImage.Dispose();
                    }
                }
                BodyImages.Clear();

                if (FooterImage != null)
                {
                    if (!FooterImage.IsDisposed)
                        FooterImage.Dispose();

                    FooterImage = null;
                }

                foreach (var control in ConfigControls)
                {
                    if (control.Label != null)
                    {
                        if (!control.Label.IsDisposed)
                            control.Label.Dispose();

                        control.Label = null;
                    }

                    if (control.Control != null)
                    {
                        if (!control.Control.IsDisposed)
                            control.Control.Dispose();

                        control.Control = null;
                    }
                }

                ConfigControls.Clear();
            }
        }

        #endregion
    }
}
