namespace Server.Views
{
    partial class DatabaseEncryptionForm
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
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.txtEncryptionKey = new System.Windows.Forms.TextBox();
            this.lblEncryptionKey = new System.Windows.Forms.Label();
            this.btnGenarateRandomKey = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(29, 28);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(80, 20);
            this.chkEnabled.TabIndex = 0;
            this.chkEnabled.Text = "Enabled";
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // txtEncryptionKey
            // 
            this.txtEncryptionKey.Location = new System.Drawing.Point(29, 95);
            this.txtEncryptionKey.Multiline = true;
            this.txtEncryptionKey.Name = "txtEncryptionKey";
            this.txtEncryptionKey.Size = new System.Drawing.Size(608, 117);
            this.txtEncryptionKey.TabIndex = 1;
            // 
            // lblEncryptionKey
            // 
            this.lblEncryptionKey.AutoSize = true;
            this.lblEncryptionKey.Location = new System.Drawing.Point(26, 76);
            this.lblEncryptionKey.Name = "lblEncryptionKey";
            this.lblEncryptionKey.Size = new System.Drawing.Size(287, 16);
            this.lblEncryptionKey.TabIndex = 2;
            this.lblEncryptionKey.Text = "Encryption Key (32 bytes) Encoded into base64";
            // 
            // btnGenarateRandomKey
            // 
            this.btnGenarateRandomKey.Location = new System.Drawing.Point(29, 218);
            this.btnGenarateRandomKey.Name = "btnGenarateRandomKey";
            this.btnGenarateRandomKey.Size = new System.Drawing.Size(608, 27);
            this.btnGenarateRandomKey.TabIndex = 3;
            this.btnGenarateRandomKey.Text = "Genarate random key";
            this.btnGenarateRandomKey.UseVisualStyleBackColor = true;
            this.btnGenarateRandomKey.Click += new System.EventHandler(this.btnGenerateRandomKey_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(29, 264);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 41);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // DatabaseEncryptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 317);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnGenarateRandomKey);
            this.Controls.Add(this.lblEncryptionKey);
            this.Controls.Add(this.txtEncryptionKey);
            this.Controls.Add(this.chkEnabled);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DatabaseEncryptionForm";
            this.Text = "Database Encryption";
            this.Load += new System.EventHandler(this.DatabaseEncryptionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.TextBox txtEncryptionKey;
        private System.Windows.Forms.Label lblEncryptionKey;
        private System.Windows.Forms.Button btnGenarateRandomKey;
        private System.Windows.Forms.Button btnSave;
    }
}