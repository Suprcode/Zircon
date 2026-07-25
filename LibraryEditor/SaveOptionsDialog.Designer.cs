using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryEditor
{
    partial class SaveOptionsDialog
    {
        private CheckBox _buildAtlasCheckBox;
        private CheckBox _buildShadowAtlasCheckBox;
        private CheckBox _buildOverlayAtlasCheckBox;
        private CheckBox _storePngSourceCheckBox;
        private NumericUpDown _atlasGroupNumeric;
        private NumericUpDown _atlasPageSizeNumeric;
        private ComboBox _individualRuntimeComboBox;
        private ComboBox _runtimeComboBox;
        private ComboBox _compressionComboBox;
        private Label _individualRuntimeLabel;
        private Label _runtimeLabel;
        private Label _compressionLabel;
        private Label _groupLabel;
        private Label _groupHintLabel;
        private Label _pageSizeLabel;
        private Button _okButton;
        private Button _cancelButton;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _buildAtlasCheckBox = new CheckBox();
            _buildShadowAtlasCheckBox = new CheckBox();
            _buildOverlayAtlasCheckBox = new CheckBox();
            _storePngSourceCheckBox = new CheckBox();
            _atlasGroupNumeric = new NumericUpDown();
            _atlasPageSizeNumeric = new NumericUpDown();
            _individualRuntimeComboBox = new ComboBox();
            _runtimeComboBox = new ComboBox();
            _compressionComboBox = new ComboBox();
            _individualRuntimeLabel = new Label();
            _runtimeLabel = new Label();
            _compressionLabel = new Label();
            _groupLabel = new Label();
            _groupHintLabel = new Label();
            _pageSizeLabel = new Label();
            _okButton = new Button();
            _cancelButton = new Button();
            ((ISupportInitialize)_atlasGroupNumeric).BeginInit();
            ((ISupportInitialize)_atlasPageSizeNumeric).BeginInit();
            SuspendLayout();
            // 
            // _buildAtlasCheckBox
            // 
            _buildAtlasCheckBox.Location = new Point(16, 18);
            _buildAtlasCheckBox.Name = "_buildAtlasCheckBox";
            _buildAtlasCheckBox.Size = new Size(160, 22);
            _buildAtlasCheckBox.TabIndex = 0;
            _buildAtlasCheckBox.Text = "Build atlas pages";
            _buildAtlasCheckBox.UseVisualStyleBackColor = true;
            _buildAtlasCheckBox.CheckedChanged += BuildAtlasCheckBox_CheckedChanged;
            // 
            // _buildShadowAtlasCheckBox
            // 
            _buildShadowAtlasCheckBox.Location = new Point(36, 42);
            _buildShadowAtlasCheckBox.Name = "_buildShadowAtlasCheckBox";
            _buildShadowAtlasCheckBox.Size = new Size(220, 22);
            _buildShadowAtlasCheckBox.TabIndex = 1;
            _buildShadowAtlasCheckBox.Text = "Build shadow atlas pages";
            _buildShadowAtlasCheckBox.UseVisualStyleBackColor = true;
            // 
            // _buildOverlayAtlasCheckBox
            // 
            _buildOverlayAtlasCheckBox.Location = new Point(36, 66);
            _buildOverlayAtlasCheckBox.Name = "_buildOverlayAtlasCheckBox";
            _buildOverlayAtlasCheckBox.Size = new Size(220, 22);
            _buildOverlayAtlasCheckBox.TabIndex = 2;
            _buildOverlayAtlasCheckBox.Text = "Build overlay atlas pages";
            _buildOverlayAtlasCheckBox.UseVisualStyleBackColor = true;
            // 
            // _storePngSourceCheckBox
            // 
            _storePngSourceCheckBox.Location = new Point(16, 94);
            _storePngSourceCheckBox.Name = "_storePngSourceCheckBox";
            _storePngSourceCheckBox.Size = new Size(260, 22);
            _storePngSourceCheckBox.TabIndex = 3;
            _storePngSourceCheckBox.Text = "Keep existing PNG source images";
            _storePngSourceCheckBox.UseVisualStyleBackColor = true;
            // 
            // _atlasGroupNumeric
            // 
            _atlasGroupNumeric.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            _atlasGroupNumeric.Location = new Point(156, 228);
            _atlasGroupNumeric.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            _atlasGroupNumeric.Name = "_atlasGroupNumeric";
            _atlasGroupNumeric.Size = new Size(120, 23);
            _atlasGroupNumeric.TabIndex = 10;
            // 
            // _atlasPageSizeNumeric
            // 
            _atlasPageSizeNumeric.Increment = new decimal(new int[] { 512, 0, 0, 0 });
            _atlasPageSizeNumeric.Location = new Point(156, 262);
            _atlasPageSizeNumeric.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
            _atlasPageSizeNumeric.Minimum = new decimal(new int[] { 512, 0, 0, 0 });
            _atlasPageSizeNumeric.Name = "_atlasPageSizeNumeric";
            _atlasPageSizeNumeric.Size = new Size(120, 23);
            _atlasPageSizeNumeric.TabIndex = 13;
            _atlasPageSizeNumeric.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            // 
            // _individualRuntimeComboBox
            // 
            _individualRuntimeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _individualRuntimeComboBox.Items.AddRange(new object[] { "Source", "DXT1", "BC7" });
            _individualRuntimeComboBox.Location = new Point(156, 126);
            _individualRuntimeComboBox.Name = "_individualRuntimeComboBox";
            _individualRuntimeComboBox.Size = new Size(180, 23);
            _individualRuntimeComboBox.TabIndex = 5;
            // 
            // _runtimeComboBox
            // 
            _runtimeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _runtimeComboBox.Items.AddRange(new object[] { "BC7 only", "BGRA32" });
            _runtimeComboBox.Location = new Point(156, 160);
            _runtimeComboBox.Name = "_runtimeComboBox";
            _runtimeComboBox.Size = new Size(180, 23);
            _runtimeComboBox.TabIndex = 7;
            // 
            // _compressionComboBox
            // 
            _compressionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _compressionComboBox.Items.AddRange(new object[] { "Deflate best", "Deflate fast", "None" });
            _compressionComboBox.Location = new Point(156, 194);
            _compressionComboBox.Name = "_compressionComboBox";
            _compressionComboBox.Size = new Size(180, 23);
            _compressionComboBox.TabIndex = 9;
            // 
            // _individualRuntimeLabel
            // 
            _individualRuntimeLabel.Location = new Point(16, 130);
            _individualRuntimeLabel.Name = "_individualRuntimeLabel";
            _individualRuntimeLabel.Size = new Size(135, 18);
            _individualRuntimeLabel.TabIndex = 4;
            _individualRuntimeLabel.Text = "Individual textures:";
            // 
            // _runtimeLabel
            // 
            _runtimeLabel.Location = new Point(16, 164);
            _runtimeLabel.Name = "_runtimeLabel";
            _runtimeLabel.Size = new Size(130, 18);
            _runtimeLabel.TabIndex = 6;
            _runtimeLabel.Text = "Atlas runtime format:";
            // 
            // _compressionLabel
            // 
            _compressionLabel.Location = new Point(16, 198);
            _compressionLabel.Name = "_compressionLabel";
            _compressionLabel.Size = new Size(130, 18);
            _compressionLabel.TabIndex = 8;
            _compressionLabel.Text = "Container compression:";
            // 
            // _groupLabel
            // 
            _groupLabel.Location = new Point(16, 232);
            _groupLabel.Name = "_groupLabel";
            _groupLabel.Size = new Size(130, 18);
            _groupLabel.TabIndex = 9;
            _groupLabel.Text = "Atlas group count:";
            // 
            // _groupHintLabel
            // 
            _groupHintLabel.Location = new Point(286, 232);
            _groupHintLabel.Name = "_groupHintLabel";
            _groupHintLabel.Size = new Size(145, 18);
            _groupHintLabel.TabIndex = 11;
            _groupHintLabel.Text = "0 = no cut-off";
            // 
            // _pageSizeLabel
            // 
            _pageSizeLabel.Location = new Point(16, 266);
            _pageSizeLabel.Name = "_pageSizeLabel";
            _pageSizeLabel.Size = new Size(130, 18);
            _pageSizeLabel.TabIndex = 12;
            _pageSizeLabel.Text = "Atlas page size:";
            // 
            // _okButton
            // 
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Location = new Point(254, 360);
            _okButton.Name = "_okButton";
            _okButton.Size = new Size(90, 28);
            _okButton.TabIndex = 14;
            _okButton.Text = "Save";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += OkButton_Click;
            // 
            // _cancelButton
            // 
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(352, 360);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(90, 28);
            _cancelButton.TabIndex = 15;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            // 
            // SaveOptionsDialog
            // 
            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(450, 404);
            Controls.Add(_buildAtlasCheckBox);
            Controls.Add(_buildShadowAtlasCheckBox);
            Controls.Add(_buildOverlayAtlasCheckBox);
            Controls.Add(_storePngSourceCheckBox);
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
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaveOptionsDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Save ZL v2 Options";
            ((ISupportInitialize)_atlasGroupNumeric).EndInit();
            ((ISupportInitialize)_atlasPageSizeNumeric).EndInit();
            ResumeLayout(false);
        }
    }
}
