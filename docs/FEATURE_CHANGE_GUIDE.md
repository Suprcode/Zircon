# Feature change guide

Use this guide when change scope is unclear or crosses owners; otherwise go directly from [TASK_ROUTER](TASK_ROUTER.md) or [GAMEPLAY_SYSTEMS](GAMEPLAY_SYSTEMS.md) to the feature. Read one decision tree or matrix row, then the relevant checklist—not this entire file.

## Canonical examples

After choosing a boundary, select one reference from [CANONICAL_EXAMPLES](CANONICAL_EXAMPLES.md); its bounded entry points cover all families below. Reuse the crafting example for a complete packet flow.

## Change decision trees

Choose the branch matching the requested behavior before opening source. Follow more than one branch only when the change crosses those boundaries; the leaves link to the existing detailed guides. Client and server classes with the same name are separate owners.

### Item change

* Is the change only presentation derived from data the client already has?
  * Yes → `Client/Scenes/GameScene.cs: CreateItemLabel` or the owning dialog/`DXItemCell`; keep display-only state client-side. [Client UI](CLIENT_UI.md).
  * No → Is the value shared by every item of this definition?
    * Yes → `LibraryCore/SystemModels/ItemInfo.cs` and System.db. [Definition data](DATA_MODEL.md).
      * Must be editable? → `Server/Views/ItemInfoView.cs` / `.Designer.cs`. [Content editors](CONTENT_AND_EDITORS.md).
      * Must be displayed? → UI reads `ClientUserItem.Info`, resolved by `ClientUserItem.Complete`. [Items and economy](gameplay/ITEMS_AND_ECONOMY.md).
    * No → What lifetime does the instance value need?
      * Persist across reloads → `ServerLibrary/DBModels/UserItem.cs` with MirDB setters. [Persistence](DATA_MODEL.md).
      * Live only on the server → owning `ServerLibrary/Models/PlayerObject.cs` or ground `ItemObject.cs` runtime state, according to its lifetime. [Server runtime](SERVER_RUNTIME.md).
    * Must instance state also reach the client? → For UserItem values, `UserItem.ToClientInfo` → `Globals.cs: ClientUserItem`; trace initial and incremental updates through CConnection. For other runtime state, follow its owning feature's payload. [Networking](NETWORKING.md).

### Monster change

* Is the request about configured content rather than live execution?
  * Monster attributes/AI selector/image identity → `LibraryCore/SystemModels/MonsterInfo.cs`, `MonsterInfoStat.cs`, `Server/Views/MonsterInfoView.cs`. [Content editors](CONTENT_AND_EDITORS.md).
  * Spawn or drop definitions → `RespawnInfo.cs` / `DropInfo.cs`; follow `Map.cs: SpawnInfo.DoSpawn` or server `MonsterObject.Drop` only if execution changes. [Monsters and spawning](gameplay/COMBAT_AND_MAGIC.md#monsters-and-spawning).
* Is it live targeting/combat/AI?
  * Shared behavior → `ServerLibrary/Models/MonsterObject.cs: ProcessSearch/ProcessTarget/ShouldAttackTarget/CanAttackTarget`. [Monster runtime](SERVER_RUNTIME.md#monster-extension-pattern).
  * One specialization → subclass selected by `MonsterObject.GetMonster(MonsterInfo)` (switches on `AI`) and its inherited hooks, such as `Monsters/ZumaKing.cs` → ZumaGuardian. [Monster runtime](SERVER_RUNTIME.md#monster-extension-pattern).
* Is it only appearance/animation/effects? → `Client/Models/MonsterObject.cs`, FrameSet and effect/library references; new action state also needs its S packet path. [Rendering and assets](RENDERING_AND_ASSETS.md), [networking](NETWORKING.md).

### Magic/spell change

* Is it data rather than execution?
  * Shared spell definition → `LibraryCore/SystemModels/MagicInfo.cs`, `Server/Views/MagicInfoView.cs`. [Content editors](CONTENT_AND_EDITORS.md).
  * Learned level/experience → `ServerLibrary/DBModels/UserMagic.cs`; if exposed, `ToClientInfo` → `Globals.cs: ClientUserMagic` and learned-state updates. [Spells and learned magic](gameplay/COMBAT_AND_MAGIC.md#spells-and-learned-magic).
* Is it server behavior?
  * Spell-specific cast/completion/damage → `ServerLibrary/Models/MagicObject.cs` and selected `Models/Magics` class; `Wizard/FireBall.cs: MagicCast/MagicComplete` is an example. [Combat and magic](gameplay/COMBAT_AND_MAGIC.md).
  * Player cast dispatch, setup or learned-spell handling → server `PlayerObject.SetupMagic/Magic/MagicToggle/LevelMagic`. [Spells and learned magic](gameplay/COMBAT_AND_MAGIC.md#spells-and-learned-magic).
* Is it client presentation?
  * Animation/effect → `Client/Models/PlayerObject.cs`, `MirEffect`/`MirProjectile`/`SpellObject`; check the action payload only if existing state is insufficient. [Rendering and assets](RENDERING_AND_ASSETS.md).
  * Skill UI/icon → `Client/Scenes/Views/MagicDialog.cs` / `MagicBarDialog.cs`, `LibraryFile.MagicIcon` for icon images. [Client UI](CLIENT_UI.md), [asset identity](RENDERING_AND_ASSETS.md#library-and-image-identity).

### Map/world change

* Is it configured world data? → `LibraryCore/SystemModels/MapInfo.cs`, MapRegion/MovementInfo/SafeZoneInfo and their Server/Views editors. [World and movement](gameplay/WORLD_AND_MOVEMENT.md).
* Is it live server behavior?
  * Cells, occupancy or object lifecycle → `ServerLibrary/Models/Map.cs: Map/Cell`, `MapObject.CurrentCell/Spawn/Despawn`. [Server runtime](SERVER_RUNTIME.md#object-hierarchy-and-lifetime).
  * Movement/teleport rules → server `PlayerObject.Move/Teleport` and SConnection validation; follow client `UserObject` prediction/CConnection reconciliation when movement semantics change. [Movement flow](gameplay/WORLD_AND_MOVEMENT.md#movement-maps-and-teleportation).
* Is it client drawing or asset content?
  * Draw ordering/overlays → owning `Client/Scenes/Views/MapControl` partial. [Rendering ownership](RENDERING_AND_ASSETS.md).
  * Map file/cell content → `MapInfo.FileName` and server `Map.Load`, plus client map loading; map cells can affect collision as well as appearance. [World and movement](gameplay/WORLD_AND_MOVEMENT.md).
  * Tile/object image only → exact library/image references used by MapControl and the ZL asset; inspect LibraryEditor if authoring images. [Asset formats](RENDERING_AND_ASSETS.md#formats-and-authoring-tools).

### UI change

* Does the request stay local to the client?
  * Layout/display → owning `Client/Scenes/Views` file and existing DX controls. [Client UI](CLIENT_UI.md).
  * Remember window/preference state → `Client/UserModels/WindowSetting.cs` / `KeyBindInfo.cs`, DXWindow and `CEnvir.LoadDatabase` as applicable. [Client preference persistence](DATA_MODEL.md).
  * Change backend drawing/resource behavior → `RenderingCore/Rendering/IRenderingPipeline.cs` and RenderingPipelineManager; ordinary dialog layout stays in DX controls. [Rendering backend](RENDERING_AND_ASSETS.md).
* Does it cross the server boundary?
  * Perform gameplay action → existing or new `LibraryCore/Network/ClientPackets.cs` request via `CEnvir.Enqueue` → `SConnection.Process` → feature owner. [Networking](NETWORKING.md).
  * Display server-owned state → existing client model and `CConnection.Process(S.Type)` → owning dialog; extend the server sender/S payload only if required state is absent. [Networking](NETWORKING.md).

### Persistence change

* Must the value survive reloads?
  * Yes → Which owner stores it?
    * Shared definition → `LibraryCore/SystemModels` in System.db, edited through the System-mode `Server/SMain.cs` session. [Definition ownership](DATA_MODEL.md).
    * Server user state → `ServerLibrary/DBModels` in Users.db through `SEnvir.Session`. [MirDB mechanics](DATA_MODEL.md#mirdb-mechanics).
    * Client MirDB preference → `Client/UserModels` through `CEnvir.LoadDatabase`'s Users-mode session under Data; this is separate from server Users.db. [Client session](DATA_MODEL.md#global-registries-and-load-ownership).
  * No → What owns the value?
    * Server runtime only → owning `ServerLibrary/Models` object/feature and its cleanup, such as PlayerObject's active crafting timer. [Server runtime](SERVER_RUNTIME.md).
    * Local presentation only → owning client model/dialog; no MirDB property required. [Client UI](CLIENT_UI.md).
    * Network representation only → packet properties or transfer structures such as `LibraryCore/Globals.cs: ClientUserItem`; serialization does not make a field persistent. [Networking](NETWORKING.md#wire-compatibility).

## Dependency matrix

Inspection hints, not mandatory edits: **Yes** = direct owner; **Usually** = normal consumer/path; **Maybe** = only under the stated condition; **No** = skip for this scope. Verify source before expanding. Packet inspection includes existing send/receive paths and transfer structures, not necessarily new fields. Shared definition means SystemModels, not every shared enum. MirDB user state includes client preferences; Editor includes content and image tools. UI includes MapControl; Assets includes rendering/library code.

### Items, monsters and magic

Source routes: [items](gameplay/ITEMS_AND_ECONOMY.md), [combat/magic](gameplay/COMBAT_AND_MAGIC.md).

| Change type | Shared definition | Server runtime | MirDB user state | Packet | Client model | UI | Assets | Editor |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| ItemInfo property | Yes | Maybe: rule consumer | No | No: System.db | Maybe: lookup use | Maybe: displayed | Maybe: appearance | Usually |
| UserItem per-instance property | No | Usually | Yes | Maybe: exposed | Maybe: exposed | Maybe: displayed | No | Maybe: user tooling |
| Item movement rule | Maybe: eligibility data | Yes | Usually: ownership/slot | Usually | Usually: grid state | Usually: cells/locks | No | Maybe: definition field |
| Monster targeting rule | Maybe: configurable | Yes | No | Maybe: new action state | Maybe: new action | No | Maybe: new animation | Maybe: configurable |
| MonsterInfo content property | Yes | Usually | No | Maybe: visible runtime state | Maybe: visible state | No | Maybe: appearance | Usually |
| New monster visual | Maybe: image selection | Maybe: new action | No | Maybe: new action state | Yes | No | Yes | Maybe: selection/authoring |
| Spell damage rule | Maybe: configured power | Yes | No | Usually: existing results | Maybe: changed result handling | Maybe: damage hint | No | Maybe: power field |
| New MagicInfo property | Yes | Maybe: rule consumer | No | No: System.db | Maybe: consumer | Maybe: displayed | Maybe: presentation | Usually |
| Learned magic state | No | Yes | Yes: UserMagic | Usually | Usually: ClientUserMagic | Usually: skill display | No | Maybe: user tooling |
| Client-only spell effect | No | No | No | No: existing action | Yes | No | Usually | Maybe: image authoring |

### Progression, world and integration

Source routes: [quests](gameplay/QUESTS_AND_PROGRESSION.md), [crafting](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md), [world](gameplay/WORLD_AND_MOVEMENT.md); UI, persistence, networking and asset owners are linked in the decision trees above.

| Change type | Shared definition | Server runtime | MirDB user state | Packet | Client model | UI | Assets | Editor |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Quest definition | Yes | Usually: checks/rewards | Maybe: progress compatibility | Maybe: new progress payload | Usually: definition use | Usually: quest display | No | Usually |
| Quest progress state | No | Yes | Yes: UserQuest | Usually | Usually | Usually: tracker | No | Maybe: user tooling |
| Crafting rule | Maybe: configured rule | Yes | Maybe: progression/items | Usually | Maybe: mirrored eligibility | Usually: craftability | No | Maybe: rule field |
| New crafting definition | Yes | Usually | Maybe: favourite/progression | Maybe: new state | Usually | Usually | Maybe: new icon | Yes |
| UI-only dialog change | No | No | No | No | No | Yes | Maybe: image/primitive | Maybe: image authoring |
| New gameplay button | Maybe: action data | Yes: action owner | Maybe: saved outcome | Yes: existing/new request | Maybe: result state | Yes | Maybe: image | Maybe: action data |
| Map definition | Yes | Usually | Maybe: saved location compatibility | Maybe: new map state | Maybe: client map data | Usually: MapControl | Maybe: map file/image | Usually |
| Map rendering change | No | No | No | No: existing state | Maybe: actor drawing | Yes: MapControl | Usually | Maybe: image authoring |
| Packet-only protocol addition | No | Yes: server endpoint | No | Yes | Maybe: transfer consumer | No | No | No |
| Client preference/window setting | No | No | Yes: Client/UserModels | No | No | Yes: setting consumer | No | No |
| New image library | No | No | No | No | Maybe: model draw site | Maybe: control draw site | Yes: Libraries/reader | Usually: LibraryEditor |
| New image codec | No | No | No | No | No | Maybe: preview options | Yes: ZL reader/metadata | Yes: paired writer |
| Editor-only presentation change | No | No | No | No | No | No: game UI | No | Yes: Server/Views |

Definition-only rows assume distribution through System.db; if a value becomes per-instance or needs a new runtime payload, also follow that state/packet row. Pure presentation rows assume existing authoritative timing/state; gameplay changes require the corresponding server row.

## Item property → inventory tooltip

### Usually required

* Select definition versus instance state with the [item tree](#item-change); inspect its consumer, conversion/display and editor only as applicable.

### Usually NOT required

* Definition-only fields need no new UserItem/instance-packet fields; instance-only fields need no ItemInfo change. [Item ownership and source anchors](gameplay/ITEMS_AND_ECONOMY.md#inventory-equipment-and-storage).

1. Follow [MirDB rules](DATA_MODEL.md#mirdb-mechanics) for the selected definition/instance model; use ItemInfoStat/Stat for an actual stat.
2. Inspect affected rule consumers: `PlayerObject.ItemUse/CanUseItem/CanWearItem/RefreshStats`, and `SEnvir.CreateFreshItem` for initialization.
3. Audit initial/incremental transfers and copies for client-visible instance state; follow [network compatibility](NETWORKING.md#wire-compatibility).
4. Follow [tooltip anchors](CLIENT_RUNTIME.md#gamescene-and-other-partials) and [editor/distribution paths](CONTENT_AND_EDITORS.md); LibraryEditor edits images.
5. Verify relevant save/load and definition lookup; check shared tooltips in inventory/equipment/storage.

## New spell with visual effect

1. `LibraryCore/SystemModels/MagicInfo.cs`, `LibraryCore/Enum.cs: MagicType`; preserve numeric compatibility and inspect shared frame/action/stat effects.
2. Use the selected spell and [registration/execution rules](gameplay/COMBAT_AND_MAGIC.md#spells-and-learned-magic); `Wizard/FireBall.cs` is the targeted delayed-hit reference.

3. `PlayerObject.Magic`, learned `DBModels/UserMagic.cs`, cooldown/resource validation and delayed actions. Do not apply damage in the client effect callback.
4. `C.Magic → SConnection.Process → PlayerObject.Magic → S.ObjectMagic/ObjectProjectile` and `CConnection.Process` handlers. Add fields only if existing action data cannot express the feature; then audit serialization on both ends.
5. `Client/Models/PlayerObject.cs` magic cases and `MirEffect/MirProjectile/SpellObject`, plus `LibraryCore/FrameSet.cs`, Libraries.cs image mapping and `Client/Envir/DXSoundManager.cs` if audio changes.
6. `Views/MagicDialog.cs`, MagicBarDialog and `Server/Views/MagicInfoView.cs`; author/inspect the icon/effect library in LibraryEditor and distribute assets/System.db.

## Monster change

### Usually required

* Server MonsterObject and the selected subclass for targeting/AI; use the [monster decision tree](#monster-change).

### Usually NOT required

* Client, UI, assets or packets for choosing among existing actions; definition/spawn editing unless configured content changes.

For definition/spawn/drop and packet entry points use [monsters and spawning](gameplay/COMBAT_AND_MAGIC.md#monsters-and-spawning); for targeting hooks and factory selection use [monster runtime](SERVER_RUNTIME.md#monster-extension-pattern). A new attack stage/projectile/death appearance needs its client counterpart even when server damage already works.

## New client action or gameplay feature

Check these questions against the nearest complete feature (recipe crafting is a useful bounded example):

* Shared definition or only request arguments? Does Globals/client system data need a new collection?
* Which server owner validates and mutates state? Which stage/observer/dead/map/item checks apply?
* Persistent state or runtime timer? Which DBObject/association and lifecycle cleanup?
* C packet, public SConnection.Process overload and forwarding method? See [NETWORKING](NETWORKING.md).
* S update(s), public CConnection.Process overload(s), rejection and initial-state paths?
* Correct client model, dialog refresh, observer view and pending-input/lock release?
* Assets/audio, editor fields, definition distribution, localization or user preferences?
* Do death/logout/despawn, cancellation, repeated request and late response leave state consistent?

Packets use reflection; preserve compatible builds and follow [dispatch/wire rules](NETWORKING.md).

## Dialog/button

### Usually required

* **Existing dialog layout:** the owning `Client/Scenes/Views` file and the existing `Client/Controls` types used by its nearby controls; preserve parenting, clipping and cache invalidation.

### Usually NOT required

* `ServerLibrary` or packet definitions unless the interaction changes gameplay; `RenderingCore` unless an existing DX control cannot supply the required drawing primitive/resource behavior.
* GameScene registration, `WindowType` and key bindings for repositioning existing children; inspect these only when window integration changes.

Follow the [dialog integration checklist](CLIENT_UI.md#add-a-button-or-dialog) and DX ownership/cache rules. Keep local actions local; gameplay requests follow the packet/server checklist above.

## Persisted data

Follow [MirDB change/relationship rules](DATA_MODEL.md#mirdb-mechanics) and select a [model example](CANONICAL_EXAMPLES.md#data-models). Review transfers only for client-visible state; verify representative save/load/delete/migration behavior, including deployed data when relevant.

## Image or effect

### Usually required

* **Client-only visual effect:** the owning client model/effect (for example `Client/Models/MonsterObject.cs` and its `MirEffect`/`MirProjectile` construction), exact library/image/frame references, and `LibraryCore/Libraries.cs` mapping if adding a library.

### Usually NOT required

* Server damage logic, MirDB or packet changes when existing action state already drives the effect. Expand to sender/receiver and server actions only when authoritative timing or state changes; a local frame-duration adjustment alone does not require that expansion.
* RenderingCore or image-library writers when reusing existing effects and supported assets; open them only for rendering/format changes.

Follow [asset identity, paired format readers/writers and resource rules](RENDERING_AND_ASSETS.md). Verify the selected image/frame layout for asset edits and graphics/cache behavior for low-level rendering changes.

## Cross-project fan-out reminders

| Value / registration | Other places to inspect |
| --- | --- |
| `Enum.cs: ItemType`, GridType, EquipmentSlot | P item validation/moves/use, CConnection grid switches, DXItemCell, GameScene tooltip, editor enums |
| `Enum.cs: MagicType` | MagicTypeAttribute/SEnvir.MagicTypes/P.SetupMagic, client PlayerObject effects, icons and MagicInfo |
| `Enum.cs: MonsterImage` / MonsterInfo.AI | Image: client MonsterObject images/frames. AI integer: server MonsterObject.GetMonster. Check both fields in MonsterInfo/editor when defining a new monster. |
| `Enum.cs: MirAction/MirAnimation/MirDirection/ObjectType` | client action/frame/direction math, server action creation, packet serialization, object hierarchy |
| `Stat.cs: Stat` | Stats encoding/aggregation, server calculation, tooltip/display and stat editors; preserve values |
| `Libraries.cs: LibraryFile` | enum-to-file map, client library load, draw-site index and distributed library |
| Client `WindowType` / `KeyBindAction` | preference DB, CEnvir default key setup, GameScene/menu window handling |
| Event trigger/action attributes | EventInfoHandler discovery/mappings, SystemModels/EventInfo and EventInfoView |
| Packet subclass/property | Packet discovery IDs + reflected property payload, both Process handlers and send sites |

## Navigation review

Use [TASK_ROUTER](TASK_ROUTER.md) for the first owner and [CANONICAL_EXAMPLES](CANONICAL_EXAMPLES.md) for one representative implementation. Expand only along affected dependencies; source counts are not a reason to omit a necessary boundary.
