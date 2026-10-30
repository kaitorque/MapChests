# MapChests

A Risk of Rain 2 mod combining **MiniMapMod** and **QoLChests** into a single plugin: a minimap and interactable highlights that share one set of categories, colours and icons.

It is made mainly to be used with my [ArtifactOfParallel](https://github.com/kaitorque/ArtifactOfParallel) mod, which gives each player their own private chests: the minimap and highlights only show your own and shared objects, never another player's. It also works fine without it.

> **Disclaimer:** MapChests is an unofficial, fan-made merge. It is not affiliated with or endorsed by the original authors. It is made mainly for me and my friends to use together. Neither original mod has a license, so all rights to their code stay with their authors. **If you are the author of MiniMapMod or QoLChests and want this taken down, open an issue on [the GitHub page](https://github.com/kaitorque/MapChests/issues) and it will be removed.**
>
> Please report problems with MapChests to [MapChests](https://github.com/kaitorque/MapChests/issues), not to the original authors.

## Credits

MapChests is built on the work of these mods. All credit for the original work goes to their authors; their full commit histories are kept in this repository.

| Original mod | Author | Source | Thunderstore |
|---|---|---|---|
| MiniMapMod | Arty Redd (DekuDesu), with contributions from Arcafanetiz and Ultimeit | [artyredd/MiniMapMod](https://github.com/artyredd/MiniMapMod) | [DekuDesu-MiniMapMod](https://thunderstore.io/package/DekuDesu/MiniMapMod/) |
| QoLChests | Faustvii | [Faustvii/R2Mods](https://github.com/Faustvii/R2Mods) (`QoLChests/` and `Faust.Shared/`) | [Faustvii-QoLChests](https://thunderstore.io/package/Faustvii/QoLChests/) |

If you like these features, please support the original mods.

The icon uses the game's own chest sprite. Risk of Rain 2 and its assets belong to Hopoo Games and Gearbox.

---

## Features

- **Highlights** interactables in the world with a coloured outline, and hides or fades them a moment after they're used.
- **Minimap** with an icon for every interactable, plus enemies, allies, players and dropped items. Used interactables turn gray or disappear.
- **One category per kind of interactable** (small chest, Shrine of Chance, equipment multishop, and so on), shared by the highlight and the minimap, each with its own colour and icon.
- **Colour system** that draws your eye to what gives items:
    1. Item sources (chests, lockboxes, shops, printers, scrappers, void and lunar item sources, drones) are bright, one colour per family.
    2. Shrines are muted versions of their sigil colour in game.
    3. Barrels and void stalks (money or coins only) are dim.

  The full list of colours and icons is in [Colors.md](https://github.com/kaitorque/MapChests/blob/main/Colors.md).
- Tracks objects that appear mid-stage: Mimics, the Hallowed shrine, Ancient Sepulcher, void potentials and more.
- Works with [ArtifactOfParallel](https://github.com/kaitorque/ArtifactOfParallel) (private chests per player): you only see highlights and icons for your own and shared objects.

## Controls

- `M` shows or hides the minimap.
- `=` and `-` zoom the minimap in and out. The zoom level is saved; icons keep their size when zooming.

## Config

One file, `kaitorque.MapChests.cfg`, with one section per category: highlight on/off and colour, hide when used, minimap on/off, colour, used colour, used icon (gray or hidden), and the icon itself (a game sprite path), its size and elevation marker. Global sections cover hiding (time, fade, Drifter) and the minimap (keys, zoom, icon scale).

With [Risk Of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) installed, most settings can be changed in game and apply immediately.

The first time it runs, MapChests copies your settings from `Faust.QoLChests.cfg` and `MiniMap.cfg` if you have them. Those files are never changed.

## Installation

Import the zip in r2modman (Settings, then **Import local mod**). Disable MiniMapMod and QoLChests: MapChests replaces both and won't load alongside them.

Requires BepInExPack and HookGenPatcher. Risk Of Options and ArtifactOfParallel are optional.

## Building

Requires the .NET SDK. Risk Of Options is referenced from the r2modman profile set in `MapChests/MapChests.csproj` (`Profile`).

```powershell
.\MapChests\package.ps1
```

This builds the plugin, installs it into the r2modman profile and creates an r2modman-importable zip next to the repo folder. `tools/make_icon.py` rebuilds `icon.png` from the game's files (needs Python with UnityPy and Pillow).

## License

No license. The original mods were published without one, so MapChests can't be given an open-source license either. The code is shared here for transparency and so the original authors can see what was changed. See the disclaimer at the top.
