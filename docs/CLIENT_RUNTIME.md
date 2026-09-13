# Client runtime

## Entry, globals and loop

`Client/Program.cs` loads configuration, constructs `TargetForm`, populates `CEnvir.LibraryList` from `LibraryCore/Libraries.cs`, initializes the rendering pipeline with fallback, and runs `CEnvir.GameLoop` through the pipeline message loop. Shutdown unloads client resources and shuts down rendering.

`CEnvir` owns Target, Connection, Now/input modifier state, loaded libraries, client Session, preferences, storage arrays, language and database readiness/version fields. `LoadDatabase` runs background loading; inspect Loaded/Loading gates before using Globals collections. `GameScene.Game` identifies the live game scene; `DXControl.ActiveScene` also covers LoginScene and SelectScene. Client MapObject static target/user accessors point into the active GameScene; they are not server state.

```text
CEnvir.GameLoop
  ApplyPendingPipelineSwitch (may return for this frame)
  UpdateRealtime → Now, mouse routing, connection.Process, cache maintenance
  UpdateSimulation → active scene.Process
  RenderGame → RenderingPipelineManager.RenderFrame(active scene.Draw)
  optional frame-rate limit
```

## Same name, different ownership

| ServerLibrary type | Client type | Synchronization / boundary |
| --- | --- | --- |
| `Models/PlayerObject` validates gameplay and owns CharacterInfo/session state | `Models/PlayerObject` draws players; `Models/UserObject` adds local input/action state | S.ObjectPlayer, S.StartGame, S.ObjectMove, S.PlayerUpdate; C.Move/Attack/Magic requests |
| `Models/MonsterObject` executes AI/combat | `Models/MonsterObject` draws body/frames/effects | S.ObjectMonster, S.ObjectAttack/Magic, S.ObjectDied |
| `Models/MapObject` cell, stats, visibility and delayed combat | `Models/MapObject` position for presentation, action queues, animation, labels and effects | object-action/state packets |
| `Models/ItemObject` ground availability/ownership | `Models/ItemObject` ground rendering/labels | S.ObjectItem, S.ObjectRemove; C.PickUp |
| `Models/NPCObject` server NPC presence | `Models/NPCObject` NPC drawing; Views/NPCDialog interaction | S.ObjectNPC, S.NPCResponse; C.NPCCall/NPCButton |
| `Models/SpellObject` persistent world spell processing | `Models/SpellObject` visual spell actor | S.ObjectSpell, S.ObjectSpellChanged, S.ObjectRemove |
| `Models/Map` server movement cells/population | `Scenes/Views/MapControl` map rendering/input/object list | S.MapChanged plus object packets; not equivalent classes |

## Object and animation lifecycle

`Client/Envir/CConnection.cs: Process(S.ObjectPlayer/ObjectMonster/ObjectNPC/ObjectItem/ObjectSpell)` constructs the corresponding model using packet data. The models register with `GameScene.Game.MapControl.Objects`. `Process(S.ObjectRemove)` finds the object by ObjectID and calls `Remove`, including selection/display cleanup. Map changes and scene disposal are larger cleanup boundaries; inspect those when retaining references.

`Models/MapObject.cs` owns `ActionQueue` of `Models/ObjectAction.cs`, CurrentAction/Direction/CurrentLocation, Frames, CurrentFrame, FrameStart, FrameIndex and DrawFrame. `Process → UpdateFrame` advances `LibraryCore/FrameSet.cs` frame definitions using CEnvir.Now and action timing. PlayerObject/MonsterObject choose animation and image offsets; `MirEffect`, `MirProjectile`, `MirLineEffect`, `SpellObject` and particle sources handle different visual mechanisms.

CConnection queues remote actors' ObjectActions. Local actions can already be predicted: `Process(S.ObjectTurn/ObjectMove)` treats the non-observer user separately, compares server coordinates/direction, and uses scene displacement/correction rather than blindly replaying the remote animation. Changing movement requires both `UserObject` input timing and server `PlayerObject.Move`, not just frame counts.

## GameScene and other partials

| Source | Responsibility / anchors |
| --- | --- |
| `Client/Scenes/GameScene.cs` | Scene singleton/user/observer, construction of all dialog boxes, processing/input, item/magic hints, inventory interactions, disposal |
| `Client/Scenes/GameScene.AutoPath.cs` | Route state, pending cancellation, movement presentation, progress tracking; `SetAutoPathRoutes`, `ApplyAutoPathRoutes`, `UpdateAutoPathProgress` |
| `Client/Scenes/Views/MapControl.cs` | World rendering, map tiles/objects, input/action selection |
| `Client/Scenes/Views/MapControl.Loot.cs` | Ground-loot rendering integration; helpers in `Client/Models/GroundLootPiles.cs`, `GroundItemLabels.cs`, `ItemHighlights.cs`, `LootEffect.cs` |
| `Client/Scenes/Views/MapControl.Names.cs` | World-name overlay drawing/cache: `DrawWorldNames`, `DrawWorldNamesDirect`, `ReleaseWorldNames` |

Main GameScene file entry points:

* Dialog registration: constructor assignments such as InventoryBox, CharacterBox, CraftingRecipeBox and CraftingProgressBox.
* Item tooltip: `MouseItem → CreateItemLabel`; `GetItemLabelDisplayInfo`, `AddItemLabelMetadata`, `AddEquipmentItemInfo`, `AddPotionItemInfo`, `AddItemLabelRequirements`, `AddItemLabelDescription`. This tooltip is not owned solely by InventoryDialog.
* Scene lifetime: `Dispose(bool)` and user/observer setters.
* Auto-path drawing: BigMapDialog/MiniMapDialog and `Views/AutoPathRouteControl.cs`; authoritative planning lives in server AutoPathService.

## State authority anchors

| State | Server authority to inspect | Client role / packets |
| --- | --- | --- |
| Position | PlayerObject.Move, MapObject.CurrentCell | UserObject prediction, CConnection reconciliation; S.ObjectMove/UserLocation |
| HP/MP/combat | MapObject/PlayerObject damage and HP/MP methods, MagicObject | display/animation; S.HealthChanged/ManaChanged/ObjectStruck/ObjectDied |
| Items/equipment | PlayerObject.ItemMove/ItemUse/CanWearItem, UserItem | client grids, lock/pending feedback; S.ItemMove/ItemsGained/ItemChanged/PlayerUpdate |
| Buffs | MapObject.BuffAdd/BuffRemove and BuffInfo | BuffDialog and visual effects; S.BuffAdd/Remove/Changed, S.ObjectBuffAdd/Remove |
| Quest progress | PlayerObject quest methods/UserQuest | list/tracker presentation; S.QuestChanged/QuestCancelled |
| Cooldowns | server action/magic deadlines | UserObject presentation/input gating; S.MagicCooldown and object-action timing |
| UI layout/keys | no server ownership implied | client UserModels and CEnvir.Session |

Canonical feature: crafting's server partial, `CConnection.Process(S.CraftingState/Started/Ended)`, UserObject crafting fields and `Views/CraftingDialogs.cs`. It demonstrates shared definition lookup, server validation and UI progress without moving game rules into the dialog.
