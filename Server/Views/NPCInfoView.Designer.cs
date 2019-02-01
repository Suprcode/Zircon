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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NPCInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.NPCInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.NPCInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colNPCName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEntryPage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.PageLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PageLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 2;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(736, 144);
            // 
            // SaveButton
            // 
            this.SaveButton.Caption = "Save Database";
            this.SaveButton.Glyph = ((System.Drawing.Image)(resources.GetObject("SaveButton.Glyph")));
            this.SaveButton.Id = 1;
            this.SaveButton.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("SaveButton.LargeGlyph")));
            this.SaveButton.LargeWidth = 60;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SaveButton_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.ItemLinks.Add(this.SaveButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "Saving";
            // 
            // NPCInfoGridControl
            // 
            this.NPCInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NPCInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.NPCInfoGridControl.MainView = this.NPCInfoGridView;
            this.NPCInfoGridControl.MenuManager = this.ribbon;
            this.NPCInfoGridControl.Name = "NPCInfoGridControl";
            this.NPCInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RegionLookUpEdit,
            this.PageLookUpEdit});
            this.NPCInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.NPCInfoGridControl.Size = new System.Drawing.Size(736, 427);
            this.NPCInfoGridControl.TabIndex = 2;
            this.NPCInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.NPCInfoGridView});
            // 
            // NPCInfoGridView
            // 
            this.NPCInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colNPCName,
            this.colImage,
            this.colEntryPage,
            this.gridColumn1});
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
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PageLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).EndInit();
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
    }
}