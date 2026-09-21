# Rendering and assets

## Ownership and current backend selection

`Client/Program.cs` supplies host settings, initializes graphics and runs the message loop. `Client/Envir/CEnvir.cs: RenderGame` draws the active scene through `RenderingCore/Rendering/RenderingPipelineManager.cs`. Client MapControl/models own game draw ordering, image/frame choice and effects; RenderingCore owns backend graphics calls, textures, targets and caches.

Start low-level work at `Rendering/IRenderingPipeline.cs`, `RenderingPipelineManager.cs`, `RenderingPipelineContext.cs` and `RenderTexture.cs`. The manager's **PipelineFactories** currently registers Silk D3D11 and Silk Vulkan, with D3D11 as default. `RenderingPipelineIds.cs` also names OpenGL; a constant alone is not proof of a registered runtime backend. SharpDX D3D9/D3D11 source directories exist; inspect factory registration before claiming they are selectable. Vulkan's implementation file is named `Rendering/SilkVulkan/SilkVulcanRenderingPipeline.cs` (filename spelling differs from class spelling).

Hosts offering runtime backend switching supply `RenderingHostSettings.RecreateRenderTarget`. The manager calls it after shutting down the old renderer and before initializing the replacement. The client uses `TargetForm.RecreateRenderTarget` to renew the native HWND while preserving managed controls; reusing a window after Vulkan FIFO presentation can leave DirectX 11's blt output hidden behind the last Vulkan frame.

Canonical references: [MirEffect and DXImageControl](CANONICAL_EXAMPLES.md#client-visuals-and-assets).

## Library and image identity

RenderingPipelineManager is partial: `RenderingPipelineManager.cs` owns selection/common operations; `UIBorderRendering.cs` border primitives; `UICacheContext.cs` presentation coordinate/cache context; `UICacheKey.cs` cache identity; `UICacheTargetPool.cs` pooled UI targets. These all live under `RenderingCore/Rendering/`.

`LibraryCore/Libraries.cs` contains both `LibraryFile` and `Libraries.LibraryList`, the actual enum → relative filename map. Examples: GameInter → `Data/GameInter.Zl`, Interface → `Data/Interface.Zl`, MagicIcon → `Data/MIcon.Zl`, MiniMapIcon → `Data/MiniMapIcon.Zl`.

`Client/Program.cs` creates `Shared.Rendering.MirLibrary` entries in `CEnvir.LibraryList` for available mapped files. `RenderingCore/Library/MirLibrary.cs` owns streams, lazy library/image reading and texture creation. `Client/Controls/DXImageControl.cs` resolves LibraryFile and Index; game models combine library, base index, animation frame and direction. An image number has meaning only within its library and layout.

For a new visual, inspect the current draw site and the library using `LibraryEditor/LMain.cs`; preserve index, offsets, shadow/overlay layers, frame count and direction stride. Adding/reordering images can break unrelated hard-coded ranges. New enum values need an actual `Libraries.LibraryList` entry and distributed asset file.

## Formats and authoring tools

| Boundary | Reader / writer authority |
| --- | --- |
| Runtime ZL | `RenderingCore/Library/MirLibrary.cs` (`ReadLibrary`, compressed container path, image loading) |
| ZL2 metadata/container | `RenderingCore/LibraryFormat/ZlFormat.cs`, `ZlImageMetadata.cs`, `ZlAtlasPageMetadata.cs`, `ZlPayloadSegment.cs` |
| Asset authoring | `LibraryEditor/Mir3Library.cs`, `LibrarySaveOptions.cs`, `LibraryConversionOptions.cs`; `LMain.cs` |
| Legacy conversion | LibraryEditor's `WeMadeLibrary.cs`, `WTLLibrary.cs`, `CrystalLibraryV1.cs`, `CrystalLibraryV2.cs`; ImageManager has a separate conversion path |

The runtime reader supports its older ZL header path and the `ZL2` signature/container path. ZL2 describes image/atlas counts, compression and metadata/index offsets. `ZlImageCodec`, `ZlRuntimeTexturePreference`, `ZlContainerCompression` and atlas layers are explicit on-disk values. A codec/metadata change must be supported by both reader and writer; a file extension alone does not identify all format details. ImageManager's older writer should not be assumed equivalent to LibraryEditor's current writer.

## Resource lifetime and drawing

`MirLibrary` is IDisposable and owns image/atlas state and file streams. Its static GetNow/GetCacheDuration/GetUseZlAtlasPages callbacks configure runtime behavior. `RenderingPipelineManager.RegisterTextureCache`, `RegisterControlCache`, `RegisterSoundCache` connect cache items to the active pipeline; `CEnvir.UpdateRealtime` calls MemoryClear periodically.

For resource changes inspect `Rendering/CacheItems.cs`, `RenderTexture.cs`, `RenderSurface.cs`, `RenderTargetResource.cs`, `RenderTexturePool.cs`, `UICacheTargetPool.cs`, `UICacheContext.cs` and `UICacheKey.cs`. Pool acquisition/release is not equivalent to disposing an arbitrary owned texture. Follow the specific owner's path and invalidate cached children when their content changes.

Client `Program.InvalidateRenderCaches/InvalidateUiRenderCaches` handles backend/resolution changes. DXControl participates in ITextureCacheItem, texture validity and child caching; refer to [CLIENT_UI](CLIENT_UI.md) before changing BeforeDraw/AfterDraw behavior. Backend state (blend, target, clipping, scale) is shared through the manager: preserve save/restore scopes in surrounding drawing code.

`Client/Models/MirEffect.cs`, `MirProjectile.cs`, `MirLineEffect.cs`, `SpellObject.cs`, PlayerObject and MonsterObject select visual effects. Server delayed damage is a separate mechanism in ServerLibrary; changing a client effect duration does not by itself change damage timing. `Client/Envir/DXSoundManager.cs` maps sound identifiers/events; `DXSound.cs` owns sound playback/cache integration. Audio still has client SharpDX/NAudio dependencies independent of graphics selection.

## Migration context and checks

The historical `vortice-migration.md` proposal is absent from this checkout. Current backend authority is the project files and registered pipeline factories above, not historical package assumptions.

Check project paths/scopes are in [PROJECT_MAP](PROJECT_MAP.md#supporting-folders-and-checks); automated checks do not replace exercising the graphical client. For world-name scaling, inspect `Client/Scenes/Views/MapControl.Names.cs` with DXLabel because the overlay has its own cache/lifetime.

Needs verification: exact image contents/index availability in deployed ZL libraries and backend/device visual parity require the actual assets and graphics runs; this documentation does not certify either.
