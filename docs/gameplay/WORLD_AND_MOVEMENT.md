# World and movement

[Gameplay router](../GAMEPLAY_SYSTEMS.md)

## Reading this guide

Paths are repository-relative: **P** = `ServerLibrary/Models/PlayerObject.cs`; **Defs** = `LibraryCore/SystemModels/`; **DB** = `ServerLibrary/DBModels/`; **Views** = `Client/Scenes/Views/`.

Start at the selected feature's anchors; packet lists are entry points, not exhaustive protocols. C/S denote client/server senders; dispatch and transfer structures: [NETWORKING](../NETWORKING.md). Follow other boundaries only as needed via the [guide index](../README.md); editor counterparts: [CONTENT_AND_EDITORS](../CONTENT_AND_EDITORS.md).

## Change boundaries

* **Usually required:** Authoritative map/location or lifecycle owner and matching route/action state.
* **Usually NOT required:** Item economy, social UI and asset formats unless the transition/event uses them.
* Apply these defaults to the selected section; follow its direct dependencies when scope crosses a boundary.

## Movement, maps and teleportation

* **Definitions / persistence:** Defs MapInfo, MapRegion, MovementInfo, SafeZoneInfo; DB CharacterInfo location; `LibraryCore/Enum.cs` MirDirection.
* **Server:** P `Move`, `Turn`, `Teleport`, `TeleportRing`; `Models/MapObject.cs` CurrentCell/Spawn; `Models/Map.cs`; SEnvir.GetMap.
* **Client / UI:** `Client/Models/UserObject.cs`, `Client/Scenes/GameScene.cs`, Views MapControl/BigMapDialog/MiniMapDialog.
* **Packets / flow:** C.Move/Turn/TeleportRing → SConnection/P validation → S.ObjectMove/ObjectTurn/UserLocation/MapChanged; client predicts local actions and reconciles to server coordinates.
* **Important:** map/cell setters maintain occupancy and visibility; do not assign only a displayed location. A map asset and a MapInfo definition are distinct inputs.
* **Start here:** P Move; server MapObject.cs; UserObject.cs; CConnection.Process(S.ObjectMove).

## Auto-pathing

* **Definitions / state:** Globals.cs AutoPathRoute/AutoPathRouteLeg; map/movement/region definitions; runtime AutoPathState.
* **Server:** `ServerLibrary/Models/Players/PlayerObject.AutoPath.cs` forwards to `Models/AutoPath/AutoPathService.cs`; `AutoPathRoutePlanner.cs` plans routes. `MonsterObject.AutoPath.cs` handles monster pathing separately.
* **Client / UI:** `Client/Scenes/GameScene.AutoPath.cs`; Views AutoPathRouteControl, BigMapDialog, MiniMapDialog.
* **Packets / flow:** C.AutoPathStart/AutoPathWaypoint/AutoPathCancel/AutoPathMoveStarted → service; S.AutoPathChanged → scene routes. Route presentation and movement-start/cancellation state must stay coordinated.
* **Important:** scene cancellation can suppress stale non-empty route updates; server crafting start cancels active auto-path. Do not replace the workflow with client-only path drawing.
* **Start here:** AutoPathService.cs; AutoPathRoutePlanner.cs; GameScene.AutoPath.cs; SConnection auto-path handlers.

## Instances and dungeon finder

* **Definitions / persistence:** Defs InstanceInfo.cs/DungeonInfo.cs/MapInfo.cs; CharacterInfo timers and runtime instance maps.
* **Server / UI:** P JoinInstance/GetInstance/CheckInstanceFreeSpace/SetTimer; SEnvir instance lifecycle; Views DungeonFinderDialog.cs and TimerDialog.cs.
* **Packets / flow:** C.JoinInstance → P checks/entry; map transition uses S.MapChanged and ordinary spawn/location updates; inspect SConnection and P for additional result messaging.
* **Start here:** P JoinInstance/GetInstance; SEnvir.GetMap; InstanceInfo.cs; DungeonFinderDialog.cs.

## Configured world/player/monster events and commands

* **Definitions / state:** Defs EventInfo.cs; server `Envir/Events/EventInfoHandler.cs`, interfaces, Attributes, Actions and Triggers; SEnvir.EventHandler.
* **Flow:** EventInfoHandler discovers concrete trigger/action implementations through interfaces plus attributes and builds name/type mappings. Runtime calls such as time/minute processing invoke configured behavior; client effects depend on the chosen action, not a universal event packet.
* **Commands:** `ServerLibrary/Envir/Commands/PlayerCommandHandler.cs` and neighboring handlers; P.Chat and SEnvir.CommandHandler are starting call sites.
* **Start here:** EventInfoHandler.cs; EventInfo.cs; matching action/trigger implementation; `Server/Views/EventInfoView.cs`. This is separate from PluginCore's editor-extension loader.
