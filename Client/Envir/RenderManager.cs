using System;
using System.Drawing;
using Client;
using SlimDX.Direct3D9;

namespace Client.Envir
{
    public enum RendererBackend
    {
        DirectX9,
        DirectX11
    }

    public static class RenderManager
    {
        public static RendererBackend ActiveBackend { get; private set; } = RendererBackend.DirectX9;
        public static RendererBackend RequestedBackend { get; private set; } = RendererBackend.DirectX9;

        public static bool UsingDirectX11 => ActiveBackend == RendererBackend.DirectX11;

        public static void Initialize(TargetForm target)
        {
            RequestedBackend = Config.UseDirectX11 ? RendererBackend.DirectX11 : RendererBackend.DirectX9;

            if (RequestedBackend == RendererBackend.DirectX11)
            {
                if (DX11Manager.TryCreate(target))
                {
                    ActiveBackend = RendererBackend.DirectX11;
                }
                else
                {
                    Console.WriteLine("[Render] DirectX 11 requested but unavailable, falling back to DirectX 9 renderer.");
                    ActiveBackend = RendererBackend.DirectX9;
                    DXManager.Create();
                }
            }
            else
            {
                ActiveBackend = RendererBackend.DirectX9;
                DXManager.Create();
            }

            Console.WriteLine($"[Render] Active renderer: {ActiveBackend}");
        }

        public static void Shutdown()
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.Unload();
            else
                DXManager.Unload();
        }

        public static void MemoryClear()
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.MemoryClear();
            else
                DXManager.MemoryClear();
        }

        public static bool DeviceLost
        {
            get
            {
                return ActiveBackend == RendererBackend.DirectX11
                    ? DX11Manager.DeviceLost
                    : DXManager.DeviceLost;
            }
        }

        public static void AttemptReset()
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.AttemptReset();
            else
                DXManager.AttemptReset();
        }

        public static void AttemptRecovery()
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.AttemptRecovery();
            else
                DXManager.AttemptRecovery();
        }

        public static void Render(Action drawScene)
        {
            if (ActiveBackend == RendererBackend.DirectX11)
            {
                DX11Manager.Render(drawScene);
                return;
            }

            DXManager.Device.Clear(ClearFlags.Target, Color.Black, 1, 0);
            DXManager.Device.BeginScene();
            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);

            drawScene?.Invoke();

            DXManager.Sprite.End();
            DXManager.Device.EndScene();
            DXManager.Device.Present();
        }

        public static void MarkDeviceLost()
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.DeviceLost = true;
            else
                DXManager.DeviceLost = true;
        }

        public static void ToggleFullScreen()
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.ToggleFullScreen();
            else
                DXManager.ToggleFullScreen();
        }

        public static void SetResolution(Size size)
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.SetResolution(size);
            else
                DXManager.SetResolution(size);
        }

        public static void ResetDevice()
        {
            if (ActiveBackend == RendererBackend.DirectX11)
                DX11Manager.ResetDevice();
            else
                DXManager.ResetDevice();
        }
    }
}
