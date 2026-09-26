# Quality of life chests

- Hides chests when they are empty.
- Hides used shop terminals.
- Highlights chests and interactables.

For feature suggestions or bug reports go [here](https://github.com/Faustvii/R2Mods/issues)

## Changelog

### 1.3.3

- **Breaking config change:** split Chest and Barrel settings into category-specific settings. Review the migration notes above after updating.
- Added independent highlight, color, and hide settings for Equipment Barrels, Lunar Pods, and Void Cradles, plus separate hide behavior for Stealthed Chests.
- Fixed Equipment Barrel hide behavior and Quality interactable classification.
- Added support for interactables from Quality(Goorakh)
- Added support for interactables from Sandswept

#### Breaking configuration change in 1.3.3

**This release splits chest and barrel categories and intentionally changes configuration entries. Review your QoLChests config after updating.**

| Previous setting | Replacement / new behavior |
| ---------------- | -------------------------- |
| `Hide → Chest` | Still controls standard chests; use `Hide → Stealthed Chests`, `Hide → Lunar Pod`, and `Hide → Void Cradle` for those categories. |
| `Hide → Barrels` | Still controls standard barrels; `Hide → Equipment Barrel` controls equipment barrels. |
| `Highlight → Chest` | Still controls standard chests; use `Highlight → Lunar Pod` and `Highlight → Void Cradle` for those categories. |
| `Highlight → Barrels` | Still controls standard barrels; `Highlight → Equipment Barrel` controls equipment barrels. |
| `Highlight → ChestColor` | Still controls standard chests; use `Highlight → Lunar Pod Color` and `Highlight → Void Cradle Color` for those categories. |
| `Highlight → BarrelColor` | Still controls standard barrels; use `Highlight → Equipment Barrel Color` for equipment barrels. |

### 1.3.2

- Added highlights to newt statues
- Added highlights to pressure plates
- Moved barrels into their own category
- Fixed a null pointer happening sometimes in interactable spawn hooks.
- Better handling of drifter detection (It might actually work now..)

### 1.3.1

This release might have unexpected bugs or issues when playing as drifter. (Hopefully better than the current state though)

- Added missing Alloy Collective interactables to be highlighted
- Added a setting to stop hide of used interactbles from happening when playing as drifter. (First iteration)

### 1.3.0

- Update to the new Alloy Collective update.

### 1.2.4

- Fix uploaded version (1.2.3 did not contain the updated changes)

### 1.2.3

- Fixed stealthed chests being put into normal chest config category
- Added separate option for lockbox highlighting
- Fixed spelling mistake in config entry (Turrent highlight color might need to be reset)

### 1.2.2

- Fixed Gilded Coast not having highlights
- Fixed Prime Meridian not having highlights

### 1.2.1

- Fixed a compatability issue with Hunk

### 1.2.0

#### New Features

- More colors available to use for highlighting
- You can now choose a color per category
- Added highlighting of Starstorm2 drones
- Added highlight of lockboxes

#### Fixes

- You should now be able to change settings with Risk of Options mid run and have them take effect immediately
- Removed collision with hidden chests
- Fixed Large category chests not being hidden/faded at all
- Fixed category chests "symbol" not being hidden/faded
- Fixed Shipping manifest terminals "fire" effect sticking around after hiding the rest
- Fixed Hide/Fade of shops only working when you are the host

### 1.1.11

- Forgot to add a hook for setting updates

### 1.1.10

- Added back fading (It is super hacky though, but kind of works)
- Added back hide/fade delay
- Updated some descriptions for config options, so it is eaiser to know which requires restart / scene change to take affect.

### 1.1.9

- Fixed issues with Artifact of delusion
- Highlighting of Lemurian eggs (Artifact of devotion)
- Added highlighting of void cradles
- Added highlighting of some missing large chests

### 1.1.8

- Added option to toggle highlight of cloaked chests off.
- You can now change highlight color through Risk Of Options as well.

### 1.1.7

- Added Risk of options integration
- Fade out time is now affected by configuration.

### 1.1.6

- Added Fade instead of hide option (Based on FadeEmptyChest mod)
- Added highlight color option

### 1.1.5

- Fixed dependency issue with BetterUnityPlugin not being loaded

### 1.1.4

- Fixed issue with Executive Card
- Fixed issue with Shipping Request Form

### 1.1.3

- Updated to new Risk of rain patch

### 1.1.2

- Added option to disable hiding of used interactables.
- Added option to remove highlighting from used interactables.

### 1.1.1

- Split highlights into more categories and added options for enabling/disabling them. (Chests, Turrets, Drones, Shops, Scrapper, Duplicator)
- Adaptive chest now also hides after use.

### 1.1.0

- Shop Terminals now also disappear when used.
- It's possible to highlight drones/turrets aswell
- Shops / Chests are hidden quicker by default, it's configurable now.

### 1.0.0

- Release of QoL chests.
