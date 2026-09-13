# Zircon: agent navigation

Legend of Mir 3 client, server simulation, shared game definitions/MirDB, graphics, and Windows content tools. Source is authoritative; these documents are navigation aids. Inspect the referenced implementation before changing behavior. Preserve the existing architecture and distinguish the identically named client/server types.

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

* `LibraryCore/SystemModels/ItemInfo.cs` defines an item. `ServerLibrary/DBModels/UserItem.cs` persists an instance; `PlayerObject.ItemMove` validates moves. `S.ItemMove` reaches `Client/Envir/CConnection.cs`, which updates client grids. `LibraryCore/Globals.cs: ClientUserItem.Info` resolves a definition from the client's system database.
* `ServerLibrary/Models/MonsterObject.cs` selects targets and executes combat. `Client/Models/MonsterObject.cs` selects image frames/effects. Changing one does not change the other.
* Local movement has client prediction and server reconciliation; do not mistake client coordinates or animation for the authoritative server cell. See [CLIENT_RUNTIME](docs/CLIENT_RUNTIME.md).

## Where to start for a change

1. Find the feature in [GAMEPLAY_SYSTEMS](docs/GAMEPLAY_SYSTEMS.md).
2. Read its dependency pattern in [FEATURE_CHANGE_GUIDE](docs/FEATURE_CHANGE_GUIDE.md).
3. Decide which boundaries change: shared definition, server, packet, client representation, UI, graphics/assets, MirDB, editor.
4. Open those referenced files/methods first. Expand searches only for direct dependencies.
5. For large partial classes, use the maps in [SERVER_RUNTIME](docs/SERVER_RUNTIME.md) and [CLIENT_RUNTIME](docs/CLIENT_RUNTIME.md).

## Critical rules

* Packet dispatch is `public Process(ConcretePacket p)` discovered by reflection, not a central opcode switch. Packet IDs and property order are derived by reflection; changes require compatible client/server builds. See [NETWORKING](docs/NETWORKING.md).
* Receive callbacks queue packets; the environment's processing loop invokes gameplay handlers. Preserve that ownership for mutable game objects. Startup/loading also uses background work; do not infer that every method is thread-safe.
* Use MirDB collection creation and model setters calling `OnChanged`. Relationship setters maintain inverse links; aggregate deletion can delete related objects. A collection's integer indexer is a list position, not a DBObject identity lookup. See [DATA_MODEL](docs/DATA_MODEL.md).
* Server `MapObject.Spawn`/`Despawn` and cell/map setters maintain multiple registries and visibility. Do not replace them with a list edit.
* Asset identity is library plus image index; frame/direction offsets matter. Register new libraries in `LibraryCore/Libraries.cs` and check runtime/editor format compatibility.
* DX controls own children and cached graphics. Preserve parent assignment, invalidation and disposal; see [CLIENT_UI](docs/CLIENT_UI.md).
* System definitions are edited in `Server/Views`, distributed to the client's `Data/System.db`, and resolved by index. An instance packet is not automatically a definition update.

## Naming aliases and core files

`C = Library.Network.ClientPackets` means client → server; `S = Library.Network.ServerPackets` means server → client; `G = Library.Network.GeneralPackets` supplies handshake/ping/disconnect. `Frame = Library.Frame` appears in client models. Project folder and namespace names differ: ServerLibrary uses `Server`, LibraryCore uses `Library` and `MirDB`, RenderingCore uses `Shared`.

| Start | Files |
| --- | --- |
| Networking | `LibraryCore/Network/{Packet,BaseConnection,ClientPackets,ServerPackets}.cs`; `Client/Envir/CConnection.cs`; `ServerLibrary/Envir/SConnection.cs` |
| Server | `ServerLibrary/Envir/SEnvir.cs`; `ServerLibrary/Models/{Map,MapObject,PlayerObject,MonsterObject}.cs` |
| Client | `Client/Program.cs`; `Client/Envir/CEnvir.cs`; `Client/Scenes/GameScene.cs`; `Client/Models/UserObject.cs` |
| Shared data | `LibraryCore/Globals.cs`, `Enum.cs`, `Stat.cs`, `SystemModels` |
| Database | `LibraryCore/MirDB/{Session,DBObject,DBCollection,DBMapping}.cs` |
| UI / graphics | `Client/Controls/DXControl.cs`; `RenderingCore/Rendering/RenderingPipelineManager.cs`; `RenderingCore/Library/MirLibrary.cs` |

## Documentation maintenance

When making a change, update documentation only when the change affects how future developers or AI agents need to understand, locate or extend the system.

Update documentation when a feature moves; a client/server flow changes; packets are added or removed from an important flow; a major model relationship changes; a new extension pattern or significant subsystem is introduced; a canonical example is removed; or an important invariant changes.

Do not update documentation for trivial implementation changes. Keep feature-specific documentation concise.
