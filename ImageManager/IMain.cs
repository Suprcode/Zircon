using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ImageManager
{
    public partial class IMain : XtraForm
    {

        public int TotleCount;
        public int ProgressCount;

        public IMain()
        {
            InitializeComponent();
            /*
            string path = @"C:\Legend of Mir 3 Old\Clean Client\SoundList.wwl";

            byte[] text = File.ReadAllBytes(path);

            string test = Encoding.ASCII.GetString(text, 234, 16);

            int start = 234;

            Dictionary<int, string> files = new Dictionary<int, string>();
            while (start <= text.Length - 16)
            {
                int count = BitConverter.ToUInt16(text, start);

                if (count > 254)
                {
                    

                }

                string file = Encoding.ASCII.GetString(text, start + 2, 14);
                file = file.Substring(0, file.IndexOf('\0'));

                files[count] = file;

                start += 16;
            }

            List<string> lines = new List<string>();
            foreach (KeyValuePair<int, string> pair in files)
            {
                lines.Add($"{pair.Key} = {pair.Value}");
                
            }
            File.WriteAllLines(@"C:\Legend of Mir 3 Old\Clean Client\SoundList2.txt", lines);
            */
        }

        private async void ConvertLibrariesButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo((string) SelectedFolderButtonEdit.EditValue);

            if (!directory.Exists) return;

            FileInfo[] targets = directory.GetFiles("*.WTL", SubFoldersCheckEdit.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            TotleCount = targets.Length;
            ProgressCount = 0;

            if (targets.Length == 0) return;

            Task task = Task.Run(() =>
            {
                ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                Parallel.For(0, targets.Length, po, i =>
                {
                    if (!targets[i].FullName.EndsWith("_S.wtl") && !targets[i].FullName.StartsWith("MonS-"))
                    {
                        using (WTLLibrary wtl = new WTLLibrary(targets[i].FullName))
                        {
                            Mir3Library library = wtl.Convert();

                            library.Save(Path.ChangeExtension(targets[i].FullName, @".Zl"));
                        }
                    }
                    Interlocked.Increment(ref ProgressCount);
                });
            });

            while (!task.IsCompleted)
            {
                UpdateProgress();

                await Task.Delay(100);
            }

            ProgressLabel.Text = "Compeleted.";
        }


        public void UpdateProgress()
        {
            ProgressLabel.Text = $"{ProgressCount} of {TotleCount}.";
        }

        private void SelectedFolderButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (FolderDialog.ShowDialog() == DialogResult.OK)
                SelectedFolderButtonEdit.EditValue = FolderDialog.SelectedPath;
        }

        private async void CreaetLibrariesButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo((string)SelectedFolderButtonEdit.EditValue);

            if (!directory.Exists) return;

            DirectoryInfo[] targets = directory.GetDirectories("*.*", SubFoldersCheckEdit.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (targets.Length == 0)
                targets = new[] { directory };

            TotleCount = targets.Length;
            ProgressCount = 0;

            if (targets.Length == 0) return;

            Task task = Task.Run(() =>
            {
                ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 15 };

                Parallel.For(0, targets.Length, po, i =>
                {
                    if (!File.Exists(targets[i].FullName + "\\Placements.txt")) return;

                    string[] placements = File.ReadAllLines(targets[i].FullName + "\\Placements.txt");

                    Mir3Library lib = new Mir3Library { Images = new Mir3Image[placements.Length] };

                    ParallelOptions po1 = new ParallelOptions { MaxDegreeOfParallelism = 15 };
                    Parallel.For(0, placements.Length, po1, index =>
                    {
                        string fileName = string.Format(targets[i].FullName + "\\{0:00000}.bmp", index);

                        if (!File.Exists(fileName)) return;

                        string[] placement = placements[index].Split(',');

                        short x = short.Parse(placement[0]);
                        short y = short.Parse(placement[1]);


                        using (Bitmap image = new Bitmap(fileName))
                        {
                            lib.Images[index] = new Mir3Image
                            {
                                Width = (short) image.Width,
                                Height = (short) image.Height,
                                OffSetX = x,
                                OffSetY = y,
                                Data = MImage.GetBytes(image)
                            };
                        }
                    });

                    lib.Save(targets[i].FullName + ".Zl");

                    Interlocked.Increment(ref ProgressCount);

                });
            });

            while (!task.IsCompleted)
            {
                UpdateProgress();

                await Task.Delay(100);
            }

            ProgressLabel.Text = "Compeleted.";
        }

        //WTLLibrary lib = new WTLLibrary(@"C:\Zircon Server\Data Works\Test\WM-Hum.wtl");
        private void simpleButton1_Click(object sender, EventArgs e)
        {
          /*  DirectoryInfo dInfo = new DirectoryInfo(@"C:\Zircon Server\Data Works\Inventory");
            FileInfo[] files = dInfo.GetFiles("*.BMP");

            foreach (FileInfo file in files)
            {
                Bitmap image = new Bitmap(file.FullName);
                
                image.Save(Path.ChangeExtension(file.FullName, @".png"), ImageFormat.Png);

                image.Dispose();
            }*/

            DirectoryInfo dInfo = new DirectoryInfo(@"C:\Zircon Server\Data Works\Game\MiniMap");
            FileInfo[] files = dInfo.GetFiles("*.BMP");

            foreach (FileInfo file in files)
            {
                Bitmap image = new Bitmap(file.FullName);

                if (image.Width <= 800 && image.Height <= 600)
                {
                    image.Dispose();
                    continue;
                }

                Bitmap nImage = new Bitmap(image, new Size(Math.Min(image.Width, 800), Math.Min(image.Height, 600)));

                image.Dispose();
                nImage.Save(file.FullName, ImageFormat.Bmp);

                nImage.Dispose();
            }
        }

        private void IMain_Load(object sender, EventArgs e)
        {

        }
    }
}
 