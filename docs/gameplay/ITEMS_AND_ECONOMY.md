# Items and economy

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

* **Usually required:** Item definitions/instances, transaction validation and affected grids or dialogs.
* **Usually NOT required:** Monster AI, movement and spell effects unless directly affected.
* Apply these defaults to the selected section; follow its direct dependencies when scope crosses a boundary.

## Inventory, equipment and storage

* **Purpose / definitions:** item ownership, slots, use and equipment; Defs `ItemInfo.cs`, `ItemInfoStat.cs`, `SetInfo.cs`; `LibraryCore/Enum.cs` GridType, EquipmentSlot, ItemType.
* **Server / persistence:** P `Items` region: `ItemMove`, `ItemUse`, `CanWearItem`, `CanGainItems`, `GainItem`, `ParseLinks`; DB `UserItem.cs`, `UserItemStat.cs`, `UserItemSocket.cs`, CharacterInfo/AccountInfo ownership.
* **Client / UI:** `Client/Controls/DXItemCell.cs`, DXItemGrid; Views `InventoryDialog.cs`, `CharacterDialog.cs`, `StorageDialog.cs`; `GameScene.CreateItemLabel` owns the common tooltip.
* **Packets / flow:** C.ItemMove/ItemUse/ItemSort/ItemSplit/ItemLock → P validation and instance changes → S.ItemMove/ItemSort/ItemChanged/ItemsGained/ItemStatsChanged/ItemDurability. Client handlers choose grids by GridType and update/release pending cells.
* **Important:** definition data and instance data cross different boundaries; ClientUserItem resolves InfoIndex from Globals. Equipment appearance also uses S.PlayerUpdate. Storage shares item movement paths, not a separate universal storage packet.
* **Start here:** Defs ItemInfo.cs; DB UserItem.cs; P ItemMove; DXItemCell.cs; GameScene.CreateItemLabel.

## Ground drops, pickup and loot display

* **Definitions / state:** Defs `DropInfo.cs`, `ItemInfo.cs`, `CurrencyInfo.cs`; DB UserItem/UserDrop.
* **Server:** `ServerLibrary/Models/MonsterObject.cs: Drop`; P `ItemDrop`, `PickUp`, `DeathDrop`; `Models/ItemObject.cs` ground lifetime.
* **Client / UI:** `Client/Models/ItemObject.cs`; Views `MapControl.Loot.cs` and `MapControl.Names.cs`; GroundLootPiles/GroundItemLabels/ItemHighlights/LootEffect under Client/Models.
* **Packets / flow:** C.ItemDrop/PickUp/CurrencyDrop → server ground/item changes; S.ObjectItem/ObjectRemove and ItemsGained/CurrencyChanged synchronize presence and gains. Changing ground labels does not change server ownership.
* **Start here:** P PickUp; server ItemObject.cs; client ItemObject.cs; MapControl.Loot.cs. Focused checks: `Tests/GroundLootChecks`.

## NPC scripts, shops and refinement

* **Definitions / state:** Defs NPCInfo.cs contains NPCPage/check/action/value structures; ItemInfo, WeaponCraftStatsInfo; DB GameNPCData.cs and RefineInfo.cs.
* **Server:** Models NPCObject.cs executes page checks/actions/values; P NPCCall/NPCButton and NPCBuy/NPCSell/NPCRepair/NPCRefine/NPCWeaponCraft.
* **Client / UI:** Views NPCDialog.cs (many related dialogs in one file), NPCSocketDialog.cs, NPCSocketCombineDialog.cs; client NPCObject for world drawing.
* **Packets / flow:** C.NPCCall/NPCButton → server page execution → S.NPCResponse (page index + values) or NPCClose; transaction requests use C.NPCBuy/NPCSell/NPCRefine/NPCSocketItem and corresponding item/NPC updates.
* **Important:** a client button does not authorize a transaction. NPCObject checks pages before actions and can follow success/failure pages. Recipe crafting below is a different system from NPCWeaponCraft.
* **Start here:** NPCObject.cs; P NPCCall/NPCButton or transaction method; Defs NPCInfo.cs; Views NPCDialog.cs; `Server/Views/NPCPageView.cs`.

## Trade

* **State / persistence:** P TradeItems and partner state; transferred DB UserItem/UserCurrency, not a separate TradeInfo record.
* **Server / UI:** P trade methods reached by SConnection trade handlers; Views TradeDialog.cs and DXItemCell.
* **Packets / flow:** C.TradeRequest/RequestResponse/AddItem/AddGold/Confirm/Close → server; S.TradeRequest/Open/AddItem/ItemAdded/AddGold/GoldAdded/Unlock/Close.
* **Start here:** SConnection.Process(C.TradeConfirm); P matching trade method; TradeDialog.cs; UserItem.cs. Preserve confirmation/unlock and both peers' cleanup when the offered contents change.

## Marketplace and game store

* **Definitions / persistence:** Defs StoreInfo; DB AuctionInfo/AuctionHistoryInfo, GameStoreFavourite/GameStoreSale, UserItem, UserCurrency.
* **Server / UI:** P MarketPlace region; SConnection marketplace search handlers; Views ConsignmentDialog.cs and GameStoreDialog.cs.
* **Packets / flow:** C.MarketPlaceSearch/Consign/Buy/CancelConsign/StoreBuy, GameStoreFavouriteToggle/GameStoreGift; S.MarketPlaceSearch/Consign/Buy/StoreBuy, GameStoreData/TopItems/FavouriteChanged/Gift.
* **Start here:** SConnection.Process(C.MarketPlaceSearch); P MarketPlaceBuy; AuctionInfo.cs; ConsignmentDialog.cs. Search/result state also lives on SConnection, not solely PlayerObject.

## Loot boxes and bundles

* **Definitions / persistence:** Defs LootBoxInfo.cs/BundleInfo.cs; DB UserItem and CharacterInfo; Globals.ClientLootBoxItemInfo/ClientBundleItemInfo.
* **Server / UI:** P Loot Boxes/Bundles regions; Views LootBoxDialog.cs/BundleDialog.cs.
* **Packets / flow:** C.LootBoxOpen/Reroll/ConfirmSelection/Reveal/TakeItems, C.BundleOpen/Confirm → P corresponding methods and S loot-box/bundle responses plus inventory updates.
* **Start here:** P LootBoxOpen/LootBoxConfirm or BundleOpen/BundleConfirm; definition file; matching dialog. Preserve multi-step selection/reveal state when changing reward logic.
