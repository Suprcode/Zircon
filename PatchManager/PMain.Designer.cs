namespace PatchManager
{
    partial class PMain
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.CleanClientButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
            this.DLookAndFeel = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.UsernameTextEdit = new DevExpress.XtraEditors.TextEdit();
            this.UseLoginCheckEdit = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.HostTextEdit = new DevExpress.XtraEditors.TextEdit();
            this.PasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
            this.UploadPatchButton = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.StatusLabel = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.UploadSizeLabel = new DevExpress.XtraEditors.LabelControl();
            this.TotalProgressBar = new DevExpress.XtraEditors.ProgressBarControl();
            this.UploadSpeedLabel = new DevExpress.XtraEditors.LabelControl();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.ProtocolDropDown = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.CleanClientButtonEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsernameTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UseLoginCheckEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HostTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordTextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalProgressBar.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProtocolDropDown.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 15);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(61, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Clean Client:";
            // 
            // CleanClientButtonEdit
            // 
            this.CleanClientButtonEdit.Location = new System.Drawing.Point(79, 12);
            this.CleanClientButtonEdit.Name = "CleanClientButtonEdit";
            this.CleanClientButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.CleanClientButtonEdit.Size = new System.Drawing.Size(233, 20);
            this.CleanClientButtonEdit.TabIndex = 1;
            this.CleanClientButtonEdit.EditValueChanged += new System.EventHandler(this.CleanClientButtonEdit_EditValueChanged);
            // 
            // DLookAndFeel
            // 
            this.DLookAndFeel.LookAndFeel.SkinName = "Blue";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(47, 41);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(26, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Host:";
            // 
            // UsernameTextEdit
            // 
            this.UsernameTextEdit.Location = new System.Drawing.Point(79, 117);
            this.UsernameTextEdit.Name = "UsernameTextEdit";
            this.UsernameTextEdit.Size = new System.Drawing.Size(233, 20);
            this.UsernameTextEdit.TabIndex = 3;
            this.UsernameTextEdit.EditValueChanged += new System.EventHandler(this.UsernameTextEdit_EditValueChanged);
            // 
            // UseLoginCheckEdit
            // 
            this.UseLoginCheckEdit.Location = new System.Drawing.Point(79, 92);
            this.UseLoginCheckEdit.Name = "UseLoginCheckEdit";
            this.UseLoginCheckEdit.Properties.Caption = "";
            this.UseLoginCheckEdit.Size = new System.Drawing.Size(75, 19);
            this.UseLoginCheckEdit.TabIndex = 4;
            this.UseLoginCheckEdit.CheckedChanged += new System.EventHandler(this.UseLoginCheckEdit_CheckedChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(23, 95);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(50, 13);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Use Login:";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(21, 120);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(52, 13);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "Username:";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(23, 146);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(50, 13);
            this.labelControl5.TabIndex = 7;
            this.labelControl5.Text = "Password:";
            // 
            // HostTextEdit
            // 
            this.HostTextEdit.Location = new System.Drawing.Point(79, 38);
            this.HostTextEdit.Name = "HostTextEdit";
            this.HostTextEdit.Size = new System.Drawing.Size(233, 20);
            this.HostTextEdit.TabIndex = 8;
            this.HostTextEdit.EditValueChanged += new System.EventHandler(this.HostTextEdit_EditValueChanged);
            // 
            // PasswordTextEdit
            // 
            this.PasswordTextEdit.Location = new System.Drawing.Point(79, 143);
            this.PasswordTextEdit.Name = "PasswordTextEdit";
            this.PasswordTextEdit.Properties.PasswordChar = '*';
            this.PasswordTextEdit.Size = new System.Drawing.Size(233, 20);
            this.PasswordTextEdit.TabIndex = 9;
            this.PasswordTextEdit.EditValueChanged += new System.EventHandler(this.PasswordTextEdit_EditValueChanged);
            // 
            // UploadPatchButton
            // 
            this.UploadPatchButton.Location = new System.Drawing.Point(79, 169);
            this.UploadPatchButton.Name = "UploadPatchButton";
            this.UploadPatchButton.Size = new System.Drawing.Size(233, 23);
            this.UploadPatchButton.TabIndex = 10;
            this.UploadPatchButton.Text = "Upload Patch";
            this.UploadPatchButton.Click += new System.EventHandler(this.UploadPatchButton_Click);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(14, 210);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(35, 13);
            this.labelControl6.TabIndex = 11;
            this.labelControl6.Text = "Status:";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Location = new System.Drawing.Point(55, 210);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(41, 13);
            this.StatusLabel.TabIndex = 12;
            this.StatusLabel.Text = "<None>";
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(12, 229);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(37, 13);
            this.labelControl8.TabIndex = 13;
            this.labelControl8.Text = "Upload:";
            // 
            // UploadSizeLabel
            // 
            this.UploadSizeLabel.Location = new System.Drawing.Point(55, 229);
            this.UploadSizeLabel.Name = "UploadSizeLabel";
            this.UploadSizeLabel.Size = new System.Drawing.Size(41, 13);
            this.UploadSizeLabel.TabIndex = 14;
            this.UploadSizeLabel.Text = "<None>";
            // 
            // TotalProgressBar
            // 
            this.TotalProgressBar.Location = new System.Drawing.Point(12, 248);
            this.TotalProgressBar.Name = "TotalProgressBar";
            this.TotalProgressBar.Size = new System.Drawing.Size(300, 18);
            this.TotalProgressBar.TabIndex = 15;
            // 
            // UploadSpeedLabel
            // 
            this.UploadSpeedLabel.Location = new System.Drawing.Point(262, 229);
            this.UploadSpeedLabel.Name = "UploadSpeedLabel";
            this.UploadSpeedLabel.Size = new System.Drawing.Size(41, 13);
            this.UploadSpeedLabel.TabIndex = 17;
            this.UploadSpeedLabel.Text = "<None>";
            // 
            // labelControl10
            // 
            this.labelControl10.Location = new System.Drawing.Point(222, 229);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(34, 13);
            this.labelControl10.TabIndex = 16;
            this.labelControl10.Text = "Speed:";
            // 
            // ProtocolDropDown
            // 
            this.ProtocolDropDown.Location = new System.Drawing.Point(79, 65);
            this.ProtocolDropDown.Name = "ProtocolDropDown";
            this.ProtocolDropDown.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ProtocolDropDown.Properties.Items.AddRange(new object[] {
            "Ftp",
            "SFtp"});
            this.ProtocolDropDown.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.ProtocolDropDown.Size = new System.Drawing.Size(100, 20);
            this.ProtocolDropDown.TabIndex = 18;
            this.ProtocolDropDown.SelectedIndexChanged += new System.EventHandler(this.ProtocolDropDown_SelectedIndexChanged);
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(30, 68);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(43, 13);
            this.labelControl7.TabIndex = 19;
            this.labelControl7.Text = "Protocol:";
            // 
            // PMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 276);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.ProtocolDropDown);
            this.Controls.Add(this.UploadSpeedLabel);
            this.Controls.Add(this.labelControl10);
            this.Controls.Add(this.TotalProgressBar);
            this.Controls.Add(this.UploadSizeLabel);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.UploadPatchButton);
            this.Controls.Add(this.PasswordTextEdit);
            this.Controls.Add(this.HostTextEdit);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.UseLoginCheckEdit);
            this.Controls.Add(this.UsernameTextEdit);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.CleanClientButtonEdit);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Patch Manager";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CleanClientButtonEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UsernameTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UseLoginCheckEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HostTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordTextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalProgressBar.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProtocolDropDown.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ButtonEdit CleanClientButtonEdit;
        private DevExpress.LookAndFeel.DefaultLookAndFeel DLookAndFeel;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit UsernameTextEdit;
        private DevExpress.XtraEditors.CheckEdit UseLoginCheckEdit;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit HostTextEdit;
        private DevExpress.XtraEditors.TextEdit PasswordTextEdit;
        private DevExpress.XtraEditors.SimpleButton UploadPatchButton;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl StatusLabel;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl UploadSizeLabel;
        private DevExpress.XtraEditors.ProgressBarControl TotalProgressBar;
        private DevExpress.XtraEditors.LabelControl UploadSpeedLabel;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private System.Windows.Forms.FolderBrowserDialog FolderDialog;
        private DevExpress.XtraEditors.ComboBoxEdit ProtocolDropDown;
        private DevExpress.XtraEditors.LabelControl labelControl7;
    }
}

