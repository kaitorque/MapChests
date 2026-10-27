# MapChests – Design and Work Plan

This is the working brief for building MapChests. Read it fully before changing code.

## 1. Goal

MapChests is one BepInEx plugin for Risk of Rain 2 that replaces two mods:

- **MiniMapMod** (DekuDesu / Arty Redd): a minimap with icons for interactables, enemies, players and items.
- **QoLChests** (Faustvii): highlights interactables, and hides or fades them once used.

The two must share **one definition of interactable categories**. A chest is a "Chest" for highlighting, hiding, and its minimap icon alike, with one place to classify it and one config section per category.

### Constraints

- **One plugin, one DLL.** `MiniMapLibrary` is folded into the main assembly. The original mods are uninstalled when MapChests is used.
- **Local only for now.** Build and test in the owner's r2modman profile. Do not push to any remote, and do not publish to Thunderstore.
- **Keep credit.** The original commit history is preserved in this repo. When moving files, use `git mv` so history and blame follow the code. Keep the Credits section in `README.md`.
- **Credit all borrowed code.** Whenever code or logic from another person's work is ported (for example winterofhell's 4.2.0 update, section 3.1), add them to the Credits section of `README.md` in the same commit, and name the source (repo, PR, file) in the commit message.
- **No licenses upstream.** None of the source repositories (DekuDesu/MiniMapMod, winterofhell's fork, Faustvii/R2Mods) has a license file. That is fine for local use; before any public release, ask the original authors for permission or check with the owner.
- **Ask before adding scope.** Anything beyond this plan (new features, balance changes) needs the owner's approval first.

## 2. Environment

| Item | Value |
|---|---|
| OS / shell | Windows, PowerShell |
| .NET SDK | 10.0.203 (`dotnet build`) |
| Game install | `E:\SteamLibrary\steamapps\common\Risk of Rain 2` (managed DLLs in `Risk of Rain 2_Data\Managed`) |
| Game version | 1.5.0 (Hallowed Concepts, DLC4) |
| r2modman profile | `%APPDATA%\r2modmanPlus-local\RiskOfRain2\profiles\VP\BepInEx` |
| Installed relevant mods | `DekuDesu-MiniMapMod` 4.0.0, `Faustvii-QoLChests` 1.3.6, `Rune580-Risk_Of_Options` |
| Decompiled game source | `F:\Mod\RoR2\_decomp` (RoR2, entity states). Use it to check game types and behaviour. |
| Reference build setup | `F:\Mod\RoR2\MonsoonLikeDrizzle` (small plugin by the owner: csproj, `package.ps1`) |

## 3. Current state of the repo

Both projects sit side by side, unchanged from upstream:

| Path | What it is |
|---|---|
| `MiniMapMod/` | Minimap plugin (`MiniMapPlugin.cs`, `Minimap.cs`). GUID `MiniMap`. Legacy csproj referencing ~130 game DLLs committed in `MiniMapMod/libs/`. |
| `MiniMapLibrary/` | Minimap scanning, settings and config abstractions. Built as a separate DLL. |
| `QoLChests/` | Highlight/hide plugin. GUID `Faust.QoLChests`. Modern csproj using NuGet game libraries (`RiskOfRain2.GameLibs` 1.5.0-r.1, `MMHOOK.RoR2`). |
| `Faust.Shared/` | Shared code QoLChests copies into its build (logging, Risk of Options and mod-compat helpers). |
| `MiniMapMod.sln`, `post_build.ps1`, `manifest.json`, `4.0.0/` | Upstream minimap build and release files. `post_build.ps1` has the original author's paths and is not usable as is. |

### How each mod classifies interactables today

**QoLChests** tags prefabs with a category marker component, then works from the markers:

- `QoLChests/Constants.cs` lists addressable prefab paths per category. At startup, `InteractableStateHandler.LoadHighlightableResources` adds an `InteractableHighlightCategoryMarker` with the category to each prefab.
- `Hooks/SceneHooks.cs` tags preplaced scene objects (newt statues, pressure plates, preplaced chests, Prime Meridian and Gilded Coast chests) by object name.
- `Hooks/InteractableSpawnHooks.cs` and `Api/QoLChestsApi.cs` tag modded prefabs registered by name.
- "Used" state goes through `Handlers/InteractableUsedHandler.cs`, which adds an `InteractableUsed` component, hides or fades the object (`VisibilityHandler`) and optionally removes the highlight.

**MiniMapMod** never tags anything. Every rescan, it searches the scene for component types and sorts them by string matching (`MiniMapMod/MiniMapPlugin.cs`):

- `ChestBehavior` whose `PurchaseInteraction.contextToken` contains `CHEST` → Chest. Tokens containing `STEALTH` are dropped, and `LUNAR_CHEST_CONTEXT` → LunarPod.
- `PurchaseInteraction` tokens: `DUPLICATOR` → Printer, `TERMINAL` → Shop, `EQUIPMENTBARREL` → Equipment, `FAN_CONTEXT` → Special, `GOLDSHORE` → Portal, `TOTEM` → Totem.
- Fixed component types: `RouletteChestController` → Chest; six shrine behaviours (Blood, Chance, Combat, Healing, Restack, Boss) → Shrine; `ScrapperController` → Utility; `SummonMasterBehavior` → Drone; `BarrelInteraction` → Barrel; `TeleporterInteraction` → Teleporter; `PortalStatueBehavior` (newt) → NewtAltar.
- "Active" (bought or not) comes from `PurchaseInteraction.available`. Used objects stay on the map in the inactive color.

### Category comparison

| Thing in game | QoLChests category | Minimap kind | Mismatch |
|---|---|---|---|
| Chests (small, large, legendary, category, adaptive, DLC4 chests) | Chest | Chest | Same name; matched differently (prefab list vs. token text) |
| Cloaked chest | StealthedChest | not shown | Minimap hides it on purpose |
| Rusty / encrusted lockbox | Lockbox | probably not shown (token has no `CHEST`) | Verify in game |
| Lunar pod | LunarPod | LunarPod | – |
| Void cradle | VoidCradle | probably Chest (token contains `CHEST`) | Verify in game |
| Barrel | Barrel | Barrel | – |
| Equipment barrel | EquipmentBarrel | Equipment | Name only |
| Multishop / lunar shop / drone shop / temporary-item terminals | Shop | Shop (token `TERMINAL`) | Coverage may differ |
| 3D printers | Duplicator | Printer | Name only |
| Scrapper, drone scrapper | Scrapper | Utility (`ScrapperController` only) | Drone scrapper may be missing |
| Broken drones, drone stations, Lemurian eggs | Drone | Drone | Minimap counts every `SummonMasterBehavior` |
| Broken turret | Turret | Drone | Minimap merges turrets into drones |
| Shrines (base + DLC2/3/4: Halcyonite, Colossus access, Rebirth, Catastrophe, Compulsion, Death, Dread, Hallowing...) | Shrine (full list) | Shrine (6 base behaviours only) | **Minimap misses most DLC shrines** |
| Newt altar | NewtStatue | NewtAltar | Name only |
| Pressure plates (Abandoned Aqueduct) | PressurePlate | not shown | – |
| Teleporter, portals, Gilded Coast totems, fans | – | Teleporter, Portal, Totem, Special | QoLChests ignores these |
| Enemies, players, minions, items, neutral NPCs | – | EnemyMonster/Lunar/Void, Player, Minion, Item, Neutral | Not interactables; minimap only |

### 3.1 Upstream minimap 4.2.0 update (reference only)

[artyredd/MiniMapMod PR #64](https://github.com/artyredd/MiniMapMod/pull/64) by **winterofhell** (branch `minimap-4.2.0` on `winterofhell/MiniMapMod`, open and unmerged) updates the minimap for game 1.5.0. Port its ideas and code where useful, and credit it (section 1). Do not merge the branch: it keeps the old csproj and commits game DLLs, which this plan removes. Useful parts:

- `SceneBounds.cs` and `MapProjection.cs`: the map area is taken once per stage from `SceneInfo.approximateMapBoundMesh`, falling back to ground navigation nodes, then to interactable positions only. Enemies never change the scale. One uniform scale is used for both axes.
- `Range.cs`: a corrected `Range1D` (no early return, ignores non-finite values).
- `SceneTracker.cs`: tracks objects through the game's registries (`InterfaceInstanceTracker<IInteractable>`, `InstanceTracker.GetInstancesList<GenericPickupController>()` and similar) instead of scanning the whole scene every few seconds.
- `PurchaseTokenClassifier.cs`: context-token rules including DLC tokens (drone assembly and combiner, cleansing pool, Halcyonite and Hallowed cauldrons, trial gong). Useful for the fallback classifier (4.2).
- New kinds: timed chest, stealth chest, End of the Cycle enemies, small dots for dropped pickups.

### 3.2 Known minimap bugs reported by the owner

1. **Lag on some stages, especially Sky Meadow** (last seen after the Alloyed Collective release; not yet retested on the current version). Many flying stones appear to be tracked. Toggling the minimap off and on with M clears it. Likely causes in the current code:
    - Every rescan (every 5 s, and on every character death and interaction, with a 2 s cooldown) runs `FindObjectsOfType` for about 16 types, then destroys and recreates the icon of every dynamic object (each enemy, ally and dropped item).
    - `ClearDynamicTrackedObjects` skips every other entry (`RemoveAt(i)` without `i--`), so about half the old dynamic icons survive each scan and duplicates pile up.
    - `CreateGenericInteractionScanner()` is registered as both a static and a dynamic scanner, so generic interactables get two icons.
    - If the stones carry a `TeamComponent` or are pickups, each one becomes an icon. Confirm in game what they are.
2. **Minimap slowly zooms out until icons bunch together.** Still present on the current version. Toggling with M sometimes fixes it, sometimes not until the next stage. Likely cause:
    - The map area (`Range3D` / `Range1D`) only ever grows until a reset, and every scan adds the positions of dynamic objects (enemies, allies, items). Anything far away, such as a flung or falling body, a flying enemy or an item knocked off the map, widens it for good.
    - A reset rescans right away, so if the far-away object still exists, the area grows straight back.
    - `Range1D.CheckValue` returns early after setting `Min`, so `Max` may never be set.

### Current color mapping (owner's config)

These are the owner's current values from `config\Faust.QoLChests.cfg` and `config\MiniMap.cfg` in the VP profile. The owner has already aligned the minimap colors to the highlight colors. **Use these as the MapChests defaults**, not the upstream code defaults.

QoLChests picks from a fixed palette (`ConfigHighlightColor`, hex values in `Constants.cs`). The minimap takes any RGBA hex.

| QoLChests palette | Hex |
|---|---|
| Yellow | `FFFF01` |
| Red | `FF0101` |
| Blue | `0101FF` |
| Pink | `FF017F` |
| Orange | `FF8001` |
| Green | `01FF01` |
| Cyan | `01FFFF` |
| Violet | `7F01FF` |
| Magenta | `FF01FF` |
| Gray | `808080` |
| White | `FFFFFF` |

**Interactables**

| Category | Highlight | Highlight color | Minimap kind | Minimap | Minimap active | Minimap inactive | Same color? |
|---|---|---|---|---|---|---|---|
| Chest | on | Yellow `FFFF01` | Chest | on | `FFFF01` | `808080` | yes |
| StealthedChest | on | Yellow `FFFF01` | – | – | – | – | no minimap icon |
| Lockbox | on | Red `FF0101` | – | – | – | – | no minimap icon |
| LunarPod | on | Blue `0101FF` | LunarPod | on | `4D79FF` | `808080` | **no**: lighter blue on map |
| VoidCradle | on | Magenta `FF01FF` | (shown as Chest) | on | `FFFF01` | `808080` | **no**: needs its own icon |
| Barrel | on | Yellow `FFFF01` | Barrel | on | `FFFF01` | `808080` | yes |
| EquipmentBarrel | on | Orange `FF8001` | Equipment | on | `FF8001` | `808080` | yes |
| Shop | on | Cyan `01FFFF` | Shop | on | `01FFFF` | `808080` | yes |
| Duplicator | on | Pink `FF017F` | Printer | on | `FF017F` | `808080` | yes |
| Scrapper | on | Red `FF0101` | Utility | on | `FF0101` | `808080` | yes |
| Drone | on | Green `01FF01` | Drone | on | `01FF01` | `808080` | yes |
| Turret | **off** | Green `01FF01` | (shown as Drone) | on | `01FF01` | `808080` | yes, but no own category |
| Shrine | on | Violet `7F01FF` | Shrine | on | `A04DFF` | `808080` | **no**: lighter violet on map |
| NewtStatue | on | Blue `0101FF` | NewtAltar | on | `4D79FF` | `808080` | **no**: lighter blue on map |
| PressurePlate | on | Gray `808080` | – | – | – | – | no minimap icon |
| Teleporter | – | – | Teleporter | on | `FFFFFF` | `00FF00` (charged) | minimap only |
| Portal | – | – | Portal | on | `FFFFFF` | `808080` | minimap only |
| Totem | – | – | Totem | on | `5EB2F2` | `808080` | minimap only |
| Special (fans, pods) | – | – | Special | on | `FFFFFF` | `808080` | minimap only |

The lighter minimap blues and violet (`4D79FF`, `A04DFF`) look deliberate: pure `0101FF` and `7F01FF` are hard to see on a small dark map. The unified config should therefore allow a minimap color that differs from the highlight color (see 4.4).

**Minimap-only kinds**

| Kind | Minimap | Active | Inactive |
|---|---|---|---|
| EnemyMonster | on | `DC143C` | `808080` |
| EnemyLunar | on | `FF8C8C` | `808080` |
| EnemyVoid | on | `FF00FF` | `808080` |
| Minion | on | `00FF00` | `808080` |
| Player | on | `FFFFFF` | `FFFFFF` |
| Item | on | `5EB2F2` | `00FFFF` |
| Neutral | **off** | `FFFFFF` | `808080` |

**Other QoLChests settings in use:** hide is on for every hideable category, with **Fade = true** and **Time = 1**; **RemoveWhenUsed = true**; **DoNotHideAsDrifter = true**.

## 4. Target design

### 4.1 One category model

Both features use **one enum, `MapCategory`** (`Categories/MapCategory.cs`), and **one object per category, `CategoryDefinition`**, listed in `CategoryCatalog`. Enemies, players, minions, items and neutrals are in the same enum, with no highlight group. Categories are as fine-grained as the game allows (owner decision), about 75 in total:

- **Chests:** small, large, legendary, damage, healing, utility (each covering small and large versions), adaptive, cloaked, secret (in "HOLDER: Secret"), rusty lockbox, encrusted lockbox, void cradle, void potential, lunar pod, shipping request, scavenger bag, hollow, Nkuhana, trial, other.
- **Shops:** multishop, equipment multishop, lunar shop, drone shop, temporary item shop, haunted vending machine, other.
- **Printers and scrappers:** common, uncommon, legendary, boss, other printer; scrapper, drone scrapper.
- **Drones:** drone, turret, drone station, Lemurian egg. **Barrels:** barrel, equipment barrel, void stalk.
- **Shrines, one per type:** Blood, Chance, Combat, Mountain, Order, Woods, cleansing pool, Altar of Gold, Halcyonite, Shaping (Colossus), Rebirth, Catastrophe, Compulsion, Death, Dread, Hallowed, other.
- **World:** newt altar, pressure plate, teleporter, portal, gold totem, radar scanner, special.

`CategoryDefinition` holds the display name, config group, prefab paths, the old QoLChests config group (`HighlightGroup`, null when the category is never highlighted) and the old minimap icon kind (`InteractableKind`). The two old fields only pick default values and migrate the owner's settings into the category's config section (`Config/CategorySettings.cs`). "Other" categories catch untagged objects from other mods. Add a category in `MapCategory` and `CategoryCatalog` only; the catalog throws at startup if a value has no definition. Keep the enum append-only once released.

### 4.2 One classifier

QoLChests' marker approach becomes the single source of truth:

1. `CategoryCatalog` (from `Constants.cs`) maps prefab paths to categories, including teleporters, portals, the gold totem and the radar scanner. All 122 paths were checked against the game's addressables catalog.
2. Startup and scene hooks tag prefabs and preplaced objects with a `CategoryMarker` component (renamed from `InteractableHighlightCategoryMarker`). Preplaced chests take their category from their prefab name. The name-based registry and modded-prefab hooks stay.
3. The minimap scans for `CategoryMarker` (`Minimap/CategoryScanner.cs`) instead of matching context tokens. Generic interactions (portals) stay in the dynamic scan because they spawn mid-stage.
4. `CategoryClassifier` handles untagged interactables with the old minimap rules and tags them. It runs at stage start (before highlights are applied) and on each minimap static scan, and logs each unknown object name once.

### 4.3 One "used" state

`InteractableUsedHandler` stays the only place that decides an interactable is used. The minimap's active check becomes "has no `InteractableUsed` component and, if it has a `PurchaseInteraction`, `available` is true". A per-category config option chooses whether a used icon turns gray (inactive colour) or disappears.

### 4.4 One config

There is one config file, `kaitorque.MapChests.cfg`, with one section per category, named `Group: Category` (for example `Shrine: Shrine of Blood`). Each section holds:

- Highlight, Highlight color, Hide when used (only categories that can be highlighted). The colour is any RGB colour, not the old palette.
- Minimap, Minimap color, Minimap used color, Used icon (Gray or Hidden; not for enemies, players and other entities).
- Icon, Icon width, Icon height and the elevation marker settings (file only). Icons are game sprite paths.

Global sections: `Hide` (time, fade, remove highlight when used, do not hide as Drifter) and `Minimap` (toggle key, zoom keys, zoom level, log level).

Defaults:
- Highlight colour starts from the owner's group colour in section 3 (chests yellow, shops cyan, and so on), so all subtypes of a group match for now. The exceptions: each shrine has its own colour so they can be told apart; the equipment multishop is dark cyan (0099CC); lunar things (pod, shop, newt altar) are royal blue (3366FF), brighter than pure blue for the dark minimap; the legendary chest is gold and the cloaked chest pale yellow; the pressure plate is white; the rusty lockbox is yellow and the encrusted lockbox magenta; the hollow chest is yellow-green; the temporary item shop pale cyan and the vending machine teal; the drone scrapper green, the drone station medium green and the turret mint green with highlight on; the void stalk magenta. Some categories also get their own icon: lock for lockboxes, loot bag for the scavenger bag and hollow chest, drone for the drone shop, shrine for the newt altar, wrench for the cleansing pool (a trade, like the scrapper; same small size, lunar blue). Icons are turned gray when loaded so colours tint cleanly; the game's wrench sprite is yellow. The full list with descriptions is in `Colors.md`.
- Minimap colour starts as the highlight colour (owner decision; this replaced the old lighter map shades for blue and violet). The two stay separate settings. Categories that are never highlighted keep the owner's minimap colours.
- When `kaitorque.MapChests.cfg` does not exist yet, the values are migrated from `Faust.QoLChests.cfg` and `MiniMap.cfg` (or the Phase 0 copies). Palette colour names become RGB colours. The old files are never changed.

| Shrine | Colour | Shrine | Colour |
|---|---|---|---|
| Blood | FF3355 | Halcyonite | D9B36C |
| Chance | FFD24D | Shaping (Colossus) | 01FF01 |
| Combat | FF01FF | Rebirth | FF66CC |
| Mountain | 00CCFF | Catastrophe | 9933FF |
| Order | CCFF33 | Compulsion | FF8001 |
| Woods | 1AB3A6 | Death | FFFFFF |
| Cleansing pool | 3366FF (lunar) | Dread | FFCC99 |
| Altar of Gold | FFD24D | Hallowed | FFFF99 |
| Ancient Sepulcher | CC99FF | Other | FF8C8C |

Risk of Options shows the global hide and zoom settings and, per category, highlight on/off, highlight colour, hide when used, minimap on/off, minimap colour and used icon. Any change re-applies highlights and rebuilds the minimap mid-run.

### 4.5 Project layout

```
MapChests/
  MapChests.csproj            single project, single DLL
  MapChestsPlugin.cs          [BepInPlugin("kaitorque.MapChests", "MapChests", "0.1.0")]
  Api/                        QoLChestsApi (to be renamed)
  Categories/                 InteractableCategory, EntityKind, CategoryCatalog, CategoryMarker, registry
  State/                      used-state handler and components
  Highlight/                  highlighting, hiding and fading (from QoLChests)
  Hooks/                      game hooks (from QoLChests)
  Minimap/                    minimap UI, scanners, tracked objects (from MiniMapMod); Minimap/Library/ from MiniMapLibrary
  Config/                     the one config, Risk of Options registration
  Compat/                     Quality, Sandswept, Starstorm 2, Hunk, Risk of Options (from QoLChests + Faust.Shared)
  libs/RiskOfOptions.dll      Risk of Options reference (not on NuGet)
  manifest.json               package manifest
  package.ps1                 build, copy into the VP profile, zip (modelled on MonsoonLikeDrizzle)
MapChests.sln, NuGet.Config   solution and package sources (the BepInEx feed is required)
```

Namespaces are still the originals (`Faust.QoLChests.*`, `Faust.Shared`, `MiniMapMod`, `MiniMapLibrary`); rename them when the code is reworked in later phases.

- Use the NuGet references from `QoLChests.csproj` for game libraries. Delete `MiniMapMod/libs/` from the repo; those are the game's own DLLs and should not be committed.
- Add `[BepInIncompatibility("MiniMap")]` and `[BepInIncompatibility("Faust.QoLChests")]` so it never runs alongside the originals.
- Keep `QoLChestsApi` under a MapChests name. Other mods detect QoLChests by its GUID, so they won't find this plugin; the compat handlers already register those mods' prefabs from our side.

## 5. Work phases

Each phase should end with a successful build and an in-game check in the VP profile.

### Phase 0 – Build as one plugin, no behaviour change
- [x] Create `MapChests.csproj` with the NuGet references; compile all four source folders into one DLL.
- [x] One `MapChestsPlugin` whose `Awake` runs the existing QoLChests and minimap startup code. The minimap plugin class became the `MinimapController` component.
- [x] Move files with `git mv` into the target layout.
- [x] `package.ps1` builds and copies the DLL into `profiles\VP\BepInEx\plugins\MapChests\`.
- [x] In game, with the two original mods disabled: minimap shows, chests highlight and hide as before.

Phase 0 used temporary files `kaitorque.MapChests.Highlight.cfg` and `kaitorque.MapChests.Minimap.cfg`. Phase 2 replaced them with `kaitorque.MapChests.cfg`; the temporary files are only read once, for migration, and can be deleted afterwards.

### Phase 1 – Shared categories
- [x] Introduce `MapCategory`, `CategoryDefinition` (4.1) and `CategoryMarker`; port QoLChests to it.
- [x] Minimap scans `CategoryMarker` and maps category to icon; add the fallback classifier (4.2).
- [x] In game: every row of the comparison table shows the same category in both features. The Ancient Sepulcher is enabled by the stage after it starts, so it is tracked from `WakeController.Start`.

Expected visible changes after Phase 1 (until Phase 2 gives each category its own settings): lockboxes, void potentials, scavenger bags, void stalks, Lemurian eggs, drone stations, the radar scanner and DLC shrines now get minimap icons; scavenger bags, void potentials and void stalks also highlight and hide using the nearest old group (chest, void cradle, barrel). Cloaked chests and pressure plates still have no icon. Generic interactions no longer get two icons.

### Phase 2 – Shared state and config
- [x] Minimap active check uses the shared used state (4.3).
- [x] Merge configs into the per-category layout (4.4) with Risk of Options entries.
- [x] Each category has its own highlight and minimap colour, defaulting to the owner's mapping in section 3 (shrines get distinct defaults).
- [x] Per-category option: used icon turns gray or disappears.
- [x] Per-category minimap icon setting. Icons are game sprites only (`LegacyResourcesAPI` paths such as `Textures/MiscIcons/...`); the mod ships no image assets. The game has no book icon, so the Ancient Sepulcher keeps the shrine icon.
- [ ] In game: settings migrated from the old files, shrine colours, used icons gray or hidden, Risk of Options changes apply mid-run.

### Phase 3 – Minimap fixes for the current game
Verify each item in game, then fix it:
- [ ] Missing icons: DLC shrines, lockboxes, cloaked chests (now its own category), drone scrapper, pressure plates.
- [ ] Objects spawned after the stage starts: static objects are scanned once per stage (`ScanStaticTypes`), so late spawns (Halcyonite shrines, trial chests) can be missed.
- [ ] List-removal bug: `ClearDynamicTrackedObjects` and `UpdateIconPositions` call `RemoveAt(i)` inside a forward `for` loop without `i--`, which skips the next element.
- [ ] Icons load through `LegacyResourcesAPI` paths (`Textures/MiscIcons/...`); confirm they still resolve in 1.5.0.
- [ ] Zoom-out bug (3.2, item 2): take the map area per stage from the scene, never from enemies or items, and fix `Range1D` (port from section 3.1).
- [x] `Log.LogException` (`Minimap/Log.cs`) loops forever when an exception has no inner exception, because `head` is only advanced when `InnerException` is not null. Any exception caught in `ScanScene` would freeze the game.
- [ ] `GlobalEventManager.onCharacterDeathGlobal` is obsolete in 1.5.0 (use `onCharacterDeathServer` or `onDeathProcServer`); check it still fires on clients.
- [ ] Lag bug (3.2, item 1): find out in game what the Sky Meadow stones are; stop rebuilding every dynamic icon each scan (the duplicate generic-interaction scanner was removed in Phase 1); consider registry-based tracking (section 3.1).

### Phase 4 – ArtifactOfParallel compatibility
ArtifactOfParallel (`F:\Mod\RoR2\ArtifactOfParallel`, owner's mod, still in design; see its `DESIGN.md`) is an artifact that gives each player their own private chests, shrines of chance, multishops, etc. Only the owner can see and open them; whatever drops out is shared. Other players' private objects are never sent to a client, but the **host** has all of them. Objects can also become visible or hidden mid-stage: a dead or disconnected player's chests become shared, and go back to them on revival or reconnect.

What breaks today:

| Where | Situation | Result |
|---|---|---|
| Client | Stage start | Works: other players' objects don't exist on the client |
| Client | An object appears mid-stage | Not highlighted and no minimap icon until next stage; only `StageEventHooks` types are tracked late |
| Client | An object disappears mid-stage | Highlight goes with it; the minimap icon stays as "used" (the `ActiveChecker` throws on the destroyed object and `CheckActive` swallows it) |
| Host | Stage start | Every player's objects get highlights and minimap icons, because `FindObjectsOfType<CategoryMarker>()` finds them and `HighlightHandler.Enable` turns `isOn` back on over ArtifactOfParallel's hiding |

**The API is ready** (ArtifactOfParallel phase 5, `ParallelApi.cs`). Plugin GUID `kaitorque.ArtifactOfParallel`; reference `ArtifactOfParallel.dll` from the profile's `plugins\Kaitorque-ArtifactOfParallel` folder with `Private=false`.

```csharp
namespace ArtifactOfParallel
{
    public static class ParallelApi
    {
        // True if the local player should see this object: not private, owned by the local player, or currently shared.
        // Also true for objects ArtifactOfParallel doesn't manage; false for a null/destroyed object.
        public static bool IsVisibleToLocalPlayer(GameObject obj);

        // Host and clients: an object became visible (true) or hidden (false) for the local player.
        public static event Action<GameObject, bool> LocalVisibilityChanged;
    }
}
```

How the event behaves:
- Host: raised whenever a private object's visibility flips (owner died, disconnected, revived or reconnected).
- Client: raised when another player's object is network-spawned mid-stage (`true`) or removed (`false`). Not raised during the first 2 seconds of a stage, so the normal stage-start scan covers those.
- It may repeat for an object already in that state; handle it idempotently.
- The object passed is the root of the interactable (for multishops, each terminal and the shop itself are raised separately).
- Call the API through a separate class marked `[MethodImpl(MethodImplOptions.NoInlining)]` so MapChests loads without ArtifactOfParallel installed.

Tasks (soft dependency, guarded by `Chainloader.PluginInfos.ContainsKey("kaitorque.ArtifactOfParallel")`; MapChests must still work without it):
- [x] Skip objects where `IsVisibleToLocalPlayer` is false in `InteractableStateHandler.TrackInteractables`, `TrackLateInteractable`, `HighlightHandler.Enable` and `CategoryScanner.ScanScene`.
- [x] Subscribe to `LocalVisibilityChanged`: `true` calls `InteractableStateHandler.TrackLateInteractable(obj)`, `false` calls the new `HideInteractable(obj)` (removes the highlight but keeps the category marker, so the object can come back). Both raise `LateInteractableChanged`, which resets the minimap.
- [x] Destroyed backing objects: already removed by `MinimapController.UpdateIconPositions` (`item.gameObject == null` uses Unity's null check), so no change needed.

Done in `Compat/ParallelCompat.cs`: finds `ArtifactOfParallel.ParallelApi` by reflection (no build reference), soft `BepInDependency` so it loads first, and `IsVisible` returns true when the mod is missing.
- [ ] In game with ArtifactOfParallel, host + client: each player only sees highlights and icons for their own and shared objects; a dead player's chests appear for the others and disappear again on revival.

## 6. Owner decisions

1. Minimap bugs: missing interactables, plus the two bugs in section 3.2.
2. Used icons: gray or hidden, chosen per category in the config.
3. Colours: every category, including each chest subtype, has its own dedicated colour for both highlight and minimap. Defaults come from the color mapping in section 3; categories without a minimap colour yet (void cradle, lockbox, cloaked chest, pressure plate) default to their highlight colour.
4. Turrets: own category, separate from drones. In general, split categories as finely as the game allows.
