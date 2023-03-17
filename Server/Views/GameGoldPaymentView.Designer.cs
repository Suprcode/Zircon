namespace Server.Views
{
    partial class GameGoldPaymentView
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
            this.GameGoldPaymentGridControl = new DevExpress.XtraGrid.GridControl();
            this.GameGoldPaymentGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.AccountLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.GameGoldPaymentGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameGoldPaymentGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AccountLookUpEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // GameGoldPaymentGridControl
            // 
            this.GameGoldPaymentGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameGoldPaymentGridControl.Location = new System.Drawing.Point(0, 0);
            this.GameGoldPaymentGridControl.MainView = this.GameGoldPaymentGridView;
            this.GameGoldPaymentGridControl.Name = "GameGoldPaymentGridControl";
            this.GameGoldPaymentGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.AccountLookUpEdit});
            this.GameGoldPaymentGridControl.Size = new System.Drawing.Size(937, 373);
            this.GameGoldPaymentGridControl.TabIndex = 0;
            this.GameGoldPaymentGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GameGoldPaymentGridView});
            // 
            // GameGoldPaymentGridView
            // 
            this.GameGoldPaymentGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13,
            this.gridColumn14,
            this.gridColumn15});
            this.GameGoldPaymentGridView.GridControl = this.GameGoldPaymentGridControl;
            this.GameGoldPaymentGridView.Name = "GameGoldPaymentGridView";
            this.GameGoldPaymentGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.GameGoldPaymentGridView.OptionsView.EnableAppearanceOddRow = true;
            this.GameGoldPaymentGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.GameGoldPaymentGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "Index";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.FieldName = "CharacterName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "Name";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "PaymentDate";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.ColumnEdit = this.AccountLookUpEdit;
            this.gridColumn5.FieldName = "Account";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // AccountLookUpEdit
            // 
            this.AccountLookUpEdit.AutoHeight = false;
            this.AccountLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.AccountLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.AccountLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("EMailAddress", "EMailAddress"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Referral", "Referral"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Banned", "Banned")});
            this.AccountLookUpEdit.DisplayMember = "EMailAddress";
            this.AccountLookUpEdit.Name = "AccountLookUpEdit";
            this.AccountLookUpEdit.NullText = "";
            // 
            // gridColumn6
            // 
            this.gridColumn6.FieldName = "TransactionID";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.FieldName = "TransactionType";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.FieldName = "Status";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            // 
            // gridColumn9
            // 
            this.gridColumn9.FieldName = "GameGoldAmount";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            // 
            // gridColumn10
            // 
            this.gridColumn10.FieldName = "Payer_EMail";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 9;
            // 
            // gridColumn11
            // 
            this.gridColumn11.FieldName = "Payer_ID";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 10;
            // 
            // gridColumn12
            // 
            this.gridColumn12.FieldName = "Price";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 11;
            // 
            // gridColumn13
            // 
            this.gridColumn13.FieldName = "Currency";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 12;
            // 
            // gridColumn14
            // 
            this.gridColumn14.FieldName = "Fee";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 13;
            // 
            // gridColumn15
            // 
            this.gridColumn15.FieldName = "Error";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 14;
            // 
            // GameGoldPaymentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 373);
            this.Controls.Add(this.GameGoldPaymentGridControl);
            this.Name = "GameGoldPaymentView";
            this.Text = "Payments";
            ((System.ComponentModel.ISupportInitialize)(this.GameGoldPaymentGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameGoldPaymentGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AccountLookUpEdit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl GameGoldPaymentGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView GameGoldPaymentGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit AccountLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
    }
}