# Content, editors and distribution

## Game data editing is in Server

`Server/Program.cs → Server/SMain.cs` hosts DevExpress views and creates a **System-mode editor Session**. `ServerLibrary/Envir/SEnvir.cs` creates the simulation's Users-mode Session, which also reads system definitions. Do not confuse those independently loaded instances.

| Change | Editor source | Shared roots |
| --- | --- | --- |
| Item property/stats | `Server/Views/ItemInfoView.cs`, `.Designer.cs`, `ItemInfoStatView.cs` | ItemInfo, ItemInfoStat, SetInfo |
| Magic | `Server/Views/MagicInfoView.cs` and designer | MagicInfo |
| Monster/drop/spawn | `Server/Views/MonsterInfoView.cs`, `DropInfoView.cs`, `RespawnInfoView.cs` and designers | MonsterInfo, DropInfo, RespawnInfo |
| Maps/instances/dungeons | `Server/Views/MapInfoView.cs`, `InstanceInfoView.cs`, `DungeonInfoView.cs` | MapInfo, InstanceInfo, DungeonInfo |
| NPC pages/commands | `Server/Views/NPCInfoView.cs` and related NPC views | `LibraryCore/SystemModels/NPCInfo.cs` |
| Quests/milestones/help | `Server/Views/QuestInfoView.cs`, `MilestoneInfoView.cs`, `HelpInfoView.cs` | QuestInfo, MilestoneInfo, HelpInfo |
| Crafting | `Server/Views/CraftingInfoView.cs` and designer | CraftingRecipeInfo, CraftingIngredientInfo, CraftingLevelInfo |
| Companions/castles/events | `Server/Views/CompanionInfoView.cs`, `CastleInfoView.cs`, `EventInfoView.cs` | corresponding SystemModels |
| Accounts/characters | `Server/Views/AccountView.cs`, `CharacterView.cs` | ServerLibrary DBModels; inspect user-session handling |

Canonical example: [ItemInfoView](CANONICAL_EXAMPLES.md#systemmodel-editor). It binds `SMain.Session.GetCollection<ItemInfo>().Binding`, configures enum/lookups and invokes `Session.Save(true)`. Designer files define visible columns and editors, so adding a model property does not prove it is editable. `CraftingInfoView` also validates ingredient-row limits/duplicates and imports/exports recipe or level data according to the selected tab.

`Server/Helpers/JsonImporter.cs` and `Server/Helpers/JsonExporter.cs` support content import/export. Follow existing model associations and lookup rules; JSON exports are not the network format. SMain's insertion helpers can shift identities and mark references modified; do not assume definition indices are stable after content restructuring.

## How definitions reach runtime

Editor save → MirDB System.db. `Server/Views/ConfigView.cs: SyncronizeLocalButton_Click` explicitly saves the editor Session and copies System.db to `Config.ClientPath/Data`. `Client/Envir/CEnvir.LoadDatabase` populates Globals from that client database. `GeneralPackets.GoodVersion` carries a system-database version; `Client/Scenes/LoginScene.cs: UpdateSystemDatabaseVersionLabel` compares/displays it. This is not automatic replacement of definitions through the game connection. Distribution can use the patching path below.

For remote **server** content upload, start at `Server/Views/SyncForm.cs`; [NETWORKING](NETWORKING.md#separate-http-paths) owns endpoint validation and reload boundaries. Uploading System.db does not reload live collections.

## Image tools and supporting binaries

* `LibraryEditor/Program.cs → LMain.cs`: opens a file argument, displays/edits libraries, converts legacy formats and writes through Mir3Library/save options. References RenderingCore, not LibraryCore's SystemModels.
* `ImageManager/Program.cs → IMain.cs`: batch converts WTL files and packs image folders into ZL, with independent format code. No project reference to RenderingCore; verify writer compatibility for modern containers.
* `Components`: bundled binary dependencies used by asset tooling; no source-level gameplay ownership.
* `Tools/convert_audio_to_ogg.cmd`: audio conversion. `Tools/RenderingCacheChecks` and `Tests/GroundLootChecks`: narrow verification programs, not content editors.

Use [RENDERING_AND_ASSETS](RENDERING_AND_ASSETS.md) for format and cache authority.

## Patching boundary

`PatchManager/PMain.cs` produces `PList.Bin`, gzipped payloads and uploads. `PatchManager/PatchInformation.cs` computes MD5 checksums and writes file metadata. `Launcher/LMain.cs` consumes the list, compares/downloads payloads, maintains Version.bin and launches Zircon.exe. Both use path flattening (`\\` replaced by `-`) for compressed web filenames.

`Launcher/PatchInformation.cs` reads the binary metadata in the writer's order; change these paired definitions together. Check gzip, filename handling and hash/length semantics at both ends before altering the manifest. Project/source references are listed in [PROJECT_MAP](PROJECT_MAP.md).

`Patcher/Program.cs` stores two command-line arguments. `Patcher/PMain.cs` waits for the launcher to exit, replaces the destination with the supplied temporary file, then starts it. It is a launcher-update helper, not the game-data patch generator.

Needs verification: deployment endpoints, external content/database files and operational publication procedures depend on installation configuration; no live publication was performed.
