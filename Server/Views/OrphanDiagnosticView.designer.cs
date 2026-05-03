namespace Server.Views
{
    partial class OrphanDiagnosticView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrphanDiagnosticView));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            ScanOrphansButton = new DevExpress.XtraBars.BarButtonItem();
            CleanOrphansButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            repositoryItemToggleSwitch1 = new DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch();
            DiagnosticGridControl = new DevExpress.XtraGrid.GridControl();
            DiagnosticGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemToggleSwitch1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)memoEdit1.Properties).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ScanOrphansButton, CleanOrphansButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 8;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemToggleSwitch1 });
            ribbon.Size = new System.Drawing.Size(865, 144);
            // 
            // ScanOrphansButton
            // 
            ScanOrphansButton.Caption = "Scan";
            ScanOrphansButton.Id = 6;
            ScanOrphansButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ScanOrphansButton.ImageOptions.Image");
            ScanOrphansButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ScanOrphansButton.ImageOptions.LargeImage");
            ScanOrphansButton.Name = "ScanOrphansButton";
            ScanOrphansButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            ScanOrphansButton.ItemClick += ScanOrphansButton_ItemClick;
            // 
            // CleanOrphansButton
            // 
            CleanOrphansButton.Caption = "Clean";
            CleanOrphansButton.Id = 7;
            CleanOrphansButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("CleanOrphansButton.ImageOptions.Image");
            CleanOrphansButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("CleanOrphansButton.ImageOptions.LargeImage");
            CleanOrphansButton.Name = "CleanOrphansButton";
            CleanOrphansButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            CleanOrphansButton.ItemClick += CleanOrphansButton_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(ScanOrphansButton);
            ribbonPageGroup1.ItemLinks.Add(CleanOrphansButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Diagnostics";
            // 
            // repositoryItemToggleSwitch1
            // 
            repositoryItemToggleSwitch1.AutoHeight = false;
            repositoryItemToggleSwitch1.Name = "repositoryItemToggleSwitch1";
            repositoryItemToggleSwitch1.OffText = "Off";
            repositoryItemToggleSwitch1.OnText = "On";
            // 
            // DiagnosticGridControl
            // 
            DiagnosticGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            DiagnosticGridControl.Location = new System.Drawing.Point(3, 3);
            DiagnosticGridControl.MainView = DiagnosticGridView;
            DiagnosticGridControl.MenuManager = ribbon;
            DiagnosticGridControl.Name = "DiagnosticGridControl";
            DiagnosticGridControl.Size = new System.Drawing.Size(859, 300);
            DiagnosticGridControl.TabIndex = 3;
            DiagnosticGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { DiagnosticGridView });
            // 
            // DiagnosticGridView
            // 
            DiagnosticGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1, gridColumn2, gridColumn3, gridColumn4, gridColumn5, gridColumn6, gridColumn7, gridColumn8, gridColumn9, gridColumn10, gridColumn11, gridColumn12, gridColumn13 });
            DiagnosticGridView.GridControl = DiagnosticGridControl;
            DiagnosticGridView.Name = "DiagnosticGridView";
            DiagnosticGridView.OptionsView.EnableAppearanceEvenRow = true;
            DiagnosticGridView.OptionsView.EnableAppearanceOddRow = true;
            DiagnosticGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            DiagnosticGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            gridColumn1.FieldName = "ObjectType";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            gridColumn2.FieldName = "ParentType";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            gridColumn3.FieldName = "ParentProperty";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            gridColumn4.FieldName = "ParentList";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            gridColumn5.FieldName = "TotalRows";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            gridColumn6.FieldName = "LinkedRows";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            gridColumn7.FieldName = "CleanableOrphans";
            gridColumn7.Name = "gridColumn7";
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            gridColumn8.FieldName = "ExistingTemporaryOrphans";
            gridColumn8.Name = "gridColumn8";
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 7;
            // 
            // gridColumn9
            // 
            gridColumn9.FieldName = "MissingParent";
            gridColumn9.Name = "gridColumn9";
            gridColumn9.Visible = true;
            gridColumn9.VisibleIndex = 8;
            // 
            // gridColumn10
            // 
            gridColumn10.FieldName = "DeletedParent";
            gridColumn10.Name = "gridColumn10";
            gridColumn10.Visible = true;
            gridColumn10.VisibleIndex = 9;
            // 
            // gridColumn11
            // 
            gridColumn11.FieldName = "MissingParentListLink";
            gridColumn11.Name = "gridColumn11";
            gridColumn11.Visible = true;
            gridColumn11.VisibleIndex = 10;
            // 
            // gridColumn12
            // 
            gridColumn12.FieldName = "MarkedTemporary";
            gridColumn12.Name = "gridColumn12";
            gridColumn12.Visible = true;
            gridColumn12.VisibleIndex = 11;
            // 
            // gridColumn13
            // 
            gridColumn13.FieldName = "SampleIndices";
            gridColumn13.Name = "gridColumn13";
            gridColumn13.Visible = true;
            gridColumn13.VisibleIndex = 12;
            // 
            // memoEdit1
            // 
            memoEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            memoEdit1.Location = new System.Drawing.Point(3, 309);
            memoEdit1.MenuManager = ribbon;
            memoEdit1.Name = "memoEdit1";
            memoEdit1.Size = new System.Drawing.Size(859, 105);
            memoEdit1.TabIndex = 5;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(DiagnosticGridControl, 0, 0);
            tableLayoutPanel1.Controls.Add(memoEdit1, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 144);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 111F));
            tableLayoutPanel1.Size = new System.Drawing.Size(865, 417);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // OrphanDiagnosticView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(865, 561);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(ribbon);
            Name = "OrphanDiagnosticView";
            Ribbon = ribbon;
            Text = "Orphan Diagnostics";
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemToggleSwitch1).EndInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)memoEdit1.Properties).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch repositoryItemToggleSwitch1;
        private DevExpress.XtraGrid.GridControl DiagnosticGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView DiagnosticGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraBars.BarButtonItem ScanOrphansButton;
        private DevExpress.XtraBars.BarButtonItem CleanOrphansButton;
        private DevExpress.XtraEditors.MemoEdit memoEdit1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
