# Canonical examples

Open the matching example and named method first, not every implementation in its family. Paths are repository-relative. These are pattern references, not instructions to copy unrelated feature rules; follow direct dependencies only as needed. Large owner files are listed only with bounded entry points.

## Client UI

### Simple dialog

Purpose: A small image-backed dialog with parented buttons and labels.
Use as reference when: Adding ordinary dialog layout, local close behavior and cleanup; logout checks are specific to this dialog.
File: `Client/Scenes/Views/ExitDialog.cs`.
Start at: `ExitDialog()` and `Dispose(bool)`.

### Complex dialog

Purpose: Recipe selection and ingredient presentation using tabs and item controls.
Use as reference when: Building a data-driven dialog with multiple child controls and refresh paths.
File: `Client/Scenes/Views/CraftingDialogs.cs`.
Start at: `CraftingRecipeDialog` and `CraftingDesignTab` constructors.

## Packet flow

Use this single crafting flow for the four steps below; protocol rules remain in [NETWORKING](NETWORKING.md).

### Client gameplay request

Purpose: Send a recipe/design choice through `CEnvir.Enqueue`.
Use as reference when: Wiring a gameplay button to a ClientPacket while retaining server validation.
File: `Client/Scenes/Views/CraftingDialogs.cs`.
Start at: `CraftingRecipeDialog.StartCraft` (`C.CraftingStart`).

### Server request handler

Purpose: Gate a request by game stage and delegate to its player owner.
Use as reference when: Adding a reflection-dispatched public `Process(C.X)` overload.
File: `ServerLibrary/Envir/SConnection.cs`.
Start at: `Process(C.CraftingStart p)` → `Player.StartCrafting(p)`.

### Server response

Purpose: Validate and initialize crafting before sending its start notification.
Use as reference when: Emitting a ServerPacket after authoritative state changes.
File: `ServerLibrary/Models/PlayerObject.Crafting.cs`.
Start at: `StartCrafting`, especially `Enqueue(new S.CraftingStarted ...)`.

### Client response handler

Purpose: Resolve the recipe definition and start the progress display.
Use as reference when: Consuming an S packet through a public client handler.
File: `Client/Envir/CConnection.cs`.
Start at: `Process(S.CraftingStarted p)` → `CraftingProgressBox.Start`.

## Data models

MirDB ownership, association and serialization rules remain in [DATA_MODEL](DATA_MODEL.md).

### Simple shared SystemModel

Purpose: Define shared currency metadata with ordinary change-tracked properties.
Use as reference when: Adding scalar fields to a System.db definition.
File: `LibraryCore/SystemModels/CurrencyInfo.cs`.
Start at: `CurrencyInfo.Name`, `Abbreviation` and `Type` setters.

### Associated SystemModels

Purpose: Link recipe definitions to their owned ingredient rows.
Use as reference when: Adding parent/child definition collections with matching association names.
File: `LibraryCore/SystemModels/CraftingInfo.cs`.
Start at: `CraftingRecipeInfo.Design1Ingredients` and `CraftingIngredientInfo`'s `Design1Ingredients` association.

### Server persisted DBModel

Purpose: Store an account's currency amount and definition reference.
Use as reference when: Adding user data with `[UserObject]`, change tracking and client conversion.
File: `ServerLibrary/DBModels/UserCurrency.cs`.
Start at: `UserCurrency.Amount`, `Account`, `OnDeleted` and `ToClientInfo`.

### DBModel with linked children

Purpose: Maintain item-owned stats and sockets with inverse relationships.
Use as reference when: Adding aggregate child records and their ownership/cleanup paths.
File: `ServerLibrary/DBModels/UserItem.cs`; `ServerLibrary/DBModels/UserItemSocket.cs`.
Start at: `UserItem.AddedStats/Sockets` and `UserItemSocket.Item/Gem` association setters.

### Migration property

Purpose: Read the old magic Class value and convert it into RequiredClass.
Use as reference when: Migrating an existing persisted field, not adding an ordinary property.
File: `LibraryCore/SystemModels/MagicInfo.cs`.
Start at: `[MigrationProperty("Class")] LegacyClass`, `OnLoaded` and `LegacyClassToRequiredClass`.

## Server behavior

Feature ownership and hook constraints remain in [SERVER_RUNTIME](SERVER_RUNTIME.md).

### Simple PlayerObject feature

Purpose: Validate a favourite recipe choice, store it and send refreshed crafting state.
Use as reference when: Implementing a small player-owned definition selection with an idempotent update.
File: `ServerLibrary/Models/PlayerObject.Crafting.cs`.
Start at: `SetCraftingFavourite` → `SendCraftingState`.

### PlayerObject partial feature

Purpose: Keep crafting validation, timed execution and cancellation together.
Use as reference when: Adding a cohesive player partial processed by the existing environment loop.
File: `ServerLibrary/Models/PlayerObject.Crafting.cs`.
Start at: `StartCrafting`, `ProcessCrafting`, `CancelCrafting` and `CanCraft`.

### Simple monster subclass

Purpose: Select among existing ranged attacks through one override.
Use as reference when: Adding a small attack variation with inherited targeting.
File: `ServerLibrary/Models/Monsters/OmaMage.cs`.
Start at: `OmaMage.Attack`; follow `SkeletonAxeThrower` only for inherited behavior.

### Complex monster subclass

Purpose: Combine lifecycle, health-stage summons and target processing.
Use as reference when: Coordinating multiple monster hooks; its boss stages and summons are feature-specific.
File: `ServerLibrary/Models/Monsters/ZumaKing.cs`.
Start at: `OnSpawned`, `Process` and `ProcessTarget`; inherited owner is `ZumaGuardian`.

## Client visuals and assets

Drawing ownership and asset identity remain in [RENDERING_AND_ASSETS](RENDERING_AND_ASSETS.md).

### Visual MapObject subclass

Purpose: Construct a visible ground item from server-provided item state.
Use as reference when: Mapping an object packet to a client actor's appearance; loot-pile behavior is item-specific.
File: `Client/Models/ItemObject.cs`.
Start at: `ItemObject(S.ObjectItem info)` and its appearance/library selection.

### Client effect

Purpose: Animate library frames and draw a timed visual effect.
Use as reference when: Reusing effect timing and drawing without changing server damage execution.
File: `Client/Models/MirEffect.cs`.
Start at: `MirEffect(...)`, `Process` and `Draw`.

### Library image usage

Purpose: Resolve a library/image identity and draw it as a DX control.
Use as reference when: Using existing ZL images in UI rather than introducing a rendering primitive.
File: `Client/Controls/DXImageControl.cs`; usage in `Client/Scenes/Views/ExitDialog.cs`.
Start at: `OnLibraryFileChanged` and `DrawControl`; `ExitDialog()` sets `LibraryFile.Interface` and `Index`.

## Editors and plugins

### SystemModel editor

Purpose: Bind shared item definitions to the editor session and expose editable columns.
Use as reference when: Adding a SystemModel editor or an editable definition field.
File: `Server/Views/ItemInfoView.cs` and `.Designer.cs`.
Start at: `ItemInfoView()` collection binding and designer `InitializeComponent`; see [CONTENT_AND_EDITORS](CONTENT_AND_EDITORS.md).

### Plugin host integration

Purpose: Connect plugin log/view/map events and load plugins into the editor host.
Use as reference when: Integrating the existing editor plugin loader, not implementing gameplay hooks.
File: `Server/SMain.cs`.
Start at: `SMain()` subscriptions to `PluginLoader.Instance` and `PluginLoader.LoadPlugins`; see [PLUGINS](PLUGINS.md) for contracts and the absence of an in-solution concrete extension example.
