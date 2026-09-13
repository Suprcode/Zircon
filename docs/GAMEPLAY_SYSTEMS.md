# Gameplay navigation catalogue

## Reading this catalogue

Paths are repository-relative. To avoid repeating long paths in every entry:

* **P** = `ServerLibrary/Models/PlayerObject.cs`; partial paths are written explicitly. Regions/methods are mapped in [SERVER_RUNTIME](SERVER_RUNTIME.md).
* **Defs** = `LibraryCore/SystemModels/`; **DB** = `ServerLibrary/DBModels/`; **Views** = `Client/Scenes/Views/`.
* C packets are in `LibraryCore/Network/ClientPackets.cs`, handled by `ServerLibrary/Envir/SConnection.cs: Process(C.Type)`; S packets are in ServerPackets.cs, handled by `Client/Envir/CConnection.cs: Process(S.Type)`. [NETWORKING](NETWORKING.md) traces dispatch and complete examples.
* Packet lists are significant entry points, not exhaustive protocols. Shared transfer structures (`ClientUserItem`, etc.) are in `LibraryCore/Globals.cs`.
* **Start** names the first 2–5 files/anchors; inspect the listed definition/DB/packet counterparts when their boundary changes. Editor counterparts are in [CONTENT_AND_EDITORS](CONTENT_AND_EDITORS.md).

## Inventory, equipment and storage

* **Purpose / definitions:** item ownership, slots, use and equipment; Defs `ItemInfo.cs`, `ItemInfoStat.cs`, `SetInfo.cs`; `LibraryCore/Enum.cs` GridType, EquipmentSlot, ItemType.
* **Server / persistence:** P `Items` region: `ItemMove`, `ItemUse`, `CanWearItem`, `CanGainItems`, `GainItem`, `ParseLinks`; DB `UserItem.cs`, `UserItemStat.cs`, `UserItemSocket.cs`, CharacterInfo/AccountInfo ownership.
* **Client / UI:** `Client/Controls/DXItemCell.cs`, DXItemGrid; Views `InventoryDialog.cs`, `CharacterDialog.cs`, `StorageDialog.cs`; `GameScene.CreateItemLabel` owns the common tooltip.
* **Packets / flow:** C.ItemMove/ItemUse/ItemSort/ItemSplit/ItemLock → P validation and instance changes → S.ItemMove/ItemSort/ItemChanged/ItemsGained/ItemStatsChanged/ItemDurability. Client handlers choose grids by GridType and update/release pending cells.
* **Rules:** definition data and instance data cross different boundaries; ClientUserItem resolves InfoIndex from Globals. Equipment appearance also uses S.PlayerUpdate. Storage shares item movement paths, not a separate universal storage packet.
* **Start:** Defs ItemInfo.cs; DB UserItem.cs; P ItemMove; DXItemCell.cs; GameScene.CreateItemLabel.

## Ground drops, pickup and loot display

* **Definitions / state:** Defs `DropInfo.cs`, `ItemInfo.cs`, `CurrencyInfo.cs`; DB UserItem/UserDrop.
* **Server:** `ServerLibrary/Models/MonsterObject.cs: Drop`; P `ItemDrop`, `PickUp`, `DeathDrop`; `Models/ItemObject.cs` ground lifetime.
* **Client / UI:** `Client/Models/ItemObject.cs`; Views `MapControl.Loot.cs` and `MapControl.Names.cs`; GroundLootPiles/GroundItemLabels/ItemHighlights/LootEffect under Client/Models.
* **Packets / flow:** C.ItemDrop/PickUp/CurrencyDrop → server ground/item changes; S.ObjectItem/ObjectRemove and ItemsGained/CurrencyChanged synchronize presence and gains. Changing ground labels does not change server ownership.
* **Start:** P PickUp; server ItemObject.cs; client ItemObject.cs; MapControl.Loot.cs. Focused checks: `Tests/GroundLootChecks`.

## Movement, maps and teleportation

* **Definitions / persistence:** Defs MapInfo, MapRegion, MovementInfo, SafeZoneInfo; DB CharacterInfo location; `LibraryCore/Enum.cs` MirDirection.
* **Server:** P `Move`, `Turn`, `Teleport`, `TeleportRing`; `Models/MapObject.cs` CurrentCell/Spawn; `Models/Map.cs`; SEnvir.GetMap.
* **Client / UI:** `Client/Models/UserObject.cs`, `Client/Scenes/GameScene.cs`, Views MapControl/BigMapDialog/MiniMapDialog.
* **Packets / flow:** C.Move/Turn/TeleportRing → SConnection/P validation → S.ObjectMove/ObjectTurn/UserLocation/MapChanged; client predicts local actions and reconciles to server coordinates.
* **Rules:** map/cell setters maintain occupancy and visibility; do not assign only a displayed location. A map asset and a MapInfo definition are distinct inputs.
* **Start:** P Move; server MapObject.cs; UserObject.cs; CConnection.Process(S.ObjectMove).

## Auto-pathing

* **Definitions / state:** Globals.cs AutoPathRoute/AutoPathRouteLeg; map/movement/region definitions; runtime AutoPathState.
* **Server:** `ServerLibrary/Models/Players/PlayerObject.AutoPath.cs` forwards to `Models/AutoPath/AutoPathService.cs`; `AutoPathRoutePlanner.cs` plans routes. `MonsterObject.AutoPath.cs` handles monster pathing separately.
* **Client / UI:** `Client/Scenes/GameScene.AutoPath.cs`; Views AutoPathRouteControl, BigMapDialog, MiniMapDialog.
* **Packets / flow:** C.AutoPathStart/AutoPathWaypoint/AutoPathCancel/AutoPathMoveStarted → service; S.AutoPathChanged → scene routes. Route presentation and movement-start/cancellation state must stay coordinated.
* **Rules:** scene cancellation can suppress stale non-empty route updates; server crafting start cancels active auto-path. Do not replace the workflow with client-only path drawing.
* **Start:** AutoPathService.cs; AutoPathRoutePlanner.cs; GameScene.AutoPath.cs; SConnection auto-path handlers.

## Combat, HP/MP, buffs and death

* **Definitions / state:** `LibraryCore/Stat.cs`, Enum.cs MagicType/BuffType/PoisonType; DB BuffInfo, CharacterInfo, UserMagic.
* **Server:** P `Combat`, `RefreshStats`, HP/MP methods, `Die`; Models MapObject damage/buff/poison methods, MagicObject and MonsterObject; DelayedAction.
* **Client / UI:** Models PlayerObject/MonsterObject/UserObject/MapObject, MirEffect; Views BuffDialog and health displays.
* **Packets / flow:** C.Attack/RangeAttack/Magic → server attacks; S.ObjectAttack/ObjectMagic/ObjectStruck/ObjectDied, HealthChanged/ManaChanged/StatsUpdate, BuffAdd/Remove/Changed and ObjectBuffAdd/Remove update local displays and visible actors.
* **Rules:** server computes combat results; animation and health display do not determine damage. Visible object buffs and user buff details have separate update packets.
* **Start:** P Attacked/MagicAttack; MapObject.cs; the relevant MagicObject subclass; CConnection response handler.

## Spells and learned magic

* **Definitions / persistence:** Defs MagicInfo.cs; Enum.cs MagicType; DB UserMagic.cs; Globals.ClientUserMagic.
* **Server:** P SetupMagic/Magic/MagicToggle/LevelMagic; SEnvir.MagicTypes; `Models/MagicObject.cs`; implementations in `Models/Magics` class folders.
* **Client / UI:** Models PlayerObject.cs magic/effect cases, UserObject.cs action input; Views MagicDialog.cs/MagicBarDialog.cs; FrameSet and Libraries.
* **Packets / flow:** C.Magic/MagicToggle/MagicKey → execution/learned-state handling; S.ObjectMagic/ObjectProjectile/NewMagic/MagicLeveled/MagicCooldown.
* **Rules:** server implementation registration uses MagicTypeAttribute, while visual cases are separate. `Models/Magics/Wizard/FireBall.cs` is a canonical targeted delayed hit: MagicCast schedules DelayMagic; MagicComplete applies damage.
* **Start:** MagicInfo.cs; FireBall.cs or the relevant spell; P SetupMagic/Magic; client PlayerObject.cs; MagicDialog.cs.

## Monsters and spawning

* **Definitions:** Defs MonsterInfo.cs/MonsterInfoStat.cs, RespawnInfo.cs, DropInfo.cs; Enum.cs MonsterImage.
* **Server:** Models MonsterObject.GetMonster, ProcessAI/Search/Target/ShouldAttackTarget; Models/Monsters specialization; Map.cs SpawnInfo.DoSpawn.
* **Client:** Models MonsterObject.cs and FrameSet; represented by S.ObjectMonster/ObjectMove/ObjectAttack/ObjectMagic/ObjectDied/ObjectRemove, without a client spawn request.
* **Persistence / rules:** MonsterInfo/RespawnInfo describe content; a live MonsterObject is simulation state. Target choice belongs on the server; subclass overrides may supersede base hooks.
* **Start:** MonsterObject.GetMonster; selected subclass; MonsterObject.ProcessSearch/ProcessTarget. For new visuals also client MonsterObject.cs.

## NPC scripts, shops and refinement

* **Definitions / state:** Defs NPCInfo.cs contains NPCPage/check/action/value structures; ItemInfo, WeaponCraftStatsInfo; DB GameNPCData.cs and RefineInfo.cs.
* **Server:** Models NPCObject.cs executes page checks/actions/values; P NPCCall/NPCButton and NPCBuy/NPCSell/NPCRepair/NPCRefine/NPCWeaponCraft.
* **Client / UI:** Views NPCDialog.cs (many related dialogs in one file), NPCSocketDialog.cs, NPCSocketCombineDialog.cs; client NPCObject for world drawing.
* **Packets / flow:** C.NPCCall/NPCButton → server page execution → S.NPCResponse (page index + values) or NPCClose; transaction requests use C.NPCBuy/NPCSell/NPCRefine/NPCSocketItem and corresponding item/NPC updates.
* **Rules:** a client button does not authorize a transaction. NPCObject checks pages before actions and can follow success/failure pages. Recipe crafting below is a different system from NPCWeaponCraft.
* **Start:** NPCObject.cs; P NPCCall/NPCButton or transaction method; Defs NPCInfo.cs; Views NPCDialog.cs; `Server/Views/NPCPageView.cs`.

## Recipe crafting

* **Definitions:** Defs CraftingInfo.cs: CraftingRecipeInfo, CraftingIngredientInfo, CraftingLevelInfo; ItemInfo; Stat.CraftingSuccess.
* **Server / persistence:** `ServerLibrary/Models/PlayerObject.Crafting.cs`; CharacterInfo crafting level/experience/favourite recipe; P.Process invokes ProcessCrafting.
* **Client / UI:** Views CraftingDialogs.cs and CharacterDialog.cs; UserObject crafting progression; GameScene registers recipe/progress boxes. Editor: Server/Views/CraftingInfoView.cs and designer.
* **Packets / flow:** C.CraftingStart → StartCrafting → S.CraftingStarted; ProcessCrafting on completion → item/currency changes and S.CraftingEnded. C.CraftingSetFavourite → S.CraftingState; C.CraftingCancel ends current work.
* **Rules:** server checks recipe/materials/output capacity before starting and again at completion; consumes ingredients/gold before rolling success. Valid designs have 1–5 distinct positive ingredient entries. Death/despawn cancels; auto-path is cancelled at start.
* **Start:** PlayerObject.Crafting.cs; CraftingInfo.cs; CraftingDialogs.cs; CConnection crafting handlers; CraftingInfoView.cs.

## Quests and tracking

* **Definitions / persistence:** Defs QuestInfo.cs; DB UserQuest.cs (UserQuestTask too), CharacterInfo and AccountInfo associations; Globals.ClientUserQuest.
* **Server / client:** P Quests region and ProcessQuests; Views QuestDialog.cs/QuestTrackerDialog.cs and NPCDialog.cs quest dialogs.
* **Packets / flow:** C.QuestAccept/Complete/Track/Abandon → P → S.QuestChanged/QuestCancelled; client lists/tracker update.
* **Rules:** P.Quests combines character and account quests; QuestAccept requires a live NPC context and selects persistence owner according to QuestType.
* **Start:** P QuestAccept/QuestComplete; QuestInfo.cs; UserQuest.cs; QuestDialog.cs.

## Milestones

* **Definitions / persistence:** Defs MilestoneInfo.cs; DB UserMilestone.cs; Globals.ClientUserMilestone; cross-feature LogMilestone call sites.
* **Server / client:** `Models/PlayerObject.Milestone.cs`: LogMilestone/CheckMilestones/MilestoneClaim; Views QuestDialog.cs includes milestone presentation and MilestoneAchievedDialog.
* **Packets / flow:** C.MilestoneNotify/Active/Claim; S.UserMilestones/MilestoneEarned. Logs/checks can run as consequences of other gameplay rather than a direct request.
* **Start:** PlayerObject.Milestone.cs; MilestoneInfo.cs; UserMilestone.cs; QuestDialog.cs; Server/Views/MilestoneInfoView.cs.

## Groups and looking for group

* **Definitions / state:** Globals.ClientLookingForGroup and group constants; P GroupMembers/invitations/LFGSettings are runtime coordination.
* **Server / client:** P Group and Looking For Group regions; Views GroupDialog.cs (group health/list UI as well).
* **Packets / flow:** C.GroupInvite/Response/Request/Switch/LFGUpdate → P group/LFG methods; S.GroupMember/Remove/Invite/Request/LFG/Update.
* **Rules / start:** distinguish request/accept/member changes and LFG broadcasts. Start P GroupInvite/GroupJoin/LFGUpdate, SConnection.Process(C.GroupResponse), GroupDialog.cs.

## Guilds, storage and wars

* **Definitions / persistence:** DB GuildInfo.cs, GuildMemberInfo.cs, GuildWarInfo.cs, UserItem ownership; Globals.ClientGuildInfo; shared CastleInfo for conquest.
* **Server / client:** P Guild region and item-grid movement; SEnvir.CheckGuildWars; Views GuildDialog.cs.
* **Packets / flow:** C.GuildCreate/EditNotice/EditMember/InviteMember/Response/War → P; S.GuildInfo/Update/Invite/Stats/NewItem/GetItem/WarStarted/WarFinished and member/funds changes.
* **Start:** P GuildCreate/SendGuildInfo; DB GuildInfo.cs; GuildDialog.cs; SConnection guild handlers. Check rights and both participants' updates in the specific operation before changing behavior.

## Conquest and castles

* **Definitions / persistence:** Defs CastleInfo/Gate/Guard/Flag; DB UserConquest/UserConquestStats and GuildInfo.
* **Server / client:** `Models/ConquestWar.cs`, SEnvir.StartConquest, P GuildConquest/repair/toggle methods; Models/Monsters castle actors; Views GuildDialog.cs and world models.
* **Packets / flow:** C.GuildRequestConquest/GuildToggleCastleGates/GuildRepairCastleGates/GuildRepairCastleGuards; S.GuildConquestDate/Started/Finished/GuildCastleInfo plus actor updates.
* **Start:** ConquestWar.cs; P GuildConquest; CastleInfo.cs; GuildDialog.cs. Scheduling and world objects are server-owned; client dialog only presents/request changes.

## Trade

* **State / persistence:** P TradeItems and partner state; transferred DB UserItem/UserCurrency, not a separate TradeInfo record.
* **Server / UI:** P trade methods reached by SConnection trade handlers; Views TradeDialog.cs and DXItemCell.
* **Packets / flow:** C.TradeRequest/RequestResponse/AddItem/AddGold/Confirm/Close → server; S.TradeRequest/Open/AddItem/ItemAdded/AddGold/GoldAdded/Unlock/Close.
* **Start:** SConnection.Process(C.TradeConfirm); P matching trade method; TradeDialog.cs; UserItem.cs. Preserve confirmation/unlock and both peers' cleanup when the offered contents change.

## Marketplace and game store

* **Definitions / persistence:** Defs StoreInfo; DB AuctionInfo/AuctionHistoryInfo, GameStoreFavourite/GameStoreSale, UserItem, UserCurrency.
* **Server / UI:** P MarketPlace region; SConnection marketplace search handlers; Views ConsignmentDialog.cs and GameStoreDialog.cs.
* **Packets / flow:** C.MarketPlaceSearch/Consign/Buy/CancelConsign/StoreBuy, GameStoreFavouriteToggle/GameStoreGift; S.MarketPlaceSearch/Consign/Buy/StoreBuy, GameStoreData/TopItems/FavouriteChanged/Gift.
* **Start:** SConnection.Process(C.MarketPlaceSearch); P MarketPlaceBuy; AuctionInfo.cs; ConsignmentDialog.cs. Search/result state also lives on SConnection, not solely PlayerObject.

## Mail, chat, friends and blocks

* **Definitions / persistence:** Globals ClientMailInfo/ClientFriendInfo/ClientBlockInfo; DB MailInfo, FriendInfo, BlockInfo, linked UserItem.
* **Server / UI:** P Mail and Communication regions; SConnection friend/block handlers; Views CommunicationDialog.cs, ChatTab.cs and ChatTextBox.cs.
* **Packets / flow:** C.MailSend/GetItem/Delete/Opened → S.MailSend/List/New/Delete/ItemDelete; C.Chat → P.Chat → S.Chat; C.FriendAdd/Remove and BlockAdd/Remove → matching S changes.
* **Start:** P MailSend or Chat; SConnection relevant handler; CommunicationDialog.cs; DB MailInfo.cs. Client chat tabs/settings are separate persisted client preferences.

## Companions, combat pets and mounts

* **Definitions / persistence:** Defs CompanionInfo/LevelInfo/SkillInfo/Speech; DB UserCompanion/UserCompanionUnlock/CompanionFilters; combat pets also use MonsterObject/PetOwner.
* **Server / UI:** P Companions region, Mount/Taming; MonsterObject pet behavior; Views CompanionDialog.cs and HorseTameDialog.cs.
* **Packets / flow:** C.CompanionAdopt/Unlock/Retrieve/Store/Release/SendCompanionFilters → S.CompanionAdopt/Unlock/Retrieve/Store/Release/Update/SkillUpdate. C.ChangePetMode, Mount, Taming/TamingSuccess use separate pet/mount paths and S.ChangePetMode/ObjectMount/ObjectTaming.
* **Start:** P CompanionAdopt/CompanionSpawn; UserCompanion.cs; CompanionDialog.cs; MonsterObject.ProcessAI for combat pets. Do not treat every kind of pet as the companion inventory system.

## Fishing and mining

* **Definitions / persistence:** Defs FishingInfo.cs, MineInfo.cs and item stats; rewards use ordinary items/currency.
* **Server / UI:** P FishingCast/UseBait/Mining; Views FishingDialog.cs; UserObject/MapControl action input.
* **Packets / flow:** C.FishingCast/Mining → server action checks; S.ObjectFishing/FishingStats/ObjectMining plus ordinary rewards. FishingCast includes client-reported state/caught input; inspect its validations rather than claiming the server simulates every part of the minigame.
* **Start:** P FishingCast or Mining; FishingInfo.cs/MineInfo.cs; FishingDialog.cs; SConnection action handler.

## Instances and dungeon finder

* **Definitions / persistence:** Defs InstanceInfo.cs/DungeonInfo.cs/MapInfo.cs; CharacterInfo timers and runtime instance maps.
* **Server / UI:** P JoinInstance/GetInstance/CheckInstanceFreeSpace/SetTimer; SEnvir instance lifecycle; Views DungeonFinderDialog.cs and TimerDialog.cs.
* **Packets / flow:** C.JoinInstance → P checks/entry; map transition uses S.MapChanged and ordinary spawn/location updates; inspect SConnection and P for additional result messaging.
* **Start:** P JoinInstance/GetInstance; SEnvir.GetMap; InstanceInfo.cs; DungeonFinderDialog.cs.

## Currency, discipline, fame and character progression

* **Definitions / persistence:** Defs CurrencyInfo/BaseStat/DisciplineInfo/FameInfo; DB UserCurrency/UserDiscipline/CharacterInfo; Stat.
* **Server / UI:** P GainExperience/LevelUp/RefreshStats, GetCurrency/CurrencyChanged, IncreaseDiscipline; Views CharacterDialog.cs/CurrencyDialog.cs and Character/FameEffectDecider.cs.
* **Packets / flow:** C.IncreaseDiscipline/Hermit when requested; S.GainedExperience/LevelChanged/StatsUpdate/CurrencyChanged synchronize server progression; other gains are gameplay-driven.
* **Start:** P relevant method; definition file; corresponding DB model; CharacterDialog.cs. P.AddDefaultCurrencies associates currency rows with Character.Account: check account vs character ownership for each type.

## Loot boxes and bundles

* **Definitions / persistence:** Defs LootBoxInfo.cs/BundleInfo.cs; DB UserItem and CharacterInfo; Globals.ClientLootBoxItemInfo/ClientBundleItemInfo.
* **Server / UI:** P Loot Boxes/Bundles regions; Views LootBoxDialog.cs/BundleDialog.cs.
* **Packets / flow:** C.LootBoxOpen/Reroll/ConfirmSelection/Reveal/TakeItems, C.BundleOpen/Confirm → P corresponding methods and S loot-box/bundle responses plus inventory updates.
* **Start:** P LootBoxOpen/LootBoxConfirm or BundleOpen/BundleConfirm; definition file; matching dialog. Preserve multi-step selection/reveal state when changing reward logic.

## Marriage, appearance and fortune

* **Definitions / persistence:** DB CharacterInfo, UserItem, UserFortuneInfo; ItemInfo; Globals.ClientFortuneInfo.
* **Server / UI:** P Marriage region and relevant SConnection appearance/fortune handlers; Views NPCDialog.cs wedding-ring UI, EditCharacterDialog.cs, FortuneCheckerDialog.cs; client PlayerObject equipment/shape rendering.
* **Packets / flow:** C.MarriageResponse/MakeRing/Teleport → S.MarriageInvite/Info/MakeRing; C.HairChange/ArmourDye/NameChange/CaptionChange and FortuneCheck → feature responses and appearance updates.
* **Start:** P MarriageJoin/MarriageMakeRing or SConnection requested handler; DB CharacterInfo; relevant dialog; client PlayerObject for appearance.

## Login, selection, ranking and observation

* **State / persistence:** DB AccountInfo/CharacterInfo; SConnection Stage/Account/Player/Observed/Observers; SEnvir ranking state.
* **Server / client:** SConnection and SEnvir login/selection/start methods; Client LoginScene.cs/SelectScene.cs; Views RankingDialog.cs, CharacterDialog.cs; GameScene.Observer.
* **Packets / flow:** G handshake; C.Login/NewCharacter/StartGame → S.Login/NewCharacter/StartGame; C.RankRequest/RankSearch/Inspect → S.Rankings/RankSearch/Inspect; C.ObserverRequest/ObservableSwitch → S.StartObserver/ObservableSwitch.
* **Start:** SConnection relevant Process method; SEnvir.StartGame or RankingSort; CConnection paired Process; LoginScene/RankingDialog. Observation has a separate stage and update forwarding; preserve its gates.

## Configuration, keys and help

* **Definitions / persistence:** ConfigReader and client/server Config.cs; client UserModels KeyBindInfo/WindowSetting/ChatTab settings; Defs HelpInfo.cs.
* **Client / UI:** CEnvir.GetKeyAction/CheckKeyBinds, GameScene key handling; Controls DXConfigWindow/DXKeyBindWindow; Views HelpDialog.cs.
* **Flow:** preferences are loaded/saved locally; HelpInfo comes through system data. A key can trigger an existing gameplay packet but adding a key binding does not inherently require a new protocol.
* **Start:** Client/UserModels/KeyBindInfo.cs; CEnvir.GetKeyAction; owning scene/dialog; Server/Views/HelpInfoView.cs for help content.

## Configured world/player/monster events and commands

* **Definitions / state:** Defs EventInfo.cs; server `Envir/Events/EventInfoHandler.cs`, interfaces, Attributes, Actions and Triggers; SEnvir.EventHandler.
* **Flow:** EventInfoHandler discovers concrete trigger/action implementations through interfaces plus attributes and builds name/type mappings. Runtime calls such as time/minute processing invoke configured behavior; client effects depend on the chosen action, not a universal event packet.
* **Commands:** `ServerLibrary/Envir/Commands/PlayerCommandHandler.cs` and neighboring handlers; P.Chat and SEnvir.CommandHandler are starting call sites.
* **Start:** EventInfoHandler.cs; EventInfo.cs; matching action/trigger implementation; `Server/Views/EventInfoView.cs`. This is separate from PluginCore's editor-extension loader.

## Scope

This catalogue groups related systems rather than documenting every spell, monster, packet or NPC command. For an unlisted sub-operation, start at its family handler/model above and follow direct calls. Runtime details that depend on a deployed definition database, asset contents or installation configuration require those inputs; see each specialist guide's Needs verification notes.
