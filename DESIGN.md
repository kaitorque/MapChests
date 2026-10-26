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

Replace both enums with one enum, `InteractableCategory`, covering the union of the interactable rows above. The owner wants categories **as fine-grained as possible**, each with its own colour, so split broad groups into their real types:

- **Chests:** separate small, large, legendary, category chests (damage, healing, utility), adaptive, lockbox (rusty), encrusted lockbox, cloaked (stealthed) chest, hidden or secret chests, timed chest, void cradle, lunar pod, and the DLC chests.
- **Drones and turrets:** turret is its own category, never merged into drones. Split further where the game has distinct types (drone stations, Lemurian eggs).
- **Others:** keep `Barrel, EquipmentBarrel, Shop, Duplicator, Scrapper, Shrine, NewtStatue, PressurePlate, Teleporter, Portal, Totem, Special`, and split them where distinct types exist (for example printer tiers, shrine types).

The exact list is built in Phase 1 from the prefab paths in `QoLChests/Constants.cs`, the decompiled game source and the token list in section 3.1. QoLChests' enum values are part of its public API ("only append, never renumber"); MapChests has its own API, so it can renumber, but should stay append-only once released.

Keep the non-interactable minimap kinds (enemies, players, minions, items, neutral) in a separate enum, `EntityKind`. They have no highlight or hide behaviour.

### 4.2 One classifier

QoLChests' marker approach becomes the single source of truth:

1. A `CategoryCatalog` (from `Constants.cs`) maps prefab paths to categories. Add rows for categories QoLChests did not cover (teleporter, portals, totems, fans) where a prefab path exists.
2. Startup and scene hooks tag prefabs and preplaced objects with a `CategoryMarker` component (renamed from `InteractableHighlightCategoryMarker`). Keep the name-based registry and modded-prefab hooks.
3. The minimap scans for `CategoryMarker` instead of matching context tokens, and picks each icon from the marker's category.
4. A **fallback classifier** handles untagged interactables (for example from other mods) using the old minimap rules. It logs each unknown prefab name once, so missing catalog rows are easy to spot.

### 4.3 One "used" state

`InteractableUsedHandler` stays the only place that decides an interactable is used. The minimap's active check becomes "has no `InteractableUsed` component and, if it has a `PurchaseInteraction`, `available` is true". A per-category config option chooses whether a used icon turns gray (inactive colour) or disappears.

### 4.4 One config

There is one config file, `kaitorque.MapChests.cfg`, with one section per category. Each section holds:

- Color: every category (including each chest subtype) has its own dedicated colour, covering both the highlight and the minimap icon
- Highlight: enabled, palette color
- Hide when used: enabled (categories that support it)
- Minimap: enabled, icon, width, height, inactive color, used icon (gray or hidden), elevation marker settings. The minimap active color stays a separate hex value from the highlight palette color (the owner uses lighter shades on the map for blue and violet). Defaults come from the "Current color mapping" table in section 3.

Global sections cover the minimap (toggle key, zoom keys, zoom level, log level) and hiding (hide time, fade instead of hide, remove highlight when used, do-not-hide-as-Drifter). Register everything with Risk of Options, which is installed. Changes should apply mid-run wherever QoLChests already supports that.

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
- [ ] In game, with the two original mods disabled: minimap shows, chests highlight and hide as before.

Until Phase 2 merges the configs, the two features use temporary files `kaitorque.MapChests.Highlight.cfg` and `kaitorque.MapChests.Minimap.cfg`. On first run they are copied from `Faust.QoLChests.cfg` and `MiniMap.cfg`, so the owner's settings carry over; the original files are never changed.

### Phase 1 – Shared categories
- [ ] Introduce `InteractableCategory` (4.1) and `CategoryMarker`; port QoLChests to it.
- [ ] Minimap scans `CategoryMarker` and maps category to icon; add the fallback classifier (4.2).
- [ ] Every row of the comparison table shows the same category in both features.

### Phase 2 – Shared state and config
- [ ] Minimap active check uses the shared used state (4.3).
- [ ] Merge configs into the per-category layout (4.4) with Risk of Options entries.
- [ ] Each category has its own highlight and minimap colour, defaulting to the owner's mapping in section 3 (new subtypes get distinct defaults).
- [ ] Per-category option: used icon turns gray or disappears.

### Phase 3 – Minimap fixes for the current game
Verify each item in game, then fix it:
- [ ] Missing icons: DLC shrines, lockboxes, cloaked chests (now its own category), drone scrapper, pressure plates.
- [ ] Objects spawned after the stage starts: static objects are scanned once per stage (`ScanStaticTypes`), so late spawns (Halcyonite shrines, trial chests) can be missed.
- [ ] List-removal bug: `ClearDynamicTrackedObjects` and `UpdateIconPositions` call `RemoveAt(i)` inside a forward `for` loop without `i--`, which skips the next element.
- [ ] Icons load through `LegacyResourcesAPI` paths (`Textures/MiscIcons/...`); confirm they still resolve in 1.5.0.
- [ ] Zoom-out bug (3.2, item 2): take the map area per stage from the scene, never from enemies or items, and fix `Range1D` (port from section 3.1).
- [ ] `Log.LogException` (`Minimap/Log.cs`) loops forever when an exception has no inner exception, because `head` is only advanced when `InnerException` is not null. Any exception caught in `ScanScene` would freeze the game.
- [ ] `GlobalEventManager.onCharacterDeathGlobal` is obsolete in 1.5.0 (use `onCharacterDeathServer` or `onDeathProcServer`); check it still fires on clients.
- [ ] Lag bug (3.2, item 1): find out in game what the Sky Meadow stones are; stop rebuilding every dynamic icon each scan; remove the duplicate generic-interaction scanner; consider registry-based tracking (section 3.1).

## 6. Owner decisions

1. Minimap bugs: missing interactables, plus the two bugs in section 3.2.
2. Used icons: gray or hidden, chosen per category in the config.
3. Colours: every category, including each chest subtype, has its own dedicated colour for both highlight and minimap. Defaults come from the color mapping in section 3; categories without a minimap colour yet (void cradle, lockbox, cloaked chest, pressure plate) default to their highlight colour.
4. Turrets: own category, separate from drones. In general, split categories as finely as the game allows.
