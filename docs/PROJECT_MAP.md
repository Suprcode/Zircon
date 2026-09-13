# Project map

Authority: `Zircon Server.sln`, each named project's `.csproj`, and its `Program.cs` where executable. Main projects target .NET 10; Windows applications/rendering/plugin contracts use `net10.0-windows8.0`, while LibraryCore, ServerLibrary and ServerCore use `net10.0`.

## Direct project dependencies

Arrow means **references**, not data flow. These are the actual `ProjectReference` edges:

```text
Client ----------> LibraryCore, RenderingCore
Server ----------> LibraryCore, ServerLibrary, PluginCore, RenderingCore
ServerCore ------> LibraryCore, ServerLibrary
ServerLibrary ---> LibraryCore
PluginCore ------> LibraryCore
PluginStandalone -> PluginCore
LibraryEditor ---> RenderingCore
PatchManager ----> LibraryCore
RenderingCacheChecks -> RenderingCore
LabelRenderingChecks -> Client
LibraryCore, RenderingCore, ImageManager, Launcher, Patcher: no ProjectReference
```

Launcher compiles linked `LibraryCore/ConfigReader.cs` and `LibraryCore/Time.cs`; this is a source dependency despite having no project reference. GroundLootChecks similarly links selected client source files. Package/binary references remain separate from this graph.

## Projects and entry points

| Project / output | Responsibilities and first files | Direct consumers |
| --- | --- | --- |
| LibraryCore / library | `LibraryCore/Globals.cs` transfer structures and registries; `SystemModels` definitions; `Network`; `MirDB`; `Enum.cs`, `Stat.cs`, `FrameSet.cs`, `ConfigReader.cs` | Client, Server, ServerCore, ServerLibrary, PluginCore, PatchManager |
| ServerLibrary / library | `Envir/SEnvir.cs`, `SConnection.cs`; `Models` simulation, `Models/Magics`, `Models/Monsters`; `DBModels`; configuration/commands/events | Server, ServerCore |
| Server / WinExe | `Program.Main → SMain`; `Views` edits system and user data, diagnostics, map viewing; `Helpers` supports content operations | Operator application |
| ServerCore / Exe | `Program.Main` loads configuration/encryption, starts SEnvir and handles console cancellation | Console host |
| Client / WinExe (`Zircon`) | `Program.Main → TargetForm`, graphics initialization/message loop; `Envir`, `Models`, `Scenes`, `Controls`, `UserModels` | Player application |
| RenderingCore / library | `Rendering/IRenderingPipeline.cs`, `RenderingPipelineManager.cs`, backend directories; `Library/MirLibrary.cs`, `LibraryFormat` | Client, Server, LibraryEditor, RenderingCacheChecks |
| LibraryEditor / WinExe | `Program.Main → LMain`; `Mir3Library.cs`, legacy readers, conversion/save option dialogs | Asset editor |
| ImageManager / WinExe | `Program.Main → IMain`; batch WTL conversion and image-folder packing | Independent conversion application |
| PluginCore / library | `PluginLoader`, `AbstractStart`, `AbstractPlugin`, `Types`, `Events`, `GridActions`, `Helpers/RibbonHelper` | Server, PluginStandalone |
| PluginStandalone / Exe | `Program.Main`; configured plugin must expose an eligible standalone form | Plugin host |
| Launcher / WinExe | `Program.Main → LMain`; `PatchInformation.cs`, `Config.cs`; download/check patch files and launch Zircon | Distribution application |
| Patcher / WinExe | `Program.Main → PMain`; takes replacement source/destination arguments, waits, moves file and restarts launcher | Launcher invokes `Patcher.exe` |
| PatchManager / WinExe | `Program.Main → PMain`; patch manifest/checksum generation and upload | Creates files consumed by Launcher |

`Server` and `ServerCore` both declare `Server.Program`; they are alternate entry-point assemblies. `ServerLibrary` source uses `Server.*` namespaces, not `ServerLibrary.*`. RenderingCore's public source uses `Shared.Rendering` / `Shared.Envir`.

## Supporting folders and checks

* `Components`: ManagedSquish/native Squish/SlimDX binaries; no project file. Inspect actual assembly references before changing an asset codec dependency.
* `Tools/RenderingCacheChecks/RenderingCacheChecks.csproj`: standalone graphics/cache check executable, outside the main solution's project list.
* `Tools/LabelRenderingChecks/LabelRenderingChecks.csproj`: Windows executable referencing Client; `Program.Main` initializes client controls and a rendering session for label checks. It is outside the main solution.
* `Tests/GroundLootChecks/GroundLootChecks.csproj`: standalone linked-source checks for ground loot/render helpers; its README describes execution/scope.
* `Tools/convert_audio_to_ogg.cmd`: audio conversion utility.
* `SharedRendering` is not a project in the current solution/reference graph. Start graphics work in RenderingCore; do not select a similarly named output directory as authority.
* `ServerLibrary.Tests`, `BuildChecks`, `.build`, `tmp`, `artifacts` are not additional projects in the main solution. Temporary/generated content is not a canonical runtime extension point.

Needs verification: a full clean build and external DevExpress/native dependency availability were not tested for this documentation-only change. Inspect project-specific output paths before building: several configurations write outside the source tree.
