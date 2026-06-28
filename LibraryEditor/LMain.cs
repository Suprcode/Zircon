using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryEditor
{
    public partial class LMain : Form
    {
        private readonly Dictionary<int, int> _indexList = new Dictionary<int, int>();
        private Mir3Library _library;
        private Mir3Library.Mir3Image _selectedImage, _exportImage;
        private Image _originalImage;
        public int newImages;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public LMain()
        {
            InitializeComponent();

            SendMessage(PreviewListView.Handle, 4149, 0, 5242946); //80 x 66

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            AddAtlasMenuItems();

            if (Program.openFileWith.Length > 0 &&
                File.Exists(Program.openFileWith))
            {
                OpenLibraryDialog.FileName = Program.openFileWith;
                _library = new Mir3Library(OpenLibraryDialog.FileName);
                PreviewListView.VirtualListSize = _library.Images.Count;

                // Show .Lib path in application title.
                this.Text = OpenLibraryDialog.FileName.ToString();
                UpdateLibraryVersionLabel();

                PreviewListView.SelectedIndices.Clear();

                if (PreviewListView.Items.Count > 0)
                    PreviewListView.Items[0].Selected = true;

                radioButtonImage.Enabled = true;
                radioButtonShadow.Enabled = true;
                radioButtonOverlay.Enabled = true;
            }
        }

        private void AddAtlasMenuItems()
        {
            ToolStripMenuItem validateAtlasMenuItem = new ToolStripMenuItem("Validate ZL Atlas Metadata")
            {
                ToolTipText = "Check that generated atlas page/source metadata is internally consistent."
            };
            validateAtlasMenuItem.Click += validateAtlasToolStripMenuItem_Click;

            ToolStripMenuItem exportAtlasMenuItem = new ToolStripMenuItem("Export ZL Atlas Debug Pages")
            {
                ToolTipText = "Export atlas page PNGs and rectangle overlays for visual validation."
            };
            exportAtlasMenuItem.Click += exportAtlasToolStripMenuItem_Click;

            functionsToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            functionsToolStripMenuItem.DropDownItems.Add(validateAtlasMenuItem);
            functionsToolStripMenuItem.DropDownItems.Add(exportAtlasMenuItem);
        }

        private void validateAtlasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;

            string report = _library.ValidateAtlasMetadata();
            toolStripStatusLabel.ForeColor = report.StartsWith("Atlas metadata valid")
                ? SystemColors.ControlText
                : Color.Red;
            toolStripStatusLabel.Text = report.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            MessageBox.Show(report, "ZL Atlas Validation");
        }

        private void exportAtlasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose where to export atlas debug PNGs.";
                dialog.SelectedPath = GetDefaultAtlasDebugDirectory();

                if (dialog.ShowDialog() != DialogResult.OK) return;

                string reportPath = _library.ExportAtlasDebugPages(dialog.SelectedPath);
                toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                toolStripStatusLabel.Text = $"Exported atlas debug pages. Report: {Path.GetFileName(reportPath)}";
                MessageBox.Show($"Atlas debug pages exported to:{Environment.NewLine}{dialog.SelectedPath}", "ZL Atlas Debug Export");
            }
        }


        private void PrepareSaveDialogForCurrentLibrary()
        {
            if (string.IsNullOrEmpty(_library.FileName))
                return;

            string directory = Path.GetDirectoryName(_library.FileName);
            if (!string.IsNullOrEmpty(directory))
                SaveLibraryDialog.InitialDirectory = directory;

            SaveLibraryDialog.FileName = Path.GetFileName(_library.FileName);
        }

        private string GetDefaultAtlasDebugDirectory()
        {
            if (_library == null || string.IsNullOrEmpty(_library.FileName))
                return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            string directory = Path.GetDirectoryName(_library.FileName);
            if (string.IsNullOrEmpty(directory))
                return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            return Path.Combine(directory, "AtlasDebug", Path.GetFileNameWithoutExtension(_library.FileName));
        }

        private async Task RunModalProgressAsync(string title, Action<IProgress<LibraryProgress>, CancellationToken> work)
        {
            using (CancellationTokenSource cancellation = new CancellationTokenSource())
            using (ProgressDialog dialog = new ProgressDialog(title))
            {
                dialog.CancelRequested += (_, _) => cancellation.Cancel();
                Progress<LibraryProgress> progress = new Progress<LibraryProgress>(dialog.Report);
                _ = dialog.Handle;
                Task task = Task.Run(() => work(progress, cancellation.Token), cancellation.Token);

                _ = task.ContinueWith(_ =>
                {
                    if (dialog.IsHandleCreated && !dialog.IsDisposed)
                        dialog.BeginInvoke(new Action(dialog.Close));
                }, TaskScheduler.Default);

                dialog.ShowDialog(this);
                await task;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            return; //Not added yet

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (Path.GetExtension(files[0]).ToUpper() == ".WIL" ||
                Path.GetExtension(files[0]).ToUpper() == ".WZL" ||
                Path.GetExtension(files[0]).ToUpper() == ".MIZ")
            {
                try
                {
                    ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                    Parallel.For(0, files.Length, options, i =>
                    {
                        if (Path.GetExtension(files[i]).ToUpper() == ".WTL")
                        {
                            WTLLibrary WTLlib = new WTLLibrary(files[i]);
                            WTLlib.ToMLibrary();
                        }
                        else
                        {
                            WeMadeLibrary WILlib = new WeMadeLibrary(files[i]);
                            WILlib.ToMLibrary();
                        }
                        toolStripProgressBar.Value++;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                toolStripProgressBar.Value = 0;

                MessageBox.Show(
                    string.Format("Successfully converted {0} {1}",
                    (OpenWeMadeDialog.FileNames.Length).ToString(),
                    (OpenWeMadeDialog.FileNames.Length > 1) ? "libraries" : "library"));
            }
            else if (Path.GetExtension(files[0]).ToUpper() == ".LIB")
            {
                ClearInterface();
                ImageList.Images.Clear();
                PreviewListView.Items.Clear();
                _indexList.Clear();

                if (_library != null) _library.Close();
                //_library = new MLibraryV2(files[0]);
                //PreviewListView.VirtualListSize = _library.Images.Count;
                PreviewListView.RedrawItems(0, PreviewListView.Items.Count - 1, true);

                // Show .Lib path in application title.
                this.Text = files[0].ToString();
            }
            else
            {
                return;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void ClearInterface()
        {
            _selectedImage = null;
            ImageBox.Image = null;
            ZoomTrackBar.Value = 1;

            WidthLabel.Text = "<No Image>";
            HeightLabel.Text = "<No Image>";
            ShadowTypeLabel.Text = "<No Image>";
            OffSetXTextBox.Text = string.Empty;
            OffSetYTextBox.Text = string.Empty;
            OffSetXTextBox.BackColor = SystemColors.Window;
            OffSetYTextBox.BackColor = SystemColors.Window;
            UpdateLibraryVersionLabel();
        }

        private void UpdateLibraryVersionLabel(Mir3Library.Mir3Image image = null)
        {
            string libraryText = GetLibraryVersionText(image, false);
            string atlasText = GetAtlasText(image, false);

            LibraryVersionLabel.Text = libraryText;
            AtlasLabel.Text = atlasText;
            toolTip.SetToolTip(LibraryVersionLabel, GetLibraryVersionText(image, true));
            toolTip.SetToolTip(AtlasLabel, GetAtlasText(image, true));
        }

        private string GetLibraryVersionText(Mir3Library.Mir3Image image, bool fullText)
        {
            if (_library == null)
                return "<None>";

            if (image == null)
                return "<No Format>";

            switch (_library.Version)
            {
                case 0:
                    return "ZL v0 (DXT1)";
                case Mir3Library.LIBRARY_VERSION:
                    return "ZL v1 (DXT5)";
                case Mir3Library.COMPRESSED_LIBRARY_VERSION:
                    string typeText = GetCompressedLibraryTypeText(image, fullText);
                    return fullText
                        ? "ZL v2 compressed (" + typeText + ")"
                        : "ZL v2 (" + (typeText.EndsWith(" Only") ? typeText : typeText.Replace(" ", string.Empty)) + ")";
                default:
                    return "ZL v" + _library.Version;
            }
        }

        private string GetCompressedLibraryTypeText(Mir3Library.Mir3Image image, bool fullText)
        {
            string sourceText = GetSelectedSourceCodecText(image);
            string atlasText = GetSelectedAtlasRuntimeText(image);

            if (string.IsNullOrEmpty(atlasText))
                return GetLibraryTypeText(image);

            return fullText
                ? $"{sourceText} source + {atlasText} atlas"
                : $"{sourceText} + {atlasText}";
        }

        private string GetSelectedSourceCodecText(Mir3Library.Mir3Image image)
        {
            if (image == null)
                return "PNG";

            if (!SelectedLayerHasPayload(image))
                return "<No Format>";

            if (radioButtonShadow.Checked)
                return image.ShadowCodec.ToString();

            if (radioButtonOverlay.Checked)
                return image.OverlayCodec.ToString();

            return image.ImageCodec.ToString();
        }

        private string GetSelectedAtlasRuntimeText(Mir3Library.Mir3Image image)
        {
            int selectedPage = GetSelectedAtlasPage(image);
            if (selectedPage < 0 || _library?.AtlasPages == null || selectedPage >= _library.AtlasPages.Count)
                return null;

            Mir3Library.Mir3AtlasPage page = _library.AtlasPages[selectedPage];
            if (page == null)
                return null;

            if (page.Bc7DataSize > 0 || page.RuntimePreference == ZlRuntimeTexturePreference.Bc7)
                return "BC7";

            if (page.FallbackDataSize > 0 || page.RuntimePreference == ZlRuntimeTexturePreference.Bc7Dxt5)
                return "BC7/DXT5";

            return page.Codec == ZlImageCodec.Png ? "BGRA32" : page.Codec.ToString();
        }

        private string GetLibraryTypeText(Mir3Library.Mir3Image image)
        {
            if (image == null)
                return "<No Format>";

            if (!SelectedLayerHasPayload(image))
                return "<No Format>";

            ZlImageCodec codec = image.ImageCodec;
            ZlRuntimeTexturePreference preference = image.ImageRuntimePreference;

            if (radioButtonShadow.Checked)
            {
                codec = image.ShadowCodec;
                preference = image.ShadowRuntimePreference;
            }
            else if (radioButtonOverlay.Checked)
            {
                codec = image.OverlayCodec;
                preference = image.OverlayRuntimePreference;
            }

            if (preference == ZlRuntimeTexturePreference.Bc7)
                return codec == ZlImageCodec.Bc7 ? "BC7 Only" : codec + " + BC7";

            if (preference == ZlRuntimeTexturePreference.Bc7Dxt5)
                return codec + " + BC7/DXT5";

            if (preference == ZlRuntimeTexturePreference.Dxt1)
                return codec == ZlImageCodec.Dxt1 ? "DXT1" : codec + " + DXT1";

            if (preference == ZlRuntimeTexturePreference.Dxt5)
                return codec == ZlImageCodec.Dxt5 ? "DXT5" : codec + " + DXT5";

            if (image.DataSize == 0)
                return "<No Format>";

            return codec + " + " + preference;
        }

        private bool SelectedLayerHasPayload(Mir3Library.Mir3Image image)
        {
            if (image == null)
                return false;

            if (radioButtonShadow.Checked)
            {
                return image.ShadowWidth > 0 &&
                       image.ShadowHeight > 0 &&
                       (image.ShadowDataSize > 0 || image.ShadowBc7DataSize > 0 || image.ShadowFallbackDataSize > 0 || image.ShadowAtlasPage >= 0);
            }

            if (radioButtonOverlay.Checked)
            {
                return image.OverlayWidth > 0 &&
                       image.OverlayHeight > 0 &&
                       (image.OverlayDataSize > 0 || image.OverlayBc7DataSize > 0 || image.OverlayFallbackDataSize > 0 || image.OverlayAtlasPage >= 0);
            }

            return image.Width > 0 &&
                   image.Height > 0 &&
                   (image.ImageDataSize > 0 || image.ImageBc7DataSize > 0 || image.ImageFallbackDataSize > 0 || image.AtlasPage >= 0);
        }

        private string GetAtlasText(Mir3Library.Mir3Image image, bool fullText)
        {
            if (_library == null)
                return "<None>";

            int atlasCount = _library.AtlasPages?.Count ?? 0;
            if (atlasCount == 0)
                return "No";

            int selectedPage = GetSelectedAtlasPage(image);
            if (selectedPage >= 0)
            {
                if (fullText)
                    return $"Yes ({atlasCount} pages, selected page {selectedPage})";

                return $"{atlasCount} pages, p{selectedPage}";
            }

            return fullText ? $"Yes ({atlasCount} pages)" : $"{atlasCount} pages";
        }

        private int GetSelectedAtlasPage(Mir3Library.Mir3Image image)
        {
            if (image == null)
                return -1;

            if (radioButtonShadow.Checked)
                return image.ShadowAtlasPage;

            if (radioButtonOverlay.Checked)
                return image.OverlayAtlasPage;

            return image.AtlasPage;
        }

        private void PreviewListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PreviewListView.SelectedIndices.Count == 0)
            {
                ClearInterface();
                return;
            }

            int selectedIndex = PreviewListView.SelectedIndices[0];
            _selectedImage = _library.GetImage(selectedIndex);

            if (_selectedImage == null)
            {
                ClearInterface();
                return;
            }

            if (radioButtonImage.Checked)
            {
                WidthLabel.Text = _selectedImage.Width.ToString();
                HeightLabel.Text = _selectedImage.Height.ToString();

                ShadowTypeLabel.Text = _selectedImage.ShadowType.ToString();

                OffSetXTextBox.Text = _selectedImage.OffSetX.ToString();
                OffSetYTextBox.Text = _selectedImage.OffSetY.ToString();

                ImageBox.Image = _selectedImage.Image;
            }
            else if (radioButtonShadow.Checked)
            {
                WidthLabel.Text = _selectedImage.ShadowWidth.ToString();
                HeightLabel.Text = _selectedImage.ShadowHeight.ToString();

                ShadowTypeLabel.Text = _selectedImage.ShadowType.ToString();

                OffSetXTextBox.Text = _selectedImage.ShadowOffSetX.ToString();
                OffSetYTextBox.Text = _selectedImage.ShadowOffSetY.ToString();

                ImageBox.Image = _selectedImage.ShadowImage;
            }
            if (radioButtonOverlay.Checked)
            {
                WidthLabel.Text = _selectedImage.OverlayWidth.ToString();
                HeightLabel.Text = _selectedImage.OverlayHeight.ToString();

                ShadowTypeLabel.Text = _selectedImage.ShadowType.ToString();

                OffSetXTextBox.Text = _selectedImage.OffSetX.ToString();
                OffSetYTextBox.Text = _selectedImage.OffSetY.ToString();

                ImageBox.Image = _selectedImage.OverlayImage;
            }

            UpdateLibraryVersionLabel(_selectedImage);

            // Keep track of what image/s are selected.
            if (PreviewListView.SelectedIndices.Count > 1)
            {
                toolStripStatusLabel.ForeColor = Color.Red;
                toolStripStatusLabel.Text = "Multiple images selected.";
            }
            else
            {
                toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                toolStripStatusLabel.Text = "Selected Image: " + string.Format("{0} / {1}",
                PreviewListView.SelectedIndices[0].ToString(),
                (PreviewListView.Items.Count - 1).ToString());
            }

            nudJump.Value = PreviewListView.SelectedIndices[0];
        }

        private void PreviewListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int index;

            if (_indexList.TryGetValue(e.ItemIndex, out index))
            {
                e.Item = new ListViewItem { ImageIndex = index, Text = e.ItemIndex.ToString() };
                return;
            }

            index = ImageList.Images.Count;
            _indexList.Add(e.ItemIndex, index);
            if (radioButtonImage.Checked)
                ImageList.Images.Add(_library.GetPreview(e.ItemIndex, ImageType.Image));
            else if (radioButtonShadow.Checked)
                ImageList.Images.Add(_library.GetPreview(e.ItemIndex, ImageType.Shadow));
            else if (radioButtonOverlay.Checked)
                ImageList.Images.Add(_library.GetPreview(e.ItemIndex, ImageType.Overlay));
            e.Item = new ListViewItem { ImageIndex = index, Text = e.ItemIndex.ToString() };
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library._fileName == null) return;

            if (ImportImageDialog.ShowDialog() != DialogResult.OK) return;

            List<string> fileNames = new List<string>(ImportImageDialog.FileNames);

            //fileNames.Sort();
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = fileNames.Count;

            for (int i = 0; i < fileNames.Count; i++)
            {
                string fileName = fileNames[i];
                Bitmap image;

                try
                {
                    image = new Bitmap(fileName);
                }
                catch
                {
                    continue;
                }

                fileName = Path.Combine(Path.GetDirectoryName(fileName), "Placements", Path.GetFileNameWithoutExtension(fileName));
                fileName = Path.ChangeExtension(fileName, ".txt");

                short x = 0;
                short y = 0;

                if (File.Exists(fileName))
                {
                    string[] placements = File.ReadAllLines(fileName);

                    if (placements.Length > 0)
                        short.TryParse(placements[0], out x);
                    if (placements.Length > 1)
                        short.TryParse(placements[1], out y);
                }

                _library.AddImage(image, x, y);
                toolStripProgressBar.Value++;
                //image.Dispose();
            }

            PreviewListView.VirtualListSize = _library.Images.Count;
            toolStripProgressBar.Value = 0;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            if (_library != null) _library.Close();
            _library = new Mir3Library(SaveLibraryDialog.FileName);
            PreviewListView.VirtualListSize = 0;
            _library.Save(SaveLibraryDialog.FileName);
            UpdateLibraryVersionLabel();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenLibraryDialog.ShowDialog() != DialogResult.OK) return;
            ClearInterface();
            PreviewListView.VirtualListSize = 0;
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            if (_library != null) _library.Close();
            try
            {
                _library = new Mir3Library(OpenLibraryDialog.FileName);
            }
            catch (Exception ex)
            {
                _library = null;
                PreviewListView.VirtualListSize = 0;
                toolStripStatusLabel.ForeColor = Color.Red;
                toolStripStatusLabel.Text = "Failed to open library.";
                MessageBox.Show(this, ex.Message, "Open Library", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PreviewListView.VirtualListSize = _library.Images.Count;
            UpdateLibraryVersionLabel();

            // Show .Lib path in application title.
            this.Text = OpenLibraryDialog.FileName.ToString();

            PreviewListView.SelectedIndices.Clear();

            if (PreviewListView.Items.Count > 0)
                PreviewListView.Items[0].Selected = true;

            radioButtonImage.Enabled = true;
            radioButtonShadow.Enabled = true;
            radioButtonOverlay.Enabled = true;
        }

        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;

            LibrarySaveOptions options = ShowSaveOptionsDialog();
            if (options == null) return;

            try
            {
                await RunModalProgressAsync("Saving Library", (progress, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    progress.Report(new LibraryProgress("Saving " + Path.GetFileName(_library.FileName), isMarquee: true));
                    SaveCurrentLibrary(_library.FileName, options, progress, token);
                });

                OnLibrarySaved(_library.FileName, options);
            }
            catch (OperationCanceledException)
            {
                toolStripStatusLabel.Text = "Save cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;

            LibrarySaveOptions options = ShowSaveOptionsDialog();
            if (options == null) return;

            PrepareSaveDialogForCurrentLibrary();
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                _library._fileName = SaveLibraryDialog.FileName;
                await RunModalProgressAsync("Saving Library", (progress, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    progress.Report(new LibraryProgress("Saving " + Path.GetFileName(SaveLibraryDialog.FileName), isMarquee: true));
                    SaveCurrentLibrary(_library._fileName, options, progress, token);
                });

                OnLibrarySaved(_library._fileName, options);
            }
            catch (OperationCanceledException)
            {
                toolStripStatusLabel.Text = "Save cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private LibrarySaveOptions ShowSaveOptionsDialog()
        {
            using (SaveOptionsDialog dialog = new SaveOptionsDialog(CountShadowEntries(_library), CountOverlayEntries(_library)))
            {
                return dialog.ShowDialog(this) == DialogResult.OK ? dialog.Options : null;
            }
        }

        private void SaveCurrentLibrary(string path, LibrarySaveOptions options, IProgress<LibraryProgress> progress, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _library.Version = Mir3Library.COMPRESSED_LIBRARY_VERSION;
            _library.ContainerCompression = options.ContainerCompression;

            foreach (Mir3Library.Mir3Image image in _library.Images)
            {
                token.ThrowIfCancellationRequested();
                image?.SetVersion(Mir3Library.COMPRESSED_LIBRARY_VERSION);
            }

            _library.SetRuntimePreferenceForAllImages(options.IndividualRuntimePreference, options.StorePngSourceImages, progress, Path.GetFileName(path), token);

            if (options.BuildAtlasMetadata)
            {
                bool reuseAtlas = _library.CanReuseAtlasMetadata(
                    options.AtlasPageSize,
                    options.AtlasGroupImageCount,
                    options.AtlasRuntimePreference,
                    true,
                    options.BuildShadowAtlasMetadata,
                    options.BuildOverlayAtlasMetadata);

                if (!reuseAtlas)
                    _library.BuildAtlasMetadata(options.AtlasPageSize, 2, options.AtlasGroupImageCount, options.AtlasRuntimePreference, progress, Path.GetFileName(path), true, options.BuildShadowAtlasMetadata, options.BuildOverlayAtlasMetadata);
            }
            else
            {
                ClearAtlasMetadata();
            }

            token.ThrowIfCancellationRequested();
            _library.Save(path);
        }

        private void ClearAtlasMetadata()
        {
            _library.AtlasPages.Clear();
            _library.AtlasGroupImageCount = 0;
            _library.AtlasPageSize = 0;

            foreach (Mir3Library.Mir3Image image in _library.Images)
            {
                if (image == null)
                    continue;

                image.AtlasPage = -1;
                image.ShadowAtlasPage = -1;
                image.OverlayAtlasPage = -1;
                image.SourceRectangle = Rectangle.Empty;
                image.ShadowSourceRectangle = Rectangle.Empty;
                image.OverlaySourceRectangle = Rectangle.Empty;
                image.VisibleBounds = Rectangle.Empty;
            }
        }

        private static int CountShadowEntries(Mir3Library library)
        {
            return library?.CountAtlasLayerEntries(ZlAtlasLayer.Shadow) ?? 0;
        }

        private static int CountOverlayEntries(Mir3Library library)
        {
            return library?.CountAtlasLayerEntries(ZlAtlasLayer.Overlay) ?? 0;
        }

        private void OnLibrarySaved(string path, LibrarySaveOptions options)
        {
            toolStripStatusLabel.ForeColor = SystemColors.ControlText;
            toolStripStatusLabel.Text = options.BuildAtlasMetadata
                ? $"Saved ZL v2 with atlas metadata using {options.ContainerCompression}."
                : $"Saved ZL v2 without atlas metadata using {options.ContainerCompression}.";

            if (!string.IsNullOrEmpty(_library.LastCompressionReport))
                toolStripStatusLabel.Text += $" Report: {Path.GetFileName(path)}.report.txt";

            _library.FileName = path;
            _library._fileName = path;
            Text = path;
            UpdateLibraryVersionLabel(_selectedImage);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to delete the selected Image?",
                "Delete Selected.",
                MessageBoxButtons.YesNoCancel) != DialogResult.Yes) return;

            var removeList = PreviewListView.SelectedIndices.Cast<int>().ToList();

            removeList.Sort();

            for (int i = removeList.Count - 1; i >= 0; i--)
                _library.RemoveImage(removeList[i]);

            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize -= removeList.Count;
        }

        private async void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LibraryConversionOptions options;
            using (ConversionOptionsDialog dialog = new ConversionOptionsDialog())
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                options = dialog.Options;
            }

            toolStripProgressBar.Maximum = options.FileNames.Length;
            toolStripProgressBar.Value = 0;

            try
            {
                await RunModalProgressAsync("Converting Libraries", (progress, token) =>
                {
                    string[] fileNames = options.FileNames;
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        string fileName = fileNames[i];
                        string displayName = Path.GetFileName(fileName);
                        IProgress<LibraryProgress> fileProgress = new FileProgress(progress, i, fileNames.Length, displayName);

                        fileProgress.Report(new LibraryProgress($"Converting {displayName}", 0, 0, true));
                        ConvertLibrary(fileName, options, fileProgress, token);
                        progress.Report(new LibraryProgress($"Converted {displayName}", 1, 1)
                        {
                            CountText = "Complete",
                            OverallValue = i + 1,
                            OverallMaximum = fileNames.Length,
                            OverallText = $"File {i + 1:N0} of {fileNames.Length:N0}: {displayName}",
                            GroupText = options.BuildAtlasMetadata ? "Atlas complete" : "Runtime textures complete"
                        });
                    }
                });
            }
            catch (OperationCanceledException)
            {
                toolStripProgressBar.Value = 0;
                toolStripStatusLabel.Text = "Conversion cancelled.";
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            toolStripProgressBar.Value = 0;

            MessageBox.Show(string.Format("Successfully converted {0} {1}",
                (options.FileNames.Length).ToString(),
                (options.FileNames.Length > 1) ? "libraries" : "library"));
        }

        private void ConvertLibrary(string fileName, LibraryConversionOptions options, IProgress<LibraryProgress> progress, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            string extension = Path.GetExtension(fileName).ToUpper();

            if (extension == ".WTL")
            {
                WTLLibrary WTLlib = new WTLLibrary(fileName);
                WTLlib.ToMLibrary(false, progress, token, options);
            }
            else if (extension == ".LIB")
            {
                int currentVersion;
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    currentVersion = reader.ReadInt32();
                }

                if (currentVersion == 1)
                {
                    CrystalLibraryV1 v1Lib = new CrystalLibraryV1(fileName);
                    v1Lib.ToMLibrary(false, progress, token, options);
                }
                else
                {
                    CrystalLibraryV2 v2Lib = new CrystalLibraryV2(fileName);
                    v2Lib.ToMLibrary(false, progress, token, options);
                }
            }
            else
            {
                WeMadeLibrary WILlib = new WeMadeLibrary(fileName);
                WILlib.ToMLibrary(false, progress, token, options);
            }
        }

        private sealed class FileProgress : IProgress<LibraryProgress>
        {
            private readonly IProgress<LibraryProgress> _inner;
            private readonly int _fileIndex;
            private readonly int _fileCount;
            private readonly string _fileName;

            public FileProgress(IProgress<LibraryProgress> inner, int fileIndex, int fileCount, string fileName)
            {
                _inner = inner;
                _fileIndex = fileIndex;
                _fileCount = fileCount;
                _fileName = fileName;
            }

            public void Report(LibraryProgress value)
            {
                if (value == null)
                    return;

                if (value.OverallMaximum <= 0)
                    value.OverallMaximum = _fileCount;

                if (value.OverallValue <= 0)
                    value.OverallValue = _fileIndex;

                if (string.IsNullOrEmpty(value.OverallText))
                    value.OverallText = $"File {_fileIndex + 1:N0} of {_fileCount:N0}: {_fileName}";

                _inner.Report(value);
            }
        }

        private void copyToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PreviewListView.SelectedIndices.Count == 0) return;
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            Mir3Library tempLibrary = new Mir3Library(SaveLibraryDialog.FileName);

            List<int> copyList = new List<int>();

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
                copyList.Add(PreviewListView.SelectedIndices[i]);

            copyList.Sort();

            for (int i = 0; i < copyList.Count; i++)
            {
                Mir3Library.Mir3Image image = _library.GetImage(copyList[i]);
                tempLibrary.AddImage(image.Image, image.OffSetX, image.OffSetY);
            }

            tempLibrary.Save(SaveLibraryDialog.FileName);
        }

        private void removeBlanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove the blank images?",
                "Remove Blanks",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            _library.RemoveBlanks();
            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
        }

        private void countBlanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLibraryDialog.Multiselect = true;

            if (OpenLibraryDialog.ShowDialog() != DialogResult.OK)
            {
                OpenLibraryDialog.Multiselect = false;
                return;
            }

            OpenLibraryDialog.Multiselect = false;

            CrystalLibraryV2.Load = false;

            int count = 0;

            for (int i = 0; i < OpenLibraryDialog.FileNames.Length; i++)
            {
                CrystalLibraryV2 library = new CrystalLibraryV2(OpenLibraryDialog.FileNames[i]);

                for (int x = 0; x < library.Count; x++)
                {
                    if (library.Images[x].Length <= 8)
                        count++;
                }

                library.Close();
            }

            CrystalLibraryV2.Load = true;
            MessageBox.Show(count.ToString());
        }

        private void OffSetXTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp))
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                Mir3Library.Mir3Image image = _library.GetImage(PreviewListView.SelectedIndices[i]);
                image.OffSetX = temp;
            }
        }

        private void OffSetYTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox control = sender as TextBox;

            if (control == null || !control.Focused) return;

            short temp;

            if (!short.TryParse(control.Text, out temp))
            {
                control.BackColor = Color.Red;
                return;
            }

            control.BackColor = SystemColors.Window;

            for (int i = 0; i < PreviewListView.SelectedIndices.Count; i++)
            {
                Mir3Library.Mir3Image image = _library.GetImage(PreviewListView.SelectedIndices[i]);
                image.OffSetY = temp;
            }
        }

        private void InsertImageButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;
            if (ImportImageDialog.ShowDialog() != DialogResult.OK) return;

            List<string> fileNames = new List<string>(ImportImageDialog.FileNames);

            //fileNames.Sort();

            int index = PreviewListView.SelectedIndices[0];

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = fileNames.Count;

            for (int i = fileNames.Count - 1; i >= 0; i--)
            {
                string fileName = fileNames[i];

                Bitmap image;

                try
                {
                    image = new Bitmap(fileName);
                }
                catch
                {
                    continue;
                }

                fileName = Path.Combine(Path.GetDirectoryName(fileName), "Placements", Path.GetFileNameWithoutExtension(fileName));
                fileName = Path.ChangeExtension(fileName, ".txt");

                short x = 0;
                short y = 0;

                if (File.Exists(fileName))
                {
                    string[] placements = File.ReadAllLines(fileName);

                    if (placements.Length > 0)
                        short.TryParse(placements[0], out x);
                    if (placements.Length > 1)
                        short.TryParse(placements[1], out y);
                }

                _library.InsertImage(index, image, x, y);

                toolStripProgressBar.Value++;
            }

            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
            toolStripProgressBar.Value = 0;
            _library.Save(_library._fileName);
        }

        private void safeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove the blank images?",
                "Remove Blanks", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            _library.RemoveBlanks(true);
            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
        }

        private const int HowDeepToScan = 6;

        /*public static void ProcessDir(string sourceDir, int recursionLvl, string outputDir)
        {
            if (recursionLvl <= HowDeepToScan)
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(sourceDir);
                foreach (string fileName in fileEntries)
                {
                    if (Directory.Exists(outputDir) != true) Directory.CreateDirectory(outputDir);
                    MLibraryv0 OldLibrary = new MLibraryv0(fileName);
                    MLibraryV2 NewLibrary = new MLibraryV2(outputDir + Path.GetFileName(fileName)) { Images = new List<MLibraryV2.MImage>(), IndexList = new List<int>(), Count = OldLibrary.Images.Count }; ;
                    for (int i = 0; i < OldLibrary.Images.Count; i++)
                        NewLibrary.Images.Add(null);
                    for (int j = 0; j < OldLibrary.Images.Count; j++)
                    {
                        MLibraryv0.MImage oldimage = OldLibrary.GetMImage(j);
                        NewLibrary.Images[j] = new MLibraryV2.MImage(oldimage.FBytes, oldimage.Width, oldimage.Height) { X = oldimage.X, Y = oldimage.Y };
                    }
                    NewLibrary.Save();
                    for (int i = 0; i < NewLibrary.Images.Count; i++)
                    {
                        if (NewLibrary.Images[i].Preview != null)
                            NewLibrary.Images[i].Preview.Dispose();
                        if (NewLibrary.Images[i].Image != null)
                            NewLibrary.Images[i].Image.Dispose();
                        if (NewLibrary.Images[i].MaskImage != null)
                            NewLibrary.Images[i].MaskImage.Dispose();
                    }
                    for (int i = 0; i < OldLibrary.Images.Count; i++)
                    {
                        if (OldLibrary.Images[i].Preview != null)
                            OldLibrary.Images[i].Preview.Dispose();
                        if (OldLibrary.Images[i].Image != null)
                            OldLibrary.Images[i].Image.Dispose();
                    }
                    NewLibrary.Images.Clear();
                    NewLibrary.IndexList.Clear();
                    OldLibrary.Images.Clear();
                    OldLibrary.IndexList.Clear();
                    NewLibrary.Close();
                    OldLibrary.Close();
                    NewLibrary = null;
                    OldLibrary = null;
                }

                // Recurse into subdirectories of this directory.
                string[] subdirEntries = Directory.GetDirectories(sourceDir);
                foreach (string subdir in subdirEntries)
                {
                    // Do not iterate through re-parse points.
                    if (Path.GetFileName(Path.GetFullPath(subdir).TrimEnd(Path.DirectorySeparatorChar)) == Path.GetFileName(Path.GetFullPath(outputDir).TrimEnd(Path.DirectorySeparatorChar))) continue;
                    if ((File.GetAttributes(subdir) &
                         FileAttributes.ReparsePoint) !=
                             FileAttributes.ReparsePoint)
                        ProcessDir(subdir, recursionLvl + 1, outputDir + " \\" + Path.GetFileName(Path.GetFullPath(subdir).TrimEnd(Path.DirectorySeparatorChar)) + "\\");
                }
            }
        }*/

        // Export a single image.
        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            string _fileName = Path.GetFileName(OpenLibraryDialog.FileName);
            string _newName = _fileName.Remove(_fileName.IndexOf('.'));
            string _folder = Application.StartupPath + "\\Exported\\" + _newName + "\\";

            Bitmap blank = new Bitmap(1, 1);

            // Create the folder if it doesn't exist.
            (new FileInfo(_folder)).Directory.Create();

            ListView.SelectedIndexCollection _col = PreviewListView.SelectedIndices;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = _col.Count;

            for (int i = _col[0]; i < (_col[0] + _col.Count); i++)
            {
                _exportImage = _library.GetImage(i);
                if (_exportImage?.Image == null)
                {
                    blank.Save(_folder + i.ToString() + ".png", ImageFormat.Png);
                }
                else
                {
                    _exportImage.Image.Save(_folder + i.ToString() + ".png", ImageFormat.Png);
                }

                toolStripProgressBar.Value++;

                if (!Directory.Exists(_folder + "/Placements/"))
                    Directory.CreateDirectory(_folder + "/Placements/");

                int offSetX = _exportImage?.OffSetX ?? 0;
                int offSetY = _exportImage?.OffSetY ?? 0;

                File.WriteAllLines(_folder + "/Placements/" + i.ToString() + ".txt", new string[] { offSetX.ToString(), offSetY.ToString() });
            }

            toolStripProgressBar.Value = 0;
            MessageBox.Show("Saving to " + _folder + "...", "Image Saved", MessageBoxButtons.OK);
        }

        // Don't let the splitter go out of sight on resizing.
        private void LMain_Resize(object sender, EventArgs e)
        {
            if (splitContainer1.SplitterDistance <= this.Height - 150) return;
            if (this.Height - 150 > 0)
            {
                splitContainer1.SplitterDistance = this.Height - 150;
            }
        }

        // Resize the image(Zoom).
        private Image ImageBoxZoom(Image image, Size size)
        {
            _originalImage = _selectedImage.Image;
            Bitmap _bmp = new Bitmap(_originalImage, Convert.ToInt32(_originalImage.Width * size.Width), Convert.ToInt32(_originalImage.Height * size.Height));
            Graphics _gfx = Graphics.FromImage(_bmp);
            return _bmp;
        }

        // Zoom in and out.
        private void ZoomTrackBar_Scroll(object sender, EventArgs e)
        {
            if (ImageBox.Image == null)
            {
                ZoomTrackBar.Value = 1;
            }
            if (ZoomTrackBar.Value > 0)
            {
                try
                {
                    PreviewListView.Items[(int)nudJump.Value].EnsureVisible();

                    Bitmap _newBMP = new Bitmap(_selectedImage.Width * ZoomTrackBar.Value, _selectedImage.Height * ZoomTrackBar.Value);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_newBMP))
                    {
                        if (checkBoxPreventAntiAliasing.Checked == true)
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingMode = CompositingMode.SourceCopy;
                        }

                        if (checkBoxQuality.Checked == true)
                        {
                            g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        }

                        g.DrawImage(_selectedImage.Image, new Rectangle(0, 0, _newBMP.Width, _newBMP.Height));
                    }
                    ImageBox.Image = _newBMP;

                    toolStripStatusLabel.ForeColor = SystemColors.ControlText;
                    toolStripStatusLabel.Text = "Selected Image: " + string.Format("{0} / {1}",
                        PreviewListView.SelectedIndices[0].ToString(),
                        (PreviewListView.Items.Count - 1).ToString());
                }
                catch
                {
                    return;
                }
            }
        }

        // Swap the image panel background colour Black/White.
        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (panel.BackColor == Color.Black)
            {
                panel.BackColor = Color.GhostWhite;
            }
            else
            {
                panel.BackColor = Color.Black;
            }
        }

        private void PreviewListView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            // Keep track of what image/s are selected.
            ListView.SelectedIndexCollection _col = PreviewListView.SelectedIndices;

            if (_col.Count > 1)
            {
                toolStripStatusLabel.ForeColor = Color.Red;
                toolStripStatusLabel.Text = "Multiple images selected.";
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();

            if (ofd.FileName == "") return;

            Bitmap newBmp = new Bitmap(ofd.FileName);

            ImageList.Images.Clear();
            _indexList.Clear();
            _library.ReplaceImage(PreviewListView.SelectedIndices[0], newBmp, 0, 0);
            PreviewListView.VirtualListSize = _library.Images.Count;

            try
            {
                PreviewListView.RedrawItems(0, PreviewListView.Items.Count - 1, true);
                ImageBox.Image = _library.Images[PreviewListView.SelectedIndices[0]].Image;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void previousImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PreviewListView.Visible && PreviewListView.Items.Count > 0)
                {
                    int index = PreviewListView.SelectedIndices[0];
                    index = index - 1;
                    PreviewListView.SelectedIndices.Clear();
                    this.PreviewListView.Items[index].Selected = true;
                    PreviewListView.Items[index].EnsureVisible();

                    if (_selectedImage == null || _selectedImage.Height == 1 && _selectedImage.Width == 1 && PreviewListView.SelectedIndices[0] != 0)
                    {
                        previousImageToolStripMenuItem_Click(null, null);
                    }
                }
            }
            catch (Exception)
            {
                PreviewListView.SelectedIndices.Clear();
                this.PreviewListView.Items[PreviewListView.Items.Count - 1].Selected = true;
            }
        }

        private void nextImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PreviewListView.Visible && PreviewListView.Items.Count > 0)
                {
                    int index = PreviewListView.SelectedIndices[0];
                    index = index + 1;
                    PreviewListView.SelectedIndices.Clear();
                    this.PreviewListView.Items[index].Selected = true;
                    PreviewListView.Items[index].EnsureVisible();

                    if (_selectedImage == null || _selectedImage.Height == 1 && _selectedImage.Width == 1 && PreviewListView.SelectedIndices[0] != 0)
                    {
                        nextImageToolStripMenuItem_Click(null, null);
                    }
                }
            }
            catch (Exception)
            {
                PreviewListView.SelectedIndices.Clear();
                this.PreviewListView.Items[0].Selected = true;
            }
        }

        // Move Left and Right through images.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                previousImageToolStripMenuItem_Click(null, null);
                return true;
            }

            if (keyData == Keys.Right)
            {
                nextImageToolStripMenuItem_Click(null, null);
                return true;
            }

            if (keyData == Keys.Up) //Not 100% accurate but works for now.
            {
                double d = Math.Floor((double)(PreviewListView.Width / 67));
                int index = PreviewListView.SelectedIndices[0] - (int)d;

                PreviewListView.SelectedIndices.Clear();
                if (index < 0)
                    index = 0;

                this.PreviewListView.Items[index].Selected = true;

                return true;
            }

            if (keyData == Keys.Down) //Not 100% accurate but works for now.
            {
                double d = Math.Floor((double)(PreviewListView.Width / 67));
                int index = PreviewListView.SelectedIndices[0] + (int)d;

                PreviewListView.SelectedIndices.Clear();
                if (index > PreviewListView.Items.Count - 1)
                    index = PreviewListView.Items.Count - 1;

                this.PreviewListView.Items[index].Selected = true;

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonSkipNext_Click(object sender, EventArgs e)
        {
            nextImageToolStripMenuItem_Click(null, null);
        }

        private void buttonSkipPrevious_Click(object sender, EventArgs e)
        {
            previousImageToolStripMenuItem_Click(null, null);
        }

        private void checkBoxQuality_CheckedChanged(object sender, EventArgs e)
        {
            ZoomTrackBar_Scroll(null, null);
        }

        private void checkBoxPreventAntiAliasing_CheckedChanged(object sender, EventArgs e)
        {
            ZoomTrackBar_Scroll(null, null);
        }

        private void nudJump_ValueChanged(object sender, EventArgs e)
        {
            if (PreviewListView.Items.Count - 1 >= nudJump.Value)
            {
                PreviewListView.SelectedIndices.Clear();
                PreviewListView.Items[(int)nudJump.Value].Selected = true;
                PreviewListView.Items[(int)nudJump.Value].EnsureVisible();
            }
        }

        private void OpenLibraryDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void radioButtonImage_CheckedChanged(object sender, EventArgs e)
        {
            int index = PreviewListView.SelectedIndices[0];            
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            PreviewListView.VirtualListSize = 0;
            PreviewListView.VirtualListSize = _library.Images.Count;

            OffSetXTextBox.Enabled = true;
            OffSetYTextBox.Enabled = true;
            AddButton.Enabled = true;
            DeleteButton.Enabled = true;
            buttonReplace.Enabled = true;
            InsertImageButton.Enabled = true;

            PreviewListView.Items[index].Selected = true;
            PreviewListView.Items[index].EnsureVisible();
        }

        private void radioButtonShadow_CheckedChanged(object sender, EventArgs e)
        {
            int index = PreviewListView.SelectedIndices[0];
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            PreviewListView.VirtualListSize = 0;
            PreviewListView.VirtualListSize = _library.Images.Count;

            OffSetXTextBox.Enabled = false;
            OffSetYTextBox.Enabled = false;
            AddButton.Enabled = false;
            DeleteButton.Enabled = false;
            buttonReplace.Enabled = false;
            InsertImageButton.Enabled = false;

            PreviewListView.Items[index].Selected = true;
            PreviewListView.Items[index].EnsureVisible();
        }

        private void radioButtonOverlay_CheckedChanged(object sender, EventArgs e)
        {
            int index = PreviewListView.SelectedIndices[0];
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            PreviewListView.VirtualListSize = 0;
            PreviewListView.VirtualListSize = _library.Images.Count;

            OffSetXTextBox.Enabled = false;
            OffSetYTextBox.Enabled = false;
            AddButton.Enabled = false;
            DeleteButton.Enabled = false;
            buttonReplace.Enabled = false;
            InsertImageButton.Enabled = false;

            PreviewListView.Items[index].Selected = true;
            PreviewListView.Items[index].EnsureVisible();
        }

        private void nudJump_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Enter key is down.
                if (PreviewListView.Items.Count - 1 >= nudJump.Value)
                {
                    PreviewListView.SelectedIndices.Clear();
                    PreviewListView.Items[(int)nudJump.Value].Selected = true;
                    PreviewListView.Items[(int)nudJump.Value].EnsureVisible();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;



            return dialogResult;
        }
        private void AddBlanksButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library._fileName == null) return;

            string value = "0";
            int temp = 0;
            if (InputBox("Add Blanks To End Of Library", "How Many Blank Images To Add:", ref value) == DialogResult.OK)
            {
                if (!int.TryParse(value, out temp))
                {
                    MessageBox.Show("Should be a numeric value");
                    return;
                }
                if (temp <= 0)
                {
                    MessageBox.Show("Must be atleast 1");
                    return;
                }
                newImages = temp;
            }
            for (int i = 0; i < newImages; i++)
            {
                Bitmap image = new Bitmap(1, 1);
                _library.AddImage(image, 0, 0);
            }
            PreviewListView.VirtualListSize = _library.Images.Count;
        }
        private void InsertBlanksButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library._fileName == null) return;
            if (PreviewListView.SelectedIndices.Count == 0) return;

            string value = "0";
            int temp = 0;
            if (InputBox("Insert Blank Images At Current Selected Position", "How Many Blank Images To Insert:", ref value) == DialogResult.OK)
            {
                if (!int.TryParse(value, out temp))
                {
                    MessageBox.Show("Should be a numeric value");
                    return;
                }
                if (temp <= 0)
                {
                    MessageBox.Show("Must be atleast 1");
                    return;
                }
                newImages = temp;
            }
            int index = PreviewListView.SelectedIndices[0];
            for (int i = 0; i < newImages; i++)
            {
                Bitmap image = new Bitmap(1, 1);
                _library.InsertImage(index, image, 0, 0);
            }

            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;
        }
        private void MergeButton_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (_library.FileName == null) return;

            string value = "1000";
            int temp = 0;
            if (InputBox("Merge Blanks", "How Many Images Per Object:", ref value) == DialogResult.OK)
            {
                if (!int.TryParse(value, out temp))
                {
                    MessageBox.Show("Should be a numeric value");
                    return;
                }
                if (temp <= 0)
                {
                    MessageBox.Show("Must be atleast 1");
                    return;
                }
                newImages = temp;
            }

            _library.AddBlanks(newImages);

            if (OpenMergeDialog.ShowDialog() != DialogResult.OK) return;

            toolStripProgressBar.Maximum = OpenMergeDialog.FileNames.Length;
            toolStripProgressBar.Value = 0;

            try
            {
                foreach (string file in OpenMergeDialog.FileNames)
                {
                    if (Path.GetExtension(file).ToUpper() == ".ZL")
                    {
                        Mir3Library newLib = new Mir3Library(file);
                        foreach (Mir3Library.Mir3Image image in newLib.Images)
                        {
                            if (image == null)
                            {
                                Bitmap blank = new Bitmap(1, 1);
                                _library.AddImage(blank, 0, 0);
                            }
                            else
                            {
                                _library.AddImage(image.Image, image.OffSetX, image.OffSetY);
                            }
                        }
                    }
                    else if (Path.GetExtension(file).ToUpper() == ".WTL")
                    {
                        WTLLibrary WTLlib = new WTLLibrary(file);
                        WTLlib.MergeToMLibrary(_library, newImages);
                    }
                    else if (Path.GetExtension(file).ToUpper() == ".LIB")
                    {
                        FileStream stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
                        BinaryReader reader = new BinaryReader(stream);
                        int CurrentVersion = reader.ReadInt32();
                        stream.Close();
                        stream.Dispose();
                        reader.Dispose();
                        if (CurrentVersion == 1)
                        {
                            CrystalLibraryV1 v1Lib = new CrystalLibraryV1(file);
                            v1Lib.MergeToMLibrary(_library, newImages);
                        }
                        else
                        {
                            CrystalLibraryV2 v2Lib = new CrystalLibraryV2(file);
                            v2Lib.MergeToMLibrary(_library, newImages);
                        }
                    }
                    else
                    {
                        WeMadeLibrary WILlib = new WeMadeLibrary(file);
                        WILlib.MergeToMLibrary(_library, newImages);

                    }
                    toolStripProgressBar.Value++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            toolStripProgressBar.Value = 0;

            MessageBox.Show(string.Format("Successfully merged {0} {1}",
                (OpenMergeDialog.FileNames.Length).ToString(),
                (OpenMergeDialog.FileNames.Length > 1) ? "libraries" : "library"));

            ImageList.Images.Clear();
            _indexList.Clear();
            PreviewListView.VirtualListSize = _library.Images.Count;

            _library.Save(_library.FileName);
        }

    }
}
