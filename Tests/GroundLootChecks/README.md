# Ground loot performance

## Implemented

- Stable reusable render-row buckets replace a complete object/effect scan for every map row. Scenery remains interleaved with objects, and player effects retain their special pass.
- Ground items update camera-relative position and colour without character animation, damage, poison, or buff processing.
- Ground images, currency image selection, item-part resolution, and image dimensions are cached. Missing dimensions retry while resources load; replaced image arrays invalidate dimensions.
- Separate reusable overlay lists omit invisible item labels and keep items out of actor overlay passes. Sprite, animation-bound and light-bound culling are independent.
- Loot labels use a reference-counted style cache. Removed drops release their labels, including at map disposal. Highlight settings are parsed once and individual results refresh on settings/name changes.
- Loot effects share animation data, calculate their fixed loop directly from elapsed time, cache animation bounds and expose constant light values. Combat effects retain their original timing implementation.
- **Settings > Game > Compact Loot Piles** is optional and defaults off. It chooses the highest visual priority, then the lowest object ID on ties; it displays one sprite/glow per tile. Counts appear only in the hovered pile's list, with wheel paging for large piles. Permanent item names follow the Item Names setting; compact mode adds no permanent count labels. Actual item objects and server pickup rules are unchanged.
- Hover lists rebuild on cell membership changes, rather than traversing a large pile on every render.
- Scenes without ground items bypass loot preparation, overlay-list rebuilding and render-row buckets. Removing the last item clears the loot caches and restores the original object/effect scan path.

## Automated checks

Run from the repository root:

```powershell
dotnet run --project Tests/GroundLootChecks/GroundLootChecks.csproj -c Release
dotnet build Client/Client.csproj --no-restore -p:OutDir=artifacts/loot-build/
```

The executable links the production bucket, pile and label-cache code. It checks ordered equivalence to the previous row scan over 200 changing scenes, empty/reset boundaries, deterministic selection after representative removal and zero-allocation selection across 8,000 drops, and 10,000 reference-counted label allocation/release cycles. Labels are headless substitutes: GPU disposal and visual layout still require the live checks below.

It also links the production `MirEffect` and `LootEffect` classes against headless scene/library substitutes. Checks cover ordinary effects owning separate arrays, loot effects sharing matching animation data, elapsed-time animation after returning to view, cached bounds invalidation on image-array replacement, suppressed pile lights, and normal effect completion. The specialized constructor supplies only common effect state; shared animation data is assigned by `LootEffect` without first allocating throwaway arrays.

The quality review removed duplicate sprite culling, unused per-item pile counts and repeated writes for losing pile candidates. Name and hover labels are now acquired independently when needed, and label culling uses the same bounds as drawing. Normal and compact hover layout paths are separate; compact labels remain anchored to the pile. Chat scrolling takes precedence over piles behind the chat area.

Initial row-selection benchmark, 50 rows and 1,000 repetitions after warming:

| Items | Previous checks | Grouping + visits | Previous ms | New ms | New bytes/iteration |
|---:|---:|---:|---:|---:|---:|
| 100 | 5,000 | 200 | 0.010028 | 0.004415 | 0 |
| 500 | 25,000 | 1,000 | 0.024429 | 0.012418 | 0 |
| 1,000 | 50,000 | 2,000 | 0.036664 | 0.014264 | 0 |
| 2,000 | 100,000 | 4,000 | 0.067204 | 0.009848 | 0 |

These are CPU algorithm measurements, not game FPS predictions. Tiered compilation and machine load affect timings. The benchmark verifies identical output checksums and zero warmed allocations.

## Live validation still required

No logged-in client/test server was available during implementation. Run these scenarios on a test character, with the same resolution, renderer, frame cap and camera position for each comparison:

1. Create 100, 500, 1,000 and 2,000 drops, both spread across the view and piled on a few tiles. Include currency at different quantities, item parts, normal/enhanced/superior/elite items and quest items.
2. Compare names on/off, effects on/off, and compact mode on/off. Include night lighting and off-screen lights that overlap the viewport.
3. Walk through the pile and behind scenery, shake the camera, and cross every viewport edge. Verify sprite and effect ordering, label visibility, and smooth animation when effects return to view.
4. Hover and page through very large piles at all screen edges and UI/font scales. Confirm every drop appears in the list; pick up the representative and check the replacement and count immediately.
5. Toggle highlighting, clear highlighting, and use identical names with different rarity colours. Verify label colours remain independent.
6. Pick up and expire drops repeatedly, change maps, reconnect and reset rendering resources. Track process memory over a prolonged repeated-drop test; warmed memory should settle rather than grow with every cycle.

The initial runtime CSV profiler has been removed following an empty-scene FPS regression report. Its measurement scopes were executed in render/update paths even when recording was disabled. Use an external sampling/graphics profiler for live diagnosis, and compare FPS without a profiler attached. The headless benchmark remains available.

Include a zero-drop baseline before adding loot and again after removing the final drop. Use the same build configuration, renderer, resolution, scene and frame-cap settings. The empty-scene correction removes profiling scopes, bypasses loot-specific preparation/render buckets, and avoids assigning character-name colours on every render. The user confirmed that empty-scene performance recovered and subsequently reported roughly 6,000 FPS with about 8,000 compacted drops. Those are user observations, not automated benchmark results; compare again after code changes.

## Conditional ground-item texture cache

A new flattened ground-item texture layer was deliberately not added. The plan makes it conditional on remaining rendering cost after these changes. The current map texture already caches world output, and sprites are interleaved with scenery by row. A further cache would need ordered segments and resource/camera invalidation, with additional textures and compositing. Prototype it only if live GPU/CPU profiling shows static item submission still dominates; accept it only if it improves the same test scenes without breaking occlusion or increasing memory disproportionately.
