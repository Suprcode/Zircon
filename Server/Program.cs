using System;
using System.Runtime;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using Library;
using Server.Envir;
using Autofac;

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

            var container = BuildContainer();
            var form = container.Resolve<SMain>();

            Application.Run(form);

            ConfigReader.Save();
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SMain>().SingleInstance();

            return builder.Build();
        }
    }
}
