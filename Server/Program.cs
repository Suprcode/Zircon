﻿using System;
using System.Runtime;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using Library;
using Server.Envir;
using System.Reflection;

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
            var assembly = Assembly.GetAssembly(typeof(Config));
            ConfigReader.Load(assembly);

            Config.LoadVersion();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            Application.Run(new SMain());

            ConfigReader.Save(typeof(Config).Assembly);
        }

    }
}
