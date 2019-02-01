namespace Server.Views
{
    partial class SystemLogView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemLogView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ClearLogsButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.LogListBoxControl = new DevExpress.XtraEditors.ListBoxControl();
            this.InterfaceTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogListBoxControl)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ClearLogsButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 2;
            this.ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(717, 144);
            // 
            // ClearLogsButton
            // 
            this.ClearLogsButton.Caption = "Clear Logs";
            this.ClearLogsButton.Glyph = ((System.Drawing.Image)(resources.GetObject("ClearLogsButton.Glyph")));
            this.ClearLogsButton.Id = 1;
            this.ClearLogsButton.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("ClearLogsButton.LargeGlyph")));
            this.ClearLogsButton.LargeWidth = 50;
            this.ClearLogsButton.Name = "ClearLogsButton";
            this.ClearLogsButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ClearLogsButton_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.ClearLogsButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "Actions";
            // 
            // LogListBoxControl
            // 
            this.LogListBoxControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.LogListBoxControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogListBoxControl.ItemAutoHeight = true;
            this.LogListBoxControl.Location = new System.Drawing.Point(0, 144);
            this.LogListBoxControl.Name = "LogListBoxControl";
            this.LogListBoxControl.Size = new System.Drawing.Size(717, 339);
            this.LogListBoxControl.TabIndex = 1;
            // 
            // InterfaceTimer
            // 
            this.InterfaceTimer.Enabled = true;
            this.InterfaceTimer.Interval = 1000;
            this.InterfaceTimer.Tick += new System.EventHandler(this.InterfaceTimer_Tick);
            // 
            // SystemLogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 483);
            this.Controls.Add(this.LogListBoxControl);
            this.Controls.Add(this.ribbon);
            this.Name = "SystemLogView";
            this.Ribbon = this.ribbon;
            this.Text = "System Logs";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogListBoxControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem ClearLogsButton;
        private DevExpress.XtraEditors.ListBoxControl LogListBoxControl;
        private System.Windows.Forms.Timer InterfaceTimer;
    }
}