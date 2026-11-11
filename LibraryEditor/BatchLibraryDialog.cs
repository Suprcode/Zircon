using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryEditor
{
    public sealed partial class BatchLibraryDialog : Form
    {
        private readonly bool split;
        private bool running;

        public BatchLibraryDialog() : this(true) { }

        public BatchLibraryDialog(bool split)
        {
            InitializeComponent();
            this.split = split;
            Text = split ? "Batch Split ZL Files" : "Batch Export ZL Frames";
            frameLabel.Text = split ? "Split before frame:" : "Export frame:";
            frame.Minimum = split ? 1 : 0;
            frame.Value = split ? 100 : 0;
            splitHint.Visible = split;
            placements.Visible = !split;
            run.Text = split ? "Split" : "Export";
            UpdatePreview();
        }

        private void AddFiles_Click(object sender, EventArgs e)
        {
            using var open = new OpenFileDialog { Filter = "ZL libraries (*.zl)|*.zl", Multiselect = true };
            if (open.ShowDialog(this) != DialogResult.OK) return;
            foreach (string path in open.FileNames)
                if (!files.Items.Cast<string>().Contains(path, StringComparer.OrdinalIgnoreCase)) files.Items.Add(path);
        }

        private void RemoveFiles_Click(object sender, EventArgs e)
        {
            foreach (var item in files.SelectedItems.Cast<string>().ToArray()) files.Items.Remove(item);
        }

        private void ClearFiles_Click(object sender, EventArgs e) => files.Items.Clear();

        private void Browse_Click(object sender, EventArgs e)
        {
            using var pick = new FolderBrowserDialog();
            if (pick.ShowDialog(this) == DialogResult.OK) folder.Text = pick.SelectedPath;
        }

        private void Repeat_CheckedChanged(object sender, EventArgs e) => interval.Enabled = repeat.Checked;
        private void Naming_Changed(object sender, EventArgs e) => UpdatePreview();
        private void BatchLibraryDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (running) e.Cancel = true;
        }
        internal static string MakeName(string pattern, string source, long number)
        {
            string name = Regex.Replace(pattern.Replace("{file}", Path.GetFileNameWithoutExtension(source)), "#+", m => number.ToString("D" + m.Length));
            if (string.IsNullOrWhiteSpace(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.EndsWith('.') || name.EndsWith(' ') ||
                Regex.IsMatch(name, @"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])($|\.)", RegexOptions.IgnoreCase))
                throw new ArgumentException("The pattern produces an invalid filename.");
            return name;
        }
        private void UpdatePreview()
        {
            try { preview.Text = "Preview: " + MakeName(pattern.Text, "Example.zl", (long)counter.Value) + (split ? ".zl" : ".png"); }
            catch (ArgumentException) { preview.Text = "Invalid filename pattern"; }
        }

        private async void Run(object sender, EventArgs e)
        {
            if (files.Items.Count == 0 || string.IsNullOrWhiteSpace(folder.Text))
            { MessageBox.Show(this, "Add ZL files and choose an output folder."); return; }
            string[] inputs = files.Items.Cast<string>().ToArray();
            string output = folder.Text;
            string template = pattern.Text;
            int first = (int)frame.Value, step = repeat.Checked ? (int)interval.Value : 0;
            long start = (long)counter.Value;
            bool offsets = placements.Checked;
            bool report = includeReport.Checked;
            bool reset = resetCounter.Checked;
            running = true;
            run.Enabled = false;
            using var cancel = new CancellationTokenSource();
            using var progressDialog = new ProgressDialog(Text);
            progressDialog.CancelRequested += (_, _) => cancel.Cancel();
            var progress = new Progress<LibraryProgress>(progressDialog.Report);
            int written = 0;
            try
            {
                _ = progressDialog.Handle;
                var task = Task.Run(() => Process(inputs, output, template, first, step, start, offsets, progress, cancel.Token, () => written++, report, reset));
                progressDialog.FormClosing += (_, args) =>
                {
                    if (task.IsCompleted) return;
                    cancel.Cancel();
                    args.Cancel = true;
                };
                _ = task.ContinueWith(_ => { if (!progressDialog.IsDisposed) progressDialog.BeginInvoke(new Action(progressDialog.Close)); }, TaskScheduler.Default);
                progressDialog.ShowDialog(this);
                await task;
                MessageBox.Show(this, $"Completed. Created {written} files in {output}.", Text);
            }
            catch (OperationCanceledException) { MessageBox.Show(this, $"Cancelled. {written} completed outputs remain in the output folder.", Text); }
            catch (Exception ex) { MessageBox.Show(this, $"Stopped after {written} outputs. Completed outputs remain.\n\n{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { running = false; run.Enabled = true; }
        }

        private void Process(string[] inputs, string output, string template, int first, int step, long number, bool offsets,
            IProgress<LibraryProgress> progress, CancellationToken token, Action completed, bool report = false, bool reset = false)
        {
            output = Path.GetFullPath(output);
            MakeName(template, inputs[0], number);
            Directory.CreateDirectory(output);
            var sourcePaths = new HashSet<string>(inputs.Select(Path.GetFullPath), StringComparer.OrdinalIgnoreCase);
            long startingNumber = number;
            for (int file = 0; file < inputs.Length; file++)
            {
                if (reset) number = startingNumber;
                token.ThrowIfCancellationRequested();
                progress.Report(new LibraryProgress("Loading " + Path.GetFileName(inputs[file]), isMarquee: true) { OverallValue = file, OverallMaximum = inputs.Length });
                Mir3Library library = null;
                try
                {
                    library = new Mir3Library(inputs[file]);
                    if (first >= library.Images.Count)
                        throw new InvalidOperationException($"{inputs[file]} has {library.Images.Count} frames; frame {first} is outside the library.");
                    int position = split ? 0 : first;
                    while (position < library.Images.Count)
                    {
                        token.ThrowIfCancellationRequested();
                        int end = split ? (position == 0 ? first : step == 0 ? library.Images.Count : (int)Math.Min((long)position + step, library.Images.Count)) : position + 1;
                        string path = Path.Combine(output, MakeName(template, inputs[file], number++) + (split ? ".zl" : ".png"));
                        if (sourcePaths.Contains(path) || File.Exists(path)) throw new IOException("Output already exists or matches a source: " + path);
                        if (report && File.Exists(path + ".report.txt")) throw new IOException("Report already exists: " + path + ".report.txt");
                        string placement = Path.Combine(output, "Placements", Path.GetFileNameWithoutExtension(path) + ".txt");
                        if (!split && offsets && File.Exists(placement)) throw new IOException("Placement already exists: " + placement);
                        progress.Report(new LibraryProgress(Path.GetFileName(path), position, library.Images.Count) { OverallValue = file, OverallMaximum = inputs.Length });
                        string temporary = Path.Combine(output, Guid.NewGuid().ToString("N") + ".tmp");
                        try
                        {
                            if (split)
                            {
                                // Payloads are fully loaded by the reader. Keep atlas pages and their IDs,
                                // so every layer and its original encoding survives a split unchanged.
                                var part = new Mir3Library(temporary) { Version = library.Version, ContainerCompression = library.ContainerCompression,
                                    Images = library.Images.GetRange(position, end - position), AtlasPages = library.AtlasPages,
                                    AtlasPageSize = library.AtlasPageSize, AtlasGroupImageCount = 0 };
                                part.Save(temporary, false);
                            }
                            else
                            {
                                var image = library.CreateImage(position, ImageType.Image)?.Image;
                                if (image != null) image.Save(temporary, ImageFormat.Png);
                                else { using var blank = new Bitmap(1, 1); blank.Save(temporary, ImageFormat.Png); }
                            }
                            token.ThrowIfCancellationRequested();
                            File.Move(temporary, path);
                            completed();
                            if (report)
                            {
                                using var reportStream = new FileStream(path + ".report.txt", FileMode.CreateNew);
                                using var reportWriter = new StreamWriter(reportStream);
                                reportWriter.WriteLine(split ? "ZL split report" : "Frame export report");
                                reportWriter.WriteLine("Source: " + inputs[file]);
                                reportWriter.WriteLine("Output: " + path);
                                reportWriter.WriteLine($"Source frames (0-based): {position} to {end - 1}");
                                reportWriter.WriteLine($"Frame count: {end - position}");
                                reportWriter.WriteLine($"Output bytes: {new FileInfo(path).Length}");
                            }
                            if (!split && offsets)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(placement));
                                var image = library.GetImage(position);
                                using var stream = new FileStream(placement, FileMode.CreateNew);
                                using var writer = new StreamWriter(stream);
                                writer.WriteLine(image?.OffSetX ?? 0);
                                writer.WriteLine(image?.OffSetY ?? 0);
                            }
                        }
                        finally { if (File.Exists(temporary)) File.Delete(temporary); }
                        if (!split && step == 0) break;
                        long next = split ? end : (long)position + step;
                        if (next >= library.Images.Count) break;
                        position = (int)next;
                    }
                }
                finally
                {
                    if (library != null)
                    {
                        library.Close();
                        foreach (var image in library.Images)
                        {
                            if (image == null) continue;
                            image.Image?.Dispose(); image.ShadowImage?.Dispose(); image.OverlayImage?.Dispose();
                            image.Preview?.Dispose(); image.ShadowPreview?.Dispose(); image.OverlayPreview?.Dispose();
                            image.Dispose();
                        }
                    }
                }
            }
        }
    }
}
