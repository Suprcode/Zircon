using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryEditor
{
    partial class BatchLibraryDialog
    {
        private IContainer components = null;
        private Button addFiles;
        private Button removeFiles;
        private Button clearFiles;
        private ListBox files;
        private Label folderLabel;
        private TextBox folder;
        private Button browse;
        private Label frameLabel;
        private NumericUpDown frame;
        private Label frameHint;
        private CheckBox repeat;
        private NumericUpDown interval;
        private Label intervalLabel;
        private Label patternLabel;
        private TextBox pattern;
        private Label counterLabel;
        private NumericUpDown counter;
        private CheckBox resetCounter;
        private Label previewLabel;
        private Label preview;
        private Label namingHint;
        private Label splitHint;
        private CheckBox placements;
        private CheckBox includeReport;
        private Button run;
        private Button close;
        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            addFiles = new Button();
            removeFiles = new Button();
            clearFiles = new Button();
            files = new ListBox();
            folderLabel = new Label();
            folder = new TextBox();
            browse = new Button();
            frameLabel = new Label();
            frame = new NumericUpDown();
            frameHint = new Label();
            repeat = new CheckBox();
            interval = new NumericUpDown();
            intervalLabel = new Label();
            patternLabel = new Label();
            pattern = new TextBox();
            counterLabel = new Label();
            counter = new NumericUpDown();
            resetCounter = new CheckBox();
            previewLabel = new Label();
            preview = new Label();
            namingHint = new Label();
            splitHint = new Label();
            placements = new CheckBox();
            includeReport = new CheckBox();
            run = new Button();
            close = new Button();
            ((ISupportInitialize)frame).BeginInit();
            ((ISupportInitialize)interval).BeginInit();
            ((ISupportInitialize)counter).BeginInit();
            SuspendLayout();
            // addFiles
            addFiles.Location = new Point(16, 10);
            addFiles.Name = "addFiles";
            addFiles.Size = new Size(90, 26);
            addFiles.TabIndex = 0;
            addFiles.Text = "Add Files";
            addFiles.Click += AddFiles_Click;
            // removeFiles
            removeFiles.Location = new Point(112, 10);
            removeFiles.Name = "removeFiles";
            removeFiles.Size = new Size(90, 26);
            removeFiles.TabIndex = 1;
            removeFiles.Text = "Remove";
            removeFiles.Click += RemoveFiles_Click;
            // clearFiles
            clearFiles.Location = new Point(208, 10);
            clearFiles.Name = "clearFiles";
            clearFiles.Size = new Size(90, 26);
            clearFiles.TabIndex = 2;
            clearFiles.Text = "Clear";
            clearFiles.Click += ClearFiles_Click;
            // files
            files.Location = new Point(16, 38);
            files.Name = "files";
            files.Size = new Size(528, 109);
            files.TabIndex = 3;
            files.HorizontalScrollbar = true;
            files.SelectionMode = SelectionMode.MultiExtended;
            files.IntegralHeight = false;
            // folderLabel
            folderLabel.Location = new Point(16, 168);
            folderLabel.Name = "folderLabel";
            folderLabel.Size = new Size(140, 23);
            folderLabel.TabIndex = 4;
            folderLabel.Text = "Output folder:";
            folderLabel.TextAlign = ContentAlignment.MiddleLeft;
            // folder
            folder.Location = new Point(156, 168);
            folder.Name = "folder";
            folder.Size = new Size(292, 23);
            folder.TabIndex = 5;
            // browse
            browse.Location = new Point(454, 166);
            browse.Name = "browse";
            browse.Size = new Size(90, 26);
            browse.TabIndex = 6;
            browse.Text = "Browse...";
            browse.Click += Browse_Click;
            // frameLabel
            frameLabel.Location = new Point(16, 204);
            frameLabel.Name = "frameLabel";
            frameLabel.Size = new Size(140, 23);
            frameLabel.TabIndex = 7;
            frameLabel.Text = "Split before frame:";
            frameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // frame
            frame.Location = new Point(156, 204);
            frame.Name = "frame";
            frame.Size = new Size(120, 23);
            frame.TabIndex = 8;
            frame.Maximum = int.MaxValue;
            frame.Minimum = 1;
            frame.Value = 100;
            // frameHint
            frameHint.Location = new Point(286, 204);
            frameHint.Name = "frameHint";
            frameHint.Size = new Size(258, 23);
            frameHint.TabIndex = 9;
            frameHint.Text = "Frame numbers start at 0";
            frameHint.TextAlign = ContentAlignment.MiddleLeft;
            // repeat
            repeat.Location = new Point(16, 240);
            repeat.Name = "repeat";
            repeat.Size = new Size(130, 23);
            repeat.TabIndex = 10;
            repeat.Text = "Repeat every";
            repeat.CheckedChanged += Repeat_CheckedChanged;
            // interval
            interval.Location = new Point(156, 240);
            interval.Name = "interval";
            interval.Size = new Size(120, 23);
            interval.TabIndex = 11;
            interval.Maximum = int.MaxValue;
            interval.Minimum = 1;
            interval.Value = 100;
            interval.Enabled = false;
            // intervalLabel
            intervalLabel.Location = new Point(286, 240);
            intervalLabel.Name = "intervalLabel";
            intervalLabel.Size = new Size(100, 23);
            intervalLabel.TabIndex = 12;
            intervalLabel.Text = "frames";
            intervalLabel.TextAlign = ContentAlignment.MiddleLeft;
            // patternLabel
            patternLabel.Location = new Point(16, 276);
            patternLabel.Name = "patternLabel";
            patternLabel.Size = new Size(140, 23);
            patternLabel.TabIndex = 13;
            patternLabel.Text = "Filename pattern:";
            patternLabel.TextAlign = ContentAlignment.MiddleLeft;
            // pattern
            pattern.Location = new Point(156, 276);
            pattern.Name = "pattern";
            pattern.Size = new Size(388, 23);
            pattern.TabIndex = 14;
            pattern.Text = "{file}-###";
            pattern.TextChanged += Naming_Changed;
            // counterLabel
            counterLabel.Location = new Point(16, 312);
            counterLabel.Name = "counterLabel";
            counterLabel.Size = new Size(140, 23);
            counterLabel.TabIndex = 15;
            counterLabel.Text = "Starting counter:";
            counterLabel.TextAlign = ContentAlignment.MiddleLeft;
            // counter
            counter.Location = new Point(156, 312);
            counter.Name = "counter";
            counter.Size = new Size(120, 23);
            counter.TabIndex = 16;
            counter.Maximum = int.MaxValue;
            counter.Value = 1;
            counter.ValueChanged += Naming_Changed;
            // resetCounter
            resetCounter.Location = new Point(286, 312);
            resetCounter.Name = "resetCounter";
            resetCounter.Size = new Size(258, 23);
            resetCounter.TabIndex = 17;
            resetCounter.Text = "Reset counter for each file";
            // previewLabel
            previewLabel.Location = new Point(16, 348);
            previewLabel.Name = "previewLabel";
            previewLabel.Size = new Size(140, 23);
            previewLabel.TabIndex = 18;
            previewLabel.Text = "Filename preview:";
            previewLabel.TextAlign = ContentAlignment.MiddleLeft;
            // preview
            preview.Location = new Point(156, 348);
            preview.Name = "preview";
            preview.Size = new Size(388, 23);
            preview.TabIndex = 19;
            preview.Text = "Preview: Dragon-001.zl";
            preview.TextAlign = ContentAlignment.MiddleLeft;
            preview.AutoEllipsis = true;
            // namingHint
            namingHint.Location = new Point(16, 388);
            namingHint.Name = "namingHint";
            namingHint.Size = new Size(528, 60);
            namingHint.TabIndex = 20;
            namingHint.Text = "Example: {file}-### with Dragon.zl produces Dragon-001, Dragon-002...\nReset counter restarts at the starting value for each source file.\nExtensions are added automatically. Existing files are never overwritten.";
            namingHint.TextAlign = ContentAlignment.MiddleLeft;
            // splitHint
            splitHint.Location = new Point(16, 454);
            splitHint.Name = "splitHint";
            splitHint.Size = new Size(528, 38);
            splitHint.TabIndex = 21;
            splitHint.Text = "All frames are kept, including the final shorter segment.\nEach output starts at frame 0.";
            splitHint.TextAlign = ContentAlignment.MiddleLeft;
            // placements
            placements.Location = new Point(16, 454);
            placements.Name = "placements";
            placements.Size = new Size(528, 23);
            placements.TabIndex = 22;
            placements.Text = "Export placement offsets alongside PNGs";
            placements.Checked = true;
            placements.CheckState = CheckState.Checked;
            placements.Visible = false;
            // includeReport
            includeReport.Location = new Point(16, 496);
            includeReport.Name = "includeReport";
            includeReport.Size = new Size(528, 23);
            includeReport.TabIndex = 23;
            includeReport.Text = "Include report file";
            // run
            run.Location = new Point(354, 536);
            run.Name = "run";
            run.Size = new Size(90, 28);
            run.TabIndex = 24;
            run.Text = "Split";
            run.Click += Run;
            // close
            close.Location = new Point(450, 536);
            close.Name = "close";
            close.Size = new Size(90, 28);
            close.TabIndex = 25;
            close.Text = "Cancel";
            close.DialogResult = DialogResult.Cancel;
            AcceptButton = run;
            CancelButton = close;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 580);
            Controls.Add(addFiles);
            Controls.Add(removeFiles);
            Controls.Add(clearFiles);
            Controls.Add(files);
            Controls.Add(folderLabel);
            Controls.Add(folder);
            Controls.Add(browse);
            Controls.Add(frameLabel);
            Controls.Add(frame);
            Controls.Add(frameHint);
            Controls.Add(repeat);
            Controls.Add(interval);
            Controls.Add(intervalLabel);
            Controls.Add(patternLabel);
            Controls.Add(pattern);
            Controls.Add(counterLabel);
            Controls.Add(counter);
            Controls.Add(resetCounter);
            Controls.Add(previewLabel);
            Controls.Add(preview);
            Controls.Add(namingHint);
            Controls.Add(splitHint);
            Controls.Add(placements);
            Controls.Add(includeReport);
            Controls.Add(run);
            Controls.Add(close);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BatchLibraryDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Batch Split ZL Files";
            FormClosing += BatchLibraryDialog_FormClosing;
            ((ISupportInitialize)frame).EndInit();
            ((ISupportInitialize)interval).EndInit();
            ((ISupportInitialize)counter).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
