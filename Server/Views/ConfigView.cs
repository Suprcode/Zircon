using System;
using System.ComponentModel;
using System.IO.Compression;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Models;
using S = Library.Network.ServerPackets;

namespace Server.Views
{
    public partial class ConfigView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ConfigView()
        {
            InitializeComponent();
            this.SyncronizeRemoteButton.Click += SyncronizeRemoteButton_Click;
            this.SyncronizeLocalButton.Click += SyncronizeLocalButton_Click;
            this.DatabaseEncryptionButton.Click += DatabaseEncryptionButton_Click;
            MysteryShipRegionIndexEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            LairRegionIndexEdit.Properties.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
        }

        private void DatabaseEncryptionButton_Click(object sender, EventArgs e)
        {
            var form = new DatabaseEncryptionForm();
            form.ShowDialog();
        }

        private void SyncronizeRemoteButton_Click(object sender, EventArgs e)
        {
            var form = new SyncForm();
            form.ShowDialog();
        }

        private void SyncronizeLocalButton_Click(object sender, EventArgs e)
        {
            SEnvir.Log($"Starting local syncronization...");

            SMain.Session.Save(true);

            File.Copy(SMain.Session.SystemPath, Path.Combine(Config.ClientPath, "Data\\", Path.GetFileName(SMain.Session.SystemPath)), true);

            SEnvir.Log($"Syncronization completed...");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadSettings();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            SaveSettings();
        }

        public void LoadSettings()
        {
            //Network
            IPAddressEdit.EditValue = Config.IPAddress;
            PortEdit.EditValue = Config.Port;
            TimeOutEdit.EditValue = Config.TimeOut;
            PingDelayEdit.EditValue = Config.PingDelay;
            UserCountPortEdit.EditValue = Config.UserCountPort;
            MaxPacketEdit.EditValue = Config.MaxPacket;
            PacketBanTimeEdit.EditValue = Config.PacketBanTime;


            //Control
            AllowNewAccountEdit.EditValue = Config.AllowNewAccount;
            AllowChangePasswordEdit.EditValue = Config.AllowChangePassword;
            AllowLoginEdit.EditValue = Config.AllowLogin;
            AllowNewCharacterEdit.EditValue = Config.AllowNewCharacter;
            AllowDeleteCharacterEdit.EditValue = Config.AllowDeleteCharacter;
            AllowStartGameEdit.EditValue = Config.AllowStartGame;
            AllowWarriorEdit.EditValue = Config.AllowWarrior;
            AllowWizardEdit.EditValue = Config.AllowWizard;
            AllowTaoistEdit.EditValue = Config.AllowTaoist;
            AllowAssassinEdit.EditValue = Config.AllowAssassin;
            RelogDelayEdit.EditValue = Config.RelogDelay;
            AllowRequestPasswordResetEdit.EditValue = Config.AllowRequestPasswordReset;
            AllowWebResetPasswordEdit.EditValue = Config.AllowWebResetPassword;
            AllowManualResetPasswordEdit.EditValue = Config.AllowManualResetPassword;
            AllowDeleteAccountEdit.EditValue = Config.AllowDeleteAccount;
            AllowManualActivationEdit.EditValue = Config.AllowManualActivation;
            AllowWebActivationEdit.EditValue = Config.AllowWebActivation;
            AllowRequestActivationEdit.EditValue = Config.AllowRequestActivation;

            //System
            CheckVersionEdit.EditValue = Config.CheckVersion;
            VersionPathEdit.EditValue = Config.VersionPath;
            DBSaveDelayEdit.EditValue = Config.DBSaveDelay;
            MapPathEdit.EditValue = Config.MapPath;
            MasterPasswordEdit.EditValue = Config.MasterPassword;
            ClientPathEdit.EditValue = Config.ClientPath;
            ReleaseDateEdit.EditValue = Config.ReleaseDate;
            RabbitEventEndEdit.EditValue = Config.EasterEventEnd;

            //Mail
            MailServerEdit.EditValue = Config.MailServer;
            MailPortEdit.EditValue = Config.MailPort;
            MailUseSSLEdit.EditValue = Config.MailUseSSL;
            MailAccountEdit.EditValue = Config.MailAccount;
            MailPasswordEdit.EditValue = Config.MailPassword;
            MailFromEdit.EditValue = Config.MailFrom;
            MailDisplayNameEdit.EditValue = Config.MailDisplayName;

            //WebServer
            WebPrefixEdit.EditValue = Config.WebPrefix;
            WebCommandLinkEdit.EditValue = Config.WebCommandLink;
            ActivationSuccessLinkEdit.EditValue = Config.ActivationSuccessLink;
            ActivationFailLinkEdit.EditValue = Config.ActivationFailLink;
            ResetSuccessLinkEdit.EditValue = Config.ResetSuccessLink;
            ResetFailLinkEdit.EditValue = Config.ResetFailLink;
            DeleteSuccessLinkEdit.EditValue = Config.DeleteSuccessLink;
            DeleteFailLinkEdit.EditValue = Config.DeleteFailLink;

            BuyPrefixEdit.EditValue = Config.BuyPrefix;
            BuyAddressEdit.EditValue = Config.BuyAddress;
            IPNPrefixEdit.EditValue = Config.IPNPrefix;
            ReceiverEMailEdit.EditValue = Config.ReceiverEMail;
            ProcessGameGoldEdit.EditValue = Config.ProcessGameGold;
            AllowBuyGameGoldEdit.EditValue = Config.AllowBuyGameGold;


            //Players
            MaxViewRangeEdit.EditValue = Config.MaxViewRange;
            ShoutDelayEdit.EditValue = Config.ShoutDelay;
            GlobalDelayEdit.EditValue = Config.GlobalDelay;
            MaxLevelEdit.EditValue = Config.MaxLevel;
            DayCycleCountEdit.EditValue = Config.DayCycleCount;
            SkillExpEdit.EditValue = Config.SkillExp;
            AllowObservationEdit.EditValue = Config.AllowObservation;
            BrownDurationEdit.EditValue = Config.BrownDuration;
            PKPointRateEdit.EditValue = Config.PKPointRate;
            PKPointTickRateEdit.EditValue = Config.PKPointTickRate;
            RedPointEdit.EditValue = Config.RedPoint;
            PvPCurseDurationEdit.EditValue = Config.PvPCurseDuration;
            PvPCurseRateEdit.EditValue = Config.PvPCurseRate;
            AutoReviveDelayEdit.EditValue = Config.AutoReviveDelay;
            EnableStruckEdit.EditValue = Config.EnableStruck;
            EnableHermitEdit.EditValue = Config.EnableHermit;
            EnableFortuneEdit.EditValue = Config.EnableFortune;

            //Monsters
            DeadDurationEdit.EditValue = Config.DeadDuration;
            HarvestDurationEdit.EditValue = Config.HarvestDuration;
            MysteryShipRegionIndexEdit.EditValue = Config.MysteryShipRegionIndex;
            LairRegionIndexEdit.EditValue = Config.LairRegionIndex;

            //Items
            DropDurationEdit.EditValue = Config.DropDuration;
            DropDistanceEdit.EditValue = Config.DropDistance;
            DropLayersEdit.EditValue = Config.DropLayers;
            TorchRateEdit.EditValue = Config.TorchRate;
            SpecialRepairDelayEdit.EditValue = Config.SpecialRepairDelay;
            MaxLuckEdit.EditValue = Config.MaxLuck;
            LuckRateEdit.EditValue = Config.LuckRate;
            MaxCurseEdit.EditValue = Config.MaxCurse;
            CurseRateEdit.EditValue = Config.CurseRate;
            MaxStrengthEdit.EditValue = Config.MaxStrength;
            StrengthAddRateEdit.EditValue = Config.StrengthAddRate;
            StrengthLossRateEdit.EditValue = Config.StrengthLossRate;

            //Rates
            ExperienceRateEdit.EditValue = Config.ExperienceRate;
            DropRateEdit.EditValue = Config.DropRate;
            GoldRateEdit.EditValue = Config.GoldRate;
            SkillRateEdit.EditValue = Config.SkillRate;
            CompanionRateEdit.EditValue = Config.CompanionRate;
        }
        public void SaveSettings()
        {
            //Network
            Config.IPAddress = (string)IPAddressEdit.EditValue;
            Config.Port = (ushort)PortEdit.EditValue;
            Config.TimeOut = (TimeSpan)TimeOutEdit.EditValue;
            Config.PingDelay = (TimeSpan)PingDelayEdit.EditValue;
            Config.UserCountPort = (ushort)UserCountPortEdit.EditValue;
            Config.MaxPacket = (int)MaxPacketEdit.EditValue;
            Config.PacketBanTime = (TimeSpan)PacketBanTimeEdit.EditValue;


            //Control
            Config.AllowNewAccount = (bool)AllowNewAccountEdit.EditValue;
            Config.AllowChangePassword = (bool)AllowChangePasswordEdit.EditValue;
            Config.AllowLogin = (bool)AllowLoginEdit.EditValue;
            Config.AllowNewCharacter = (bool)AllowNewCharacterEdit.EditValue;
            Config.AllowDeleteCharacter = (bool)AllowDeleteCharacterEdit.EditValue;
            Config.AllowStartGame = (bool)AllowStartGameEdit.EditValue;
            Config.AllowWarrior = (bool)AllowWarriorEdit.EditValue;
            Config.AllowWizard = (bool)AllowWizardEdit.EditValue;
            Config.AllowTaoist = (bool)AllowTaoistEdit.EditValue;
            Config.AllowAssassin = (bool)AllowAssassinEdit.EditValue;
            Config.RelogDelay = (TimeSpan)RelogDelayEdit.EditValue;
            Config.AllowRequestPasswordReset = (bool)AllowRequestPasswordResetEdit.EditValue;
            Config.AllowWebResetPassword = (bool)AllowWebResetPasswordEdit.EditValue;
            Config.AllowManualResetPassword = (bool)AllowManualResetPasswordEdit.EditValue;
            Config.AllowDeleteAccount = (bool)AllowDeleteAccountEdit.EditValue;
            Config.AllowManualActivation = (bool)AllowManualActivationEdit.EditValue;
            Config.AllowWebActivation = (bool)AllowWebActivationEdit.EditValue;
            Config.AllowRequestActivation = (bool)AllowRequestActivationEdit.EditValue;

            //System
            Config.CheckVersion = (bool)CheckVersionEdit.EditValue;
            Config.VersionPath = (string)VersionPathEdit.EditValue;
            Config.DBSaveDelay = (TimeSpan)DBSaveDelayEdit.EditValue;
            Config.MapPath = (string)MapPathEdit.EditValue;
            Config.MasterPassword = (string)MasterPasswordEdit.EditValue;
            Config.ClientPath = (string)ClientPathEdit.EditValue;
            Config.ReleaseDate = (DateTime)ReleaseDateEdit.EditValue;
            Config.EasterEventEnd = (DateTime)RabbitEventEndEdit.EditValue;

            //Mail
            Config.MailServer = (string)MailServerEdit.EditValue;
            Config.MailPort = (int)MailPortEdit.EditValue;
            Config.MailUseSSL = (bool)MailUseSSLEdit.EditValue;
            Config.MailAccount = (string)MailAccountEdit.EditValue;
            Config.MailPassword = (string)MailPasswordEdit.EditValue;
            Config.MailFrom = (string)MailFromEdit.EditValue;
            Config.MailDisplayName = (string)MailDisplayNameEdit.EditValue;

            //WebServer
            Config.WebPrefix = (string)WebPrefixEdit.EditValue;
            Config.WebCommandLink = (string)WebCommandLinkEdit.EditValue;
            Config.ActivationSuccessLink = (string)ActivationSuccessLinkEdit.EditValue;
            Config.ActivationFailLink = (string)ActivationFailLinkEdit.EditValue;
            Config.ResetSuccessLink = (string)ResetSuccessLinkEdit.EditValue;
            Config.ResetFailLink = (string)ResetFailLinkEdit.EditValue;
            Config.DeleteSuccessLink = (string)DeleteSuccessLinkEdit.EditValue;
            Config.DeleteFailLink = (string)DeleteFailLinkEdit.EditValue;

            Config.BuyPrefix = (string)BuyPrefixEdit.EditValue;
            Config.BuyAddress = (string)BuyAddressEdit.EditValue;
            Config.IPNPrefix = (string)IPNPrefixEdit.EditValue;
            Config.ReceiverEMail = (string)ReceiverEMailEdit.EditValue;
            Config.ProcessGameGold = (bool)ProcessGameGoldEdit.EditValue;
            Config.AllowBuyGameGold = (bool)AllowBuyGameGoldEdit.EditValue;

            //Players
            Config.MaxViewRange = (int)MaxViewRangeEdit.EditValue;
            Config.ShoutDelay = (TimeSpan)ShoutDelayEdit.EditValue;
            Config.GlobalDelay = (TimeSpan)GlobalDelayEdit.EditValue;
            Config.MaxLevel = (int)MaxLevelEdit.EditValue;
            Config.DayCycleCount = (int)DayCycleCountEdit.EditValue;
            Config.SkillExp = (int)SkillExpEdit.EditValue;
            Config.AllowObservation = (bool)AllowObservationEdit.EditValue;
            Config.BrownDuration = (TimeSpan)BrownDurationEdit.EditValue;
            Config.PKPointRate = (int)PKPointRateEdit.EditValue;
            Config.PKPointTickRate = (TimeSpan)PKPointTickRateEdit.EditValue;
            Config.RedPoint = (int)RedPointEdit.EditValue;
            Config.PvPCurseDuration = (TimeSpan)PvPCurseDurationEdit.EditValue;
            Config.PvPCurseRate = (int)PvPCurseRateEdit.EditValue;
            Config.AutoReviveDelay = (TimeSpan)AutoReviveDelayEdit.EditValue;
            Config.EnableStruck = (bool)EnableStruckEdit.EditValue;
            Config.EnableHermit = (bool)EnableHermitEdit.EditValue;
            Config.EnableFortune = (bool)EnableFortuneEdit.EditValue;

            //Monsters
            Config.DeadDuration = (TimeSpan)DeadDurationEdit.EditValue;
            Config.HarvestDuration = (TimeSpan)HarvestDurationEdit.EditValue;
            Config.MysteryShipRegionIndex = (int)MysteryShipRegionIndexEdit.EditValue;
            Config.LairRegionIndex = (int)LairRegionIndexEdit.EditValue;

            //Items
            Config.DropDuration = (TimeSpan)DropDurationEdit.EditValue;
            Config.DropDistance = (int)DropDistanceEdit.EditValue;
            Config.DropLayers = (int)DropLayersEdit.EditValue;
            Config.TorchRate = (int)TorchRateEdit.EditValue;
            Config.SpecialRepairDelay = (TimeSpan)SpecialRepairDelayEdit.EditValue;
            Config.MaxLuck = (int)MaxLuckEdit.EditValue;
            Config.LuckRate = (int)LuckRateEdit.EditValue;
            Config.MaxCurse = (int)MaxCurseEdit.EditValue;
            Config.CurseRate = (int)CurseRateEdit.EditValue;

            Config.MaxStrength = (int)MaxStrengthEdit.EditValue;
            Config.StrengthAddRate = (int)StrengthAddRateEdit.EditValue;
            Config.StrengthLossRate = (int)StrengthLossRateEdit.EditValue;

            //Rates
            Config.ExperienceRate = (int)ExperienceRateEdit.EditValue;
            Config.DropRate = (int)DropRateEdit.EditValue;
            Config.GoldRate = (int)GoldRateEdit.EditValue;
            Config.SkillRate = (int)SkillRateEdit.EditValue;
            Config.CompanionRate = (int)CompanionRateEdit.EditValue;

            if (SEnvir.Started)
            {
                SEnvir.ServerBuffChanged = true;
            }

            ConfigReader.Save(typeof(Config).Assembly);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveSettings();
        }
        private void ReloadButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadSettings();
        }


        private void CheckVersionButton_Click(object sender, EventArgs e)
        {
            byte[] old = Config.ClientHash;

            Config.LoadVersion();

            if (Functions.IsMatch(old, Config.ClientHash) || !SEnvir.Started) return;

            SEnvir.Broadcast(new S.Chat { Text = "A new version has been made available, please update when possible.", Type = MessageType.Announcement });
        }
        private void VersionPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (OpenDialog.ShowDialog() != DialogResult.OK) return;

            VersionPathEdit.EditValue = OpenDialog.FileName;
        }
        private void MapPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (FolderDialog.ShowDialog() != DialogResult.OK) return;

            MapPathEdit.EditValue = FolderDialog.SelectedPath;
        }

        private void ClientPathEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (FolderDialog.ShowDialog() != DialogResult.OK) return;

            ClientPathEdit.EditValue = FolderDialog.SelectedPath;
        }
    }
}