using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryEditor
{
    partial class ProgressDialog
    {
        private IContainer components;
        private Label _messageLabel;
        private Label _overallLabel;
        private Label _currentLabel;
        private Label _groupLabel;
        private Label _timeLabel;
        private ProgressBar _overallProgressBar;
        private ProgressBar _currentProgressBar;
        private ProgressBar _groupProgressBar;
        private Button _cancelButton;
        private Timer _timer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            _messageLabel = new Label();
            _overallLabel = new Label();
            _overallProgressBar = new ProgressBar();
            _currentLabel = new Label();
            _currentProgressBar = new ProgressBar();
            _groupLabel = new Label();
            _groupProgressBar = new ProgressBar();
            _timeLabel = new Label();
            _cancelButton = new Button();
            _timer = new Timer(components);
            SuspendLayout();
            // 
            // _messageLabel
            // 
            _messageLabel.AutoEllipsis = true;
            _messageLabel.Location = new Point(14, 14);
            _messageLabel.Name = "_messageLabel";
            _messageLabel.Size = new Size(490, 18);
            _messageLabel.TabIndex = 0;
            _messageLabel.Text = "Working...";
            // 
            // _overallLabel
            // 
            _overallLabel.AutoEllipsis = true;
            _overallLabel.Location = new Point(14, 36);
            _overallLabel.Name = "_overallLabel";
            _overallLabel.Size = new Size(490, 18);
            _overallLabel.TabIndex = 1;
            _overallLabel.Text = "Files";
            // 
            // _overallProgressBar
            // 
            _overallProgressBar.Location = new Point(14, 58);
            _overallProgressBar.Name = "_overallProgressBar";
            _overallProgressBar.Size = new Size(490, 12);
            _overallProgressBar.Style = ProgressBarStyle.Continuous;
            _overallProgressBar.TabIndex = 2;
            // 
            // _currentLabel
            // 
            _currentLabel.AutoEllipsis = true;
            _currentLabel.Location = new Point(14, 80);
            _currentLabel.Name = "_currentLabel";
            _currentLabel.Size = new Size(490, 18);
            _currentLabel.TabIndex = 3;
            _currentLabel.Text = "Current file";
            // 
            // _currentProgressBar
            // 
            _currentProgressBar.Location = new Point(14, 102);
            _currentProgressBar.Name = "_currentProgressBar";
            _currentProgressBar.Size = new Size(490, 12);
            _currentProgressBar.Style = ProgressBarStyle.Marquee;
            _currentProgressBar.TabIndex = 4;
            // 
            // _groupLabel
            // 
            _groupLabel.AutoEllipsis = true;
            _groupLabel.Location = new Point(14, 124);
            _groupLabel.Name = "_groupLabel";
            _groupLabel.Size = new Size(490, 18);
            _groupLabel.TabIndex = 5;
            _groupLabel.Text = "Current step";
            // 
            // _groupProgressBar
            // 
            _groupProgressBar.Location = new Point(14, 146);
            _groupProgressBar.Name = "_groupProgressBar";
            _groupProgressBar.Size = new Size(490, 12);
            _groupProgressBar.Style = ProgressBarStyle.Continuous;
            _groupProgressBar.TabIndex = 6;
            // 
            // _timeLabel
            // 
            _timeLabel.AutoEllipsis = true;
            _timeLabel.Location = new Point(14, 170);
            _timeLabel.Name = "_timeLabel";
            _timeLabel.Size = new Size(330, 18);
            _timeLabel.TabIndex = 7;
            // 
            // _cancelButton
            // 
            _cancelButton.Location = new Point(414, 166);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Size = new Size(90, 26);
            _cancelButton.TabIndex = 8;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += CancelButton_Click;
            // 
            // _timer
            // 
            _timer.Interval = 500;
            _timer.Tick += Timer_Tick;
            // 
            // ProgressDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 214);
            ControlBox = false;
            Controls.Add(_messageLabel);
            Controls.Add(_overallLabel);
            Controls.Add(_overallProgressBar);
            Controls.Add(_currentLabel);
            Controls.Add(_currentProgressBar);
            Controls.Add(_groupLabel);
            Controls.Add(_groupProgressBar);
            Controls.Add(_timeLabel);
            Controls.Add(_cancelButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Progress";
            ResumeLayout(false);
        }
    }
}
