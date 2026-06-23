using System;
using System.Windows.Forms;

namespace LibraryEditor
{
    public sealed partial class SaveOptionsDialog : Form
    {
        public LibrarySaveOptions Options { get; private set; }

        public SaveOptionsDialog(int shadowEntryCount = 0, int overlayEntryCount = 0)
        {
            InitializeComponent();
            _individualRuntimeComboBox.SelectedIndex = 0;
            _runtimeComboBox.SelectedIndex = 0;
            _compressionComboBox.SelectedIndex = 0;
            _buildShadowAtlasCheckBox.Checked = shadowEntryCount > 100;
            _buildOverlayAtlasCheckBox.Checked = overlayEntryCount > 100;
            UpdateAtlasControls();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Options = new LibrarySaveOptions
            {
                BuildAtlasMetadata = _buildAtlasCheckBox.Checked,
                BuildShadowAtlasMetadata = _buildAtlasCheckBox.Checked && _buildShadowAtlasCheckBox.Checked,
                BuildOverlayAtlasMetadata = _buildAtlasCheckBox.Checked && _buildOverlayAtlasCheckBox.Checked,
                StorePngSourceImages = _storePngSourceCheckBox.Checked,
                AtlasGroupImageCount = _buildAtlasCheckBox.Checked ? (int)_atlasGroupNumeric.Value : 0,
                AtlasPageSize = _buildAtlasCheckBox.Checked ? (int)_atlasPageSizeNumeric.Value : 2048,
                IndividualRuntimePreference = GetSelectedIndividualRuntimePreference(),
                AtlasRuntimePreference = GetSelectedRuntimePreference(),
                ContainerCompression = GetSelectedContainerCompression()
            };
        }

        private ZlRuntimeTexturePreference GetSelectedRuntimePreference()
        {
            switch (_runtimeComboBox.SelectedIndex)
            {
                case 1:
                    return ZlRuntimeTexturePreference.Bgra32;
                default:
                    return ZlRuntimeTexturePreference.Bc7;
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
                case 1:
                    return ZlContainerCompression.DeflateFast;
                case 2:
                    return ZlContainerCompression.None;
                default:
                    return ZlContainerCompression.DeflateBest;
            }
        }

        private void BuildAtlasCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAtlasControls();
        }

        private void UpdateAtlasControls()
        {
            bool enabled = _buildAtlasCheckBox.Checked;
            _runtimeComboBox.Enabled = enabled;
            _buildShadowAtlasCheckBox.Enabled = enabled;
            _buildOverlayAtlasCheckBox.Enabled = enabled;
            _atlasGroupNumeric.Enabled = enabled;
            _atlasPageSizeNumeric.Enabled = enabled;
        }
    }
}
