namespace Server.Views
{
    partial class MagicInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MagicInfoView));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            ImportButton = new DevExpress.XtraBars.BarButtonItem();
            ExportButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            JsonImportExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            MagicImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            SchoolImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            ClassImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            MagicInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn19 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn20 = new DevExpress.XtraGrid.Columns.GridColumn();
            MagicInfoGridControl = new DevExpress.XtraGrid.GridControl();
            PropertyImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            gridColumn21 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MagicImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SchoolImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ClassImageComboBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MagicInfoGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MagicInfoGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PropertyImageComboBox).BeginInit();
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
            ribbon.Size = new System.Drawing.Size(896, 144);
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
            // MagicImageComboBox
            // 
            MagicImageComboBox.AutoHeight = false;
            MagicImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            MagicImageComboBox.Name = "MagicImageComboBox";
            // 
            // SchoolImageComboBox
            // 
            SchoolImageComboBox.AutoHeight = false;
            SchoolImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            SchoolImageComboBox.Name = "SchoolImageComboBox";
            // 
            // ClassImageComboBox
            // 
            ClassImageComboBox.AutoHeight = false;
            ClassImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            ClassImageComboBox.Name = "ClassImageComboBox";
            // 
            // MagicInfoGridView
            // 
            MagicInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn15, gridColumn1, gridColumn2, gridColumn3, gridColumn21, gridColumn4, gridColumn16, gridColumn5, gridColumn6, gridColumn7, gridColumn13, gridColumn8, gridColumn17, gridColumn9, gridColumn10, gridColumn11, gridColumn12, gridColumn18, gridColumn19, gridColumn14, gridColumn20 });
            MagicInfoGridView.GridControl = MagicInfoGridControl;
            MagicInfoGridView.Name = "MagicInfoGridView";
            MagicInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            MagicInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            MagicInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            MagicInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            MagicInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn15
            // 
            gridColumn15.FieldName = "Index";
            gridColumn15.Name = "gridColumn15";
            gridColumn15.OptionsColumn.AllowEdit = false;
            gridColumn15.OptionsColumn.ReadOnly = true;
            gridColumn15.Visible = true;
            gridColumn15.VisibleIndex = 0;
            // 
            // gridColumn1
            // 
            gridColumn1.FieldName = "Name";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 1;
            // 
            // gridColumn2
            // 
            gridColumn2.ColumnEdit = MagicImageComboBox;
            gridColumn2.FieldName = "Magic";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 2;
            // 
            // gridColumn3
            // 
            gridColumn3.ColumnEdit = SchoolImageComboBox;
            gridColumn3.FieldName = "School";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 3;
            // 
            // gridColumn4
            // 
            gridColumn4.ColumnEdit = ClassImageComboBox;
            gridColumn4.FieldName = "Class";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 5;
            // 
            // gridColumn16
            // 
            gridColumn16.FieldName = "Icon";
            gridColumn16.Name = "gridColumn16";
            gridColumn16.Visible = true;
            gridColumn16.VisibleIndex = 6;
            // 
            // gridColumn5
            // 
            gridColumn5.FieldName = "BaseCost";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 7;
            // 
            // gridColumn6
            // 
            gridColumn6.FieldName = "LevelCost";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 8;
            // 
            // gridColumn7
            // 
            gridColumn7.FieldName = "MinBasePower";
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 9;
            // 
            // gridColumn13
            // 
            gridColumn13.FieldName = "MaxBasePower";
            gridColumn13.Name = "gridColumn13";
            gridColumn13.Visible = true;
            gridColumn13.VisibleIndex = 10;
            // 
            // gridColumn8
            // 
            gridColumn8.FieldName = "MinLevelPower";
            gridColumn8.Name = "gridColumn8";
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 11;
            // 
            // gridColumn17
            // 
            gridColumn17.FieldName = "MaxLevelPower";
            gridColumn17.Name = "gridColumn17";
            gridColumn17.Visible = true;
            gridColumn17.VisibleIndex = 12;
            // 
            // gridColumn9
            // 
            gridColumn9.Caption = "Need Level 1";
            gridColumn9.FieldName = "NeedLevel1";
            gridColumn9.Name = "gridColumn9";
            gridColumn9.Visible = true;
            gridColumn9.VisibleIndex = 13;
            // 
            // gridColumn10
            // 
            gridColumn10.Caption = "Need Level 2";
            gridColumn10.FieldName = "NeedLevel2";
            gridColumn10.Name = "gridColumn10";
            gridColumn10.Visible = true;
            gridColumn10.VisibleIndex = 14;
            // 
            // gridColumn11
            // 
            gridColumn11.Caption = "Need Level 3";
            gridColumn11.FieldName = "NeedLevel3";
            gridColumn11.Name = "gridColumn11";
            gridColumn11.Visible = true;
            gridColumn11.VisibleIndex = 15;
            // 
            // gridColumn12
            // 
            gridColumn12.Caption = "Experience 1";
            gridColumn12.FieldName = "Experience1";
            gridColumn12.Name = "gridColumn12";
            gridColumn12.Visible = true;
            gridColumn12.VisibleIndex = 16;
            // 
            // gridColumn18
            // 
            gridColumn18.Caption = "Experience 2";
            gridColumn18.FieldName = "Experience2";
            gridColumn18.Name = "gridColumn18";
            gridColumn18.Visible = true;
            gridColumn18.VisibleIndex = 17;
            // 
            // gridColumn19
            // 
            gridColumn19.Caption = "Experience 3";
            gridColumn19.FieldName = "Experience3";
            gridColumn19.Name = "gridColumn19";
            gridColumn19.Visible = true;
            gridColumn19.VisibleIndex = 18;
            // 
            // gridColumn14
            // 
            gridColumn14.FieldName = "Delay";
            gridColumn14.Name = "gridColumn14";
            gridColumn14.Visible = true;
            gridColumn14.VisibleIndex = 19;
            // 
            // gridColumn20
            // 
            gridColumn20.FieldName = "Description";
            gridColumn20.Name = "gridColumn20";
            gridColumn20.Visible = true;
            gridColumn20.VisibleIndex = 20;
            // 
            // MagicInfoGridControl
            // 
            MagicInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            MagicInfoGridControl.Location = new System.Drawing.Point(0, 144);
            MagicInfoGridControl.MainView = MagicInfoGridView;
            MagicInfoGridControl.MenuManager = ribbon;
            MagicInfoGridControl.Name = "MagicInfoGridControl";
            MagicInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { MagicImageComboBox, SchoolImageComboBox, ClassImageComboBox, PropertyImageComboBox });
            MagicInfoGridControl.Size = new System.Drawing.Size(896, 395);
            MagicInfoGridControl.TabIndex = 2;
            MagicInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { MagicInfoGridView });
            // 
            // PropertyImageComboBox
            // 
            PropertyImageComboBox.AutoHeight = false;
            PropertyImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            PropertyImageComboBox.Name = "PropertyImageComboBox";
            // 
            // gridColumn21
            // 
            gridColumn21.ColumnEdit = PropertyImageComboBox;
            gridColumn21.FieldName = "Property";
            gridColumn21.Name = "gridColumn21";
            gridColumn21.Visible = true;
            gridColumn21.VisibleIndex = 4;
            // 
            // MagicInfoView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(896, 539);
            Controls.Add(MagicInfoGridControl);
            Controls.Add(ribbon);
            Name = "MagicInfoView";
            Ribbon = ribbon;
            Text = "Magic Info";
            Load += MagicInfoView_Load;
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)MagicImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)SchoolImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)ClassImageComboBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)MagicInfoGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)MagicInfoGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)PropertyImageComboBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox MagicImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox SchoolImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox ClassImageComboBox;
        private DevExpress.XtraGrid.Views.Grid.GridView MagicInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.GridControl MagicInfoGridControl;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn18;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn19;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn20;
        private DevExpress.XtraBars.BarButtonItem ImportButton;
        private DevExpress.XtraBars.BarButtonItem ExportButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup JsonImportExport;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn21;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox PropertyImageComboBox;
    }
}