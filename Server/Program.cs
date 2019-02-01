using System;
using System.Runtime;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using Library;
using Server.Envir;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.mmmmmmmmmmm
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigReader.Load();
            Config.LoadVersion();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            Application.Run(new SMain());

            ConfigReader.Save();
        }
    }
}
