using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Client.Scenes.Views;
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
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
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

            DXControl.ActiveScene = new LoginScene(Config.ExtendedLogin ? Config.GameSize : Config.IntroSceneSize);

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

                    DXConfigWindow.ActiveConfig?.UpdateScaleControlState();
                },
                GetActiveSceneSize = () => DXControl.ActiveScene?.Size ?? Config.GameSize,
                GetMonitorScale = TargetForm.GetMonitorScale,
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

        internal static void InvalidateRenderCaches()
        {
            InvalidateControlCaches(false);

            foreach (MirLibrary library in CEnvir.LibraryList.Values)
                library?.DisposeTextures();
        }

        internal static void InvalidateUiRenderCaches()
        {
            InvalidateControlCaches(true);
        }

        private static void InvalidateControlCaches(bool skipWorld)
        {
            HashSet<DXControl> visited = new();
            InvalidateControlTree(DXControl.ActiveScene, visited, skipWorld);

            foreach (DXControl messageBox in DXControl.MessageBoxList.ToArray())
                InvalidateControlTree(messageBox, visited, skipWorld);

            InvalidateControlTree(DXControl.MouseControl, visited, skipWorld);
            InvalidateControlTree(DXControl.FocusControl, visited, skipWorld);
            InvalidateControlTree(DXControl.DebugLabel, visited, skipWorld);
            InvalidateControlTree(DXControl.HintLabel, visited, skipWorld);
            InvalidateControlTree(DXControl.PingLabel, visited, skipWorld);
        }

        private static void InvalidateControlTree(DXControl control, HashSet<DXControl> visited, bool skipWorld)
        {
            if (control == null || !visited.Add(control))
                return;

            if (skipWorld && control is MapControl)
                return;

            control.DisposeTexture();

            foreach (DXControl child in control.Controls.ToArray())
                InvalidateControlTree(child, visited, skipWorld);
        }
    }
}
