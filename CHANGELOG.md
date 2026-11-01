# Changelog

## 1.0.1

- Fixed stutter in multiplayer with ArtifactOfParallel: when an interactable appears, is revealed (Mimic) or changes visibility, only its own minimap icon is updated instead of rebuilding the whole minimap.
- Fixed hitches in big fights: the regular rescan of enemies, allies and items keeps existing icons and only adds or removes the ones that changed, instead of recreating every icon.
- A single broken icon is dropped instead of resetting the whole minimap.

## 1.0.0

First public release. MapChests merges [MiniMapMod](https://thunderstore.io/package/DekuDesu/MiniMapMod/) 4.0.0 by DekuDesu and [QoLChests](https://thunderstore.io/package/Faustvii/QoLChests/) 1.3.6 by Faustvii into one plugin. Behaviour differs from both originals.

- One plugin (`kaitorque.MapChests`) for the minimap and the highlights. Won't load alongside MiniMapMod or QoLChests.
- One category per kind of interactable (small chest, Shrine of Chance, equipment multishop, and so on), shared by the highlight and the minimap. Each has its own colour, used colour, icon, icon size and elevation marker.
- One config file, `kaitorque.MapChests.cfg`. On first run, settings are copied from `Faust.QoLChests.cfg` and `MiniMap.cfg` if present (those files are not changed).
- New colour scheme: item sources bright, shrines muted in their sigil colour, barrels and void stalks dim.
- New categories: Mimic, Ancient Sepulcher, Hallowed shrine, cloaked chests and pressure plates (shown by default), and more.
- Used interactables turn gray or disappear on the minimap. Shrines lose their highlight once used up, on the host and clients; shrines with uses left keep it. The Ancient Sepulcher counts as used once activated.
- Tracks objects that appear mid-stage: Mimics, the Hallowed shrine, Ancient Sepulcher, void potentials and more.
- [ArtifactOfParallel](https://github.com/kaitorque/ArtifactOfParallel) support: you only see highlights and icons for your own and shared objects, and changes mid-stage are followed.
- Minimap no longer shrinks during a stage, and uses one scale for both axes.
- Zoom level is saved. Icons keep their size when zooming; new `Icon scale` setting resizes every icon.
- Fixed game sprites with trimmed padding looking stretched on the minimap.
- Fixed a minimap bug when removing objects from the tracked list.
- Settings can be changed in game with [Risk Of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/).
- Updated for Risk of Rain 2 1.5.0.
