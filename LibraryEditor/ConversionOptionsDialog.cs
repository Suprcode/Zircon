using System;
using System.Windows.Forms;

namespace LibraryEditor
{
    public partial class ConversionOptionsDialog : Form
    {
        public LibraryConversionOptions Options { get; private set; }

        public ConversionOptionsDialog()
        {
            InitializeComponent();
            SelectDefaultOptions();
            UpdateAtlasControls();
            UpdateSummary();
        }

        private void SelectDefaultOptions()
        {
            SelectComboBoxDefault(_individualRuntimeComboBox, 0);
            SelectComboBoxDefault(_runtimeComboBox, 0);
            SelectComboBoxDefault(_compressionComboBox, 0);
        }

        private static void SelectComboBoxDefault(ComboBox comboBox, int selectedIndex)
        {
            if (comboBox.Items.Count == 0 || comboBox.SelectedIndex >= 0)
                return;

            comboBox.SelectedIndex = Math.Min(selectedIndex, comboBox.Items.Count - 1);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (_openDialog.ShowDialog(this) != DialogResult.OK)
                return;

            foreach (string fileName in _openDialog.FileNames)
            {
                if (!_fileListBox.Items.Contains(fileName))
                    _fileListBox.Items.Add(fileName);
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            while (_fileListBox.SelectedItems.Count > 0)
                _fileListBox.Items.Remove(_fileListBox.SelectedItems[0]);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_fileListBox.Items.Count == 0)
            {
                MessageBox.Show(this, "Choose at least one library to convert.", "Convert Libraries");
                DialogResult = DialogResult.None;
                return;
            }

            string[] fileNames = new string[_fileListBox.Items.Count];
            for (int i = 0; i < _fileListBox.Items.Count; i++)
                fileNames[i] = _fileListBox.Items[i].ToString();

            Options = new LibraryConversionOptions
            {
                FileNames = fileNames,
                BuildAtlasMetadata = _buildAtlasCheckBox.Checked,
                BuildShadowAtlasMetadata = _buildAtlasCheckBox.Checked && _buildShadowAtlasCheckBox.Checked,
                BuildOverlayAtlasMetadata = _buildAtlasCheckBox.Checked && _buildOverlayAtlasCheckBox.Checked,
                StorePngSourceImages = false,
                AtlasGroupImageCount = _buildAtlasCheckBox.Checked ? (int)_atlasGroupNumeric.Value : 0,
                AtlasPageSize = _buildAtlasCheckBox.Checked ? (int)_atlasPageSizeNumeric.Value : 2048,
                IndividualRuntimePreference = GetSelectedIndividualRuntimePreference(),
                RuntimePreference = GetSelectedRuntimePreference(),
                ContainerCompression = GetSelectedContainerCompression()
            };
        }

        private ZlRuntimeTexturePreference GetSelectedRuntimePreference()
        {
            switch (_runtimeComboBox.SelectedIndex)
            {
                case 0:
                default:
                    return ZlRuntimeTexturePreference.Bc7;
                case 1:
                    return ZlRuntimeTexturePreference.Bgra32;
            }
        }

        private ZlRuntimeTexturePreference GetSelectedIndividualRuntimePreference()
        {
            return _individualRuntimeComboBox.SelectedIndex switch
            {
                0 => ZlRuntimeTexturePreference.Source,
                1 => ZlRuntimeTexturePreference.Dxt1,
                2 => ZlRuntimeTexturePreference.Bc7,
                _ => ZlRuntimeTexturePreference.None,
            };
        }

        private ZlContainerCompression GetSelectedContainerCompression()
        {
            switch (_compressionComboBox.SelectedIndex)
            {
                case 0:
                default:
                    return ZlContainerCompression.DeflateBest;
                case 1:
                    return ZlContainerCompression.DeflateFast;
                case 2:
                    return ZlContainerCompression.None;
            }
        }

        private void BuildAtlasCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAtlasControls();
        }

        private void SummaryControl_Changed(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        private void UpdateAtlasControls()
        {
            bool enabled = _buildAtlasCheckBox.Checked;
            _runtimeComboBox.Enabled = enabled;
            _buildShadowAtlasCheckBox.Enabled = enabled;
            _buildOverlayAtlasCheckBox.Enabled = enabled;
            _atlasGroupNumeric.Enabled = enabled;
            _atlasPageSizeNumeric.Enabled = enabled;
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            if (_summaryLabel == null)
                return;

            string individualText = _individualRuntimeComboBox.SelectedIndex switch
            {
                0 => "Source: preserves the source texture codec where possible; ideal for WTL-derived runtime libraries.",
                1 => "DXT1: ideal for opaque tiles or images without alpha.",
                2 => "BC7: best quality for standalone DX11 textures.",
                _ => "Individual runtime: none; relies on atlas or PNG decode."
            };

            string atlasText;
            if (_buildAtlasCheckBox.Checked)
            {
                atlasText = _runtimeComboBox.SelectedIndex == 0
                    ? "Atlas: BC7 pages for the fastest DX11 sprite path."
                    : "Atlas: BGRA pages; larger but simple to inspect.";

                if (_buildShadowAtlasCheckBox.Checked || _buildOverlayAtlasCheckBox.Checked)
                    atlasText += " Shadow/overlay layers included.";

                if (_atlasGroupNumeric.Value > 0)
                    atlasText += $" Split every {_atlasGroupNumeric.Value:N0} images.";
            }
            else
            {
                atlasText = "Atlas: disabled; better for map/tile libraries with poor page locality.";
            }

            string compressionText = _compressionComboBox.SelectedIndex switch
            {
                1 => "Compression: Deflate fast; quicker conversion, slightly larger files.",
                2 => "Compression: none; fastest load/save, largest files.",
                _ => "Compression: Deflate best; recommended default size/speed balance."
            };

            _summaryLabel.Text = $"{individualText}{Environment.NewLine}{Environment.NewLine}{atlasText}{Environment.NewLine}{Environment.NewLine}{compressionText}";
        }
    }
}
