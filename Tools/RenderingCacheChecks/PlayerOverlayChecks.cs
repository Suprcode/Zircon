using System.Runtime.InteropServices;
using Shared.Rendering;
using R = Shared.Rendering.RenderingPipelineManager;

internal static class PlayerOverlayChecks
{
    public static int Run(bool vulkan, string mapLibraryPath = null)
    {
        using Form form = new() { ShowInTaskbar = false, ClientSize = new Size(128, 128) };
        Exception renderError = null;
        var settings = new RenderingHostSettings
        {
            GameSize = form.ClientSize, GetActiveSceneSize = () => form.ClientSize,
            VSync = false, SaveException = ex => renderError = ex
        };
        using var session = R.CreateSession(vulkan ? RenderingPipelineIds.SilkVulkan : RenderingPipelineIds.SilkDXD3D11,
            new RenderingPipelineContext(form, settings));
        using MirLibrary mapLibrary = mapLibraryPath == null ? null : new MirLibrary(mapLibraryPath);
        mapLibrary?.ReadLibrary();
        // Use the real map-library draw path when content is supplied, including texture uploads.
        int[] mapFrames = mapLibrary == null ? Array.Empty<int>() : Enumerable.Range(0, mapLibrary.Images.Length)
            .Where(i => mapLibrary.Images[i] != null && mapLibrary.Images[i].Width > 0 && mapLibrary.Images[i].Height > 0)
            .Take(8).ToArray();
        void DrawMapImages()
        {
            foreach (int index in mapFrames)
                mapLibrary.Draw(index, 0, 85, Color.White, false, 1F, Shared.Envir.ImageType.Image);
        }

        for (int frame = 0; frame < 3; frame++)
        {
            if (!R.RenderFrame(() =>
            {
                DrawAndCheck(vulkan, DrawMapImages);
                CheckShadowExclusion(vulkan);
                CheckStateRestoration();
            }))
                throw new InvalidOperationException("Player overlay render failed", renderError);
        }
        Console.WriteLine(vulkan
            ? "Player overlay: Vulkan composition / mask / pool reuse smoke passed (no GPU readback)."
            : "Player overlay: DirectX pixel checks passed (rear-layer occlusion, screen glow/black backing, dark roofs, shadow exclusion, foreground coverage, world alpha, pool reuse).");
        if (mapLibrary != null)
        {
            if (mapFrames.Length == 0) throw new Exception("Map library contains no usable images.");
            Console.WriteLine($"Real map-library path checked: {mapFrames.Length} images from {mapLibraryPath}.");
        }
        return 0;
    }

    private static void DrawAndCheck(bool vulkan, Action drawMapImages)
    {
        RenderSurface world = R.GetCurrentSurface();
        Rectangle bounds = new(Point.Empty, new Size(128, 128));
        try
        {
            R.Clear(RenderClearFlags.Target, Color.Black, 0, 0);
            DrawPlayer();
            DrawForeground();
            drawMapImages();
            byte[] before = vulkan ? null : ReadPixels();

            // Exercise the same compositor used by MapControl, including screen-blended wings.
            R.DrawOccludedComposite(bounds.Size, DrawPlayer, () =>
            {
                DrawForeground();
                drawMapImages();
            }, .65F);

            if (vulkan) return;
            byte[] after = ReadPixels();
            // The rear red layer must stay hidden by the green body in both passes.
            Expect(after, 30, 35, 0, 255, 0);
            Expect(after, 60, 35, 89, 255, 89);
            // Half-alpha foreground must retain half-strength overlay coverage, not alpha squared.
            Expect(after, 75, 35, 86, 255, 86);
            Expect(after, 90, 35, 98, 255, 98);
            // Screen-blended red and black wing pixels must not darken a white roof.
            Expect(after, 60, 15, 255, 255, 255);
            Expect(after, 60, 25, 105, 105, 105);
            // A coloured glow brightens a dark roof; its black backing leaves shadow intact.
            Expect(after, 60, 12, 203, 105, 105);
            Expect(after, 75, 65, 86, 170, 171);
            // A foreground hole, an uncovered rear effect and the empty background must be unchanged.
            foreach (Point p in new[] { new Point(30, 35), new Point(25, 15), new Point(5, 5), new Point(60, 100) })
            {
                int offset = (p.Y * 128 + p.X) * 4;
                for (int channel = 0; channel < 4; channel++)
                    if (before[offset + channel] != after[offset + channel])
                        throw new Exception($"Overlay changed uncovered pixel {p} channel {channel}.");
            }
            // Check the entire background, including both dark and light covered pixels.
            // A mask must never introduce RGB where the player target has no coverage.
            Rectangle silhouette = new(20, 10, 80, 70);
            for (int y = 0; y < 128; y++)
                for (int x = 0; x < 128; x++)
                {
                    int offset = (y * 128 + x) * 4;
                    if (before[offset + 3] != after[offset + 3])
                        throw new Exception($"Overlay changed world alpha ({x},{y}).");
                    if (silhouette.Contains(x, y)) continue;
                    for (int channel = 0; channel < 4; channel++)
                        if (before[offset + channel] != after[offset + channel])
                            throw new Exception($"Overlay changed background pixel ({x},{y}) channel {channel}.");
                }
        }
        finally
        {
            R.SetSurface(world);
            R.FlushSprite();
            R.SetOpacity(1F);
            R.SetBlend(false);
        }
    }

    private static void DrawPlayer()
    {
        // Real wings use a screen blend, including opaque-alpha black backing pixels.
        R.SetBlend(true, 1F, BlendMode.NORMAL);
        R.FillRectangle(new Rectangle(20, 10, 80, 70), Color.Black);
        R.FillRectangle(new Rectangle(20, 10, 80, 10), Color.Red);
        R.SetBlend(false);
        R.FillRectangle(new Rectangle(20, 30, 80, 50), Color.Lime);
        R.FillRectangle(new Rectangle(20, 60, 80, 20), Color.FromArgb(128, Color.Blue));
    }

    private static void DrawForeground()
    {
        // Three strips exercise no cover, opaque cover, partial cover and overlapping cover.
        R.FillRectangle(new Rectangle(50, 0, 20, 128), Color.White);
        R.FillRectangle(new Rectangle(70, 0, 30, 128), Color.FromArgb(128, Color.White));
        R.FillRectangle(new Rectangle(85, 0, 15, 128), Color.FromArgb(128, Color.White));
        // Dark foreground overlaps both coloured wing pixels and black effect backing.
        R.FillRectangle(new Rectangle(50, 10, 20, 4), Color.DimGray);
        R.FillRectangle(new Rectangle(50, 20, 20, 10), Color.DimGray);
        // A dark building outside the player silhouette must never receive overlay colour.
        R.FillRectangle(new Rectangle(10, 95, 110, 20), Color.DimGray);
    }

    private static byte[] ReadPixels()
    {
        R.FlushSprite();
        object surface = R.GetCurrentSurface().NativeHandle;
        object texture = surface.GetType().GetProperty("Texture").GetValue(surface);
        using TextureLock data = R.LockTexture(RenderTexture.From(texture), TextureLockMode.ReadOnly);
        byte[] result = new byte[128 * 128 * 4];
        for (int y = 0; y < 128; y++)
            Marshal.Copy(data.DataPointer + y * data.Pitch, result, y * 128 * 4, 128 * 4);
        return result;
    }

    private static void CheckShadowExclusion(bool vulkan)
    {
        // Match the map's checkerboard shadow encoding, including opaque black pixels.
        RenderTexture shadow = R.CreateTexture(new Size(16, 16), RenderTextureFormat.A8R8G8B8,
            RenderTextureUsage.None, RenderTexturePool.Managed);
        try
        {
            using (TextureLock data = R.LockTexture(shadow, TextureLockMode.Discard))
            {
                byte[] row = new byte[16 * 4];
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++) row[x * 4 + 3] = (byte)((x + y) % 2 == 0 ? 255 : 0);
                    Marshal.Copy(row, 0, data.DataPointer + y * data.Pitch, row.Length);
                }
            }
            void DrawShadow(float opacity)
            {
                R.EnableSolidShadowFillEffect(opacity);
                R.DrawTexture(shadow, new Rectangle(0, 0, 16, 16), new RectangleF(0, 0, 128, 128), Color.White);
                R.DisableSpriteShaderEffect();
            }
            R.Clear(RenderClearFlags.Target, Color.DimGray, 0, 0);
            DrawPlayer();
            DrawShadow(.5F);
            byte[] before = vulkan ? null : ReadPixels();
            // The production mask uses zero shadow opacity, while buildings keep their alpha.
            R.DrawOccludedComposite(new Size(128, 128), DrawPlayer, () => DrawShadow(0F), .65F);
            if (!vulkan && !before.SequenceEqual(ReadPixels()))
                throw new Exception("Shadow-only coverage changed the normal player or background.");
        }
        finally
        {
            R.FlushSprite();
            R.ReleaseTexture(shadow);
        }
    }

    private static void Expect(byte[] pixels, int x, int y, int red, int green, int blue)
    {
        int offset = (y * 128 + x) * 4;
        int[] expected = { blue, green, red };
        for (int channel = 0; channel < 3; channel++)
            if (Math.Abs(pixels[offset + channel] - expected[channel]) > 3)
                throw new Exception($"Pixel ({x},{y}) channel {channel}: expected {expected[channel]}, got {pixels[offset + channel]}.");
    }

    private static void CheckStateRestoration()
    {
        RenderSurface world = R.GetCurrentSurface();
        try
        {
            R.SetOpacity(.8F);
            R.SetBlend(true, .3F, BlendMode.HIGHLIGHT);
            int captures = 0;
            R.DrawOccludedComposite(new Size(128, 128), () =>
            {
                // Both captures must start identically even if the callback changes state.
                if (R.GetOpacity() != 1F || R.IsBlending())
                    throw new Exception("Actor capture inherited callback state.");
                captures++;
                R.FillRectangle(new Rectangle(20, 30, 80, 50), Color.Lime);
                R.SetOpacity(.2F);
                R.SetBlend(true, .5F, BlendMode.NORMAL);
            }, () => R.FillRectangle(new Rectangle(50, 0, 20, 128), Color.White), .65F);
            if (captures != 2) throw new Exception("Actor was not captured twice.");
            AssertRestored();

            // Errors in either callback must restore the target and caller's blend/opacity.
            foreach (bool failInMask in new[] { false, true })
            {
                bool caught = false;
                try
                {
                    R.DrawOccludedComposite(new Size(128, 128),
                        () => { if (!failInMask) throw new ApplicationException("actor"); },
                        () => throw new ApplicationException("mask"), .65F);
                }
                catch (ApplicationException) { caught = true; }
                if (!caught) throw new Exception("Expected callback failure was not propagated.");
                AssertRestored();
            }

            void AssertRestored()
            {
                if (!ReferenceEquals(world.NativeHandle, R.GetCurrentSurface().NativeHandle) ||
                    R.GetOpacity() != .8F || !R.IsBlending() ||
                    R.GetBlendRate() != .3F || R.GetBlendMode() != BlendMode.HIGHLIGHT)
                    throw new Exception("Compositor did not restore the caller's render state.");
            }
        }
        finally
        {
            R.SetSurface(world);
            R.FlushSprite();
            R.SetOpacity(1F);
            R.SetBlend(false);
        }
    }
}
