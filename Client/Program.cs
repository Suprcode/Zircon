using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Library;
using Sentry;
using SlimDX.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ConfigReader.Load(Assembly.GetAssembly(typeof(Config)));

            ApplyCommandLineOverrides(args);

            if (Config.SentryEnabled && !string.IsNullOrEmpty(Config.SentryDSN))
            {
                using (SentrySdk.Init(Config.SentryDSN))
                    Init();
            }
            else
            {
                Init();
            }

            ConfigReader.Save(typeof(Config).Assembly);
        }

        static void Init()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            foreach (KeyValuePair<LibraryFile, string> pair in Libraries.LibraryList)
            {
                if (!File.Exists(@".\" + pair.Value)) continue;

                CEnvir.LibraryList[pair.Key] = new MirLibrary(@".\" + pair.Value);
            }

            CEnvir.Target = new TargetForm();

            RenderManager.Initialize(CEnvir.Target);
            DXSoundManager.Create();

            DXControl.ActiveScene = new LoginScene(Config.ExtendedLogin ? Config.GameSize : Config.IntroSceneSize);

            MessagePump.Run(CEnvir.Target, CEnvir.GameLoop);

            CEnvir.Session?.Save(true);
            CEnvir.Unload();
            RenderManager.Shutdown();
            DXSoundManager.Unload();
        }

        private static void ApplyCommandLineOverrides(string[] args)
        {
            if (args == null || args.Length == 0) return;

            foreach (string argument in args)
            {
                if (string.IsNullOrWhiteSpace(argument)) continue;

                string value = argument.Trim();

                if (value.StartsWith("--renderer=", StringComparison.OrdinalIgnoreCase))
                {
                    string renderer = value.Substring("--renderer=".Length);

                    if (string.Equals(renderer, "dx11", StringComparison.OrdinalIgnoreCase))
                        Config.UseDirectX11 = true;
                    else if (string.Equals(renderer, "dx9", StringComparison.OrdinalIgnoreCase))
                        Config.UseDirectX11 = false;

                    continue;
                }

                switch (value.ToLowerInvariant())
                {
                    case "--dx11":
                    case "/dx11":
                        Config.UseDirectX11 = true;
                        break;
                    case "--dx9":
                    case "/dx9":
                        Config.UseDirectX11 = false;
                        break;
                }
            }
        }
    }
}
