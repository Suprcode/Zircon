# World-name rendering check

Run `dotnet run --project Tools/LabelRenderingChecks` on Windows with DirectX 11 available.

Uses real client labels in a hidden 1024×768 render host at 100% scaling. Compares 2,000 overlapping and edge-clipped labels drawn every frame against a physical-pixel name layer refreshed every 16 frames. Checks identical pixels and reports warmed CPU/frame timings and texture rebuild counts. Timings illustrate cache reuse, not a prediction of in-game FPS; game invalidation frequency depends on movement and animation.

The executable is standalone and is not referenced by the client.
