namespace Server.Views
{
    partial class DiagnosticView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagnosticView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.DiagnosticGridControl = new DevExpress.XtraGrid.GridControl();
            this.DiagnosticGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemToggleSwitch1 = new DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch();
            this.DiagnosticButton = new DevExpress.XtraBars.BarButtonItem();
            this.ResetTimeButton = new DevExpress.XtraBars.BarButtonItem();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiagnosticGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiagnosticGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemToggleSwitch1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.DiagnosticButton,
            this.ResetTimeButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 6;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemToggleSwitch1});
            this.ribbon.Size = new System.Drawing.Size(690, 144);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.DiagnosticButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.ResetTimeButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "Saving";
            // 
            // DiagnosticGridControl
            // 
            this.DiagnosticGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DiagnosticGridControl.Location = new System.Drawing.Point(0, 144);
            this.DiagnosticGridControl.MainView = this.DiagnosticGridView;
            this.DiagnosticGridControl.MenuManager = this.ribbon;
            this.DiagnosticGridControl.Name = "DiagnosticGridControl";
            this.DiagnosticGridControl.Size = new System.Drawing.Size(690, 328);
            this.DiagnosticGridControl.TabIndex = 2;
            this.DiagnosticGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.DiagnosticGridView});
            // 
            // DiagnosticGridView
            // 
            this.DiagnosticGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6});
            this.DiagnosticGridView.GridControl = this.DiagnosticGridControl;
            this.DiagnosticGridView.Name = "DiagnosticGridView";
            this.DiagnosticGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.DiagnosticGridView.OptionsView.EnableAppearanceOddRow = true;
            this.DiagnosticGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.DiagnosticGridView.OptionsView.ShowGroupPanel = false;
            // 
            // repositoryItemToggleSwitch1
            // 
            this.repositoryItemToggleSwitch1.AutoHeight = false;
            this.repositoryItemToggleSwitch1.Name = "repositoryItemToggleSwitch1";
            this.repositoryItemToggleSwitch1.OffText = "Off";
            this.repositoryItemToggleSwitch1.OnText = "On";
            // 
            // DiagnosticButton
            // 
            this.DiagnosticButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.DiagnosticButton.Caption = "Diagnostics";
            this.DiagnosticButton.Glyph = ((System.Drawing.Image)(resources.GetObject("DiagnosticButton.Glyph")));
            this.DiagnosticButton.Id = 4;
            this.DiagnosticButton.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("DiagnosticButton.LargeGlyph")));
            this.DiagnosticButton.Name = "DiagnosticButton";
            this.DiagnosticButton.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.DiagnosticButton_DownChanged);
            // 
            // ResetTimeButton
            // 
            this.ResetTimeButton.Caption = "Reset Times";
            this.ResetTimeButton.Glyph = ((System.Drawing.Image)(resources.GetObject("ResetTimeButton.Glyph")));
            this.ResetTimeButton.Id = 5;
            this.ResetTimeButton.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("ResetTimeButton.LargeGlyph")));
            this.ResetTimeButton.LargeWidth = 60;
            this.ResetTimeButton.Name = "ResetTimeButton";
            this.ResetTimeButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ResetTimeButton_ItemClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "Name";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.FieldName = "Count";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "TotalMilliseconds";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "LargestMilliseconds";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.FieldName = "TotalSize";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            this.gridColumn6.FieldName = "LargestSize";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // DiagnosticView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 472);
            this.Controls.Add(this.DiagnosticGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "DiagnosticView";
            this.Ribbon = this.ribbon;
            this.Text = "Diagnostics";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiagnosticGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiagnosticGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemToggleSwitch1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraGrid.GridControl DiagnosticGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView DiagnosticGridView;
        private DevExpress.XtraBars.BarButtonItem DiagnosticButton;
        private DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch repositoryItemToggleSwitch1;
        private DevExpress.XtraBars.BarButtonItem ResetTimeButton;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
    }
}