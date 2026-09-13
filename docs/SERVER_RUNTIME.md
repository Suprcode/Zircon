# Server runtime

## Entry and central ownership

Start with `ServerLibrary/Envir/SEnvir.cs`, `SConnection.cs`, `ServerLibrary/Models/MapObject.cs` and `Map.cs`. Both `Server/Program.cs → SMain` and `ServerCore/Program.cs` host this simulation; ServerCore is not a second implementation of combat.

`SEnvir` is process-global state: Session and system/user collections, connections/new-connection queue, maps/instances, Players, linked-list Objects, ActiveObjects, Random, Now, rankings, conquest wars, event handling, logs and network listeners. `StartServer` starts its environment thread. Startup initializes data, calls `StartEnvir`, creates/loads maps and associated regions/movements/NPCs, and starts networking and WebServer. `StopEnvir` unwinds world state and registries. See individual methods rather than reading the whole file.

| SEnvir responsibility | Search anchor |
| --- | --- |
| Startup / shutdown / loop | `StartServer`, `StartEnvir`, `StopEnvir`, `while (Started)` |
| Network admission | `StartNetwork`, `Connection`, `NewConnections`, `StopNetwork` |
| Definition + user data | `Session`, `GetCollection`, `Save` |
| Maps / lazy maps / instances | `GetMap`, `CreateMovements`, `CreateNPCs`, `CreateSafeZones`, `Instances` |
| Accounts / character selection | `Login`, `NewCharacter`, `StartGame` |
| Item construction | `CreateFreshItem` |
| Spell registration | `MagicTypes` |
| Scheduled world state | `ConquestWars`, `EventHandler`, `StartConquest` |

## Canonical examples

Use [server behavior examples](CANONICAL_EXAMPLES.md#server-behavior): crafting favourite selection for a small player feature, the crafting partial for timed work, OmaMage for a single monster override and ZumaKing only for multiple coordinated hooks.

## Processing and timing

The loop refreshes `SEnvir.Now`, admits queued connections and processes connections, then calls every player's `StartProcess`. Non-player ActiveObjects are processed in a rolling, approximately one-millisecond work window. Timed branches process maps/instances, wars, events, WebServer and saving. This is not a fixed tick that necessarily visits every monster every iteration.

`Models/MapObject.cs: StartProcess` handles activation/due delayed actions and processing. `Models/DelayedAction.cs` carries action time/type/parameters; derived `ProcessAction` interprets them. `PlayerObject.Process` calls feature processing including crafting; `MonsterObject.Process` handles live/dead timing and AI hooks. Use `SEnvir.Now` and existing action/attack/move/search deadlines; do not add sleeping waits inside gameplay handlers.

Socket callbacks and startup parallel loads exist, as do log/background operations. Ordinary packet gameplay handlers and player/monster processing share the environment loop; that is the ownership pattern to preserve, not a blanket assertion that the entire server runs on one thread.

## Object hierarchy and lifetime

```text
Server.Models.MapObject (ServerLibrary/Models/MapObject.cs)
├─ PlayerObject (persistent CharacterInfo + SConnection)
├─ MonsterObject (MonsterInfo + AI; Models/Monsters specializations)
├─ NPCObject (NPC content and interaction)
├─ ItemObject (ground item presence)
└─ SpellObject (world spell presence)
```

`MagicObject` is the spell execution abstraction, distinct from the map-resident `SpellObject`.

`MapObject.Spawn(Map,Point)` rejects an existing Node, checks map/player restrictions and cell validity, assigns CurrentCell, adds the object to `SEnvir.Objects`, and invokes `OnSpawned`. Map/cell changes call `Map.AddObject/RemoveObject` and `Cell.AddObject/RemoveObject`. Visibility sets (`NearByPlayers`, `SeenByPlayers`, `DataSeenByPlayers`) and player visible-object sets participate in notifications.

`Despawn` requires a Node, invokes pre-despawn hooks, clears map/cell, removes visible objects, unlinks Node and active registration, invokes `OnDespawned`, then `CleanUp`. Do not manually delete one registry entry; do not call Despawn twice. Inspect derived cleanup for pets, spells, crafting and auto-path state.

## Maps and broadcasts

`Map.Load` reads `Config.MapPath + MapInfo.FileName + .map`, dimensions and movement flags into `Cell[,]`/ValidCells. Missing maps log and return. `SEnvir` supports both eager and lazy loading; instance map dictionaries are keyed separately from ordinary maps.

`Map` maintains Objects, Players, Bosses, NPCs, castle-specific lists and OrderedObjects. `SpawnInfo.DoSpawn` uses respawn content and `MonsterObject.GetMonster`. A cell's occupancy and map lists are not interchangeable. `Map.Process` handles map-level timed work; object AI runs through the environment's active-object path.

`PlayerObject.AddObject/RemoveObject` and object `GetInfoPacket/GetDataPacket` determine which representation reaches a player. Nearby/full visibility and data-object visibility have separate packet families; do not turn a targeted notification into a global broadcast without inspecting its recipients.

## PlayerObject partial map

All paths are relative to `ServerLibrary/Models/`:

| File | Responsibility / first methods |
| --- | --- |
| `PlayerObject.cs` | Core lifecycle plus most feature families; use region map below |
| `PlayerObject.Crafting.cs` | Recipe validation, start/completion/cancellation, material aggregation, favourite and crafting progression: `StartCrafting`, `ProcessCrafting`, `CanCraft` |
| `PlayerObject.Milestone.cs` | Event logging, eligibility, active milestone and reward claim: `LogMilestone`, `CheckMilestones`, `MilestoneClaim` |
| `Players/PlayerObject.AutoPath.cs` | Player AutoPathState and forwarding methods to AutoPathService; planning/execution is in `AutoPath/AutoPathService.cs` and `AutoPathRoutePlanner.cs` |

Most systems still live in the main file. Do not invent `PlayerObject.Inventory.cs` or move code merely to match the documentation.

| Main-file region | Useful anchors |
| --- | --- |
| Initialization / Process / Game Session | constructor, `SetupMagic`, `Process`, `StartGame`, `StopGame` |
| Character Stats / Progression and Revival | `RefreshStats`, `GainExperience`, `LevelUp`, `SetHP`, `ChangeMP` |
| Objects View / Teleportation | `AddObject`, `RemoveObject`, `Teleport`, `TeleportRing` |
| Communication / Observation | `Chat`, `Inspect`, `SetUpObserver` |
| Marriage / Companions | `MarriageRequest`, `MarriageJoin`, `CompanionAdopt`, `CompanionSpawn`, `SetFilters` |
| Quests / Mail | `QuestAccept`, `QuestComplete`, `QuestTrack`, `MailGetItem`, `MailSend` |
| MarketPlace | `MarketPlaceConsign`, `MarketPlaceBuy`, `MarketPlaceStoreBuy`, `GameStoreGift` |
| Guild / Group / Looking For Group | `GuildCreate`, `SendGuildInfo`, `GroupInvite`, `GroupJoin`, `LFGUpdate` |
| Items | `ParseLinks`, `CanGainItems`, `GainItem`, `ItemUse`, `ItemMove`, `ItemSort`, `PickUp`, `CanWearItem` |
| NPC operations | `NPCCall`, `NPCButton`, `NPCBuy`, `NPCSell`, `NPCRefine`, `NPCWeaponCraft` |
| Packet Actions | `Turn`, `Move`, `Attack`, `Magic`, `Mining`, `FishingCast`, `Taming` |
| Combat | `AttackLocation`, `MagicAttack`, `Attacked`, `CanAttackTarget`, `Die`, `DeathDrop` |
| Network State | `GetInfoPacket`, `GetDataPacket`, `SendShapeUpdate` |
| Instance / Dungeon Finder | `JoinInstance`, `GetInstance`, `CheckInstanceFreeSpace`, `SetTimer` |
| Currency / Friends / Discipline | `GetCurrency`, `UpdateOnlineState`, `IncreaseDiscipline` |
| Loot Boxes / Bundles | `LootBoxOpen`, `LootBoxConfirm`, `BundleOpen`, `BundleConfirm` |

## Monster extension pattern

### Usually required

* For **server-only monster AI**, inspect `ServerLibrary/Models/MonsterObject.cs` and the subclass selected by `GetMonster`, including its base class (for example OmaMage → SkeletonAxeThrower). Follow only the changed search, target, attack or timing hooks.
* For targeting/aggro, inspect `ProcessSearch`, `ProperSearch`, `ProcessTarget`, `ShouldAttackTarget` and `CanAttackTarget`; for movement or delayed execution, follow the called pathing/`ProcessAction` implementation.

### Usually NOT required

* Client MonsterObject, UI, RenderingCore and packet definitions while reusing existing action semantics. New visible attack stages, projectiles or state require client action handling and an assessment of existing S action payloads.
* `SEnvir` startup, Server/ServerCore hosts, MirDB and definition editors unless scheduling, lifecycle, persistence or configurable content actually changes. A target-choice adjustment does not require reading the whole environment loop.

`MonsterObject.GetMonster(MonsterInfo)` switches on **MonsterInfo.AI**, the integer behavior selector; MonsterInfo.Image is a separate visual identity. Read the selected subclass before altering base targeting. Base hooks: `ProcessAI`, `ProcessSearch`, `ProperSearch`, `ProcessRoam`, `ProcessTarget`, `ShouldAttackTarget`, `CanAttackTarget`, `Attack`, `Walk`, `Die`, `Drop`. Search deadlines, pet modes and target validity constrain these paths. Base ProcessSearch considers eligible players and their pets within ViewRange, keeps the closest candidates and randomly selects among tied candidates; pets use ProperSearch instead.

Use the [simple and complex monster examples](CANONICAL_EXAMPLES.md#simple-monster-subclass) for bounded implementations.
* `Models/AutoPath/MonsterObject.AutoPath.cs` supplies monster pathing behavior; it is another partial of MonsterObject, not a second monster class.

A target-selection-only change usually stays here. New action/projectile/appearance semantics require [CLIENT_RUNTIME](CLIENT_RUNTIME.md), the S object-action packets and the client MonsterObject image/animation cases.
