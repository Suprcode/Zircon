# Task router

After [AGENTS](../AGENTS.md), choose one starting guide by intent. Source names below are anchors, not a list of files to open together; **P** = `ServerLibrary/Models/PlayerObject.cs`.

| Request mentions | Read first | First source area |
| --- | --- | --- |
| test, build, verify, validation, rendering check | [Verification](VERIFICATION.md) | Changed project boundary and relevant focused check entry point |
| item, inventory, equipment, storage | [Items/economy](gameplay/ITEMS_AND_ECONOMY.md) | ItemInfo / UserItem / P item methods |
| drop, pickup, ground loot | [Items/economy](gameplay/ITEMS_AND_ECONOMY.md#ground-drops-pickup-and-loot-display) | P.PickUp / server or client ItemObject |
| spell, magic, buff | [Combat/magic](gameplay/COMBAT_AND_MAGIC.md) | MagicInfo / P.Magic / server MagicObject; MapObject buff methods |
| damage, attack, combat | [Combat/magic](gameplay/COMBAT_AND_MAGIC.md) | P / server MapObject combat methods |
| monster, AI, target, aggro | [Monster runtime](SERVER_RUNTIME.md#monster-extension-pattern) | ServerLibrary/Models/MonsterObject.cs and selected subclass |
| movement, walk, teleport | [World/movement](gameplay/WORLD_AND_MOVEMENT.md) | P.Move/Teleport / server MapObject |
| auto path | [Auto-pathing](gameplay/WORLD_AND_MOVEMENT.md#auto-pathing) | ServerLibrary/Models/AutoPath/AutoPathService.cs |
| map | [World/movement](gameplay/WORLD_AND_MOVEMENT.md) | MapInfo content / server Map / client MapControl by intent |
| quest | [Progression](gameplay/QUESTS_AND_PROGRESSION.md) | QuestInfo / P.QuestAccept/QuestComplete |
| guild | [Social](gameplay/SOCIAL_AND_GROUPS.md#guilds-storage-and-wars) | P.GuildCreate/SendGuildInfo |
| group, party | [Social](gameplay/SOCIAL_AND_GROUPS.md#groups-and-looking-for-group) | P.GroupInvite/GroupJoin |
| crafting | [Activities](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md#recipe-crafting) | ServerLibrary/Models/PlayerObject.Crafting.cs |
| fishing, mining | [Activities](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md#fishing-and-mining) | P.FishingCast/Mining |
| companion, pet | [Companions/pets](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md#companions-combat-pets-and-mounts) | CompanionInfo/UserCompanion, P.CompanionSpawn; server MonsterObject for combat pets |
| button, dialog, window | [Client UI](CLIENT_UI.md) | Client/Scenes/Views owning dialog |
| tooltip | [Client UI](CLIENT_UI.md) | Client/Scenes/GameScene.cs: CreateItemLabel / magic hints |
| client translation, language, localization, local UI/game message or error text | [Client UI](CLIENT_UI.md) | Owning dialog/caller → Client/Envir/Translations/StringMessages.cs and EnglishMessages.cs / ChineseMessages.cs |
| server-generated chat, system message, rejection text | [Server runtime](SERVER_RUNTIME.md) | Sending caller / SConnection.Language → ServerLibrary/Envir/Translations/StringMessages.cs and language implementations |
| NPC dialogue, content text | [Content/editors](CONTENT_AND_EDITORS.md#game-data-editing-is-in-server) | LibraryCore/SystemModels/NPCInfo.cs: NPCPage.Say; Server/Views/NPCPageView.cs / NPCInfoView.cs; other definition text → owning SystemModel/view |
| config, configuration, default setting or option (client/server files) | [Config serialization](DATA_MODEL.md#other-serialization-boundaries) | Client/Envir/Config.cs or ServerLibrary/Envir/Config.cs → LibraryCore/ConfigReader.cs section/key loading and saving |
| persisted client preference, key bind, remembered window position | [Data model](DATA_MODEL.md#first-files-and-boundaries) | Client/UserModels/KeyBindInfo.cs / WindowSetting.cs; Client/Envir/CEnvir.cs client MirDB session, not server Users.db |
| sound, audio, music, effect sound | [Sound/playback ownership](RENDERING_AND_ASSETS.md#resource-lifetime-and-drawing) | Client/Envir/DXSoundManager.cs mappings and triggering caller; DXSound.cs playback/cache; Tools/convert_audio_to_ogg.cmd conversion |
| sprite, image, animation, frame | [Rendering/assets](RENDERING_AND_ASSETS.md) | Client/Models owning model / FrameSet / LibraryFile |
| launcher, patch, updater, client update, patch manifest | [Patching boundary](CONTENT_AND_EDITORS.md#patching-boundary) | Launcher/LMain.cs downloads/launches; Patcher/PMain.cs replaces launcher; PatchManager/PMain.cs publishes manifests/payloads; paired PatchInformation.cs files |
| packet, sync, send, receive | [Networking](NETWORKING.md) | LibraryCore/Network packets / SConnection / CConnection |
| DBObject, persisted, save/load | [Data model](DATA_MODEL.md) | LibraryCore/MirDB / SystemModels / ServerLibrary/DBModels by owner |
| System.db | [Data model](DATA_MODEL.md), then [content flow](CONTENT_AND_EDITORS.md#how-definitions-reach-runtime) if distributing | LibraryCore/SystemModels / Server/Views |
| Users.db | [Data model](DATA_MODEL.md) | ServerLibrary/DBModels; Client/UserModels for client preferences |
| editor field | [Content/editors](CONTENT_AND_EDITORS.md) | Server/Views model view and designer |
| plugin | [Plugins](PLUGINS.md) | PluginCore / Server/SMain.cs host |

## Do not route by keyword alone

Route the requested behavior: **monster image** → client/rendering/content; **monster targeting** → server AI; **item tooltip** → client UI; **item ownership** → server/persistence; **magic icon** → client/rendering; **magic damage** → server combat. Follow the selected guide's source anchors; expand only when the behavior crosses a boundary.

**Inventory window caption** → client UI/localization; **server rejection message** → server translations; **NPC dialogue** → content model/editor. For **monster message**, identify which of these owns the text first. **Sound when a monster attacks** → client sound/effect caller, not server targeting or low-level graphics.

**Default server setting** → server Config; **remember a player's window position** → client preference MirDB. For **window setting**, distinguish persisted layout from a Config option. **Patch image** → determine whether the request changes an asset or its launcher-based distribution.
