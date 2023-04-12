namespace Server.Views
{
    partial class CastleInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CastleInfoView));
            this.CastleInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.CastleInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MapLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MonsterLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ImportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.CastleInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CastleInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.SuspendLayout();
            // 
            // CastleInfoGridControl
            // 
            this.CastleInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CastleInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.CastleInfoGridControl.MainView = this.CastleInfoGridView;
            this.CastleInfoGridControl.MenuManager = this.ribbon;
            this.CastleInfoGridControl.Name = "CastleInfoGridControl";
            this.CastleInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MonsterLookUpEdit,
            this.RegionLookUpEdit,
            this.MapLookUpEdit,
            this.repositoryItemTextEdit1,
            this.ItemLookUpEdit});
            this.CastleInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.CastleInfoGridControl.Size = new System.Drawing.Size(742, 380);
            this.CastleInfoGridControl.TabIndex = 2;
            this.CastleInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CastleInfoGridView});
            // 
            // CastleInfoGridView
            // 
            this.CastleInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn9,
            this.gridColumn7,
            this.gridColumn8});
            this.CastleInfoGridView.GridControl = this.CastleInfoGridControl;
            this.CastleInfoGridView.Name = "CastleInfoGridView";
            this.CastleInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.CastleInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.CastleInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.CastleInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.CastleInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.CastleInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "Name";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.ColumnEdit = this.MapLookUpEdit;
            this.gridColumn2.FieldName = "Map";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
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
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "StartTime";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "Duration";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn5.FieldName = "CastleRegion";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
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
            // gridColumn6
            // 
            this.gridColumn6.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn6.FieldName = "AttackSpawnRegion";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn9
            // 
            this.gridColumn9.ColumnEdit = this.ItemLookUpEdit;
            this.gridColumn9.FieldName = "Item";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 6;
            // 
            // ItemLookUpEdit
            // 
            this.ItemLookUpEdit.AutoHeight = false;
            this.ItemLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
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
            // gridColumn7
            // 
            this.gridColumn7.ColumnEdit = this.MonsterLookUpEdit;
            this.gridColumn7.FieldName = "Monster";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 7;
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
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "IsBoss")});
            this.MonsterLookUpEdit.DisplayMember = "MonsterName";
            this.MonsterLookUpEdit.Name = "MonsterLookUpEdit";
            this.MonsterLookUpEdit.NullText = "[Monster is Null]";
            // 
            // gridColumn8
            // 
            this.gridColumn8.ColumnEdit = this.repositoryItemTextEdit1;
            this.gridColumn8.FieldName = "Discount";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 8;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton,
            this.ribbon.SearchEditItem,
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
            // CastleInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 524);
            this.Controls.Add(this.CastleInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "CastleInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Castle Info";
            ((System.ComponentModel.ISupportInitialize)(this.CastleInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CastleInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl CastleInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CastleInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MonsterLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit RegionLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MapLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
    }
}