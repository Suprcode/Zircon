# Crafting, companions and activities

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

* **Usually required:** Selected activity handler, definitions, reward/progression owner and input/dialog path.
* **Usually NOT required:** Unrelated social systems or graphics internals; new packets for local preferences.
* Apply these defaults to the selected section; follow its direct dependencies when scope crosses a boundary.

## Recipe crafting

* **Definitions:** Defs CraftingInfo.cs: CraftingRecipeInfo, CraftingIngredientInfo, CraftingLevelInfo; ItemInfo; Stat.CraftingSuccess.
* **Server / persistence:** `ServerLibrary/Models/PlayerObject.Crafting.cs`; CharacterInfo crafting level/experience/favourite recipe; P.Process invokes ProcessCrafting.
* **Client / UI:** Views CraftingDialogs.cs and CharacterDialog.cs; UserObject crafting progression; GameScene registers recipe/progress boxes. Editor: Server/Views/CraftingInfoView.cs and designer.
* **Packets / flow:** C.CraftingStart → StartCrafting → S.CraftingStarted; ProcessCrafting on completion → item/currency changes and S.CraftingEnded. C.CraftingSetFavourite → S.CraftingState; C.CraftingCancel ends current work.
* **Important:** server checks recipe/materials/output capacity before starting and again at completion; consumes ingredients/gold before rolling success. Valid designs have 1–5 distinct positive ingredient entries. Death/despawn cancels; auto-path is cancelled at start.
* **Start here:** PlayerObject.Crafting.cs; CraftingInfo.cs; CraftingDialogs.cs; CConnection crafting handlers; CraftingInfoView.cs.

## Companions, combat pets and mounts

* **Definitions / persistence:** Defs CompanionInfo/LevelInfo/SkillInfo/Speech; DB UserCompanion/UserCompanionUnlock/CompanionFilters; combat pets also use MonsterObject/PetOwner.
* **Server / UI:** P Companions region, Mount/Taming; MonsterObject pet behavior; Views CompanionDialog.cs and HorseTameDialog.cs.
* **Packets / flow:** C.CompanionAdopt/Unlock/Retrieve/Store/Release/SendCompanionFilters → S.CompanionAdopt/Unlock/Retrieve/Store/Release/Update/SkillUpdate. C.ChangePetMode, Mount, Taming/TamingSuccess use separate pet/mount paths and S.ChangePetMode/ObjectMount/ObjectTaming.
* **Start here:** P CompanionAdopt/CompanionSpawn; UserCompanion.cs; CompanionDialog.cs; MonsterObject.ProcessAI for combat pets. Do not treat every kind of pet as the companion inventory system.

## Fishing and mining

* **Definitions / persistence:** Defs FishingInfo.cs, MineInfo.cs and item stats; rewards use ordinary items/currency.
* **Server / UI:** P FishingCast/UseBait/Mining; Views FishingDialog.cs; UserObject/MapControl action input.
* **Packets / flow:** C.FishingCast/Mining → server action checks; S.ObjectFishing/FishingStats/ObjectMining plus ordinary rewards. FishingCast includes client-reported state/caught input; inspect its validations rather than claiming the server simulates every part of the minigame.
* **Start here:** P FishingCast or Mining; FishingInfo.cs/MineInfo.cs; FishingDialog.cs; SConnection action handler.

## Configuration, keys and help

* **Definitions / persistence:** ConfigReader and client/server Config.cs; client UserModels KeyBindInfo/WindowSetting/ChatTab settings; Defs HelpInfo.cs.
* **Client / UI:** CEnvir.GetKeyAction/CheckKeyBinds, GameScene key handling; Controls DXConfigWindow/DXKeyBindWindow; Views HelpDialog.cs.
* **Flow:** preferences are loaded/saved locally; HelpInfo comes through system data. A key can trigger an existing gameplay packet but adding a key binding does not inherently require a new protocol.
* **Start here:** Client/UserModels/KeyBindInfo.cs; CEnvir.GetKeyAction; owning scene/dialog; Server/Views/HelpInfoView.cs for help content.
