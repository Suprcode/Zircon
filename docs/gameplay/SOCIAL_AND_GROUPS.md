# Social and groups

[Gameplay router](../GAMEPLAY_SYSTEMS.md)

## Reading this guide

Paths are repository-relative: **P** = `ServerLibrary/Models/PlayerObject.cs`; **Defs** = `LibraryCore/SystemModels/`; **DB** = `ServerLibrary/DBModels/`; **Views** = `Client/Scenes/Views/`.

Start at the selected feature's anchors; packet lists are entry points, not exhaustive protocols. C/S denote client/server senders; dispatch and transfer structures: [NETWORKING](../NETWORKING.md). Follow other boundaries only as needed via the [guide index](../README.md); editor counterparts: [CONTENT_AND_EDITORS](../CONTENT_AND_EDITORS.md).

## Change boundaries

* **Usually required:** Participant state, rights/context checks and affected participants’ updates and cleanup.
* **Usually NOT required:** Spell implementations and map assets unless conquest or appearance changes require them.
* Apply these defaults to the selected section; follow its direct dependencies when scope crosses a boundary.

## Groups and looking for group

* **Definitions / state:** Globals.ClientLookingForGroup/ClientGroupLootInfo and group constants; P GroupMembers/invitations/LFGSettings coordinate membership while `PlayerObject.GroupLoot.cs` owns the runtime shared bag, balanced member carry-weight allocation, bagless award queue and active distribution. `Config.EnableGroupLoot` controls server-wide availability and is sent in ClientUser at login. CharacterInfo persists each character's preferred mode, filters, Enable Bag choice, Need restriction and manual-taking permission; solo players can edit these defaults, a new group initializes its shared settings from its leader, and other members see those active settings read-only.
* **Server / client:** P Group and Looking For Group regions, `PlayerObject.GroupLoot.cs`, ItemObject pickup routing; Views GroupDialog.cs and GroupLootDialogs.cs.
* **Packets / flow:** C.GroupInvite/Response/Request/Switch/LFGUpdate cover membership; C.GroupLootSettings/Share/Take/Vote → the group-loot partial → S.GroupLootUpdate/VotePrompt/Result.
* **Important / start here:** distinguish request/accept/member changes, LFG broadcasts and server-authoritative loot ownership. With group loot enabled, each monster makes one ordinary group drop roll and snapshots every account in the group at death onto the resulting ground items, allowing those members to see and collect the drops even if they enter the area later. Later joiners are not included. The roll uses the credited kill owner's drop modifiers and fortune record, while personal quest rewards remain limited to members who were nearby at death. Any snapshotted member can collect a nonmatching item directly. With Enable Bag selected, matching items enter the shared group bag; Random, Need/Greed and Round Robin use Share, while Free For All lets any member immediately claim them. With Enable Bag cleared, Random and Round Robin award matching pickups immediately, Need/Greed prompts immediately through a server-owned queue, and Free For All gives the item directly to its collector. Harvest rewards remain per-character. When manual taking is enabled for retained bag contents in the non-Free For All modes, any member can claim an item while no Share is active if their inventory can receive it, and the result is announced to the group. Optional Need restrictions require the item to be currently equippable and are enforced both when prompting and resolving votes. Ordinary ground currency is immediately divided across all current group members after guild tax, independently of the optional shared bag; quest-bound currency retains its normal pickup handling. Start P GroupJoin/GroupLeave, `PlayerObject.GroupLoot.cs`, SConnection group handlers and GroupLootDialogs.cs.

## Guilds, storage and wars

* **Definitions / persistence:** DB GuildInfo.cs, GuildMemberInfo.cs, GuildWarInfo.cs, UserItem ownership; Globals.ClientGuildInfo; shared CastleInfo for conquest.
* **Server / client:** P Guild region and item-grid movement; SEnvir.CheckGuildWars; Views GuildDialog.cs.
* **Packets / flow:** C.GuildCreate/EditNotice/EditMember/InviteMember/Response/War → P; S.GuildInfo/Update/Invite/Stats/NewItem/GetItem/WarStarted/WarFinished and member/funds changes.
* **Start here:** P GuildCreate/SendGuildInfo; DB GuildInfo.cs; GuildDialog.cs; SConnection guild handlers. Check rights and both participants' updates in the specific operation before changing behavior.

## Conquest and castles

* **Definitions / persistence:** Defs CastleInfo/Gate/Guard/Flag; DB UserConquest/UserConquestStats and GuildInfo.
* **Server / client:** `Models/ConquestWar.cs`, SEnvir.StartConquest, P GuildConquest/repair/toggle methods; Models/Monsters castle actors; Views GuildDialog.cs and world models.
* **Packets / flow:** C.GuildRequestConquest/GuildToggleCastleGates/GuildRepairCastleGates/GuildRepairCastleGuards; S.GuildConquestDate/Started/Finished/GuildCastleInfo plus actor updates.
* **Start here:** ConquestWar.cs; P GuildConquest; CastleInfo.cs; GuildDialog.cs. Scheduling and world objects are server-owned; client dialog only presents/request changes.

## Mail, chat, friends and blocks

* **Definitions / persistence:** Globals ClientMailInfo/ClientFriendInfo/ClientBlockInfo; DB MailInfo, FriendInfo, BlockInfo, linked UserItem.
* **Server / UI:** P Mail and Communication regions; SConnection friend/block handlers; Views CommunicationDialog.cs, ChatTab.cs and ChatTextBox.cs.
* **Packets / flow:** C.MailSend/GetItem/Delete/Opened → S.MailSend/List/New/Delete/ItemDelete; C.Chat → P.Chat → S.Chat; C.FriendAdd/Remove and BlockAdd/Remove → matching S changes.
* **Start here:** P MailSend or Chat; SConnection relevant handler; CommunicationDialog.cs; DB MailInfo.cs. Client chat tabs/settings are separate persisted client preferences.

## Marriage, appearance and fortune

* **Definitions / persistence:** DB CharacterInfo, UserItem, UserFortuneInfo; ItemInfo; Globals.ClientFortuneInfo.
* **Server / UI:** P Marriage region and relevant SConnection appearance/fortune handlers; Views NPCDialog.cs wedding-ring UI, EditCharacterDialog.cs, FortuneCheckerDialog.cs; client PlayerObject equipment/shape rendering.
* **Packets / flow:** C.MarriageResponse/MakeRing/Teleport → S.MarriageInvite/Info/MakeRing; C.HairChange/ArmourDye/NameChange/CaptionChange and FortuneCheck → feature responses and appearance updates.
* **Start here:** P MarriageJoin/MarriageMakeRing or SConnection requested handler; DB CharacterInfo; relevant dialog; client PlayerObject for appearance.
