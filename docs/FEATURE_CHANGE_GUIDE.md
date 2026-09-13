# Feature change guide

Use [GAMEPLAY_SYSTEMS](GAMEPLAY_SYSTEMS.md) to select a family section and its source entry points, then this guide to identify boundaries. Follow actual dependencies; a new dialog is not automatically a new server feature.

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
  * One specialization → subclass selected by `MonsterObject.GetMonster(MonsterInfo.AI)` and its inherited hooks, such as `Monsters/ZumaKing.cs` → ZumaGuardian. [Monster runtime](SERVER_RUNTIME.md#monster-extension-pattern).
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

`Check` means conditional on the changed semantics, not mandatory edits.

| Change | Shared | Server | Network | Client | UI | Editor/content |
| --- | --- | --- | --- | --- | --- | --- |
| Item definition property shown in tooltip | ItemInfo | Check rule users | Check; definition normally System.db | definition lookup | GameScene tooltip | ItemInfoView + System.db |
| Per-instance item property | ClientUserItem if exposed | UserItem + rule users | conversion/payload | instance state | Check | user-data tooling if editable |
| Spell with new visuals | MagicInfo/MagicType | MagicObject registration/execution | existing action payload or extension | PlayerObject/effects | icons/learned skill | MagicInfoView/assets |
| Monster target algorithm | none normally | base/subclass hooks | Check new observable action | Check new action only | none normally | Check changed configuration |
| Button on existing dialog | none normally | Check action | Check action | event | owning dialog | existing asset or new image |
| New client action | packet and any structures | SConnection + behavior | both endpoints | send/response | Check | Check definitions |
| Recipe behavior | CraftingInfo if rule data changes | crafting partial | Check state payload | CConnection/UserObject | CraftingDialogs | CraftingInfoView |
| New image codec | ZL format metadata | none normally | none normally | reader via RenderingCore | Check preview | LibraryEditor writer |

## Item property → inventory tooltip

### Usually required

* **ItemInfo definition property:** `LibraryCore/SystemModels/ItemInfo.cs` and its actual rule consumers; `Server/Views/ItemInfoView.cs` / `.Designer.cs` if editable, and `Client/Scenes/GameScene.cs: CreateItemLabel` if displayed. Distribute updated System.db definitions.
* **Per-instance UserItem property:** `ServerLibrary/DBModels/UserItem.cs` and the code mutating that value. If exposed to the client, inspect `ToClientInfo`, `LibraryCore/Globals.cs: ClientUserItem` (including copies), initial transfer and runtime item updates in `Client/Envir/CConnection.cs`; inspect the display only if relevant.

### Usually NOT required

* Definition-only fields do not normally need `UserItem` persistence fields or new item-instance packet properties: `ClientUserItem.Complete` resolves `InfoIndex` to the shared definition.
* Instance-only fields do not normally need `ItemInfo` or its editor unless the design also adds a definition-level default/rule. Server-private instance values do not need client transfer fields.

1. Definition property: `LibraryCore/SystemModels/ItemInfo.cs` (or ItemInfoStat/Stat for an actual stat). Copy MirDB setter/attributes. Instance property: `ServerLibrary/DBModels/UserItem.cs` instead; do not put one item's roll into the shared definition.
2. Rule consumers: `ServerLibrary/Models/PlayerObject.cs: ItemUse, CanUseItem, CanWearItem, RefreshStats`; `SEnvir.CreateFreshItem` if initialization changes.
3. Synchronization: `UserItem.ToClientInfo → LibraryCore/Globals.cs: ClientUserItem → CConnection item handlers`. Definition-only values are resolved by InfoIndex in the client's System.db. Per-instance values need explicit conversion/structure updates; audit Packet serialization and all full/incremental updates.
4. UI: `Client/Scenes/GameScene.cs: CreateItemLabel`, GetItemLabelDisplayInfo, AddItemLabelMetadata/AddEquipmentItemInfo/AddItemLabelDescription. InventoryDialog/DXItemCell set the hovered item but do not own all tooltip formatting.
5. Editor: `Server/Views/ItemInfoView.cs` and `.Designer.cs`, stat view/lookups as needed; Helpers/JsonImporter/JsonExporter and definition distribution. **LibraryEditor is for image assets.**
6. Verify persistence load/save and lookup against compatible system data; check hover for inventory/equipment/storage because the tooltip is shared.

## New spell with visual effect

1. `LibraryCore/SystemModels/MagicInfo.cs`, `LibraryCore/Enum.cs: MagicType`; preserve numeric compatibility and inspect shared frame/action/stat effects.
2. `ServerLibrary/Models/Magics/...`: use the closest existing spell. Canonical targeted delayed spell: `Wizard/FireBall.cs`; base `Models/MagicObject.cs` defines casting/completion hooks. Add the matching MagicTypeAttribute and constructor shape used by `PlayerObject.SetupMagic`; `SEnvir.CreateMagic` currently discovers non-abstract **direct subclasses** of MagicObject carrying the attribute. An indirectly derived class will not automatically register.
3. `PlayerObject.Magic`, learned `DBModels/UserMagic.cs`, cooldown/resource validation and delayed actions. Do not apply damage in the client effect callback.
4. `C.Magic → SConnection.Process → PlayerObject.Magic → S.ObjectMagic/ObjectProjectile` and `CConnection.Process` handlers. Add fields only if existing action data cannot express the feature; then audit serialization on both ends.
5. `Client/Models/PlayerObject.cs` magic cases and `MirEffect/MirProjectile/SpellObject`, plus `LibraryCore/FrameSet.cs`, Libraries.cs image mapping and `Client/Envir/DXSoundManager.cs` if audio changes.
6. `Views/MagicDialog.cs`, MagicBarDialog and `Server/Views/MagicInfoView.cs`; author/inspect the icon/effect library in LibraryEditor and distribute assets/System.db.

## Monster change

### Usually required

* **Targeting / aggro:** `ServerLibrary/Models/MonsterObject.cs` search/target/eligibility hooks and the subclass selected by `GetMonster`, including inherited overrides.
* **Server-only AI:** the selected `Models/Monsters` behavior and base hooks it calls; follow server attack/delayed-action or movement code only when those semantics change.

### Usually NOT required

* `Client/Models/MonsterObject.cs`, UI dialogs, `RenderingCore` and packet definitions for choosing among existing targets/actions. Expand to client presentation and action payloads only for new visible state, animation or action semantics.
* `MonsterInfo`, respawn editors and `Map.SpawnInfo` for a behavior-only adjustment unless configuration or spawning itself changes.

Target selection: `ServerLibrary/Models/MonsterObject.cs: GetMonster → selected Models/Monsters subclass → ProcessAI/ProcessSearch/ProperSearch/ProcessTarget/ShouldAttackTarget/CanAttackTarget`. Check subclass overrides first. Small attack example: OmaMage; complex targeting/spawn example: ZumaKing (inherits ZumaGuardian).

New monster definition/behavior: `LibraryCore/SystemModels/MonsterInfo.cs`, `MonsterInfoStat.cs`, `RespawnInfo.cs`; `Map.cs: SpawnInfo.DoSpawn`; MonsterObject.GetMonster factory mapping. New image/action: `LibraryCore/Enum.cs: MonsterImage/MirAnimation`, `Client/Models/MonsterObject.cs`, FrameSet, Libraries and sound mappings. S.ObjectMonster and object action packets carry presentation state. Editors: Server/Views/MonsterInfoView, RespawnInfoView and DropInfoView.

A changed server choice among existing targets/actions usually needs no UI edit. A new attack stage/projectile/death appearance requires its client counterpart even if damage already works.

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

Packet IDs/properties are reflective. There is no central opcode enum or manual handler registration to update; use the concrete Process signature and compatible builds.

## Dialog/button

### Usually required

* **Existing dialog layout:** the owning `Client/Scenes/Views` file and the existing `Client/Controls` types used by its nearby controls; preserve parenting, clipping and cache invalidation.

### Usually NOT required

* `ServerLibrary` or packet definitions unless the interaction changes gameplay; `RenderingCore` unless an existing DX control cannot supply the required drawing primitive/resource behavior.
* GameScene registration, `WindowType` and key bindings for repositioning existing children; inspect these only when window integration changes.

Use [CLIENT_UI](CLIENT_UI.md). Start with the owning Views file, GameScene construction/fields and DXButton/DXControl. Copy a nearby Parent/event/image-state pattern. New windows may require `Client/UserModels/WindowSetting.cs: WindowType`, `KeyBindInfo.cs: KeyBindAction`, CEnvir key setup, MenuDialog and scene key handling. Check DXWindow settings/lifetime; not every view derives from DXWindow.

Keep a purely local button local. A gameplay request follows the packet/server path above. Verify hover/pressed state, clipping, scale, cached-child invalidation and disposal.

## Persisted data

Use [DATA_MODEL](DATA_MODEL.md). Inspect DBMapping/DBValue support, Session assembly discovery and mode, UserObject/IgnoreProperty/Association/MigrationProperty metadata, defaults and setter OnChanged. Use collection CreateNewObject and relationship setters. Check aggregate deletion and the difference between DB identity and Binding position. Review initial/full transfer and incremental packets only for state actually sent to the client.

Canonical linked data: `ServerLibrary/DBModels/UserItem.cs` with owner/stat/socket relationships; shared recipe/ingredient data: `LibraryCore/SystemModels/CraftingInfo.cs`. Test representative save/load/delete/migration behavior when implementation changes warrant it; old deployed data needs its own verification.

## Image or effect

### Usually required

* **Client-only visual effect:** the owning client model/effect (for example `Client/Models/MonsterObject.cs` and its `MirEffect`/`MirProjectile` construction), exact library/image/frame references, and `LibraryCore/Libraries.cs` mapping if adding a library.

### Usually NOT required

* Server damage logic, MirDB or packet changes when existing action state already drives the effect. Expand to sender/receiver and server actions only when authoritative timing or state changes; a local frame-duration adjustment alone does not require that expansion.
* RenderingCore or image-library writers when reusing existing effects and supported assets; open them only for rendering/format changes.

Use [RENDERING_AND_ASSETS](RENDERING_AND_ASSETS.md). Existing library: inspect the exact image and surrounding frame/direction layout, then change the owning DXImageControl/model effect. New library: LibraryFile + Libraries.LibraryList + asset distribution. Format change: pair RenderingCore reader/metadata with LibraryEditor writer. Low-level rendering change: check PipelineFactories/IRenderingPipeline, backend implementation, cache/resource lifecycle and graphics verification.

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

The five target tasks resolve without a repository-wide scan: item/tooltip → first section; spell/effect → spell section; monster targeting → monster section; dialog button → dialog section; client action → networking checklist. Follow those narrow entry points and only expand when an implementation calls a direct dependency not already mapped.
