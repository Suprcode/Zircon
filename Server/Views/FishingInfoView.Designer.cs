namespace Server.Views
{
    partial class FishingInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FishingInfoView));
            this.FishingDropInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ColItem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.ColChance = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.FishingInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.FishingInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ImportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.FishingDropInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FishingInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FishingInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.SuspendLayout();
            // 
            // FishingDropInfoGridView
            // 
            this.FishingDropInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.ColItem,
            this.ColChance,
            this.gridColumn3,
            this.gridColumn4});
            this.FishingDropInfoGridView.GridControl = this.FishingInfoGridControl;
            this.FishingDropInfoGridView.Name = "FishingDropInfoGridView";
            this.FishingDropInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.FishingDropInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.FishingDropInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.FishingDropInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.FishingDropInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.FishingDropInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // ColItem
            // 
            this.ColItem.Caption = "Item";
            this.ColItem.ColumnEdit = this.ItemLookUpEdit;
            this.ColItem.FieldName = "Item";
            this.ColItem.Name = "ColItem";
            this.ColItem.Visible = true;
            this.ColItem.VisibleIndex = 0;
            // 
            // ItemLookUpEdit
            // 
            this.ItemLookUpEdit.AutoHeight = false;
            this.ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size")});
            this.ItemLookUpEdit.DisplayMember = "ItemName";
            this.ItemLookUpEdit.Name = "ItemLookUpEdit";
            this.ItemLookUpEdit.NullText = "[Item is null]";
            // 
            // ColChance
            // 
            this.ColChance.Caption = "Chance";
            this.ColChance.FieldName = "Chance";
            this.ColChance.Name = "ColChance";
            this.ColChance.Visible = true;
            this.ColChance.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Perfect Catch";
            this.gridColumn3.FieldName = "PerfectCatch";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Throw Quality";
            this.gridColumn4.FieldName = "ThrowQuality";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            // 
            // FishingInfoGridControl
            // 
            this.FishingInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = this.FishingDropInfoGridView;
            gridLevelNode1.RelationName = "Drops";
            this.FishingInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.FishingInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.FishingInfoGridControl.MainView = this.FishingInfoGridView;
            this.FishingInfoGridControl.MenuManager = this.ribbon;
            this.FishingInfoGridControl.Name = "FishingInfoGridControl";
            this.FishingInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RegionLookUpEdit,
            this.ItemLookUpEdit});
            this.FishingInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.FishingInfoGridControl.Size = new System.Drawing.Size(713, 335);
            this.FishingInfoGridControl.TabIndex = 3;
            this.FishingInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.FishingInfoGridView,
            this.FishingDropInfoGridView});
            // 
            // FishingInfoGridView
            // 
            this.FishingInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.FishingInfoGridView.GridControl = this.FishingInfoGridControl;
            this.FishingInfoGridView.Name = "FishingInfoGridView";
            this.FishingInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.FishingInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.FishingInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.FishingInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.FishingInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.FishingInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Name";
            this.gridColumn1.FieldName = "Name";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Region";
            this.gridColumn2.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn2.FieldName = "Region";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
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
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ribbon.SearchEditItem,
            this.SaveDatabaseButton,
            this.ImportButton,
            this.ExportButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
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
            this.ribbonPageGroup1,
            this.JsonImportExport});
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
            // JsonImportExport
            // 
            this.JsonImportExport.ItemLinks.Add(this.ImportButton);
            this.JsonImportExport.ItemLinks.Add(this.ExportButton);
            this.JsonImportExport.Name = "JsonImportExport";
            this.JsonImportExport.Text = "Json";
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
            // FishingInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 479);
            this.Controls.Add(this.FishingInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "FishingInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Fishing Info";
            ((System.ComponentModel.ISupportInitialize)(this.FishingDropInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FishingInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FishingInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveDatabaseButton;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit RegionLookUpEdit;
        private DevExpress.XtraGrid.Views.Grid.GridView FishingInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.GridControl FishingInfoGridControl;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Views.Grid.GridView FishingDropInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn ColItem;
        private DevExpress.XtraGrid.Columns.GridColumn ColChance;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
    }
}