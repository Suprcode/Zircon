# Networking

## Start here

`LibraryCore/Network/Packet.cs` defines the wire format; `BaseConnection.cs` owns TCP queues and dispatch. Definitions are `ClientPackets.cs`, `ServerPackets.cs`, `GeneralPackets.cs` in that directory. Endpoints are `Client/Envir/CConnection.cs` and `ServerLibrary/Envir/SConnection.cs`.

**Direction is verified by send sites and handlers:** client UI calls `CEnvir.Enqueue(new C.CraftingStart ...)`; SConnection has `Process(C.CraftingStart)` and forwards to PlayerObject. PlayerObject enqueues `S.CraftingStarted`; CConnection has `Process(S.CraftingStarted)`. C and S name the sending side. G packets are exchanged during connection management.

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

| Trigger | Request → server state | Response → client state |
| --- | --- | --- |
| Craft recipe | `Client/Scenes/Views/CraftingDialogs.cs` sends `C.CraftingStart`; `SConnection.Process(C.CraftingStart)` → `PlayerObject.StartCrafting` in `PlayerObject.Crafting.cs` | `S.CraftingStarted` sets client crafting presentation; server `ProcessCrafting` revalidates, consumes materials/gold and grants result; `S.CraftingEnded` / `S.CraftingState` handlers update `GameScene.Game.User` and crafting dialogs |
| Move | `Client/Models/UserObject.cs` issues `C.Move`; `SConnection.Process(C.Move)` applies game-stage/action checks → `PlayerObject.Move` | `S.ObjectMove` in CConnection reconciles the local user or queues another object's `ObjectAction`; `S.UserLocation` handles location correction |
| Item transfer | `Client/Controls/DXItemCell.cs` sends `C.ItemMove`; `SConnection.Process(C.ItemMove)` → `PlayerObject.ItemMove` validates source/destination and updates `UserItem` ownership/slot | `S.ItemMove` handler selects grids by `GridType`, releases client cell locks, and applies the result; definition is still resolved from client system data |
| Object enters/leaves view | `PlayerObject.AddObject` / visibility machinery uses server object's `GetInfoPacket`; no client spawn authority | `S.ObjectPlayer`, `S.ObjectMonster`, `S.ObjectNPC`, `S.ObjectItem`, `S.ObjectSpell` handlers construct representations; `S.ObjectRemove` calls `Remove` |

The detailed feature index is [GAMEPLAY_SYSTEMS](GAMEPLAY_SYSTEMS.md). Treat listed packets as entry points, not a claim that every feature fits request/reply: spawns, damage, buffs and visibility updates can be unsolicited broadcasts.

## Wire compatibility

Authority: `Packet` static constructor, `GetPacketBytes`, `ReceivePacket`, `WriteObject`, `ReadObject`.

* Packet discovery includes types whose **immediate BaseType is Packet** in the executing shared assembly. IDs are list positions encoded as `short`, not explicit constants. The comparator puts GeneralPackets first, compares names within namespaces, and otherwise compares type names. Adding/renaming a packet can change unrelated IDs; do not invent an opcode registration table.
* Frame: four-byte total length, two-byte packet ID, reflected property payload. Receive waits for the full declared length. No explicit maximum packet size is enforced in `ReceivePacket`; do not equate the socket buffer with a validated limit.
* Public properties are walked with `GetProperties()` without a separately declared serialization order. `[IgnorePropertyPacket]` skips a property; fields such as `ObserverPacket` are not part of that property walk.
* Explicit primitive readers/writers include numeric types, bool, char, string, byte array, Color, Point, Size, DateTime and TimeSpan. Enums use their underlying type. Recursive handling includes objects, `List<>`, `Dictionary<,>` and `SortedDictionary<,>`; inspect both read/write branches before introducing another type. Null class markers, list counts, and dictionary entries are part of the format; strings normalize null to empty.
* `ReadObject` invokes methods marked `[CompleteObject]` after populating properties. `Globals.cs: ClientUserItem.Complete` resolves its non-serialized Info field from InfoIndex and completes sockets. Copying the properties without this completion step does not produce a fully usable client item. In the recursive object-list reader, a null element marker is skipped rather than added as a null list slot; do not use that encoding to preserve positional holes.
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

1. Add a direct Packet subclass with supported public properties to ClientPackets.
2. Follow the closest client send site through `CEnvir.Enqueue`.
3. Add `public void Process(C.NewAction p)` to SConnection; follow stage, observer and player validation from the adjacent feature.
4. Delegate player behavior to PlayerObject/the appropriate partial; preserve synchronous state ownership.
5. If needed, add an S packet and `public void Process(S.NewUpdate p)` in CConnection; update the correct model and dialog.
6. Audit wire IDs/properties, enum values, observer visibility, rejection/lock release, lifecycle cancellation and matched builds.
