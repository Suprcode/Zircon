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
            this.InstanceMapGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MapInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.InstanceInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.InstanceInfoStatsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.StatComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.InstanceInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.InstanceTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceMapGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapInfoLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceInfoStatsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceTypeImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.SuspendLayout();
            // 
            // InstanceMapGridView
            // 
            this.InstanceMapGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2});
            this.InstanceMapGridView.GridControl = this.InstanceInfoGridControl;
            this.InstanceMapGridView.Name = "InstanceMapGridView";
            this.InstanceMapGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.InstanceMapGridView.OptionsView.EnableAppearanceOddRow = true;
            this.InstanceMapGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.InstanceMapGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.InstanceMapGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn2
            // 
            this.gridColumn2.ColumnEdit = this.MapInfoLookUpEdit;
            this.gridColumn2.FieldName = "Map";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn2.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // MapInfoLookUpEdit
            // 
            this.MapInfoLookUpEdit.AutoHeight = false;
            this.MapInfoLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.MapInfoLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MapInfoLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FileName", "File Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Description")});
            this.MapInfoLookUpEdit.DisplayMember = "ServerDescription";
            this.MapInfoLookUpEdit.Name = "MapInfoLookUpEdit";
            this.MapInfoLookUpEdit.NullText = "[Map is null]";
            // 
            // InstanceInfoGridControl
            // 
            this.InstanceInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = this.InstanceMapGridView;
            gridLevelNode1.RelationName = "Maps";
            gridLevelNode2.LevelTemplate = this.InstanceInfoStatsGridView;
            gridLevelNode2.RelationName = "BuffStats";
            this.InstanceInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1,
            gridLevelNode2});
            this.InstanceInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.InstanceInfoGridControl.MainView = this.InstanceInfoGridView;
            this.InstanceInfoGridControl.MenuManager = this.ribbon;
            this.InstanceInfoGridControl.Name = "InstanceInfoGridControl";
            this.InstanceInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MapInfoLookUpEdit,
            this.RegionLookUpEdit,
            this.InstanceTypeImageComboBox,
            this.StatComboBox});
            this.InstanceInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.InstanceInfoGridControl.Size = new System.Drawing.Size(713, 335);
            this.InstanceInfoGridControl.TabIndex = 2;
            this.InstanceInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.InstanceInfoStatsGridView,
            this.InstanceInfoGridView,
            this.InstanceMapGridView});
            // 
            // InstanceInfoStatsGridView
            // 
            this.InstanceInfoStatsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colStat,
            this.colAmount});
            this.InstanceInfoStatsGridView.GridControl = this.InstanceInfoGridControl;
            this.InstanceInfoStatsGridView.Name = "InstanceInfoStatsGridView";
            this.InstanceInfoStatsGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.InstanceInfoStatsGridView.OptionsView.EnableAppearanceOddRow = true;
            this.InstanceInfoStatsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.InstanceInfoStatsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.InstanceInfoStatsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colStat
            // 
            this.colStat.ColumnEdit = this.StatComboBox;
            this.colStat.FieldName = "Stat";
            this.colStat.Name = "colStat";
            this.colStat.Visible = true;
            this.colStat.VisibleIndex = 0;
            // 
            // StatComboBox
            // 
            this.StatComboBox.AutoHeight = false;
            this.StatComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.StatComboBox.Name = "StatComboBox";
            // 
            // colAmount
            // 
            this.colAmount.FieldName = "Amount";
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 1;
            // 
            // InstanceInfoGridView
            // 
            this.InstanceInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn6,
            this.gridColumn5,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn10,
            this.gridColumn9,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13});
            this.InstanceInfoGridView.GridControl = this.InstanceInfoGridControl;
            this.InstanceInfoGridView.Name = "InstanceInfoGridView";
            this.InstanceInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.InstanceInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.InstanceInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.InstanceInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.InstanceInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.InstanceInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Name";
            this.gridColumn1.FieldName = "Name";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Max Instances";
            this.gridColumn6.FieldName = "MaxInstances";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 2;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Show On Dungeon Finder";
            this.gridColumn5.FieldName = "ShowOnDungeonFinder";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Min Level";
            this.gridColumn3.FieldName = "MinPlayerLevel";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 5;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Max Level";
            this.gridColumn4.FieldName = "MaxPlayerLevel";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 6;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Min Group";
            this.gridColumn7.FieldName = "MinPlayerCount";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 7;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Max Group";
            this.gridColumn8.FieldName = "MaxPlayerCount";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 8;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Connect Region";
            this.gridColumn10.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn10.FieldName = "ConnectRegion";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 10;
            // 
            // RegionLookUpEdit
            // 
            this.RegionLookUpEdit.AutoHeight = false;
            this.RegionLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.RegionLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RegionLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Server Description"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size")});
            this.RegionLookUpEdit.DisplayMember = "ServerDescription";
            this.RegionLookUpEdit.Name = "RegionLookUpEdit";
            this.RegionLookUpEdit.NullText = "[Region is null]";
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Reconnect Region";
            this.gridColumn9.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn9.FieldName = "ReconnectRegion";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 11;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Type";
            this.gridColumn11.ColumnEdit = this.InstanceTypeImageComboBox;
            this.gridColumn11.FieldName = "Type";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 1;
            // 
            // InstanceTypeImageComboBox
            // 
            this.InstanceTypeImageComboBox.AutoHeight = false;
            this.InstanceTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.InstanceTypeImageComboBox.Name = "InstanceTypeImageComboBox";
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Cooldown (In Minutes)";
            this.gridColumn12.FieldName = "CooldownTimeInMinutes";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 9;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "Join In SafeZone Only";
            this.gridColumn13.FieldName = "SafeZoneOnly";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 4;
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ribbon.SearchEditItem,
            this.SaveDatabaseButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 2;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(713, 144);
            // 
            // SaveDatabaseButton
            // 
            this.SaveDatabaseButton.Caption = "Save Database";
            this.SaveDatabaseButton.Id = 1;
            this.SaveDatabaseButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SaveDatabaseButton.ImageOptions.Image")));
            this.SaveDatabaseButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SaveDatabaseButton.ImageOptions.LargeImage")));
            this.SaveDatabaseButton.LargeWidth = 60;
            this.SaveDatabaseButton.Name = "SaveDatabaseButton";
            this.SaveDatabaseButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SaveDatabaseButton_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonPageGroup1.ItemLinks.Add(this.SaveDatabaseButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Saving";
            // 
            // InstanceInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 479);
            this.Controls.Add(this.InstanceInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "InstanceInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Instance Info";
            ((System.ComponentModel.ISupportInitialize)(this.InstanceMapGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapInfoLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceInfoStatsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InstanceTypeImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}