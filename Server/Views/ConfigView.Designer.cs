using Microsoft.Win32;

namespace Server.Views
{
    partial class ConfigView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigView));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            ReloadButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            PacketBanTimeEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            SyncronizeButton = new DevExpress.XtraEditors.SimpleButton();
            DatabaseEncryptionButton = new DevExpress.XtraEditors.SimpleButton();
            labelControl86 = new DevExpress.XtraEditors.LabelControl();
            labelControl87 = new DevExpress.XtraEditors.LabelControl();
            MaxPacketEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl51 = new DevExpress.XtraEditors.LabelControl();
            UserCountPortEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl6 = new DevExpress.XtraEditors.LabelControl();
            PingDelayEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            TimeOutEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            PortEdit = new DevExpress.XtraEditors.TextEdit();
            IPAddressEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            labelControl16 = new DevExpress.XtraEditors.LabelControl();
            AllowRequestActivationEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl22 = new DevExpress.XtraEditors.LabelControl();
            AllowWebActivationEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl17 = new DevExpress.XtraEditors.LabelControl();
            AllowManualActivationEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl18 = new DevExpress.XtraEditors.LabelControl();
            AllowDeleteAccountEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl19 = new DevExpress.XtraEditors.LabelControl();
            AllowManualResetPasswordEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl20 = new DevExpress.XtraEditors.LabelControl();
            AllowWebResetPasswordEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl21 = new DevExpress.XtraEditors.LabelControl();
            AllowRequestPasswordResetEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl40 = new DevExpress.XtraEditors.LabelControl();
            AllowWizardEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl39 = new DevExpress.XtraEditors.LabelControl();
            AllowTaoistEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl38 = new DevExpress.XtraEditors.LabelControl();
            AllowAssassinEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl36 = new DevExpress.XtraEditors.LabelControl();
            AllowWarriorEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl15 = new DevExpress.XtraEditors.LabelControl();
            RelogDelayEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            labelControl14 = new DevExpress.XtraEditors.LabelControl();
            AllowStartGameEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl12 = new DevExpress.XtraEditors.LabelControl();
            AllowDeleteCharacterEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl11 = new DevExpress.XtraEditors.LabelControl();
            AllowNewCharacterEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl9 = new DevExpress.XtraEditors.LabelControl();
            AllowLoginEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl8 = new DevExpress.XtraEditors.LabelControl();
            AllowChangePasswordEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl7 = new DevExpress.XtraEditors.LabelControl();
            AllowNewAccountEdit = new DevExpress.XtraEditors.CheckEdit();
            xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
            RabbitEventEndEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl85 = new DevExpress.XtraEditors.LabelControl();
            ReleaseDateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl70 = new DevExpress.XtraEditors.LabelControl();
            ClientPathEdit = new DevExpress.XtraEditors.ButtonEdit();
            labelControl96 = new DevExpress.XtraEditors.LabelControl();
            MasterPasswordEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl67 = new DevExpress.XtraEditors.LabelControl();
            MapPathEdit = new DevExpress.XtraEditors.ButtonEdit();
            labelControl13 = new DevExpress.XtraEditors.LabelControl();
            labelControl10 = new DevExpress.XtraEditors.LabelControl();
            DBSaveDelayEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            CheckVersionButton = new DevExpress.XtraEditors.SimpleButton();
            VersionPathEdit = new DevExpress.XtraEditors.ButtonEdit();
            labelControl5 = new DevExpress.XtraEditors.LabelControl();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            CheckVersionEdit = new DevExpress.XtraEditors.CheckEdit();
            xtraTabPage4 = new DevExpress.XtraTab.XtraTabPage();
            MailDisplayNameEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl31 = new DevExpress.XtraEditors.LabelControl();
            MailFromEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl30 = new DevExpress.XtraEditors.LabelControl();
            MailPasswordEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl29 = new DevExpress.XtraEditors.LabelControl();
            MailAccountEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl28 = new DevExpress.XtraEditors.LabelControl();
            labelControl27 = new DevExpress.XtraEditors.LabelControl();
            MailUseSSLEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl25 = new DevExpress.XtraEditors.LabelControl();
            MailPortEdit = new DevExpress.XtraEditors.TextEdit();
            MailServerEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl26 = new DevExpress.XtraEditors.LabelControl();
            xtraTabPage5 = new DevExpress.XtraTab.XtraTabPage();
            labelControl81 = new DevExpress.XtraEditors.LabelControl();
            AllowBuyGameGoldEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl80 = new DevExpress.XtraEditors.LabelControl();
            ProcessGameGoldEdit = new DevExpress.XtraEditors.CheckEdit();
            ReceiverEMailEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl79 = new DevExpress.XtraEditors.LabelControl();
            IPNPrefixEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl73 = new DevExpress.XtraEditors.LabelControl();
            BuyAddressEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl72 = new DevExpress.XtraEditors.LabelControl();
            BuyPrefixEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl71 = new DevExpress.XtraEditors.LabelControl();
            DeleteFailLinkEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl37 = new DevExpress.XtraEditors.LabelControl();
            DeleteSuccessLinkEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl43 = new DevExpress.XtraEditors.LabelControl();
            ResetFailLinkEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl32 = new DevExpress.XtraEditors.LabelControl();
            ResetSuccessLinkEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl33 = new DevExpress.XtraEditors.LabelControl();
            ActivationFailLinkEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl34 = new DevExpress.XtraEditors.LabelControl();
            ActivationSuccessLinkEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl35 = new DevExpress.XtraEditors.LabelControl();
            labelControl41 = new DevExpress.XtraEditors.LabelControl();
            WebCommandLinkEdit = new DevExpress.XtraEditors.TextEdit();
            WebPrefixEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl42 = new DevExpress.XtraEditors.LabelControl();
            xtraTabPage6 = new DevExpress.XtraTab.XtraTabPage();
            labelControl90 = new DevExpress.XtraEditors.LabelControl();
            EnableHermitEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl88 = new DevExpress.XtraEditors.LabelControl();
            EnableStruckEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl69 = new DevExpress.XtraEditors.LabelControl();
            AutoReviveDelayEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            PvPCurseRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl83 = new DevExpress.XtraEditors.LabelControl();
            labelControl84 = new DevExpress.XtraEditors.LabelControl();
            PvPCurseDurationEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            RedPointEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl77 = new DevExpress.XtraEditors.LabelControl();
            labelControl78 = new DevExpress.XtraEditors.LabelControl();
            PKPointTickRateEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            PKPointRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl76 = new DevExpress.XtraEditors.LabelControl();
            labelControl75 = new DevExpress.XtraEditors.LabelControl();
            BrownDurationEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            labelControl24 = new DevExpress.XtraEditors.LabelControl();
            AllowObservationEdit = new DevExpress.XtraEditors.CheckEdit();
            SkillExpEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl53 = new DevExpress.XtraEditors.LabelControl();
            DayCycleCountEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl52 = new DevExpress.XtraEditors.LabelControl();
            MaxLevelEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl46 = new DevExpress.XtraEditors.LabelControl();
            labelControl45 = new DevExpress.XtraEditors.LabelControl();
            GlobalDelayEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            labelControl44 = new DevExpress.XtraEditors.LabelControl();
            ShoutDelayEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            MaxViewRangeEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl23 = new DevExpress.XtraEditors.LabelControl();
            xtraTabPage7 = new DevExpress.XtraTab.XtraTabPage();
            LairRegionIndexEdit = new DevExpress.XtraEditors.LookUpEdit();
            labelControl82 = new DevExpress.XtraEditors.LabelControl();
            MysteryShipRegionIndexEdit = new DevExpress.XtraEditors.LookUpEdit();
            labelControl89 = new DevExpress.XtraEditors.LabelControl();
            labelControl74 = new DevExpress.XtraEditors.LabelControl();
            HarvestDurationEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            labelControl47 = new DevExpress.XtraEditors.LabelControl();
            DeadDurationEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            xtraTabPage8 = new DevExpress.XtraTab.XtraTabPage();
            StrengthLossRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl64 = new DevExpress.XtraEditors.LabelControl();
            StrengthAddRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl65 = new DevExpress.XtraEditors.LabelControl();
            MaxStrengthEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl66 = new DevExpress.XtraEditors.LabelControl();
            CurseRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl63 = new DevExpress.XtraEditors.LabelControl();
            MaxCurseEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl62 = new DevExpress.XtraEditors.LabelControl();
            LuckRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl61 = new DevExpress.XtraEditors.LabelControl();
            MaxLuckEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl60 = new DevExpress.XtraEditors.LabelControl();
            labelControl59 = new DevExpress.XtraEditors.LabelControl();
            SpecialRepairDelayEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            TorchRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl54 = new DevExpress.XtraEditors.LabelControl();
            DropLayersEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl50 = new DevExpress.XtraEditors.LabelControl();
            DropDistanceEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl49 = new DevExpress.XtraEditors.LabelControl();
            labelControl48 = new DevExpress.XtraEditors.LabelControl();
            DropDurationEdit = new DevExpress.XtraEditors.TimeSpanEdit();
            xtraTabPage9 = new DevExpress.XtraTab.XtraTabPage();
            CompanionRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl68 = new DevExpress.XtraEditors.LabelControl();
            SkillRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl58 = new DevExpress.XtraEditors.LabelControl();
            GoldRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl57 = new DevExpress.XtraEditors.LabelControl();
            DropRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl56 = new DevExpress.XtraEditors.LabelControl();
            ExperienceRateEdit = new DevExpress.XtraEditors.TextEdit();
            labelControl55 = new DevExpress.XtraEditors.LabelControl();
            OpenDialog = new DevExpress.XtraEditors.XtraOpenFileDialog(components);
            FolderDialog = new DevExpress.XtraEditors.XtraFolderBrowserDialog(components);
            EnableFortuneEdit = new DevExpress.XtraEditors.CheckEdit();
            labelControl91 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).BeginInit();
            xtraTabControl1.SuspendLayout();
            xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PacketBanTimeEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxPacketEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)UserCountPortEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PingDelayEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TimeOutEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PortEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)IPAddressEdit.Properties).BeginInit();
            xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AllowRequestActivationEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowWebActivationEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowManualActivationEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowDeleteAccountEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowManualResetPasswordEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowWebResetPasswordEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowRequestPasswordResetEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowWizardEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowTaoistEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowAssassinEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowWarriorEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RelogDelayEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowStartGameEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowDeleteCharacterEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowNewCharacterEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowLoginEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowChangePasswordEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowNewAccountEdit.Properties).BeginInit();
            xtraTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)RabbitEventEndEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ReleaseDateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ClientPathEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MasterPasswordEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapPathEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DBSaveDelayEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)VersionPathEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CheckVersionEdit.Properties).BeginInit();
            xtraTabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MailDisplayNameEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MailFromEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MailPasswordEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MailAccountEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MailUseSSLEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MailPortEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MailServerEdit.Properties).BeginInit();
            xtraTabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AllowBuyGameGoldEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ProcessGameGoldEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ReceiverEMailEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)IPNPrefixEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BuyAddressEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BuyPrefixEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DeleteFailLinkEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DeleteSuccessLinkEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ResetFailLinkEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ResetSuccessLinkEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ActivationFailLinkEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ActivationSuccessLinkEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WebCommandLinkEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WebPrefixEdit.Properties).BeginInit();
            xtraTabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)EnableHermitEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)EnableStruckEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AutoReviveDelayEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PvPCurseRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PvPCurseDurationEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RedPointEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PKPointTickRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PKPointRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BrownDurationEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AllowObservationEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SkillExpEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DayCycleCountEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxLevelEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)GlobalDelayEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ShoutDelayEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxViewRangeEdit.Properties).BeginInit();
            xtraTabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LairRegionIndexEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MysteryShipRegionIndexEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HarvestDurationEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DeadDurationEdit.Properties).BeginInit();
            xtraTabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)StrengthLossRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StrengthAddRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxStrengthEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CurseRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxCurseEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LuckRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxLuckEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SpecialRepairDelayEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TorchRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DropLayersEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DropDistanceEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DropDurationEdit.Properties).BeginInit();
            xtraTabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CompanionRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SkillRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)GoldRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DropRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ExperienceRateEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)EnableFortuneEdit.Properties).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveButton, ReloadButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(769, 144);
            // 
            // SaveButton
            // 
            SaveButton.Caption = "Save";
            SaveButton.Id = 1;
            SaveButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.Image");
            SaveButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.LargeImage");
            SaveButton.Name = "SaveButton";
            SaveButton.ItemClick += SaveButton_ItemClick;
            // 
            // ReloadButton
            // 
            ReloadButton.Caption = "Reload";
            ReloadButton.Id = 2;
            ReloadButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ReloadButton.ImageOptions.Image");
            ReloadButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ReloadButton.ImageOptions.LargeImage");
            ReloadButton.Name = "ReloadButton";
            ReloadButton.ItemClick += ReloadButton_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(SaveButton);
            ribbonPageGroup1.ItemLinks.Add(ReloadButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Actions";
            // 
            // xtraTabControl1
            // 
            xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            xtraTabControl1.Location = new System.Drawing.Point(0, 144);
            xtraTabControl1.Name = "xtraTabControl1";
            xtraTabControl1.SelectedTabPage = xtraTabPage1;
            xtraTabControl1.Size = new System.Drawing.Size(769, 420);
            xtraTabControl1.TabIndex = 2;
            xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { xtraTabPage1, xtraTabPage2, xtraTabPage3, xtraTabPage4, xtraTabPage5, xtraTabPage6, xtraTabPage7, xtraTabPage8, xtraTabPage9 });
            // 
            // xtraTabPage1
            // 
            xtraTabPage1.Controls.Add(PacketBanTimeEdit);
            xtraTabPage1.Controls.Add(SyncronizeButton);
            xtraTabPage1.Controls.Add(DatabaseEncryptionButton);
            xtraTabPage1.Controls.Add(labelControl86);
            xtraTabPage1.Controls.Add(labelControl87);
            xtraTabPage1.Controls.Add(MaxPacketEdit);
            xtraTabPage1.Controls.Add(labelControl51);
            xtraTabPage1.Controls.Add(UserCountPortEdit);
            xtraTabPage1.Controls.Add(labelControl6);
            xtraTabPage1.Controls.Add(PingDelayEdit);
            xtraTabPage1.Controls.Add(TimeOutEdit);
            xtraTabPage1.Controls.Add(labelControl3);
            xtraTabPage1.Controls.Add(labelControl2);
            xtraTabPage1.Controls.Add(PortEdit);
            xtraTabPage1.Controls.Add(IPAddressEdit);
            xtraTabPage1.Controls.Add(labelControl1);
            xtraTabPage1.Name = "xtraTabPage1";
            xtraTabPage1.Size = new System.Drawing.Size(763, 392);
            xtraTabPage1.Text = "Network";
            // 
            // PacketBanTimeEdit
            // 
            PacketBanTimeEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            PacketBanTimeEdit.Location = new System.Drawing.Point(108, 174);
            PacketBanTimeEdit.MenuManager = ribbon;
            PacketBanTimeEdit.Name = "PacketBanTimeEdit";
            PacketBanTimeEdit.Properties.AllowEditDays = false;
            PacketBanTimeEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            PacketBanTimeEdit.Properties.Mask.EditMask = "HH:mm:ss";
            PacketBanTimeEdit.Size = new System.Drawing.Size(100, 20);
            PacketBanTimeEdit.TabIndex = 40;
            // 
            // SyncronizeButton
            // 
            SyncronizeButton.Location = new System.Drawing.Point(20, 200);
            SyncronizeButton.Name = "SyncronizeButton";
            SyncronizeButton.Size = new System.Drawing.Size(200, 23);
            SyncronizeButton.TabIndex = 41;
            SyncronizeButton.Text = "Syncronize Remote DB";
            // 
            // DatabaseEncryptionButton
            // 
            DatabaseEncryptionButton.Location = new System.Drawing.Point(20, 226);
            DatabaseEncryptionButton.Name = "DatabaseEncryptionButton";
            DatabaseEncryptionButton.Size = new System.Drawing.Size(200, 23);
            DatabaseEncryptionButton.TabIndex = 41;
            DatabaseEncryptionButton.Text = "Config Database Encryption";
            // 
            // labelControl86
            // 
            labelControl86.Location = new System.Drawing.Point(20, 177);
            labelControl86.Name = "labelControl86";
            labelControl86.Size = new System.Drawing.Size(82, 13);
            labelControl86.TabIndex = 39;
            labelControl86.Text = "Packet Ban Time:";
            // 
            // labelControl87
            // 
            labelControl87.Location = new System.Drawing.Point(38, 151);
            labelControl87.Name = "labelControl87";
            labelControl87.Size = new System.Drawing.Size(64, 13);
            labelControl87.TabIndex = 38;
            labelControl87.Text = "Max Packets:";
            // 
            // MaxPacketEdit
            // 
            MaxPacketEdit.Location = new System.Drawing.Point(108, 148);
            MaxPacketEdit.MenuManager = ribbon;
            MaxPacketEdit.Name = "MaxPacketEdit";
            MaxPacketEdit.Properties.Appearance.Options.UseTextOptions = true;
            MaxPacketEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MaxPacketEdit.Properties.Mask.EditMask = "n0";
            MaxPacketEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            MaxPacketEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MaxPacketEdit.Size = new System.Drawing.Size(100, 20);
            MaxPacketEdit.TabIndex = 37;
            // 
            // labelControl51
            // 
            labelControl51.Location = new System.Drawing.Point(21, 125);
            labelControl51.Name = "labelControl51";
            labelControl51.Size = new System.Drawing.Size(81, 13);
            labelControl51.TabIndex = 36;
            labelControl51.Text = "User Count Port:";
            // 
            // UserCountPortEdit
            // 
            UserCountPortEdit.Location = new System.Drawing.Point(108, 122);
            UserCountPortEdit.MenuManager = ribbon;
            UserCountPortEdit.Name = "UserCountPortEdit";
            UserCountPortEdit.Properties.Appearance.Options.UseTextOptions = true;
            UserCountPortEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            UserCountPortEdit.Properties.Mask.EditMask = "n0";
            UserCountPortEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            UserCountPortEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            UserCountPortEdit.Size = new System.Drawing.Size(100, 20);
            UserCountPortEdit.TabIndex = 35;
            // 
            // labelControl6
            // 
            labelControl6.Location = new System.Drawing.Point(48, 99);
            labelControl6.Name = "labelControl6";
            labelControl6.Size = new System.Drawing.Size(54, 13);
            labelControl6.TabIndex = 34;
            labelControl6.Text = "Ping Delay:";
            // 
            // PingDelayEdit
            // 
            PingDelayEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            PingDelayEdit.Location = new System.Drawing.Point(108, 96);
            PingDelayEdit.MenuManager = ribbon;
            PingDelayEdit.Name = "PingDelayEdit";
            PingDelayEdit.Properties.AllowEditDays = false;
            PingDelayEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            PingDelayEdit.Properties.Mask.EditMask = "HH:mm:ss";
            PingDelayEdit.Size = new System.Drawing.Size(100, 20);
            PingDelayEdit.TabIndex = 33;
            // 
            // TimeOutEdit
            // 
            TimeOutEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            TimeOutEdit.Location = new System.Drawing.Point(108, 70);
            TimeOutEdit.MenuManager = ribbon;
            TimeOutEdit.Name = "TimeOutEdit";
            TimeOutEdit.Properties.AllowEditDays = false;
            TimeOutEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            TimeOutEdit.Properties.Mask.EditMask = "HH:mm:ss";
            TimeOutEdit.Size = new System.Drawing.Size(100, 20);
            TimeOutEdit.TabIndex = 32;
            // 
            // labelControl3
            // 
            labelControl3.Location = new System.Drawing.Point(55, 73);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(47, 13);
            labelControl3.TabIndex = 31;
            labelControl3.Text = "Time Out:";
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(78, 47);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(24, 13);
            labelControl2.TabIndex = 30;
            labelControl2.Text = "Port:";
            // 
            // PortEdit
            // 
            PortEdit.Location = new System.Drawing.Point(108, 44);
            PortEdit.MenuManager = ribbon;
            PortEdit.Name = "PortEdit";
            PortEdit.Properties.Appearance.Options.UseTextOptions = true;
            PortEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            PortEdit.Properties.Mask.EditMask = "n0";
            PortEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            PortEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            PortEdit.Size = new System.Drawing.Size(100, 20);
            PortEdit.TabIndex = 29;
            // 
            // IPAddressEdit
            // 
            IPAddressEdit.Location = new System.Drawing.Point(108, 18);
            IPAddressEdit.MenuManager = ribbon;
            IPAddressEdit.Name = "IPAddressEdit";
            IPAddressEdit.Properties.Appearance.Options.UseTextOptions = true;
            IPAddressEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            IPAddressEdit.Size = new System.Drawing.Size(100, 20);
            IPAddressEdit.TabIndex = 27;
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(46, 21);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(56, 13);
            labelControl1.TabIndex = 28;
            labelControl1.Text = "IP Address:";
            // 
            // xtraTabPage2
            // 
            xtraTabPage2.Controls.Add(labelControl16);
            xtraTabPage2.Controls.Add(AllowRequestActivationEdit);
            xtraTabPage2.Controls.Add(labelControl22);
            xtraTabPage2.Controls.Add(AllowWebActivationEdit);
            xtraTabPage2.Controls.Add(labelControl17);
            xtraTabPage2.Controls.Add(AllowManualActivationEdit);
            xtraTabPage2.Controls.Add(labelControl18);
            xtraTabPage2.Controls.Add(AllowDeleteAccountEdit);
            xtraTabPage2.Controls.Add(labelControl19);
            xtraTabPage2.Controls.Add(AllowManualResetPasswordEdit);
            xtraTabPage2.Controls.Add(labelControl20);
            xtraTabPage2.Controls.Add(AllowWebResetPasswordEdit);
            xtraTabPage2.Controls.Add(labelControl21);
            xtraTabPage2.Controls.Add(AllowRequestPasswordResetEdit);
            xtraTabPage2.Controls.Add(labelControl40);
            xtraTabPage2.Controls.Add(AllowWizardEdit);
            xtraTabPage2.Controls.Add(labelControl39);
            xtraTabPage2.Controls.Add(AllowTaoistEdit);
            xtraTabPage2.Controls.Add(labelControl38);
            xtraTabPage2.Controls.Add(AllowAssassinEdit);
            xtraTabPage2.Controls.Add(labelControl36);
            xtraTabPage2.Controls.Add(AllowWarriorEdit);
            xtraTabPage2.Controls.Add(labelControl15);
            xtraTabPage2.Controls.Add(RelogDelayEdit);
            xtraTabPage2.Controls.Add(labelControl14);
            xtraTabPage2.Controls.Add(AllowStartGameEdit);
            xtraTabPage2.Controls.Add(labelControl12);
            xtraTabPage2.Controls.Add(AllowDeleteCharacterEdit);
            xtraTabPage2.Controls.Add(labelControl11);
            xtraTabPage2.Controls.Add(AllowNewCharacterEdit);
            xtraTabPage2.Controls.Add(labelControl9);
            xtraTabPage2.Controls.Add(AllowLoginEdit);
            xtraTabPage2.Controls.Add(labelControl8);
            xtraTabPage2.Controls.Add(AllowChangePasswordEdit);
            xtraTabPage2.Controls.Add(labelControl7);
            xtraTabPage2.Controls.Add(AllowNewAccountEdit);
            xtraTabPage2.Name = "xtraTabPage2";
            xtraTabPage2.Size = new System.Drawing.Size(763, 392);
            xtraTabPage2.Text = "Control";
            // 
            // labelControl16
            // 
            labelControl16.Location = new System.Drawing.Point(403, 167);
            labelControl16.Name = "labelControl16";
            labelControl16.Size = new System.Drawing.Size(117, 13);
            labelControl16.TabIndex = 95;
            labelControl16.Text = "Allow Manual Activation:";
            // 
            // AllowRequestActivationEdit
            // 
            AllowRequestActivationEdit.Location = new System.Drawing.Point(526, 164);
            AllowRequestActivationEdit.MenuManager = ribbon;
            AllowRequestActivationEdit.Name = "AllowRequestActivationEdit";
            AllowRequestActivationEdit.Properties.Caption = "";
            AllowRequestActivationEdit.Size = new System.Drawing.Size(100, 19);
            AllowRequestActivationEdit.TabIndex = 94;
            // 
            // labelControl22
            // 
            labelControl22.Location = new System.Drawing.Point(415, 142);
            labelControl22.Name = "labelControl22";
            labelControl22.Size = new System.Drawing.Size(105, 13);
            labelControl22.TabIndex = 93;
            labelControl22.Text = "Allow Web Activation:";
            // 
            // AllowWebActivationEdit
            // 
            AllowWebActivationEdit.Location = new System.Drawing.Point(526, 139);
            AllowWebActivationEdit.MenuManager = ribbon;
            AllowWebActivationEdit.Name = "AllowWebActivationEdit";
            AllowWebActivationEdit.Properties.Caption = "";
            AllowWebActivationEdit.Size = new System.Drawing.Size(100, 19);
            AllowWebActivationEdit.TabIndex = 92;
            // 
            // labelControl17
            // 
            labelControl17.Location = new System.Drawing.Point(397, 117);
            labelControl17.Name = "labelControl17";
            labelControl17.Size = new System.Drawing.Size(123, 13);
            labelControl17.TabIndex = 91;
            labelControl17.Text = "Allow Request Activation:";
            // 
            // AllowManualActivationEdit
            // 
            AllowManualActivationEdit.Location = new System.Drawing.Point(526, 114);
            AllowManualActivationEdit.MenuManager = ribbon;
            AllowManualActivationEdit.Name = "AllowManualActivationEdit";
            AllowManualActivationEdit.Properties.Caption = "";
            AllowManualActivationEdit.Size = new System.Drawing.Size(100, 19);
            AllowManualActivationEdit.TabIndex = 90;
            // 
            // labelControl18
            // 
            labelControl18.Location = new System.Drawing.Point(415, 92);
            labelControl18.Name = "labelControl18";
            labelControl18.Size = new System.Drawing.Size(105, 13);
            labelControl18.TabIndex = 89;
            labelControl18.Text = "Allow Delete Account:";
            // 
            // AllowDeleteAccountEdit
            // 
            AllowDeleteAccountEdit.Location = new System.Drawing.Point(526, 89);
            AllowDeleteAccountEdit.MenuManager = ribbon;
            AllowDeleteAccountEdit.Name = "AllowDeleteAccountEdit";
            AllowDeleteAccountEdit.Properties.Caption = "";
            AllowDeleteAccountEdit.Size = new System.Drawing.Size(100, 19);
            AllowDeleteAccountEdit.TabIndex = 88;
            // 
            // labelControl19
            // 
            labelControl19.Location = new System.Drawing.Point(423, 67);
            labelControl19.Name = "labelControl19";
            labelControl19.Size = new System.Drawing.Size(97, 13);
            labelControl19.TabIndex = 87;
            labelControl19.Text = "Allow Manual Reset:";
            // 
            // AllowManualResetPasswordEdit
            // 
            AllowManualResetPasswordEdit.Location = new System.Drawing.Point(526, 64);
            AllowManualResetPasswordEdit.MenuManager = ribbon;
            AllowManualResetPasswordEdit.Name = "AllowManualResetPasswordEdit";
            AllowManualResetPasswordEdit.Properties.Caption = "";
            AllowManualResetPasswordEdit.Size = new System.Drawing.Size(100, 19);
            AllowManualResetPasswordEdit.TabIndex = 86;
            // 
            // labelControl20
            // 
            labelControl20.Location = new System.Drawing.Point(435, 42);
            labelControl20.Name = "labelControl20";
            labelControl20.Size = new System.Drawing.Size(85, 13);
            labelControl20.TabIndex = 85;
            labelControl20.Text = "Allow Web Reset:";
            // 
            // AllowWebResetPasswordEdit
            // 
            AllowWebResetPasswordEdit.Location = new System.Drawing.Point(526, 39);
            AllowWebResetPasswordEdit.MenuManager = ribbon;
            AllowWebResetPasswordEdit.Name = "AllowWebResetPasswordEdit";
            AllowWebResetPasswordEdit.Properties.Caption = "";
            AllowWebResetPasswordEdit.Size = new System.Drawing.Size(100, 19);
            AllowWebResetPasswordEdit.TabIndex = 84;
            // 
            // labelControl21
            // 
            labelControl21.Location = new System.Drawing.Point(399, 17);
            labelControl21.Name = "labelControl21";
            labelControl21.Size = new System.Drawing.Size(121, 13);
            labelControl21.TabIndex = 83;
            labelControl21.Text = "Allow Request Password:";
            // 
            // AllowRequestPasswordResetEdit
            // 
            AllowRequestPasswordResetEdit.Location = new System.Drawing.Point(526, 14);
            AllowRequestPasswordResetEdit.MenuManager = ribbon;
            AllowRequestPasswordResetEdit.Name = "AllowRequestPasswordResetEdit";
            AllowRequestPasswordResetEdit.Properties.Caption = "";
            AllowRequestPasswordResetEdit.Size = new System.Drawing.Size(100, 19);
            AllowRequestPasswordResetEdit.TabIndex = 82;
            // 
            // labelControl40
            // 
            labelControl40.Location = new System.Drawing.Point(222, 42);
            labelControl40.Name = "labelControl40";
            labelControl40.Size = new System.Drawing.Size(65, 13);
            labelControl40.TabIndex = 81;
            labelControl40.Text = "Allow Wizard:";
            // 
            // AllowWizardEdit
            // 
            AllowWizardEdit.Location = new System.Drawing.Point(293, 39);
            AllowWizardEdit.MenuManager = ribbon;
            AllowWizardEdit.Name = "AllowWizardEdit";
            AllowWizardEdit.Properties.Caption = "";
            AllowWizardEdit.Size = new System.Drawing.Size(100, 19);
            AllowWizardEdit.TabIndex = 80;
            // 
            // labelControl39
            // 
            labelControl39.Location = new System.Drawing.Point(226, 67);
            labelControl39.Name = "labelControl39";
            labelControl39.Size = new System.Drawing.Size(61, 13);
            labelControl39.TabIndex = 79;
            labelControl39.Text = "Allow Taoist:";
            // 
            // AllowTaoistEdit
            // 
            AllowTaoistEdit.Location = new System.Drawing.Point(293, 64);
            AllowTaoistEdit.MenuManager = ribbon;
            AllowTaoistEdit.Name = "AllowTaoistEdit";
            AllowTaoistEdit.Properties.Caption = "";
            AllowTaoistEdit.Size = new System.Drawing.Size(100, 19);
            AllowTaoistEdit.TabIndex = 78;
            // 
            // labelControl38
            // 
            labelControl38.Location = new System.Drawing.Point(214, 92);
            labelControl38.Name = "labelControl38";
            labelControl38.Size = new System.Drawing.Size(73, 13);
            labelControl38.TabIndex = 77;
            labelControl38.Text = "Allow Assassin:";
            // 
            // AllowAssassinEdit
            // 
            AllowAssassinEdit.Location = new System.Drawing.Point(293, 89);
            AllowAssassinEdit.MenuManager = ribbon;
            AllowAssassinEdit.Name = "AllowAssassinEdit";
            AllowAssassinEdit.Properties.Caption = "";
            AllowAssassinEdit.Size = new System.Drawing.Size(100, 19);
            AllowAssassinEdit.TabIndex = 76;
            // 
            // labelControl36
            // 
            labelControl36.Location = new System.Drawing.Point(219, 17);
            labelControl36.Name = "labelControl36";
            labelControl36.Size = new System.Drawing.Size(68, 13);
            labelControl36.TabIndex = 73;
            labelControl36.Text = "Allow Warrior:";
            // 
            // AllowWarriorEdit
            // 
            AllowWarriorEdit.Location = new System.Drawing.Point(293, 14);
            AllowWarriorEdit.MenuManager = ribbon;
            AllowWarriorEdit.Name = "AllowWarriorEdit";
            AllowWarriorEdit.Properties.Caption = "";
            AllowWarriorEdit.Size = new System.Drawing.Size(100, 19);
            AllowWarriorEdit.TabIndex = 72;
            // 
            // labelControl15
            // 
            labelControl15.Location = new System.Drawing.Point(41, 167);
            labelControl15.Name = "labelControl15";
            labelControl15.Size = new System.Drawing.Size(61, 13);
            labelControl15.TabIndex = 71;
            labelControl15.Text = "Relog Delay:";
            // 
            // RelogDelayEdit
            // 
            RelogDelayEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            RelogDelayEdit.Location = new System.Drawing.Point(108, 164);
            RelogDelayEdit.MenuManager = ribbon;
            RelogDelayEdit.Name = "RelogDelayEdit";
            RelogDelayEdit.Properties.AllowEditDays = false;
            RelogDelayEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RelogDelayEdit.Properties.Mask.EditMask = "HH:mm:ss";
            RelogDelayEdit.Size = new System.Drawing.Size(100, 20);
            RelogDelayEdit.TabIndex = 70;
            // 
            // labelControl14
            // 
            labelControl14.Location = new System.Drawing.Point(44, 142);
            labelControl14.Name = "labelControl14";
            labelControl14.Size = new System.Drawing.Size(58, 13);
            labelControl14.TabIndex = 69;
            labelControl14.Text = "Start Game:";
            // 
            // AllowStartGameEdit
            // 
            AllowStartGameEdit.Location = new System.Drawing.Point(108, 139);
            AllowStartGameEdit.MenuManager = ribbon;
            AllowStartGameEdit.Name = "AllowStartGameEdit";
            AllowStartGameEdit.Properties.Caption = "";
            AllowStartGameEdit.Size = new System.Drawing.Size(100, 19);
            AllowStartGameEdit.TabIndex = 68;
            // 
            // labelControl12
            // 
            labelControl12.Location = new System.Drawing.Point(16, 117);
            labelControl12.Name = "labelControl12";
            labelControl12.Size = new System.Drawing.Size(86, 13);
            labelControl12.TabIndex = 67;
            labelControl12.Text = "Delete Character:";
            // 
            // AllowDeleteCharacterEdit
            // 
            AllowDeleteCharacterEdit.Location = new System.Drawing.Point(108, 114);
            AllowDeleteCharacterEdit.MenuManager = ribbon;
            AllowDeleteCharacterEdit.Name = "AllowDeleteCharacterEdit";
            AllowDeleteCharacterEdit.Properties.Caption = "";
            AllowDeleteCharacterEdit.Size = new System.Drawing.Size(100, 19);
            AllowDeleteCharacterEdit.TabIndex = 66;
            // 
            // labelControl11
            // 
            labelControl11.Location = new System.Drawing.Point(26, 92);
            labelControl11.Name = "labelControl11";
            labelControl11.Size = new System.Drawing.Size(76, 13);
            labelControl11.TabIndex = 65;
            labelControl11.Text = "New Character:";
            // 
            // AllowNewCharacterEdit
            // 
            AllowNewCharacterEdit.Location = new System.Drawing.Point(108, 89);
            AllowNewCharacterEdit.MenuManager = ribbon;
            AllowNewCharacterEdit.Name = "AllowNewCharacterEdit";
            AllowNewCharacterEdit.Properties.Caption = "";
            AllowNewCharacterEdit.Size = new System.Drawing.Size(100, 19);
            AllowNewCharacterEdit.TabIndex = 64;
            // 
            // labelControl9
            // 
            labelControl9.Location = new System.Drawing.Point(73, 67);
            labelControl9.Name = "labelControl9";
            labelControl9.Size = new System.Drawing.Size(29, 13);
            labelControl9.TabIndex = 63;
            labelControl9.Text = "Login:";
            // 
            // AllowLoginEdit
            // 
            AllowLoginEdit.Location = new System.Drawing.Point(108, 64);
            AllowLoginEdit.MenuManager = ribbon;
            AllowLoginEdit.Name = "AllowLoginEdit";
            AllowLoginEdit.Properties.Caption = "";
            AllowLoginEdit.Size = new System.Drawing.Size(100, 19);
            AllowLoginEdit.TabIndex = 62;
            // 
            // labelControl8
            // 
            labelControl8.Location = new System.Drawing.Point(12, 42);
            labelControl8.Name = "labelControl8";
            labelControl8.Size = new System.Drawing.Size(90, 13);
            labelControl8.TabIndex = 61;
            labelControl8.Text = "Change Password:";
            // 
            // AllowChangePasswordEdit
            // 
            AllowChangePasswordEdit.Location = new System.Drawing.Point(108, 39);
            AllowChangePasswordEdit.MenuManager = ribbon;
            AllowChangePasswordEdit.Name = "AllowChangePasswordEdit";
            AllowChangePasswordEdit.Properties.Caption = "";
            AllowChangePasswordEdit.Size = new System.Drawing.Size(100, 19);
            AllowChangePasswordEdit.TabIndex = 60;
            // 
            // labelControl7
            // 
            labelControl7.Location = new System.Drawing.Point(35, 17);
            labelControl7.Name = "labelControl7";
            labelControl7.Size = new System.Drawing.Size(67, 13);
            labelControl7.TabIndex = 59;
            labelControl7.Text = "New Account:";
            // 
            // AllowNewAccountEdit
            // 
            AllowNewAccountEdit.Location = new System.Drawing.Point(108, 14);
            AllowNewAccountEdit.MenuManager = ribbon;
            AllowNewAccountEdit.Name = "AllowNewAccountEdit";
            AllowNewAccountEdit.Properties.Caption = "";
            AllowNewAccountEdit.Size = new System.Drawing.Size(100, 19);
            AllowNewAccountEdit.TabIndex = 58;
            // 
            // xtraTabPage3
            // 
            xtraTabPage3.Controls.Add(RabbitEventEndEdit);
            xtraTabPage3.Controls.Add(labelControl85);
            xtraTabPage3.Controls.Add(ReleaseDateEdit);
            xtraTabPage3.Controls.Add(labelControl70);
            xtraTabPage3.Controls.Add(ClientPathEdit);
            xtraTabPage3.Controls.Add(labelControl96);
            xtraTabPage3.Controls.Add(MasterPasswordEdit);
            xtraTabPage3.Controls.Add(labelControl67);
            xtraTabPage3.Controls.Add(MapPathEdit);
            xtraTabPage3.Controls.Add(labelControl13);
            xtraTabPage3.Controls.Add(labelControl10);
            xtraTabPage3.Controls.Add(DBSaveDelayEdit);
            xtraTabPage3.Controls.Add(CheckVersionButton);
            xtraTabPage3.Controls.Add(VersionPathEdit);
            xtraTabPage3.Controls.Add(labelControl5);
            xtraTabPage3.Controls.Add(labelControl4);
            xtraTabPage3.Controls.Add(CheckVersionEdit);
            xtraTabPage3.Name = "xtraTabPage3";
            xtraTabPage3.Size = new System.Drawing.Size(763, 392);
            xtraTabPage3.Text = "System";
            // 
            // RabbitEventEndEdit
            // 
            RabbitEventEndEdit.Location = new System.Drawing.Point(103, 275);
            RabbitEventEndEdit.MenuManager = ribbon;
            RabbitEventEndEdit.Name = "RabbitEventEndEdit";
            RabbitEventEndEdit.Properties.Appearance.Options.UseTextOptions = true;
            RabbitEventEndEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            RabbitEventEndEdit.Properties.Mask.EditMask = "f";
            RabbitEventEndEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            RabbitEventEndEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            RabbitEventEndEdit.Size = new System.Drawing.Size(250, 20);
            RabbitEventEndEdit.TabIndex = 74;
            // 
            // labelControl85
            // 
            labelControl85.Location = new System.Drawing.Point(31, 278);
            labelControl85.Name = "labelControl85";
            labelControl85.Size = new System.Drawing.Size(66, 13);
            labelControl85.TabIndex = 75;
            labelControl85.Text = "Rabbit Event:";
            // 
            // ReleaseDateEdit
            // 
            ReleaseDateEdit.Location = new System.Drawing.Point(103, 249);
            ReleaseDateEdit.MenuManager = ribbon;
            ReleaseDateEdit.Name = "ReleaseDateEdit";
            ReleaseDateEdit.Properties.Appearance.Options.UseTextOptions = true;
            ReleaseDateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ReleaseDateEdit.Properties.Mask.EditMask = "f";
            ReleaseDateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            ReleaseDateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            ReleaseDateEdit.Size = new System.Drawing.Size(250, 20);
            ReleaseDateEdit.TabIndex = 72;
            // 
            // labelControl70
            // 
            labelControl70.Location = new System.Drawing.Point(31, 252);
            labelControl70.Name = "labelControl70";
            labelControl70.Size = new System.Drawing.Size(68, 13);
            labelControl70.TabIndex = 73;
            labelControl70.Text = "Release Date:";
            // 
            // ClientPathEdit
            // 
            ClientPathEdit.Location = new System.Drawing.Point(103, 94);
            ClientPathEdit.MenuManager = ribbon;
            ClientPathEdit.Name = "ClientPathEdit";
            ClientPathEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            ClientPathEdit.Size = new System.Drawing.Size(250, 20);
            ClientPathEdit.TabIndex = 71;
            ClientPathEdit.ButtonClick += ClientPathEdit_ButtonClick;
            // 
            // labelControl96
            // 
            labelControl96.Location = new System.Drawing.Point(41, 97);
            labelControl96.Name = "labelControl96";
            labelControl96.Size = new System.Drawing.Size(56, 13);
            labelControl96.TabIndex = 70;
            labelControl96.Text = "Client Path:";
            // 
            // MasterPasswordEdit
            // 
            MasterPasswordEdit.Location = new System.Drawing.Point(103, 198);
            MasterPasswordEdit.MenuManager = ribbon;
            MasterPasswordEdit.Name = "MasterPasswordEdit";
            MasterPasswordEdit.Properties.Appearance.Options.UseTextOptions = true;
            MasterPasswordEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MasterPasswordEdit.Properties.PasswordChar = '*';
            MasterPasswordEdit.Size = new System.Drawing.Size(100, 20);
            MasterPasswordEdit.TabIndex = 68;
            // 
            // labelControl67
            // 
            labelControl67.Location = new System.Drawing.Point(11, 201);
            labelControl67.Name = "labelControl67";
            labelControl67.Size = new System.Drawing.Size(86, 13);
            labelControl67.TabIndex = 69;
            labelControl67.Text = "Master Password:";
            // 
            // MapPathEdit
            // 
            MapPathEdit.Location = new System.Drawing.Point(103, 172);
            MapPathEdit.MenuManager = ribbon;
            MapPathEdit.Name = "MapPathEdit";
            MapPathEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            MapPathEdit.Size = new System.Drawing.Size(250, 20);
            MapPathEdit.TabIndex = 30;
            MapPathEdit.ButtonClick += MapPathEdit_ButtonClick;
            // 
            // labelControl13
            // 
            labelControl13.Location = new System.Drawing.Point(48, 175);
            labelControl13.Name = "labelControl13";
            labelControl13.Size = new System.Drawing.Size(49, 13);
            labelControl13.TabIndex = 29;
            labelControl13.Text = "Map Path:";
            // 
            // labelControl10
            // 
            labelControl10.Location = new System.Drawing.Point(23, 149);
            labelControl10.Name = "labelControl10";
            labelControl10.Size = new System.Drawing.Size(74, 13);
            labelControl10.TabIndex = 28;
            labelControl10.Text = "DB Save Delay:";
            // 
            // DBSaveDelayEdit
            // 
            DBSaveDelayEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            DBSaveDelayEdit.Location = new System.Drawing.Point(103, 146);
            DBSaveDelayEdit.MenuManager = ribbon;
            DBSaveDelayEdit.Name = "DBSaveDelayEdit";
            DBSaveDelayEdit.Properties.AllowEditDays = false;
            DBSaveDelayEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            DBSaveDelayEdit.Properties.Mask.EditMask = "HH:mm:ss";
            DBSaveDelayEdit.Size = new System.Drawing.Size(100, 20);
            DBSaveDelayEdit.TabIndex = 27;
            // 
            // CheckVersionButton
            // 
            CheckVersionButton.Location = new System.Drawing.Point(103, 65);
            CheckVersionButton.Name = "CheckVersionButton";
            CheckVersionButton.Size = new System.Drawing.Size(86, 23);
            CheckVersionButton.TabIndex = 26;
            CheckVersionButton.Text = "Check Version";
            CheckVersionButton.Click += CheckVersionButton_Click;
            // 
            // VersionPathEdit
            // 
            VersionPathEdit.Location = new System.Drawing.Point(103, 39);
            VersionPathEdit.MenuManager = ribbon;
            VersionPathEdit.Name = "VersionPathEdit";
            VersionPathEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            VersionPathEdit.Size = new System.Drawing.Size(250, 20);
            VersionPathEdit.TabIndex = 25;
            VersionPathEdit.ButtonClick += VersionPathEdit_ButtonClick;
            // 
            // labelControl5
            // 
            labelControl5.Location = new System.Drawing.Point(33, 42);
            labelControl5.Name = "labelControl5";
            labelControl5.Size = new System.Drawing.Size(64, 13);
            labelControl5.TabIndex = 24;
            labelControl5.Text = "Version Path:";
            // 
            // labelControl4
            // 
            labelControl4.Location = new System.Drawing.Point(26, 17);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(71, 13);
            labelControl4.TabIndex = 23;
            labelControl4.Text = "Check Version:";
            // 
            // CheckVersionEdit
            // 
            CheckVersionEdit.Location = new System.Drawing.Point(103, 14);
            CheckVersionEdit.MenuManager = ribbon;
            CheckVersionEdit.Name = "CheckVersionEdit";
            CheckVersionEdit.Properties.Caption = "";
            CheckVersionEdit.Size = new System.Drawing.Size(100, 19);
            CheckVersionEdit.TabIndex = 22;
            // 
            // xtraTabPage4
            // 
            xtraTabPage4.Controls.Add(MailDisplayNameEdit);
            xtraTabPage4.Controls.Add(labelControl31);
            xtraTabPage4.Controls.Add(MailFromEdit);
            xtraTabPage4.Controls.Add(labelControl30);
            xtraTabPage4.Controls.Add(MailPasswordEdit);
            xtraTabPage4.Controls.Add(labelControl29);
            xtraTabPage4.Controls.Add(MailAccountEdit);
            xtraTabPage4.Controls.Add(labelControl28);
            xtraTabPage4.Controls.Add(labelControl27);
            xtraTabPage4.Controls.Add(MailUseSSLEdit);
            xtraTabPage4.Controls.Add(labelControl25);
            xtraTabPage4.Controls.Add(MailPortEdit);
            xtraTabPage4.Controls.Add(MailServerEdit);
            xtraTabPage4.Controls.Add(labelControl26);
            xtraTabPage4.Name = "xtraTabPage4";
            xtraTabPage4.Size = new System.Drawing.Size(763, 392);
            xtraTabPage4.Text = "Mail";
            // 
            // MailDisplayNameEdit
            // 
            MailDisplayNameEdit.Location = new System.Drawing.Point(108, 173);
            MailDisplayNameEdit.MenuManager = ribbon;
            MailDisplayNameEdit.Name = "MailDisplayNameEdit";
            MailDisplayNameEdit.Properties.Appearance.Options.UseTextOptions = true;
            MailDisplayNameEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MailDisplayNameEdit.Size = new System.Drawing.Size(100, 20);
            MailDisplayNameEdit.TabIndex = 70;
            // 
            // labelControl31
            // 
            labelControl31.Location = new System.Drawing.Point(34, 176);
            labelControl31.Name = "labelControl31";
            labelControl31.Size = new System.Drawing.Size(68, 13);
            labelControl31.TabIndex = 71;
            labelControl31.Text = "Display Name:";
            // 
            // MailFromEdit
            // 
            MailFromEdit.Location = new System.Drawing.Point(108, 147);
            MailFromEdit.MenuManager = ribbon;
            MailFromEdit.Name = "MailFromEdit";
            MailFromEdit.Properties.Appearance.Options.UseTextOptions = true;
            MailFromEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MailFromEdit.Size = new System.Drawing.Size(100, 20);
            MailFromEdit.TabIndex = 68;
            // 
            // labelControl30
            // 
            labelControl30.Location = new System.Drawing.Point(74, 150);
            labelControl30.Name = "labelControl30";
            labelControl30.Size = new System.Drawing.Size(28, 13);
            labelControl30.TabIndex = 69;
            labelControl30.Text = "From:";
            // 
            // MailPasswordEdit
            // 
            MailPasswordEdit.Location = new System.Drawing.Point(108, 121);
            MailPasswordEdit.MenuManager = ribbon;
            MailPasswordEdit.Name = "MailPasswordEdit";
            MailPasswordEdit.Properties.Appearance.Options.UseTextOptions = true;
            MailPasswordEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MailPasswordEdit.Properties.PasswordChar = '*';
            MailPasswordEdit.Size = new System.Drawing.Size(100, 20);
            MailPasswordEdit.TabIndex = 66;
            // 
            // labelControl29
            // 
            labelControl29.Location = new System.Drawing.Point(52, 124);
            labelControl29.Name = "labelControl29";
            labelControl29.Size = new System.Drawing.Size(50, 13);
            labelControl29.TabIndex = 67;
            labelControl29.Text = "Password:";
            // 
            // MailAccountEdit
            // 
            MailAccountEdit.Location = new System.Drawing.Point(108, 95);
            MailAccountEdit.MenuManager = ribbon;
            MailAccountEdit.Name = "MailAccountEdit";
            MailAccountEdit.Properties.Appearance.Options.UseTextOptions = true;
            MailAccountEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MailAccountEdit.Size = new System.Drawing.Size(100, 20);
            MailAccountEdit.TabIndex = 64;
            // 
            // labelControl28
            // 
            labelControl28.Location = new System.Drawing.Point(59, 98);
            labelControl28.Name = "labelControl28";
            labelControl28.Size = new System.Drawing.Size(43, 13);
            labelControl28.TabIndex = 65;
            labelControl28.Text = "Account:";
            // 
            // labelControl27
            // 
            labelControl27.Location = new System.Drawing.Point(60, 73);
            labelControl27.Name = "labelControl27";
            labelControl27.Size = new System.Drawing.Size(42, 13);
            labelControl27.TabIndex = 63;
            labelControl27.Text = "Use SSL:";
            // 
            // MailUseSSLEdit
            // 
            MailUseSSLEdit.Location = new System.Drawing.Point(108, 70);
            MailUseSSLEdit.MenuManager = ribbon;
            MailUseSSLEdit.Name = "MailUseSSLEdit";
            MailUseSSLEdit.Properties.Caption = "";
            MailUseSSLEdit.Size = new System.Drawing.Size(100, 19);
            MailUseSSLEdit.TabIndex = 62;
            // 
            // labelControl25
            // 
            labelControl25.Location = new System.Drawing.Point(78, 47);
            labelControl25.Name = "labelControl25";
            labelControl25.Size = new System.Drawing.Size(24, 13);
            labelControl25.TabIndex = 34;
            labelControl25.Text = "Port:";
            // 
            // MailPortEdit
            // 
            MailPortEdit.Location = new System.Drawing.Point(108, 44);
            MailPortEdit.MenuManager = ribbon;
            MailPortEdit.Name = "MailPortEdit";
            MailPortEdit.Properties.Appearance.Options.UseTextOptions = true;
            MailPortEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MailPortEdit.Properties.Mask.EditMask = "n0";
            MailPortEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            MailPortEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MailPortEdit.Size = new System.Drawing.Size(100, 20);
            MailPortEdit.TabIndex = 33;
            // 
            // MailServerEdit
            // 
            MailServerEdit.Location = new System.Drawing.Point(108, 18);
            MailServerEdit.MenuManager = ribbon;
            MailServerEdit.Name = "MailServerEdit";
            MailServerEdit.Properties.Appearance.Options.UseTextOptions = true;
            MailServerEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MailServerEdit.Size = new System.Drawing.Size(100, 20);
            MailServerEdit.TabIndex = 31;
            // 
            // labelControl26
            // 
            labelControl26.Location = new System.Drawing.Point(66, 21);
            labelControl26.Name = "labelControl26";
            labelControl26.Size = new System.Drawing.Size(36, 13);
            labelControl26.TabIndex = 32;
            labelControl26.Text = "Server:";
            // 
            // xtraTabPage5
            // 
            xtraTabPage5.Controls.Add(labelControl81);
            xtraTabPage5.Controls.Add(AllowBuyGameGoldEdit);
            xtraTabPage5.Controls.Add(labelControl80);
            xtraTabPage5.Controls.Add(ProcessGameGoldEdit);
            xtraTabPage5.Controls.Add(ReceiverEMailEdit);
            xtraTabPage5.Controls.Add(labelControl79);
            xtraTabPage5.Controls.Add(IPNPrefixEdit);
            xtraTabPage5.Controls.Add(labelControl73);
            xtraTabPage5.Controls.Add(BuyAddressEdit);
            xtraTabPage5.Controls.Add(labelControl72);
            xtraTabPage5.Controls.Add(BuyPrefixEdit);
            xtraTabPage5.Controls.Add(labelControl71);
            xtraTabPage5.Controls.Add(DeleteFailLinkEdit);
            xtraTabPage5.Controls.Add(labelControl37);
            xtraTabPage5.Controls.Add(DeleteSuccessLinkEdit);
            xtraTabPage5.Controls.Add(labelControl43);
            xtraTabPage5.Controls.Add(ResetFailLinkEdit);
            xtraTabPage5.Controls.Add(labelControl32);
            xtraTabPage5.Controls.Add(ResetSuccessLinkEdit);
            xtraTabPage5.Controls.Add(labelControl33);
            xtraTabPage5.Controls.Add(ActivationFailLinkEdit);
            xtraTabPage5.Controls.Add(labelControl34);
            xtraTabPage5.Controls.Add(ActivationSuccessLinkEdit);
            xtraTabPage5.Controls.Add(labelControl35);
            xtraTabPage5.Controls.Add(labelControl41);
            xtraTabPage5.Controls.Add(WebCommandLinkEdit);
            xtraTabPage5.Controls.Add(WebPrefixEdit);
            xtraTabPage5.Controls.Add(labelControl42);
            xtraTabPage5.Name = "xtraTabPage5";
            xtraTabPage5.Size = new System.Drawing.Size(763, 392);
            xtraTabPage5.Text = "Web Server";
            // 
            // labelControl81
            // 
            labelControl81.Location = new System.Drawing.Point(375, 203);
            labelControl81.Name = "labelControl81";
            labelControl81.Size = new System.Drawing.Size(104, 13);
            labelControl81.TabIndex = 101;
            labelControl81.Text = "Allow Buy Game Gold:";
            // 
            // AllowBuyGameGoldEdit
            // 
            AllowBuyGameGoldEdit.Location = new System.Drawing.Point(485, 200);
            AllowBuyGameGoldEdit.MenuManager = ribbon;
            AllowBuyGameGoldEdit.Name = "AllowBuyGameGoldEdit";
            AllowBuyGameGoldEdit.Properties.Caption = "";
            AllowBuyGameGoldEdit.Size = new System.Drawing.Size(100, 19);
            AllowBuyGameGoldEdit.TabIndex = 100;
            // 
            // labelControl80
            // 
            labelControl80.Location = new System.Drawing.Point(384, 177);
            labelControl80.Name = "labelControl80";
            labelControl80.Size = new System.Drawing.Size(95, 13);
            labelControl80.TabIndex = 99;
            labelControl80.Text = "Process Game Gold:";
            // 
            // ProcessGameGoldEdit
            // 
            ProcessGameGoldEdit.Location = new System.Drawing.Point(485, 174);
            ProcessGameGoldEdit.MenuManager = ribbon;
            ProcessGameGoldEdit.Name = "ProcessGameGoldEdit";
            ProcessGameGoldEdit.Properties.Caption = "";
            ProcessGameGoldEdit.Size = new System.Drawing.Size(100, 19);
            ProcessGameGoldEdit.TabIndex = 98;
            // 
            // ReceiverEMailEdit
            // 
            ReceiverEMailEdit.Location = new System.Drawing.Point(485, 122);
            ReceiverEMailEdit.MenuManager = ribbon;
            ReceiverEMailEdit.Name = "ReceiverEMailEdit";
            ReceiverEMailEdit.Properties.Appearance.Options.UseTextOptions = true;
            ReceiverEMailEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ReceiverEMailEdit.Size = new System.Drawing.Size(210, 20);
            ReceiverEMailEdit.TabIndex = 96;
            // 
            // labelControl79
            // 
            labelControl79.Location = new System.Drawing.Point(406, 125);
            labelControl79.Name = "labelControl79";
            labelControl79.Size = new System.Drawing.Size(73, 13);
            labelControl79.TabIndex = 97;
            labelControl79.Text = "Receiver EMail:";
            // 
            // IPNPrefixEdit
            // 
            IPNPrefixEdit.Location = new System.Drawing.Point(485, 96);
            IPNPrefixEdit.MenuManager = ribbon;
            IPNPrefixEdit.Name = "IPNPrefixEdit";
            IPNPrefixEdit.Properties.Appearance.Options.UseTextOptions = true;
            IPNPrefixEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            IPNPrefixEdit.Size = new System.Drawing.Size(210, 20);
            IPNPrefixEdit.TabIndex = 94;
            // 
            // labelControl73
            // 
            labelControl73.Location = new System.Drawing.Point(427, 99);
            labelControl73.Name = "labelControl73";
            labelControl73.Size = new System.Drawing.Size(52, 13);
            labelControl73.TabIndex = 95;
            labelControl73.Text = "IPN Prefix:";
            // 
            // BuyAddressEdit
            // 
            BuyAddressEdit.Location = new System.Drawing.Point(485, 44);
            BuyAddressEdit.MenuManager = ribbon;
            BuyAddressEdit.Name = "BuyAddressEdit";
            BuyAddressEdit.Properties.Appearance.Options.UseTextOptions = true;
            BuyAddressEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            BuyAddressEdit.Size = new System.Drawing.Size(210, 20);
            BuyAddressEdit.TabIndex = 92;
            // 
            // labelControl72
            // 
            labelControl72.Location = new System.Drawing.Point(415, 47);
            labelControl72.Name = "labelControl72";
            labelControl72.Size = new System.Drawing.Size(64, 13);
            labelControl72.TabIndex = 93;
            labelControl72.Text = "Buy Address:";
            // 
            // BuyPrefixEdit
            // 
            BuyPrefixEdit.Location = new System.Drawing.Point(485, 18);
            BuyPrefixEdit.MenuManager = ribbon;
            BuyPrefixEdit.Name = "BuyPrefixEdit";
            BuyPrefixEdit.Properties.Appearance.Options.UseTextOptions = true;
            BuyPrefixEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            BuyPrefixEdit.Size = new System.Drawing.Size(210, 20);
            BuyPrefixEdit.TabIndex = 90;
            // 
            // labelControl71
            // 
            labelControl71.Location = new System.Drawing.Point(426, 21);
            labelControl71.Name = "labelControl71";
            labelControl71.Size = new System.Drawing.Size(53, 13);
            labelControl71.TabIndex = 91;
            labelControl71.Text = "Buy Prefix:";
            // 
            // DeleteFailLinkEdit
            // 
            DeleteFailLinkEdit.Location = new System.Drawing.Point(149, 278);
            DeleteFailLinkEdit.MenuManager = ribbon;
            DeleteFailLinkEdit.Name = "DeleteFailLinkEdit";
            DeleteFailLinkEdit.Properties.Appearance.Options.UseTextOptions = true;
            DeleteFailLinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            DeleteFailLinkEdit.Size = new System.Drawing.Size(210, 20);
            DeleteFailLinkEdit.TabIndex = 88;
            // 
            // labelControl37
            // 
            labelControl37.Location = new System.Drawing.Point(71, 281);
            labelControl37.Name = "labelControl37";
            labelControl37.Size = new System.Drawing.Size(75, 13);
            labelControl37.TabIndex = 89;
            labelControl37.Text = "Delete Fail Link:";
            // 
            // DeleteSuccessLinkEdit
            // 
            DeleteSuccessLinkEdit.Location = new System.Drawing.Point(149, 252);
            DeleteSuccessLinkEdit.MenuManager = ribbon;
            DeleteSuccessLinkEdit.Name = "DeleteSuccessLinkEdit";
            DeleteSuccessLinkEdit.Properties.Appearance.Options.UseTextOptions = true;
            DeleteSuccessLinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            DeleteSuccessLinkEdit.Size = new System.Drawing.Size(210, 20);
            DeleteSuccessLinkEdit.TabIndex = 86;
            // 
            // labelControl43
            // 
            labelControl43.Location = new System.Drawing.Point(49, 255);
            labelControl43.Name = "labelControl43";
            labelControl43.Size = new System.Drawing.Size(97, 13);
            labelControl43.TabIndex = 87;
            labelControl43.Text = "Delete Success Link:";
            // 
            // ResetFailLinkEdit
            // 
            ResetFailLinkEdit.Location = new System.Drawing.Point(149, 200);
            ResetFailLinkEdit.MenuManager = ribbon;
            ResetFailLinkEdit.Name = "ResetFailLinkEdit";
            ResetFailLinkEdit.Properties.Appearance.Options.UseTextOptions = true;
            ResetFailLinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ResetFailLinkEdit.Size = new System.Drawing.Size(210, 20);
            ResetFailLinkEdit.TabIndex = 84;
            // 
            // labelControl32
            // 
            labelControl32.Location = new System.Drawing.Point(71, 203);
            labelControl32.Name = "labelControl32";
            labelControl32.Size = new System.Drawing.Size(72, 13);
            labelControl32.TabIndex = 85;
            labelControl32.Text = "Reset Fail Link:";
            // 
            // ResetSuccessLinkEdit
            // 
            ResetSuccessLinkEdit.Location = new System.Drawing.Point(149, 174);
            ResetSuccessLinkEdit.MenuManager = ribbon;
            ResetSuccessLinkEdit.Name = "ResetSuccessLinkEdit";
            ResetSuccessLinkEdit.Properties.Appearance.Options.UseTextOptions = true;
            ResetSuccessLinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ResetSuccessLinkEdit.Size = new System.Drawing.Size(210, 20);
            ResetSuccessLinkEdit.TabIndex = 82;
            // 
            // labelControl33
            // 
            labelControl33.Location = new System.Drawing.Point(49, 177);
            labelControl33.Name = "labelControl33";
            labelControl33.Size = new System.Drawing.Size(94, 13);
            labelControl33.TabIndex = 83;
            labelControl33.Text = "Reset Success Link:";
            // 
            // ActivationFailLinkEdit
            // 
            ActivationFailLinkEdit.Location = new System.Drawing.Point(149, 122);
            ActivationFailLinkEdit.MenuManager = ribbon;
            ActivationFailLinkEdit.Name = "ActivationFailLinkEdit";
            ActivationFailLinkEdit.Properties.Appearance.Options.UseTextOptions = true;
            ActivationFailLinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ActivationFailLinkEdit.Size = new System.Drawing.Size(210, 20);
            ActivationFailLinkEdit.TabIndex = 80;
            // 
            // labelControl34
            // 
            labelControl34.Location = new System.Drawing.Point(51, 125);
            labelControl34.Name = "labelControl34";
            labelControl34.Size = new System.Drawing.Size(92, 13);
            labelControl34.TabIndex = 81;
            labelControl34.Text = "Activation Fail Link:";
            // 
            // ActivationSuccessLinkEdit
            // 
            ActivationSuccessLinkEdit.Location = new System.Drawing.Point(149, 96);
            ActivationSuccessLinkEdit.MenuManager = ribbon;
            ActivationSuccessLinkEdit.Name = "ActivationSuccessLinkEdit";
            ActivationSuccessLinkEdit.Properties.Appearance.Options.UseTextOptions = true;
            ActivationSuccessLinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ActivationSuccessLinkEdit.Size = new System.Drawing.Size(210, 20);
            ActivationSuccessLinkEdit.TabIndex = 78;
            // 
            // labelControl35
            // 
            labelControl35.Location = new System.Drawing.Point(29, 99);
            labelControl35.Name = "labelControl35";
            labelControl35.Size = new System.Drawing.Size(114, 13);
            labelControl35.TabIndex = 79;
            labelControl35.Text = "Activation Success Link:";
            // 
            // labelControl41
            // 
            labelControl41.Location = new System.Drawing.Point(71, 47);
            labelControl41.Name = "labelControl41";
            labelControl41.Size = new System.Drawing.Size(72, 13);
            labelControl41.TabIndex = 75;
            labelControl41.Text = "Command Link:";
            // 
            // WebCommandLinkEdit
            // 
            WebCommandLinkEdit.Location = new System.Drawing.Point(149, 44);
            WebCommandLinkEdit.MenuManager = ribbon;
            WebCommandLinkEdit.Name = "WebCommandLinkEdit";
            WebCommandLinkEdit.Properties.Appearance.Options.UseTextOptions = true;
            WebCommandLinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            WebCommandLinkEdit.Properties.Mask.EditMask = "n0";
            WebCommandLinkEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            WebCommandLinkEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            WebCommandLinkEdit.Size = new System.Drawing.Size(210, 20);
            WebCommandLinkEdit.TabIndex = 74;
            // 
            // WebPrefixEdit
            // 
            WebPrefixEdit.Location = new System.Drawing.Point(149, 18);
            WebPrefixEdit.MenuManager = ribbon;
            WebPrefixEdit.Name = "WebPrefixEdit";
            WebPrefixEdit.Properties.Appearance.Options.UseTextOptions = true;
            WebPrefixEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            WebPrefixEdit.Size = new System.Drawing.Size(210, 20);
            WebPrefixEdit.TabIndex = 72;
            // 
            // labelControl42
            // 
            labelControl42.Location = new System.Drawing.Point(86, 21);
            labelControl42.Name = "labelControl42";
            labelControl42.Size = new System.Drawing.Size(57, 13);
            labelControl42.TabIndex = 73;
            labelControl42.Text = "Web Prefix:";
            // 
            // xtraTabPage6
            // 
            xtraTabPage6.Controls.Add(labelControl91);
            xtraTabPage6.Controls.Add(EnableFortuneEdit);
            xtraTabPage6.Controls.Add(labelControl90);
            xtraTabPage6.Controls.Add(EnableHermitEdit);
            xtraTabPage6.Controls.Add(labelControl88);
            xtraTabPage6.Controls.Add(EnableStruckEdit);
            xtraTabPage6.Controls.Add(labelControl69);
            xtraTabPage6.Controls.Add(AutoReviveDelayEdit);
            xtraTabPage6.Controls.Add(PvPCurseRateEdit);
            xtraTabPage6.Controls.Add(labelControl83);
            xtraTabPage6.Controls.Add(labelControl84);
            xtraTabPage6.Controls.Add(PvPCurseDurationEdit);
            xtraTabPage6.Controls.Add(RedPointEdit);
            xtraTabPage6.Controls.Add(labelControl77);
            xtraTabPage6.Controls.Add(labelControl78);
            xtraTabPage6.Controls.Add(PKPointTickRateEdit);
            xtraTabPage6.Controls.Add(PKPointRateEdit);
            xtraTabPage6.Controls.Add(labelControl76);
            xtraTabPage6.Controls.Add(labelControl75);
            xtraTabPage6.Controls.Add(BrownDurationEdit);
            xtraTabPage6.Controls.Add(labelControl24);
            xtraTabPage6.Controls.Add(AllowObservationEdit);
            xtraTabPage6.Controls.Add(SkillExpEdit);
            xtraTabPage6.Controls.Add(labelControl53);
            xtraTabPage6.Controls.Add(DayCycleCountEdit);
            xtraTabPage6.Controls.Add(labelControl52);
            xtraTabPage6.Controls.Add(MaxLevelEdit);
            xtraTabPage6.Controls.Add(labelControl46);
            xtraTabPage6.Controls.Add(labelControl45);
            xtraTabPage6.Controls.Add(GlobalDelayEdit);
            xtraTabPage6.Controls.Add(labelControl44);
            xtraTabPage6.Controls.Add(ShoutDelayEdit);
            xtraTabPage6.Controls.Add(MaxViewRangeEdit);
            xtraTabPage6.Controls.Add(labelControl23);
            xtraTabPage6.Name = "xtraTabPage6";
            xtraTabPage6.Size = new System.Drawing.Size(763, 392);
            xtraTabPage6.Text = "Players";
            // 
            // labelControl90
            // 
            labelControl90.Location = new System.Drawing.Point(353, 199);
            labelControl90.Name = "labelControl90";
            labelControl90.Size = new System.Drawing.Size(70, 13);
            labelControl90.TabIndex = 130;
            labelControl90.Text = "Enable Hermit:";
            // 
            // EnableHermitEdit
            // 
            EnableHermitEdit.Location = new System.Drawing.Point(429, 196);
            EnableHermitEdit.MenuManager = ribbon;
            EnableHermitEdit.Name = "EnableHermitEdit";
            EnableHermitEdit.Properties.Caption = "";
            EnableHermitEdit.Size = new System.Drawing.Size(100, 19);
            EnableHermitEdit.TabIndex = 129;
            // 
            // labelControl88
            // 
            labelControl88.Location = new System.Drawing.Point(354, 177);
            labelControl88.Name = "labelControl88";
            labelControl88.Size = new System.Drawing.Size(69, 13);
            labelControl88.TabIndex = 128;
            labelControl88.Text = "Enable Struck:";
            // 
            // EnableStruckEdit
            // 
            EnableStruckEdit.Location = new System.Drawing.Point(429, 174);
            EnableStruckEdit.MenuManager = ribbon;
            EnableStruckEdit.Name = "EnableStruckEdit";
            EnableStruckEdit.Properties.Caption = "";
            EnableStruckEdit.Size = new System.Drawing.Size(100, 19);
            EnableStruckEdit.TabIndex = 127;
            // 
            // labelControl69
            // 
            labelControl69.Location = new System.Drawing.Point(18, 202);
            labelControl69.Name = "labelControl69";
            labelControl69.Size = new System.Drawing.Size(93, 13);
            labelControl69.TabIndex = 126;
            labelControl69.Text = "Auto Revive Delay:";
            // 
            // AutoReviveDelayEdit
            // 
            AutoReviveDelayEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            AutoReviveDelayEdit.Location = new System.Drawing.Point(117, 199);
            AutoReviveDelayEdit.MenuManager = ribbon;
            AutoReviveDelayEdit.Name = "AutoReviveDelayEdit";
            AutoReviveDelayEdit.Properties.AllowEditDays = false;
            AutoReviveDelayEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            AutoReviveDelayEdit.Properties.Mask.EditMask = "HH:mm:ss";
            AutoReviveDelayEdit.Size = new System.Drawing.Size(100, 20);
            AutoReviveDelayEdit.TabIndex = 125;
            // 
            // PvPCurseRateEdit
            // 
            PvPCurseRateEdit.Location = new System.Drawing.Point(429, 148);
            PvPCurseRateEdit.MenuManager = ribbon;
            PvPCurseRateEdit.Name = "PvPCurseRateEdit";
            PvPCurseRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            PvPCurseRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            PvPCurseRateEdit.Properties.Mask.EditMask = "n0";
            PvPCurseRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            PvPCurseRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            PvPCurseRateEdit.Size = new System.Drawing.Size(100, 20);
            PvPCurseRateEdit.TabIndex = 124;
            // 
            // labelControl83
            // 
            labelControl83.Location = new System.Drawing.Point(344, 151);
            labelControl83.Name = "labelControl83";
            labelControl83.Size = new System.Drawing.Size(79, 13);
            labelControl83.TabIndex = 123;
            labelControl83.Text = "PvP Curse Rate:";
            // 
            // labelControl84
            // 
            labelControl84.Location = new System.Drawing.Point(326, 125);
            labelControl84.Name = "labelControl84";
            labelControl84.Size = new System.Drawing.Size(97, 13);
            labelControl84.TabIndex = 122;
            labelControl84.Text = "PvP Curse Duration:";
            // 
            // PvPCurseDurationEdit
            // 
            PvPCurseDurationEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            PvPCurseDurationEdit.Location = new System.Drawing.Point(429, 122);
            PvPCurseDurationEdit.MenuManager = ribbon;
            PvPCurseDurationEdit.Name = "PvPCurseDurationEdit";
            PvPCurseDurationEdit.Properties.AllowEditDays = false;
            PvPCurseDurationEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            PvPCurseDurationEdit.Properties.Mask.EditMask = "HH:mm:ss";
            PvPCurseDurationEdit.Size = new System.Drawing.Size(100, 20);
            PvPCurseDurationEdit.TabIndex = 121;
            // 
            // RedPointEdit
            // 
            RedPointEdit.Location = new System.Drawing.Point(429, 96);
            RedPointEdit.MenuManager = ribbon;
            RedPointEdit.Name = "RedPointEdit";
            RedPointEdit.Properties.Appearance.Options.UseTextOptions = true;
            RedPointEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            RedPointEdit.Properties.Mask.EditMask = "n0";
            RedPointEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            RedPointEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            RedPointEdit.Size = new System.Drawing.Size(100, 20);
            RedPointEdit.TabIndex = 112;
            // 
            // labelControl77
            // 
            labelControl77.Location = new System.Drawing.Point(373, 99);
            labelControl77.Name = "labelControl77";
            labelControl77.Size = new System.Drawing.Size(50, 13);
            labelControl77.TabIndex = 111;
            labelControl77.Text = "Red Point:";
            // 
            // labelControl78
            // 
            labelControl78.Location = new System.Drawing.Point(333, 73);
            labelControl78.Name = "labelControl78";
            labelControl78.Size = new System.Drawing.Size(90, 13);
            labelControl78.TabIndex = 110;
            labelControl78.Text = "PK Point Tick Rate:";
            // 
            // PKPointTickRateEdit
            // 
            PKPointTickRateEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            PKPointTickRateEdit.Location = new System.Drawing.Point(429, 70);
            PKPointTickRateEdit.MenuManager = ribbon;
            PKPointTickRateEdit.Name = "PKPointTickRateEdit";
            PKPointTickRateEdit.Properties.AllowEditDays = false;
            PKPointTickRateEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            PKPointTickRateEdit.Properties.Mask.EditMask = "HH:mm:ss";
            PKPointTickRateEdit.Size = new System.Drawing.Size(100, 20);
            PKPointTickRateEdit.TabIndex = 109;
            // 
            // PKPointRateEdit
            // 
            PKPointRateEdit.Location = new System.Drawing.Point(429, 44);
            PKPointRateEdit.MenuManager = ribbon;
            PKPointRateEdit.Name = "PKPointRateEdit";
            PKPointRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            PKPointRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            PKPointRateEdit.Properties.Mask.EditMask = "n0";
            PKPointRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            PKPointRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            PKPointRateEdit.Size = new System.Drawing.Size(100, 20);
            PKPointRateEdit.TabIndex = 108;
            // 
            // labelControl76
            // 
            labelControl76.Location = new System.Drawing.Point(354, 47);
            labelControl76.Name = "labelControl76";
            labelControl76.Size = new System.Drawing.Size(69, 13);
            labelControl76.TabIndex = 107;
            labelControl76.Text = "PK Point Rate:";
            // 
            // labelControl75
            // 
            labelControl75.Location = new System.Drawing.Point(345, 21);
            labelControl75.Name = "labelControl75";
            labelControl75.Size = new System.Drawing.Size(78, 13);
            labelControl75.TabIndex = 106;
            labelControl75.Text = "Brown Duration:";
            // 
            // BrownDurationEdit
            // 
            BrownDurationEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            BrownDurationEdit.Location = new System.Drawing.Point(429, 18);
            BrownDurationEdit.MenuManager = ribbon;
            BrownDurationEdit.Name = "BrownDurationEdit";
            BrownDurationEdit.Properties.AllowEditDays = false;
            BrownDurationEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            BrownDurationEdit.Properties.Mask.EditMask = "HH:mm:ss";
            BrownDurationEdit.Size = new System.Drawing.Size(100, 20);
            BrownDurationEdit.TabIndex = 105;
            // 
            // labelControl24
            // 
            labelControl24.Location = new System.Drawing.Point(20, 177);
            labelControl24.Name = "labelControl24";
            labelControl24.Size = new System.Drawing.Size(91, 13);
            labelControl24.TabIndex = 83;
            labelControl24.Text = "Allow Observation:";
            // 
            // AllowObservationEdit
            // 
            AllowObservationEdit.Location = new System.Drawing.Point(117, 174);
            AllowObservationEdit.MenuManager = ribbon;
            AllowObservationEdit.Name = "AllowObservationEdit";
            AllowObservationEdit.Properties.Caption = "";
            AllowObservationEdit.Size = new System.Drawing.Size(100, 19);
            AllowObservationEdit.TabIndex = 82;
            // 
            // SkillExpEdit
            // 
            SkillExpEdit.Location = new System.Drawing.Point(117, 148);
            SkillExpEdit.MenuManager = ribbon;
            SkillExpEdit.Name = "SkillExpEdit";
            SkillExpEdit.Properties.Appearance.Options.UseTextOptions = true;
            SkillExpEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            SkillExpEdit.Properties.Mask.EditMask = "n0";
            SkillExpEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            SkillExpEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            SkillExpEdit.Size = new System.Drawing.Size(100, 20);
            SkillExpEdit.TabIndex = 81;
            // 
            // labelControl53
            // 
            labelControl53.Location = new System.Drawing.Point(69, 151);
            labelControl53.Name = "labelControl53";
            labelControl53.Size = new System.Drawing.Size(42, 13);
            labelControl53.TabIndex = 80;
            labelControl53.Text = "Skill Exp:";
            // 
            // DayCycleCountEdit
            // 
            DayCycleCountEdit.Location = new System.Drawing.Point(117, 122);
            DayCycleCountEdit.MenuManager = ribbon;
            DayCycleCountEdit.Name = "DayCycleCountEdit";
            DayCycleCountEdit.Properties.Appearance.Options.UseTextOptions = true;
            DayCycleCountEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            DayCycleCountEdit.Properties.Mask.EditMask = "n0";
            DayCycleCountEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            DayCycleCountEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            DayCycleCountEdit.Size = new System.Drawing.Size(100, 20);
            DayCycleCountEdit.TabIndex = 79;
            // 
            // labelControl52
            // 
            labelControl52.Location = new System.Drawing.Point(27, 125);
            labelControl52.Name = "labelControl52";
            labelControl52.Size = new System.Drawing.Size(84, 13);
            labelControl52.TabIndex = 78;
            labelControl52.Text = "Day Cycle Count:";
            // 
            // MaxLevelEdit
            // 
            MaxLevelEdit.Location = new System.Drawing.Point(117, 96);
            MaxLevelEdit.MenuManager = ribbon;
            MaxLevelEdit.Name = "MaxLevelEdit";
            MaxLevelEdit.Properties.Appearance.Options.UseTextOptions = true;
            MaxLevelEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MaxLevelEdit.Properties.Mask.EditMask = "n0";
            MaxLevelEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            MaxLevelEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MaxLevelEdit.Size = new System.Drawing.Size(100, 20);
            MaxLevelEdit.TabIndex = 77;
            // 
            // labelControl46
            // 
            labelControl46.Location = new System.Drawing.Point(59, 99);
            labelControl46.Name = "labelControl46";
            labelControl46.Size = new System.Drawing.Size(52, 13);
            labelControl46.TabIndex = 76;
            labelControl46.Text = "Max Level:";
            // 
            // labelControl45
            // 
            labelControl45.Location = new System.Drawing.Point(48, 73);
            labelControl45.Name = "labelControl45";
            labelControl45.Size = new System.Drawing.Size(63, 13);
            labelControl45.TabIndex = 75;
            labelControl45.Text = "Global Delay:";
            // 
            // GlobalDelayEdit
            // 
            GlobalDelayEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            GlobalDelayEdit.Location = new System.Drawing.Point(117, 70);
            GlobalDelayEdit.MenuManager = ribbon;
            GlobalDelayEdit.Name = "GlobalDelayEdit";
            GlobalDelayEdit.Properties.AllowEditDays = false;
            GlobalDelayEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            GlobalDelayEdit.Properties.Mask.EditMask = "HH:mm:ss";
            GlobalDelayEdit.Size = new System.Drawing.Size(100, 20);
            GlobalDelayEdit.TabIndex = 74;
            // 
            // labelControl44
            // 
            labelControl44.Location = new System.Drawing.Point(49, 47);
            labelControl44.Name = "labelControl44";
            labelControl44.Size = new System.Drawing.Size(62, 13);
            labelControl44.TabIndex = 73;
            labelControl44.Text = "Shout Delay:";
            // 
            // ShoutDelayEdit
            // 
            ShoutDelayEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            ShoutDelayEdit.Location = new System.Drawing.Point(117, 44);
            ShoutDelayEdit.MenuManager = ribbon;
            ShoutDelayEdit.Name = "ShoutDelayEdit";
            ShoutDelayEdit.Properties.AllowEditDays = false;
            ShoutDelayEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ShoutDelayEdit.Properties.Mask.EditMask = "HH:mm:ss";
            ShoutDelayEdit.Size = new System.Drawing.Size(100, 20);
            ShoutDelayEdit.TabIndex = 72;
            // 
            // MaxViewRangeEdit
            // 
            MaxViewRangeEdit.Location = new System.Drawing.Point(117, 18);
            MaxViewRangeEdit.MenuManager = ribbon;
            MaxViewRangeEdit.Name = "MaxViewRangeEdit";
            MaxViewRangeEdit.Properties.Appearance.Options.UseTextOptions = true;
            MaxViewRangeEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MaxViewRangeEdit.Properties.Mask.EditMask = "n0";
            MaxViewRangeEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            MaxViewRangeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MaxViewRangeEdit.Size = new System.Drawing.Size(100, 20);
            MaxViewRangeEdit.TabIndex = 35;
            // 
            // labelControl23
            // 
            labelControl23.Location = new System.Drawing.Point(28, 21);
            labelControl23.Name = "labelControl23";
            labelControl23.Size = new System.Drawing.Size(83, 13);
            labelControl23.TabIndex = 34;
            labelControl23.Text = "Max View Range:";
            // 
            // xtraTabPage7
            // 
            xtraTabPage7.Controls.Add(LairRegionIndexEdit);
            xtraTabPage7.Controls.Add(labelControl82);
            xtraTabPage7.Controls.Add(MysteryShipRegionIndexEdit);
            xtraTabPage7.Controls.Add(labelControl89);
            xtraTabPage7.Controls.Add(labelControl74);
            xtraTabPage7.Controls.Add(HarvestDurationEdit);
            xtraTabPage7.Controls.Add(labelControl47);
            xtraTabPage7.Controls.Add(DeadDurationEdit);
            xtraTabPage7.Name = "xtraTabPage7";
            xtraTabPage7.Size = new System.Drawing.Size(763, 392);
            xtraTabPage7.Text = "Monsters";
            // 
            // LairRegionIndexEdit
            // 
            LairRegionIndexEdit.Location = new System.Drawing.Point(142, 96);
            LairRegionIndexEdit.MenuManager = ribbon;
            LairRegionIndexEdit.Name = "LairRegionIndexEdit";
            LairRegionIndexEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            LairRegionIndexEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            LairRegionIndexEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Server Description"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size") });
            LairRegionIndexEdit.Properties.DisplayMember = "ServerDescription";
            LairRegionIndexEdit.Properties.NullText = "[Region is null]";
            LairRegionIndexEdit.Properties.ValueMember = "Index";
            LairRegionIndexEdit.Size = new System.Drawing.Size(174, 20);
            LairRegionIndexEdit.TabIndex = 125;
            // 
            // labelControl82
            // 
            labelControl82.Location = new System.Drawing.Point(79, 99);
            labelControl82.Name = "labelControl82";
            labelControl82.Size = new System.Drawing.Size(57, 13);
            labelControl82.TabIndex = 124;
            labelControl82.Text = "Lair Region:";
            // 
            // MysteryShipRegionIndexEdit
            // 
            MysteryShipRegionIndexEdit.Location = new System.Drawing.Point(142, 70);
            MysteryShipRegionIndexEdit.MenuManager = ribbon;
            MysteryShipRegionIndexEdit.Name = "MysteryShipRegionIndexEdit";
            MysteryShipRegionIndexEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            MysteryShipRegionIndexEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            MysteryShipRegionIndexEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Server Description"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size") });
            MysteryShipRegionIndexEdit.Properties.DisplayMember = "ServerDescription";
            MysteryShipRegionIndexEdit.Properties.NullText = "[Region is null]";
            MysteryShipRegionIndexEdit.Properties.ValueMember = "Index";
            MysteryShipRegionIndexEdit.Size = new System.Drawing.Size(174, 20);
            MysteryShipRegionIndexEdit.TabIndex = 123;
            // 
            // labelControl89
            // 
            labelControl89.Location = new System.Drawing.Point(34, 73);
            labelControl89.Name = "labelControl89";
            labelControl89.Size = new System.Drawing.Size(102, 13);
            labelControl89.TabIndex = 117;
            labelControl89.Text = "Mystery Ship Region:";
            // 
            // labelControl74
            // 
            labelControl74.Location = new System.Drawing.Point(50, 47);
            labelControl74.Name = "labelControl74";
            labelControl74.Size = new System.Drawing.Size(86, 13);
            labelControl74.TabIndex = 104;
            labelControl74.Text = "Harvest Duration:";
            // 
            // HarvestDurationEdit
            // 
            HarvestDurationEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            HarvestDurationEdit.Location = new System.Drawing.Point(142, 44);
            HarvestDurationEdit.MenuManager = ribbon;
            HarvestDurationEdit.Name = "HarvestDurationEdit";
            HarvestDurationEdit.Properties.AllowEditDays = false;
            HarvestDurationEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            HarvestDurationEdit.Properties.Mask.EditMask = "HH:mm:ss";
            HarvestDurationEdit.Size = new System.Drawing.Size(100, 20);
            HarvestDurationEdit.TabIndex = 103;
            // 
            // labelControl47
            // 
            labelControl47.Location = new System.Drawing.Point(63, 21);
            labelControl47.Name = "labelControl47";
            labelControl47.Size = new System.Drawing.Size(73, 13);
            labelControl47.TabIndex = 75;
            labelControl47.Text = "Dead Duration:";
            // 
            // DeadDurationEdit
            // 
            DeadDurationEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            DeadDurationEdit.Location = new System.Drawing.Point(142, 18);
            DeadDurationEdit.MenuManager = ribbon;
            DeadDurationEdit.Name = "DeadDurationEdit";
            DeadDurationEdit.Properties.AllowEditDays = false;
            DeadDurationEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            DeadDurationEdit.Properties.Mask.EditMask = "HH:mm:ss";
            DeadDurationEdit.Size = new System.Drawing.Size(100, 20);
            DeadDurationEdit.TabIndex = 74;
            // 
            // xtraTabPage8
            // 
            xtraTabPage8.Controls.Add(StrengthLossRateEdit);
            xtraTabPage8.Controls.Add(labelControl64);
            xtraTabPage8.Controls.Add(StrengthAddRateEdit);
            xtraTabPage8.Controls.Add(labelControl65);
            xtraTabPage8.Controls.Add(MaxStrengthEdit);
            xtraTabPage8.Controls.Add(labelControl66);
            xtraTabPage8.Controls.Add(CurseRateEdit);
            xtraTabPage8.Controls.Add(labelControl63);
            xtraTabPage8.Controls.Add(MaxCurseEdit);
            xtraTabPage8.Controls.Add(labelControl62);
            xtraTabPage8.Controls.Add(LuckRateEdit);
            xtraTabPage8.Controls.Add(labelControl61);
            xtraTabPage8.Controls.Add(MaxLuckEdit);
            xtraTabPage8.Controls.Add(labelControl60);
            xtraTabPage8.Controls.Add(labelControl59);
            xtraTabPage8.Controls.Add(SpecialRepairDelayEdit);
            xtraTabPage8.Controls.Add(TorchRateEdit);
            xtraTabPage8.Controls.Add(labelControl54);
            xtraTabPage8.Controls.Add(DropLayersEdit);
            xtraTabPage8.Controls.Add(labelControl50);
            xtraTabPage8.Controls.Add(DropDistanceEdit);
            xtraTabPage8.Controls.Add(labelControl49);
            xtraTabPage8.Controls.Add(labelControl48);
            xtraTabPage8.Controls.Add(DropDurationEdit);
            xtraTabPage8.Name = "xtraTabPage8";
            xtraTabPage8.Size = new System.Drawing.Size(763, 392);
            xtraTabPage8.Text = "Items";
            // 
            // StrengthLossRateEdit
            // 
            StrengthLossRateEdit.Location = new System.Drawing.Point(342, 205);
            StrengthLossRateEdit.MenuManager = ribbon;
            StrengthLossRateEdit.Name = "StrengthLossRateEdit";
            StrengthLossRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            StrengthLossRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            StrengthLossRateEdit.Properties.Mask.EditMask = "n0";
            StrengthLossRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            StrengthLossRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            StrengthLossRateEdit.Size = new System.Drawing.Size(100, 20);
            StrengthLossRateEdit.TabIndex = 100;
            // 
            // labelControl64
            // 
            labelControl64.Location = new System.Drawing.Point(240, 208);
            labelControl64.Name = "labelControl64";
            labelControl64.Size = new System.Drawing.Size(96, 13);
            labelControl64.TabIndex = 99;
            labelControl64.Text = "Strength Loss Rate:";
            // 
            // StrengthAddRateEdit
            // 
            StrengthAddRateEdit.Location = new System.Drawing.Point(342, 179);
            StrengthAddRateEdit.MenuManager = ribbon;
            StrengthAddRateEdit.Name = "StrengthAddRateEdit";
            StrengthAddRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            StrengthAddRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            StrengthAddRateEdit.Properties.Mask.EditMask = "n0";
            StrengthAddRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            StrengthAddRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            StrengthAddRateEdit.Size = new System.Drawing.Size(100, 20);
            StrengthAddRateEdit.TabIndex = 98;
            // 
            // labelControl65
            // 
            labelControl65.Location = new System.Drawing.Point(242, 182);
            labelControl65.Name = "labelControl65";
            labelControl65.Size = new System.Drawing.Size(94, 13);
            labelControl65.TabIndex = 97;
            labelControl65.Text = "Strength Add Rate:";
            // 
            // MaxStrengthEdit
            // 
            MaxStrengthEdit.Location = new System.Drawing.Point(342, 153);
            MaxStrengthEdit.MenuManager = ribbon;
            MaxStrengthEdit.Name = "MaxStrengthEdit";
            MaxStrengthEdit.Properties.Appearance.Options.UseTextOptions = true;
            MaxStrengthEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MaxStrengthEdit.Properties.Mask.EditMask = "n0";
            MaxStrengthEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            MaxStrengthEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MaxStrengthEdit.Size = new System.Drawing.Size(100, 20);
            MaxStrengthEdit.TabIndex = 96;
            // 
            // labelControl66
            // 
            labelControl66.Location = new System.Drawing.Point(267, 156);
            labelControl66.Name = "labelControl66";
            labelControl66.Size = new System.Drawing.Size(69, 13);
            labelControl66.TabIndex = 95;
            labelControl66.Text = "Max Strength:";
            // 
            // CurseRateEdit
            // 
            CurseRateEdit.Location = new System.Drawing.Point(121, 231);
            CurseRateEdit.MenuManager = ribbon;
            CurseRateEdit.Name = "CurseRateEdit";
            CurseRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            CurseRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            CurseRateEdit.Properties.Mask.EditMask = "n0";
            CurseRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            CurseRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            CurseRateEdit.Size = new System.Drawing.Size(100, 20);
            CurseRateEdit.TabIndex = 94;
            // 
            // labelControl63
            // 
            labelControl63.Location = new System.Drawing.Point(57, 234);
            labelControl63.Name = "labelControl63";
            labelControl63.Size = new System.Drawing.Size(58, 13);
            labelControl63.TabIndex = 93;
            labelControl63.Text = "Curse Rate:";
            // 
            // MaxCurseEdit
            // 
            MaxCurseEdit.Location = new System.Drawing.Point(121, 205);
            MaxCurseEdit.MenuManager = ribbon;
            MaxCurseEdit.Name = "MaxCurseEdit";
            MaxCurseEdit.Properties.Appearance.Options.UseTextOptions = true;
            MaxCurseEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MaxCurseEdit.Properties.Mask.EditMask = "n0";
            MaxCurseEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            MaxCurseEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MaxCurseEdit.Size = new System.Drawing.Size(100, 20);
            MaxCurseEdit.TabIndex = 92;
            // 
            // labelControl62
            // 
            labelControl62.Location = new System.Drawing.Point(60, 208);
            labelControl62.Name = "labelControl62";
            labelControl62.Size = new System.Drawing.Size(55, 13);
            labelControl62.TabIndex = 91;
            labelControl62.Text = "Max Curse:";
            // 
            // LuckRateEdit
            // 
            LuckRateEdit.Location = new System.Drawing.Point(121, 179);
            LuckRateEdit.MenuManager = ribbon;
            LuckRateEdit.Name = "LuckRateEdit";
            LuckRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            LuckRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            LuckRateEdit.Properties.Mask.EditMask = "n0";
            LuckRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            LuckRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            LuckRateEdit.Size = new System.Drawing.Size(100, 20);
            LuckRateEdit.TabIndex = 90;
            // 
            // labelControl61
            // 
            labelControl61.Location = new System.Drawing.Point(64, 182);
            labelControl61.Name = "labelControl61";
            labelControl61.Size = new System.Drawing.Size(51, 13);
            labelControl61.TabIndex = 89;
            labelControl61.Text = "Luck Rate:";
            // 
            // MaxLuckEdit
            // 
            MaxLuckEdit.Location = new System.Drawing.Point(121, 153);
            MaxLuckEdit.MenuManager = ribbon;
            MaxLuckEdit.Name = "MaxLuckEdit";
            MaxLuckEdit.Properties.Appearance.Options.UseTextOptions = true;
            MaxLuckEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            MaxLuckEdit.Properties.Mask.EditMask = "n0";
            MaxLuckEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            MaxLuckEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MaxLuckEdit.Size = new System.Drawing.Size(100, 20);
            MaxLuckEdit.TabIndex = 88;
            // 
            // labelControl60
            // 
            labelControl60.Location = new System.Drawing.Point(67, 156);
            labelControl60.Name = "labelControl60";
            labelControl60.Size = new System.Drawing.Size(48, 13);
            labelControl60.TabIndex = 87;
            labelControl60.Text = "Max Luck:";
            // 
            // labelControl59
            // 
            labelControl59.Location = new System.Drawing.Point(14, 130);
            labelControl59.Name = "labelControl59";
            labelControl59.Size = new System.Drawing.Size(101, 13);
            labelControl59.TabIndex = 86;
            labelControl59.Text = "Special Repair Delay:";
            // 
            // SpecialRepairDelayEdit
            // 
            SpecialRepairDelayEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            SpecialRepairDelayEdit.Location = new System.Drawing.Point(121, 127);
            SpecialRepairDelayEdit.MenuManager = ribbon;
            SpecialRepairDelayEdit.Name = "SpecialRepairDelayEdit";
            SpecialRepairDelayEdit.Properties.AllowEditDays = false;
            SpecialRepairDelayEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            SpecialRepairDelayEdit.Properties.Mask.EditMask = "HH:mm:ss";
            SpecialRepairDelayEdit.Size = new System.Drawing.Size(100, 20);
            SpecialRepairDelayEdit.TabIndex = 85;
            // 
            // TorchRateEdit
            // 
            TorchRateEdit.Location = new System.Drawing.Point(121, 101);
            TorchRateEdit.MenuManager = ribbon;
            TorchRateEdit.Name = "TorchRateEdit";
            TorchRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            TorchRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            TorchRateEdit.Properties.Mask.EditMask = "n0";
            TorchRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            TorchRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            TorchRateEdit.Size = new System.Drawing.Size(100, 20);
            TorchRateEdit.TabIndex = 84;
            // 
            // labelControl54
            // 
            labelControl54.Location = new System.Drawing.Point(58, 104);
            labelControl54.Name = "labelControl54";
            labelControl54.Size = new System.Drawing.Size(57, 13);
            labelControl54.TabIndex = 83;
            labelControl54.Text = "Torch Rate:";
            // 
            // DropLayersEdit
            // 
            DropLayersEdit.Location = new System.Drawing.Point(121, 75);
            DropLayersEdit.MenuManager = ribbon;
            DropLayersEdit.Name = "DropLayersEdit";
            DropLayersEdit.Properties.Appearance.Options.UseTextOptions = true;
            DropLayersEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            DropLayersEdit.Properties.Mask.EditMask = "n0";
            DropLayersEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            DropLayersEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            DropLayersEdit.Size = new System.Drawing.Size(100, 20);
            DropLayersEdit.TabIndex = 81;
            // 
            // labelControl50
            // 
            labelControl50.Location = new System.Drawing.Point(53, 78);
            labelControl50.Name = "labelControl50";
            labelControl50.Size = new System.Drawing.Size(62, 13);
            labelControl50.TabIndex = 80;
            labelControl50.Text = "Drop Layers:";
            // 
            // DropDistanceEdit
            // 
            DropDistanceEdit.Location = new System.Drawing.Point(121, 49);
            DropDistanceEdit.MenuManager = ribbon;
            DropDistanceEdit.Name = "DropDistanceEdit";
            DropDistanceEdit.Properties.Appearance.Options.UseTextOptions = true;
            DropDistanceEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            DropDistanceEdit.Properties.Mask.EditMask = "n0";
            DropDistanceEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            DropDistanceEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            DropDistanceEdit.Size = new System.Drawing.Size(100, 20);
            DropDistanceEdit.TabIndex = 79;
            // 
            // labelControl49
            // 
            labelControl49.Location = new System.Drawing.Point(44, 52);
            labelControl49.Name = "labelControl49";
            labelControl49.Size = new System.Drawing.Size(71, 13);
            labelControl49.TabIndex = 78;
            labelControl49.Text = "Drop Distance:";
            // 
            // labelControl48
            // 
            labelControl48.Location = new System.Drawing.Point(44, 26);
            labelControl48.Name = "labelControl48";
            labelControl48.Size = new System.Drawing.Size(71, 13);
            labelControl48.TabIndex = 77;
            labelControl48.Text = "Drop Duration:";
            // 
            // DropDurationEdit
            // 
            DropDurationEdit.EditValue = System.TimeSpan.Parse("00:00:00");
            DropDurationEdit.Location = new System.Drawing.Point(121, 23);
            DropDurationEdit.MenuManager = ribbon;
            DropDurationEdit.Name = "DropDurationEdit";
            DropDurationEdit.Properties.AllowEditDays = false;
            DropDurationEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            DropDurationEdit.Properties.Mask.EditMask = "HH:mm:ss";
            DropDurationEdit.Size = new System.Drawing.Size(100, 20);
            DropDurationEdit.TabIndex = 76;
            // 
            // xtraTabPage9
            // 
            xtraTabPage9.Controls.Add(CompanionRateEdit);
            xtraTabPage9.Controls.Add(labelControl68);
            xtraTabPage9.Controls.Add(SkillRateEdit);
            xtraTabPage9.Controls.Add(labelControl58);
            xtraTabPage9.Controls.Add(GoldRateEdit);
            xtraTabPage9.Controls.Add(labelControl57);
            xtraTabPage9.Controls.Add(DropRateEdit);
            xtraTabPage9.Controls.Add(labelControl56);
            xtraTabPage9.Controls.Add(ExperienceRateEdit);
            xtraTabPage9.Controls.Add(labelControl55);
            xtraTabPage9.Name = "xtraTabPage9";
            xtraTabPage9.Size = new System.Drawing.Size(763, 392);
            xtraTabPage9.Text = "Rates";
            // 
            // CompanionRateEdit
            // 
            CompanionRateEdit.Location = new System.Drawing.Point(108, 125);
            CompanionRateEdit.MenuManager = ribbon;
            CompanionRateEdit.Name = "CompanionRateEdit";
            CompanionRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            CompanionRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            CompanionRateEdit.Properties.Mask.EditMask = "n0";
            CompanionRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            CompanionRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            CompanionRateEdit.Size = new System.Drawing.Size(100, 20);
            CompanionRateEdit.TabIndex = 89;
            // 
            // labelControl68
            // 
            labelControl68.Location = new System.Drawing.Point(19, 128);
            labelControl68.Name = "labelControl68";
            labelControl68.Size = new System.Drawing.Size(83, 13);
            labelControl68.TabIndex = 88;
            labelControl68.Text = "Companion Rate:";
            // 
            // SkillRateEdit
            // 
            SkillRateEdit.Location = new System.Drawing.Point(108, 99);
            SkillRateEdit.MenuManager = ribbon;
            SkillRateEdit.Name = "SkillRateEdit";
            SkillRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            SkillRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            SkillRateEdit.Properties.Mask.EditMask = "n0";
            SkillRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            SkillRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            SkillRateEdit.Size = new System.Drawing.Size(100, 20);
            SkillRateEdit.TabIndex = 87;
            // 
            // labelControl58
            // 
            labelControl58.Location = new System.Drawing.Point(55, 102);
            labelControl58.Name = "labelControl58";
            labelControl58.Size = new System.Drawing.Size(47, 13);
            labelControl58.TabIndex = 86;
            labelControl58.Text = "Skill Rate:";
            // 
            // GoldRateEdit
            // 
            GoldRateEdit.Location = new System.Drawing.Point(108, 73);
            GoldRateEdit.MenuManager = ribbon;
            GoldRateEdit.Name = "GoldRateEdit";
            GoldRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            GoldRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            GoldRateEdit.Properties.Mask.EditMask = "n0";
            GoldRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            GoldRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            GoldRateEdit.Size = new System.Drawing.Size(100, 20);
            GoldRateEdit.TabIndex = 85;
            // 
            // labelControl57
            // 
            labelControl57.Location = new System.Drawing.Point(51, 76);
            labelControl57.Name = "labelControl57";
            labelControl57.Size = new System.Drawing.Size(51, 13);
            labelControl57.TabIndex = 84;
            labelControl57.Text = "Gold Rate:";
            // 
            // DropRateEdit
            // 
            DropRateEdit.Location = new System.Drawing.Point(108, 47);
            DropRateEdit.MenuManager = ribbon;
            DropRateEdit.Name = "DropRateEdit";
            DropRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            DropRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            DropRateEdit.Properties.Mask.EditMask = "n0";
            DropRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            DropRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            DropRateEdit.Size = new System.Drawing.Size(100, 20);
            DropRateEdit.TabIndex = 83;
            // 
            // labelControl56
            // 
            labelControl56.Location = new System.Drawing.Point(49, 50);
            labelControl56.Name = "labelControl56";
            labelControl56.Size = new System.Drawing.Size(53, 13);
            labelControl56.TabIndex = 82;
            labelControl56.Text = "Drop Rate:";
            // 
            // ExperienceRateEdit
            // 
            ExperienceRateEdit.Location = new System.Drawing.Point(108, 21);
            ExperienceRateEdit.MenuManager = ribbon;
            ExperienceRateEdit.Name = "ExperienceRateEdit";
            ExperienceRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            ExperienceRateEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ExperienceRateEdit.Properties.Mask.EditMask = "n0";
            ExperienceRateEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            ExperienceRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            ExperienceRateEdit.Size = new System.Drawing.Size(100, 20);
            ExperienceRateEdit.TabIndex = 81;
            // 
            // labelControl55
            // 
            labelControl55.Location = new System.Drawing.Point(19, 24);
            labelControl55.Name = "labelControl55";
            labelControl55.Size = new System.Drawing.Size(83, 13);
            labelControl55.TabIndex = 80;
            labelControl55.Text = "Experience Rate:";
            // 
            // OpenDialog
            // 
            OpenDialog.FileName = "Zircon.exe";
            OpenDialog.Filter = "Zircon Client|Zircon.exe|All Files|*.*";
            // 
            // FolderDialog
            // 
            FolderDialog.SelectedPath = ".\\";
            // 
            // EnableFortuneEdit
            // 
            EnableFortuneEdit.Location = new System.Drawing.Point(429, 218);
            EnableFortuneEdit.MenuManager = ribbon;
            EnableFortuneEdit.Name = "EnableFortuneEdit";
            EnableFortuneEdit.Properties.Caption = "";
            EnableFortuneEdit.Size = new System.Drawing.Size(100, 19);
            EnableFortuneEdit.TabIndex = 131;
            // 
            // labelControl91
            // 
            labelControl91.Location = new System.Drawing.Point(346, 221);
            labelControl91.Name = "labelControl91";
            labelControl91.Size = new System.Drawing.Size(77, 13);
            labelControl91.TabIndex = 132;
            labelControl91.Text = "Enable Fortune:";
            // 
            // ConfigView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(769, 564);
            Controls.Add(xtraTabControl1);
            Controls.Add(ribbon);
            Name = "ConfigView";
            Ribbon = ribbon;
            Text = "Config";
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)xtraTabControl1).EndInit();
            xtraTabControl1.ResumeLayout(false);
            xtraTabPage1.ResumeLayout(false);
            xtraTabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PacketBanTimeEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxPacketEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)UserCountPortEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)PingDelayEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)TimeOutEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)PortEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)IPAddressEdit.Properties).EndInit();
            xtraTabPage2.ResumeLayout(false);
            xtraTabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AllowRequestActivationEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowWebActivationEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowManualActivationEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowDeleteAccountEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowManualResetPasswordEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowWebResetPasswordEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowRequestPasswordResetEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowWizardEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowTaoistEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowAssassinEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowWarriorEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)RelogDelayEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowStartGameEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowDeleteCharacterEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowNewCharacterEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowLoginEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowChangePasswordEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowNewAccountEdit.Properties).EndInit();
            xtraTabPage3.ResumeLayout(false);
            xtraTabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)RabbitEventEndEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ReleaseDateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ClientPathEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MasterPasswordEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapPathEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DBSaveDelayEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)VersionPathEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)CheckVersionEdit.Properties).EndInit();
            xtraTabPage4.ResumeLayout(false);
            xtraTabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MailDisplayNameEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MailFromEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MailPasswordEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MailAccountEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MailUseSSLEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MailPortEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MailServerEdit.Properties).EndInit();
            xtraTabPage5.ResumeLayout(false);
            xtraTabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AllowBuyGameGoldEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ProcessGameGoldEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ReceiverEMailEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)IPNPrefixEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)BuyAddressEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)BuyPrefixEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DeleteFailLinkEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DeleteSuccessLinkEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ResetFailLinkEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ResetSuccessLinkEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ActivationFailLinkEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ActivationSuccessLinkEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)WebCommandLinkEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)WebPrefixEdit.Properties).EndInit();
            xtraTabPage6.ResumeLayout(false);
            xtraTabPage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)EnableHermitEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)EnableStruckEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AutoReviveDelayEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)PvPCurseRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)PvPCurseDurationEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)RedPointEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)PKPointTickRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)PKPointRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)BrownDurationEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)AllowObservationEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)SkillExpEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DayCycleCountEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxLevelEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)GlobalDelayEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ShoutDelayEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxViewRangeEdit.Properties).EndInit();
            xtraTabPage7.ResumeLayout(false);
            xtraTabPage7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LairRegionIndexEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MysteryShipRegionIndexEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)HarvestDurationEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DeadDurationEdit.Properties).EndInit();
            xtraTabPage8.ResumeLayout(false);
            xtraTabPage8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)StrengthLossRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)StrengthAddRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxStrengthEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)CurseRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxCurseEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)LuckRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxLuckEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)SpecialRepairDelayEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)TorchRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DropLayersEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DropDistanceEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DropDurationEdit.Properties).EndInit();
            xtraTabPage9.ResumeLayout(false);
            xtraTabPage9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)CompanionRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)SkillRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)GoldRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)DropRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)ExperienceRateEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)EnableFortuneEdit.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraBars.BarButtonItem ReloadButton;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraEditors.LabelControl labelControl51;
        private DevExpress.XtraEditors.TextEdit UserCountPortEdit;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TimeSpanEdit PingDelayEdit;
        private DevExpress.XtraEditors.TimeSpanEdit TimeOutEdit;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit PortEdit;
        private DevExpress.XtraEditors.TextEdit IPAddressEdit;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.LabelControl labelControl40;
        private DevExpress.XtraEditors.CheckEdit AllowWizardEdit;
        private DevExpress.XtraEditors.LabelControl labelControl39;
        private DevExpress.XtraEditors.CheckEdit AllowTaoistEdit;
        private DevExpress.XtraEditors.LabelControl labelControl38;
        private DevExpress.XtraEditors.CheckEdit AllowAssassinEdit;
        private DevExpress.XtraEditors.LabelControl labelControl36;
        private DevExpress.XtraEditors.CheckEdit AllowWarriorEdit;
        private DevExpress.XtraEditors.LabelControl labelControl15;
        private DevExpress.XtraEditors.TimeSpanEdit RelogDelayEdit;
        private DevExpress.XtraEditors.LabelControl labelControl14;
        private DevExpress.XtraEditors.CheckEdit AllowStartGameEdit;
        private DevExpress.XtraEditors.LabelControl labelControl12;
        private DevExpress.XtraEditors.CheckEdit AllowDeleteCharacterEdit;
        private DevExpress.XtraEditors.LabelControl labelControl11;
        private DevExpress.XtraEditors.CheckEdit AllowNewCharacterEdit;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.CheckEdit AllowLoginEdit;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.CheckEdit AllowChangePasswordEdit;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.CheckEdit AllowNewAccountEdit;
        private DevExpress.XtraEditors.XtraOpenFileDialog OpenDialog;
        private DevExpress.XtraEditors.XtraFolderBrowserDialog FolderDialog;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage4;
        private DevExpress.XtraEditors.LabelControl labelControl16;
        private DevExpress.XtraEditors.CheckEdit AllowRequestActivationEdit;
        private DevExpress.XtraEditors.LabelControl labelControl22;
        private DevExpress.XtraEditors.CheckEdit AllowWebActivationEdit;
        private DevExpress.XtraEditors.LabelControl labelControl17;
        private DevExpress.XtraEditors.CheckEdit AllowManualActivationEdit;
        private DevExpress.XtraEditors.LabelControl labelControl18;
        private DevExpress.XtraEditors.CheckEdit AllowDeleteAccountEdit;
        private DevExpress.XtraEditors.LabelControl labelControl19;
        private DevExpress.XtraEditors.CheckEdit AllowManualResetPasswordEdit;
        private DevExpress.XtraEditors.LabelControl labelControl20;
        private DevExpress.XtraEditors.CheckEdit AllowWebResetPasswordEdit;
        private DevExpress.XtraEditors.LabelControl labelControl21;
        private DevExpress.XtraEditors.CheckEdit AllowRequestPasswordResetEdit;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage5;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage6;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage7;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
        private DevExpress.XtraEditors.ButtonEdit MapPathEdit;
        private DevExpress.XtraEditors.LabelControl labelControl13;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.TimeSpanEdit DBSaveDelayEdit;
        private DevExpress.XtraEditors.SimpleButton CheckVersionButton;
        private DevExpress.XtraEditors.ButtonEdit VersionPathEdit;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.CheckEdit CheckVersionEdit;
        private DevExpress.XtraEditors.TextEdit MailDisplayNameEdit;
        private DevExpress.XtraEditors.LabelControl labelControl31;
        private DevExpress.XtraEditors.TextEdit MailFromEdit;
        private DevExpress.XtraEditors.LabelControl labelControl30;
        private DevExpress.XtraEditors.TextEdit MailPasswordEdit;
        private DevExpress.XtraEditors.LabelControl labelControl29;
        private DevExpress.XtraEditors.TextEdit MailAccountEdit;
        private DevExpress.XtraEditors.LabelControl labelControl28;
        private DevExpress.XtraEditors.LabelControl labelControl27;
        private DevExpress.XtraEditors.CheckEdit MailUseSSLEdit;
        private DevExpress.XtraEditors.LabelControl labelControl25;
        private DevExpress.XtraEditors.TextEdit MailPortEdit;
        private DevExpress.XtraEditors.TextEdit MailServerEdit;
        private DevExpress.XtraEditors.LabelControl labelControl26;
        private DevExpress.XtraEditors.LabelControl labelControl23;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage8;
        private DevExpress.XtraEditors.TextEdit ActivationFailLinkEdit;
        private DevExpress.XtraEditors.LabelControl labelControl34;
        private DevExpress.XtraEditors.TextEdit ActivationSuccessLinkEdit;
        private DevExpress.XtraEditors.LabelControl labelControl35;
        private DevExpress.XtraEditors.LabelControl labelControl41;
        private DevExpress.XtraEditors.TextEdit WebCommandLinkEdit;
        private DevExpress.XtraEditors.TextEdit WebPrefixEdit;
        private DevExpress.XtraEditors.LabelControl labelControl42;
        private DevExpress.XtraEditors.TextEdit DeleteFailLinkEdit;
        private DevExpress.XtraEditors.LabelControl labelControl37;
        private DevExpress.XtraEditors.TextEdit DeleteSuccessLinkEdit;
        private DevExpress.XtraEditors.LabelControl labelControl43;
        private DevExpress.XtraEditors.TextEdit ResetFailLinkEdit;
        private DevExpress.XtraEditors.LabelControl labelControl32;
        private DevExpress.XtraEditors.TextEdit ResetSuccessLinkEdit;
        private DevExpress.XtraEditors.LabelControl labelControl33;
        private DevExpress.XtraEditors.TextEdit MaxLevelEdit;
        private DevExpress.XtraEditors.LabelControl labelControl46;
        private DevExpress.XtraEditors.LabelControl labelControl45;
        private DevExpress.XtraEditors.TimeSpanEdit GlobalDelayEdit;
        private DevExpress.XtraEditors.LabelControl labelControl44;
        private DevExpress.XtraEditors.TimeSpanEdit ShoutDelayEdit;
        private DevExpress.XtraEditors.TextEdit MaxViewRangeEdit;
        private DevExpress.XtraEditors.LabelControl labelControl47;
        private DevExpress.XtraEditors.TimeSpanEdit DeadDurationEdit;
        private DevExpress.XtraEditors.TextEdit DropDistanceEdit;
        private DevExpress.XtraEditors.LabelControl labelControl49;
        private DevExpress.XtraEditors.LabelControl labelControl48;
        private DevExpress.XtraEditors.TimeSpanEdit DropDurationEdit;
        private DevExpress.XtraEditors.TextEdit DropLayersEdit;
        private DevExpress.XtraEditors.LabelControl labelControl50;
        private DevExpress.XtraEditors.TextEdit DayCycleCountEdit;
        private DevExpress.XtraEditors.LabelControl labelControl52;
        private DevExpress.XtraEditors.TextEdit SkillExpEdit;
        private DevExpress.XtraEditors.LabelControl labelControl53;
        private DevExpress.XtraEditors.TextEdit TorchRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl54;
        private DevExpress.XtraEditors.LabelControl labelControl59;
        private DevExpress.XtraEditors.TimeSpanEdit SpecialRepairDelayEdit;
        private DevExpress.XtraEditors.TextEdit CurseRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl63;
        private DevExpress.XtraEditors.TextEdit MaxCurseEdit;
        private DevExpress.XtraEditors.LabelControl labelControl62;
        private DevExpress.XtraEditors.TextEdit LuckRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl61;
        private DevExpress.XtraEditors.TextEdit MaxLuckEdit;
        private DevExpress.XtraEditors.LabelControl labelControl60;
        private DevExpress.XtraEditors.TextEdit StrengthLossRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl64;
        private DevExpress.XtraEditors.TextEdit StrengthAddRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl65;
        private DevExpress.XtraEditors.TextEdit MaxStrengthEdit;
        private DevExpress.XtraEditors.LabelControl labelControl66;
        private DevExpress.XtraEditors.TextEdit MasterPasswordEdit;
        private DevExpress.XtraEditors.LabelControl labelControl67;
        private DevExpress.XtraEditors.LabelControl labelControl24;
        private DevExpress.XtraEditors.CheckEdit AllowObservationEdit;
        private DevExpress.XtraEditors.LabelControl labelControl74;
        private DevExpress.XtraEditors.TimeSpanEdit HarvestDurationEdit;
        private DevExpress.XtraEditors.LabelControl labelControl75;
        private DevExpress.XtraEditors.TimeSpanEdit BrownDurationEdit;
        private DevExpress.XtraEditors.TextEdit PvPCurseRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl83;
        private DevExpress.XtraEditors.LabelControl labelControl84;
        private DevExpress.XtraEditors.TimeSpanEdit PvPCurseDurationEdit;
        private DevExpress.XtraEditors.TextEdit RedPointEdit;
        private DevExpress.XtraEditors.LabelControl labelControl77;
        private DevExpress.XtraEditors.LabelControl labelControl78;
        private DevExpress.XtraEditors.TimeSpanEdit PKPointTickRateEdit;
        private DevExpress.XtraEditors.TextEdit PKPointRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl76;
        private DevExpress.XtraEditors.LabelControl labelControl89;
        private DevExpress.XtraEditors.LookUpEdit MysteryShipRegionIndexEdit;
        private DevExpress.XtraEditors.ButtonEdit ClientPathEdit;
        private DevExpress.XtraEditors.LabelControl labelControl96;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage9;
        private DevExpress.XtraEditors.TextEdit CompanionRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl68;
        private DevExpress.XtraEditors.TextEdit SkillRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl58;
        private DevExpress.XtraEditors.TextEdit GoldRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl57;
        private DevExpress.XtraEditors.TextEdit DropRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl56;
        private DevExpress.XtraEditors.LabelControl labelControl55;
        private DevExpress.XtraEditors.TextEdit ExperienceRateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl69;
        private DevExpress.XtraEditors.TimeSpanEdit AutoReviveDelayEdit;
        private DevExpress.XtraEditors.TextEdit ReleaseDateEdit;
        private DevExpress.XtraEditors.LabelControl labelControl70;
        private DevExpress.XtraEditors.TextEdit BuyPrefixEdit;
        private DevExpress.XtraEditors.LabelControl labelControl71;
        private DevExpress.XtraEditors.LabelControl labelControl81;
        private DevExpress.XtraEditors.CheckEdit AllowBuyGameGoldEdit;
        private DevExpress.XtraEditors.LabelControl labelControl80;
        private DevExpress.XtraEditors.CheckEdit ProcessGameGoldEdit;
        private DevExpress.XtraEditors.TextEdit ReceiverEMailEdit;
        private DevExpress.XtraEditors.LabelControl labelControl79;
        private DevExpress.XtraEditors.TextEdit IPNPrefixEdit;
        private DevExpress.XtraEditors.LabelControl labelControl73;
        private DevExpress.XtraEditors.TextEdit BuyAddressEdit;
        private DevExpress.XtraEditors.LabelControl labelControl72;
        private DevExpress.XtraEditors.LookUpEdit LairRegionIndexEdit;
        private DevExpress.XtraEditors.LabelControl labelControl82;
        private DevExpress.XtraEditors.TextEdit RabbitEventEndEdit;
        private DevExpress.XtraEditors.LabelControl labelControl85;
        private DevExpress.XtraEditors.TimeSpanEdit PacketBanTimeEdit;
        private DevExpress.XtraEditors.SimpleButton SyncronizeButton;
        private DevExpress.XtraEditors.SimpleButton DatabaseEncryptionButton;
        private DevExpress.XtraEditors.LabelControl labelControl86;
        private DevExpress.XtraEditors.LabelControl labelControl87;
        private DevExpress.XtraEditors.TextEdit MaxPacketEdit;
        private DevExpress.XtraEditors.LabelControl labelControl88;
        private DevExpress.XtraEditors.CheckEdit EnableStruckEdit;
        private DevExpress.XtraEditors.LabelControl labelControl90;
        private DevExpress.XtraEditors.CheckEdit EnableHermitEdit;
        private DevExpress.XtraEditors.LabelControl labelControl91;
        private DevExpress.XtraEditors.CheckEdit EnableFortuneEdit;
    }
}