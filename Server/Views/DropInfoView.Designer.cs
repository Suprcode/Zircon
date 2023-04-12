namespace Server.Views
{
    partial class DropInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SavingButton = new DevExpress.XtraBars.BarButtonItem();
            this.ImportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.DropInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.DropInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MonsterLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ribbon.SearchEditItem,
            this.SavingButton,
            this.ImportButton,
            this.ExportButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(706, 144);
            // 
            // SavingButton
            // 
            this.SavingButton.Caption = "Save Database";
            this.SavingButton.Id = 1;
            this.SavingButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SavingButton.ImageOptions.Image")));
            this.SavingButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SavingButton.ImageOptions.LargeImage")));
            this.SavingButton.LargeWidth = 60;
            this.SavingButton.Name = "SavingButton";
            this.SavingButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SavingButton_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.SavingButton);
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
            // DropInfoGridControl
            // 
            this.DropInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DropInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.DropInfoGridControl.MainView = this.DropInfoGridView;
            this.DropInfoGridControl.MenuManager = this.ribbon;
            this.DropInfoGridControl.Name = "DropInfoGridControl";
            this.DropInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MonsterLookUpEdit,
            this.ItemLookUpEdit});
            this.DropInfoGridControl.Size = new System.Drawing.Size(706, 438);
            this.DropInfoGridControl.TabIndex = 1;
            this.DropInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.DropInfoGridView});
            // 
            // DropInfoGridView
            // 
            this.DropInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7});
            this.DropInfoGridView.GridControl = this.DropInfoGridControl;
            this.DropInfoGridView.Name = "DropInfoGridView";
            this.DropInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.DropInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.DropInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.DropInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.DropInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.ColumnEdit = this.MonsterLookUpEdit;
            this.gridColumn1.FieldName = "Monster";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
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
            // gridColumn2
            // 
            this.gridColumn2.ColumnEdit = this.ItemLookUpEdit;
            this.gridColumn2.FieldName = "Item";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn2.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
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
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "Chance";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "Amount";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.FieldName = "DropSet";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            this.gridColumn6.FieldName = "PartOnly";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.FieldName = "EasterEvent";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // DropInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 582);
            this.Controls.Add(this.DropInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "DropInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Drop Info";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SavingButton;
        private DevExpress.XtraGrid.GridControl DropInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView DropInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MonsterLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
    }
}