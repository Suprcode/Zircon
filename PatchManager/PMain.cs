using System;
using System.Collections.Concurrent;
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
using WinSCP;

namespace PatchManager
{
    public partial class PMain : DevExpress.XtraEditors.XtraForm
    {
        public const string PListFileName = "PList.Bin";

        public const string ClientFileName = "Zircon.exe";

        public const string TempDownloadDirectory = "In";

        public long TotalUpload, TotalProgress, CurrentProgress, TotalProgressPercent;
        public long Speed;
        public bool Error;

        public PMain()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
            InitializeComponent();
        }

        #region Event Handlers
        private void PMain_Load(object sender, EventArgs e)
        {
            CleanClientButtonEdit.EditValue = Config.CleanClient;
            HostTextEdit.EditValue = Config.Host;
            UseLoginCheckEdit.EditValue = Config.UseLogin;
            UsernameTextEdit.EditValue = Config.Username;
            PasswordTextEdit.EditValue = Config.Password;
            ProtocolDropDown.EditValue = Config.Protocol;
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

        private void ProtocolDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.Protocol = (string)ProtocolDropDown.EditValue;
        }

        private void UploadPatchButton_Click(object sender, EventArgs e)
        {
            CreatePatch();
        }
        #endregion

        private void InterfaceLock(bool enabled)
        {
            CleanClientButtonEdit.Enabled = enabled;
            HostTextEdit.Enabled = enabled;
            UseLoginCheckEdit.Enabled = enabled;
            UsernameTextEdit.Enabled = enabled;
            PasswordTextEdit.Enabled = enabled;
            UploadPatchButton.Enabled = enabled;
            ProtocolDropDown.Enabled = enabled;
        }


        private async void CreatePatch()
        {
            InterfaceLock(false);

            Progress<string> progress = new Progress<string>(s => StatusLabel.Text = s);

            List<PatchInformation> currentVersion = await Task.Run(() => CreateVersion(progress));
            List<PatchInformation> liveVersion = await Task.Run(() => GetPatchInformation(progress));

            List<PatchInformation> patch = await Task.Run(() => CalculatePatch(currentVersion, liveVersion, progress));

            Task task = Task.Run(() => UploadFiles(patch, progress));

            while (!task.IsCompleted)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                CreateSizeLabel();
            }

            InterfaceLock(true);

            if (!Error)
                SaveVersion(currentVersion);

            if (Directory.Exists(".\\Patch\\"))
                Directory.Delete(".\\Patch\\", true);

            if (Directory.Exists(TempDownloadDirectory))
                Directory.Delete(TempDownloadDirectory, true);

            StatusLabel.Text = "Complete.";
            UploadSizeLabel.Text = "Complete.";
            UploadSpeedLabel.Text = "Complete.";
        }

        private void CreateSizeLabel()
        {
            const decimal KB = 1024;
            const decimal MB = KB * 1024;
            const decimal GB = MB * 1024;

            long progress = TotalProgress;

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
                TotalProgressBar.EditValue = Math.Max(0, Math.Min(100, TotalProgressPercent));

            long speed = Speed;

            if (speed > GB)
                UploadSpeedLabel.Text = $"{speed / GB:#,##0.0}GBps";
            else if (speed > MB)
                UploadSpeedLabel.Text = $"{speed / MB:#,##0.0}MBps";
            else if (speed > KB)
                UploadSpeedLabel.Text = $"{speed / KB:#,##0}KBps";
            else
                UploadSpeedLabel.Text = $"{speed:#,##0}Bps";
        }


        private void OpenSession(Session session)
        {
            if (session.Opened) return;

            Uri uri = null;

            if (!string.IsNullOrEmpty(Config.Host))
            {
                uri = new Uri(Config.Host);
            }

            if (!Protocol.TryParse(Config.Protocol, true, out Protocol protocol))
            {
                protocol = Protocol.Ftp;
            }

            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = protocol,
                HostName = uri.Host,
                UserName = Config.Username,
                Password = Config.Password
            };

            if (sessionOptions.Protocol == Protocol.Sftp)
            {
                var fingerprint = session.ScanFingerprint(sessionOptions, "SHA-256");

                sessionOptions.SshHostKeyFingerprint = fingerprint;
            }

            session.Open(sessionOptions);
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
            using Session session = new Session();
            OpenSession(session);

            try
            {
                progress.Report("Downloading Patch Information");

                if (!Directory.Exists(TempDownloadDirectory))
                    Directory.CreateDirectory(TempDownloadDirectory);

                var rootPath = (new Uri(Config.Host)).AbsolutePath;

                TransferOptions transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary,
                    OverwriteMode = OverwriteMode.Overwrite
                };

                if (!session.FileExists(Path.Combine(rootPath, PListFileName)))
                {
                    progress.Report("Patch Information Not Found");

                    return null;
                }

                var result = session.GetFiles(Path.Combine(rootPath, PListFileName), Path.Combine(TempDownloadDirectory, PListFileName), options: transferOptions);
                result.Check();

                using BinaryReader reader = new BinaryReader(File.Open(Path.Combine(TempDownloadDirectory, PListFileName), FileMode.Open, FileAccess.Read));

                List<PatchInformation> list = new List<PatchInformation>();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                    list.Add(new PatchInformation(reader));

                return list;
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

            int count = 0;

            Parallel.For(0, current.Count, new ParallelOptions { MaxDegreeOfParallelism = 8 }, i =>
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

                file.UploadFileName = ".\\Patch\\" + webFileName;
                file.PatchFileName = Path.Combine(Directory.GetCurrentDirectory(), $"Patch\\{webFileName}");

                file.CompressedLength = Compress(Config.CleanClient + file.FileName, file.PatchFileName);
                Interlocked.Add(ref TotalUpload, file.CompressedLength);

                lock (patch)
                    patch.Add(file);

                progress.Report($"File Created: {Interlocked.Increment(ref count)} of {current.Count}");

            });

            return patch;
        }

        private bool UploadFiles(List<PatchInformation> patch, IProgress<string> progress)
        {
            var rootPath = (new Uri(Config.Host)).AbsolutePath;

            using Session session = new Session();

            int current = 0;

            session.FileTransferProgress += (o, e) =>
            {
                Speed = e.CPS;

                TotalProgress = (long)(e.OverallProgress * TotalUpload);
                TotalProgressPercent  = (long)(e.OverallProgress * 100);
            };

            session.FileTransferred += (o, e) =>
            {
                progress.Report($"Files Uploaded: {Interlocked.Increment(ref current)} of {patch.Count}");
            };

            OpenSession(session);

            TransferOptions transferOptions = new TransferOptions
            {
                TransferMode = TransferMode.Binary,
                OverwriteMode = OverwriteMode.Overwrite
            };

            if (!session.FileExists(rootPath))
            {
                session.CreateDirectory(rootPath);
            }

            var result = session.PutFilesToDirectory(".\\Patch\\", rootPath, options: transferOptions);
            result.Check();

            return true;
        }

        private void SaveVersion(List<PatchInformation> current)
        {
            if (!Directory.Exists(".\\Patch\\"))
                Directory.CreateDirectory(".\\Patch\\");

            using (MemoryStream mStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(mStream))
            {
                foreach (PatchInformation info in current)
                    info.Save(writer);

                var tempFilePath = Path.Combine(".\\Patch\\", PListFileName).Replace(@"\", "/");

                File.WriteAllBytes(tempFilePath, mStream.ToArray());

                var rootPath = (new Uri(Config.Host)).AbsolutePath;

                using Session session = new Session();

                session.FileTransferProgress += (o, e) =>
                {
                };

                session.FileTransferred += (o, e) =>
                {
                };

                OpenSession(session);

                TransferOptions transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary,
                    OverwriteMode = OverwriteMode.Overwrite
                };

                var result = session.PutFileToDirectory(tempFilePath, rootPath, options: transferOptions);
            }
        }


        #region Helpers
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
            var dir = Path.GetDirectoryName(destFile);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (FileStream tofile = File.Create(destFile))
            using (FileStream fromfile = File.OpenRead(sourceFile))
            {
                using (GZipStream gStream = new GZipStream(tofile, CompressionMode.Compress, true))
                    fromfile.CopyTo(gStream);

                return tofile.Length;
            }
        }
        #endregion
    }
}
