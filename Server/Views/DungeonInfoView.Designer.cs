namespace Server.Views
{
    partial class DungeonInfoView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DungeonInfoView));
            DungeonMapGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            FloorColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RoleColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            RoleImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            MapColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            MapInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            DungeonInfoGridControl = new DevExpress.XtraGrid.GridControl();
            DungeonInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            NameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            DescriptionColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            SpawnMultiplierColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            SpawnMultiplierSpinEdit = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)DungeonMapGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RoleImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapInfoLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DungeonInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DungeonInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SpawnMultiplierSpinEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            SuspendLayout();
            // 
            // DungeonMapGridView
            // 
            DungeonMapGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { FloorColumn, RoleColumn, MapColumn });
            DungeonMapGridView.GridControl = DungeonInfoGridControl;
            DungeonMapGridView.Name = "DungeonMapGridView";
            DungeonMapGridView.OptionsView.EnableAppearanceEvenRow = true;
            DungeonMapGridView.OptionsView.EnableAppearanceOddRow = true;
            DungeonMapGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            DungeonMapGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            DungeonMapGridView.OptionsView.ShowGroupPanel = false;
            DungeonMapGridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] { new DevExpress.XtraGrid.Columns.GridColumnSortInfo(FloorColumn, DevExpress.Data.ColumnSortOrder.Ascending) });
            // 
            // FloorColumn
            // 
            FloorColumn.Caption = "Floor";
            FloorColumn.FieldName = "Floor";
            FloorColumn.Name = "FloorColumn";
            FloorColumn.Visible = true;
            FloorColumn.VisibleIndex = 2;
            // 
            // RoleColumn
            // 
            RoleColumn.Caption = "Role";
            RoleColumn.ColumnEdit = RoleImageComboBox;
            RoleColumn.FieldName = "Role";
            RoleColumn.Name = "RoleColumn";
            RoleColumn.Visible = true;
            RoleColumn.VisibleIndex = 1;
            // 
            // RoleImageComboBox
            // 
            RoleImageComboBox.AutoHeight = false;
            RoleImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RoleImageComboBox.Name = "RoleImageComboBox";
            // 
            // MapColumn
            // 
            MapColumn.Caption = "Map";
            MapColumn.ColumnEdit = MapInfoLookUpEdit;
            MapColumn.FieldName = "Map";
            MapColumn.Name = "MapColumn";
            MapColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            MapColumn.Visible = true;
            MapColumn.VisibleIndex = 0;
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
            // DungeonInfoGridControl
            // 
            DungeonInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = DungeonMapGridView;
            gridLevelNode1.RelationName = "Maps";
            DungeonInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            DungeonInfoGridControl.Location = new System.Drawing.Point(0, 144);
            DungeonInfoGridControl.MainView = DungeonInfoGridView;
            DungeonInfoGridControl.MenuManager = ribbon;
            DungeonInfoGridControl.Name = "DungeonInfoGridControl";
            DungeonInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { RoleImageComboBox, MapInfoLookUpEdit, SpawnMultiplierSpinEdit });
            DungeonInfoGridControl.ShowOnlyPredefinedDetails = true;
            DungeonInfoGridControl.Size = new System.Drawing.Size(975, 307);
            DungeonInfoGridControl.TabIndex = 2;
            DungeonInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { DungeonInfoGridView, DungeonMapGridView });
            // 
            // DungeonInfoGridView
            // 
            DungeonInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { NameColumn, DescriptionColumn, SpawnMultiplierColumn });
            DungeonInfoGridView.GridControl = DungeonInfoGridControl;
            DungeonInfoGridView.Name = "DungeonInfoGridView";
            DungeonInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            DungeonInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            DungeonInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            DungeonInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            DungeonInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            DungeonInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // NameColumn
            // 
            NameColumn.Caption = "Name";
            NameColumn.FieldName = "Name";
            NameColumn.Name = "NameColumn";
            NameColumn.Visible = true;
            NameColumn.VisibleIndex = 0;
            // 
            // DescriptionColumn
            // 
            DescriptionColumn.Caption = "Description";
            DescriptionColumn.FieldName = "Description";
            DescriptionColumn.Name = "DescriptionColumn";
            DescriptionColumn.Visible = true;
            DescriptionColumn.VisibleIndex = 1;
            // 
            // SpawnMultiplierColumn
            // 
            SpawnMultiplierColumn.Caption = "Spawn Multiplier";
            SpawnMultiplierColumn.ColumnEdit = SpawnMultiplierSpinEdit;
            SpawnMultiplierColumn.FieldName = "SpawnMultiplier";
            SpawnMultiplierColumn.Name = "SpawnMultiplierColumn";
            SpawnMultiplierColumn.Visible = true;
            SpawnMultiplierColumn.VisibleIndex = 2;
            // 
            // SpawnMultiplierSpinEdit
            // 
            SpawnMultiplierSpinEdit.AutoHeight = false;
            SpawnMultiplierSpinEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            SpawnMultiplierSpinEdit.IsFloatValue = false;
            SpawnMultiplierSpinEdit.MaskSettings.Set("mask", "N0");
            SpawnMultiplierSpinEdit.MaxValue = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            SpawnMultiplierSpinEdit.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
            SpawnMultiplierSpinEdit.Name = "SpawnMultiplierSpinEdit";
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, SaveDatabaseButton, ImportButton, ExportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(975, 144);
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
            // DungeonInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(975, 451);
            Controls.Add(DungeonInfoGridControl);
            Controls.Add(ribbon);
            Name = "DungeonInfoView";
            Ribbon = ribbon;
            Text = "Dungeon Info";
            ((System.ComponentModel.ISupportInitialize)DungeonMapGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)RoleImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapInfoLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)DungeonInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)DungeonInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)SpawnMultiplierSpinEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraBars.BarButtonItem SaveDatabaseButton;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraGrid.GridControl DungeonInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView DungeonInfoGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView DungeonMapGridView;
        private DevExpress.XtraGrid.Columns.GridColumn NameColumn;
        private DevExpress.XtraGrid.Columns.GridColumn DescriptionColumn;
        private DevExpress.XtraGrid.Columns.GridColumn SpawnMultiplierColumn;
        private DevExpress.XtraGrid.Columns.GridColumn FloorColumn;
        private DevExpress.XtraGrid.Columns.GridColumn RoleColumn;
        private DevExpress.XtraGrid.Columns.GridColumn MapColumn;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit SpawnMultiplierSpinEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RoleImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MapInfoLookUpEdit;
    }
}
