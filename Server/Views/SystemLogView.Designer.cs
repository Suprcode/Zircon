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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemLogView));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            ClearLogsButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            LogListBoxControl = new DevExpress.XtraEditors.ListBoxControl();
            InterfaceTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LogListBoxControl).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ClearLogsButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 2;
            ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
            ribbon.Name = "ribbon";
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.Size = new System.Drawing.Size(675, 144);
            // 
            // ClearLogsButton
            // 
            ClearLogsButton.Caption = "Clear Logs";
            ClearLogsButton.Id = 1;
            ClearLogsButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ClearLogsButton.ImageOptions.Image");
            ClearLogsButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ClearLogsButton.ImageOptions.LargeImage");
            ClearLogsButton.LargeWidth = 50;
            ClearLogsButton.Name = "ClearLogsButton";
            ClearLogsButton.ItemClick += ClearLogsButton_ItemClick;
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
            ribbonPageGroup1.ItemLinks.Add(ClearLogsButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "Actions";
            // 
            // LogListBoxControl
            // 
            LogListBoxControl.Dock = System.Windows.Forms.DockStyle.Fill;
            LogListBoxControl.ItemAutoHeight = true;
            LogListBoxControl.Location = new System.Drawing.Point(0, 144);
            LogListBoxControl.Name = "LogListBoxControl";
            LogListBoxControl.Size = new System.Drawing.Size(675, 318);
            LogListBoxControl.TabIndex = 1;
            // 
            // InterfaceTimer
            // 
            InterfaceTimer.Enabled = true;
            InterfaceTimer.Interval = 1000;
            InterfaceTimer.Tick += InterfaceTimer_Tick;
            // 
            // SystemLogView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(675, 462);
            Controls.Add(LogListBoxControl);
            Controls.Add(ribbon);
            Name = "SystemLogView";
            Ribbon = ribbon;
            Text = "System Logs";
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)LogListBoxControl).EndInit();
            ResumeLayout(false);
            PerformLayout();

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