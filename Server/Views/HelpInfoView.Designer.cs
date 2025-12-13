namespace Server.Views
{
    partial class HelpInfoView
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpInfoView));
            PageGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            HelpGridControl = new DevExpress.XtraGrid.GridControl();
            HelpGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colTitle = new DevExpress.XtraGrid.Columns.GridColumn();
            colOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            HelpMemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
            PageMemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
            SectionMemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
            SectionGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)PageGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HelpGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HelpGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HelpMemoEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PageMemoEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SectionMemoEdit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SectionGridView).BeginInit();
            SuspendLayout();
            // 
            // PageGridView
            // 
            PageGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn6, gridColumn7, gridColumn9 });
            PageGridView.DetailHeight = 325;
            PageGridView.GridControl = HelpGridControl;
            PageGridView.LevelIndent = 0;
            PageGridView.Name = "PageGridView";
            PageGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            PageGridView.OptionsView.EnableAppearanceEvenRow = true;
            PageGridView.OptionsView.EnableAppearanceOddRow = true;
            PageGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            PageGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            PageGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn6
            // 
            gridColumn6.Caption = "Title";
            gridColumn6.FieldName = "Title";
            gridColumn6.MinWidth = 17;
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 0;
            gridColumn6.Width = 64;
            // 
            // gridColumn7
            // 
            gridColumn7.Caption = "Order";
            gridColumn7.FieldName = "Order";
            gridColumn7.MinWidth = 17;
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 1;
            gridColumn7.Width = 64;
            // 
            // gridColumn9
            // 
            gridColumn9.Caption = "Sections";
            gridColumn9.FieldName = "Sections";
            gridColumn9.MinWidth = 17;
            gridColumn9.Name = "gridColumn9";
            gridColumn9.Width = 64;
            // 
            // HelpGridControl
            // 
            HelpGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = PageGridView;
            gridLevelNode2.LevelTemplate = SectionGridView;
            gridLevelNode2.RelationName = "Sections";
            gridLevelNode1.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode2 });
            gridLevelNode1.RelationName = "Pages";
            HelpGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1 });
            HelpGridControl.Location = new System.Drawing.Point(0, 144);
            HelpGridControl.MainView = HelpGridView;
            HelpGridControl.MenuManager = ribbon;
            HelpGridControl.Name = "HelpGridControl";
            HelpGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { HelpMemoEdit, PageMemoEdit, SectionMemoEdit });
            HelpGridControl.ShowOnlyPredefinedDetails = true;
            HelpGridControl.Size = new System.Drawing.Size(644, 323);
            HelpGridControl.TabIndex = 2;
            HelpGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { HelpGridView, SectionGridView, PageGridView });
            // 
            // HelpGridView
            // 
            HelpGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colTitle, colOrder, colDescription });
            HelpGridView.DetailHeight = 325;
            HelpGridView.GridControl = HelpGridControl;
            HelpGridView.Name = "HelpGridView";
            HelpGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            HelpGridView.OptionsView.EnableAppearanceEvenRow = true;
            HelpGridView.OptionsView.EnableAppearanceOddRow = true;
            HelpGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            HelpGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            HelpGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colTitle
            // 
            colTitle.Caption = "Title";
            colTitle.FieldName = "Title";
            colTitle.MinWidth = 17;
            colTitle.Name = "colTitle";
            colTitle.Visible = true;
            colTitle.VisibleIndex = 0;
            colTitle.Width = 64;
            // 
            // colOrder
            // 
            colOrder.Caption = "Order";
            colOrder.FieldName = "Order";
            colOrder.MinWidth = 17;
            colOrder.Name = "colOrder";
            colOrder.Visible = true;
            colOrder.VisibleIndex = 1;
            colOrder.Width = 64;
            // 
            // colDescription
            // 
            colDescription.Caption = "Description";
            colDescription.FieldName = "Description";
            colDescription.MinWidth = 17;
            colDescription.Name = "colDescription";
            colDescription.Visible = true;
            colDescription.VisibleIndex = 2;
            colDescription.Width = 64;
            // 
            // ribbon
            // 
            ribbon.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(26, 28, 26, 28);
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, SaveButton, ImportButton, ExportButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 5;
            ribbon.Name = "ribbon";
            ribbon.OptionsMenuMinWidth = 283;
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(644, 144);
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
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1, JsonGroup });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.ItemLinks.Add(SaveButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Saving";
            // 
            // JsonGroup
            // 
            JsonGroup.ItemLinks.Add(ImportButton);
            JsonGroup.ItemLinks.Add(ExportButton);
            JsonGroup.Name = "JsonGroup";
            JsonGroup.Text = "Json";
            // 
            // HelpMemoEdit
            // 
            HelpMemoEdit.AutoHeight = false;
            HelpMemoEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            HelpMemoEdit.Name = "HelpMemoEdit";
            HelpMemoEdit.ShowIcon = false;
            // 
            // PageMemoEdit
            // 
            PageMemoEdit.AutoHeight = false;
            PageMemoEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            PageMemoEdit.Name = "PageMemoEdit";
            PageMemoEdit.ShowIcon = false;
            // 
            // SectionMemoEdit
            // 
            SectionMemoEdit.AutoHeight = false;
            SectionMemoEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            SectionMemoEdit.Name = "SectionMemoEdit";
            // 
            // SectionGridView
            // 
            SectionGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn10, gridColumn11, gridColumn13 });
            SectionGridView.DetailHeight = 325;
            SectionGridView.GridControl = HelpGridControl;
            SectionGridView.Name = "SectionGridView";
            SectionGridView.OptionsDetail.EnableMasterViewMode = false;
            SectionGridView.OptionsView.EnableAppearanceEvenRow = true;
            SectionGridView.OptionsView.EnableAppearanceOddRow = true;
            SectionGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            SectionGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            SectionGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn10
            // 
            gridColumn10.Caption = "Title";
            gridColumn10.FieldName = "Title";
            gridColumn10.MinWidth = 17;
            gridColumn10.Name = "gridColumn10";
            gridColumn10.Visible = true;
            gridColumn10.VisibleIndex = 0;
            gridColumn10.Width = 64;
            // 
            // gridColumn11
            // 
            gridColumn11.Caption = "Order";
            gridColumn11.FieldName = "Order";
            gridColumn11.MinWidth = 17;
            gridColumn11.Name = "gridColumn11";
            gridColumn11.Visible = true;
            gridColumn11.VisibleIndex = 1;
            gridColumn11.Width = 64;
            // 
            // gridColumn13
            // 
            gridColumn13.Caption = "Content";
            gridColumn13.ColumnEdit = SectionMemoEdit;
            gridColumn13.FieldName = "Content";
            gridColumn13.MinWidth = 17;
            gridColumn13.Name = "gridColumn13";
            gridColumn13.Visible = true;
            gridColumn13.VisibleIndex = 2;
            gridColumn13.Width = 64;
            // 
            // HelpInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(644, 467);
            Controls.Add(HelpGridControl);
            Controls.Add(ribbon);
            Name = "HelpInfoView";
            Ribbon = ribbon;
            Text = "Help";
            ((System.ComponentModel.ISupportInitialize)PageGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)HelpGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)HelpGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)HelpMemoEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)PageMemoEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)SectionMemoEdit).EndInit();
            ((System.ComponentModel.ISupportInitialize)SectionGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonGroup;
        private DevExpress.XtraGrid.GridControl HelpGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView HelpGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView PageGridView;
        private DevExpress.XtraGrid.Views.Grid.GridView SectionGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colTitle;
        private DevExpress.XtraGrid.Columns.GridColumn colOrder;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit HelpMemoEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit PageMemoEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit SectionMemoEdit;
    }
}