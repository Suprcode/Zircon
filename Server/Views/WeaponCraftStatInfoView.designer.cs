namespace Server.Views
{
    partial class WeaponCraftStatInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeaponCraftStatInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ItemInfoStatGridControl = new DevExpress.XtraGrid.GridControl();
            this.ItemInfoStatGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.StatImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.JsonImportButton = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ImportButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoStatGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoStatGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).BeginInit();
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
            this.ribbon.Size = new System.Drawing.Size(684, 144);
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
            this.JsonImportButton});
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
            // ItemInfoStatGridControl
            // 
            this.ItemInfoStatGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemInfoStatGridControl.Location = new System.Drawing.Point(0, 144);
            this.ItemInfoStatGridControl.MainView = this.ItemInfoStatGridView;
            this.ItemInfoStatGridControl.MenuManager = this.ribbon;
            this.ItemInfoStatGridControl.Name = "ItemInfoStatGridControl";
            this.ItemInfoStatGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.ItemLookUpEdit,
            this.StatImageComboBox});
            this.ItemInfoStatGridControl.Size = new System.Drawing.Size(684, 325);
            this.ItemInfoStatGridControl.TabIndex = 2;
            this.ItemInfoStatGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ItemInfoStatGridView});
            // 
            // ItemInfoStatGridView
            // 
            this.ItemInfoStatGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn5,
            this.gridColumn2,
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn4});
            this.ItemInfoStatGridView.GridControl = this.ItemInfoStatGridControl;
            this.ItemInfoStatGridView.Name = "ItemInfoStatGridView";
            this.ItemInfoStatGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.ItemInfoStatGridView.OptionsView.EnableAppearanceOddRow = true;
            this.ItemInfoStatGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.ItemInfoStatGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.ItemInfoStatGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn5
            // 
            this.gridColumn5.FieldName = "RequiredClass";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.ColumnEdit = this.StatImageComboBox;
            this.gridColumn2.FieldName = "Stat";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // StatImageComboBox
            // 
            this.StatImageComboBox.AutoHeight = false;
            this.StatImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.StatImageComboBox.Name = "StatImageComboBox";
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "MinValue";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 2;
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "MaxValue";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "Weight";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            // 
            // ItemLookUpEdit
            // 
            this.ItemLookUpEdit.AutoHeight = false;
            this.ItemLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemType", "Item Type"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Price", "Price"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("StackSize", "Stack Size")});
            this.ItemLookUpEdit.DisplayMember = "ItemName";
            this.ItemLookUpEdit.Name = "ItemLookUpEdit";
            this.ItemLookUpEdit.NullText = "[Item is null]";
            // 
            // JsonImportButton
            // 
            this.JsonImportButton.ItemLinks.Add(this.ImportButton);
            this.JsonImportButton.ItemLinks.Add(this.ExportButton);
            this.JsonImportButton.Name = "JsonImportButton";
            this.JsonImportButton.Text = "Json";
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
            // WeaponCraftStatInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 469);
            this.Controls.Add(this.ItemInfoStatGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "WeaponCraftStatInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "Weapon Craft Stat Info";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoStatGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemInfoStatGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl ItemInfoStatGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView ItemInfoStatGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportButton;
    }
}