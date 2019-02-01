using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;

namespace Launcher
{
    public partial class LMain : DevExpress.XtraEditors.XtraForm
    {
        public const string PListFileName = "PList.Bin";
        public const string ClientPath = ".\\";
        public const string ClientFileName = "Zircon.exe";

        public DateTime LastSpeedCheck;
        public long TotalDownload, TotalProgress, CurrentProgress, LastDownloadProcess;
        public bool NeedUpdate;

        public static bool HasError;

        public LMain()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;

            InitializeComponent();

        }

        private void LMain_Load(object sender, EventArgs e)
        {
            CheckPatch(false);
        }

        private void PatchNotesHyperlinkControl_HyperlinkClick(object sender, DevExpress.Utils.HyperlinkClickEventArgs e)
        {
            PatchNotesHyperlinkControl.LinkVisited = true;
            Process.Start(e.Link);
        }

        private void RepairButton_Click(object sender, EventArgs e)
        {
            CheckPatch(true);
        }

        private async void CheckPatch(bool repair)
        {
            HasError = false;
            RepairButton.Enabled = false;
            StartGameButton.Enabled = false;
            TotalDownload = 0;
            TotalProgress = 0;
            CurrentProgress = 0;
            TotalProgressBar.EditValue = 0;
            LastSpeedCheck = Time.Now;
            NeedUpdate = false;

            Progress<string> progress = new Progress<string>(s => StatusLabel.Text = s);

            List<PatchInformation> liveVersion = await Task.Run(() => GetPatchInformation(progress));

            if (liveVersion == null)
            {
                DownloadSizeLabel.Text = "Downloading failed.";
                RepairButton.Enabled = true;
                StartGameButton.Enabled = true;
                return;
            }

            List<PatchInformation> currentVersion = repair ? null : await Task.Run(() => LoadVersion(progress));
            List<PatchInformation> patch = await Task.Run(() => CalculatePatch(liveVersion, currentVersion, progress));

            StatusLabel.Text = "Downloading";
            CreateSizeLabel();

            Task task = Task.Run(() => DownloadPatch(patch, progress));

            while (!task.IsCompleted)
            {
                CreateSizeLabel();

                await Task.Delay(100);
            }

            CreateSizeLabel();

            SaveVersion(liveVersion);

            StatusLabel.Text = "Complete";
            DownloadSizeLabel.Text = "Complete.";
            DownloadSpeedLabel.Text = "Complete.";

            if (Directory.Exists(ClientPath + "Patch\\"))
                Directory.Delete(ClientPath + "Patch\\", true);

            if (NeedUpdate)
            {
                File.WriteAllBytes(Program.PatcherFileName, Properties.Resources.Patcher);
                Process.Start(Program.PatcherFileName, $"\"{Application.ExecutablePath}.tmp\" \"{Application.ExecutablePath}\"");
                Environment.Exit(0);
            }

            try
            {
                if (File.Exists(Program.PatcherFileName))
                    File.Delete(Program.PatcherFileName);
            }
            catch (Exception) {}

            RepairButton.Enabled = true;
            StartGameButton.Enabled = true;
        }
        private void CreateSizeLabel()
        {
            const decimal KB = 1024;
            const decimal MB = KB*1024;
            const decimal GB = MB*1024;

            long progress = TotalProgress + CurrentProgress;

            StringBuilder text = new StringBuilder();

            if (progress > GB)
                text.Append($"{progress/GB:#,##0.0}GB");
            else if (progress > MB)
                text.Append($"{progress/MB:#,##0.0}MB");
            else if (progress > KB)
                text.Append($"{progress/KB:#,##0}KB");
            else
                text.Append($"{progress:#,##0}B");

            if (TotalDownload > GB)
                text.Append($" / {TotalDownload/GB:#,##0.0}GB");
            else if (TotalDownload > MB)
                text.Append($" / {TotalDownload/MB:#,##0.0}MB");
            else if (TotalDownload > KB)
                text.Append($" / {TotalDownload/KB:#,##0}KB");
            else
                text.Append($" / {TotalDownload:#,##0}B");

            DownloadSizeLabel.Text = text.ToString();

            if (TotalDownload > 0)
                TotalProgressBar.EditValue = Math.Max(0, Math.Min(100, (int) (progress*100/TotalDownload)));

            long speed = (progress - LastDownloadProcess)*TimeSpan.TicksPerSecond/(Time.Now.Ticks - LastSpeedCheck.Ticks); //May cause errors?
            LastDownloadProcess = progress;

            if (speed > GB)
                DownloadSpeedLabel.Text = $"{speed/GB:#,##0.0}GBps";
            else if (speed > MB)
                DownloadSpeedLabel.Text = $"{speed/MB:#,##0.0}MBps";
            else if (speed > KB)
                DownloadSpeedLabel.Text = $"{speed/KB:#,##0}KBps";
            else
                DownloadSpeedLabel.Text = $"{speed:#,##0}Bps";

            LastSpeedCheck = Time.Now;
        }

        private List<PatchInformation> LoadVersion(IProgress<string> progress)
        {
            List<PatchInformation> list = new List<PatchInformation>();
            try
            {
                if (File.Exists(ClientPath + "Version.bin"))
                {
                    using (MemoryStream mStream = new MemoryStream(File.ReadAllBytes(ClientPath + "Version.bin")))
                    using (BinaryReader reader = new BinaryReader(mStream))
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                            list.Add(new PatchInformation(reader));

                    progress.Report("Calculating Patch.");
                    return list;
                }


                progress.Report("Version Info is missing, Running Repairing");

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
        private List<PatchInformation> CalculatePatch(IReadOnlyList<PatchInformation> list, List<PatchInformation> current, IProgress<string> progress)
        {
            List<PatchInformation> patch = new List<PatchInformation>();

            if (list == null) return patch;

            for (int i = 0; i < list.Count; i++)
            {
                progress.Report($"Files Checked: {i + 1} of {list.Count}");

                PatchInformation file = list[i];
                if (current != null && current.Any(x => x.FileName == file.FileName && IsMatch(x.CheckSum, file.CheckSum))) continue;

                if (File.Exists(ClientPath + file.FileName))
                {
                    byte[] CheckSum;
                    using (MD5 md5 = MD5.Create())
                    {
                        using (FileStream stream = File.OpenRead(ClientPath + file.FileName))
                            CheckSum = md5.ComputeHash(stream);
                    }

                    if (IsMatch(CheckSum, file.CheckSum))
                        continue;
                }

                patch.Add(file);
                TotalDownload += file.CompressedLength;
            }

            return patch;
        }
        public bool IsMatch(byte[] a, byte[] b, long offSet = 0)
        {
            if (b == null || a == null || b.Length + offSet > a.Length || offSet < 0) return false;

            for (int i = 0; i < b.Length; i++)
                if (a[offSet + i] != b[i])
                    return false;

            return true;
        }

        private void SaveVersion(List<PatchInformation> version)
        {
            using (FileStream fStream = File.Create(ClientPath + "Version.bin"))
            using (BinaryWriter writer = new BinaryWriter(fStream))
            {
                foreach (PatchInformation info in version)
                    info.Save(writer);
            }

        }
        private void DownloadPatch(List<PatchInformation> patch, IProgress<string> progress)
        {
            List<Task> tasks = new List<Task>();

            foreach (PatchInformation file in patch)
            {
                if (!Download(file)) continue;

                tasks.Add(Task.Run(() => Extract(file)));
            }

            if (tasks.Count == 0) return;

            progress.Report("Downloaded, Extracting.");

            Task.WaitAll(tasks.ToArray());
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(ClientPath + ClientFileName);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool Download(PatchInformation file)
        {
            string webFileName = file.FileName.Replace("\\", "-") + ".gz";

            try
            {
                using (WebClient client = new WebClient())
                {
                    if (Config.UseLogin) client.Credentials = new NetworkCredential(Config.Username, Config.Password);

                    bool downloading = true;
                    client.DownloadProgressChanged += (o, e) => CurrentProgress = e.BytesReceived;
                    client.DownloadFileCompleted += (o, e) => downloading = false;

                    if (!Directory.Exists(ClientPath + "Patch\\"))
                        Directory.CreateDirectory(ClientPath + "Patch\\");

                     client.DownloadFileAsync(new Uri(Config.Host + webFileName), $"{ClientPath}Patch\\{webFileName}");

                    while (downloading)
                        Thread.Sleep(1);

                    CurrentProgress = 0;
                    TotalProgress += file.CompressedLength;
                }

                return true;
            }
            catch (Exception)
            {
                file.CheckSum = new byte[8];
            }

            return false;
        }
        private void Extract(PatchInformation file)
        {
            string webFileName = file.FileName.Replace("\\", "-") + ".gz";

            try
            {
                string toPath = ClientPath + file.FileName;

                if (Application.ExecutablePath.EndsWith(file.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    toPath += ".tmp";
                    NeedUpdate = true;
                }

               
                 if (File.Exists(toPath)) File.Delete(toPath);

                Decompress($"{ClientPath}Patch\\{webFileName}", toPath);

            }
            catch (UnauthorizedAccessException ex)
            {
                file.CheckSum = new byte[8];

                if (HasError) return;
                HasError = true;
                MessageBox.Show(ex.Message + "\n\nFile might be in use, please make sure the game is closed.", "File Error", MessageBoxButtons.OK);
            }
            catch
            {
                file.CheckSum = new byte[8];
            }
        }
        private static void Decompress(string sourceFile, string destFile)
        {
            if (!Directory.Exists(Path.GetDirectoryName(destFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));

            using (FileStream tofile = File.Create(destFile))
            using (FileStream fromfile = File.OpenRead(sourceFile))
            using (GZipStream gStream = new GZipStream(fromfile, CompressionMode.Decompress))
            {
                gStream.CopyTo(tofile);
            }
        }
    }
}
