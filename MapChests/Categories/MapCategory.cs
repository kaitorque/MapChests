namespace MapChests.Categories;

/// <summary>
/// Every kind of object MapChests highlights, hides or shows on the minimap.
/// Each value has one <see cref="CategoryDefinition"/> in <see cref="CategoryCatalog"/>.
/// </summary>
public enum MapCategory
{
    // Chests
    SmallChest,
    LargeChest,
    LegendaryChest,
    DamageChest,
    HealingChest,
    UtilityChest,
    AdaptiveChest,
    CloakedChest,
    SecretChest,
    RustyLockbox,
    EncrustedLockbox,
    VoidCradle,
    VoidPotential,
    LunarPod,
    ShippingRequest,
    ScavengerBag,
    HollowChest,
    NkuhanaChest,
    TrialChest,
    OtherChest,

    // Shops
    Multishop,
    EquipmentMultishop,
    LunarShop,
    DroneShop,
    TemporaryItemShop,
    VendingMachine,
    OtherShop,

    // Printers and scrappers
    CommonPrinter,
    UncommonPrinter,
    LegendaryPrinter,
    BossPrinter,
    OtherPrinter,
    Scrapper,
    DroneScrapper,

    // Drones
    Drone,
    Turret,
    DroneStation,
    LemurianEgg,

    // Barrels
    Barrel,
    EquipmentBarrel,
    VoidStalk,

    // Shrines
    BloodShrine,
    ChanceShrine,
    CombatShrine,
    MountainShrine,
    OrderShrine,
    WoodsShrine,
    CleansingPool,
    GoldAltar,
    HalcyoniteShrine,
    ColossusShrine,
    RebirthShrine,
    CatastropheShrine,
    CompulsionShrine,
    DeathShrine,
    DreadShrine,
    HallowedShrine,
    AncientSepulcher,
    OtherShrine,

    // World
    NewtAltar,
    PressurePlate,
    Teleporter,
    Portal,
    GoldTotem,
    RadarScanner,
    Special,

    // Minimap only
    Enemy,
    LunarEnemy,
    VoidEnemy,
    Minion,
    Player,
    Item,
    Neutral,
}
