using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            if (Program.openFileWith.Length > 0 &&
                File.Exists(Program.openFileWith))
            {
                OpenLibraryDialog.FileName = Program.openFileWith;
                _library = new Mir3Library(OpenLibraryDialog.FileName);
                PreviewListView.VirtualListSize = _library.Images.Count;

                // Show .Lib path in application title.
                this.Text = OpenLibraryDialog.FileName.ToString();

                PreviewListView.SelectedIndices.Clear();

                if (PreviewListView.Items.Count > 0)
                    PreviewListView.Items[0].Selected = true;

                radioButtonImage.Enabled = true;
                radioButtonShadow.Enabled = true;
                radioButtonOverlay.Enabled = true;
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
        }

        private void PreviewListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PreviewListView.SelectedIndices.Count == 0)
            {
                ClearInterface();
                return;
            }

            _selectedImage = _library.GetImage(PreviewListView.SelectedIndices[0]);

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

            _indexList.Add(e.ItemIndex, ImageList.Images.Count);
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
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenLibraryDialog.ShowDialog() != DialogResult.OK) return;
            MessageBox.Show(OpenLibraryDialog.FileName);
            ClearInterface();
            ImageList.Images.Clear();
            PreviewListView.Items.Clear();
            _indexList.Clear();

            if (_library != null) _library.Close();
            _library = new Mir3Library(OpenLibraryDialog.FileName);
            PreviewListView.VirtualListSize = _library.Images.Count;

            // Show .Lib path in application title.
            this.Text = OpenLibraryDialog.FileName.ToString();

            PreviewListView.SelectedIndices.Clear();

            if (PreviewListView.Items.Count > 0)
                PreviewListView.Items[0].Selected = true;

            radioButtonImage.Enabled = true;
            radioButtonShadow.Enabled = true;
            radioButtonOverlay.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            _library.Save(_library.FileName);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_library == null) return;
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;

            _library._fileName = SaveLibraryDialog.FileName;
            _library.Save(_library._fileName);
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

        private void convertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenWeMadeDialog.ShowDialog() != DialogResult.OK) return;

            toolStripProgressBar.Maximum = OpenWeMadeDialog.FileNames.Length;
            toolStripProgressBar.Value = 0;

            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 1 };
                Parallel.For(0, OpenWeMadeDialog.FileNames.Length, options, i =>
                            {
                                if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]).ToUpper() == ".WTL")
                                {
                                    WTLLibrary WTLlib = new WTLLibrary(OpenWeMadeDialog.FileNames[i]);
                                    WTLlib.ToMLibrary();
                                }
                                else if (Path.GetExtension(OpenWeMadeDialog.FileNames[i]).ToUpper() == ".LIB")
                                {
                                    FileStream stream = new FileStream(OpenWeMadeDialog.FileNames[i], FileMode.Open, FileAccess.ReadWrite);
                                    BinaryReader reader = new BinaryReader(stream);
                                    int CurrentVersion = reader.ReadInt32();
                                    stream.Close();
                                    stream.Dispose();
                                    reader.Dispose();
                                    if (CurrentVersion == 1)
                                    {
                                        MLibrary v1Lib = new MLibrary(OpenWeMadeDialog.FileNames[i]);
                                        v1Lib.ToMLibrary();
                                    }
                                    else
                                    {
                                        MLibraryV2 v2Lib = new MLibraryV2(OpenWeMadeDialog.FileNames[i]);
                                        v2Lib.ToMLibrary();
                                    }
                                }
                                else
                                {
                                    WeMadeLibrary WILlib = new WeMadeLibrary(OpenWeMadeDialog.FileNames[i]);
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

            MessageBox.Show(string.Format("Successfully converted {0} {1}",
                (OpenWeMadeDialog.FileNames.Length).ToString(),
                (OpenWeMadeDialog.FileNames.Length > 1) ? "libraries" : "library"));
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

            MLibraryV2.Load = false;

            int count = 0;

            for (int i = 0; i < OpenLibraryDialog.FileNames.Length; i++)
            {
                MLibraryV2 library = new MLibraryV2(OpenLibraryDialog.FileNames[i]);

                for (int x = 0; x < library.Count; x++)
                {
                    if (library.Images[x].Length <= 8)
                        count++;
                }

                library.Close();
            }

            MLibraryV2.Load = true;
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
                            MLibrary v1Lib = new MLibrary(file);
                            v1Lib.MergeToMLibrary(_library, newImages);
                        }
                        else
                        {
                            MLibraryV2 v2Lib = new MLibraryV2(file);
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