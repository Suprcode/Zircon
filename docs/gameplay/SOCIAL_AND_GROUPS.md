# Social and groups

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

* **Usually required:** Participant state, rights/context checks and affected participants’ updates and cleanup.
* **Usually NOT required:** Spell implementations and map assets unless conquest or appearance changes require them.
* Apply these defaults to the selected section; follow its direct dependencies when scope crosses a boundary.

## Groups and looking for group

* **Definitions / state:** Globals.ClientLookingForGroup and group constants; P GroupMembers/invitations/LFGSettings are runtime coordination.
* **Server / client:** P Group and Looking For Group regions; Views GroupDialog.cs (group health/list UI as well).
* **Packets / flow:** C.GroupInvite/Response/Request/Switch/LFGUpdate → P group/LFG methods; S.GroupMember/Remove/Invite/Request/LFG/Update.
* **Important / start here:** distinguish request/accept/member changes and LFG broadcasts. Start P GroupInvite/GroupJoin/LFGUpdate, SConnection.Process(C.GroupResponse), GroupDialog.cs.

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
