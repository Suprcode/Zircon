namespace Server.Views
{
    partial class CompanionInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompanionInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.CompanionInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.CompanionInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MonsterInfoLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tabPane1 = new DevExpress.XtraBars.Navigation.TabPane();
            this.tabNavigationPage1 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.tabNavigationPage2 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.CompanionLevelInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.CompanionLevelInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.tabNavigationPage3 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.CompanionSkillInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.CompanionSkillInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ImportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterInfoLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabPane1)).BeginInit();
            this.tabPane1.SuspendLayout();
            this.tabNavigationPage1.SuspendLayout();
            this.tabNavigationPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionLevelInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionLevelInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).BeginInit();
            this.tabNavigationPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionSkillInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionSkillInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit2)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton,
            this.ribbon.SearchEditItem,
            this.ImportButton,
            this.ExportButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(972, 144);
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
            // CompanionInfoGridControl
            // 
            this.CompanionInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompanionInfoGridControl.Location = new System.Drawing.Point(0, 0);
            this.CompanionInfoGridControl.MainView = this.CompanionInfoGridView;
            this.CompanionInfoGridControl.MenuManager = this.ribbon;
            this.CompanionInfoGridControl.Name = "CompanionInfoGridControl";
            this.CompanionInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MonsterInfoLookUpEdit});
            this.CompanionInfoGridControl.Size = new System.Drawing.Size(972, 371);
            this.CompanionInfoGridControl.TabIndex = 2;
            this.CompanionInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CompanionInfoGridView});
            // 
            // CompanionInfoGridView
            // 
            this.CompanionInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn4});
            this.CompanionInfoGridView.GridControl = this.CompanionInfoGridControl;
            this.CompanionInfoGridView.Name = "CompanionInfoGridView";
            this.CompanionInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.CompanionInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.CompanionInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.CompanionInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.CompanionInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.ColumnEdit = this.MonsterInfoLookUpEdit;
            this.gridColumn1.FieldName = "MonsterInfo";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // MonsterInfoLookUpEdit
            // 
            this.MonsterInfoLookUpEdit.AutoHeight = false;
            this.MonsterInfoLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.MonsterInfoLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MonsterInfoLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AI", "AI"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Level", "Level"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Experience", "Experience"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "Is Boss")});
            this.MonsterInfoLookUpEdit.DisplayMember = "MonsterName";
            this.MonsterInfoLookUpEdit.Name = "MonsterInfoLookUpEdit";
            this.MonsterInfoLookUpEdit.NullText = "[Monster is null]";
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "Price";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "Available";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            // 
            // tabPane1
            // 
            this.tabPane1.Controls.Add(this.tabNavigationPage1);
            this.tabPane1.Controls.Add(this.tabNavigationPage2);
            this.tabPane1.Controls.Add(this.tabNavigationPage3);
            this.tabPane1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPane1.Location = new System.Drawing.Point(0, 144);
            this.tabPane1.Name = "tabPane1";
            this.tabPane1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.tabNavigationPage1,
            this.tabNavigationPage2,
            this.tabNavigationPage3});
            this.tabPane1.RegularSize = new System.Drawing.Size(972, 400);
            this.tabPane1.SelectedPage = this.tabNavigationPage1;
            this.tabPane1.Size = new System.Drawing.Size(972, 400);
            this.tabPane1.TabIndex = 3;
            // 
            // tabNavigationPage1
            // 
            this.tabNavigationPage1.Caption = "Companion Info";
            this.tabNavigationPage1.Controls.Add(this.CompanionInfoGridControl);
            this.tabNavigationPage1.Name = "tabNavigationPage1";
            this.tabNavigationPage1.Size = new System.Drawing.Size(972, 371);
            // 
            // tabNavigationPage2
            // 
            this.tabNavigationPage2.Caption = "Companion Level Info";
            this.tabNavigationPage2.Controls.Add(this.CompanionLevelInfoGridControl);
            this.tabNavigationPage2.Name = "tabNavigationPage2";
            this.tabNavigationPage2.Size = new System.Drawing.Size(972, 371);
            // 
            // CompanionLevelInfoGridControl
            // 
            this.CompanionLevelInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompanionLevelInfoGridControl.Location = new System.Drawing.Point(0, 0);
            this.CompanionLevelInfoGridControl.MainView = this.CompanionLevelInfoGridView;
            this.CompanionLevelInfoGridControl.MenuManager = this.ribbon;
            this.CompanionLevelInfoGridControl.Name = "CompanionLevelInfoGridControl";
            this.CompanionLevelInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemLookUpEdit1});
            this.CompanionLevelInfoGridControl.Size = new System.Drawing.Size(972, 371);
            this.CompanionLevelInfoGridControl.TabIndex = 3;
            this.CompanionLevelInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CompanionLevelInfoGridView});
            // 
            // CompanionLevelInfoGridView
            // 
            this.CompanionLevelInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
            this.CompanionLevelInfoGridView.GridControl = this.CompanionLevelInfoGridControl;
            this.CompanionLevelInfoGridView.Name = "CompanionLevelInfoGridView";
            this.CompanionLevelInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.CompanionLevelInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.CompanionLevelInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.CompanionLevelInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.CompanionLevelInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn2
            // 
            this.gridColumn2.FieldName = "Level";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // gridColumn5
            // 
            this.gridColumn5.FieldName = "MaxExperience";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 1;
            // 
            // gridColumn6
            // 
            this.gridColumn6.FieldName = "InventorySpace";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 2;
            // 
            // gridColumn7
            // 
            this.gridColumn7.FieldName = "InventoryWeight";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 3;
            // 
            // gridColumn8
            // 
            this.gridColumn8.FieldName = "MaxHunger";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 4;
            // 
            // repositoryItemLookUpEdit1
            // 
            this.repositoryItemLookUpEdit1.AutoHeight = false;
            this.repositoryItemLookUpEdit1.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemLookUpEdit1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AI", "AI"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Level", "Level"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Experience", "Experience"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "Is Boss")});
            this.repositoryItemLookUpEdit1.DisplayMember = "MonsterName";
            this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
            this.repositoryItemLookUpEdit1.NullText = "[Monster is null]";
            // 
            // tabNavigationPage3
            // 
            this.tabNavigationPage3.Caption = "Companion Skill Info";
            this.tabNavigationPage3.Controls.Add(this.CompanionSkillInfoGridControl);
            this.tabNavigationPage3.Name = "tabNavigationPage3";
            this.tabNavigationPage3.Size = new System.Drawing.Size(972, 371);
            // 
            // CompanionSkillInfoGridControl
            // 
            this.CompanionSkillInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompanionSkillInfoGridControl.Location = new System.Drawing.Point(0, 0);
            this.CompanionSkillInfoGridControl.MainView = this.CompanionSkillInfoGridView;
            this.CompanionSkillInfoGridControl.MenuManager = this.ribbon;
            this.CompanionSkillInfoGridControl.Name = "CompanionSkillInfoGridControl";
            this.CompanionSkillInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemLookUpEdit2});
            this.CompanionSkillInfoGridControl.Size = new System.Drawing.Size(972, 371);
            this.CompanionSkillInfoGridControl.TabIndex = 4;
            this.CompanionSkillInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CompanionSkillInfoGridView});
            // 
            // CompanionSkillInfoGridView
            // 
            this.CompanionSkillInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13});
            this.CompanionSkillInfoGridView.GridControl = this.CompanionSkillInfoGridControl;
            this.CompanionSkillInfoGridView.Name = "CompanionSkillInfoGridView";
            this.CompanionSkillInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.CompanionSkillInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.CompanionSkillInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.CompanionSkillInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.CompanionSkillInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn9
            // 
            this.gridColumn9.FieldName = "Level";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 0;
            // 
            // gridColumn10
            // 
            this.gridColumn10.FieldName = "StatType";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 1;
            // 
            // gridColumn11
            // 
            this.gridColumn11.FieldName = "MinAmount";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 2;
            // 
            // gridColumn12
            // 
            this.gridColumn12.FieldName = "MaxAmount";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 3;
            // 
            // gridColumn13
            // 
            this.gridColumn13.FieldName = "Weight";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 4;
            // 
            // repositoryItemLookUpEdit2
            // 
            this.repositoryItemLookUpEdit2.AutoHeight = false;
            this.repositoryItemLookUpEdit2.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.repositoryItemLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemLookUpEdit2.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AI", "AI"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Level", "Level"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Experience", "Experience"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "Is Boss")});
            this.repositoryItemLookUpEdit2.DisplayMember = "MonsterName";
            this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
            this.repositoryItemLookUpEdit2.NullText = "[Monster is null]";
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
            // CompanionInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 544);
            this.Controls.Add(this.tabPane1);
            this.Controls.Add(this.ribbon);
            this.Name = "CompanionInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Companion Info";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonsterInfoLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabPane1)).EndInit();
            this.tabPane1.ResumeLayout(false);
            this.tabNavigationPage1.ResumeLayout(false);
            this.tabNavigationPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CompanionLevelInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionLevelInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).EndInit();
            this.tabNavigationPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CompanionSkillInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanionSkillInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl CompanionInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CompanionInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraBars.Navigation.TabPane tabPane1;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage1;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage2;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MonsterInfoLookUpEdit;
        private DevExpress.XtraGrid.GridControl CompanionLevelInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CompanionLevelInfoGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage3;
        private DevExpress.XtraGrid.GridControl CompanionSkillInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CompanionSkillInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEdit2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
    }
}