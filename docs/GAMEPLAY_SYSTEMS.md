# Gameplay fast router

Choose a feature, read its linked family section, then inspect its **Start here** anchors. Source is authoritative; expand to other guides only when the change crosses a boundary.

Paths are repository-relative. **P** = `ServerLibrary/Models/PlayerObject.cs`; **SC** = `ServerLibrary/Envir/SConnection.cs`; **SEnvir** = `ServerLibrary/Envir/SEnvir.cs`; **Views** = `Client/Scenes/Views/`; **CEnvir** = `Client/Envir/CEnvir.cs`.

| Feature | Detailed doc | Main server entry | Main client/UI entry |
| --- | --- | --- | --- |
| Inventory, equipment and storage | [ITEMS_AND_ECONOMY](gameplay/ITEMS_AND_ECONOMY.md#inventory-equipment-and-storage) | `P.ItemMove` | `Client/Controls/DXItemCell.cs` |
| Ground drops, pickup and loot display | [ITEMS_AND_ECONOMY](gameplay/ITEMS_AND_ECONOMY.md#ground-drops-pickup-and-loot-display) | `P.PickUp; ServerLibrary/Models/ItemObject.cs` | `Client/Scenes/Views/MapControl.Loot.cs` |
| NPC scripts, shops and refinement | [ITEMS_AND_ECONOMY](gameplay/ITEMS_AND_ECONOMY.md#npc-scripts-shops-and-refinement) | `ServerLibrary/Models/NPCObject.cs; P.NPCCall` | `Views/NPCDialog.cs` |
| Trade | [ITEMS_AND_ECONOMY](gameplay/ITEMS_AND_ECONOMY.md#trade) | `SC.Process(C.TradeConfirm)` | `Views/TradeDialog.cs` |
| Marketplace and game store | [ITEMS_AND_ECONOMY](gameplay/ITEMS_AND_ECONOMY.md#marketplace-and-game-store) | `SC.Process(C.MarketPlaceSearch); P.MarketPlaceBuy` | `Views/ConsignmentDialog.cs; Views/GameStoreDialog.cs` |
| Loot boxes and bundles | [ITEMS_AND_ECONOMY](gameplay/ITEMS_AND_ECONOMY.md#loot-boxes-and-bundles) | `P.LootBoxOpen / BundleOpen` | `Views/LootBoxDialog.cs; Views/BundleDialog.cs` |
| Combat, HP/MP, buffs and death | [COMBAT_AND_MAGIC](gameplay/COMBAT_AND_MAGIC.md#combat-hpmp-buffs-and-death) | `P.Attacked / MagicAttack` | `Client/Models/MapObject.cs` |
| Spells and learned magic | [COMBAT_AND_MAGIC](gameplay/COMBAT_AND_MAGIC.md#spells-and-learned-magic) | `P.Magic; ServerLibrary/Models/MagicObject.cs` | `Client/Models/PlayerObject.cs; Views/MagicDialog.cs` |
| Monsters and spawning | [COMBAT_AND_MAGIC](gameplay/COMBAT_AND_MAGIC.md#monsters-and-spawning) | `ServerLibrary/Models/MonsterObject.cs: GetMonster / ProcessTarget` | `Client/Models/MonsterObject.cs` |
| Movement, maps and teleportation | [WORLD_AND_MOVEMENT](gameplay/WORLD_AND_MOVEMENT.md#movement-maps-and-teleportation) | `P.Move; ServerLibrary/Models/MapObject.cs` | `Client/Models/UserObject.cs` |
| Auto-pathing | [WORLD_AND_MOVEMENT](gameplay/WORLD_AND_MOVEMENT.md#auto-pathing) | `ServerLibrary/Models/AutoPath/AutoPathService.cs` | `Client/Scenes/GameScene.AutoPath.cs` |
| Instances and dungeon finder | [WORLD_AND_MOVEMENT](gameplay/WORLD_AND_MOVEMENT.md#instances-and-dungeon-finder) | `P.JoinInstance / GetInstance` | `Views/DungeonFinderDialog.cs` |
| Configured world/player/monster events and commands | [WORLD_AND_MOVEMENT](gameplay/WORLD_AND_MOVEMENT.md#configured-worldplayermonster-events-and-commands) | `ServerLibrary/Envir/Events/EventInfoHandler.cs; ServerLibrary/Envir/Commands/PlayerCommandHandler.cs` | `Depends on selected action; no universal event packet` |
| Quests and tracking | [QUESTS_AND_PROGRESSION](gameplay/QUESTS_AND_PROGRESSION.md#quests-and-tracking) | `P.QuestAccept / QuestComplete` | `Views/QuestDialog.cs; Views/QuestTrackerDialog.cs` |
| Milestones | [QUESTS_AND_PROGRESSION](gameplay/QUESTS_AND_PROGRESSION.md#milestones) | `ServerLibrary/Models/PlayerObject.Milestone.cs` | `Views/QuestDialog.cs` |
| Currency, discipline, fame and character progression | [QUESTS_AND_PROGRESSION](gameplay/QUESTS_AND_PROGRESSION.md#currency-discipline-fame-and-character-progression) | `P.GainExperience / CurrencyChanged / IncreaseDiscipline` | `Views/CharacterDialog.cs; Views/CurrencyDialog.cs` |
| Login, selection, ranking and observation | [QUESTS_AND_PROGRESSION](gameplay/QUESTS_AND_PROGRESSION.md#login-selection-ranking-and-observation) | `SC.Process; SEnvir.StartGame / RankingSort` | `Client/Scenes/LoginScene.cs; Client/Scenes/SelectScene.cs; Views/RankingDialog.cs` |
| Groups and looking for group | [SOCIAL_AND_GROUPS](gameplay/SOCIAL_AND_GROUPS.md#groups-and-looking-for-group) | `P.GroupInvite / GroupJoin / LFGUpdate` | `Views/GroupDialog.cs` |
| Guilds, storage and wars | [SOCIAL_AND_GROUPS](gameplay/SOCIAL_AND_GROUPS.md#guilds-storage-and-wars) | `P.GuildCreate / SendGuildInfo` | `Views/GuildDialog.cs` |
| Conquest and castles | [SOCIAL_AND_GROUPS](gameplay/SOCIAL_AND_GROUPS.md#conquest-and-castles) | `ServerLibrary/Models/ConquestWar.cs; P.GuildConquest` | `Views/GuildDialog.cs` |
| Mail, chat, friends and blocks | [SOCIAL_AND_GROUPS](gameplay/SOCIAL_AND_GROUPS.md#mail-chat-friends-and-blocks) | `P.MailSend / Chat; SC friend/block handlers` | `Views/CommunicationDialog.cs; Views/ChatTab.cs` |
| Marriage, appearance and fortune | [SOCIAL_AND_GROUPS](gameplay/SOCIAL_AND_GROUPS.md#marriage-appearance-and-fortune) | `P.MarriageJoin / MarriageMakeRing; SC appearance/fortune handlers` | `Views/NPCDialog.cs; Views/EditCharacterDialog.cs; Views/FortuneCheckerDialog.cs` |
| Recipe crafting | [CRAFTING_COMPANIONS_AND_ACTIVITIES](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md#recipe-crafting) | `ServerLibrary/Models/PlayerObject.Crafting.cs` | `Views/CraftingDialogs.cs` |
| Companions, combat pets and mounts | [CRAFTING_COMPANIONS_AND_ACTIVITIES](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md#companions-combat-pets-and-mounts) | `P.CompanionAdopt / CompanionSpawn` | `Views/CompanionDialog.cs; Views/HorseTameDialog.cs` |
| Fishing and mining | [CRAFTING_COMPANIONS_AND_ACTIVITIES](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md#fishing-and-mining) | `P.FishingCast / Mining` | `Views/FishingDialog.cs; Client/Models/UserObject.cs` |
| Configuration, keys and help | [CRAFTING_COMPANIONS_AND_ACTIVITIES](gameplay/CRAFTING_COMPANIONS_AND_ACTIVITIES.md#configuration-keys-and-help) | `Server/Views/HelpInfoView.cs for help content; preferences local` | `Client/UserModels/KeyBindInfo.cs; CEnvir.GetKeyAction` |

## Scope

These family guides group related systems rather than documenting every spell, monster, packet or NPC command. For an unlisted sub-operation, start at its family handler/model in the linked guide and follow direct calls. Runtime details that depend on a deployed definition database, asset contents or installation configuration require those inputs; see each specialist guide's Needs verification notes.
