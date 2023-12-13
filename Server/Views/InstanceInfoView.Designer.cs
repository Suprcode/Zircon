namespace Server.Views
{
    partial class InstanceInfoView
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstanceInfoView));
            InstanceMapGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            MapInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            InstanceInfoGridControl = new DevExpress.XtraGrid.GridControl();
            InstanceInfoStatsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colStat = new DevExpress.XtraGrid.Columns.GridColumn();
            StatComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            InstanceInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            InstanceTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            gridColumn19 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)InstanceMapGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapInfoLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)InstanceInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)InstanceInfoStatsGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StatComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)InstanceInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RegionLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)InstanceTypeImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            SuspendLayout();
            // 
            // InstanceMapGridView
            // 
            InstanceMapGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn2, gridColumn19 });
            InstanceMapGridView.GridControl = InstanceInfoGridControl;
            InstanceMapGridView.Name = "InstanceMapGridView";
            InstanceMapGridView.OptionsView.EnableAppearanceEvenRow = true;
            InstanceMapGridView.OptionsView.EnableAppearanceOddRow = true;
            InstanceMapGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            InstanceMapGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            InstanceMapGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn2
            // 
            gridColumn2.ColumnEdit = MapInfoLookUpEdit;
            gridColumn2.FieldName = "Map";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            gridColumn2.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 0;
            // 
            // MapInfoLookUpEdit
            // 
            MapInfoLookUpEdit.AutoHeight = false;
            MapInfoLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            MapInfoLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            MapInfoLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FileName", "File Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Description") });
            MapInfoLookUpEdit.DisplayMember = "ServerDescription";
            MapInfoLookUpEdit.Name = "MapInfoLookUpEdit";
            MapInfoLookUpEdit.NullText = "[Map is null]";
            // 
            // InstanceInfoGridControl
            // 
            InstanceInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = InstanceMapGridView;
            gridLevelNode1.RelationName = "Maps";
            gridLevelNode2.LevelTemplate = InstanceInfoStatsGridView;
            gridLevelNode2.RelationName = "BuffStats";
            InstanceInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1, gridLevelNode2 });
            InstanceInfoGridControl.Location = new System.Drawing.Point(0, 144);
            InstanceInfoGridControl.MainView = InstanceInfoGridView;
            InstanceInfoGridControl.MenuManager = ribbon;
            InstanceInfoGridControl.Name = "InstanceInfoGridControl";
            InstanceInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { MapInfoLookUpEdit, RegionLookUpEdit, InstanceTypeImageComboBox, StatComboBox, ItemLookUpEdit });
            InstanceInfoGridControl.ShowOnlyPredefinedDetails = true;
            InstanceInfoGridControl.Size = new System.Drawing.Size(1031, 335);
            InstanceInfoGridControl.TabIndex = 2;
            InstanceInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { InstanceInfoStatsGridView, InstanceInfoGridView, InstanceMapGridView });
            // 
            // InstanceInfoStatsGridView
            // 
            InstanceInfoStatsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colStat, colAmount });
            InstanceInfoStatsGridView.GridControl = InstanceInfoGridControl;
            InstanceInfoStatsGridView.Name = "InstanceInfoStatsGridView";
            InstanceInfoStatsGridView.OptionsView.EnableAppearanceEvenRow = true;
            InstanceInfoStatsGridView.OptionsView.EnableAppearanceOddRow = true;
            InstanceInfoStatsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            InstanceInfoStatsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            InstanceInfoStatsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colStat
            // 
            colStat.ColumnEdit = StatComboBox;
            colStat.FieldName = "Stat";
            colStat.Name = "colStat";
            colStat.Visible = true;
            colStat.VisibleIndex = 0;
            // 
            // StatComboBox
            // 
            StatComboBox.AutoHeight = false;
            StatComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            StatComboBox.Name = "StatComboBox";
            // 
            // colAmount
            // 
            colAmount.FieldName = "Amount";
            colAmount.Name = "colAmount";
            colAmount.Visible = true;
            colAmount.VisibleIndex = 1;
            // 
            // InstanceInfoGridView
            // 
            InstanceInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1, gridColumn6, gridColumn5, gridColumn17, gridColumn18, gridColumn3, gridColumn4, gridColumn7, gridColumn8, gridColumn10, gridColumn9, gridColumn11, gridColumn12, gridColumn13, gridColumn14, gridColumn15, gridColumn16 });
            InstanceInfoGridView.GridControl = InstanceInfoGridControl;
            InstanceInfoGridView.Name = "InstanceInfoGridView";
            InstanceInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            InstanceInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            InstanceInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            InstanceInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            InstanceInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            InstanceInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            gridColumn1.Caption = "Name";
            gridColumn1.FieldName = "Name";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn6
            // 
            gridColumn6.Caption = "Max Instances";
            gridColumn6.FieldName = "MaxInstances";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 2;
            // 
            // gridColumn5
            // 
            gridColumn5.Caption = "Show On Dungeon Finder";
            gridColumn5.FieldName = "ShowOnDungeonFinder";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 3;
            // 
            // gridColumn17
            // 
            gridColumn17.Caption = "Allow Rejoin";
            gridColumn17.FieldName = "AllowRejoin";
            gridColumn17.Name = "gridColumn17";
            gridColumn17.Visible = true;
            gridColumn17.VisibleIndex = 5;
            // 
            // gridColumn18
            // 
            gridColumn18.Caption = "Save Place";
            gridColumn18.FieldName = "SavePlace";
            gridColumn18.Name = "gridColumn18";
            gridColumn18.Visible = true;
            gridColumn18.VisibleIndex = 6;
            // 
            // gridColumn3
            // 
            gridColumn3.Caption = "Min Level";
            gridColumn3.FieldName = "MinPlayerLevel";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 7;
            // 
            // gridColumn4
            // 
            gridColumn4.Caption = "Max Level";
            gridColumn4.FieldName = "MaxPlayerLevel";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 8;
            // 
            // gridColumn7
            // 
            gridColumn7.Caption = "Min Player";
            gridColumn7.FieldName = "MinPlayerCount";
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 9;
            // 
            // gridColumn8
            // 
            gridColumn8.Caption = "Max Player";
            gridColumn8.FieldName = "MaxPlayerCount";
            gridColumn8.Name = "gridColumn8";
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 10;
            // 
            // gridColumn10
            // 
            gridColumn10.Caption = "Connect Region";
            gridColumn10.ColumnEdit = RegionLookUpEdit;
            gridColumn10.FieldName = "ConnectRegion";
            gridColumn10.Name = "gridColumn10";
            gridColumn10.Visible = true;
            gridColumn10.VisibleIndex = 15;
            // 
            // RegionLookUpEdit
            // 
            RegionLookUpEdit.AutoHeight = false;
            RegionLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            RegionLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RegionLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Server Description"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size") });
            RegionLookUpEdit.DisplayMember = "ServerDescription";
            RegionLookUpEdit.Name = "RegionLookUpEdit";
            RegionLookUpEdit.NullText = "[Region is null]";
            // 
            // gridColumn9
            // 
            gridColumn9.Caption = "Reconnect Region";
            gridColumn9.ColumnEdit = RegionLookUpEdit;
            gridColumn9.FieldName = "ReconnectRegion";
            gridColumn9.Name = "gridColumn9";
            gridColumn9.Visible = true;
            gridColumn9.VisibleIndex = 16;
            // 
            // gridColumn11
            // 
            gridColumn11.Caption = "Type";
            gridColumn11.ColumnEdit = InstanceTypeImageComboBox;
            gridColumn11.FieldName = "Type";
            gridColumn11.Name = "gridColumn11";
            gridColumn11.Visible = true;
            gridColumn11.VisibleIndex = 1;
            // 
            // InstanceTypeImageComboBox
            // 
            InstanceTypeImageComboBox.AutoHeight = false;
            InstanceTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            InstanceTypeImageComboBox.Name = "InstanceTypeImageComboBox";
            // 
            // gridColumn12
            // 
            gridColumn12.Caption = "Cooldown (In Minutes)";
            gridColumn12.FieldName = "CooldownTimeInMinutes";
            gridColumn12.Name = "gridColumn12";
            gridColumn12.Visible = true;
            gridColumn12.VisibleIndex = 13;
            // 
            // gridColumn13
            // 
            gridColumn13.Caption = "Join In SafeZone Only";
            gridColumn13.FieldName = "SafeZoneOnly";
            gridColumn13.Name = "gridColumn13";
            gridColumn13.Visible = true;
            gridColumn13.VisibleIndex = 4;
            // 
            // gridColumn14
            // 
            gridColumn14.Caption = "Required Item";
            gridColumn14.ColumnEdit = ItemLookUpEdit;
            gridColumn14.FieldName = "RequiredItem";
            gridColumn14.Name = "gridColumn14";
            gridColumn14.Visible = true;
            gridColumn14.VisibleIndex = 11;
            // 
            // ItemLookUpEdit
            // 
            ItemLookUpEdit.AutoHeight = false;
            ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "ItemType") });
            ItemLookUpEdit.DisplayMember = "ItemName";
            ItemLookUpEdit.Name = "ItemLookUpEdit";
            ItemLookUpEdit.NullText = "[Item is Null]";
            // 
            // gridColumn15
            // 
            gridColumn15.Caption = "Single Use Item";
            gridColumn15.FieldName = "RequiredItemSingleUse";
            gridColumn15.Name = "gridColumn15";
            gridColumn15.Visible = true;
            gridColumn15.VisibleIndex = 12;
            // 
            // gridColumn16
            // 
            gridColumn16.Caption = "Time Limit (In Minutes)";
            gridColumn16.FieldName = "TimeLimitInMinutes";
            gridColumn16.Name = "gridColumn16";
            gridColumn16.Visible = true;
            gridColumn16.VisibleIndex = 14;
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveDatabaseButton, ImportButton, ExportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(1031, 144);
            // 
            // SaveDatabaseButton
            // 
            SaveDatabaseButton.Caption = "Save Database";
            SaveDatabaseButton.Id = 1;
            SaveDatabaseButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SaveDatabaseButton.ImageOptions.Image");
            SaveDatabaseButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SaveDatabaseButton.ImageOptions.LargeImage");
            SaveDatabaseButton.LargeWidth = 60;
            SaveDatabaseButton.Name = "SaveDatabaseButton";
            SaveDatabaseButton.ItemClick += SaveDatabaseButton_ItemClick;
            // 
            // ImportButton
            // 
            ImportButton.Caption = "Import";
            ImportButton.Id = 2;
            ImportButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ImportButton.ImageOptions.Image");
            ImportButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ImportButton.ImageOptions.LargeImage");
            ImportButton.Name = "ImportButton";
            ImportButton.ItemClick += ImportButton_ItemClick;
            // 
            // ExportButton
            // 
            ExportButton.Caption = "Export";
            ExportButton.Id = 3;
            ExportButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ExportButton.ImageOptions.Image");
            ExportButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ExportButton.ImageOptions.LargeImage");
            ExportButton.Name = "ExportButton";
            ExportButton.ItemClick += ExportButton_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, JsonImportExport });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(SaveDatabaseButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Saving";
            // 
            // JsonImportExport
            // 
            JsonImportExport.ItemLinks.Add(ImportButton);
            JsonImportExport.ItemLinks.Add(ExportButton);
            JsonImportExport.Name = "JsonImportExport";
            JsonImportExport.Text = "Json";
            // 
            // gridColumn19
            // 
            gridColumn19.FieldName = "RespawnIndex";
            gridColumn19.Name = "gridColumn19";
            gridColumn19.Visible = true;
            gridColumn19.VisibleIndex = 1;
            // 
            // InstanceInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1031, 479);
            Controls.Add(InstanceInfoGridControl);
            Controls.Add(ribbon);
            Name = "InstanceInfoView";
            Ribbon = ribbon;
            Text = "Instance Info";
            ((System.ComponentModel.ISupportInitialize)InstanceMapGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapInfoLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)InstanceInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)InstanceInfoStatsGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)StatComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)InstanceInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)RegionLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)InstanceTypeImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveDatabaseButton;
        private DevExpress.XtraGrid.GridControl InstanceInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView InstanceMapGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Views.Grid.GridView InstanceInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MapInfoLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit RegionLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox InstanceTypeImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Views.Grid.GridView InstanceInfoStatsGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colStat;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn18;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn19;
    }
}