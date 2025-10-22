# Migrating from SharpDX to Vortice.Windows

## Why migrate
SharpDX 4.2.0 only distributes .NET Framework assemblies and has been archived since 2019, so projects targeting modern .NET versions consume it through compatibility shims and surface NU1701 warnings. 【F:Client/Client.csproj†L40-L47】 In contrast, [Vortice.Windows](https://www.nuget.org/packages/Vortice.Windows/) is actively maintained and publishes libraries built for current .NET runtimes.

## Package-level changes
1. Remove the `SharpDX`, `SharpDX.Desktop`, `SharpDX.Direct3D9`, `SharpDX.DirectSound`, and `SharpDX.Mathematics` package references from `Client.csproj`.
2. Add the NuGet packages:
   - `Vortice.Windows` (brings in Direct3D9, DirectSound, DXGI, and common math types)
   - `Vortice.Multimedia` (Wave formats, XAudio helpers)
   - `Vortice.D3DCompiler` if shader compilation is required at runtime.
3. Remove any binding redirects or probing paths related to the old assemblies.

## Namespace and type updates
Vortice namespaces differ from SharpDX; most Direct3D9 classes live under `Vortice.Direct3D9`, audio in `Vortice.DirectSound`, math helpers in `Vortice.Mathematics`, and Win32 support in `Vortice.Win32`.

Key replacements include:

| SharpDX type | Vortice.Windows replacement |
|--------------|-----------------------------|
| `SharpDX.Direct3D9.Device` | `Vortice.Direct3D9.Device9` |
| `SharpDX.Direct3D9.PresentParameters` | `Vortice.Direct3D9.PresentParameters` |
| `SharpDX.Direct3D9.Sprite` | `Vortice.Direct3D9.Sprite` |
| `SharpDX.DirectSound.SecondarySoundBuffer` | `Vortice.DirectSound.SecondarySoundBuffer` |
| `SharpDX.Mathematics.Interop.RawColorBGRA` | `Vortice.Mathematics.Color4` / `ColorBGRA` |

`System.Numerics` vectors remain compatible, but constructors typically require explicit conversion to `Vector2F`/`Vector3F` or `ColorBGRA` structs provided by Vortice.

## Rendering adjustments
* **Device creation:** Vortice exposes static `D3D9.CreateDevice` helpers instead of instance constructors. You supply the adapter (`Adapter` enum), device type, window handle, create flags, and `PresentParameters`. Device reset handling is similar but the `Reset` method returns a `Result` struct that should be checked for `ResultCode.DeviceLost` and `ResultCode.DeviceNotReset`.
* **Sprites & textures:** `Sprite.Draw` accepts `ColorBGRA`, `Rect?`, and `Vector3?` as in SharpDX, but the structs come from the Vortice namespaces. Replace helper extensions to emit `Vortice.Mathematics` structures.
* **Surfaces & locking:** Methods such as `Surface.LockRectangle` return `LockedRectangle` containing a `DataStream` with different ownership semantics. Swap SharpDX-specific extension methods with Vortice equivalents (`ReadRange`, `WriteRange`).

## Audio pipeline
SharpDX’s DirectSound buffer flags map directly to Vortice enums. However, buffer status polling relies on `SecondarySoundBuffer.Status` returning a `BufferStatus` flags enum, so any custom helper wrappers must be updated. Access to play cursors uses `SecondarySoundBuffer.CurrentPlayPosition`/`CurrentWritePosition` properties.

If the project already bridges through NAudio for WAV loading, you can keep those types and only change the final interop layer that feeds the raw PCM data into Vortice’s buffers (`SecondarySoundBuffer.Write`).

## Input and window loop
SharpDX’s `RenderLoop.Run` has an equivalent in Vortice via `Vortice.Win32.MessagePump`. When migrating, ensure that the Win32 window handle (`HWND`) is passed to `MessagePump.Run` and that idle handlers call into the rendering loop using the new device types.

## Testing considerations
Moving to Vortice is a sizeable refactor:
* Every SharpDX namespace import must be replaced.
* Helper extensions for color, rectangles, and vectors must be rewritten.
* DirectSound wrappers and buffer management need verification to ensure the behaviour matches SharpDX’s implementation.
* Shader compilation, texture loading, and device lost/reset paths must be regression-tested on Windows 10/11 with both windowed and full-screen modes.

Expect several hundred lines of changes across rendering, audio, and project files. Because Vortice APIs are similar yet not drop-in replacements, budget time for a full build and manual play-test cycle.
