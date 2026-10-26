namespace Faust.QoLChests.Configs;

/// <summary>
/// The original QoLChests categories, which are still the keys of the highlight and hide config entries.
/// Each <see cref="MapChests.Categories.MapCategory"/> maps to one of these until the configs are merged.
/// </summary>
public enum HighlightGroup
{
    Chest = 0,
    Shop = 1,
    Scrapper = 2,
    Duplicator = 3,
    Drone = 4,
    Turret = 5,
    StealthedChest = 6,
    Shrine = 7,
    Lockbox = 8,
    Barrel = 9,
    NewtStatue = 10,
    PressurePlate = 11,
    EquipmentBarrel = 12,
    LunarPod = 13,
    VoidCradle = 14
}
