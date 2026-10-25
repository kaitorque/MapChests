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

## 4. Target design

### 4.1 One category model

Replace both enums with one enum, `InteractableCategory`, covering the union of the interactable rows above:

`Chest, StealthedChest, Lockbox, LunarPod, VoidCradle, Barrel, EquipmentBarrel, Shop, Duplicator, Scrapper, Drone, Turret, Shrine, NewtStatue, PressurePlate, Teleporter, Portal, Totem, Special`

Keep the non-interactable minimap kinds (enemies, players, minions, items, neutral) in a separate enum, `EntityKind`. They have no highlight or hide behaviour.

### 4.2 One classifier

QoLChests' marker approach becomes the single source of truth:

1. A `CategoryCatalog` (from `Constants.cs`) maps prefab paths to categories. Add rows for categories QoLChests did not cover (teleporter, portals, totems, fans) where a prefab path exists.
2. Startup and scene hooks tag prefabs and preplaced objects with a `CategoryMarker` component (renamed from `InteractableHighlightCategoryMarker`). Keep the name-based registry and modded-prefab hooks.
3. The minimap scans for `CategoryMarker` instead of matching context tokens, and picks each icon from the marker's category.
4. A **fallback classifier** handles untagged interactables (for example from other mods) using the old minimap rules. It logs each unknown prefab name once, so missing catalog rows are easy to spot.

### 4.3 One "used" state

`InteractableUsedHandler` stays the only place that decides an interactable is used. The minimap's active check becomes "has no `InteractableUsed` component and, if it has a `PurchaseInteraction`, `available` is true". A per-category option (open question in section 6) chooses whether a used icon turns gray or disappears once the object is hidden.

### 4.4 One config

There is one config file, `kaitorque.MapChests.cfg`, with one section per category. Each section holds:

- Highlight: enabled, color
- Hide when used: enabled (categories that support it)
- Minimap: enabled, icon, width, height, active color, inactive color, elevation marker settings

Global sections cover the minimap (toggle key, zoom keys, zoom level, log level) and hiding (hide time, fade instead of hide, remove highlight when used, do-not-hide-as-Drifter). Register everything with Risk of Options, which is installed. Changes should apply mid-run wherever QoLChests already supports that.

### 4.5 Project layout

```
MapChests/
  MapChests.csproj            single project, single DLL
  MapChestsPlugin.cs          [BepInPlugin("kaitorque.MapChests", "MapChests", "0.1.0")]
  Categories/                 InteractableCategory, EntityKind, CategoryCatalog, CategoryMarker, registry
  State/                      used-state handler and components
  Highlight/                  highlighting, hiding and fading (from QoLChests)
  Minimap/                    minimap UI, scanners, tracked objects (from MiniMapMod + MiniMapLibrary)
  Config/                     the one config, Risk of Options registration
  Compat/                     Quality, Sandswept, Starstorm 2, Hunk, Risk of Options (from QoLChests + Faust.Shared)
  package.ps1                 build, copy into the VP profile, zip (modelled on MonsoonLikeDrizzle)
```

- Use the NuGet references from `QoLChests.csproj` for game libraries. Delete `MiniMapMod/libs/` from the repo; those are the game's own DLLs and should not be committed.
- Add `[BepInIncompatibility("MiniMap")]` and `[BepInIncompatibility("Faust.QoLChests")]` so it never runs alongside the originals.
- Keep `QoLChestsApi` under a MapChests name. Other mods detect QoLChests by its GUID, so they won't find this plugin; the compat handlers already register those mods' prefabs from our side.

## 5. Work phases

Each phase should end with a successful build and an in-game check in the VP profile.

### Phase 0 – Build as one plugin, no behaviour change
- [ ] Create `MapChests.csproj` with the NuGet references; compile all four source folders into one DLL.
- [ ] One `MapChestsPlugin` whose `Awake` runs the existing QoLChests and minimap startup code.
- [ ] Move files with `git mv` into the target layout.
- [ ] `package.ps1` builds and copies the DLL into `profiles\VP\BepInEx\plugins\MapChests\`.
- [ ] In game, with the two original mods disabled: minimap shows, chests highlight and hide as before.

### Phase 1 – Shared categories
- [ ] Introduce `InteractableCategory` (4.1) and `CategoryMarker`; port QoLChests to it.
- [ ] Minimap scans `CategoryMarker` and maps category to icon; add the fallback classifier (4.2).
- [ ] Every row of the comparison table shows the same category in both features.

### Phase 2 – Shared state and config
- [ ] Minimap active check uses the shared used state (4.3).
- [ ] Merge configs into the per-category layout (4.4) with Risk of Options entries.
- [ ] Option: minimap icon color follows the category's highlight color.

### Phase 3 – Minimap fixes for the current game
Verify each item in game, then fix it:
- [ ] Missing icons: DLC shrines, lockboxes, cloaked chests (now its own category), drone scrapper, pressure plates.
- [ ] Objects spawned after the stage starts: static objects are scanned once per stage (`ScanStaticTypes`), so late spawns (Halcyonite shrines, trial chests, copies from loot-instancing mods) can be missed.
- [ ] List-removal bug: `ClearDynamicTrackedObjects` and `UpdateIconPositions` call `RemoveAt(i)` inside a forward `for` loop without `i--`, which skips the next element.
- [ ] Icons load through `LegacyResourcesAPI` paths (`Textures/MiscIcons/...`); confirm they still resolve in 1.5.0.
- [ ] Any bugs the owner reports (section 6).

### Phase 4 – Optional: loot-instancing awareness
Mods like Equilibrium or InstancedLoot give each player their own copy of a chest in the same spot. The minimap then draws every copy, and unopened copies of other players cover yours. If the owner wants this, show only the copies the local player can use. No instancing mod is installed right now, so leave this until asked.

## 6. Open questions for the owner

1. Which minimap bugs and missing features do you see in game? (Feeds Phase 3.)
2. Once a used chest is hidden, should its minimap icon disappear, or stay in the inactive (gray) color?
3. Should minimap icon colors follow the highlight colors by default, or keep the minimap's own colors?
4. Should turrets keep their own category and icon, separate from drones?
