using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Faust.QoLChests.Configs;
using MiniMapLibrary;

namespace MapChests.Categories;

/// <summary>
/// The single list of categories. Add a category here and in <see cref="MapCategory"/>.
/// </summary>
public static class CategoryCatalog
{
    private static readonly Dictionary<MapCategory, CategoryDefinition> Definitions = [];
    private static readonly Dictionary<string, MapCategory> PrefabNames = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Regex InstanceSuffix = new(@"(\(Clone\)|\s*\(\d+\))+$", RegexOptions.Compiled);

    public static IEnumerable<CategoryDefinition> All => Definitions.Values;

    public static CategoryDefinition Get(MapCategory category) => Definitions[category];

    /// <summary>
    /// Looks up a category by the name of a spawned or preplaced object, e.g. "Chest1(Clone)" or "Chest1 (2)".
    /// </summary>
    public static bool TryGetByObjectName(string objectName, out MapCategory category) =>
        PrefabNames.TryGetValue(InstanceSuffix.Replace(objectName, "").Trim(), out category);

    static CategoryCatalog()
    {
        HighlightGroup? NoHighlight = null;
        InteractableKind? NoIcon = null;

        // Chests
        Add(MapCategory.SmallChest, "Small chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/Chest1/Chest1.prefab",
            "RoR2/DLC4/Chest1SadObjective.prefab");
        Add(MapCategory.LargeChest, "Large chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/Chest2/Chest2.prefab");
        Add(MapCategory.LegendaryChest, "Legendary chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/GoldChest/GoldChest.prefab",
            "RoR2/DLC4/GoldChestSad.prefab");
        Add(MapCategory.DamageChest, "Damage chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/CategoryChest/CategoryChestDamage.prefab",
            "RoR2/DLC1/CategoryChest2/CategoryChest2Damage Variant.prefab");
        Add(MapCategory.HealingChest, "Healing chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/CategoryChest/CategoryChestHealing.prefab",
            "RoR2/DLC1/CategoryChest2/CategoryChest2Healing Variant.prefab");
        Add(MapCategory.UtilityChest, "Utility chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/CategoryChest/CategoryChestUtility.prefab",
            "RoR2/DLC1/CategoryChest2/CategoryChest2Utility Variant.prefab");
        Add(MapCategory.AdaptiveChest, "Adaptive chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/CasinoChest/CasinoChest.prefab");
        Add(MapCategory.CloakedChest, "Cloaked chest", HighlightGroup.StealthedChest, NoIcon,
            "RoR2/Base/Chest1StealthedVariant/Chest1StealthedVariant.prefab");
        Add(MapCategory.SecretChest, "Secret chest", HighlightGroup.Chest, InteractableKind.Chest);
        Add(MapCategory.RustyLockbox, "Rusty lockbox", HighlightGroup.Lockbox, InteractableKind.Chest,
            "RoR2/Base/TreasureCache/Lockbox.prefab",
            "RoR2/DLC4/LockboxSoulObjective.prefab",
            "RoR2/DLC4/LockboxTrial.prefab");
        Add(MapCategory.EncrustedLockbox, "Encrusted lockbox", HighlightGroup.Lockbox, InteractableKind.Chest,
            "RoR2/DLC1/TreasureCacheVoid/LockboxVoid.prefab");
        Add(MapCategory.VoidCradle, "Void cradle", HighlightGroup.VoidCradle, InteractableKind.Chest,
            "RoR2/DLC1/VoidChest/VoidChest.prefab");
        Add(MapCategory.VoidPotential, "Void potential", HighlightGroup.VoidCradle, InteractableKind.Chest,
            "RoR2/DLC1/VoidTriple/VoidTriple.prefab");
        Add(MapCategory.LunarPod, "Lunar pod", HighlightGroup.LunarPod, InteractableKind.LunarPod,
            "RoR2/Base/LunarChest/LunarChest.prefab");
        Add(MapCategory.ShippingRequest, "Shipping request", HighlightGroup.Shop, InteractableKind.Shop,
            "RoR2/DLC1/FreeChestTerminalShippingDrone/FreeChestTerminalShippingDrone.prefab");
        Add(MapCategory.ScavengerBag, "Scavenger bag", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/Base/Scav/ScavBackpack.prefab",
            "RoR2/Base/ScavLunar/ScavLunarBackpack.prefab");
        Add(MapCategory.HollowChest, "Hollow chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/DLC4/HollowChest/HollowChestFree.prefab");
        Add(MapCategory.NkuhanaChest, "Nkuhana chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/DLC4/NkuhanaChest.prefab");
        Add(MapCategory.TrialChest, "Trial chest", HighlightGroup.Chest, InteractableKind.Chest,
            "RoR2/DLC4/TrialCategoryChestDamage.prefab",
            "RoR2/DLC4/TrialCategoryChest2Damage.prefab");
        Add(MapCategory.OtherChest, "Other chest", HighlightGroup.Chest, InteractableKind.Chest);

        // Shops
        Add(MapCategory.Multishop, "Multishop", HighlightGroup.Shop, InteractableKind.Shop,
            "RoR2/Base/MultiShopTerminal/MultiShopTerminal.prefab",
            "RoR2/Base/MultiShopTerminal/ShopTerminal.prefab",
            "RoR2/Base/MultiShopLargeTerminal/MultiShopLargeTerminal.prefab",
            "RoR2/DLC4/TrialMultiShopTerminal.prefab",
            "RoR2/DLC4/TrialTripleShop.prefab",
            "RoR2/DLC4/TrialTripleShopLarge.prefab");
        Add(MapCategory.EquipmentMultishop, "Equipment multishop", HighlightGroup.Shop, InteractableKind.Shop,
            "RoR2/Base/MultiShopEquipmentTerminal/MultiShopEquipmentTerminal.prefab");
        Add(MapCategory.LunarShop, "Lunar shop", HighlightGroup.Shop, InteractableKind.Shop,
            "RoR2/Base/LunarShopTerminal/LunarShopTerminal.prefab");
        Add(MapCategory.DroneShop, "Drone shop", HighlightGroup.Shop, InteractableKind.Shop,
            "RoR2/DLC3/TripleDroneShopTerminal/TripleDroneShopTerminal.prefab");
        Add(MapCategory.TemporaryItemShop, "Temporary item shop", HighlightGroup.Shop, InteractableKind.Shop,
            "RoR2/DLC3/TemporaryItemsDistributor/TemporaryItemsShopTerminal.prefab");
        Add(MapCategory.VendingMachine, "Haunted vending machine", HighlightGroup.Shop, InteractableKind.Shop,
            "RoR2/DLC4/ShrineCompulsion/HauntedVendingMachine.prefab");
        Add(MapCategory.OtherShop, "Other shop", HighlightGroup.Shop, InteractableKind.Shop);

        // Printers and scrappers
        Add(MapCategory.CommonPrinter, "Common printer", HighlightGroup.Duplicator, InteractableKind.Printer,
            "RoR2/Base/Duplicator/Duplicator.prefab");
        Add(MapCategory.UncommonPrinter, "Uncommon printer", HighlightGroup.Duplicator, InteractableKind.Printer,
            "RoR2/Base/DuplicatorLarge/DuplicatorLarge.prefab");
        Add(MapCategory.LegendaryPrinter, "Legendary printer", HighlightGroup.Duplicator, InteractableKind.Printer,
            "RoR2/Base/DuplicatorMilitary/DuplicatorMilitary.prefab");
        Add(MapCategory.BossPrinter, "Boss printer", HighlightGroup.Duplicator, InteractableKind.Printer,
            "RoR2/Base/DuplicatorWild/DuplicatorWild.prefab");
        Add(MapCategory.OtherPrinter, "Other printer", HighlightGroup.Duplicator, InteractableKind.Printer);
        Add(MapCategory.Scrapper, "Scrapper", HighlightGroup.Scrapper, InteractableKind.Utility,
            "RoR2/Base/Scrapper/Scrapper.prefab");
        Add(MapCategory.DroneScrapper, "Drone scrapper", HighlightGroup.Scrapper, InteractableKind.Utility,
            "RoR2/DLC3/DroneScrapper/DroneScrapper.prefab");

        // Drones
        Add(MapCategory.Drone, "Drone", HighlightGroup.Drone, InteractableKind.Drone,
            "RoR2/Base/Drones/Drone1Broken.prefab",
            "RoR2/Base/Drones/Drone2Broken.prefab",
            "RoR2/Base/Drones/EmergencyDroneBroken.prefab",
            "RoR2/Base/Drones/EquipmentDroneBroken.prefab",
            "RoR2/Base/Drones/FlameDroneBroken.prefab",
            "RoR2/Base/Drones/MegaDroneBroken.prefab",
            "RoR2/Base/Drones/MissileDroneBroken.prefab",
            "RoR2/DLC3/Drones/BombardmentDroneBroken.prefab",
            "RoR2/DLC3/Drones/CleanupDroneBroken.prefab",
            "RoR2/DLC3/Drones/CopycatDroneBroken.prefab",
            "RoR2/DLC3/Drones/HaulerDroneBroken.prefab",
            "RoR2/DLC3/Drones/JailerDroneBroken.prefab",
            "RoR2/DLC3/Drones/JunkDroneBroken.prefab",
            "RoR2/DLC3/Drones/RechargeDroneBroken.prefab");
        Add(MapCategory.Turret, "Turret", HighlightGroup.Turret, InteractableKind.Drone,
            "RoR2/Base/Drones/Turret1Broken.prefab");
        Add(MapCategory.DroneStation, "Drone station", HighlightGroup.Drone, InteractableKind.Drone,
            "RoR2/DLC3/DroneCombinerStation/DroneCombinerStation.prefab",
            "RoR2/DLC3/DroneAssemblyStation/DroneAssemblyStation.prefab");
        Add(MapCategory.LemurianEgg, "Lemurian egg", HighlightGroup.Drone, InteractableKind.Drone,
            "RoR2/CU8/LemurianEgg/LemurianEgg.prefab");

        // Barrels
        Add(MapCategory.Barrel, "Barrel", HighlightGroup.Barrel, InteractableKind.Barrel,
            "RoR2/Base/Barrel1/Barrel1.prefab");
        Add(MapCategory.EquipmentBarrel, "Equipment barrel", HighlightGroup.EquipmentBarrel, InteractableKind.Equipment,
            "RoR2/Base/EquipmentBarrel/EquipmentBarrel.prefab");
        Add(MapCategory.VoidStalk, "Void stalk", HighlightGroup.Barrel, InteractableKind.Barrel,
            "RoR2/DLC1/VoidCoinBarrel/VoidCoinBarrel.prefab");

        // Shrines
        Add(MapCategory.BloodShrine, "Shrine of Blood", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/Base/ShrineBlood/ShrineBlood.prefab",
            "RoR2/Base/ShrineBlood/ShrineBloodSandy Variant.prefab",
            "RoR2/Base/ShrineBlood/ShrineBloodSnowy Variant.prefab");
        Add(MapCategory.ChanceShrine, "Shrine of Chance", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/Base/ShrineChance/ShrineChance.prefab",
            "RoR2/Base/ShrineChance/ShrineChanceSandy Variant.prefab",
            "RoR2/Base/ShrineChance/ShrineChanceSnowy Variant.prefab");
        Add(MapCategory.CombatShrine, "Shrine of Combat", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/Base/ShrineCombat/ShrineCombat.prefab",
            "RoR2/Base/ShrineCombat/ShrineCombatSandy Variant.prefab",
            "RoR2/Base/ShrineCombat/ShrineCombatSnowy Variant.prefab",
            "RoR2/DLC3/ShrineCombatCollective.prefab",
            "RoR2/DLC4/ShrineCombatPoison.prefab");
        Add(MapCategory.MountainShrine, "Shrine of the Mountain", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/Base/ShrineBoss/ShrineBoss.prefab",
            "RoR2/Base/ShrineBoss/ShrineBossSandy Variant.prefab",
            "RoR2/Base/ShrineBoss/ShrineBossSnowy Variant.prefab");
        Add(MapCategory.OrderShrine, "Shrine of Order", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/Base/ShrineRestack/ShrineRestack.prefab",
            "RoR2/Base/ShrineRestack/ShrineRestackSandy Variant.prefab",
            "RoR2/Base/ShrineRestack/ShrineRestackSnowy Variant.prefab",
            "RoR2/DLC4/ShrineCompulsion/ShrineRestackQuest Variant.prefab");
        Add(MapCategory.WoodsShrine, "Shrine of the Woods", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/Base/ShrineHealing/ShrineHealing.prefab");
        Add(MapCategory.CleansingPool, "Cleansing pool", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/Base/ShrineCleanse/ShrineCleanse.prefab",
            "RoR2/Base/ShrineCleanse/ShrineCleanseSandy Variant.prefab",
            "RoR2/Base/ShrineCleanse/ShrineCleanseSnowy Variant.prefab");
        Add(MapCategory.GoldAltar, "Altar of Gold", HighlightGroup.Shrine, InteractableKind.Portal,
            "RoR2/Base/ShrineGoldshoresAccess/ShrineGoldshoresAccess.prefab");
        Add(MapCategory.HalcyoniteShrine, "Halcyonite shrine", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC2/ShrineHalcyonite.prefab",
            "RoR2/DLC2/ShrineHalcyonite_Stage1 Variant.prefab");
        Add(MapCategory.ColossusShrine, "Shrine of Shaping", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC2/ShrineColossusAccess.prefab");
        Add(MapCategory.RebirthShrine, "Shrine of Rebirth", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC2/ShrineRebirth.prefab");
        Add(MapCategory.CatastropheShrine, "Shrine of Catastrophe", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC4/ShrineCatastrophe/ShrineCatastrophe.prefab");
        Add(MapCategory.CompulsionShrine, "Shrine of Compulsion", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC4/ShrineCompulsion/ShrineCompulsion.prefab");
        Add(MapCategory.DeathShrine, "Shrine of Death", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC4/ShrineDeath/ShrineDeath.prefab");
        Add(MapCategory.DreadShrine, "Shrine of Dread", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC4/ShrineDread/ShrineDread.prefab");
        Add(MapCategory.HallowedShrine, "Hallowed shrine", HighlightGroup.Shrine, InteractableKind.Shrine,
            "RoR2/DLC4/HallowingShrineBase.prefab",
            "RoR2/DLC4/BetterHallowingShrineBase.prefab");
        Add(MapCategory.OtherShrine, "Other shrine", HighlightGroup.Shrine, InteractableKind.Shrine);

        // World
        Add(MapCategory.NewtAltar, "Newt altar", HighlightGroup.NewtStatue, InteractableKind.NewtAltar);
        Add(MapCategory.PressurePlate, "Pressure plate", HighlightGroup.PressurePlate, NoIcon);
        Add(MapCategory.Teleporter, "Teleporter", NoHighlight, InteractableKind.Teleporter,
            "RoR2/Base/Teleporters/Teleporter1.prefab",
            "RoR2/Base/Teleporters/LunarTeleporter Variant.prefab",
            "RoR2/Base/Teleporters/LunarTeleporter Variant, SkipCharge.prefab",
            "RoR2/DLC3/conduitcanyon/Teleporter_ConduitCanyon_Variant.prefab");
        Add(MapCategory.Portal, "Portal", NoHighlight, InteractableKind.Portal,
            "RoR2/Base/PortalArena/PortalArena.prefab",
            "RoR2/Base/PortalArtifactworld/PortalArtifactworld.prefab",
            "RoR2/Base/PortalGoldshores/PortalGoldshores.prefab",
            "RoR2/Base/PortalMS/PortalMS.prefab",
            "RoR2/Base/PortalShop/PortalShop.prefab",
            "RoR2/DLC1/PortalVoid/PortalVoid.prefab",
            "RoR2/DLC1/DeepVoidPortal/DeepVoidPortal.prefab",
            "RoR2/DLC1/GameModes/InfiniteTowerRun/PortalInfiniteTower.prefab",
            "RoR2/DLC2/PortalColossus.prefab",
            "RoR2/DLC3/EyePortal/EyePortal.prefab",
            "RoR2/DLC3/HardwareProgPortal.prefab",
            "RoR2/DLC3/HardwareProgPortal_Haunt.prefab",
            "RoR2/DLC4/ChampRushPortal.prefab",
            "RoR2/DLC4/ChampRushExitPortal.prefab",
            "RoR2/DLC4/TrialPortalEntry.prefab",
            "RoR2/DLC4/TrialPortalEntryDanger.prefab",
            "RoR2/DLC4/TrialPortalExit.prefab",
            "RoR2/DLC4/champrush/PortalToUltima.prefab");
        Add(MapCategory.GoldTotem, "Gold totem", NoHighlight, InteractableKind.Totem,
            "RoR2/Base/goldshores/GoldshoresBeacon.prefab");
        Add(MapCategory.RadarScanner, "Radar scanner", NoHighlight, InteractableKind.Special,
            "RoR2/Base/RadarTower/RadarTower.prefab");
        Add(MapCategory.Special, "Special", NoHighlight, InteractableKind.Special);

        // Minimap only
        Add(MapCategory.Enemy, "Enemy", NoHighlight, InteractableKind.EnemyMonster);
        Add(MapCategory.LunarEnemy, "Lunar enemy", NoHighlight, InteractableKind.EnemyLunar);
        Add(MapCategory.VoidEnemy, "Void enemy", NoHighlight, InteractableKind.EnemyVoid);
        Add(MapCategory.Minion, "Minion", NoHighlight, InteractableKind.Minion);
        Add(MapCategory.Player, "Player", NoHighlight, InteractableKind.Player);
        Add(MapCategory.Item, "Item", NoHighlight, InteractableKind.Item);
        Add(MapCategory.Neutral, "Neutral", NoHighlight, InteractableKind.Neutral);

        foreach (MapCategory category in Enum.GetValues(typeof(MapCategory)))
        {
            if (!Definitions.ContainsKey(category))
                throw new InvalidOperationException($"{nameof(MapCategory)}.{category} has no definition in {nameof(CategoryCatalog)}");
        }
    }

    private static void Add(
        MapCategory category,
        string displayName,
        HighlightGroup? highlightGroup,
        InteractableKind? minimapKind,
        params string[] prefabPaths
    )
    {
        Definitions.Add(category, new CategoryDefinition(category, displayName, highlightGroup, minimapKind, prefabPaths));

        foreach (var path in prefabPaths)
            PrefabNames.Add(Path.GetFileNameWithoutExtension(path), category);
    }
}
