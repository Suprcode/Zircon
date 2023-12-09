namespace Server.Views
{
    partial class CurrencyInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurrencyInfoView));
            CurrencyInfoImageGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            CurrencyInfoGridControl = new DevExpress.XtraGrid.GridControl();
            CurrencyInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            CurrencyTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)CurrencyInfoImageGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CurrencyInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CurrencyInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CurrencyTypeImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            SuspendLayout();
            // 
            // CurrencyInfoImageGridView
            // 
            CurrencyInfoImageGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn6, gridColumn7 });
            CurrencyInfoImageGridView.GridControl = CurrencyInfoGridControl;
            CurrencyInfoImageGridView.Name = "CurrencyInfoImageGridView";
            CurrencyInfoImageGridView.OptionsView.EnableAppearanceEvenRow = true;
            CurrencyInfoImageGridView.OptionsView.EnableAppearanceOddRow = true;
            CurrencyInfoImageGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            CurrencyInfoImageGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            CurrencyInfoImageGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn6
            // 
            gridColumn6.Caption = "Image";
            gridColumn6.FieldName = "Image";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 0;
            // 
            // gridColumn7
            // 
            gridColumn7.Caption = "Amount";
            gridColumn7.FieldName = "Amount";
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 1;
            // 
            // CurrencyInfoGridControl
            // 
            CurrencyInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = CurrencyInfoImageGridView;
            gridLevelNode1.RelationName = "Images";
            CurrencyInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            CurrencyInfoGridControl.Location = new System.Drawing.Point(0, 144);
            CurrencyInfoGridControl.MainView = CurrencyInfoGridView;
            CurrencyInfoGridControl.MenuManager = ribbon;
            CurrencyInfoGridControl.Name = "CurrencyInfoGridControl";
            CurrencyInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { ItemLookUpEdit, CurrencyTypeImageComboBox });
            CurrencyInfoGridControl.Size = new System.Drawing.Size(694, 432);
            CurrencyInfoGridControl.TabIndex = 1;
            CurrencyInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { CurrencyInfoGridView, CurrencyInfoImageGridView });
            // 
            // CurrencyInfoGridView
            // 
            CurrencyInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1, gridColumn5, gridColumn2, gridColumn3, gridColumn4 });
            CurrencyInfoGridView.GridControl = CurrencyInfoGridControl;
            CurrencyInfoGridView.Name = "CurrencyInfoGridView";
            CurrencyInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            CurrencyInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            CurrencyInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            CurrencyInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            gridColumn1.Caption = "Name";
            gridColumn1.FieldName = "Name";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn5
            // 
            gridColumn5.Caption = "Abbreviation";
            gridColumn5.FieldName = "Abbreviation";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 1;
            // 
            // gridColumn2
            // 
            gridColumn2.Caption = "Type";
            gridColumn2.ColumnEdit = CurrencyTypeImageComboBox;
            gridColumn2.FieldName = "Type";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 2;
            // 
            // CurrencyTypeImageComboBox
            // 
            CurrencyTypeImageComboBox.AutoHeight = false;
            CurrencyTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            CurrencyTypeImageComboBox.Name = "CurrencyTypeImageComboBox";
            // 
            // gridColumn3
            // 
            gridColumn3.Caption = "Item";
            gridColumn3.ColumnEdit = ItemLookUpEdit;
            gridColumn3.FieldName = "DropItem";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            gridColumn3.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 3;
            // 
            // ItemLookUpEdit
            // 
            ItemLookUpEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            ItemLookUpEdit.AutoHeight = false;
            ItemLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size") });
            ItemLookUpEdit.DisplayMember = "ItemName";
            ItemLookUpEdit.Name = "ItemLookUpEdit";
            ItemLookUpEdit.NullText = "[Item is null]";
            ItemLookUpEdit.UseCtrlScroll = false;
            // 
            // gridColumn4
            // 
            gridColumn4.Caption = "Exchange Rate";
            gridColumn4.FieldName = "ExchangeRate";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 4;
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveDatabaseButton, ImportButton, ExportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(694, 144);
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
            // CurrencyInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(694, 576);
            Controls.Add(CurrencyInfoGridControl);
            Controls.Add(ribbon);
            Name = "CurrencyInfoView";
            Ribbon = ribbon;
            Text = "Currency Info";
            ((System.ComponentModel.ISupportInitialize)CurrencyInfoImageGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)CurrencyInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)CurrencyInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)CurrencyTypeImageComboBox).EndInit();
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
        private DevExpress.XtraGrid.GridControl CurrencyInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CurrencyInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox CurrencyTypeImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Views.Grid.GridView CurrencyInfoImageGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
    }
}