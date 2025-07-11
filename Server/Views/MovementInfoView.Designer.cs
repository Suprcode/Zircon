namespace Server.Views
{
    partial class MovementInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovementInfoView));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            MovementGridControl = new DevExpress.XtraGrid.GridControl();
            MovementGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            MapLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            SpawnLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            InstanceLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            MapIconImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MovementGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MovementGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SpawnLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)InstanceLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapIconImageComboBox).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveButton, ImportButton, ExportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(719, 144);
            // 
            // SaveButton
            // 
            SaveButton.Caption = "Save Database";
            SaveButton.Id = 1;
            SaveButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.Image");
            SaveButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.LargeImage");
            SaveButton.LargeWidth = 60;
            SaveButton.Name = "SaveButton";
            SaveButton.ItemClick += SaveButton_ItemClick;
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
            ribbonPageGroup1.ItemLinks.Add(SaveButton);
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
            // MovementGridControl
            // 
            MovementGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            MovementGridControl.Location = new System.Drawing.Point(0, 144);
            MovementGridControl.MainView = MovementGridView;
            MovementGridControl.MenuManager = ribbon;
            MovementGridControl.Name = "MovementGridControl";
            MovementGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { MapLookUpEdit, MapIconImageComboBox, ItemLookUpEdit, SpawnLookUpEdit, InstanceLookUpEdit });
            MovementGridControl.ShowOnlyPredefinedDetails = true;
            MovementGridControl.Size = new System.Drawing.Size(719, 384);
            MovementGridControl.TabIndex = 2;
            MovementGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { MovementGridView });
            // 
            // MovementGridView
            // 
            MovementGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1, gridColumn2, gridColumn3, gridColumn4, gridColumn5, gridColumn6, gridColumn7, gridColumn8, gridColumn9 });
            MovementGridView.GridControl = MovementGridControl;
            MovementGridView.Name = "MovementGridView";
            MovementGridView.OptionsView.EnableAppearanceEvenRow = true;
            MovementGridView.OptionsView.EnableAppearanceOddRow = true;
            MovementGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            MovementGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            MovementGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            gridColumn1.ColumnEdit = MapLookUpEdit;
            gridColumn1.FieldName = "SourceRegion";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            gridColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            // 
            // MapLookUpEdit
            // 
            MapLookUpEdit.AutoHeight = false;
            MapLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            MapLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            MapLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Description"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size") });
            MapLookUpEdit.DisplayMember = "ServerDescription";
            MapLookUpEdit.Name = "MapLookUpEdit";
            MapLookUpEdit.NullText = "[Region is null]";
            // 
            // gridColumn2
            // 
            gridColumn2.ColumnEdit = MapLookUpEdit;
            gridColumn2.FieldName = "DestinationRegion";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            gridColumn2.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            gridColumn3.FieldName = "Icon";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            gridColumn4.ColumnEdit = ItemLookUpEdit;
            gridColumn4.FieldName = "NeedItem";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 3;
            // 
            // ItemLookUpEdit
            // 
            ItemLookUpEdit.AutoHeight = false;
            ItemLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size") });
            ItemLookUpEdit.DisplayMember = "ItemName";
            ItemLookUpEdit.Name = "ItemLookUpEdit";
            ItemLookUpEdit.NullText = "[Item is null]";
            // 
            // gridColumn5
            // 
            gridColumn5.ColumnEdit = SpawnLookUpEdit;
            gridColumn5.FieldName = "NeedSpawn";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 4;
            // 
            // SpawnLookUpEdit
            // 
            SpawnLookUpEdit.AutoHeight = false;
            SpawnLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            SpawnLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            SpawnLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("RegionName", "Region"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster") });
            SpawnLookUpEdit.DisplayMember = "MonsterName";
            SpawnLookUpEdit.Name = "SpawnLookUpEdit";
            SpawnLookUpEdit.NullText = "[Spawn is null]";
            // 
            // gridColumn6
            // 
            gridColumn6.FieldName = "Effect";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 7;
            // 
            // gridColumn7
            // 
            gridColumn7.FieldName = "RequiredClass";
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 8;
            // 
            // gridColumn8
            // 
            gridColumn8.Caption = "Need Instance";
            gridColumn8.ColumnEdit = InstanceLookUpEdit;
            gridColumn8.FieldName = "NeedInstance";
            gridColumn8.Name = "gridColumn8";
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 5;
            // 
            // InstanceLookUpEdit
            // 
            InstanceLookUpEdit.AutoHeight = false;
            InstanceLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            InstanceLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Type", "Type") });
            InstanceLookUpEdit.DisplayMember = "Name";
            InstanceLookUpEdit.Name = "InstanceLookUpEdit";
            InstanceLookUpEdit.NullText = "[Instance is null]";
            // 
            // MapIconImageComboBox
            // 
            MapIconImageComboBox.AutoHeight = false;
            MapIconImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            MapIconImageComboBox.Name = "MapIconImageComboBox";
            // 
            // gridColumn9
            // 
            gridColumn9.FieldName = "NeedHole";
            gridColumn9.Name = "gridColumn9";
            gridColumn9.Visible = true;
            gridColumn9.VisibleIndex = 6;
            // 
            // MovementInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(719, 528);
            Controls.Add(MovementGridControl);
            Controls.Add(ribbon);
            Name = "MovementInfoView";
            Ribbon = ribbon;
            Text = "Movement Info";
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)MovementGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)MovementGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)SpawnLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)InstanceLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapIconImageComboBox).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl MovementGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView MovementGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MapLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox MapIconImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit SpawnLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit InstanceLookUpEdit;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
    }
}