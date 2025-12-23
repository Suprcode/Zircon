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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapViewer));
            ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            ZoomResetButton = new DevExpress.XtraBars.BarButtonItem();
            ZoomInButton = new DevExpress.XtraBars.BarButtonItem();
            ZoomOutButton = new DevExpress.XtraBars.BarButtonItem();
            AttributesButton = new DevExpress.XtraBars.BarButtonItem();
            SelectionButton = new DevExpress.XtraBars.BarButtonItem();
            SaveButton = new DevExpress.XtraBars.BarButtonItem();
            CancelButton1 = new DevExpress.XtraBars.BarButtonItem();
            BlockedOnlyButton = new DevExpress.XtraBars.BarButtonItem();
            ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            DXPanel = new DevExpress.XtraEditors.PanelControl();
            MapVScroll = new DevExpress.XtraEditors.VScrollBar();
            MapHScroll = new DevExpress.XtraEditors.HScrollBar();
            barManager1 = new DevExpress.XtraBars.BarManager(components);
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            bar3 = new DevExpress.XtraBars.Bar();
            MapSizeLabel = new DevExpress.XtraBars.BarStaticItem();
            PositionLabel = new DevExpress.XtraBars.BarStaticItem();
            SelectedCellsLabel = new DevExpress.XtraBars.BarStaticItem();
            ((System.ComponentModel.ISupportInitialize)ribbon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DXPanel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbon.ExpandCollapseItem, ribbon.SearchEditItem, ZoomResetButton, ZoomInButton, ZoomOutButton, AttributesButton, SelectionButton, SaveButton, CancelButton1, BlockedOnlyButton });
            ribbon.Location = new System.Drawing.Point(0, 0);
            ribbon.MaxItemId = 13;
            ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
            ribbon.Name = "ribbon";
            ribbon.OptionsPageCategories.ShowCaptions = false;
            ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { ribbonPage1 });
            ribbon.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            ribbon.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            ribbon.ShowQatLocationSelector = false;
            ribbon.ShowToolbarCustomizeItem = false;
            ribbon.Size = new System.Drawing.Size(1078, 144);
            ribbon.Toolbar.ShowCustomizeItem = false;
            // 
            // ZoomResetButton
            // 
            ZoomResetButton.Caption = "Reset";
            ZoomResetButton.Id = 2;
            ZoomResetButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ZoomResetButton.ImageOptions.Image");
            ZoomResetButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ZoomResetButton.ImageOptions.LargeImage");
            ZoomResetButton.Name = "ZoomResetButton";
            ZoomResetButton.ItemClick += ZoomResetButton_ItemClick;
            // 
            // ZoomInButton
            // 
            ZoomInButton.Caption = "Zoom In";
            ZoomInButton.Id = 3;
            ZoomInButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ZoomInButton.ImageOptions.Image");
            ZoomInButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ZoomInButton.ImageOptions.LargeImage");
            ZoomInButton.Name = "ZoomInButton";
            ZoomInButton.ItemClick += ZoomInButton_ItemClick;
            // 
            // ZoomOutButton
            // 
            ZoomOutButton.Caption = "Zoom Out";
            ZoomOutButton.Id = 4;
            ZoomOutButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("ZoomOutButton.ImageOptions.Image");
            ZoomOutButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("ZoomOutButton.ImageOptions.LargeImage");
            ZoomOutButton.Name = "ZoomOutButton";
            ZoomOutButton.ItemClick += ZoomOutButton_ItemClick;
            // 
            // AttributesButton
            // 
            AttributesButton.Caption = "Attributes";
            AttributesButton.Id = 5;
            AttributesButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("AttributesButton.ImageOptions.Image");
            AttributesButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("AttributesButton.ImageOptions.LargeImage");
            AttributesButton.Name = "AttributesButton";
            AttributesButton.ItemClick += AttributesButton_ItemClick;
            // 
            // SelectionButton
            // 
            SelectionButton.Caption = "Selection";
            SelectionButton.Id = 6;
            SelectionButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SelectionButton.ImageOptions.Image");
            SelectionButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SelectionButton.ImageOptions.LargeImage");
            SelectionButton.Name = "SelectionButton";
            SelectionButton.ItemClick += SelectionButton_ItemClick;
            // 
            // SaveButton
            // 
            SaveButton.Caption = "Save";
            SaveButton.Id = 10;
            SaveButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.Image");
            SaveButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("SaveButton.ImageOptions.LargeImage");
            SaveButton.Name = "SaveButton";
            SaveButton.ItemClick += SaveButton_ItemClick;
            // 
            // CancelButton1
            // 
            CancelButton1.Caption = "Cancel";
            CancelButton1.Id = 11;
            CancelButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("CancelButton1.ImageOptions.Image");
            CancelButton1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("CancelButton1.ImageOptions.LargeImage");
            CancelButton1.Name = "CancelButton1";
            CancelButton1.ItemClick += CancelButton_ItemClick;
            // 
            // BlockedOnlyButton
            // 
            BlockedOnlyButton.Caption = "Attribute Selection";
            BlockedOnlyButton.Id = 12;
            BlockedOnlyButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("BlockedOnlyButton.ImageOptions.Image");
            BlockedOnlyButton.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("BlockedOnlyButton.ImageOptions.LargeImage");
            BlockedOnlyButton.Name = "BlockedOnlyButton";
            BlockedOnlyButton.ItemClick += BlockedOnlyButton_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { ribbonPageGroup2, ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "Home";
            // 
            // ribbonPageGroup2
            // 
            ribbonPageGroup2.AllowTextClipping = false;
            ribbonPageGroup2.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup2.ItemLinks.Add(SaveButton);
            ribbonPageGroup2.ItemLinks.Add(CancelButton1);
            ribbonPageGroup2.Name = "ribbonPageGroup2";
            ribbonPageGroup2.Text = "Selection";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(ZoomResetButton);
            ribbonPageGroup1.ItemLinks.Add(ZoomInButton);
            ribbonPageGroup1.ItemLinks.Add(ZoomOutButton);
            ribbonPageGroup1.ItemLinks.Add(AttributesButton);
            ribbonPageGroup1.ItemLinks.Add(BlockedOnlyButton);
            ribbonPageGroup1.ItemLinks.Add(SelectionButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "View";
            // 
            // DXPanel
            // 
            DXPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DXPanel.Location = new System.Drawing.Point(0, 150);
            DXPanel.Name = "DXPanel";
            DXPanel.Size = new System.Drawing.Size(1061, 432);
            DXPanel.TabIndex = 2;
            DXPanel.MouseDown += DXPanel_MouseDown;
            DXPanel.MouseEnter += DXPanel_MouseEnter;
            DXPanel.MouseLeave += DXPanel_MouseLeave;
            DXPanel.MouseMove += DXPanel_MouseMove;
            DXPanel.MouseUp += DXPanel_MouseUp;
            // 
            // MapVScroll
            // 
            MapVScroll.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            MapVScroll.Location = new System.Drawing.Point(1061, 150);
            MapVScroll.Name = "MapVScroll";
            MapVScroll.Size = new System.Drawing.Size(17, 432);
            MapVScroll.TabIndex = 4;
            MapVScroll.ValueChanged += MapVScroll_ValueChanged;
            // 
            // MapHScroll
            // 
            MapHScroll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            MapHScroll.Location = new System.Drawing.Point(0, 565);
            MapHScroll.Name = "MapHScroll";
            MapHScroll.Size = new System.Drawing.Size(1061, 17);
            MapHScroll.TabIndex = 5;
            MapHScroll.ValueChanged += MapHScroll_ValueChanged;
            // 
            // barManager1
            // 
            barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] { bar3 });
            barManager1.DockControls.Add(barDockControlTop);
            barManager1.DockControls.Add(barDockControlBottom);
            barManager1.DockControls.Add(barDockControlLeft);
            barManager1.DockControls.Add(barDockControlRight);
            barManager1.Form = this;
            barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] { MapSizeLabel, PositionLabel, SelectedCellsLabel });
            barManager1.MaxItemId = 3;
            barManager1.StatusBar = bar3;
            // 
            // barDockControlTop
            // 
            barDockControlTop.CausesValidation = false;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlTop.Location = new System.Drawing.Point(0, 0);
            barDockControlTop.Manager = barManager1;
            barDockControlTop.Size = new System.Drawing.Size(1078, 0);
            // 
            // barDockControlBottom
            // 
            barDockControlBottom.CausesValidation = false;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlBottom.Location = new System.Drawing.Point(0, 584);
            barDockControlBottom.Manager = barManager1;
            barDockControlBottom.Size = new System.Drawing.Size(1078, 25);
            // 
            // barDockControlLeft
            // 
            barDockControlLeft.CausesValidation = false;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            barDockControlLeft.Manager = barManager1;
            barDockControlLeft.Size = new System.Drawing.Size(0, 584);
            // 
            // barDockControlRight
            // 
            barDockControlRight.CausesValidation = false;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            barDockControlRight.Location = new System.Drawing.Point(1078, 0);
            barDockControlRight.Manager = barManager1;
            barDockControlRight.Size = new System.Drawing.Size(0, 584);
            // 
            // bar3
            // 
            bar3.BarName = "Status bar";
            bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            bar3.DockCol = 0;
            bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            bar3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(MapSizeLabel), new DevExpress.XtraBars.LinkPersistInfo(PositionLabel), new DevExpress.XtraBars.LinkPersistInfo(SelectedCellsLabel) });
            bar3.OptionsBar.AllowQuickCustomization = false;
            bar3.OptionsBar.DrawDragBorder = false;
            bar3.OptionsBar.UseWholeRow = true;
            bar3.Text = "Status bar";
            // 
            // MapSizeLabel
            // 
            MapSizeLabel.Caption = "Map Size : 0,0";
            MapSizeLabel.Id = 0;
            MapSizeLabel.Name = "MapSizeLabel";
            // 
            // PositionLabel
            // 
            PositionLabel.Caption = "Position: 0,0";
            PositionLabel.Id = 1;
            PositionLabel.Name = "PositionLabel";
            // 
            // SelectedCellsLabel
            // 
            SelectedCellsLabel.Caption = "Selected Cells : 0";
            SelectedCellsLabel.Id = 2;
            SelectedCellsLabel.Name = "SelectedCellsLabel";
            // 
            // MapViewer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1078, 609);
            Controls.Add(MapHScroll);
            Controls.Add(MapVScroll);
            Controls.Add(DXPanel);
            Controls.Add(ribbon);
            Controls.Add(barDockControlLeft);
            Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom);
            Controls.Add(barDockControlTop);
            Name = "MapViewer";
            Ribbon = ribbon;
            Text = "Map Viewer";
            ((System.ComponentModel.ISupportInitialize)ribbon).EndInit();
            ((System.ComponentModel.ISupportInitialize)DXPanel).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager1).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarStaticItem MapSizeLabel;
        private DevExpress.XtraBars.BarStaticItem PositionLabel;
        private DevExpress.XtraBars.BarStaticItem SelectedCellsLabel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}