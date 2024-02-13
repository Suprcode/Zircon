namespace Server.Views
{
    partial class FameInfoView
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode3 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode4 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FameInfoView));
            FameInfoStatGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            StatComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            FameInfoGridControl = new DevExpress.XtraGrid.GridControl();
            FameInfoRewardGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            FameInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)FameInfoStatGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StatComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FameInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FameInfoRewardGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FameInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            SuspendLayout();
            // 
            // FameInfoStatGridView
            // 
            FameInfoStatGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn6, gridColumn7 });
            FameInfoStatGridView.GridControl = FameInfoGridControl;
            FameInfoStatGridView.Name = "FameInfoStatGridView";
            FameInfoStatGridView.OptionsView.EnableAppearanceEvenRow = true;
            FameInfoStatGridView.OptionsView.EnableAppearanceOddRow = true;
            FameInfoStatGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            FameInfoStatGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            FameInfoStatGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn6
            // 
            gridColumn6.Caption = "Stat";
            gridColumn6.ColumnEdit = StatComboBox;
            gridColumn6.FieldName = "Stat";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 0;
            // 
            // StatComboBox
            // 
            StatComboBox.AutoHeight = false;
            StatComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            StatComboBox.Name = "StatComboBox";
            // 
            // gridColumn7
            // 
            gridColumn7.Caption = "Amount";
            gridColumn7.FieldName = "Amount";
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 1;
            // 
            // FameInfoGridControl
            // 
            FameInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode3.LevelTemplate = FameInfoStatGridView;
            gridLevelNode3.RelationName = "BuffStats";
            gridLevelNode4.LevelTemplate = FameInfoRewardGridView;
            gridLevelNode4.RelationName = "ItemRewards";
            FameInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode3, gridLevelNode4 });
            FameInfoGridControl.Location = new System.Drawing.Point(0, 144);
            FameInfoGridControl.MainView = FameInfoGridView;
            FameInfoGridControl.MenuManager = ribbon;
            FameInfoGridControl.Name = "FameInfoGridControl";
            FameInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { ItemLookUpEdit, StatComboBox });
            FameInfoGridControl.Size = new System.Drawing.Size(694, 432);
            FameInfoGridControl.TabIndex = 1;
            FameInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { FameInfoRewardGridView, FameInfoGridView, FameInfoStatGridView });
            // 
            // FameInfoRewardGridView
            // 
            FameInfoRewardGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn8, gridColumn9 });
            FameInfoRewardGridView.GridControl = FameInfoGridControl;
            FameInfoRewardGridView.Name = "FameInfoRewardGridView";
            FameInfoRewardGridView.OptionsView.EnableAppearanceEvenRow = true;
            FameInfoRewardGridView.OptionsView.EnableAppearanceOddRow = true;
            FameInfoRewardGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            FameInfoRewardGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            FameInfoRewardGridView.OptionsView.ShowGroupPanel = false;
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
            // FameInfoGridView
            // 
            FameInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1, gridColumn2, gridColumn3, gridColumn4, gridColumn5 });
            FameInfoGridView.GridControl = FameInfoGridControl;
            FameInfoGridView.Name = "FameInfoGridView";
            FameInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            FameInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            FameInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            FameInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            gridColumn1.Caption = "Name";
            gridColumn1.FieldName = "Name";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            gridColumn2.Caption = "Shape";
            gridColumn2.FieldName = "Shape";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            gridColumn3.Caption = "Description";
            gridColumn3.FieldName = "Description";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            gridColumn4.Caption = "Cost";
            gridColumn4.FieldName = "Cost";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            gridColumn5.Caption = "Order";
            gridColumn5.FieldName = "Order";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 4;
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
            // FameInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(694, 576);
            Controls.Add(FameInfoGridControl);
            Controls.Add(ribbon);
            Name = "FameInfoView";
            Ribbon = ribbon;
            Text = "Fame Info";
            ((System.ComponentModel.ISupportInitialize)FameInfoStatGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)StatComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)FameInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)FameInfoRewardGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)FameInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveDatabaseButton;
        private DevExpress.XtraGrid.GridControl FameInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView FameInfoGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Views.Grid.GridView FameInfoStatGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Views.Grid.GridView FameInfoRewardGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatComboBox;
    }
}