using Faust.QoLChests.Configs;
using UnityEngine;

namespace Faust.QoLChests;

public static class Constants
{
    public static HighlightableResource[] ChestResourcesPaths =
    [
        new("RoR2/Base/CasinoChest/CasinoChest.prefab", InteractableCategory.Chest),
        new("RoR2/Base/CategoryChest/CategoryChestDamage.prefab", InteractableCategory.Chest),
        new("RoR2/Base/CategoryChest/CategoryChestHealing.prefab", InteractableCategory.Chest),
        new("RoR2/Base/CategoryChest/CategoryChestUtility.prefab", InteractableCategory.Chest),
        new("RoR2/Base/Chest1/Chest1.prefab", InteractableCategory.Chest),
        new("RoR2/Base/Chest2/Chest2.prefab", InteractableCategory.Chest),
        new("RoR2/Base/GoldChest/GoldChest.prefab", InteractableCategory.Chest),
        new("RoR2/Base/LunarChest/LunarChest.prefab", InteractableCategory.Chest),
        new(
            "RoR2/DLC1/CategoryChest2/CategoryChest2Damage Variant.prefab",
            InteractableCategory.Chest
        ),
        new(
            "RoR2/DLC1/CategoryChest2/CategoryChest2Healing Variant.prefab",
            InteractableCategory.Chest
        ),
        new(
            "RoR2/DLC1/CategoryChest2/CategoryChest2Utility Variant.prefab",
            InteractableCategory.Chest
        ),
        new("RoR2/DLC1/VoidChest/VoidChest.prefab", InteractableCategory.Chest),
    ];

    public static HighlightableResource[] BarrelResourcesPaths =
    [
        new("RoR2/Base/Barrel1/Barrel1.prefab", InteractableCategory.Barrel),
        new("RoR2/Base/EquipmentBarrel/EquipmentBarrel.prefab", InteractableCategory.Barrel),
    ];

    public static HighlightableResource[] LockboxResourcesPaths =
    [
        new("RoR2/Base/TreasureCache/Lockbox.prefab", InteractableCategory.Lockbox),
        new("RoR2/DLC1/TreasureCacheVoid/LockboxVoid.prefab", InteractableCategory.Lockbox),
    ];

    public static HighlightableResource[] StealthedChestResourcePaths =
    [
        new(
            "RoR2/Base/Chest1StealthedVariant/Chest1StealthedVariant.prefab",
            InteractableCategory.StealthedChest
        ),
    ];

    public static HighlightableResource[] ShopResourcePaths =
    [
        new("RoR2/Base/LunarShopTerminal/LunarShopTerminal.prefab", InteractableCategory.Shop),
        new(
            "RoR2/Base/MultiShopEquipmentTerminal/MultiShopEquipmentTerminal.prefab",
            InteractableCategory.Shop
        ),
        new(
            "RoR2/Base/MultiShopLargeTerminal/MultiShopLargeTerminal.prefab",
            InteractableCategory.Shop
        ),
        new("RoR2/Base/MultiShopTerminal/MultiShopTerminal.prefab", InteractableCategory.Shop),
        new("RoR2/Base/MultiShopTerminal/ShopTerminal.prefab", InteractableCategory.Shop),
        new(
            "RoR2/DLC1/FreeChestTerminalShippingDrone/FreeChestTerminalShippingDrone.prefab",
            InteractableCategory.Shop
        ),
        new("RoR2/DLC3/TripleDroneShopTerminal/TripleDroneShopTerminal.prefab", InteractableCategory.Shop),
        new("RoR2/DLC3/TemporaryItemsDistributor/TemporaryItemsShopTerminal.prefab", InteractableCategory.Shop),
    ];

    public static HighlightableResource[] ScrapperResourcePaths =
    [
        new("RoR2/Base/Scrapper/Scrapper.prefab", InteractableCategory.Scrapper),
        new("RoR2/DLC3/DroneScrapper/DroneScrapper.prefab", InteractableCategory.Scrapper),
    ];

    public static HighlightableResource[] DuplicatorResourcesPaths =
    [
        new("RoR2/Base/Duplicator/Duplicator.prefab", InteractableCategory.Duplicator),
        new("RoR2/Base/DuplicatorLarge/DuplicatorLarge.prefab", InteractableCategory.Duplicator),
        new(
            "RoR2/Base/DuplicatorMilitary/DuplicatorMilitary.prefab",
            InteractableCategory.Duplicator
        ),
        new("RoR2/Base/DuplicatorWild/DuplicatorWild.prefab", InteractableCategory.Duplicator),
    ];

    public static HighlightableResource[] DroneResourcesPaths =
    [
        new("RoR2/Base/Drones/Drone1Broken.prefab", InteractableCategory.Drone),
        new("RoR2/Base/Drones/Drone2Broken.prefab", InteractableCategory.Drone),
        new("RoR2/Base/Drones/EmergencyDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/Base/Drones/EquipmentDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/Base/Drones/FlameDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/Base/Drones/MegaDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/Base/Drones/MissileDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/Drones/BombardmentDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/Drones/CleanupDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/Drones/CopycatDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/Drones/HaulerDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/Drones/JailerDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/Drones/JunkDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/Drones/RechargeDroneBroken.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/DroneCombinerStation/DroneCombinerStation.prefab", InteractableCategory.Drone),
        new("RoR2/DLC3/DroneAssemblyStation/DroneAssemblyStation.prefab", InteractableCategory.Drone),
    ];

    public static HighlightableResource[] TurrentResourcePaths =
    [
        new("RoR2/Base/Drones/Turret1Broken.prefab", InteractableCategory.Turret),
    ];

    public static HighlightableResource[] ArtifactOfDevotionResourcePaths =
    [
        new("RoR2/CU8/LemurianEgg/LemurianEgg.prefab", InteractableCategory.Drone)
    ];

    public static HighlightableResource[] ShrineResourcePaths =
    [
        new("RoR2/Base/ShrineBlood/ShrineBlood.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineBlood/ShrineBloodSandy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineBlood/ShrineBloodSnowy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineBoss/ShrineBoss.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineBoss/ShrineBossSandy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineBoss/ShrineBossSnowy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineChance/ShrineChance.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineChance/ShrineChanceSandy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineChance/ShrineChanceSnowy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineCleanse/ShrineCleanse.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineCleanse/ShrineCleanseSandy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineCleanse/ShrineCleanseSnowy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineCombat/ShrineCombat.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineCombat/ShrineCombatSandy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineCombat/ShrineCombatSnowy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineGoldshoresAccess/ShrineGoldshoresAccess.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineHealing/ShrineHealing.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineRestack/ShrineRestack.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineRestack/ShrineRestackSandy Variant.prefab", InteractableCategory.Shrine),
        new("RoR2/Base/ShrineRestack/ShrineRestackSnowy Variant.prefab", InteractableCategory.Shrine),

        new("RoR2/DLC2/ShrineHalcyonite.prefab", InteractableCategory.Shrine),
        new("RoR2/DLC2/ShrineColossusAccess.prefab", InteractableCategory.Shrine),
        new("RoR2/DLC2/ShrineRebirth.prefab", InteractableCategory.Shrine),

        new("RoR2/DLC3/ShrineCombatCollective.prefab", InteractableCategory.Shrine),
    ];

    public static Color32 GetColor(ConfigHighlightColor color) =>
        color switch
        {
            ConfigHighlightColor.Yellow => Yellow,
            ConfigHighlightColor.Red => Red,
            ConfigHighlightColor.Blue => Blue,
            ConfigHighlightColor.Pink => Pink,
            ConfigHighlightColor.Orange => Orange,
            ConfigHighlightColor.Green => Green,
            ConfigHighlightColor.Cyan => Cyan,
            ConfigHighlightColor.Violet => Violet,
            ConfigHighlightColor.Magenta => Magenta,
            ConfigHighlightColor.Gray => Gray,
            ConfigHighlightColor.White => White,
            _ => Yellow
        };

    private static Color32 Yellow => new(255, 255, 1, 255);
    private static Color32 Red => new(255, 1, 1, 255);
    private static Color32 Blue => new(1, 1, 255, 255);
    private static Color32 Pink => new(255, 1, 127, 255);
    private static Color32 Orange => new(255, 128, 1, 255);
    private static Color32 Green => new(1, 255, 1, 255);
    private static Color32 Cyan => new(1, 255, 255, 255);
    private static Color32 Violet => new(127, 1, 255, 255);
    private static Color32 Magenta => new(255, 1, 255, 255);
    private static Color32 Gray => new(128, 128, 128, 255);
    private static Color32 White => new(255, 255, 255, 255);
}


// Add lockbox
// free chest
// void triple?
// lockbox void
// void supressor
// void coin barrel
