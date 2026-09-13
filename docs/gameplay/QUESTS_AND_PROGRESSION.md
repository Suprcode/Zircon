# Quests and progression

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

* **Usually required:** Progression/session method, account/character ownership and relevant updates/displays.
* **Usually NOT required:** Rendering internals, monster subclasses and unrelated transaction dialogs.
* Apply these defaults to the selected section; follow its direct dependencies when scope crosses a boundary.

## Quests and tracking

* **Definitions / persistence:** Defs QuestInfo.cs; DB UserQuest.cs (UserQuestTask too), CharacterInfo and AccountInfo associations; Globals.ClientUserQuest.
* **Server / client:** P Quests region and ProcessQuests; Views QuestDialog.cs/QuestTrackerDialog.cs and NPCDialog.cs quest dialogs.
* **Packets / flow:** C.QuestAccept/Complete/Track/Abandon → P → S.QuestChanged/QuestCancelled; client lists/tracker update.
* **Important:** P.Quests combines character and account quests; QuestAccept requires a live NPC context and selects persistence owner according to QuestType.
* **Start here:** P QuestAccept/QuestComplete; QuestInfo.cs; UserQuest.cs; QuestDialog.cs.

## Milestones

* **Definitions / persistence:** Defs MilestoneInfo.cs; DB UserMilestone.cs; Globals.ClientUserMilestone; cross-feature LogMilestone call sites.
* **Server / client:** `Models/PlayerObject.Milestone.cs`: LogMilestone/CheckMilestones/MilestoneClaim; Views QuestDialog.cs includes milestone presentation and MilestoneAchievedDialog.
* **Packets / flow:** C.MilestoneNotify/Active/Claim; S.UserMilestones/MilestoneEarned. Logs/checks can run as consequences of other gameplay rather than a direct request.
* **Start here:** PlayerObject.Milestone.cs; MilestoneInfo.cs; UserMilestone.cs; QuestDialog.cs; Server/Views/MilestoneInfoView.cs.

## Currency, discipline, fame and character progression

* **Definitions / persistence:** Defs CurrencyInfo/BaseStat/DisciplineInfo/FameInfo; DB UserCurrency/UserDiscipline/CharacterInfo; Stat.
* **Server / UI:** P GainExperience/LevelUp/RefreshStats, GetCurrency/CurrencyChanged, IncreaseDiscipline; Views CharacterDialog.cs/CurrencyDialog.cs and Character/FameEffectDecider.cs.
* **Packets / flow:** C.IncreaseDiscipline/Hermit when requested; S.GainedExperience/LevelChanged/StatsUpdate/CurrencyChanged synchronize server progression; other gains are gameplay-driven.
* **Start here:** P relevant method; definition file; corresponding DB model; CharacterDialog.cs. P.AddDefaultCurrencies associates currency rows with Character.Account: check account vs character ownership for each type.

## Login, selection, ranking and observation

* **State / persistence:** DB AccountInfo/CharacterInfo; SConnection Stage/Account/Player/Observed/Observers; SEnvir ranking state.
* **Server / client:** SConnection and SEnvir login/selection/start methods; Client LoginScene.cs/SelectScene.cs; Views RankingDialog.cs, CharacterDialog.cs; GameScene.Observer.
* **Packets / flow:** G handshake; C.Login/NewCharacter/StartGame → S.Login/NewCharacter/StartGame; C.RankRequest/RankSearch/Inspect → S.Rankings/RankSearch/Inspect; C.ObserverRequest/ObservableSwitch → S.StartObserver/ObservableSwitch.
* **Start here:** SConnection relevant Process method; SEnvir.StartGame or RankingSort; CConnection paired Process; LoginScene/RankingDialog. Observation has a separate stage and update forwarding; preserve its gates.
