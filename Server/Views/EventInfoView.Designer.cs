namespace Server.Views
{
    partial class EventInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventInfoView));
            this.TargetsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MonsterLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.EventInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.ActionsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RespawnLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MapLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.EventInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.ImportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)(this.TargetsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EventInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActionsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RespawnLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EventInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.SuspendLayout();
            // 
            // TargetsGridView
            // 
            this.TargetsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn5,
            this.gridColumn12,
            this.gridColumn3});
            this.TargetsGridView.GridControl = this.EventInfoGridControl;
            this.TargetsGridView.Name = "TargetsGridView";
            this.TargetsGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.TargetsGridView.OptionsView.EnableAppearanceOddRow = true;
            this.TargetsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.TargetsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.TargetsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn5
            // 
            this.gridColumn5.ColumnEdit = this.MonsterLookUpEdit;
            this.gridColumn5.FieldName = "Monster";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            // 
            // MonsterLookUpEdit
            // 
            this.MonsterLookUpEdit.AutoHeight = false;
            this.MonsterLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.MonsterLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MonsterLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AI", "AI"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Level", "Level"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Experience", "Experience"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "Is Boss")});
            this.MonsterLookUpEdit.DisplayMember = "MonsterName";
            this.MonsterLookUpEdit.Name = "MonsterLookUpEdit";
            this.MonsterLookUpEdit.NullText = "[Monster is null]";
            // 
            // gridColumn12
            // 
            this.gridColumn12.FieldName = "DropSet";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "Value";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // EventInfoGridControl
            // 
            this.EventInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = this.TargetsGridView;
            gridLevelNode1.RelationName = "Targets";
            gridLevelNode2.LevelTemplate = this.ActionsGridView;
            gridLevelNode2.RelationName = "Actions";
            this.EventInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1,
            gridLevelNode2});
            this.EventInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.EventInfoGridControl.MainView = this.EventInfoGridView;
            this.EventInfoGridControl.MenuManager = this.ribbon;
            this.EventInfoGridControl.Name = "EventInfoGridControl";
            this.EventInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MonsterLookUpEdit,
            this.RespawnLookUpEdit,
            this.RegionLookUpEdit,
            this.MapLookUpEdit});
            this.EventInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.EventInfoGridControl.Size = new System.Drawing.Size(742, 380);
            this.EventInfoGridControl.TabIndex = 2;
            this.EventInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ActionsGridView,
            this.EventInfoGridView,
            this.TargetsGridView});
            // 
            // ActionsGridView
            // 
            this.ActionsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn4,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11});
            this.ActionsGridView.GridControl = this.EventInfoGridControl;
            this.ActionsGridView.Name = "ActionsGridView";
            this.ActionsGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.ActionsGridView.OptionsView.EnableAppearanceOddRow = true;
            this.ActionsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.ActionsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.ActionsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "TriggerValue";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 0;
            this.gridColumn4.Width = 103;
            // 
            // gridColumn6
            // 
            this.gridColumn6.FieldName = "Type";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 1;
            this.gridColumn6.Width = 103;
            // 
            // gridColumn7
            // 
            this.gridColumn7.FieldName = "StringParameter1";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 2;
            this.gridColumn7.Width = 103;
            // 
            // gridColumn8
            // 
            this.gridColumn8.ColumnEdit = this.MonsterLookUpEdit;
            this.gridColumn8.FieldName = "MonsterParameter1";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 3;
            this.gridColumn8.Width = 103;
            // 
            // gridColumn9
            // 
            this.gridColumn9.ColumnEdit = this.RespawnLookUpEdit;
            this.gridColumn9.FieldName = "RespawnParameter1";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 4;
            this.gridColumn9.Width = 87;
            // 
            // RespawnLookUpEdit
            // 
            this.RespawnLookUpEdit.AutoHeight = false;
            this.RespawnLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.RespawnLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RespawnLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("RegionName", "Region"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster")});
            this.RespawnLookUpEdit.DisplayMember = "RegionName";
            this.RespawnLookUpEdit.Name = "RespawnLookUpEdit";
            this.RespawnLookUpEdit.NullText = "[Respawn is null]";
            // 
            // gridColumn10
            // 
            this.gridColumn10.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn10.FieldName = "RegionParameter1";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 5;
            this.gridColumn10.Width = 110;
            // 
            // RegionLookUpEdit
            // 
            this.RegionLookUpEdit.AutoHeight = false;
            this.RegionLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.RegionLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RegionLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Description")});
            this.RegionLookUpEdit.DisplayMember = "ServerDescription";
            this.RegionLookUpEdit.Name = "RegionLookUpEdit";
            this.RegionLookUpEdit.NullText = "[Region is null]";
            // 
            // gridColumn11
            // 
            this.gridColumn11.ColumnEdit = this.MapLookUpEdit;
            this.gridColumn11.FieldName = "MapParameter1";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 6;
            this.gridColumn11.Width = 115;
            // 
            // MapLookUpEdit
            // 
            this.MapLookUpEdit.AutoHeight = false;
            this.MapLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.MapLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MapLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FileName", "File Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Description")});
            this.MapLookUpEdit.DisplayMember = "Description";
            this.MapLookUpEdit.Name = "MapLookUpEdit";
            this.MapLookUpEdit.NullText = "[Map is null]";
            // 
            // EventInfoGridView
            // 
            this.EventInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.EventInfoGridView.GridControl = this.EventInfoGridControl;
            this.EventInfoGridView.Name = "EventInfoGridView";
            this.EventInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.EventInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.EventInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.EventInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.EventInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.EventInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "Description";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.FieldName = "MaxValue";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ribbon.SearchEditItem,
            this.SaveButton,
            this.ImportButton,
            this.ExportButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(742, 144);
            // 
            // SaveButton
            // 
            this.SaveButton.Caption = "Save Database";
            this.SaveButton.Id = 1;
            this.SaveButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.Image")));
            this.SaveButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.LargeImage")));
            this.SaveButton.LargeWidth = 60;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SaveButton_ItemClick);
            // 
            // ImportButton
            // 
            this.ImportButton.Caption = "Import";
            this.ImportButton.Id = 2;
            this.ImportButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ImportButton.ImageOptions.Image")));
            this.ImportButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ImportButton.ImageOptions.LargeImage")));
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ImportButton_ItemClick);
            // 
            // ExportButton
            // 
            this.ExportButton.Caption = "Export";
            this.ExportButton.Id = 3;
            this.ExportButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ExportButton.ImageOptions.Image")));
            this.ExportButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ExportButton.ImageOptions.LargeImage")));
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ExportButton_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.JsonImportExport});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonPageGroup1.ItemLinks.Add(this.SaveButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Saving";
            // 
            // JsonImportExport
            // 
            this.JsonImportExport.ItemLinks.Add(this.ImportButton);
            this.JsonImportExport.ItemLinks.Add(this.ExportButton);
            this.JsonImportExport.Name = "JsonImportExport";
            this.JsonImportExport.Text = "Json";
            // 
            // EventInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 524);
            this.Controls.Add(this.EventInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "EventInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Event Info";
            ((System.ComponentModel.ISupportInitialize)(this.TargetsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EventInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActionsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RespawnLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EventInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl EventInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView TargetsGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView ActionsGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView EventInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MonsterLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit RespawnLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit RegionLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MapLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
    }
}