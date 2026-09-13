# Feature change guide

Use [GAMEPLAY_SYSTEMS](GAMEPLAY_SYSTEMS.md) to select a family section and its source entry points, then this guide to identify boundaries. Follow actual dependencies; a new dialog is not automatically a new server feature.

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
