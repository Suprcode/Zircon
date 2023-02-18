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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapViewer));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ZoomResetButton = new DevExpress.XtraBars.BarButtonItem();
            this.ZoomInButton = new DevExpress.XtraBars.BarButtonItem();
            this.ZoomOutButton = new DevExpress.XtraBars.BarButtonItem();
            this.AttributesButton = new DevExpress.XtraBars.BarButtonItem();
            this.SelectionButton = new DevExpress.XtraBars.BarButtonItem();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.CancelButton1 = new DevExpress.XtraBars.BarButtonItem();
            this.BlockedOnlyButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.DXPanel = new DevExpress.XtraEditors.PanelControl();
            this.MapVScroll = new DevExpress.XtraEditors.VScrollBar();
            this.MapHScroll = new DevExpress.XtraEditors.HScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DXPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ribbon.SearchEditItem,
            this.ZoomResetButton,
            this.ZoomInButton,
            this.ZoomOutButton,
            this.AttributesButton,
            this.SelectionButton,
            this.SaveButton,
            this.CancelButton1,
            this.BlockedOnlyButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 13;
            this.ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
            this.ribbon.Name = "ribbon";
            this.ribbon.OptionsPageCategories.ShowCaptions = false;
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbon.ShowQatLocationSelector = false;
            this.ribbon.ShowToolbarCustomizeItem = false;
            this.ribbon.Size = new System.Drawing.Size(1092, 144);
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
            // CancelButton1
            // 
            this.CancelButton1.Caption = "Cancel";
            this.CancelButton1.Id = 11;
            this.CancelButton1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("CancelButton.ImageOptions.Image")));
            this.CancelButton1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("CancelButton.ImageOptions.LargeImage")));
            this.CancelButton1.Name = "CancelButton1";
            this.CancelButton1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.CancelButton_ItemClick);
            // 
            // BlockedOnlyButton
            // 
            this.BlockedOnlyButton.Caption = "Attribute Selection";
            this.BlockedOnlyButton.Id = 12;
            this.BlockedOnlyButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("BlockedOnlyButton.ImageOptions.Image")));
            this.BlockedOnlyButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("BlockedOnlyButton.ImageOptions.LargeImage")));
            this.BlockedOnlyButton.Name = "BlockedOnlyButton";
            this.BlockedOnlyButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BlockedOnlyButton_ItemClick);
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
            this.ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonPageGroup2.ItemLinks.Add(this.SaveButton);
            this.ribbonPageGroup2.ItemLinks.Add(this.CancelButton1);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Selection";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonPageGroup1.ItemLinks.Add(this.ZoomResetButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.ZoomInButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.ZoomOutButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.AttributesButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.BlockedOnlyButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.SelectionButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "View";
            // 
            // DXPanel
            // 
            this.DXPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DXPanel.Location = new System.Drawing.Point(0, 150);
            this.DXPanel.Name = "DXPanel";
            this.DXPanel.Size = new System.Drawing.Size(1075, 449);
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
            this.MapVScroll.Location = new System.Drawing.Point(1075, 150);
            this.MapVScroll.Name = "MapVScroll";
            this.MapVScroll.Size = new System.Drawing.Size(17, 449);
            this.MapVScroll.TabIndex = 4;
            this.MapVScroll.ValueChanged += new System.EventHandler(this.MapVScroll_ValueChanged);
            // 
            // MapHScroll
            // 
            this.MapHScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapHScroll.Location = new System.Drawing.Point(0, 599);
            this.MapHScroll.Name = "MapHScroll";
            this.MapHScroll.Size = new System.Drawing.Size(1075, 17);
            this.MapHScroll.TabIndex = 5;
            this.MapHScroll.ValueChanged += new System.EventHandler(this.MapHScroll_ValueChanged);
            // 
            // MapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 616);
            this.Controls.Add(this.MapHScroll);
            this.Controls.Add(this.MapVScroll);
            this.Controls.Add(this.DXPanel);
            this.Controls.Add(this.ribbon);
            this.Name = "MapViewer";
            this.Ribbon = this.ribbon;
            this.Text = "Map Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DXPanel)).EndInit();
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
        private DevExpress.XtraBars.BarButtonItem CancelButton1;
        private DevExpress.XtraBars.BarButtonItem BlockedOnlyButton;
    }
}