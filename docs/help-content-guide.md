# Help Content Guide

Use this guide to add or update in-game help content stored as JSON documents. The existing `docs/player-stats-help.json` file is an example that follows this format, but the same structure applies to any new topics you want to document.

## JSON structure
- Each help document is an array of entries with `Title`, `Order`, and `Description`.
- Entries contain `Pages`, each with its own `Title`, `Order`, and `Sections`.
- Sections hold the short `Content` shown to players. Keep these concise and player-friendly.

## How to add or update help content
1. Create or open a JSON file under `docs/` (for example, `docs/<topic>-help.json`).
2. Preserve the outer array and keys (`Title`, `Order`, `Description`, `Pages`, `Sections`, `Content`).
3. Add or edit sections to adjust wording, ordering, or to add new topic groups.
4. When adding new pages or sections, set `Order` values so they appear in the desired sequence.
5. Validate that the JSON remains well-formed before importing into the game.

For recurring updates, request changes by specifying the file path and the exact sections or pages to adjust.
