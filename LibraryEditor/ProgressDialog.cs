using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LibraryEditor
{
    public sealed partial class ProgressDialog : Form
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private LibraryProgress _lastProgress;
        private bool _cancelRequested;

        public event EventHandler CancelRequested;

        public ProgressDialog(string title)
        {
            InitializeComponent();
            Text = title;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _stopwatch.Start();
            _timer.Start();
            UpdateTimeLabel();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (_cancelRequested)
                return;

            _cancelRequested = true;
            _cancelButton.Enabled = false;
            _cancelButton.Text = "Cancelling";
            _messageLabel.Text = "Cancelling...";
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Report(LibraryProgress progress)
        {
            if (progress == null)
                return;

            _messageLabel.Text = progress.Message ?? string.Empty;
            _lastProgress = progress;
            UpdateOverallProgress(progress);
            UpdateCurrentProgress(progress);
            UpdateGroupProgress(progress);
            UpdateTimeLabel();
        }

        private void UpdateOverallProgress(LibraryProgress progress)
        {
            if (progress.OverallMaximum <= 0)
            {
                _overallLabel.Text = progress.OverallText ?? "Files";
                _overallProgressBar.Value = 0;
                return;
            }

            int value = Math.Min(Math.Max(0, progress.OverallValue), progress.OverallMaximum);
            _overallLabel.Text = !string.IsNullOrEmpty(progress.OverallText)
                ? progress.OverallText
                : $"File {value:N0} of {progress.OverallMaximum:N0}";
            SetProgressValue(_overallProgressBar, value, Math.Max(1, progress.OverallMaximum));
        }

        private void UpdateCurrentProgress(LibraryProgress progress)
        {
            if (progress.IsMarquee || progress.Maximum <= 0)
            {
                _currentLabel.Text = progress.CountText ?? "Current file";
                _currentProgressBar.Style = ProgressBarStyle.Marquee;
                return;
            }

            if (_currentProgressBar.Style != ProgressBarStyle.Continuous)
                _currentProgressBar.Style = ProgressBarStyle.Continuous;

            int value = Math.Min(Math.Max(0, progress.Value), progress.Maximum);
            _currentLabel.Text = !string.IsNullOrEmpty(progress.CountText)
                ? progress.CountText
                : $"Image {value:N0} of {progress.Maximum:N0}";
            SetProgressValue(_currentProgressBar, value, Math.Max(1, progress.Maximum));
        }

        private void UpdateGroupProgress(LibraryProgress progress)
        {
            if (progress.GroupMaximum <= 0)
            {
                _groupLabel.Text = progress.GroupText ?? "Current step";
                _groupProgressBar.Value = 0;
                return;
            }

            int value = Math.Min(Math.Max(0, progress.GroupValue), progress.GroupMaximum);
            _groupLabel.Text = !string.IsNullOrEmpty(progress.GroupText)
                ? progress.GroupText
                : $"Group {value:N0} of {progress.GroupMaximum:N0}";
            SetProgressValue(_groupProgressBar, value, Math.Max(1, progress.GroupMaximum));
        }

        private static void SetProgressValue(ProgressBar progressBar, int value, int maximum)
        {
            if (progressBar.Maximum > maximum && progressBar.Value > maximum)
                progressBar.Value = maximum;

            progressBar.Maximum = maximum;
            progressBar.Value = Math.Min(Math.Max(progressBar.Minimum, value), maximum);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTimeLabel();
        }

        private void UpdateTimeLabel()
        {
            TimeSpan elapsed = _stopwatch.Elapsed;
            string text = "Elapsed: " + FormatDuration(elapsed);

            if (_lastProgress != null && !_lastProgress.IsMarquee && _lastProgress.Maximum > 0 && _lastProgress.Value > 0)
            {
                double secondsPerItem = elapsed.TotalSeconds / _lastProgress.Value;
                int remainingItems = Math.Max(0, _lastProgress.Maximum - _lastProgress.Value);
                TimeSpan remaining = TimeSpan.FromSeconds(secondsPerItem * remainingItems);
                text += "   Remaining: " + FormatDuration(remaining);
            }
            else
            {
                text += "   Remaining: estimating...";
            }

            _timeLabel.Text = text;
        }

        private static string FormatDuration(TimeSpan time)
        {
            if (time.TotalHours >= 1)
                return time.ToString(@"h\:mm\:ss");

            return time.ToString(@"m\:ss");
        }
    }
}
