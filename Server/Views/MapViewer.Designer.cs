namespace Server.Views
{
    partial class MapViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapViewer));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ZoomResetButton = new DevExpress.XtraBars.BarButtonItem();
            this.ZoomInButton = new DevExpress.XtraBars.BarButtonItem();
            this.ZoomOutButton = new DevExpress.XtraBars.BarButtonItem();
            this.AttributesButton = new DevExpress.XtraBars.BarButtonItem();
            this.SelectionButton = new DevExpress.XtraBars.BarButtonItem();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.CancelButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.DXPanel = new DevExpress.XtraEditors.PanelControl();
            this.MapVScroll = new DevExpress.XtraEditors.VScrollBar();
            this.MapHScroll = new DevExpress.XtraEditors.HScrollBar();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.CoordsLabel = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bar3 = new DevExpress.XtraBars.Bar();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DXPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ZoomResetButton,
            this.ZoomInButton,
            this.ZoomOutButton,
            this.AttributesButton,
            this.SelectionButton,
            this.SaveButton,
            this.CancelButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 14;
            this.ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowCategoryInCaption = false;
            this.ribbon.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowQatLocationSelector = false;
            this.ribbon.ShowToolbarCustomizeItem = false;
            this.ribbon.Size = new System.Drawing.Size(1098, 144);
            this.ribbon.Toolbar.ShowCustomizeItem = false;
            // 
            // ZoomResetButton
            // 
            this.ZoomResetButton.Caption = "Reset";
            this.ZoomResetButton.Id = 2;
            this.ZoomResetButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ZoomResetButton.ImageOptions.Image")));
            this.ZoomResetButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ZoomResetButton.ImageOptions.LargeImage")));
            this.ZoomResetButton.Name = "ZoomResetButton";
            this.ZoomResetButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ZoomResetButton_ItemClick);
            // 
            // ZoomInButton
            // 
            this.ZoomInButton.Caption = "Zoom In";
            this.ZoomInButton.Id = 3;
            this.ZoomInButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ZoomInButton.ImageOptions.Image")));
            this.ZoomInButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ZoomInButton.ImageOptions.LargeImage")));
            this.ZoomInButton.Name = "ZoomInButton";
            this.ZoomInButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ZoomInButton_ItemClick);
            // 
            // ZoomOutButton
            // 
            this.ZoomOutButton.Caption = "Zoom Out";
            this.ZoomOutButton.Id = 4;
            this.ZoomOutButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ZoomOutButton.ImageOptions.Image")));
            this.ZoomOutButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ZoomOutButton.ImageOptions.LargeImage")));
            this.ZoomOutButton.Name = "ZoomOutButton";
            this.ZoomOutButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ZoomOutButton_ItemClick);
            // 
            // AttributesButton
            // 
            this.AttributesButton.Caption = "Attributes";
            this.AttributesButton.Id = 5;
            this.AttributesButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("AttributesButton.ImageOptions.Image")));
            this.AttributesButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("AttributesButton.ImageOptions.LargeImage")));
            this.AttributesButton.Name = "AttributesButton";
            this.AttributesButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.AttributesButton_ItemClick);
            // 
            // SelectionButton
            // 
            this.SelectionButton.Caption = "Selection";
            this.SelectionButton.Id = 6;
            this.SelectionButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SelectionButton.ImageOptions.Image")));
            this.SelectionButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SelectionButton.ImageOptions.LargeImage")));
            this.SelectionButton.Name = "SelectionButton";
            this.SelectionButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SelectionButton_ItemClick);
            // 
            // SaveButton
            // 
            this.SaveButton.Caption = "Save";
            this.SaveButton.Id = 10;
            this.SaveButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.Image")));
            this.SaveButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.LargeImage")));
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SaveButton_ItemClick);
            // 
            // CancelButton
            // 
            this.CancelButton.Caption = "Cancel";
            this.CancelButton.Id = 11;
            this.CancelButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("CancelButton.ImageOptions.Image")));
            this.CancelButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("CancelButton.ImageOptions.LargeImage")));
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.CancelButton_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.AllowTextClipping = false;
            this.ribbonPageGroup2.ItemLinks.Add(this.SaveButton);
            this.ribbonPageGroup2.ItemLinks.Add(this.CancelButton);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.ShowCaptionButton = false;
            this.ribbonPageGroup2.Text = "Selection";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.ItemLinks.Add(this.ZoomResetButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.ZoomInButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.ZoomOutButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.AttributesButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.SelectionButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "View";
            // 
            // DXPanel
            // 
            this.DXPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DXPanel.Location = new System.Drawing.Point(0, 150);
            this.DXPanel.Name = "DXPanel";
            this.DXPanel.Size = new System.Drawing.Size(1081, 450);
            this.DXPanel.TabIndex = 2;
            this.DXPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DXPanel_MouseDown);
            this.DXPanel.MouseEnter += new System.EventHandler(this.DXPanel_MouseEnter);
            this.DXPanel.MouseLeave += new System.EventHandler(this.DXPanel_MouseLeave);
            this.DXPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DXPanel_MouseMove);
            this.DXPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DXPanel_MouseUp);
            // 
            // MapVScroll
            // 
            this.MapVScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapVScroll.Location = new System.Drawing.Point(1081, 150);
            this.MapVScroll.Name = "MapVScroll";
            this.MapVScroll.Size = new System.Drawing.Size(17, 473);
            this.MapVScroll.TabIndex = 4;
            this.MapVScroll.ValueChanged += new System.EventHandler(this.MapVScroll_ValueChanged);
            // 
            // MapHScroll
            // 
            this.MapHScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapHScroll.Location = new System.Drawing.Point(0, 606);
            this.MapHScroll.Name = "MapHScroll";
            this.MapHScroll.Size = new System.Drawing.Size(1081, 17);
            this.MapHScroll.TabIndex = 5;
            this.MapHScroll.ValueChanged += new System.EventHandler(this.MapHScroll_ValueChanged);
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.CoordsLabel});
            this.barManager.MaxItemId = 2;
            this.barManager.StatusBar = this.bar1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Custom 2";
            this.bar1.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.CoordsLabel)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Custom 2";
            // 
            // CoordsLabel
            // 
            this.CoordsLabel.Caption = "X: 0 - Y: 0";
            this.CoordsLabel.Id = 1;
            this.CoordsLabel.Name = "CoordsLabel";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(1098, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 624);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(1098, 27);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 624);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1098, 0);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 624);
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // MapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 651);
            this.Controls.Add(this.MapHScroll);
            this.Controls.Add(this.MapVScroll);
            this.Controls.Add(this.DXPanel);
            this.Controls.Add(this.ribbon);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "MapViewer";
            this.Ribbon = this.ribbon;
            this.Text = "Map Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DXPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.PanelControl DXPanel;
        private DevExpress.XtraEditors.VScrollBar MapVScroll;
        private DevExpress.XtraEditors.HScrollBar MapHScroll;
        private DevExpress.XtraBars.BarButtonItem ZoomResetButton;
        private DevExpress.XtraBars.BarButtonItem ZoomInButton;
        private DevExpress.XtraBars.BarButtonItem ZoomOutButton;
        private DevExpress.XtraBars.BarButtonItem AttributesButton;
        private DevExpress.XtraBars.BarButtonItem SelectionButton;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraBars.BarButtonItem CancelButton;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarStaticItem CoordsLabel;
        private DevExpress.XtraBars.Bar bar3;
    }
}