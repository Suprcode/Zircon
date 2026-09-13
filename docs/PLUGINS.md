# Editor plugins

## Supported surface

Authority: `PluginCore/PluginLoader.cs`, `IPluginStart.cs`, `IPluginType.cs`, `AbstractStart.cs`, `AbstractPlugin.cs`, `Types`, `Events`, `GridActions.cs`; consumers `Server/SMain.cs` and `PluginStandalone/Program.cs`.

This is a server-editor extension mechanism. The inspected contracts expose menus, forms, logs, map viewing and supported grid row actions. They do not establish general hooks for every combat event or a client mod loader. Server gameplay's `Envir/Events` is a separate mechanism.

## Discovery and lifecycle

```text
SMain constructs/initializes editor Session and subscribes loader events
→ PluginLoader.LoadPlugins(ribbon page, Session)
→ current directory Plugin.*.dll files
→ Assembly.LoadFrom(file)
→ type named <filename without extension>.Start implementing IPluginStart
→ Activator.CreateInstance; attach forwarded events; add to Plugins
→ assign Session; Type.SetupMenu(plugin page)
```

`LoadPlugin` catches and logs errors and returns null on failure. Its lookup is a naming contract, not a scan for arbitrary interface implementations. `PluginLoader.Instance` owns the loaded start objects. The shown loader provides no unload/hot-reload lifecycle; do not promise one from these contracts.

## Contracts and examples

| Source | Use |
| --- | --- |
| `AbstractStart<T>` | Instantiates T, assigns `Type.Start = this`, exposes Session/name/namespace/assembly information and Log/View/MapViewer helpers; `GetPluginFolder` delegates to Globals.PluginPath |
| `AbstractPlugin` / `IPluginType` | Start reference and `SetupMenu(IComponent)` |
| `IPluginStart` | Entry contract with Type, Session, names, log/view/map events |
| `Types/IPluginForm.cs` | `SupportsStandaloneLoading`, `CreateStandaloneForm` |
| `Types/IPluginMessage.cs` | Optional `ReceiveMessage(object value)` contract; inspect actual callers before assuming gameplay delivery |
| `Helpers/RibbonHelper.cs` | Existing DevExpress ribbon construction helper |
| `GridActions.cs` | Optional `IPluginGridActionProvider.GetGridActions(Type)` → actions with key/caption/predicate/Execute and row/owner context |

`PluginGridActionBinder.Attach` enumerates current plugins, asks eligible providers for actions for a row type, and attaches them to a supported grid. Concrete consumers are `Server/Views/MonsterInfoView.cs` and `Server/Views/DungeonInfoView.cs`. Do not assume every grid offers the hook. Setup order matters because it enumerates loaded plugin instances.

`Server/SMain.cs` forwards loader Log/View/MapViewer to the application. This is the canonical integrated host example. `PluginStandalone/Program.cs` is the canonical alternate host: reads App.config's `Plugin` key, loads that file, requires IPluginForm with standalone support, and runs its returned Form. It calls LoadPlugin directly, so it does not perform integrated Session assignment/menu setup.

Needs verification: no concrete `Plugin.*` extension implementation is included in the main solution. Validate a third-party plugin's naming, runtime/dependency compatibility, standalone initialization and cleanup from its own source before using it as a canonical implementation.
