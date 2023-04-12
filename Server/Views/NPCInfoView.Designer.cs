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
            this.RequirementGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RequirementImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.QuestInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RequiredClassImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DaysOfWeekImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.NPCInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.NPCInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colNPCName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEntryPage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.PageLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.colFaceImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ItemInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.RequirementGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequirementImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.QuestInfoLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredClassImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DaysOfWeekImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PageLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoLookUpEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // RequirementGridView
            // 
            this.RequirementGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6});
            this.RequirementGridView.GridControl = this.NPCInfoGridControl;
            this.RequirementGridView.Name = "RequirementGridView";
            this.RequirementGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.RequirementGridView.OptionsView.EnableAppearanceOddRow = true;
            this.RequirementGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.RequirementGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.RequirementGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Requirement";
            this.gridColumn2.ColumnEdit = this.RequirementImageComboBox;
            this.gridColumn2.FieldName = "Requirement";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // RequirementImageComboBox
            // 
            this.RequirementImageComboBox.AutoHeight = false;
            this.RequirementImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RequirementImageComboBox.Name = "RequirementImageComboBox";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Int Parameter 1";
            this.gridColumn3.FieldName = "IntParameter1";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Quest Parameter";
            this.gridColumn4.ColumnEdit = this.QuestInfoLookUpEdit;
            this.gridColumn4.FieldName = "QuestParameter";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            // 
            // QuestInfoLookUpEdit
            // 
            this.QuestInfoLookUpEdit.AutoHeight = false;
            this.QuestInfoLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.QuestInfoLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("QuestName", "Quest Name")});
            this.QuestInfoLookUpEdit.DisplayMember = "QuestName";
            this.QuestInfoLookUpEdit.Name = "QuestInfoLookUpEdit";
            this.QuestInfoLookUpEdit.NullText = "[Quest is null]";
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Class";
            this.gridColumn5.ColumnEdit = this.RequiredClassImageComboBox;
            this.gridColumn5.FieldName = "Class";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // RequiredClassImageComboBox
            // 
            this.RequiredClassImageComboBox.AutoHeight = false;
            this.RequiredClassImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RequiredClassImageComboBox.Name = "RequiredClassImageComboBox";
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Days Of Week";
            this.gridColumn6.ColumnEdit = this.DaysOfWeekImageComboBox;
            this.gridColumn6.FieldName = "DaysOfWeek";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            // 
            // DaysOfWeekImageComboBox
            // 
            this.DaysOfWeekImageComboBox.AutoHeight = false;
            this.DaysOfWeekImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DaysOfWeekImageComboBox.Name = "DaysOfWeekImageComboBox";
            // 
            // NPCInfoGridControl
            // 
            this.NPCInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = this.RequirementGridView;
            gridLevelNode1.RelationName = "Requirements";
            this.NPCInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.NPCInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.NPCInfoGridControl.MainView = this.NPCInfoGridView;
            this.NPCInfoGridControl.MenuManager = this.ribbon;
            this.NPCInfoGridControl.Name = "NPCInfoGridControl";
            this.NPCInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RegionLookUpEdit,
            this.PageLookUpEdit,
            this.RequiredClassImageComboBox,
            this.RequirementImageComboBox,
            this.QuestInfoLookUpEdit,
            this.DaysOfWeekImageComboBox,
            this.ItemInfoLookUpEdit});
            this.NPCInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.NPCInfoGridControl.Size = new System.Drawing.Size(736, 427);
            this.NPCInfoGridControl.TabIndex = 2;
            this.NPCInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.NPCInfoGridView,
            this.RequirementGridView});
            // 
            // NPCInfoGridView
            // 
            this.NPCInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colNPCName,
            this.colImage,
            this.colEntryPage,
            this.gridColumn1,
            this.colFaceImage});
            this.NPCInfoGridView.GridControl = this.NPCInfoGridControl;
            this.NPCInfoGridView.Name = "NPCInfoGridView";
            this.NPCInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.NPCInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.NPCInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.NPCInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colNPCName
            // 
            this.colNPCName.FieldName = "NPCName";
            this.colNPCName.Name = "colNPCName";
            this.colNPCName.Visible = true;
            this.colNPCName.VisibleIndex = 1;
            // 
            // colImage
            // 
            this.colImage.FieldName = "Image";
            this.colImage.Name = "colImage";
            this.colImage.Visible = true;
            this.colImage.VisibleIndex = 2;
            // 
            // colEntryPage
            // 
            this.colEntryPage.ColumnEdit = this.PageLookUpEdit;
            this.colEntryPage.FieldName = "EntryPage";
            this.colEntryPage.Name = "colEntryPage";
            this.colEntryPage.Visible = true;
            this.colEntryPage.VisibleIndex = 3;
            // 
            // PageLookUpEdit
            // 
            this.PageLookUpEdit.AutoHeight = false;
            this.PageLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.PageLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.PageLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Description"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DialogType", "DialogType"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Say", "Say")});
            this.PageLookUpEdit.DisplayMember = "Description";
            this.PageLookUpEdit.Name = "PageLookUpEdit";
            this.PageLookUpEdit.NullText = "[Page is null]";
            // 
            // gridColumn1
            // 
            this.gridColumn1.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn1.FieldName = "Region";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
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
            // colFaceImage
            // 
            this.colFaceImage.Caption = "Face Image";
            this.colFaceImage.FieldName = "FaceImage";
            this.colFaceImage.Name = "colFaceImage";
            this.colFaceImage.Visible = true;
            this.colFaceImage.VisibleIndex = 4;
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton,
            this.ribbon.SearchEditItem,
            this.barButtonItem1,
            this.ExportButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(736, 144);
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
            // ItemInfoLookUpEdit
            // 
            this.ItemInfoLookUpEdit.AutoHeight = false;
            this.ItemInfoLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.ItemInfoLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ItemInfoLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size")});
            this.ItemInfoLookUpEdit.Name = "ItemInfoLookUpEdit";
            this.ItemInfoLookUpEdit.NullText = "[Reward is null]";
            // 
            // JsonImportExport
            // 
            this.JsonImportExport.ItemLinks.Add(this.barButtonItem1);
            this.JsonImportExport.ItemLinks.Add(this.ExportButton);
            this.JsonImportExport.Name = "JsonImportExport";
            this.JsonImportExport.Text = "Json";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Import";
            this.barButtonItem1.Id = 2;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
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
            // NPCInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 571);
            this.Controls.Add(this.NPCInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "NPCInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "NPC Info";
            ((System.ComponentModel.ISupportInitialize)(this.RequirementGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequirementImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.QuestInfoLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RequiredClassImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DaysOfWeekImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PageLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoLookUpEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}