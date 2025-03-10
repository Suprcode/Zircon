namespace Server.Views
{
    partial class BundleInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BundleInfoView));
            BundleItemInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            BundleInfoGridControl = new DevExpress.XtraGrid.GridControl();
            BundleInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            BundleItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)BundleItemInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BundleInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BundleInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BundleItemLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            SuspendLayout();
            // 
            // BundleItemInfoGridView
            // 
            BundleItemInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn8, gridColumn9 });
            BundleItemInfoGridView.GridControl = BundleInfoGridControl;
            BundleItemInfoGridView.Name = "BundleItemInfoGridView";
            BundleItemInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            BundleItemInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            BundleItemInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            BundleItemInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            BundleItemInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn8
            // 
            gridColumn8.Caption = "Amount";
            gridColumn8.FieldName = "Amount";
            gridColumn8.Name = "gridColumn8";
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 1;
            // 
            // gridColumn9
            // 
            gridColumn9.Caption = "Item";
            gridColumn9.ColumnEdit = ItemLookUpEdit;
            gridColumn9.FieldName = "Item";
            gridColumn9.Name = "gridColumn9";
            gridColumn9.Visible = true;
            gridColumn9.VisibleIndex = 0;
            // 
            // ItemLookUpEdit
            // 
            ItemLookUpEdit.AutoHeight = false;
            ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size") });
            ItemLookUpEdit.DisplayMember = "ItemName";
            ItemLookUpEdit.Name = "ItemLookUpEdit";
            ItemLookUpEdit.NullText = "[Item is null]";
            // 
            // BundleInfoGridControl
            // 
            BundleInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = BundleItemInfoGridView;
            gridLevelNode1.RelationName = "Contents";
            BundleInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            BundleInfoGridControl.Location = new System.Drawing.Point(0, 144);
            BundleInfoGridControl.MainView = BundleInfoGridView;
            BundleInfoGridControl.MenuManager = ribbon;
            BundleInfoGridControl.Name = "BundleInfoGridControl";
            BundleInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { BundleItemLookUpEdit, ItemLookUpEdit });
            BundleInfoGridControl.Size = new System.Drawing.Size(666, 418);
            BundleInfoGridControl.TabIndex = 1;
            BundleInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { BundleInfoGridView, BundleItemInfoGridView });
            // 
            // BundleInfoGridView
            // 
            BundleInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1, gridColumn2, gridColumn3 });
            BundleInfoGridView.GridControl = BundleInfoGridControl;
            BundleInfoGridView.Name = "BundleInfoGridView";
            BundleInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            BundleInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            BundleInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            BundleInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            gridColumn1.FieldName = "Description";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 1;
            // 
            // BundleItemLookUpEdit
            // 
            BundleItemLookUpEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            BundleItemLookUpEdit.AutoHeight = false;
            BundleItemLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            BundleItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            BundleItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size") });
            BundleItemLookUpEdit.DisplayMember = "ItemName";
            BundleItemLookUpEdit.Name = "BundleItemLookUpEdit";
            BundleItemLookUpEdit.NullText = "[Item is null]";
            BundleItemLookUpEdit.UseCtrlScroll = false;
            // 
            // gridColumn2
            // 
            gridColumn2.FieldName = "Type";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 2;
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveDatabaseButton, ImportButton, ExportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(666, 144);
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
            // gridColumn3
            // 
            gridColumn3.FieldName = "Index";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 0;
            // 
            // BundleInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(666, 562);
            Controls.Add(BundleInfoGridControl);
            Controls.Add(ribbon);
            Name = "BundleInfoView";
            Ribbon = ribbon;
            Text = "Bundle Info";
            ((System.ComponentModel.ISupportInitialize)BundleItemInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)BundleInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)BundleInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)BundleItemLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveDatabaseButton;
        private DevExpress.XtraGrid.GridControl BundleInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView BundleInfoGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit BundleItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Views.Grid.GridView BundleItemInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
    }
}