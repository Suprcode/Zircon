using System;
using System.Drawing;
using System.Windows.Forms;

namespace Shared.Rendering
{
    public sealed class RenderingPipelineContext
    {
        public RenderingPipelineContext(Control renderTarget, RenderingHostSettings settings = null)
        {
            RenderTarget = renderTarget ?? throw new ArgumentNullException(nameof(renderTarget));
            Settings = settings ?? new RenderingHostSettings();
        }

        public Control RenderTarget { get; }
        public RenderingHostSettings Settings { get; }
    }

    public sealed class RenderingHostSettings
    {
        public Func<DateTime> Now { get; set; } = () => DateTime.Now;
        public Action<Exception> SaveException { get; set; }
        public Action InvalidateRenderCaches { get; set; }
        public Action<bool> FullScreenChanged { get; set; }
        public Func<Size> GetActiveSceneSize { get; set; }
        public Func<string> GetDefaultMonitor { get; set; }
        public Action<string> SetDefaultMonitor { get; set; }
        public Func<string> GetRenderingPipeline { get; set; }
        public Action<string> SetRenderingPipeline { get; set; }
        public Func<Size> GetGameSize { get; set; }
        public Action<Size> SetGameSize { get; set; }
        public Func<bool> GetFullScreen { get; set; }
        public Action<bool> SetFullScreen { get; set; }
        public Func<bool> GetBorderless { get; set; }
        public Action<bool> SetBorderless { get; set; }
        public Func<bool> GetVSync { get; set; }
        public Action<bool> SetVSync { get; set; }
        public Func<bool> GetUseD3D11SpriteBatch { get; set; }
        public Action<bool> SetUseD3D11SpriteBatch { get; set; }

        private string _defaultMonitor = string.Empty;
        private string _renderingPipeline = RenderingPipelineIds.SilkDXD3D11;
        private Size _gameSize = new Size(1024, 768);
        private bool _fullScreen;
        private bool _borderless;
        private bool _vSync;
        private bool _useD3D11SpriteBatch;

        public string DefaultMonitor
        {
            get => GetDefaultMonitor?.Invoke() ?? _defaultMonitor;
            set
            {
                if (SetDefaultMonitor != null)
                    SetDefaultMonitor(value);
                else
                    _defaultMonitor = value;
            }
        }

        public string RenderingPipeline
        {
            get => GetRenderingPipeline?.Invoke() ?? _renderingPipeline;
            set
            {
                if (SetRenderingPipeline != null)
                    SetRenderingPipeline(value);
                else
                    _renderingPipeline = value;
            }
        }

        public Size GameSize
        {
            get => GetGameSize?.Invoke() ?? _gameSize;
            set
            {
                if (SetGameSize != null)
                    SetGameSize(value);
                else
                    _gameSize = value;
            }
        }

        public bool FullScreen
        {
            get => GetFullScreen?.Invoke() ?? _fullScreen;
            set
            {
                if (SetFullScreen != null)
                    SetFullScreen(value);
                else
                    _fullScreen = value;
            }
        }

        public bool Borderless
        {
            get => GetBorderless?.Invoke() ?? _borderless;
            set
            {
                if (SetBorderless != null)
                    SetBorderless(value);
                else
                    _borderless = value;
            }
        }

        public bool VSync
        {
            get => GetVSync?.Invoke() ?? _vSync;
            set
            {
                if (SetVSync != null)
                    SetVSync(value);
                else
                    _vSync = value;
            }
        }

        public bool UseD3D11SpriteBatch
        {
            get => GetUseD3D11SpriteBatch?.Invoke() ?? _useD3D11SpriteBatch;
            set
            {
                if (SetUseD3D11SpriteBatch != null)
                    SetUseD3D11SpriteBatch(value);
                else
                    _useD3D11SpriteBatch = value;
            }
        }

        public DateTime CurrentTime => Now?.Invoke() ?? DateTime.Now;
        public Size ActiveSceneSize => GetActiveSceneSize?.Invoke() ?? GameSize;

        public void NotifyFullScreenChanged()
        {
            FullScreenChanged?.Invoke(FullScreen);
        }

        public void ReportException(Exception exception)
        {
            if (exception == null)
                return;

            if (SaveException != null)
                SaveException(exception);
            else
                Console.WriteLine(exception);
        }
    }
}
