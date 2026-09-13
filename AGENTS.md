# Zircon: agent navigation

Legend of Mir 3 client, server simulation, shared game definitions/MirDB, graphics, and Windows content tools. Source is authoritative; these documents are navigation aids. Inspect the referenced implementation before changing behavior. Preserve the existing architecture and distinguish the identically named client/server types.

## Context-efficiency rules

1. Read `/AGENTS.md` first and identify the subsystem involved.
2. Read only the single most relevant documentation file initially, then open its referenced source files.
3. Read additional documentation only when the source change demonstrably crosses another subsystem boundary.
4. Do not preload every file in `/docs` "for context" or perform repository-wide searches when documentation already identifies likely source files.
5. Prefer canonical examples and directly referenced dependencies over broad discovery searches.

If the starting subsystem is unclear, use [TASK_ROUTER](docs/TASK_ROUTER.md) to map request terms and intent to one guide and source area.

Normal route: `AGENTS.md` → [TASK_ROUTER](docs/TASK_ROUTER.md) or [GAMEPLAY_SYSTEMS](docs/GAMEPLAY_SYSTEMS.md) → one relevant detailed section → initially 2–6 source files. This is a starting budget, not a limit when direct dependencies require more. Skip the router when the owner is already clear; never preload all docs.

| Known task | Detailed entry → source | Usually skip initially |
| --- | --- | --- |
| Monster targeting | [COMBAT_AND_MAGIC](docs/gameplay/COMBAT_AND_MAGIC.md#monsters-and-spawning) → server MonsterObject and relevant subclass | Client UI/rendering and packets unless visible state changes |
| Dialog layout | [CLIENT_UI](docs/CLIENT_UI.md) → owning Client/Scenes/Views file | ServerLibrary and networking unless gameplay changes |
| Packet/client-server | [NETWORKING](docs/NETWORKING.md) → concrete sender/receiver | Unrelated UI and persistence |
| Persisted property | [DATA_MODEL](docs/DATA_MODEL.md) → concrete SystemModel/DBModel | Editor/network/client unless the property crosses those boundaries |

## Repository map

| Area | Responsibility / main interaction |
| --- | --- |
| `LibraryCore` | `Library` namespaces: definitions, enums, client transfer structures, packets, configuration; `MirDB` persistence. Shared by client/server. |
| `ServerLibrary` | `Server` namespaces: authoritative simulation in `Models`, persistence in `DBModels`, networking/runtime in `Envir`. |
| `Server` | Windows `SMain` server administration and system-data editors in `Views`; hosts ServerLibrary and editor plugins. |
| `ServerCore` | Console entry point to the same ServerLibrary simulation. |
| `Client` | Input, server-state representation, scenes, DX controls, animation and sound; consumes LibraryCore and RenderingCore. |
| `RenderingCore` | `Shared.Rendering` / `Shared.Envir`: graphics pipelines, textures, caches and ZL reader; used by Client, Server and LibraryEditor. |
| `LibraryEditor` | Image-library viewer/converter/editor (`LMain`, `Mir3Library`), not the game-definition editor. |
| `ImageManager` | Separate batch image/WTL-to-ZL conversion tool (`IMain`), with its own library implementation. |
| `Components` | Bundled binary dependencies, not a C# project. |
| `PluginCore` / `PluginStandalone` | Server-editor plugin contracts/loader and separate form-plugin host. |
| `Launcher` / `Patcher` / `PatchManager` | Patch download/client launch, launcher replacement helper, and patch publication tool. |
| `Tools` | Audio conversion script and rendering-cache check executable. |

Exact project references and test projects: [PROJECT_MAP](docs/PROJECT_MAP.md).

## Client / server / shared distinction

* `LibraryCore/SystemModels/ItemInfo.cs` defines an item; `ServerLibrary/DBModels/UserItem.cs` persists an instance; `LibraryCore/Globals.cs: ClientUserItem` represents it on the client. Item flows are mapped in [GAMEPLAY_SYSTEMS](docs/GAMEPLAY_SYSTEMS.md).
* `ServerLibrary/Models/MonsterObject.cs` selects targets and executes combat. `Client/Models/MonsterObject.cs` selects image frames/effects. Changing one does not change the other.
* Local movement has client prediction and server reconciliation; do not mistake client coordinates or animation for the authoritative server cell. See [CLIENT_RUNTIME](docs/CLIENT_RUNTIME.md).

## Critical rules

* After implementation, use [VERIFICATION](docs/VERIFICATION.md) to choose the smallest relevant build/test/manual validation path. Do not claim unrelated checks validate the change.
* Packet dispatch is `public Process(ConcretePacket p)` discovered by reflection, not a central opcode switch. Packet IDs and property order are derived by reflection; changes require compatible client/server builds. See [NETWORKING](docs/NETWORKING.md).
* Receive callbacks queue packets; the environment's processing loop invokes gameplay handlers. Preserve that ownership for mutable game objects. Startup/loading also uses background work; do not infer that every method is thread-safe.
* Use MirDB collection creation and model setters calling `OnChanged`. Relationship setters maintain inverse links; aggregate deletion can delete related objects. A collection's integer indexer is a list position, not a DBObject identity lookup. See [DATA_MODEL](docs/DATA_MODEL.md).
* Server `MapObject.Spawn`/`Despawn` and cell/map setters maintain multiple registries and visibility. Do not replace them with a list edit.
* Asset identity is library plus image index; frame/direction offsets matter. Register new libraries in `LibraryCore/Libraries.cs` and check runtime/editor format compatibility.
* DX controls own children and cached graphics. Preserve parent assignment, invalidation and disposal; see [CLIENT_UI](docs/CLIENT_UI.md).
* System definitions are edited in `Server/Views`, distributed to the client's `Data/System.db`, and resolved by index. An instance packet is not automatically a definition update.

## Naming aliases and specialist routes

`C = Library.Network.ClientPackets` means client → server; `S = Library.Network.ServerPackets` means server → client; `G = Library.Network.GeneralPackets` supplies handshake/ping/disconnect. `Frame = Library.Frame` appears in client models. Project folders and namespaces differ; see the repository map above.

For runtime or partial-class navigation, choose [SERVER_RUNTIME](docs/SERVER_RUNTIME.md) or [CLIENT_RUNTIME](docs/CLIENT_RUNTIME.md) as the initial guide when relevant. For a change already known to span subsystems, start with [FEATURE_CHANGE_GUIDE](docs/FEATURE_CHANGE_GUIDE.md). These are alternative routes, not a required reading list.

## Documentation maintenance

For a representative implementation, use [CANONICAL_EXAMPLES](docs/CANONICAL_EXAMPLES.md) and open the selected method first. It covers UI, packet flow, models, player/monster behavior, visuals and editor integration; do not read every example for one task.

Documentation is a navigation index, not a second copy of the source. Keep feature-specific guidance concise.

### Update docs when

* A feature moves to another file/project or a project dependency changes.
* A significant packet flow changes, including a new major ClientPacket/ServerPacket in a documented feature.
* A new PlayerObject partial or major GameScene partial is introduced.
* A canonical example disappears or stops being representative.
* A new SystemModel/DBModel pattern or major relationship changes how future features should be implemented.
* A major rendering/backend/library mechanism changes or a documented invariant stops being true.

### Do not update docs when

* A local implementation detail or method body changes without changing navigation or feature boundaries.
* A minor helper, balance value or monster-specific constant changes.
* A one-off bug fix leaves documented architecture and flows unchanged.

### Documentation validation

Run the [documentation reference check](docs/VERIFICATION.md#documentation-reference-check) after moving/renaming documented files or headings.

When modifying a documented subsystem:

1. Check that its referenced paths and method names still exist.
2. Check that its documented “Start here” files remain representative.
3. Update only affected documentation; do not regenerate every document.

Never automatically rewrite the entire documentation set after every feature change.
