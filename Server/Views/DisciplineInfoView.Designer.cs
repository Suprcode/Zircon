namespace Server.Views
{
    partial class DisciplineInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisciplineInfoView));
            this.DisciplineInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.DisciplineInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colLevel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRequiredLevel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRequiredExperience = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRequiredGold = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFocusPoints = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveDatabaseButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ImportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.DisciplineInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DisciplineInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.SuspendLayout();
            // 
            // DisciplineInfoGridControl
            // 
            this.DisciplineInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisciplineInfoGridControl.Location = new System.Drawing.Point(0, 144);
            this.DisciplineInfoGridControl.MainView = this.DisciplineInfoGridView;
            this.DisciplineInfoGridControl.MenuManager = this.ribbon;
            this.DisciplineInfoGridControl.Name = "DisciplineInfoGridControl";
            this.DisciplineInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.DisciplineInfoGridControl.Size = new System.Drawing.Size(713, 335);
            this.DisciplineInfoGridControl.TabIndex = 3;
            this.DisciplineInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.DisciplineInfoGridView});
            // 
            // DisciplineInfoGridView
            // 
            this.DisciplineInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colLevel,
            this.colRequiredLevel,
            this.colRequiredExperience,
            this.colRequiredGold,
            this.colFocusPoints});
            this.DisciplineInfoGridView.GridControl = this.DisciplineInfoGridControl;
            this.DisciplineInfoGridView.Name = "DisciplineInfoGridView";
            this.DisciplineInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.DisciplineInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.DisciplineInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.DisciplineInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.DisciplineInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.DisciplineInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colLevel
            // 
            this.colLevel.Caption = "Level";
            this.colLevel.FieldName = "Level";
            this.colLevel.Name = "colLevel";
            this.colLevel.Visible = true;
            this.colLevel.VisibleIndex = 0;
            // 
            // colRequiredLevel
            // 
            this.colRequiredLevel.Caption = "Required Level";
            this.colRequiredLevel.FieldName = "RequiredLevel";
            this.colRequiredLevel.Name = "colRequiredLevel";
            this.colRequiredLevel.Visible = true;
            this.colRequiredLevel.VisibleIndex = 1;
            // 
            // colRequiredExperience
            // 
            this.colRequiredExperience.Caption = "Required Experience";
            this.colRequiredExperience.FieldName = "RequiredExperience";
            this.colRequiredExperience.Name = "colRequiredExperience";
            this.colRequiredExperience.Visible = true;
            this.colRequiredExperience.VisibleIndex = 2;
            // 
            // colRequiredGold
            // 
            this.colRequiredGold.Caption = "Required Gold";
            this.colRequiredGold.FieldName = "RequiredGold";
            this.colRequiredGold.Name = "colRequiredGold";
            this.colRequiredGold.Visible = true;
            this.colRequiredGold.VisibleIndex = 3;
            // 
            // colFocusPoints
            // 
            this.colFocusPoints.Caption = "Focus Points";
            this.colFocusPoints.FieldName = "FocusPoints";
            this.colFocusPoints.Name = "colFocusPoints";
            this.colFocusPoints.Visible = true;
            this.colFocusPoints.VisibleIndex = 4;
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ribbon.SearchEditItem,
            this.SaveDatabaseButton,
            this.ImportButton,
            this.ExportButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(713, 144);
            // 
            // SaveDatabaseButton
            // 
            this.SaveDatabaseButton.Caption = "Save Database";
            this.SaveDatabaseButton.Id = 1;
            this.SaveDatabaseButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SaveDatabaseButton.ImageOptions.Image")));
            this.SaveDatabaseButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SaveDatabaseButton.ImageOptions.LargeImage")));
            this.SaveDatabaseButton.LargeWidth = 60;
            this.SaveDatabaseButton.Name = "SaveDatabaseButton";
            this.SaveDatabaseButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SaveDatabaseButton_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.SaveDatabaseButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Saving";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 2;
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
            // DisciplineInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 479);
            this.Controls.Add(this.DisciplineInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "DisciplineInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Discipline Info";
            ((System.ComponentModel.ISupportInitialize)(this.DisciplineInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DisciplineInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveDatabaseButton;
        private DevExpress.XtraGrid.Views.Grid.GridView DisciplineInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colLevel;
        private DevExpress.XtraGrid.GridControl DisciplineInfoGridControl;
        private DevExpress.XtraGrid.Columns.GridColumn colRequiredLevel;
        private DevExpress.XtraGrid.Columns.GridColumn colRequiredExperience;
        private DevExpress.XtraGrid.Columns.GridColumn colRequiredGold;
        private DevExpress.XtraGrid.Columns.GridColumn colFocusPoints;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
    }
}