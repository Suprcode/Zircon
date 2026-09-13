# Verification routing

Choose the smallest build/check/manual path that exercises the changed boundary. Authority: [project map](PROJECT_MAP.md), the actual `.csproj` files, and the source/check entry points linked below. `Zircon Server.sln` contains the application projects; the three focused check executables below are outside it.

## Verification principles

* Build success is not proof of gameplay correctness. A focused check proves only the behavior and inputs it actually covers.
* Do not claim “tests pass” unless the relevant tests were actually run. Report commands, outcomes and unverified behavior separately; source inspection is not execution.
* Do not run every project for every change. Select consumers at the changed boundary; project builds also build their references.
* Network/protocol changes require compatible client and server builds. Persisted-data changes may require migration/compatibility checks with existing databases.
* Asset/rendering changes often require graphical/manual verification with real content.

## Change-to-verification matrix

Project names below mean their same-named `.csproj` in that project directory. Add a focused check only when its coverage below matches the change.

| Change type | Minimum automated/build verification | Additional/manual verification |
| --- | --- | --- |
| Shared `SystemModel` | Build LibraryCore and affected definition consumers: Client, ServerLibrary and Server (editor). ServerCore validates console-host integration when affected. Other direct LibraryCore consumers are PluginCore and PatchManager; compile them when the changed API reaches them. | Load an existing System.db copy; edit/save/reopen exposed fields in Server. Verify definition distribution to client `Data/System.db` and index resolution when definitions change. See [data model](DATA_MODEL.md). |
| Server gameplay only: targeting, combat, movement | Build ServerLibrary. Build ServerCore or Server when the host boundary changes or preparing the relevant runtime. No general server gameplay check project was found. Client build is needed only if shared types, packets or client code also change. | Reproduce the rule in a test server with representative targets, timing, movement and boundary cases; confirm authoritative state and visible outcomes. |
| Client gameplay | Build Client; use GroundLootChecks only for its linked helpers, LabelRenderingChecks only for label presentation, or RenderingCacheChecks for relevant cache behavior. | Exercise changed input, prediction/animation and reconciliation in the client; reconnect/change maps when state lifetime changes. |
| Packet/network | Build LibraryCore packet definitions, ServerLibrary and Client against the same definitions; include the affected server host for integration. | [Packet.cs](../LibraryCore/Network/Packet.cs) discovers packet types and serializes properties reflectively. Exercise sender, receiver and concrete `Process` dispatch; verify compatible versions, payload fields and round trips. Two successful builds alone do not establish interoperability. |
| MirDB / System.db | Build LibraryCore and affected model consumers (Client, ServerLibrary, Server/editor). For shared MirDB changes also exercise user-data paths. | With database copies, initialize, change, save and reload representative records; verify defaults, indices, inverse relationships/deletion and applicable migration. Check editor support and definition distribution. Source inspection alone cannot prove compatibility with deployed `.db` files. |
| Server Users.db / DBModel | Build ServerLibrary; Server if its data editor is affected. Build LibraryCore and Client if exposed transfer/client structures change. | Use server persistence to save/reload representative records and relationships, including existing Users.db copies and restart behavior. [SEnvir](../ServerLibrary/Envir/SEnvir.cs) owns the server session. |
| Client preference MirDB | Build Client; LibraryCore is a reference (apply broader MirDB routing if the engine changes). | [CEnvir](../Client/Envir/CEnvir.cs) opens a Users-mode session under `Data`; models are in `Client/UserModels`. Change a preference, exit/restart and reload old preference data. This client `Data/Users.db` is distinct from server Users.db. |
| Local client UI/dialog | Build Client; label/cache checks only if those mechanisms change. Pure layout changes need no ServerLibrary/network verification. | Open/close, layout, clipping, hover, input/focus, dragging and window/UI scales as applicable; verify disposal/reopening. |
| Ground loot / world names / labels | Build Client; run GroundLootChecks for pile, row, label ownership or loot-effect logic; LabelRenderingChecks for real label/cache drawing. | Use the [ground-loot live scenarios](../Tests/GroundLootChecks/README.md#live-validation-still-required): empty scenes, piles, pickup, names/effects toggles, occlusion, edge hover/paging, scales and map/reset lifetime. Checks do not validate server pickup rules or live FPS. |
| Rendering/cache/backend | Build RenderingCore and Client; run RenderingCacheChecks on the affected supported backend. Build Server/LibraryEditor when shared API or their rendering host behavior changes. | Run the graphical Client with real scenes, scales, clipping, resize/reset and repeated resource lifetimes. Synthetic checks cannot validate every control, asset or backend; Vulkan has no pixel readback in this check. |
| Asset/library format | Build RenderingCore plus the changed authoring tool (LibraryEditor or ImageManager); build affected runtime consumers for API changes. | Pair writer output with current [runtime MirLibrary reader](../RenderingCore/Library/MirLibrary.cs), reopen in the relevant editor, and inspect real image/frame indices, offsets, shadows/overlays and supported codecs/versions. [LibraryEditor writer](../LibraryEditor/Mir3Library.cs) supports versioned/compressed containers; [ImageManager writer](../ImageManager/Mir3Library.cs) writes its own legacy layout. They are not interchangeable. Test each affected writer/reader pair with old and new samples. |
| Server editor | Build Server (references LibraryCore, ServerLibrary, PluginCore and RenderingCore). | Open the relevant `Server/Views` workflow; create/edit/save/reopen representative data and check relationships. Include client definition delivery when edited system data is consumed there. |
| Editor plugin / standalone host | Build PluginCore and affected Server or PluginStandalone host. | Load the actual plugin and exercise its menu/form/data workflow in the changed host. |
| Launcher/patching | Build the changed Launcher, Patcher or PatchManager project. Manifest changes require both PatchManager and Launcher; self-update changes require Launcher and Patcher. | Round-trip the duplicated `PatchInformation` binary read/write format ([publisher](../PatchManager/PatchInformation.cs), [reader](../Launcher/PatchInformation.cs)); test download, checksum, extraction and repair in staging. Exercise Patcher source/destination arguments and launcher replacement/restart on disposable copies; verify the helper bundled by Launcher matches the intended Patcher build. |

## Existing focused checks

These are executable checks, not a discovered unit-test suite; run them with `dotnet run`, not merely `dotnet test`.

| Check and source | Actual coverage | Does not prove |
| --- | --- | --- |
| [GroundLootChecks](../Tests/GroundLootChecks/Program.cs), [README](../Tests/GroundLootChecks/README.md) | Links production render rows, piles, highlights, label cache and MirEffect/LootEffect against headless substitutes. Checks ordered row equivalence, empty/reset cases, deterministic pile replacement, warmed allocations, label reference ownership/colour separation and effect timing/bounds/data ownership. | GPU disposal, actual layout, whole-client integration, server pickup or game FPS. |
| [LabelRenderingChecks](../Tools/LabelRenderingChecks/Program.cs), [README](../Tools/LabelRenderingChecks/README.md) | Real Client labels in a hidden 1024×768 DirectX 11 host at 100% scaling; 2,000 overlapping/clipped draws versus a layer refreshed every 16 frames. Compares first-frame pixels per mode and reports warmed timings/rebuild counts. | Live world-name invalidation, other scales/backends, moving scenes or predicted game FPS. |
| [RenderingCacheChecks](../Tools/RenderingCacheChecks/Program.cs) | Synthetic window/UI scales 1–2, direct/cached/border pixel comparisons on DirectX 11, nested surface restoration through exceptions, pool reuse/cap and resize invalidation; counters/timings. `--vulkan` selects Vulkan draw/context/cache smoke checks without GPU readback. | Vulkan pixel equivalence, all graphics features, real asset correctness or full device-loss recovery. |

Repository inventory found these three check projects and no additional test project. `Tools/convert_audio_to_ogg.cmd` is a conversion utility, not a correctness check. Runtime/editor executables and their entry points are indexed in [PROJECT_MAP](PROJECT_MAP.md#projects-and-entry-points); generated/output directories are not additional verification suites.

## Commands

Examples from repository root, selected individually by the matrix:

```text
dotnet build LibraryCore/LibraryCore.csproj
dotnet build ServerLibrary/ServerLibrary.csproj
dotnet build ServerCore/ServerCore.csproj
dotnet build Client/Client.csproj
dotnet build Server/Server.csproj
dotnet build RenderingCore/RenderingCore.csproj
dotnet build LibraryEditor/LibraryEditor.csproj
dotnet build ImageManager/ImageManager.csproj
dotnet build Launcher/Launcher.csproj
dotnet build Patcher/Patcher.csproj
dotnet build PatchManager/PatchManager.csproj
dotnet build PluginCore/PluginCore.csproj
dotnet build PluginStandalone/PluginStandalone.csproj
dotnet run --project Tests/GroundLootChecks/GroundLootChecks.csproj -c Release
dotnet run --project Tools/LabelRenderingChecks/LabelRenderingChecks.csproj
dotnet run --project Tools/RenderingCacheChecks/RenderingCacheChecks.csproj
dotnet run --project Tools/RenderingCacheChecks/RenderingCacheChecks.csproj -- --vulkan
```

Paths, SDK project structure and check arguments were inspected; these commands were not executed for this documentation-only change. Use .NET 10; Windows applications/rendering target `net10.0-windows8.0`. Graphics checks require Windows and an available matching graphics backend. DevExpress package access and native dependencies (including authoring-tool Squish references) can prevent a clean generic CLI build. Inspect configuration/platform and output paths first: several projects write to sibling Debug/Release directories outside the source tree. Use `--no-restore` only with an applicable completed restore.

## Documentation reference check

Run from the repository root (Windows PowerShell 5.1 or PowerShell 7; no packages, graphics or game build required):

```text
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/CheckDocumentation.ps1
```

The execution-policy option applies only to that process, allowing this unsigned repository script to run. [CheckDocumentation.ps1](../Tools/CheckDocumentation.ps1) checks root AGENTS.md and `docs/**/*.md`: local inline Markdown links and reference-link definitions, ATX heading anchors (including duplicate suffixes), and single-backtick file references rooted in known repository areas. Supported file extensions are .cs, .csproj, .sln, .md, .cmd, .json, .config and .ps1; explicit root AGENTS.md and solution names are also checked. Failures include document, line and target; exit code is 1 on failure, 0 on success.

External URLs, generated/build directories, fenced examples, HTML comments, obvious placeholders, abbreviated/context-relative source paths, bare source filenames and method/class names are intentionally skipped. This is not a complete Markdown parser: Setext headings, custom HTML anchors, nested link syntax and unresolved reference-link usages are unsupported. It protects navigation, not semantic accuracy, gameplay rules or whether a canonical example remains representative. Source remains authoritative.

No existing CI/build integration point was found; run this standalone check after documentation edits or documented file/heading moves. Its results are independent of the gameplay/graphics checks above.

## Build success does not verify

* Actual gameplay rules or timing-sensitive movement/combat.
* Graphical correctness, resource lifetime under real play, or real ZL library/image/frame indices.
* Deployed System.db/Users.db compatibility; verify save/load and migration on representative copies.
* Client/server version interoperability or definition distribution.
* Patch publication/deployment infrastructure, download availability or successful launcher replacement.

**Needs verification:** clean builds and dependency availability on the target machine; execution of the selected focused checks; applicable live gameplay/graphics scenarios, deployed database compatibility, protocol interoperability and staging patch workflows. This guide records source-grounded routes, not passing results for those areas.
