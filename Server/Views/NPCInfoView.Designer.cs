namespace Server.Views
{
    partial class NPCInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NPCInfoView));
            RequirementGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            RequirementImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            QuestInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            RequiredClassImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            DaysOfWeekImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            NPCInfoGridControl = new DevExpress.XtraGrid.GridControl();
            NPCInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colNPCName = new DevExpress.XtraGrid.Columns.GridColumn();
            colImage = new DevExpress.XtraGrid.Columns.GridColumn();
            colEntryPage = new DevExpress.XtraGrid.Columns.GridColumn();
            PageLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            colFaceImage = new DevExpress.XtraGrid.Columns.GridColumn();
            colMapIcon = new DevExpress.XtraGrid.Columns.GridColumn();
            MapIconImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ItemInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            ((System.ComponentModel.ISupportInitialize)RequirementGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RequirementImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)QuestInfoLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RequiredClassImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DaysOfWeekImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NPCInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NPCInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PageLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RegionLookUpEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MapIconImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ItemInfoLookUpEdit).BeginInit();
            SuspendLayout();
            // 
            // RequirementGridView
            // 
            RequirementGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn2, gridColumn3, gridColumn4, gridColumn5, gridColumn6 });
            RequirementGridView.GridControl = NPCInfoGridControl;
            RequirementGridView.Name = "RequirementGridView";
            RequirementGridView.OptionsView.EnableAppearanceEvenRow = true;
            RequirementGridView.OptionsView.EnableAppearanceOddRow = true;
            RequirementGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            RequirementGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            RequirementGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn2
            // 
            gridColumn2.Caption = "Requirement";
            gridColumn2.ColumnEdit = RequirementImageComboBox;
            gridColumn2.FieldName = "Requirement";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 0;
            // 
            // RequirementImageComboBox
            // 
            RequirementImageComboBox.AutoHeight = false;
            RequirementImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RequirementImageComboBox.Name = "RequirementImageComboBox";
            // 
            // gridColumn3
            // 
            gridColumn3.Caption = "Int Parameter 1";
            gridColumn3.FieldName = "IntParameter1";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            gridColumn4.Caption = "Quest Parameter";
            gridColumn4.ColumnEdit = QuestInfoLookUpEdit;
            gridColumn4.FieldName = "QuestParameter";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 2;
            // 
            // QuestInfoLookUpEdit
            // 
            QuestInfoLookUpEdit.AutoHeight = false;
            QuestInfoLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            QuestInfoLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("QuestName", "Quest Name") });
            QuestInfoLookUpEdit.DisplayMember = "QuestName";
            QuestInfoLookUpEdit.Name = "QuestInfoLookUpEdit";
            QuestInfoLookUpEdit.NullText = "[Quest is null]";
            // 
            // gridColumn5
            // 
            gridColumn5.Caption = "Class";
            gridColumn5.ColumnEdit = RequiredClassImageComboBox;
            gridColumn5.FieldName = "Class";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 3;
            // 
            // RequiredClassImageComboBox
            // 
            RequiredClassImageComboBox.AutoHeight = false;
            RequiredClassImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RequiredClassImageComboBox.Name = "RequiredClassImageComboBox";
            // 
            // gridColumn6
            // 
            gridColumn6.Caption = "Days Of Week";
            gridColumn6.ColumnEdit = DaysOfWeekImageComboBox;
            gridColumn6.FieldName = "DaysOfWeek";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 4;
            // 
            // DaysOfWeekImageComboBox
            // 
            DaysOfWeekImageComboBox.AutoHeight = false;
            DaysOfWeekImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            DaysOfWeekImageComboBox.Name = "DaysOfWeekImageComboBox";
            // 
            // NPCInfoGridControl
            // 
            NPCInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = RequirementGridView;
            gridLevelNode1.RelationName = "Requirements";
            NPCInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            NPCInfoGridControl.Location = new System.Drawing.Point(0, 144);
            NPCInfoGridControl.MainView = NPCInfoGridView;
            NPCInfoGridControl.MenuManager = ribbon;
            NPCInfoGridControl.Name = "NPCInfoGridControl";
            NPCInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { RegionLookUpEdit, PageLookUpEdit, RequiredClassImageComboBox, RequirementImageComboBox, QuestInfoLookUpEdit, DaysOfWeekImageComboBox, ItemInfoLookUpEdit, MapIconImageComboBox });
            NPCInfoGridControl.ShowOnlyPredefinedDetails = true;
            NPCInfoGridControl.Size = new System.Drawing.Size(736, 427);
            NPCInfoGridControl.TabIndex = 2;
            NPCInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { NPCInfoGridView, RequirementGridView });
            // 
            // NPCInfoGridView
            // 
            NPCInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colNPCName, colImage, colEntryPage, gridColumn1, colFaceImage, colMapIcon });
            NPCInfoGridView.GridControl = NPCInfoGridControl;
            NPCInfoGridView.Name = "NPCInfoGridView";
            NPCInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            NPCInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            NPCInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            NPCInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colNPCName
            // 
            colNPCName.FieldName = "NPCName";
            colNPCName.Name = "colNPCName";
            colNPCName.Visible = true;
            colNPCName.VisibleIndex = 1;
            // 
            // colImage
            // 
            colImage.FieldName = "Image";
            colImage.Name = "colImage";
            colImage.Visible = true;
            colImage.VisibleIndex = 2;
            // 
            // colEntryPage
            // 
            colEntryPage.ColumnEdit = PageLookUpEdit;
            colEntryPage.FieldName = "EntryPage";
            colEntryPage.Name = "colEntryPage";
            colEntryPage.Visible = true;
            colEntryPage.VisibleIndex = 3;
            // 
            // PageLookUpEdit
            // 
            PageLookUpEdit.AutoHeight = false;
            PageLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            PageLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            PageLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Description"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DialogType", "DialogType"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Say", "Say") });
            PageLookUpEdit.DisplayMember = "Description";
            PageLookUpEdit.Name = "PageLookUpEdit";
            PageLookUpEdit.NullText = "[Page is null]";
            // 
            // gridColumn1
            // 
            gridColumn1.ColumnEdit = RegionLookUpEdit;
            gridColumn1.FieldName = "Region";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            gridColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            // 
            // RegionLookUpEdit
            // 
            RegionLookUpEdit.AutoHeight = false;
            RegionLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            RegionLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            RegionLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Server Description"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size") });
            RegionLookUpEdit.DisplayMember = "ServerDescription";
            RegionLookUpEdit.Name = "RegionLookUpEdit";
            RegionLookUpEdit.NullText = "[Region is null]";
            // 
            // colFaceImage
            // 
            colFaceImage.Caption = "Face Image";
            colFaceImage.FieldName = "FaceImage";
            colFaceImage.Name = "colFaceImage";
            colFaceImage.Visible = true;
            colFaceImage.VisibleIndex = 4;
            // 
            // colMapIcon
            // 
            colMapIcon.ColumnEdit = MapIconImageComboBox;
            colMapIcon.FieldName = "MapIcon";
            colMapIcon.Name = "colMapIcon";
            colMapIcon.Visible = true;
            colMapIcon.VisibleIndex = 5;
            // 
            // MapIconImageComboBox
            // 
            MapIconImageComboBox.AutoHeight = false;
            MapIconImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            MapIconImageComboBox.Name = "MapIconImageComboBox";
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveButton, barButtonItem1, ExportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 4;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(736, 144);
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
            // barButtonItem1
            // 
            barButtonItem1.Caption = "Import";
            barButtonItem1.Id = 2;
            barButtonItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem1.ImageOptions.Image");
            barButtonItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("barButtonItem1.ImageOptions.LargeImage");
            barButtonItem1.Name = "barButtonItem1";
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;
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
            JsonImportExport.ItemLinks.Add(barButtonItem1);
            JsonImportExport.ItemLinks.Add(ExportButton);
            JsonImportExport.Name = "JsonImportExport";
            JsonImportExport.Text = "Json";
            // 
            // ItemInfoLookUpEdit
            // 
            ItemInfoLookUpEdit.AutoHeight = false;
            ItemInfoLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            ItemInfoLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ItemInfoLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size") });
            ItemInfoLookUpEdit.Name = "ItemInfoLookUpEdit";
            ItemInfoLookUpEdit.NullText = "[Reward is null]";
            // 
            // NPCInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(736, 571);
            Controls.Add(NPCInfoGridControl);
            Controls.Add(ribbon);
            Name = "NPCInfoView";
            Ribbon = ribbon;
            Text = "NPC Info";
            ((System.ComponentModel.ISupportInitialize)RequirementGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)RequirementImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)QuestInfoLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)RequiredClassImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)DaysOfWeekImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)NPCInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)NPCInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)PageLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)RegionLookUpEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)MapIconImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)ItemInfoLookUpEdit).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl NPCInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView NPCInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colNPCName;
        private DevExpress.XtraGrid.Columns.GridColumn colImage;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit RegionLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colEntryPage;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit PageLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Views.Grid.GridView RequirementGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequiredClassImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit QuestInfoLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox RequirementImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox DaysOfWeekImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemInfoLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colFaceImage;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox MapIconImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn colMapIcon;
    }
}