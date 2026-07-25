namespace Server.Views
{
    partial class DiagnosticView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticView));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            DiagnosticButton = new DevExpress.XtraBars.BarButtonItem();
            ResetTimeButton = new DevExpress.XtraBars.BarButtonItem();
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
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemToggleSwitch1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridView).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, DiagnosticButton, ResetTimeButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 6;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { repositoryItemToggleSwitch1 });
            ribbon.Size = new System.Drawing.Size(907, 144);
            // 
            // DiagnosticButton
            // 
            DiagnosticButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            DiagnosticButton.Caption = "Diagnostics";
            DiagnosticButton.Id = 4;
            DiagnosticButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("DiagnosticButton.ImageOptions.Image");
            DiagnosticButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("DiagnosticButton.ImageOptions.LargeImage");
            DiagnosticButton.Name = "DiagnosticButton";
            DiagnosticButton.DownChanged += DiagnosticButton_DownChanged;
            // 
            // ResetTimeButton
            // 
            ResetTimeButton.Caption = "Reset Times";
            ResetTimeButton.Id = 5;
            ResetTimeButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ResetTimeButton.ImageOptions.Image");
            ResetTimeButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ResetTimeButton.ImageOptions.LargeImage");
            ResetTimeButton.LargeWidth = 60;
            ResetTimeButton.Name = "ResetTimeButton";
            ResetTimeButton.ItemClick += ResetTimeButton_ItemClick;
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
            ribbonPageGroup1.ItemLinks.Add(DiagnosticButton);
            ribbonPageGroup1.ItemLinks.Add(ResetTimeButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Saving";
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
            DiagnosticGridControl.Location = new System.Drawing.Point(0, 144);
            DiagnosticGridControl.MainView = DiagnosticGridView;
            DiagnosticGridControl.MenuManager = ribbon;
            DiagnosticGridControl.Name = "DiagnosticGridControl";
            DiagnosticGridControl.Size = new System.Drawing.Size(907, 443);
            DiagnosticGridControl.TabIndex = 3;
            DiagnosticGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { DiagnosticGridView });
            // 
            // DiagnosticGridView
            // 
            DiagnosticGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1, gridColumn2, gridColumn3, gridColumn4, gridColumn5, gridColumn6 });
            DiagnosticGridView.GridControl = DiagnosticGridControl;
            DiagnosticGridView.Name = "DiagnosticGridView";
            DiagnosticGridView.OptionsView.EnableAppearanceEvenRow = true;
            DiagnosticGridView.OptionsView.EnableAppearanceOddRow = true;
            DiagnosticGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            DiagnosticGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            gridColumn1.FieldName = "Name";
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            gridColumn2.FieldName = "Count";
            gridColumn2.Name = "gridColumn2";
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            gridColumn3.FieldName = "TotalMilliseconds";
            gridColumn3.Name = "gridColumn3";
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            gridColumn4.FieldName = "LargestMilliseconds";
            gridColumn4.Name = "gridColumn4";
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            gridColumn5.FieldName = "TotalSize";
            gridColumn5.Name = "gridColumn5";
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            gridColumn6.FieldName = "LargestSize";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 5;
            // 
            // DiagnosticView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(907, 587);
            Controls.Add(DiagnosticGridControl);
            Controls.Add(ribbon);
            Name = "DiagnosticView";
            Ribbon = ribbon;
            Text = "Diagnostics";
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)repositoryItemToggleSwitch1).EndInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)DiagnosticGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem DiagnosticButton;
        private DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch repositoryItemToggleSwitch1;
        private DevExpress.XtraBars.BarButtonItem ResetTimeButton;
        private DevExpress.XtraGrid.GridControl DiagnosticGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView DiagnosticGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
    }
}
