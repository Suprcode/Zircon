using System;

namespace Server.Views
{
    partial class CharacterView
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
            DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
            this.AccountLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.CharacterGridControl = new DevExpress.XtraGrid.GridControl();
            this.CharacterGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.Caption = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.AccountLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridColumn6
            // 
            gridColumn6.ColumnEdit = this.AccountLookUpEdit;
            gridColumn6.FieldName = "Account";
            gridColumn6.Name = "gridColumn6";
            gridColumn6.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            gridColumn6.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 1;
            // 
            // AccountLookUpEdit
            // 
            this.AccountLookUpEdit.AutoHeight = false;
            this.AccountLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.AccountLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.AccountLookUpEdit.CloseUpKey = new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.F5);
            this.AccountLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("EMailAddress", "E-Mail")});
            this.AccountLookUpEdit.DisplayMember = "EMailAddress";
            this.AccountLookUpEdit.Name = "AccountLookUpEdit";
            this.AccountLookUpEdit.NullText = "[Account is null]";
            // 
            // CharacterGridControl
            // 
            this.CharacterGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CharacterGridControl.Location = new System.Drawing.Point(0, 0);
            this.CharacterGridControl.MainView = this.CharacterGridView;
            this.CharacterGridControl.Name = "CharacterGridControl";
            this.CharacterGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1,
            this.AccountLookUpEdit});
            this.CharacterGridControl.Size = new System.Drawing.Size(640, 517);
            this.CharacterGridControl.TabIndex = 0;
            this.CharacterGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CharacterGridView});
            //this.CharacterGridControl.Click += new System.EventHandler(this.CharacterGridControl.Click);
            // 
            // CharacterGridView
            // 
            this.CharacterGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            gridColumn6,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn7,
            this.gridColumn8,
            this.Caption});
            this.CharacterGridView.GridControl = this.CharacterGridControl;
            this.CharacterGridView.Name = "CharacterGridView";
            this.CharacterGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "CharacterName";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.FieldName = "Class";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            // 
            // gridColumn3
            // 
            this.gridColumn3.FieldName = "Gender";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            // 
            // gridColumn4
            // 
            this.gridColumn4.FieldName = "Level";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            // 
            // gridColumn5
            // 
            this.gridColumn5.FieldName = "HairType";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.FieldName = "Deleted";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.ColumnEdit = this.repositoryItemTextEdit1;
            this.gridColumn8.FieldName = "Experience";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Mask.EditMask = "#,##0.#";
            this.repositoryItemTextEdit1.Mask.UseMaskAsDisplayFormat = true;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // Caption
            // 
            this.Caption.FieldName = "Caption";
            this.Caption.Name = "Caption";
            this.Caption.Visible = true;
            this.Caption.VisibleIndex = 8;
            // 
            // gridColumn9
            // 
            this.gridColumn9.ColumnEdit = this.repositoryItemTextEdit1;
            this.gridColumn9.FieldName = "Experience";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 7;
            // 
            // CharacterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 517);
            this.Controls.Add(this.CharacterGridControl);
            this.Name = "CharacterView";
            this.Text = "Character";
            ((System.ComponentModel.ISupportInitialize)(this.AccountLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CharacterGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl CharacterGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CharacterGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit AccountLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn Caption;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
     
    }
}