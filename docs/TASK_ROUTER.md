# Task router

After [AGENTS](../AGENTS.md), choose one starting guide by intent. Source names below are anchors, not a list of files to open together; **P** = `ServerLibrary/Models/PlayerObject.cs`.

| Request mentions | Read first | First source area |
| --- | --- | --- |
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
| sprite, image, animation, frame | [Rendering/assets](RENDERING_AND_ASSETS.md) | Client/Models owning model / FrameSet / LibraryFile |
| packet, sync, send, receive | [Networking](NETWORKING.md) | LibraryCore/Network packets / SConnection / CConnection |
| DBObject, persisted, save/load | [Data model](DATA_MODEL.md) | LibraryCore/MirDB / SystemModels / ServerLibrary/DBModels by owner |
| System.db | [Data model](DATA_MODEL.md), then [content flow](CONTENT_AND_EDITORS.md#how-definitions-reach-runtime) if distributing | LibraryCore/SystemModels / Server/Views |
| Users.db | [Data model](DATA_MODEL.md) | ServerLibrary/DBModels; Client/UserModels for client preferences |
| editor field | [Content/editors](CONTENT_AND_EDITORS.md) | Server/Views model view and designer |
| plugin | [Plugins](PLUGINS.md) | PluginCore / Server/SMain.cs host |

## Do not route by keyword alone

Route the requested behavior: **monster image** → client/rendering/content; **monster targeting** → server AI; **item tooltip** → client UI; **item ownership** → server/persistence; **magic icon** → client/rendering; **magic damage** → server combat. Follow the selected guide's source anchors; expand only when the behavior crosses a boundary.
