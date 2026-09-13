# Combat and magic

[Gameplay router](../GAMEPLAY_SYSTEMS.md)

## Reading this guide

Paths are repository-relative. To avoid repeating long paths in every entry:

* **P** = `ServerLibrary/Models/PlayerObject.cs`; partial paths are written explicitly. Regions/methods are mapped in [SERVER_RUNTIME](../SERVER_RUNTIME.md).
* **Defs** = `LibraryCore/SystemModels/`; **DB** = `ServerLibrary/DBModels/`; **Views** = `Client/Scenes/Views/`.
* C packets are in `LibraryCore/Network/ClientPackets.cs`, handled by `ServerLibrary/Envir/SConnection.cs: Process(C.Type)`; S packets are in ServerPackets.cs, handled by `Client/Envir/CConnection.cs: Process(S.Type)`. [NETWORKING](../NETWORKING.md) traces dispatch and complete examples.
* Packet lists are significant entry points, not exhaustive protocols. Shared transfer structures (`ClientUserItem`, etc.) are in `LibraryCore/Globals.cs`.
* **Start here** names the first 2–5 files/anchors; inspect the listed definition/DB/packet counterparts when their boundary changes. Editor counterparts are in [CONTENT_AND_EDITORS](../CONTENT_AND_EDITORS.md).

Boundary guides, only as needed: [NETWORKING](../NETWORKING.md), [DATA_MODEL](../DATA_MODEL.md), [SERVER_RUNTIME](../SERVER_RUNTIME.md), [CLIENT_RUNTIME](../CLIENT_RUNTIME.md), [CLIENT_UI](../CLIENT_UI.md), [RENDERING_AND_ASSETS](../RENDERING_AND_ASSETS.md).

## Change boundaries

* **Usually required:** Server combat hooks or the selected spell/monster subclass; client action handling when visible behavior changes.
* **Usually NOT required:** Economy and social UI; client visuals for targeting changes using existing actions.
* Apply these defaults to the selected section; follow its direct dependencies when scope crosses a boundary.

## Combat, HP/MP, buffs and death

* **Definitions / state:** `LibraryCore/Stat.cs`, Enum.cs MagicType/BuffType/PoisonType; DB BuffInfo, CharacterInfo, UserMagic.
* **Server:** P `Combat`, `RefreshStats`, HP/MP methods, `Die`; Models MapObject damage/buff/poison methods, MagicObject and MonsterObject; DelayedAction.
* **Client / UI:** Models PlayerObject/MonsterObject/UserObject/MapObject, MirEffect; Views BuffDialog and health displays.
* **Packets / flow:** C.Attack/RangeAttack/Magic → server attacks; S.ObjectAttack/ObjectMagic/ObjectStruck/ObjectDied, HealthChanged/ManaChanged/StatsUpdate, BuffAdd/Remove/Changed and ObjectBuffAdd/Remove update local displays and visible actors.
* **Important:** server computes combat results; animation and health display do not determine damage. Visible object buffs and user buff details have separate update packets.
* **Start here:** P Attacked/MagicAttack; MapObject.cs; the relevant MagicObject subclass; CConnection response handler.

## Spells and learned magic

* **Definitions / persistence:** Defs MagicInfo.cs; Enum.cs MagicType; DB UserMagic.cs; Globals.ClientUserMagic.
* **Server:** P SetupMagic/Magic/MagicToggle/LevelMagic; SEnvir.MagicTypes; `Models/MagicObject.cs`; implementations in `Models/Magics` class folders.
* **Client / UI:** Models PlayerObject.cs magic/effect cases, UserObject.cs action input; Views MagicDialog.cs/MagicBarDialog.cs; FrameSet and Libraries.
* **Packets / flow:** C.Magic/MagicToggle/MagicKey → execution/learned-state handling; S.ObjectMagic/ObjectProjectile/NewMagic/MagicLeveled/MagicCooldown.
* **Important:** server implementation registration uses MagicTypeAttribute, while visual cases are separate. `Models/Magics/Wizard/FireBall.cs` is a canonical targeted delayed hit: MagicCast schedules DelayMagic; MagicComplete applies damage.
* **Start here:** MagicInfo.cs; FireBall.cs or the relevant spell; P SetupMagic/Magic; client PlayerObject.cs; MagicDialog.cs.

## Monsters and spawning

### Usually required

* **Targeting / aggro:** `ServerLibrary/Models/MonsterObject.cs: ProcessSearch/ProperSearch/ProcessTarget/ShouldAttackTarget/CanAttackTarget` and relevant overrides in the subclass selected by `GetMonster` (including inherited hooks).
* **Server-only AI:** selected subclass execution and the server base hooks it calls; inspect spawn/delayed-action paths only if the behavior changes them.

### Usually NOT required

* `Client/Models/MonsterObject.cs`, UI dialogs, RenderingCore and packet definitions for a server choice among existing targets/actions. New visible state or animation/action semantics require tracing the client representation and existing S action payloads.
* MonsterInfo/RespawnInfo editors for targeting code alone; expand to definitions when configurable rules or spawn content change.

* **Definitions:** Defs MonsterInfo.cs/MonsterInfoStat.cs, RespawnInfo.cs, DropInfo.cs; Enum.cs MonsterImage.
* **Server:** Models MonsterObject.GetMonster, ProcessAI/Search/Target/ShouldAttackTarget; Models/Monsters specialization; Map.cs SpawnInfo.DoSpawn.
* **Client:** Models MonsterObject.cs and FrameSet; represented by S.ObjectMonster/ObjectMove/ObjectAttack/ObjectMagic/ObjectDied/ObjectRemove, without a client spawn request.
* **Persistence / rules:** MonsterInfo/RespawnInfo describe content; a live MonsterObject is simulation state. Target choice belongs on the server; subclass overrides may supersede base hooks.
* **Start here:** MonsterObject.GetMonster; selected subclass; MonsterObject.ProcessSearch/ProcessTarget. For new visuals also client MonsterObject.cs.
