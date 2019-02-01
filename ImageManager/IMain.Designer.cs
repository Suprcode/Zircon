namespace ImageManager
{
    partial class IMain
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
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.SelectedFolderButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.SubFoldersCheckEdit = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.ConvertButton = new DevExpress.XtraEditors.SimpleButton();
            this.progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.ProgressLabel = new DevExpress.XtraEditors.LabelControl();
            this.CreaetLibrariesButton = new DevExpress.XtraEditors.SimpleButton();
            this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedFolderButtonEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubFoldersCheckEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "ribbonPageGroup1";
            // 
            // SelectedFolderButtonEdit
            // 
            this.SelectedFolderButtonEdit.EditValue = "C:\\Zircon Server\\Data Works\\Test";
            this.SelectedFolderButtonEdit.Location = new System.Drawing.Point(113, 12);
            this.SelectedFolderButtonEdit.Name = "SelectedFolderButtonEdit";
            this.SelectedFolderButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.SelectedFolderButtonEdit.Size = new System.Drawing.Size(184, 20);
            this.SelectedFolderButtonEdit.TabIndex = 1;
            this.SelectedFolderButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.SelectedFolderButtonEdit_ButtonClick);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(41, 15);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(66, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Select Folder:";
            // 
            // SubFoldersCheckEdit
            // 
            this.SubFoldersCheckEdit.Location = new System.Drawing.Point(113, 38);
            this.SubFoldersCheckEdit.Name = "SubFoldersCheckEdit";
            this.SubFoldersCheckEdit.Properties.Caption = "";
            this.SubFoldersCheckEdit.Size = new System.Drawing.Size(75, 19);
            this.SubFoldersCheckEdit.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(12, 41);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(95, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Include SubFolders:";
            // 
            // ConvertButton
            // 
            this.ConvertButton.Location = new System.Drawing.Point(113, 63);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(184, 23);
            this.ConvertButton.TabIndex = 5;
            this.ConvertButton.Text = "Convert Libraries";
            this.ConvertButton.Click += new System.EventHandler(this.ConvertLibrariesButton_Click);
            // 
            // progressBarControl1
            // 
            this.progressBarControl1.Location = new System.Drawing.Point(12, 152);
            this.progressBarControl1.Name = "progressBarControl1";
            this.progressBarControl1.Size = new System.Drawing.Size(297, 18);
            this.progressBarControl1.TabIndex = 6;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(12, 133);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(46, 13);
            this.labelControl3.TabIndex = 7;
            this.labelControl3.Text = "Progress:";
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Location = new System.Drawing.Point(64, 133);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(41, 13);
            this.ProgressLabel.TabIndex = 8;
            this.ProgressLabel.Text = "<None>";
            // 
            // CreaetLibrariesButton
            // 
            this.CreaetLibrariesButton.Location = new System.Drawing.Point(113, 92);
            this.CreaetLibrariesButton.Name = "CreaetLibrariesButton";
            this.CreaetLibrariesButton.Size = new System.Drawing.Size(184, 23);
            this.CreaetLibrariesButton.TabIndex = 0;
            this.CreaetLibrariesButton.Text = "Create Libraries";
            this.CreaetLibrariesButton.Click += new System.EventHandler(this.CreaetLibrariesButton_Click);
            // 
            // FolderDialog
            // 
            this.FolderDialog.SelectedPath = "C:\\Zircon Server\\Data Works\\Test";
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(32, 92);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 9;
            this.simpleButton1.Text = "simpleButton1";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // IMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 182);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.progressBarControl1);
            this.Controls.Add(this.ConvertButton);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.SubFoldersCheckEdit);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.SelectedFolderButtonEdit);
            this.Controls.Add(this.CreaetLibrariesButton);
            this.Name = "IMain";
            this.Text = "Image Manager";
            this.Load += new System.EventHandler(this.IMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SelectedFolderButtonEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubFoldersCheckEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.ButtonEdit SelectedFolderButtonEdit;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit SubFoldersCheckEdit;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton ConvertButton;
        private DevExpress.XtraEditors.ProgressBarControl progressBarControl1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl ProgressLabel;
        private DevExpress.XtraEditors.SimpleButton CreaetLibrariesButton;
        private System.Windows.Forms.FolderBrowserDialog FolderDialog;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}

