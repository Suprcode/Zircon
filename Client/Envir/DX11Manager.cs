using System;
using System.Drawing;
using Client;

namespace Client.Envir
{
    public static class DX11Manager
    {
        public static bool DeviceLost { get; set; }

        public static bool TryCreate(TargetForm target)
        {
            Console.WriteLine("[Render] DirectX 11 manager initialization is not yet implemented.");
            DeviceLost = false;
            return false;
        }

        public static void Unload()
        {
        }

        public static void MemoryClear()
        {
        }

        public static void AttemptReset()
        {
        }

        public static void AttemptRecovery()
        {
        }

        public static void Render(Action drawScene)
        {
            throw new NotSupportedException("DirectX 11 rendering pipeline has not been implemented.");
        }

        public static void ToggleFullScreen()
        {
        }

        public static void SetResolution(Size size)
        {
        }

        public static void ResetDevice()
        {
        }
    }
}
