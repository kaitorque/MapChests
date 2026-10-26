# MapChests colours and icons

The current values from `kaitorque.MapChests.cfg` (VP profile). Colours are hex RGB.

- **Highlight**: the outline colour in the world. A dash means the category is never highlighted.
- **Map**: the minimap icon colour. Used icons turn gray (808080) unless the table says otherwise.
- **Your choice**: fill in the highlight colour, map colour and icon you want. Leave a cell empty to keep the current value.

Notes:
- **Equipment multishop**: the new default is orange (FF8001), but your file still has cyan.
- **Pressure plate**: gray, the same as a used icon, so on the map it always looks used.
- **Mimic**: new category. It looks exactly like a normal chest until it reveals itself, then it shows as an enemy.

## Chests

| Category | Highlight | Map | Icon | Your choice |
|---|---|---|---|---|
| Small chest | FFFF01 | FFFF01 | chest | |
| Large chest | FFFF01 | FFFF01 | chest | |
| Legendary chest | FFFF01 | FFFF01 | chest | |
| Damage chest | FFFF01 | FFFF01 | chest | |
| Healing chest | FFFF01 | FFFF01 | chest | |
| Utility chest | FFFF01 | FFFF01 | chest | |
| Adaptive chest | FFFF01 | FFFF01 | chest | |
| Cloaked chest | FFFF01 | FFFF01 | chest | |
| Secret chest | FFFF01 | FFFF01 | chest | |
| Hollow chest | FFFF01 | FFFF01 | chest | |
| Nkuhana chest | FFFF01 | FFFF01 | chest | |
| Trial chest | FFFF01 | FFFF01 | chest | |
| Mimic | FFFF01 | FFFF01 | chest | |
| Scavenger bag | FFFF01 | FFFF01 | chest | |
| Other chest | FFFF01 | FFFF01 | chest | |
| Rusty lockbox | FF0101 | FF0101 | chest | |
| Encrusted lockbox | FF0101 | FF0101 | chest | |
| Void cradle | FF01FF | FF01FF | chest | |
| Void potential | FF01FF | FF01FF | chest | |
| Lunar pod | 0101FF | 4D79FF | loot bag | |
| Shipping request | 01FFFF | 01FFFF | chest | |

## Shops

| Category | Highlight | Map | Icon | Your choice |
|---|---|---|---|---|
| Multishop | 01FFFF | 01FFFF | chest | |
| Equipment multishop | 01FFFF | 01FFFF | chest | |
| Lunar shop | 01FFFF | 01FFFF | chest | |
| Drone shop | 01FFFF | 01FFFF | chest | |
| Temporary item shop | 01FFFF | 01FFFF | chest | |
| Haunted vending machine | 01FFFF | 01FFFF | chest | |
| Other shop | 01FFFF | 01FFFF | chest | |

## Printers and scrappers

| Category | Highlight | Map | Icon | Your choice |
|---|---|---|---|---|
| Common printer | FF017F | FF017F | chest | |
| Uncommon printer | FF017F | FF017F | chest | |
| Legendary printer | FF017F | FF017F | chest | |
| Boss printer | FF017F | FF017F | chest | |
| Other printer | FF017F | FF017F | chest | |
| Scrapper | FF0101 | FF0101 | wrench | |
| Drone scrapper | FF0101 | FF0101 | wrench | |

## Drones and barrels

| Category | Highlight | Map | Icon | Your choice |
|---|---|---|---|---|
| Drone | 01FF01 | 01FF01 | drone | |
| Drone station | 01FF01 | 01FF01 | drone | |
| Lemurian egg | 01FF01 | 01FF01 | drone | |
| Turret | 01FF01 (highlight off) | 01FF01 | drone | |
| Barrel | FFFF01 | FFFF01 | circle | |
| Void stalk | FFFF01 | FFFF01 | circle | |
| Equipment barrel | FF8001 | FF8001 | chest | |

## Shrines

| Category | Highlight | Map | Icon | Your choice |
|---|---|---|---|---|
| Shrine of Blood | FF3355 | FF3355 | shrine | |
| Shrine of Chance | 33FFAA | 33FFAA | shrine | |
| Shrine of Combat | FF8833 | FF8833 | shrine | |
| Shrine of the Mountain | 3399FF | 3399FF | shrine | |
| Shrine of Order | CCFF33 | CCFF33 | shrine | |
| Shrine of the Woods | 33CC66 | 33CC66 | shrine | |
| Shrine of Shaping | B37DFF | B37DFF | shrine | |
| Shrine of Rebirth | FF66CC | FF66CC | shrine | |
| Shrine of Catastrophe | 9933FF | 9933FF | shrine | |
| Shrine of Compulsion | FF33FF | FF33FF | shrine | |
| Shrine of Death | B3B3B3 | B3B3B3 | shrine | |
| Shrine of Dread | 6666FF | 6666FF | shrine | |
| Cleansing pool | 66E0FF | 66E0FF | shrine | |
| Halcyonite shrine | D9B36C | D9B36C | shrine | |
| Hallowed shrine | E6FFCC | E6FFCC | shrine | |
| Ancient Sepulcher | CC99FF | CC99FF | shrine | |
| Altar of Gold | FFD24D | FFD24D | portal | |
| Other shrine | A04DFF | A04DFF | shrine | |

## World

| Category | Highlight | Map | Icon | Your choice |
|---|---|---|---|---|
| Teleporter | – | FFFFFF (00FF00 when charged) | teleporter | |
| Portal | – | FFFFFF | portal | |
| Newt altar | 0101FF | 4D79FF | shrine | |
| Pressure plate | 808080 | 808080 | ? | |
| Gold totem | – | 5EB2F2 | cube | |
| Radar scanner | – | FFFFFF | ? | |
| Special | – | FFFFFF | ? | |

## Map only

| Category | Map | Icon | Your choice |
|---|---|---|---|
| Player (other players) | FFFFFF | sprint | |
| Enemy | DC143C | circle | |
| Lunar enemy | FF8C8C | circle | |
| Void enemy | FF00FF | sword | |
| Minion | 00FF00 | circle | |
| Item (dropped) | 5EB2F2 (00FFFF when used) | circle | |
| Neutral | FFFFFF (map off) | circle | |

## Available icons

All are game sprites, set with a category's `Icon` setting in the config file.

| Name | Path |
|---|---|
| chest | `Textures/MiscIcons/texInventoryIconOutlined` |
| loot bag | `Textures/MiscIcons/texLootIconOutlined` |
| circle | `Textures/MiscIcons/texBarrelIcon` |
| shrine | `Textures/MiscIcons/texShrineIconOutlined` |
| teleporter | `Textures/MiscIcons/texTeleporterIconOutlined` |
| drone | `Textures/MiscIcons/texDroneIconOutlined` |
| wrench | `Textures/MiscIcons/texWIPIcon` |
| lock | `Textures/MiscIcons/texUnlockIcon` |
| dice | `Textures/MiscIcons/texRuleMapIsRandom` |
| cube | `Textures/MiscIcons/texLunarPillarIcon` |
| cross | `Textures/MiscIcons/texCriticallyHurtIcon` |
| sword | `Textures/MiscIcons/texAttackIcon` |
| sprint | `Textures/MiscIcons/texSprintIcon` |
| arrow | `Textures/MiscIcons/texOptionsArrowLeft` |
| pencil | `Textures/MiscIcons/texEditIcon` |
| portal | `Textures/MiscIcons/texQuickplay` |
| ? | `Textures/MiscIcons/texMysteryIcon` |
| lunar | `Textures/ItemIcons/BG/texLunarBGIcon` |
