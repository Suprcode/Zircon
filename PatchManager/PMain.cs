using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;

namespace PatchManager
{
    public partial class PMain : DevExpress.XtraEditors.XtraForm
    {
        public const string PListFileName = "PList.Bin";
        public const string ClientPath = ".\\";
        public const string ClientFileName = "Zircon.exe";

        public DateTime LastSpeedCheck;
        public long TotalUpload, TotalProgress, CurrentProgress, LastUploadProcess;
        public bool Error;

        public PMain()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
            InitializeComponent();
        }

        private void UploadPatchButton_Click(object sender, EventArgs e)
        {
            CreatePatch();
        }
        private async void CreatePatch()
        {
            InterfaceLock(false);

            Progress<string> progress = new Progress<string>(s => StatusLabel.Text = s);

            List<PatchInformation> currentVersion = await Task.Run(() => CreateVersion(progress));
            List<PatchInformation> liveVersion = await Task.Run(() => GetPatchInformation(progress));


            List<PatchInformation> patch = await Task.Run(() => CalculatePatch(currentVersion, liveVersion, progress));

            Task task = Task.Run(() => UploadPatch(patch, progress));

            while (!task.IsCompleted)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                CreateSizeLabel();
            }

            if (Directory.Exists(ClientPath + "Patch\\"))
                Directory.Delete(ClientPath + "Patch\\");

            InterfaceLock(true);

            if (!Error)
                SaveVersion(currentVersion);

            StatusLabel.Text = "Complete.";
            UploadSizeLabel.Text = "Complete.";
            UploadSpeedLabel.Text = "Complete.";
        }

        private void InterfaceLock(bool enabled)
        {
            CleanClientButtonEdit.Enabled = enabled;
            HostTextEdit.Enabled = enabled;
            UseLoginCheckEdit.Enabled = enabled;
            UsernameTextEdit.Enabled = enabled;
            PasswordTextEdit.Enabled = enabled;
            UploadPatchButton.Enabled = enabled;

        }

        private List<PatchInformation> CreateVersion(IProgress<string> progress)
        {
            try
            {
                string[] files = Directory.GetFiles(Config.CleanClient, "*.*", SearchOption.AllDirectories);

                PatchInformation[] list = new PatchInformation[files.Length];
                ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                int count = 0;
                Parallel.For(0, files.Length, po, i =>
                {
                    list[i] = new PatchInformation(files[i]);
                    progress.Report($"Version Created: File {Interlocked.Increment(ref count)} of {files.Length}");
                });

                return list.ToList();
            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return null;
        }
        private List<PatchInformation> GetPatchInformation(IProgress<string> progress)
        {
            try
            {
                progress.Report("Downloading Patch Information");

                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin)
                        client.Credentials = new NetworkCredential(Config.Username, Config.Password);


                    using (MemoryStream mStream = new MemoryStream(client.DownloadData(Config.Host + PListFileName)))
                    using (BinaryReader reader = new BinaryReader(mStream))
                    {
                        List<PatchInformation> list = new List<PatchInformation>();

                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                            list.Add(new PatchInformation(reader));

                        return list;
                    }
                }

            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return null;
        }
        private List<PatchInformation> CalculatePatch(List<PatchInformation> current, List<PatchInformation> live, IProgress<string> progress)
        {
            List<PatchInformation> patch = new List<PatchInformation>();

            if (current == null) return patch;

            ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 8 };
            int count = 0;
            Parallel.For(0, current.Count, po, i =>
            {
                PatchInformation file = current[i];
                PatchInformation lFile = live?.FirstOrDefault(x => x.FileName == file.FileName);

                if (lFile != null && IsMatch(lFile.CheckSum, file.CheckSum))
                {
                    file.CompressedLength = lFile.CompressedLength;
                    return;
                }

                if (!Directory.Exists(".\\Patch\\"))
                    Directory.CreateDirectory(".\\Patch\\");


                string webFileName = file.FileName.Replace("\\", "-") + ".gz";
                file.CompressedLength = Compress(Config.CleanClient + file.FileName, $".\\Patch\\{webFileName}");

                Interlocked.Add(ref TotalUpload, file.CompressedLength);

                lock (patch)
                    patch.Add(file);

                progress.Report($"File Created: {Interlocked.Increment(ref count)} of {current.Count}");

            });

            return patch;
        }
        public static bool IsMatch(byte[] a, byte[] b, long offSet = 0)
        {
            if (b == null || a == null || b.Length + offSet > a.Length || offSet < 0) return false;

            for (int i = 0; i < b.Length; i++)
                if (a[offSet + i] != b[i])
                    return false;

            return true;
        }
        private static long Compress(string sourceFile, string destFile)
        {
            using (FileStream tofile = File.Create(destFile))
            using (FileStream fromfile = File.OpenRead(sourceFile))
            {
                using (GZipStream gStream = new GZipStream(tofile, CompressionMode.Compress, true))
                    fromfile.CopyTo(gStream);

                return tofile.Length;
            }
        }


        private void UploadPatch(List<PatchInformation> patch, IProgress<string> progress)
        {
            for (int i = 0; i < patch.Count; i++)
            {
                PatchInformation file = patch[i];

                if (!Upload(file, progress))
                {
                    Error = true;
                    return;
                }

                progress.Report($"Files Uploaded: {i + 1} of {patch.Count}");
            }
        }

        private void PMain_Load(object sender, EventArgs e)
        {
            CleanClientButtonEdit.EditValue = Config.CleanClient;
            HostTextEdit.EditValue = Config.Host;
            UseLoginCheckEdit.EditValue = Config.UseLogin;
            UsernameTextEdit.EditValue = Config.Username;
            PasswordTextEdit.EditValue = Config.Password;
        }
        
        private void CleanClientButtonEdit_EditValueChanged(object sender, EventArgs e)
        {
            Config.CleanClient = (string)CleanClientButtonEdit.EditValue;
        }

        private void HostTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            Config.Host = (string)HostTextEdit.EditValue;
        }

        private void UseLoginCheckEdit_CheckedChanged(object sender, EventArgs e)
        {
            Config.UseLogin = (bool)UseLoginCheckEdit.EditValue;
        }

        private void UsernameTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            Config.Username = (string)UsernameTextEdit.EditValue;
        }

        private void PasswordTextEdit_EditValueChanged(object sender, EventArgs e)
        {
            Config.Password = (string)PasswordTextEdit.EditValue;
        }

        private bool Upload(PatchInformation file, IProgress<string> progress)
        {
            string webFileName = file.FileName.Replace("\\", "-") + ".gz";

            try
            {
                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin) client.Credentials = new NetworkCredential(Config.Username, Config.Password);

                    bool uploading = true;
                    client.UploadProgressChanged += (o, e) => CurrentProgress = e.BytesSent;
                    client.UploadFileCompleted += (o, e) =>
                    {
                        File.Delete($".\\Patch\\{webFileName}");
                        uploading = false;
                    };

                    if (!Directory.Exists(ClientPath + "Patch\\"))
                        Directory.CreateDirectory(ClientPath + "Patch\\");

                    client.UploadFileAsync(new Uri(Config.Host + webFileName), $"{ClientPath}Patch\\{webFileName}");

                    while (uploading)
                        Task.Delay(TimeSpan.FromMilliseconds(1));

                    CurrentProgress = 0;
                    TotalProgress += file.CompressedLength;
                }

                return true;
            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return false;
        }

        private void CreateSizeLabel()
        {
            const decimal KB = 1024;
            const decimal MB = KB * 1024;
            const decimal GB = MB * 1024;

            long progress = TotalProgress + CurrentProgress;

            StringBuilder text = new StringBuilder();

            if (progress > GB)
                text.Append($"{progress / GB:#,##0.0}GB");
            else if (progress > MB)
                text.Append($"{progress / MB:#,##0.0}MB");
            else if (progress > KB)
                text.Append($"{progress / KB:#,##0}KB");
            else
                text.Append($"{progress:#,##0}B");

            if (TotalUpload > GB)
                text.Append($" / {TotalUpload / GB:#,##0.0}GB");
            else if (TotalUpload > MB)
                text.Append($" / {TotalUpload / MB:#,##0.0}MB");
            else if (TotalUpload > KB)
                text.Append($" / {TotalUpload / KB:#,##0}KB");
            else
                text.Append($" / {TotalUpload:#,##0}B");

            UploadSizeLabel.Text = text.ToString();

            if (TotalUpload > 0)
                TotalProgressBar.EditValue = Math.Max(0, Math.Min(100, (int)(progress * 100 / TotalUpload)));

            long speed = (progress - LastUploadProcess) * TimeSpan.TicksPerSecond / (Time.Now.Ticks - LastSpeedCheck.Ticks); //May cause errors?
            LastUploadProcess = progress;

            if (speed > GB)
                UploadSpeedLabel.Text = $"{speed / GB:#,##0.0}GBps";
            else if (speed > MB)
                UploadSpeedLabel.Text = $"{speed / MB:#,##0.0}MBps";
            else if (speed > KB)
                UploadSpeedLabel.Text = $"{speed / KB:#,##0}KBps";
            else
                UploadSpeedLabel.Text = $"{speed:#,##0}Bps";

            LastSpeedCheck = Time.Now;
        }

        private void SaveVersion(List<PatchInformation> current)
        {
            using (MemoryStream mStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(mStream))
            {
                foreach (PatchInformation info in current)
                    info.Save(writer);

                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin)
                        client.Credentials = new NetworkCredential(Config.Username, Config.Password);

                    client.UploadData(new Uri(Config.Host + PListFileName), mStream.ToArray());
                }
            }
        }
    }
}
