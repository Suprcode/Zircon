using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Library;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            if (Config.SentryEnabled && !string.IsNullOrEmpty(Config.SentryDSN))
            {
                using (SentrySdk.Init(Config.SentryDSN))
                    Init(args);
            }
            else
            {
                Init(args);
            }

            ConfigReader.Save(typeof(Config).Assembly);
        }

        static void Init(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            MirLibrary.GetNow = () => CEnvir.Now;
            MirLibrary.GetCacheDuration = () => Config.CacheDuration;
            MirLibrary.GetUseZlAtlasPages = () => Config.UseZlAtlasPages;
            MirLibrary.DrawCounted = () => CEnvir.DPSCounter++;

            foreach (KeyValuePair<LibraryFile, string> pair in Libraries.LibraryList)
            {
                if (!File.Exists(@".\" + pair.Value)) continue;

                CEnvir.LibraryList[pair.Key] = new MirLibrary(@".\" + pair.Value);
            }

            CEnvir.Init(args);

            CEnvir.Target = new TargetForm();
            string requestedPipelineId = RenderingPipelineManager.NormalizePipelineId(Config.RenderingPipeline);
            if (!string.Equals(Config.RenderingPipeline, requestedPipelineId, StringComparison.OrdinalIgnoreCase))
                Config.RenderingPipeline = requestedPipelineId;

            string activePipelineId = RenderingPipelineManager.InitializeWithFallback(requestedPipelineId, new RenderingPipelineContext(CEnvir.Target, CreateRenderingHostSettings()));
            if (!string.Equals(Config.RenderingPipeline, activePipelineId, StringComparison.OrdinalIgnoreCase))
                Config.RenderingPipeline = activePipelineId;
            DXSoundManager.Create();

            DXControl.ActiveScene = new LoginScene(Config.GameSize);

            RenderingPipelineManager.RunMessageLoop(CEnvir.Target, CEnvir.GameLoop);

            CEnvir.Session?.Save(true);
            CEnvir.Unload();
            RenderingPipelineManager.Shutdown();
            DXSoundManager.Unload();
        }

        private static RenderingHostSettings CreateRenderingHostSettings()
        {
            return new RenderingHostSettings
            {
                Now = () => CEnvir.Now,
                SaveException = CEnvir.SaveException,
                InvalidateRenderCaches = InvalidateRenderCaches,
                FullScreenChanged = fullScreen =>
                {
                    if (DXConfigWindow.ActiveConfig?.FullScreenCheckBox != null)
                        DXConfigWindow.ActiveConfig.FullScreenCheckBox.Checked = fullScreen;
                },
                GetActiveSceneSize = () => DXControl.ActiveScene?.Size ?? Config.GameSize,
                GetDefaultMonitor = () => Config.DefaultMonitor,
                SetDefaultMonitor = value => Config.DefaultMonitor = value,
                GetRenderingPipeline = () => Config.RenderingPipeline,
                SetRenderingPipeline = value => Config.RenderingPipeline = value,
                GetGameSize = () => Config.GameSize,
                SetGameSize = value => Config.GameSize = value,
                GetFullScreen = () => Config.FullScreen,
                SetFullScreen = value => Config.FullScreen = value,
                GetBorderless = () => Config.Borderless,
                SetBorderless = value => Config.Borderless = value,
                GetVSync = () => Config.VSync,
                SetVSync = value => Config.VSync = value,
                GetUseD3D11SpriteBatch = () => Config.UseD3D11SpriteBatch,
                SetUseD3D11SpriteBatch = value => Config.UseD3D11SpriteBatch = value,
            };
        }

        private static void InvalidateRenderCaches()
        {
            HashSet<DXControl> visited = new();
            InvalidateControlTree(DXControl.ActiveScene, visited);

            foreach (DXControl messageBox in DXControl.MessageBoxList.ToArray())
                InvalidateControlTree(messageBox, visited);

            InvalidateControlTree(DXControl.MouseControl, visited);
            InvalidateControlTree(DXControl.FocusControl, visited);

            foreach (MirLibrary library in CEnvir.LibraryList.Values)
                library?.DisposeTextures();
        }

        private static void InvalidateControlTree(DXControl control, HashSet<DXControl> visited)
        {
            if (control == null || !visited.Add(control))
                return;

            control.DisposeTexture();

            foreach (DXControl child in control.Controls.ToArray())
                InvalidateControlTree(child, visited);
        }
    }
}
