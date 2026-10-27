# MapChests

A Risk of Rain 2 mod combining MiniMapMod and QoLChests into a single plugin. Work in progress; not published.

## Credits

This project is a fork of two mods, with their full commit histories preserved:

- [MiniMapMod](https://github.com/DekuDesu/MiniMapMod) by Arty Redd / DekuDesu, with contributions from Arcafanetiz and Ultimeit.
- [QoLChests](https://github.com/Faustvii/R2Mods) by Faustvii (`QoLChests/` and `Faust.Shared/` from the R2Mods repository).

All credit for the original work goes to its authors.

---

## Features

- **Highlights** interactables in the world with a coloured outline, and hides or fades them a moment after they're used.
- **Minimap** with an icon for every interactable, plus enemies, allies, players and dropped items. Used interactables turn gray or disappear.
- **One category per kind of interactable** (small chest, Shrine of Chance, equipment multishop, and so on), shared by the highlight and the minimap, each with its own colour and icon.
- **Colour system** that draws your eye to what gives items:
    1. Item sources (chests, lockboxes, shops, printers, scrappers, void and lunar item sources, drones) are bright, one colour per family.
    2. Shrines are muted versions of their sigil colour in game.
    3. Barrels and void stalks (money or coins only) are dim.

  The full list of colours and icons is in `Colors.md`.
- Tracks objects that appear mid-stage: Mimics, the Hallowed shrine, Ancient Sepulcher, void potentials and more.
- Works with Artifact of Parallel (private chests per player): you only see highlights and icons for your own and shared objects.

## Controls

- `M` shows or hides the minimap.
- `=` and `-` zoom the minimap in and out. The zoom level is saved.

## Config

One file, `kaitorque.MapChests.cfg`, with one section per category: highlight on/off and colour, hide when used, minimap on/off, colour, used colour, used icon (gray or hidden), and the icon itself (a game sprite path), its size and elevation marker. Global sections cover hiding (time, fade, Drifter) and the minimap (keys, zoom).

With Risk of Options installed, most settings can be changed in game and apply immediately.

The first time it runs, MapChests copies your settings from `Faust.QoLChests.cfg` and `MiniMap.cfg` if you have them. Those files are never changed.

## Installation

Import the zip in r2modman (Settings, then **Import local mod**). Disable MiniMapMod and QoLChests: MapChests replaces both and won't load alongside them.

Requires BepInExPack. Risk of Options and Artifact of Parallel are optional.
