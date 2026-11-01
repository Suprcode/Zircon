# Item packet security checks

Run from the repository root with .NET 10:

```powershell
dotnet run --project Tests/ItemPacketChecks/ItemPacketChecks.csproj -p:BuildInParallel=false
```

The executable calls production `SConnection.Process` and `PlayerObject` methods against a fresh, disposable MirDB session. It uses a player subclass that suppresses stats recalculation; sockets, world simulation, animations and UI are not started. The database directory is printed for inspection. Never point these checks at a deployed database.

Coverage includes duplicate-link arithmetic against `BigInteger`, 5,000 seeded random cases, invalid move/split requests, whole-transaction stack capacity, cumulative currency limits, crafting authorization and affordability, invalid accessory refinement, confirmed trade mutations, marketplace cost overflow, cash-shop charges exceeding `int.MaxValue`, NPC roll sequencing, and exclusive item ownership through swap/save/reload. Random checks target quantity arithmetic and rejected move/split requests; they are not full wire-format fuzzing.

## Source review scope

Entry points are `LibraryCore/Network/ClientPackets.cs` and the concrete `Process(C.*)` methods in `ServerLibrary/Envir/SConnection.cs`. Item operations remain synchronous on the environment processing loop. Packet definitions and wire IDs are unchanged.

| Client packet family | Server paths and review result |
| --- | --- |
| ItemMove, ItemSort, ItemSplit, ItemDelete, ItemLock | Reviewed grids, slot bounds, guild permissions, merges, owner changes and persistence. `UserItem.OnChanged` already clears other owner relationships; the save/reload check covers this existing protection. Merge arithmetic and guild merge tradability checks hardened. |
| ItemUse, BeltLinkChanged, AutoPotionLinkChanged | Reviewed owned-slot resolution, delayed use, server-selected effects, quantity consumption and item-part rewards. Links do not supply item definitions or stats. |
| ItemDrop, CurrencyDrop, PickUp | Reviewed positive quantities, funds, local ground lookup, ownership timers and removal before gain in `ItemObject`. |
| MailOpened, MailGetItem, MailDelete, MailSend | Reviewed account-scoped mail lookup, attachment removal and sender-owned item links. Shared link overflow and capacity fixes apply. |
| MarketPlaceConsign, Buy, CancelConsign, StoreBuy, GameStoreGift | Reviewed listing ownership/search state, stock, funds, fees and transfers. Added tradability, multiplication and full-width currency charge validation. Search/history/favourite packets do not grant items. |
| TradeRequest, RequestResponse, AddItem, AddGold, Confirm, Close | Reviewed partner and offered-item state, commit ownership and both balances. Freeze offers after either confirmation; revalidate tradability and independently affordable, representable balances at commit. |
| NPCBuy, NPCSell, NPCRepair, NPCFragment | Reviewed current page/type, authoritative goods, owned materials and funds. Shop decimal overflow, sale totals/balance overflow and negative/overflowing repair charges rejected before mutation. |
| NPCRefinementStone, NPCRefine, NPCRefineRetrieve | Reviewed recipe lists, quantities, owned weapon and character-scoped pending refinements. Stone recipe requires distinct single items; refinement quantities cannot exceed owned counts. Retrieval removes the pending record. |
| NPCMasterRefine, NPCMasterRefineEvaluate | Reviewed required fragment/stone/special types, owned counts, level and evaluate cost. Shared link overflow guard applies. |
| NPCAccessoryLevelUp, Upgrade, Reset, Refine | Reviewed targets/materials and upgrade state. Reject target-as-material. Combination requires two distinct valid accessories and real corundum, matching stats, valid quantity, bounds and refine type before consumption. |
| NPCWeaponCraft | Added current crafting-page authorization, funds check, valid class, distinct owned inputs and bound-input propagation before consumption/grant. |
| NPCCall, NPCButton, NPCRoll, NPCRollResult, NPCClose | Reviewed page/button resolution and scripted grants. Roll results now require one pending server roll on the current roll page; repeated roll requests cannot change that roll. |
| LootBoxOpen, Reroll, ConfirmSelection, Reveal, TakeItems; BundleOpen, Confirm | Reviewed server-owned item state, choices, reveal bits, costs, capacity and consumption. Reveal requires confirmed selection and an unrevealed valid choice. |
| QuestAccept, Complete, Track, Abandon; MilestoneClaim | Reviewed server-owned progress, choice validation and completed/claimed replay guards. Shared capacity fixes apply to rewards. |
| CraftingSetFavourite, Start, Cancel | Reviewed recipe/design validation and material/funds revalidation at timed completion in `PlayerObject.Crafting.cs`. |
| GroupLootSettings, Share, Take, Vote | Reviewed leader/member permissions, active share/vote identity, capacity and item removal in `PlayerObject.GroupLoot.cs`. |
| Companion operations, MarriageMakeRing, cosmetic changes, FortuneCheck | Reviewed account-owned companion lookup and inventory/equipment/ticket consumption; requests do not provide arbitrary item ownership or reward definitions. |
| Chat commands that can grant items/currency | Admin commands go through `AdminCommandHandler.IsAllowedByPlayer`; ordinary `@event` invokes server-configured event logic. |

## Remaining validation

These checks establish specific invariants, not absence of all exploits. Run live multi-client trades, shared guild storage, companion transfers, NPC rolls, refinement, box rewards and reconnect/restart scenarios with representative database copies. Audit custom NPC/event scripts and economy definitions separately: authorized server-configured grants and incorrect prices/recipes cannot be inferred to be safe from packet validation alone. Transport framing/resource exhaustion, crash recovery during saving, and external payment/HTTP endpoints are outside this item-transaction check.
