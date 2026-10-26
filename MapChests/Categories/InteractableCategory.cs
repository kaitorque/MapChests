namespace Faust.QoLChests.Api;

/// <summary>
/// Highlight/hide category of an interactable; each category has its own user config.
/// Values are part of the public API: only append new ones, never renumber.
/// </summary>
public enum InteractableCategory
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
