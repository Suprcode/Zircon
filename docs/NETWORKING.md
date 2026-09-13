# Networking

## Start here

`LibraryCore/Network/Packet.cs` defines the wire format; `BaseConnection.cs` owns TCP queues and dispatch. Definitions are `ClientPackets.cs`, `ServerPackets.cs`, `GeneralPackets.cs` in that directory. Endpoints are `Client/Envir/CConnection.cs` and `ServerLibrary/Envir/SConnection.cs`.

C and S name the sending side; G packets handle connection management. Verify direction at the concrete send site and Process handler.

## Canonical examples

Follow the [four crafting packet steps](CANONICAL_EXAMPLES.md#packet-flow) for client send, server Process handler, server response and client Process handler. Start at those named methods before inspecting other packet implementations.

## Transport and dispatch

```text
client UI/UserObject → CEnvir.Enqueue → CConnection/BaseConnection.SendList
→ BaseConnection.Process → Packet.GetPacketBytes → BeginSend
→ peer ReceiveData → Packet.ReceivePacket → ReceiveList
→ peer BaseConnection.Process → ProcessPacket
→ GetMethod("Process", concrete packet type) → public Process(T packet)
```

`ReceiveData` assembles partial/coalesced TCP reads in `_rawData`; its 8 KiB receive buffer is not a packet-size limit. `ReceiveList`/`SendList` are concurrent queues. Socket callbacks enqueue packets; handlers run when the owning environment calls `Connection.Process`. `PacketMethods` caches methods by **connection type and packet type** under a lock. Missing handlers go to `ProcessUnhandledPacket`; the base implementation throws `NotImplementedException`.

Server: `SEnvir` drains `NewConnections`, calls each connection's `Process`, then processes game objects. Client: `CEnvir.UpdateRealtime` calls `Connection?.Process` before scene simulation/rendering. Keep game-state changes in those established execution paths.

## Representative complete flows

Use the [canonical crafting packet flow](CANONICAL_EXAMPLES.md#packet-flow) for request/response structure. Feature-specific flows live with their owners:

* [Movement](gameplay/WORLD_AND_MOVEMENT.md#movement-maps-and-teleportation): C.Move, S.ObjectMove/UserLocation; [client reconciliation](CLIENT_RUNTIME.md#object-and-animation-lifecycle).
* [Item transfer](gameplay/ITEMS_AND_ECONOMY.md#inventory-equipment-and-storage): C/S.ItemMove, ownership/slot changes and grid locks.
* [Visibility](SERVER_RUNTIME.md#maps-and-broadcasts) and [client object lifecycle](CLIENT_RUNTIME.md#object-and-animation-lifecycle): object creation/removal is server-driven.

Spawns, damage, buffs and visibility updates can be unsolicited broadcasts; not every feature is request/reply.

## Wire compatibility

Authority: `Packet` static constructor, `GetPacketBytes`, `ReceivePacket`, `WriteObject`, `ReadObject`.

* Packet discovery includes types whose **immediate BaseType is Packet** in the executing shared assembly. IDs are list positions encoded as `short`, not explicit constants. The comparator puts GeneralPackets first, compares names within namespaces, and otherwise compares type names. Adding/renaming a packet can change unrelated IDs; do not invent an opcode registration table.
* Frame: four-byte total length, two-byte packet ID, reflected property payload. Receive waits for the full declared length. No explicit maximum packet size is enforced in `ReceivePacket`; do not equate the socket buffer with a validated limit.
* Public properties are walked with `GetProperties()` without a separately declared serialization order. `[IgnorePropertyPacket]` skips a property; fields such as `ObserverPacket` are not part of that property walk.
* Explicit primitive readers/writers include numeric types, bool, char, string, byte array, Color, Point, Size, DateTime and TimeSpan. Enums use their underlying type. Recursive handling includes objects, `List<>`, `Dictionary<,>` and `SortedDictionary<,>`; inspect both read/write branches before introducing another type. Null class markers, list counts, and dictionary entries are part of the format; strings normalize null to empty.
* `ReadObject` invokes methods marked `[CompleteObject]` after populating properties. For item completion/definition lookup, see [DATA_MODEL](DATA_MODEL.md#definition--instance--representation). In the recursive object-list reader, a null element marker is skipped rather than added as a null list slot; do not use that encoding to preserve positional holes.
* This is not a schema-negotiated protocol. Deploy matching packet/property/enum definitions. The handshake's version check is not a serializer migration mechanism.
* BaseConnection's shown game-packet path directly sends serialized bytes: it does not wrap them in transport encryption or compression. Database encryption and patch gzip are separate boundaries.

## Lifecycle and observation

`SConnection(TcpClient)` starts receive and sends `G.Connected`; CConnection answers the handshake. Server `Process(G.Connected/G.Version)` handles optional `Config.CheckVersion`; `G.GoodVersion` includes the system database version. Client `Process(G.GoodVersion)` records it. See `Client/Scenes/LoginScene.cs` for use of database readiness/version state.

`SConnection.Stage` gates login, character selection and gameplay handlers. `SEnvir` handles account/character operations and `StartGame`; client `S.Login` / `S.StartGame` handlers transition scenes. `SConnection.CleanUp` detaches account/player/observation state and calls `Player.StopGame`. Disconnect during recent combat can be delayed; preserve `TryDisconnect`/`TrySendDisconnect` behavior.

Observers are explicit (`Observed`, `Observers`, `ObserverPacket`, client `GameScene.Observer`), not a second player session. Check SConnection's enqueue/observer handling and feature gates when adding state updates.

`SConnection.Process` also checks ReceiveList.Count against Config.MaxPacket and can block an IP; this is a queue-count guard, separate from packet byte framing. `SConnection.Enqueue` forwards packets to Observers when ObserverPacket permits it.

## Separate HTTP paths

`ServerLibrary/Envir/WebServer.cs` implements HttpListener paths for account operations, payment notifications and an optional SystemDBSync upload. Account commands are queued to `WebCommandQueue` and consumed by `WebServer.Process` from SEnvir; inspect the specific callback because some account reads occur there too. These paths are separate from Packet/BaseConnection.

SystemDBSync's caller is **`Server/Views/SyncForm.cs`**, which posts the editor System.db to another server with its configured sync key. The endpoint validates enablement, method/body, declared size (10 MiB limit) and key, backs up/replaces the file. It does not reload the already-created simulation collections in that handler. This is not a game-client database-download endpoint. Client definition copying/version display is documented in [CONTENT_AND_EDITORS](CONTENT_AND_EDITORS.md).

## Adding a client action

### Usually required

* For a **packet change**, inspect its definition in `LibraryCore/Network`, concrete sender, receiving public `Process(T)` handler in SConnection or CConnection, and the feature state they read/write. If the feature has a response/update, trace that direction too.
* Inspect reflected payload compatibility in `Packet.cs`, including nested transfer structures; use the canonical flow above and compatible client/server builds.

### Usually NOT required

* Unrelated Views dialogs or DBModels: open only the UI consuming/triggering the changed feature and persistence actually storing its state.
* `BaseConnection` transport, WebServer HTTP paths or an invented opcode registry for an ordinary supported packet property. Inspect transport/serialization implementation changes only if the requested wire behavior or property type requires them.

1. Add a direct Packet subclass with supported public properties to ClientPackets.
2. Follow the closest client send site through `CEnvir.Enqueue`.
3. Add `public void Process(C.NewAction p)` to SConnection; follow stage, observer and player validation from the adjacent feature.
4. Delegate player behavior to PlayerObject/the appropriate partial; preserve synchronous state ownership.
5. If needed, add an S packet and `public void Process(S.NewUpdate p)` in CConnection; update the correct model and dialog.
6. Audit wire IDs/properties, enum values, observer visibility, rejection/lock release, lifecycle cancellation and matched builds.
