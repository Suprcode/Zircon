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
using System.Net.Http;
using System.Net.Http.Headers;
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

            List<PatchInformation> liveVersion = await GetPatchInformation(progress);

            if (liveVersion == null)
            {
                DownloadSizeLabel.Text = "Downloading failed.";
                RepairButton.Enabled = true;
                StartGameButton.Enabled = true;
                return;
            }

            List<PatchInformation> currentVersion = repair ? null : await LoadVersion(progress);
            List<PatchInformation> patch = await CalculatePatch(liveVersion, currentVersion, progress);

            StatusLabel.Text = "Downloading";
            CreateSizeLabel();

            Task task = DownloadPatch(patch, progress, new Progress<int>(percent =>
            {
                // Update progress bar or label with the percentage downloaded
                CreateSizeLabel();
            }));

            await task;

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

        private async Task<List<PatchInformation>> LoadVersion(IProgress<string> progress)
        {
            List<PatchInformation> list = new List<PatchInformation>();

            try
            {
                if (File.Exists(ClientPath + "Version.bin"))
                {
                    using (MemoryStream mStream = new MemoryStream(await File.ReadAllBytesAsync(ClientPath + "Version.bin")))
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
        private async Task<List<PatchInformation>> GetPatchInformation(IProgress<string> progress)
        {
            try
            {
                progress.Report("Downloading Patch Information");

                using (HttpClient client = new HttpClient())
                {
                    if (Config.UseLogin)
                    {
                        var byteArray = Encoding.ASCII.GetBytes($"{Config.Username}:{Config.Password}");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }

                    using (HttpResponseMessage response = await client.GetAsync(Config.Host + PListFileName + "?nocache=" + Guid.NewGuid().ToString("N")))
                    {
                        response.EnsureSuccessStatusCode();

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        using (BinaryReader reader = new BinaryReader(contentStream))
                        {
                            List<PatchInformation> list = new List<PatchInformation>();

                            while (reader.BaseStream.Position < reader.BaseStream.Length)
                                list.Add(new PatchInformation(reader));

                            return list;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                progress.Report(ex.Message);
            }

            return null;
        }
        private async Task<List<PatchInformation>> CalculatePatch(IReadOnlyList<PatchInformation> list, List<PatchInformation> current, IProgress<string> progress)
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
                            CheckSum = await md5.ComputeHashAsync(stream);
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

        private async Task DownloadPatch(List<PatchInformation> patch, IProgress<string> progress, IProgress<int> downloadProgress)
        {
            List<Task> tasks = new List<Task>();

            foreach (PatchInformation file in patch)
            {
                if (!await Download(file, downloadProgress)) continue;

                tasks.Add(Extract(file));
            }

            if (tasks.Count == 0) return;

            progress.Report("Downloaded, Extracting.");

            await Task.WhenAll(tasks);
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

        private async Task<bool> Download(PatchInformation file, IProgress<int> progress)
        {
            string webFileName = file.FileName.Replace("\\", "-") + ".gz";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (Config.UseLogin)
                    {
                        var byteArray = Encoding.ASCII.GetBytes($"{Config.Username}:{Config.Password}");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }

                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    HttpResponseMessage response = await client.GetAsync(Config.Host + webFileName, HttpCompletionOption.ResponseHeadersRead);

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        response.EnsureSuccessStatusCode();

                        if (!Directory.Exists(ClientPath + "Patch\\"))
                            Directory.CreateDirectory(ClientPath + "Patch\\");

                        using (FileStream fileStream = new FileStream($"{ClientPath}Patch\\{webFileName}", FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            long totalBytes = response.Content.Headers.ContentLength ?? -1;
                            long totalDownloadedBytes = 0;
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalDownloadedBytes += bytesRead;
                                if (totalBytes > 0)
                                    progress.Report((int)(totalDownloadedBytes * 100 / totalBytes));
                            }
                        }
                    }

                    CurrentProgress = 0;
                    TotalProgress += file.CompressedLength;

                    return true;
                }
            }
            catch (Exception)
            {
                file.CheckSum = new byte[8];
            }

            return false;
        }

        private async Task Extract(PatchInformation file)
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

                await Decompress($"{ClientPath}Patch\\{webFileName}", toPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                file.CheckSum = new byte[8];

                if (HasError) return;
                HasError = true;
                MessageBox.Show(ex.Message + "\n\nFile might be in use, please make sure the game is closed.", "File Error", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                file.CheckSum = new byte[8];
            }
        }
        private static async Task Decompress(string sourceFile, string destFile)
        {
            if (!Directory.Exists(Path.GetDirectoryName(destFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));

            using (FileStream tofile = File.Create(destFile))
            using (FileStream fromfile = File.OpenRead(sourceFile))
            using (GZipStream gStream = new GZipStream(fromfile, CompressionMode.Decompress))
            {
                await gStream.CopyToAsync(tofile);
            }
        }
    }
}
