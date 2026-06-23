using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryEditor
{
    partial class ConversionOptionsDialog
    {
        private IContainer components;
        private ListBox _fileListBox;
        private CheckBox _buildAtlasCheckBox;
        private CheckBox _buildShadowAtlasCheckBox;
        private CheckBox _buildOverlayAtlasCheckBox;
        private NumericUpDown _atlasGroupNumeric;
        private NumericUpDown _atlasPageSizeNumeric;
        private ComboBox _individualRuntimeComboBox;
        private ComboBox _runtimeComboBox;
        private ComboBox _compressionComboBox;
        private Label _summaryLabel;
        private OpenFileDialog _openDialog;
        private Button _addButton;
        private Button _removeButton;
        private Label _individualRuntimeLabel;
        private Label _runtimeLabel;
        private Label _compressionLabel;
        private Label _groupLabel;
        private Label _groupHintLabel;
        private Label _pageSizeLabel;
        private Label _summaryTitleLabel;
        private Button _okButton;
        private Button _cancelButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _fileListBox = new ListBox();
            _buildAtlasCheckBox = new CheckBox();
            _buildShadowAtlasCheckBox = new CheckBox();
            _buildOverlayAtlasCheckBox = new CheckBox();
            _atlasGroupNumeric = new NumericUpDown();
            _atlasPageSizeNumeric = new NumericUpDown();
            _individualRuntimeComboBox = new ComboBox();
            _runtimeComboBox = new ComboBox();
            _compressionComboBox = new ComboBox();
            _summaryLabel = new Label();
            _openDialog = new OpenFileDialog();
            _addButton = new Button();
            _removeButton = new Button();
            _individualRuntimeLabel = new Label();
            _runtimeLabel = new Label();
            _compressionLabel = new Label();
            _groupLabel = new Label();
            _groupHintLabel = new Label();
            _pageSizeLabel = new Label();
            _summaryTitleLabel = new Label();
            _okButton = new Button();
            _cancelButton = new Button();
            ((ISupportInitialize)_atlasGroupNumeric).BeginInit();
            ((ISupportInitialize)_atlasPageSizeNumeric).BeginInit();
            SuspendLayout();
            // 
            // _fileListBox
            // 
            _fileListBox.HorizontalScrollbar = true;
            _fileListBox.Location = new Point(16, 38);
            _fileListBox.Name = "_fileListBox";
            _fileListBox.Size = new Size(528, 109);
            _fileListBox.TabIndex = 2;
            // 
            // _buildAtlasCheckBox
            // 
            _buildAtlasCheckBox.Location = new Point(16, 178);
            _buildAtlasCheckBox.Name = "_buildAtlasCheckBox";
            _buildAtlasCheckBox.Size = new Size(160, 22);
            _buildAtlasCheckBox.TabIndex = 3;
            _buildAtlasCheckBox.Text = "Build atlas pages";
            _buildAtlasCheckBox.CheckedChanged += BuildAtlasCheckBox_CheckedChanged;
            // 
            // _buildShadowAtlasCheckBox
            // 
            _buildShadowAtlasCheckBox.Location = new Point(36, 202);
            _buildShadowAtlasCheckBox.Name = "_buildShadowAtlasCheckBox";
            _buildShadowAtlasCheckBox.Size = new Size(250, 22);
            _buildShadowAtlasCheckBox.TabIndex = 4;
            _buildShadowAtlasCheckBox.Text = "Build shadow atlas pages";
            _buildShadowAtlasCheckBox.CheckedChanged += SummaryControl_Changed;
            // 
            // _buildOverlayAtlasCheckBox
            // 
            _buildOverlayAtlasCheckBox.Location = new Point(36, 226);
            _buildOverlayAtlasCheckBox.Name = "_buildOverlayAtlasCheckBox";
            _buildOverlayAtlasCheckBox.Size = new Size(250, 22);
            _buildOverlayAtlasCheckBox.TabIndex = 5;
            _buildOverlayAtlasCheckBox.Text = "Build overlay atlas pages";
            _buildOverlayAtlasCheckBox.CheckedChanged += SummaryControl_Changed;
            // 
            // _atlasGroupNumeric
            // 
            _atlasGroupNumeric.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            _atlasGroupNumeric.Location = new Point(156, 360);
            _atlasGroupNumeric.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            _atlasGroupNumeric.Name = "_atlasGroupNumeric";
            _atlasGroupNumeric.Size = new Size(120, 23);
            _atlasGroupNumeric.TabIndex = 13;
            _atlasGroupNumeric.TextChanged += SummaryControl_Changed;
            _atlasGroupNumeric.ValueChanged += SummaryControl_Changed;
            // 
            // _atlasPageSizeNumeric
            // 
            _atlasPageSizeNumeric.Increment = new decimal(new int[] { 512, 0, 0, 0 });
            _atlasPageSizeNumeric.Location = new Point(156, 394);
            _atlasPageSizeNumeric.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
            _atlasPageSizeNumeric.Minimum = new decimal(new int[] { 512, 0, 0, 0 });
            _atlasPageSizeNumeric.Name = "_atlasPageSizeNumeric";
            _atlasPageSizeNumeric.Size = new Size(120, 23);
            _atlasPageSizeNumeric.TabIndex = 16;
            _atlasPageSizeNumeric.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            _atlasPageSizeNumeric.ValueChanged += SummaryControl_Changed;
            // 
            // _individualRuntimeComboBox
            // 
            _individualRuntimeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _individualRuntimeComboBox.Items.AddRange(new object[] { "Source", "DXT1", "BC7" });
            _individualRuntimeComboBox.Location = new Point(156, 254);
            _individualRuntimeComboBox.Name = "_individualRuntimeComboBox";
            _individualRuntimeComboBox.Size = new Size(180, 23);
            _individualRuntimeComboBox.TabIndex = 7;
            _individualRuntimeComboBox.SelectedIndexChanged += SummaryControl_Changed;
            // 
            // _runtimeComboBox
            // 
            _runtimeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _runtimeComboBox.Items.AddRange(new object[] { "BC7 only", "BGRA32" });
            _runtimeComboBox.Location = new Point(156, 288);
            _runtimeComboBox.Name = "_runtimeComboBox";
            _runtimeComboBox.Size = new Size(180, 23);
            _runtimeComboBox.TabIndex = 9;
            _runtimeComboBox.SelectedIndexChanged += SummaryControl_Changed;
            // 
            // _compressionComboBox
            // 
            _compressionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _compressionComboBox.Items.AddRange(new object[] { "Deflate best", "Deflate fast", "None" });
            _compressionComboBox.Location = new Point(156, 322);
            _compressionComboBox.Name = "_compressionComboBox";
            _compressionComboBox.Size = new Size(180, 23);
            _compressionComboBox.TabIndex = 11;
            _compressionComboBox.SelectedIndexChanged += SummaryControl_Changed;
            // 
            // _summaryLabel
            // 
            _summaryLabel.Location = new Point(352, 198);
            _summaryLabel.Name = "_summaryLabel";
            _summaryLabel.Size = new Size(190, 236);
            _summaryLabel.TabIndex = 18;
            // 
            // _openDialog
            // 
            _openDialog.Filter = "WeMade|*.Wil;*.Wtl|Shanda|*.Wzl;*.Miz|Lib|*.Lib";
            _openDialog.Multiselect = true;
            // 
            // _addButton
            // 
            _addButton.Location = new Point(16, 10);
            _addButton.Name = "_addButton";
            _addButton.Size = new Size(90, 26);
            _addButton.TabIndex = 0;
            _addButton.Text = "Add Files";
            _addButton.Click += AddButton_Click;
            // 
            // _removeButton
            // 
            _removeButton.Location = new Point(112, 10);
            _removeButton.Name = "_removeButton";
            _removeButton.Size = new Size(90, 26);
            _removeButton.TabIndex = 1;
            _removeButton.Text = "Remove";
            _removeButton.Click += RemoveButton_Click;
            // 
            // _individualRuntimeLabel
            // 
            _individualRuntimeLabel.Location = new Point(16, 258);
            _individualRuntimeLabel.Name = "_individualRuntimeLabel";
            _individualRuntimeLabel.Size = new Size(135, 18);
            _individualRuntimeLabel.TabIndex = 6;
            _individualRuntimeLabel.Text = "Individual textures:";
            // 
            // _runtimeLabel
            // 
            _runtimeLabel.Location = new Point(16, 292);
            _runtimeLabel.Name = "_runtimeLabel";
            _runtimeLabel.Size = new Size(130, 18);
            _runtimeLabel.TabIndex = 8;
            _runtimeLabel.Text = "Atlas runtime format:";
            // 
            // _compressionLabel
            // 
            _compressionLabel.Location = new Point(16, 326);
            _compressionLabel.Name = "_compressionLabel";
            _compressionLabel.Size = new Size(130, 18);
            _compressionLabel.TabIndex = 10;
            _compressionLabel.Text = "Container compression:";
            // 
            // _groupLabel
            // 
            _groupLabel.Location = new Point(16, 364);
            _groupLabel.Name = "_groupLabel";
            _groupLabel.Size = new Size(130, 18);
            _groupLabel.TabIndex = 12;
            _groupLabel.Text = "Atlas group count:";
            // 
            // _groupHintLabel
            // 
            _groupHintLabel.Location = new Point(286, 364);
            _groupHintLabel.Name = "_groupHintLabel";
            _groupHintLabel.Size = new Size(58, 18);
            _groupHintLabel.TabIndex = 14;
            _groupHintLabel.Text = "0 = none";
            // 
            // _pageSizeLabel
            // 
            _pageSizeLabel.Location = new Point(16, 398);
            _pageSizeLabel.Name = "_pageSizeLabel";
            _pageSizeLabel.Size = new Size(130, 18);
            _pageSizeLabel.TabIndex = 15;
            _pageSizeLabel.Text = "Atlas page size:";
            // 
            // _summaryTitleLabel
            // 
            _summaryTitleLabel.Location = new Point(352, 178);
            _summaryTitleLabel.Name = "_summaryTitleLabel";
            _summaryTitleLabel.Size = new Size(190, 18);
            _summaryTitleLabel.TabIndex = 17;
            _summaryTitleLabel.Text = "Summary";
            // 
            // _okButton
            // 
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Location = new Point(352, 462);
            _okButton.Name = "_okButton";
            _okButton.Size = new Size(90, 28);
            _okButton.TabIndex = 19;
            _okButton.Text = "Convert";
            _okButton.Click += OkButton_Click;
            // 
            // _cancelButton
            // 
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(450, 462);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(90, 28);
            _cancelButton.TabIndex = 20;
            _cancelButton.Text = "Cancel";
            // 
            // ConversionOptionsDialog
            // 
            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(560, 508);
            Controls.Add(_addButton);
            Controls.Add(_removeButton);
            Controls.Add(_fileListBox);
            Controls.Add(_buildAtlasCheckBox);
            Controls.Add(_buildShadowAtlasCheckBox);
            Controls.Add(_buildOverlayAtlasCheckBox);
            Controls.Add(_individualRuntimeLabel);
            Controls.Add(_individualRuntimeComboBox);
            Controls.Add(_runtimeLabel);
            Controls.Add(_runtimeComboBox);
            Controls.Add(_compressionLabel);
            Controls.Add(_compressionComboBox);
            Controls.Add(_groupLabel);
            Controls.Add(_atlasGroupNumeric);
            Controls.Add(_groupHintLabel);
            Controls.Add(_pageSizeLabel);
            Controls.Add(_atlasPageSizeNumeric);
            Controls.Add(_summaryTitleLabel);
            Controls.Add(_summaryLabel);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConversionOptionsDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Convert Libraries";
            ((ISupportInitialize)_atlasGroupNumeric).EndInit();
            ((ISupportInitialize)_atlasPageSizeNumeric).EndInit();
            ResumeLayout(false);
        }
    }
}
