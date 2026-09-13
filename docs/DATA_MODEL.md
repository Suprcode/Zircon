# Definitions, instance state and MirDB

## First files and boundaries

| Kind | Authority / consumer |
| --- | --- |
| Shared content definitions | `LibraryCore/SystemModels`; edited by `Server/Views`; read by server simulation and client system database |
| Server persisted state | `ServerLibrary/DBModels`; owned by `SEnvir.Session` and runtime objects |
| Transfer/client structures | `LibraryCore/Globals.cs`: `ClientUserItem`, `ClientUserMagic`, `ClientUserQuest`, `ClientBuffInfo`, etc.; packet serialization |
| Client preferences | `Client/UserModels/{WindowSetting,KeyBindInfo,ChatTabPageSetting,ChatTabControlSetting}.cs`; client MirDB session |
| Configuration | `LibraryCore/ConfigReader.cs`; `Client/Envir/Config.cs`; `ServerLibrary/Envir/Config` sources; separate from game DB |

`Info` is a clue, not a universal classification: `ItemInfo` is a shared definition, but `CharacterInfo` and `MailInfo` are server user data. Check inheritance/attributes and consumers.

## MirDB mechanics

Authority: `LibraryCore/MirDB/{DBObject,ADBCollection,DBCollection,Session,DBMapping,DBValue,DBRelationship,DBBindingList,Attributes}.cs`.

* `DBObject.Index` is persistent identity within its type/collection. `DBCollection<T>.CreateNewObject` increments the collection identity counter, attaches the object, calls `OnCreated`, and adds it to Binding. Do not replace with an unattached `new T` for persisted state.
* `DBCollection<T>[int]` returns `Binding[index]` (position). `Session.GetObject` / collection identity lookup resolves stored Index. Deletions and insertion helpers make these meanings materially different.
* Session discovers DBObject subclasses in explicitly supplied assemblies and creates their collections. `[UserObject]` separates user data from system data; `SessionMode` controls writable categories. System collections use BindingList, user collections List. Read-only collections still participate in reading definitions.
* Setters follow backing-field equality check, assignment, `OnChanged(old,new,nameof(Property))`. `OnChanged` marks modifications after relationship loading, updates inverse links for DBObject references, and conditionally raises property notifications. Copy `LibraryCore/SystemModels/ItemInfo.cs` or `ServerLibrary/DBModels/UserItem.cs`.
* `[Association(identity, aggregate)]` links reference/list ends. `DBObject.CreateLink/RemoveLink` match association metadata; a mismatch can throw. `DBRelationship.ConsumeKeys` resolves serialized type/index references after objects load.
* `DBObject.Delete` delegates to Session. `Session.Delete` follows aggregate relationships, deletes aggregate children and clears links. `FastDelete` is a distinct path; inspect it before choosing it. Removing a Binding entry alone is not equivalent.
* DBMapping and DBValue own property/type mapping; `[IgnoreProperty]` excludes persistence. A `[MigrationProperty(oldName)]` property receives an old unmatched field during reading and is excluded from the new saved mapping. Canonical example: `MagicInfo.LegacyClass` receives old Class data; inspect its OnLoaded conversion to RequiredClass before copying this migration pattern. `[IsIdentity]` marks model identity/display metadata; DBObject.Index remains the stored relationship identity. Do not confuse these attributes with packet `[IgnorePropertyPacket]` or JSON `[JsonIgnore]`.
* `Session.Initialize` reads mappings/records, links relationships, calls collection `OnLoaded`, and saves pending migrations. `Session.Save` / `SaveObjects` and DBCollection change tracking determine writes. `DBObject.Save` caches raw bytes and skips temporary objects.
* Session paths are `System.db`, `Users.db`, `.TMP` intermediate files and `.gz` backups; schema headers and DBValue encoding are a different format from network packets. Definition references are saved as Index (0 for null), not embedded whole objects.

When adding persisted data, verify assembly discovery, system/user classification, property mapping support, defaults/migrations, relationship symmetry and deletion, save/load roundtrip, client export needs and editor fields. A type rename or enum renumbering requires a compatibility review of existing data.

## Definition groups

All paths below are under `LibraryCore/SystemModels/`; several files contain multiple related classes.

| Family | Root files/types |
| --- | --- |
| Items / stats | `ItemInfo.cs`, `ItemInfoStat.cs`, `SetInfo.cs`, `SetInfoStat.cs`, `BaseStat.cs`, `CurrencyInfo.cs` |
| Maps / travel / instances | `MapInfo.cs`, `MapRegion.cs`, `MovementInfo.cs`, `SafeZoneInfo.cs`, `InstanceInfo.cs`, `DungeonInfo.cs` |
| Monsters / spawning / drops | `MonsterInfo.cs`, `MonsterInfoStat.cs`, `RespawnInfo.cs`, `DropInfo.cs`, `GuardInfo.cs` |
| Magic | `MagicInfo.cs`; cross-project `MagicType` enum and `Stat` |
| NPC scripts / quests / help | `NPCInfo.cs` (NPCPage and command/value definitions too), `QuestInfo.cs`, `HelpInfo.cs` |
| Progression / rewards | `MilestoneInfo.cs`, `DisciplineInfo.cs`, `FameInfo.cs`, `EventInfo.cs` |
| Crafting / fishing / mining | `CraftingInfo.cs` (level/recipe/ingredient types), `WeaponCraftStatsInfo.cs`, `FishingInfo.cs`, `MineInfo.cs` |
| Companions | `CompanionInfo.cs`, `CompanionLevelInfo.cs`, `CompanionSkillInfo.cs`, `CompanionSpeech.cs` |
| Castle content | `CastleInfo.cs`, `CastleGateInfo.cs`, `CastleGuardInfo.cs`, `CastleFlagInfo.cs` |
| Shops / reward containers | `StoreInfo.cs`, `BundleInfo.cs`, `LootBoxInfo.cs` |
| DB version | `SystemDatabaseInfo.cs`; Session system-version handling |

## Definition → instance → representation

| Definition | Server state / runtime | Client |
| --- | --- | --- |
| ItemInfo | `DBModels/UserItem.cs` references Info, holds quantity/slot/durability and linked stats/sockets; `Models/ItemObject.cs` is ground presence | `Globals.cs: ClientUserItem`; `Client/Models/ItemObject.cs` ground drawing; DXItemCell/UI |
| MonsterInfo | `Models/MonsterObject.cs` live AI; `RespawnInfo` drives spawning, not a persisted copy of the live monster | `Client/Models/MonsterObject.cs` visual actor |
| MagicInfo | `DBModels/UserMagic.cs` learned state; `Models/MagicObject.cs` and Magics subclasses execute | `ClientUserMagic`; PlayerObject effect/animation cases |
| QuestInfo | `DBModels/UserQuest.cs` and UserQuestTask; PlayerObject progression | `ClientUserQuest` / quest dialogs |
| CraftingRecipeInfo | CharacterInfo crafting level/experience/favourite; PlayerObject active timer is runtime state | UserObject crafting fields and CraftingDialogs |
| No shared CharacterInfo | `DBModels/CharacterInfo.cs` persistent character → `Models/PlayerObject.cs` live session | `Client/Models/UserObject.cs` local user and PlayerObject for actors |

`UserItem.ToClientInfo` and `ClientUserItem.InfoIndex/Info` are the critical item boundary: the `[CompleteObject]` method `ClientUserItem.Complete` resolves Info against `Globals.ItemInfoList` after packet reading. A new definition-only property can arrive through updated System.db without adding an instance packet field. A new per-instance property needs conversion and packet-structure consideration.

## Global registries and load ownership

`LibraryCore/Globals.cs` contains shared-definition collection references plus constants and transfer types. It is process-local static state, not a shared server/client memory store. `Client/Envir/CEnvir.cs: LoadDatabase` creates a Users-mode session under `Data`, initializes LibraryCore and client preference assemblies, assigns Globals collections, then marks loading complete. Its system definitions are read-only through that mode; client preferences are user data.

`ServerLibrary/Envir/SEnvir.cs` owns simulation Session and its explicit system/user collection fields (`CharacterInfoList`, `UserItemList`, etc.). `Server/SMain.cs` creates its own System-mode editor Session. Do not treat editor bindings as the live simulation's object references; inspect save/reload behavior for a proposed live-edit change.

Needs verification: compatibility with any particular deployed System.db/Users.db requires those database files and a migration roundtrip; source inspection alone cannot establish their contents.

## Other serialization boundaries

* Session reads/writes through `LibraryCore/Encryption.cs`. With a key configured, its writer uses AES and prepends an IV; the reader uses its existing header detection and requires the key for encrypted input. `Server/Views/DatabaseEncryptionForm.cs` initializes a Both-mode session and rewrites using the selected key. This is database encoding, not game-packet encryption. Type/namespace changes must account for the reader's header detection as well as DBMapping type names.
* `LibraryCore/ConfigReader.cs` discovers `[ConfigPath]` classes, `[ConfigSection]` properties and `[ConfigPropertyIgnore]`, reading/writing section/key text. Renaming a config property/section affects the file key; inspect its conversion and culture handling, not MirDB attributes. Canonical declarations: `Client/Envir/Config.cs`, `ServerLibrary/Envir/Config.cs`.
* Image containers and patch manifests have independent formats and paired readers/writers; see [RENDERING_AND_ASSETS](RENDERING_AND_ASSETS.md) and [CONTENT_AND_EDITORS](CONTENT_AND_EDITORS.md).
