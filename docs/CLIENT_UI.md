# Client DX controls and dialogs

## Start here

`Client/Controls/DXControl.cs`, the existing dialog under `Client/Scenes/Views`, its owning `Client/Scenes/GameScene.cs` field/constructor, and `DXButton.cs` / `DXImageControl.cs`. Only open RenderingCore if the change affects drawing primitives/resources.

```text
DXControl
├─ DXScene → LoginScene, SelectScene, GameScene
├─ DXWindow → game dialogs (check each declaration)
├─ DXImageControl → DXButton, DXAnimatedControl
├─ DXLabel
├─ DXTextBox → native MirTextBox hosted inside the DX wrapper
└─ DXItemCell / DXItemGrid / list, tree, tab and scroll controls
```

Exact controls live in `Client/Controls`: DXListBox, DXTreeControl, DXTabControl, DXVScrollBar, DXHScrollBar, DXComboBox. Some files contain multiple types; follow the declaration instead of assuming every dialog has a same-named file.

## Canonical examples

Start with [ExitDialog or CraftingRecipeDialog](CANONICAL_EXAMPLES.md#client-ui) for simple or complex UI; use the [crafting request](CANONICAL_EXAMPLES.md#client-gameplay-request) for gameplay buttons and [DXImageControl](CANONICAL_EXAMPLES.md#library-image-usage) for image resolution/drawing.

## Ownership, state and input

### Usually required

* For an **existing dialog layout change**, start in its `Client/Scenes/Views` file (for example `InventoryDialog.cs`) and the DX controls already used there. Inspect `DXControl` parenting, clipping and cache invalidation when affected by the layout.

### Usually NOT required

* `ServerLibrary` and `LibraryCore/Network` unless the changed interaction performs a different gameplay action; `RenderingCore` unless drawing primitives/resources change.
* GameScene construction, `WindowSetting.WindowType` and key bindings when only moving/resizing existing children. Open those integration points when adding a window or changing its lifetime/opening behavior.

`DXControl.Parent` calls `OnParentChanged`, removes the control from the old Controls list, adds it to the new one, recalculates visibility/enabled/display area, and invalidates both parents' child caches. Assign Parent through the property; avoid maintaining Controls separately.

Visible/Enabled are local flags; IsVisible/IsEnabled reflect ancestry. Location/Size feed DisplayArea and ClipArea; coordinates are parent-relative. BringToFront/SendToBack reorder the parent's list. Disposal recursively disposes children and releases resources/events; derived controls must preserve base cleanup and release their additional references/subscriptions.

`Client/TargetForm.cs` converts physical input to logical coordinates (`ToLogicalMouseEventArgs`, GameScene UI coordinate conversion), then forwards to `DXControl.ActiveScene.OnMouse...` / `OnKey...`. DXControl owns static MouseControl/FocusControl and routes controls' mouse/key events. `PassThrough` and clipping/hit testing matter for overlays. `DXTextBox.cs: MirTextBox` is a WinForms text control bridged into this system, not a normal drawn label.

## Drawing and caches

`DXControl.Draw` orders BeforeDraw → DrawControl → BeforeChildrenDraw → DrawChildControls → DrawBorder → AfterDraw. Event placement therefore determines whether an overlay covers children. `DrawChildControls` may cache child segments: CacheInParent, TextureValid, InvalidateChildCache and parent invalidation affect whether an event is redrawn. Inspect those branches before placing animated drawing in a cached parent.

`DXImageControl.LibraryFile` + Index selects an image; `DXButton` adds image states and mouse behavior. Do not infer indices across libraries; mapping and graphics resource rules live in [RENDERING_AND_ASSETS](RENDERING_AND_ASSETS.md#library-and-image-identity).

## Scene integration and examples

* `Client/Scenes/Views/InventoryDialog.cs`: item-grid dialog; actual common item tooltip is `GameScene.CreateItemLabel`, not this file.
* [Crafting dialog examples](CANONICAL_EXAMPLES.md#complex-dialog) cover composed controls; [client scene integration](CLIENT_RUNTIME.md#gamescene-and-other-partials) maps their owners.
* `Client/Controls/DXWindow.cs` and `Client/UserModels/WindowSetting.cs`: reusable window layout/settings. Inspect the window's settings identity and GameScene/menu/key action when adding a window.
* `Client/Envir/Translations/{StringMessages,EnglishMessages,ChineseMessages}.cs`: client message definitions/implementations.

## Add a button or dialog

1. Locate the owning Views file and its GameScene field; reuse its layout/image style and existing control types.
2. Construct children with Parent, Location and state. For a button, inspect DXButton's normal/hover/pressed/disabled image properties and the adjacent button's event.
3. Keep a local visibility/layout action local. If it changes gameplay, trace CEnvir.Enqueue → SConnection → server method and any response with [NETWORKING](NETWORKING.md).
4. For a new window, register construction and lifetime in GameScene; inspect `Client/UserModels/WindowSetting.cs: WindowType` and `KeyBindInfo.cs: KeyBindAction`, MenuDialog and GameScene key/window handling, DXWindow persistence and CEnvir key bindings.
5. Preserve localization, input propagation, observer restrictions, pending cell locks and packet rejection behavior as applicable.
6. Check clipping/UI scaling, cache invalidation, child order, hide/show and disposal. Reuse the existing library index only after confirming the actual asset.

A button added to an existing dialog does not itself require a new packet, enum or rendering backend. Those dependencies follow the action it performs.
