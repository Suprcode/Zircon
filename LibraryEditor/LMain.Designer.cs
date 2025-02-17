namespace LibraryEditor
{
    partial class LMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LMain));
            MainMenu = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            functionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            countBlanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            removeBlanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            safeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            skinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            ShadowTypeLabel = new System.Windows.Forms.Label();
            InsertBlankButton = new System.Windows.Forms.Button();
            AddBlankButton = new System.Windows.Forms.Button();
            nudJump = new System.Windows.Forms.NumericUpDown();
            checkBoxPreventAntiAliasing = new System.Windows.Forms.CheckBox();
            checkBoxQuality = new System.Windows.Forms.CheckBox();
            buttonSkipPrevious = new System.Windows.Forms.Button();
            buttonSkipNext = new System.Windows.Forms.Button();
            buttonReplace = new System.Windows.Forms.Button();
            pictureBox = new System.Windows.Forms.PictureBox();
            ZoomTrackBar = new System.Windows.Forms.TrackBar();
            ExportButton = new System.Windows.Forms.Button();
            mergeBtn = new System.Windows.Forms.Button();
            InsertImageButton = new System.Windows.Forms.Button();
            OffSetYTextBox = new System.Windows.Forms.TextBox();
            OffSetXTextBox = new System.Windows.Forms.TextBox();
            DeleteButton = new System.Windows.Forms.Button();
            AddButton = new System.Windows.Forms.Button();
            label10 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            HeightLabel = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            WidthLabel = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            panel = new System.Windows.Forms.Panel();
            ImageBox = new System.Windows.Forms.PictureBox();
            PreviewListView = new FixedListView();
            ImageList = new System.Windows.Forms.ImageList(components);
            OpenLibraryDialog = new System.Windows.Forms.OpenFileDialog();
            SaveLibraryDialog = new System.Windows.Forms.SaveFileDialog();
            ImportImageDialog = new System.Windows.Forms.OpenFileDialog();
            OpenWeMadeDialog = new System.Windows.Forms.OpenFileDialog();
            OpenMergeDialog = new System.Windows.Forms.OpenFileDialog();
            toolTip = new System.Windows.Forms.ToolTip(components);
            statusStrip = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            panel1 = new System.Windows.Forms.Panel();
            radioButtonOverlay = new System.Windows.Forms.RadioButton();
            radioButtonShadow = new System.Windows.Forms.RadioButton();
            radioButtonImage = new System.Windows.Forms.RadioButton();
            label2 = new System.Windows.Forms.Label();
            MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudJump).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ZoomTrackBar).BeginInit();
            panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ImageBox).BeginInit();
            statusStrip.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenu
            // 
            MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, functionsToolStripMenuItem, skinToolStripMenuItem });
            MainMenu.Location = new System.Drawing.Point(0, 0);
            MainMenu.Name = "MainMenu";
            MainMenu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            MainMenu.Size = new System.Drawing.Size(1256, 24);
            MainMenu.TabIndex = 0;
            MainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripMenuItem1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripMenuItem2, closeToolStripMenuItem });
            fileToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("fileToolStripMenuItem.Image");
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("newToolStripMenuItem.Image");
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.ToolTipText = "New .Lib";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.ToolTipText = "Open Shanda or Wemade files.";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(111, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.ToolTipText = "Saves currently open .Lib";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("saveAsToolStripMenuItem.Image");
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            saveAsToolStripMenuItem.ToolTipText = ".Lib Only.";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(111, 6);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("closeToolStripMenuItem.Image");
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.ToolTipText = "Exit Application.";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // functionsToolStripMenuItem
            // 
            functionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { copyToToolStripMenuItem, countBlanksToolStripMenuItem, removeBlanksToolStripMenuItem, convertToolStripMenuItem });
            functionsToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("functionsToolStripMenuItem.Image");
            functionsToolStripMenuItem.Name = "functionsToolStripMenuItem";
            functionsToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            functionsToolStripMenuItem.Text = "Functions";
            // 
            // copyToToolStripMenuItem
            // 
            copyToToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("copyToToolStripMenuItem.Image");
            copyToToolStripMenuItem.Name = "copyToToolStripMenuItem";
            copyToToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            copyToToolStripMenuItem.Text = "Copy To..";
            copyToToolStripMenuItem.ToolTipText = "Copy to a new .Lib or to the end of an exsisting one.";
            copyToToolStripMenuItem.Visible = false;
            copyToToolStripMenuItem.Click += copyToToolStripMenuItem_Click;
            // 
            // countBlanksToolStripMenuItem
            // 
            countBlanksToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("countBlanksToolStripMenuItem.Image");
            countBlanksToolStripMenuItem.Name = "countBlanksToolStripMenuItem";
            countBlanksToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            countBlanksToolStripMenuItem.Text = "Count Blanks";
            countBlanksToolStripMenuItem.ToolTipText = "Counts the blank images in the .Lib";
            countBlanksToolStripMenuItem.Visible = false;
            countBlanksToolStripMenuItem.Click += countBlanksToolStripMenuItem_Click;
            // 
            // removeBlanksToolStripMenuItem
            // 
            removeBlanksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { safeToolStripMenuItem });
            removeBlanksToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("removeBlanksToolStripMenuItem.Image");
            removeBlanksToolStripMenuItem.Name = "removeBlanksToolStripMenuItem";
            removeBlanksToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            removeBlanksToolStripMenuItem.Text = "Remove Blanks";
            removeBlanksToolStripMenuItem.ToolTipText = "Quick removal of blanks.";
            removeBlanksToolStripMenuItem.Click += removeBlanksToolStripMenuItem_Click;
            // 
            // safeToolStripMenuItem
            // 
            safeToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("safeToolStripMenuItem.Image");
            safeToolStripMenuItem.Name = "safeToolStripMenuItem";
            safeToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            safeToolStripMenuItem.Text = "Safe";
            safeToolStripMenuItem.ToolTipText = "Use the safe method of removing blanks.";
            safeToolStripMenuItem.Click += safeToolStripMenuItem_Click;
            // 
            // convertToolStripMenuItem
            // 
            convertToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("convertToolStripMenuItem.Image");
            convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            convertToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            convertToolStripMenuItem.Text = "Converter";
            convertToolStripMenuItem.ToolTipText = "Convert Wil/Wzl/Miz to .Lib";
            convertToolStripMenuItem.Click += convertToolStripMenuItem_Click;
            // 
            // skinToolStripMenuItem
            // 
            skinToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            skinToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("skinToolStripMenuItem.Image");
            skinToolStripMenuItem.Name = "skinToolStripMenuItem";
            skinToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            skinToolStripMenuItem.Text = "Skin";
            skinToolStripMenuItem.Visible = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            splitContainer1.Location = new System.Drawing.Point(0, 67);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            splitContainer1.Panel1MinSize = 325;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(PreviewListView);
            splitContainer1.Size = new System.Drawing.Size(1256, 967);
            splitContainer1.SplitterDistance = 479;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new System.Drawing.Point(0, 3);
            splitContainer2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(label2);
            splitContainer2.Panel1.Controls.Add(ShadowTypeLabel);
            splitContainer2.Panel1.Controls.Add(InsertBlankButton);
            splitContainer2.Panel1.Controls.Add(AddBlankButton);
            splitContainer2.Panel1.Controls.Add(nudJump);
            splitContainer2.Panel1.Controls.Add(checkBoxPreventAntiAliasing);
            splitContainer2.Panel1.Controls.Add(checkBoxQuality);
            splitContainer2.Panel1.Controls.Add(buttonSkipPrevious);
            splitContainer2.Panel1.Controls.Add(buttonSkipNext);
            splitContainer2.Panel1.Controls.Add(buttonReplace);
            splitContainer2.Panel1.Controls.Add(pictureBox);
            splitContainer2.Panel1.Controls.Add(ZoomTrackBar);
            splitContainer2.Panel1.Controls.Add(ExportButton);
            splitContainer2.Panel1.Controls.Add(mergeBtn);
            splitContainer2.Panel1.Controls.Add(InsertImageButton);
            splitContainer2.Panel1.Controls.Add(OffSetYTextBox);
            splitContainer2.Panel1.Controls.Add(OffSetXTextBox);
            splitContainer2.Panel1.Controls.Add(DeleteButton);
            splitContainer2.Panel1.Controls.Add(AddButton);
            splitContainer2.Panel1.Controls.Add(label10);
            splitContainer2.Panel1.Controls.Add(label8);
            splitContainer2.Panel1.Controls.Add(HeightLabel);
            splitContainer2.Panel1.Controls.Add(label6);
            splitContainer2.Panel1.Controls.Add(WidthLabel);
            splitContainer2.Panel1.Controls.Add(label1);
            splitContainer2.Panel1.ForeColor = System.Drawing.Color.Black;
            splitContainer2.Panel1MinSize = 240;
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(panel);
            splitContainer2.Size = new System.Drawing.Size(1254, 474);
            splitContainer2.SplitterDistance = 280;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 0;
            // 
            // ShadowTypeLabel
            // 
            ShadowTypeLabel.AutoSize = true;
            ShadowTypeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            ShadowTypeLabel.Location = new System.Drawing.Point(144, 119);
            ShadowTypeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            ShadowTypeLabel.Name = "ShadowTypeLabel";
            ShadowTypeLabel.Size = new System.Drawing.Size(75, 15);
            ShadowTypeLabel.TabIndex = 25;
            ShadowTypeLabel.Text = "<No Image>";
            // 
            // InsertBlankButton
            // 
            InsertBlankButton.ForeColor = System.Drawing.SystemColors.ControlText;
            InsertBlankButton.Image = (System.Drawing.Image)resources.GetObject("InsertBlankButton.Image");
            InsertBlankButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            InsertBlankButton.Location = new System.Drawing.Point(141, 307);
            InsertBlankButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            InsertBlankButton.Name = "InsertBlankButton";
            InsertBlankButton.Size = new System.Drawing.Size(122, 30);
            InsertBlankButton.TabIndex = 24;
            InsertBlankButton.Tag = "";
            InsertBlankButton.Text = "Insert Blanks";
            InsertBlankButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            InsertBlankButton.UseVisualStyleBackColor = true;
            InsertBlankButton.Click += InsertBlanksButton_Click;
            // 
            // AddBlankButton
            // 
            AddBlankButton.ForeColor = System.Drawing.SystemColors.ControlText;
            AddBlankButton.Image = (System.Drawing.Image)resources.GetObject("AddBlankButton.Image");
            AddBlankButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            AddBlankButton.Location = new System.Drawing.Point(12, 304);
            AddBlankButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AddBlankButton.Name = "AddBlankButton";
            AddBlankButton.Size = new System.Drawing.Size(122, 30);
            AddBlankButton.TabIndex = 23;
            AddBlankButton.Tag = "";
            AddBlankButton.Text = "Add Blanks";
            AddBlankButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            AddBlankButton.UseVisualStyleBackColor = true;
            AddBlankButton.Click += AddBlanksButton_Click;
            // 
            // nudJump
            // 
            nudJump.Location = new System.Drawing.Point(89, 348);
            nudJump.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nudJump.Maximum = new decimal(new int[] { 650000, 0, 0, 0 });
            nudJump.Name = "nudJump";
            nudJump.Size = new System.Drawing.Size(90, 23);
            nudJump.TabIndex = 21;
            nudJump.ValueChanged += nudJump_ValueChanged;
            nudJump.KeyDown += nudJump_KeyDown;
            // 
            // checkBoxPreventAntiAliasing
            // 
            checkBoxPreventAntiAliasing.AutoSize = true;
            checkBoxPreventAntiAliasing.Location = new System.Drawing.Point(110, 440);
            checkBoxPreventAntiAliasing.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxPreventAntiAliasing.Name = "checkBoxPreventAntiAliasing";
            checkBoxPreventAntiAliasing.Size = new System.Drawing.Size(112, 19);
            checkBoxPreventAntiAliasing.TabIndex = 20;
            checkBoxPreventAntiAliasing.Text = "No Anti-aliasing";
            checkBoxPreventAntiAliasing.UseVisualStyleBackColor = true;
            checkBoxPreventAntiAliasing.CheckedChanged += checkBoxPreventAntiAliasing_CheckedChanged;
            // 
            // checkBoxQuality
            // 
            checkBoxQuality.AutoSize = true;
            checkBoxQuality.Location = new System.Drawing.Point(12, 440);
            checkBoxQuality.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxQuality.Name = "checkBoxQuality";
            checkBoxQuality.Size = new System.Drawing.Size(87, 19);
            checkBoxQuality.TabIndex = 19;
            checkBoxQuality.Text = "No Blurring";
            checkBoxQuality.UseVisualStyleBackColor = true;
            checkBoxQuality.CheckedChanged += checkBoxQuality_CheckedChanged;
            // 
            // buttonSkipPrevious
            // 
            buttonSkipPrevious.ForeColor = System.Drawing.SystemColors.ControlText;
            buttonSkipPrevious.Image = (System.Drawing.Image)resources.GetObject("buttonSkipPrevious.Image");
            buttonSkipPrevious.Location = new System.Drawing.Point(48, 344);
            buttonSkipPrevious.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSkipPrevious.Name = "buttonSkipPrevious";
            buttonSkipPrevious.Size = new System.Drawing.Size(35, 30);
            buttonSkipPrevious.TabIndex = 17;
            buttonSkipPrevious.Tag = "";
            buttonSkipPrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            buttonSkipPrevious.UseVisualStyleBackColor = true;
            buttonSkipPrevious.Click += buttonSkipPrevious_Click;
            // 
            // buttonSkipNext
            // 
            buttonSkipNext.ForeColor = System.Drawing.SystemColors.ControlText;
            buttonSkipNext.Image = (System.Drawing.Image)resources.GetObject("buttonSkipNext.Image");
            buttonSkipNext.Location = new System.Drawing.Point(184, 344);
            buttonSkipNext.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSkipNext.Name = "buttonSkipNext";
            buttonSkipNext.Size = new System.Drawing.Size(35, 30);
            buttonSkipNext.TabIndex = 16;
            buttonSkipNext.Tag = "";
            buttonSkipNext.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            buttonSkipNext.UseVisualStyleBackColor = true;
            buttonSkipNext.Click += buttonSkipNext_Click;
            // 
            // buttonReplace
            // 
            buttonReplace.ForeColor = System.Drawing.SystemColors.ControlText;
            buttonReplace.Image = (System.Drawing.Image)resources.GetObject("buttonReplace.Image");
            buttonReplace.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            buttonReplace.Location = new System.Drawing.Point(12, 230);
            buttonReplace.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonReplace.Name = "buttonReplace";
            buttonReplace.Size = new System.Drawing.Size(122, 30);
            buttonReplace.TabIndex = 15;
            buttonReplace.Tag = "";
            buttonReplace.Text = "Replace Image";
            buttonReplace.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            buttonReplace.UseVisualStyleBackColor = true;
            buttonReplace.Click += buttonReplace_Click;
            // 
            // pictureBox
            // 
            pictureBox.Image = (System.Drawing.Image)resources.GetObject("pictureBox.Image");
            pictureBox.Location = new System.Drawing.Point(12, 10);
            pictureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new System.Drawing.Size(16, 16);
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox.TabIndex = 14;
            pictureBox.TabStop = false;
            toolTip.SetToolTip(pictureBox, "Switch from Black to White background.");
            pictureBox.Click += pictureBox_Click;
            // 
            // ZoomTrackBar
            // 
            ZoomTrackBar.LargeChange = 1;
            ZoomTrackBar.Location = new System.Drawing.Point(48, 381);
            ZoomTrackBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ZoomTrackBar.Minimum = 1;
            ZoomTrackBar.Name = "ZoomTrackBar";
            ZoomTrackBar.Size = new System.Drawing.Size(172, 45);
            ZoomTrackBar.TabIndex = 4;
            ZoomTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            ZoomTrackBar.Value = 1;
            ZoomTrackBar.Scroll += ZoomTrackBar_Scroll;
            // 
            // ExportButton
            // 
            ExportButton.ForeColor = System.Drawing.SystemColors.ControlText;
            ExportButton.Image = (System.Drawing.Image)resources.GetObject("ExportButton.Image");
            ExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            ExportButton.Location = new System.Drawing.Point(141, 267);
            ExportButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new System.Drawing.Size(122, 30);
            ExportButton.TabIndex = 3;
            ExportButton.Tag = "";
            ExportButton.Text = "Export Images";
            ExportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            ExportButton.UseVisualStyleBackColor = true;
            ExportButton.Click += ExportButton_Click;
            // 
            // mergeBtn
            // 
            mergeBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            mergeBtn.Image = (System.Drawing.Image)resources.GetObject("mergeBtn.Image");
            mergeBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            mergeBtn.Location = new System.Drawing.Point(12, 267);
            mergeBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mergeBtn.Name = "mergeBtn";
            mergeBtn.Size = new System.Drawing.Size(122, 30);
            mergeBtn.TabIndex = 22;
            mergeBtn.Tag = "";
            mergeBtn.Text = "Merge Libraries";
            mergeBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            mergeBtn.UseVisualStyleBackColor = true;
            mergeBtn.Click += MergeButton_Click;
            // 
            // InsertImageButton
            // 
            InsertImageButton.ForeColor = System.Drawing.SystemColors.ControlText;
            InsertImageButton.Image = (System.Drawing.Image)resources.GetObject("InsertImageButton.Image");
            InsertImageButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            InsertImageButton.Location = new System.Drawing.Point(141, 230);
            InsertImageButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            InsertImageButton.Name = "InsertImageButton";
            InsertImageButton.Size = new System.Drawing.Size(122, 30);
            InsertImageButton.TabIndex = 1;
            InsertImageButton.Tag = "";
            InsertImageButton.Text = "Insert Images";
            InsertImageButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            InsertImageButton.UseVisualStyleBackColor = true;
            InsertImageButton.Click += InsertImageButton_Click;
            // 
            // OffSetYTextBox
            // 
            OffSetYTextBox.Location = new System.Drawing.Point(144, 88);
            OffSetYTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            OffSetYTextBox.Name = "OffSetYTextBox";
            OffSetYTextBox.Size = new System.Drawing.Size(75, 23);
            OffSetYTextBox.TabIndex = 6;
            OffSetYTextBox.TextChanged += OffSetYTextBox_TextChanged;
            // 
            // OffSetXTextBox
            // 
            OffSetXTextBox.Location = new System.Drawing.Point(144, 58);
            OffSetXTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            OffSetXTextBox.Name = "OffSetXTextBox";
            OffSetXTextBox.Size = new System.Drawing.Size(75, 23);
            OffSetXTextBox.TabIndex = 5;
            OffSetXTextBox.TextChanged += OffSetXTextBox_TextChanged;
            // 
            // DeleteButton
            // 
            DeleteButton.ForeColor = System.Drawing.SystemColors.ControlText;
            DeleteButton.Image = (System.Drawing.Image)resources.GetObject("DeleteButton.Image");
            DeleteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            DeleteButton.Location = new System.Drawing.Point(141, 193);
            DeleteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new System.Drawing.Size(122, 30);
            DeleteButton.TabIndex = 2;
            DeleteButton.Tag = "";
            DeleteButton.Text = "Delete Images";
            DeleteButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            DeleteButton.UseVisualStyleBackColor = true;
            DeleteButton.Click += DeleteButton_Click;
            // 
            // AddButton
            // 
            AddButton.ForeColor = System.Drawing.SystemColors.ControlText;
            AddButton.Image = (System.Drawing.Image)resources.GetObject("AddButton.Image");
            AddButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            AddButton.Location = new System.Drawing.Point(12, 193);
            AddButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AddButton.Name = "AddButton";
            AddButton.Size = new System.Drawing.Size(122, 30);
            AddButton.TabIndex = 0;
            AddButton.Tag = "";
            AddButton.Text = "Add Images";
            AddButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            AddButton.UseVisualStyleBackColor = true;
            AddButton.Click += AddButton_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = System.Drawing.SystemColors.ControlText;
            label10.Location = new System.Drawing.Point(78, 91);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(53, 15);
            label10.TabIndex = 12;
            label10.Text = "OffSet Y:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = System.Drawing.SystemColors.ControlText;
            label8.Location = new System.Drawing.Point(78, 61);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(53, 15);
            label8.TabIndex = 11;
            label8.Text = "OffSet X:";
            // 
            // HeightLabel
            // 
            HeightLabel.AutoSize = true;
            HeightLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            HeightLabel.Location = new System.Drawing.Point(144, 35);
            HeightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            HeightLabel.Name = "HeightLabel";
            HeightLabel.Size = new System.Drawing.Size(75, 15);
            HeightLabel.TabIndex = 10;
            HeightLabel.Text = "<No Image>";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = System.Drawing.SystemColors.ControlText;
            label6.Location = new System.Drawing.Point(89, 35);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(46, 15);
            label6.TabIndex = 9;
            label6.Text = "Height:";
            // 
            // WidthLabel
            // 
            WidthLabel.AutoSize = true;
            WidthLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            WidthLabel.Location = new System.Drawing.Point(144, 14);
            WidthLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            WidthLabel.Name = "WidthLabel";
            WidthLabel.Size = new System.Drawing.Size(75, 15);
            WidthLabel.TabIndex = 8;
            WidthLabel.Text = "<No Image>";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = System.Drawing.SystemColors.ControlText;
            label1.Location = new System.Drawing.Point(92, 14);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 15);
            label1.TabIndex = 7;
            label1.Text = "Width:";
            // 
            // panel
            // 
            panel.AutoScroll = true;
            panel.BackColor = System.Drawing.Color.Black;
            panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel.Controls.Add(ImageBox);
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.Location = new System.Drawing.Point(0, 0);
            panel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel.Name = "panel";
            panel.Size = new System.Drawing.Size(967, 472);
            panel.TabIndex = 1;
            // 
            // ImageBox
            // 
            ImageBox.BackColor = System.Drawing.Color.Transparent;
            ImageBox.Location = new System.Drawing.Point(0, 0);
            ImageBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ImageBox.Name = "ImageBox";
            ImageBox.Size = new System.Drawing.Size(64, 64);
            ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            ImageBox.TabIndex = 0;
            ImageBox.TabStop = false;
            // 
            // PreviewListView
            // 
            PreviewListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            PreviewListView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PreviewListView.BackColor = System.Drawing.Color.GhostWhite;
            PreviewListView.ForeColor = System.Drawing.Color.FromArgb(142, 152, 156);
            PreviewListView.LargeImageList = ImageList;
            PreviewListView.Location = new System.Drawing.Point(0, 0);
            PreviewListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PreviewListView.Name = "PreviewListView";
            PreviewListView.Size = new System.Drawing.Size(1253, 384);
            PreviewListView.TabIndex = 0;
            PreviewListView.UseCompatibleStateImageBehavior = false;
            PreviewListView.VirtualMode = true;
            PreviewListView.RetrieveVirtualItem += PreviewListView_RetrieveVirtualItem;
            PreviewListView.SelectedIndexChanged += PreviewListView_SelectedIndexChanged;
            PreviewListView.VirtualItemsSelectionRangeChanged += PreviewListView_VirtualItemsSelectionRangeChanged;
            // 
            // ImageList
            // 
            ImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            ImageList.ImageSize = new System.Drawing.Size(64, 64);
            ImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // OpenLibraryDialog
            // 
            OpenLibraryDialog.Filter = "Zircon Library|*.Zl";
            OpenLibraryDialog.FileOk += OpenLibraryDialog_FileOk;
            // 
            // SaveLibraryDialog
            // 
            SaveLibraryDialog.Filter = "Zircon Library|*.Zl";
            // 
            // ImportImageDialog
            // 
            ImportImageDialog.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            ImportImageDialog.Multiselect = true;
            // 
            // OpenWeMadeDialog
            // 
            OpenWeMadeDialog.Filter = "WeMade|*.Wil;*.Wtl|Shanda|*.Wzl;*.Miz|Lib|*.Lib";
            OpenWeMadeDialog.Multiselect = true;
            // 
            // OpenMergeDialog
            // 
            OpenMergeDialog.Filter = "Zircon Library|*.Zl|WeMade|*.Wil;*.Wtl|Shanda|*.Wzl;*.Miz|Lib|*.Lib";
            OpenMergeDialog.Multiselect = true;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel, toolStripProgressBar });
            statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip.Location = new System.Drawing.Point(0, 915);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip.Size = new System.Drawing.Size(1256, 24);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new System.Drawing.Size(90, 19);
            toolStripStatusLabel.Text = "Selected Image:";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new System.Drawing.Size(233, 18);
            toolStripProgressBar.Step = 1;
            toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // panel1
            // 
            panel1.Controls.Add(radioButtonOverlay);
            panel1.Controls.Add(radioButtonShadow);
            panel1.Controls.Add(radioButtonImage);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 24);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1256, 32);
            panel1.TabIndex = 3;
            // 
            // radioButtonOverlay
            // 
            radioButtonOverlay.AutoSize = true;
            radioButtonOverlay.Enabled = false;
            radioButtonOverlay.Location = new System.Drawing.Point(166, 6);
            radioButtonOverlay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radioButtonOverlay.Name = "radioButtonOverlay";
            radioButtonOverlay.Size = new System.Drawing.Size(65, 19);
            radioButtonOverlay.TabIndex = 2;
            radioButtonOverlay.Text = "Overlay";
            radioButtonOverlay.UseVisualStyleBackColor = true;
            radioButtonOverlay.CheckedChanged += radioButtonOverlay_CheckedChanged;
            // 
            // radioButtonShadow
            // 
            radioButtonShadow.AutoSize = true;
            radioButtonShadow.Enabled = false;
            radioButtonShadow.Location = new System.Drawing.Point(84, 6);
            radioButtonShadow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radioButtonShadow.Name = "radioButtonShadow";
            radioButtonShadow.Size = new System.Drawing.Size(67, 19);
            radioButtonShadow.TabIndex = 1;
            radioButtonShadow.Text = "Shadow";
            radioButtonShadow.UseVisualStyleBackColor = true;
            radioButtonShadow.CheckedChanged += radioButtonShadow_CheckedChanged;
            // 
            // radioButtonImage
            // 
            radioButtonImage.AutoSize = true;
            radioButtonImage.Checked = true;
            radioButtonImage.Enabled = false;
            radioButtonImage.Location = new System.Drawing.Point(15, 6);
            radioButtonImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radioButtonImage.Name = "radioButtonImage";
            radioButtonImage.Size = new System.Drawing.Size(58, 19);
            radioButtonImage.TabIndex = 0;
            radioButtonImage.TabStop = true;
            radioButtonImage.Text = "Image";
            radioButtonImage.UseVisualStyleBackColor = true;
            radioButtonImage.CheckedChanged += radioButtonImage_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.SystemColors.ControlText;
            label2.Location = new System.Drawing.Point(51, 119);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(80, 15);
            label2.TabIndex = 26;
            label2.Text = "Shadow Type:";
            // 
            // LMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1256, 939);
            Controls.Add(panel1);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip);
            Controls.Add(MainMenu);
            DoubleBuffered = true;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = MainMenu;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(756, 513);
            Name = "LMain";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Zircon Library Editor";
            Resize += LMain_Resize;
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudJump).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)ZoomTrackBar).EndInit();
            panel.ResumeLayout(false);
            panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ImageBox).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private FixedListView PreviewListView;
        private System.Windows.Forms.ImageList ImageList;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Label HeightLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog OpenLibraryDialog;
        private System.Windows.Forms.SaveFileDialog SaveLibraryDialog;
        private System.Windows.Forms.OpenFileDialog ImportImageDialog;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.OpenFileDialog OpenWeMadeDialog;
        private System.Windows.Forms.OpenFileDialog OpenMergeDialog;
        private System.Windows.Forms.ToolStripMenuItem functionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeBlanksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem countBlanksToolStripMenuItem;
        private System.Windows.Forms.TextBox OffSetYTextBox;
        private System.Windows.Forms.TextBox OffSetXTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button InsertImageButton;
        private System.Windows.Forms.Button mergeBtn;
        private System.Windows.Forms.ToolStripMenuItem safeToolStripMenuItem;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TrackBar ZoomTrackBar;
        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripMenuItem skinToolStripMenuItem;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Button buttonSkipPrevious;
        private System.Windows.Forms.Button buttonSkipNext;
        private System.Windows.Forms.CheckBox checkBoxQuality;
        private System.Windows.Forms.CheckBox checkBoxPreventAntiAliasing;
        private System.Windows.Forms.NumericUpDown nudJump;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonOverlay;
        private System.Windows.Forms.RadioButton radioButtonShadow;
        private System.Windows.Forms.RadioButton radioButtonImage;
        private System.Windows.Forms.Button InsertBlankButton;
        private System.Windows.Forms.Button AddBlankButton;
        private System.Windows.Forms.Label ShadowTypeLabel;
        private System.Windows.Forms.Label label2;
    }
}

