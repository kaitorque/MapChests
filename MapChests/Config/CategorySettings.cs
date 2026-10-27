using System.Collections.Generic;
using BepInEx.Configuration;
using MapChests.Categories;
using MiniMapLibrary;
using UnityEngine;

namespace Faust.QoLChests.Configs;

public enum UsedIconMode
{
    Gray,
    Hidden,
}

/// <summary>
/// The config section of one <see cref="MapCategory"/>: highlight, hide and minimap settings.
/// Highlight entries are null for categories that are never highlighted; <see cref="UsedIcon"/> is null for entities.
/// </summary>
public sealed class CategorySettings
{
    public CategoryDefinition Definition { get; }

    public ConfigEntry<bool> Highlight { get; }
    public ConfigEntry<Color> HighlightColor { get; }
    public ConfigEntry<bool> HideWhenUsed { get; }

    public ConfigEntry<bool> Minimap { get; }
    public ConfigEntry<Color> MinimapColor { get; }
    public ConfigEntry<Color> MinimapUsedColor { get; }
    public ConfigEntry<UsedIconMode> UsedIcon { get; }
    public ConfigEntry<string> Icon { get; }
    public ConfigEntry<float> IconWidth { get; }
    public ConfigEntry<float> IconHeight { get; }
    public ConfigEntry<bool> ElevationMarker { get; }
    public ConfigEntry<Color> ElevationMarkerColor { get; }
    public ConfigEntry<float> ElevationMarkerWidth { get; }
    public ConfigEntry<float> ElevationMarkerHeight { get; }
    public ConfigEntry<Vector2> ElevationMarkerOffset { get; }

    public bool HighlightEnabled => Highlight?.Value ?? false;
    public bool HideEnabled => HideWhenUsed?.Value ?? false;
    public bool HideUsedIcon => UsedIcon?.Value == UsedIconMode.Hidden;

    public CategorySettings(ConfigFile config, CategoryDefinition definition, LegacyConfig legacy)
    {
        Definition = definition;
        var section = definition.ConfigSection;
        var name = definition.DisplayName.ToLowerInvariant();

        Color highlightColor = Color.white;
        if (definition.HighlightGroup is { } group)
        {
            var keys = LegacyHighlightKeys[group];
            var defaults = GroupDefaults[group];
            highlightColor = DistinctColors.TryGetValue(definition.Category, out var distinct)
                ? distinct
                : legacy.HighlightPaletteColor("Highlight", keys.Color, defaults.Color);

            Highlight = config.Bind(section, "Highlight", legacy.HighlightBool("Highlight", keys.Enabled, defaults.Highlight), $"Highlight {name}");
            HighlightColor = config.Bind(section, "Highlight color", highlightColor, $"Highlight colour for {name}");
            HideWhenUsed = config.Bind(section, "Hide when used", keys.Hide != null ? legacy.HighlightBool("Hide", keys.Hide, defaults.Hide) : defaults.Hide, $"Hide {name} a few seconds after they are used");
        }

        var kind = definition.MinimapKind;
        var kindSection = $"Icon.{kind}";
        var kindDefaults = Settings.GetSetting(kind);

        var minimapColor = definition.IsHighlightable
            ? highlightColor
            : legacy.MinimapColor(kindSection, "activeColor", MapOnlyColors.TryGetValue(kind, out var mapColor) ? mapColor : kindDefaults.ActiveColor);

        Minimap = config.Bind(section, "Minimap", legacy.MinimapBool(kindSection, "enabled", kind != InteractableKind.Neutral), $"Show {name} on the minimap");
        MinimapColor = config.Bind(section, "Minimap color", minimapColor, $"Minimap icon colour for {name}");
        MinimapUsedColor = config.Bind(section, "Minimap used color", legacy.MinimapColor(kindSection, "inactiveColor", kindDefaults.InactiveColor), $"Minimap icon colour once {name} are used");
        if (!definition.IsEntity)
            UsedIcon = config.Bind(section, "Used icon", UsedIconMode.Gray, $"What the minimap icon does once {name} are used: turn gray (the used colour) or disappear");

        var icon = DistinctIcons.TryGetValue(definition.Category, out var distinctIcon)
            ? distinctIcon
            : legacy.MinimapString(kindSection, "icon", kindDefaults.IconPath);
        Icon = config.Bind(section, "Icon", icon, "Game sprite path of the minimap icon (Textures/MiscIcons/...)");
        var hasDistinctSize = DistinctIconSizes.TryGetValue(definition.Category, out var distinctSize);
        IconWidth = config.Bind(section, "Icon width", hasDistinctSize ? distinctSize : legacy.MinimapFloat(kindSection, "width", kindDefaults.Dimensions.Width), "Width of the minimap icon");
        IconHeight = config.Bind(section, "Icon height", hasDistinctSize ? distinctSize : legacy.MinimapFloat(kindSection, "height", kindDefaults.Dimensions.Height), "Height of the minimap icon");
        ElevationMarker = config.Bind(section, "Elevation marker", legacy.MinimapBool(kindSection, "elevationMarkerEnabled", kindDefaults.ElevationMarkerEnabled), "Show an arrow when the object is above or below you");
        ElevationMarkerColor = config.Bind(section, "Elevation marker color", legacy.MinimapColor(kindSection, "elevationMarkerColor", kindDefaults.ElevationMarkerColor), "Colour of the elevation arrow");
        ElevationMarkerWidth = config.Bind(section, "Elevation marker width", legacy.MinimapFloat(kindSection, "elevationMarkerWidth", kindDefaults.ElevationMarkerDimensions.Width), "Width of the elevation arrow");
        ElevationMarkerHeight = config.Bind(section, "Elevation marker height", legacy.MinimapFloat(kindSection, "elevationMarkerHeight", kindDefaults.ElevationMarkerDimensions.Height), "Height of the elevation arrow");
        ElevationMarkerOffset = config.Bind(section, "Elevation marker offset", legacy.MinimapVector2(kindSection, "elevationMarkerOffset", new Vector2(kindDefaults.ElevationMarkerOffset.Width, kindDefaults.ElevationMarkerOffset.Height)), "Position of the elevation arrow relative to the icon centre");
    }

    private readonly struct LegacyKeys(string enabled, string color, string hide)
    {
        public string Enabled { get; } = enabled;
        public string Color { get; } = color;
        public string Hide { get; } = hide;
    }

    private readonly struct GroupDefault(bool highlight, Color color, bool hide)
    {
        public bool Highlight { get; } = highlight;
        public Color Color { get; } = color;
        public bool Hide { get; } = hide;
    }

    private static Color Hex(string hex) => ColorUtility.TryParseHtmlString("#" + hex, out var color) ? color : Color.white;

    private static readonly Dictionary<HighlightGroup, LegacyKeys> LegacyHighlightKeys = new()
    {
        [HighlightGroup.Chest] = new("Chest", "ChestColor", "Chest"),
        [HighlightGroup.Shop] = new("Shops", "ShopColor", "Shops"),
        [HighlightGroup.Scrapper] = new("Scrapper", "ScrapperColor", null),
        [HighlightGroup.Duplicator] = new("Duplicator", "DuplicatorColor", null),
        [HighlightGroup.Drone] = new("Drones", "DronesColor", null),
        [HighlightGroup.Turret] = new("Turrets", "TurretsColor", null),
        [HighlightGroup.StealthedChest] = new("Stealthed Chests", "StealthedChestsColor", "Stealthed Chests"),
        [HighlightGroup.Lockbox] = new("Lockboxes", "LockboxesColor", null),
        [HighlightGroup.Barrel] = new("Barrels", "BarrelColor", "Barrels"),
        [HighlightGroup.EquipmentBarrel] = new("Equipment Barrel", "EquipmentBarrelColor", "Equipment Barrel"),
        [HighlightGroup.LunarPod] = new("Lunar Pod", "LunarPodColor", "Lunar Pod"),
        [HighlightGroup.VoidCradle] = new("Void Cradle", "VoidCradleColor", "Void Cradle"),
        [HighlightGroup.NewtStatue] = new("Newt Statues", "NewtStatueColor", null),
        [HighlightGroup.PressurePlate] = new("Pressure Plates", "PressurePlateColor", null),
        [HighlightGroup.Shrine] = new("Shrines", "ShrineColor", null),
    };

    // Owner's current settings (DESIGN.md section 3)
    private static readonly Dictionary<HighlightGroup, GroupDefault> GroupDefaults = new()
    {
        [HighlightGroup.Chest] = new(true, Hex("FFFF01"), true),
        [HighlightGroup.Shop] = new(true, Hex("01FFFF"), true),
        [HighlightGroup.Scrapper] = new(true, Hex("FF0101"), false),
        [HighlightGroup.Duplicator] = new(true, Hex("FF017F"), false),
        [HighlightGroup.Drone] = new(true, Hex("01FF01"), false),
        [HighlightGroup.Turret] = new(true, Hex("33FFAA"), false),
        [HighlightGroup.StealthedChest] = new(true, Hex("FFFF99"), true),
        [HighlightGroup.Lockbox] = new(true, Hex("FF0101"), false),
        [HighlightGroup.Barrel] = new(true, Hex("FFFF01"), true),
        [HighlightGroup.EquipmentBarrel] = new(true, Hex("FF8001"), true),
        [HighlightGroup.LunarPod] = new(true, Hex("3366FF"), true),
        [HighlightGroup.VoidCradle] = new(true, Hex("FF01FF"), true),
        [HighlightGroup.NewtStatue] = new(true, Hex("3366FF"), false),
        [HighlightGroup.PressurePlate] = new(true, Hex("FFFFFF"), false),
        [HighlightGroup.Shrine] = new(true, Hex("7F01FF"), false),
    };

    /// <summary>Owner's colour choices that differ from the group colour; every other category starts with its group colour.</summary>
    private static readonly Dictionary<MapCategory, Color> DistinctColors = new()
    {
        [MapCategory.RustyLockbox] = Hex("FFFF01"),
        [MapCategory.EncrustedLockbox] = Hex("FF01FF"),
        [MapCategory.EquipmentMultishop] = Hex("0099CC"),
        [MapCategory.LegendaryChest] = Hex("FFD24D"),
        [MapCategory.LunarShop] = Hex("3366FF"),
        [MapCategory.TemporaryItemShop] = Hex("99FFFF"),
        [MapCategory.VendingMachine] = Hex("1AB3A6"),
        [MapCategory.HollowChest] = Hex("CCFF66"),
        [MapCategory.DroneScrapper] = Hex("01FF01"),
        [MapCategory.DroneStation] = Hex("33B333"),
        // Gold and coins only: dim, so chests and void cradles stand out
        [MapCategory.Barrel] = Hex("A68C40"),
        [MapCategory.VoidStalk] = Hex("A64DA6"),
        // Shrines: muted versions of their sigil colours, so item sources draw the eye first;
        // shrines that give items stay a little brighter
        [MapCategory.ChanceShrine] = Hex("CCD24D"),
        [MapCategory.HalcyoniteShrine] = Hex("D9B36C"),
        [MapCategory.HallowedShrine] = Hex("E6E6B3"),
        [MapCategory.AncientSepulcher] = Hex("CC99FF"),
        [MapCategory.GoldAltar] = Hex("FFD24D"),
        [MapCategory.BloodShrine] = Hex("B34D4D"),
        [MapCategory.CombatShrine] = Hex("8066CC"),
        [MapCategory.MountainShrine] = Hex("4DB8D9"),
        [MapCategory.OrderShrine] = Hex("CC5C99"),
        [MapCategory.WoodsShrine] = Hex("5CB35C"),
        [MapCategory.ColossusShrine] = Hex("B3A366"),
        [MapCategory.CatastropheShrine] = Hex("4D80D9"),
        [MapCategory.CompulsionShrine] = Hex("A6553A"),
        [MapCategory.DeathShrine] = Hex("8C4D73"),
        [MapCategory.DreadShrine] = Hex("7373BF"),
        [MapCategory.RebirthShrine] = Hex("66B3A6"),
        [MapCategory.CleansingPool] = Hex("3366FF"),
        [MapCategory.OtherShrine] = Hex("CC8080"),
    };

    /// <summary>Owner's icon choices that differ from the old minimap icon kind.</summary>
    private static readonly Dictionary<MapCategory, string> DistinctIcons = new()
    {
        [MapCategory.ScavengerBag] = Settings.Icons.LootBag,
        [MapCategory.HollowChest] = Settings.Icons.LootBag,
        [MapCategory.RustyLockbox] = Settings.Icons.Lock,
        [MapCategory.EncrustedLockbox] = Settings.Icons.Lock,
        [MapCategory.DroneShop] = Settings.Icons.Drone,
        [MapCategory.NewtAltar] = Settings.Icons.Shrine,
        [MapCategory.CleansingPool] = Settings.Icons.Wrench,
    };

    /// <summary>Icon sizes for categories whose distinct icon belongs to a kind with another size.</summary>
    private static readonly Dictionary<MapCategory, float> DistinctIconSizes = new()
    {
        [MapCategory.CleansingPool] = 5f,
    };

    // Owner's current minimap colours for categories that are never highlighted
    private static readonly Dictionary<InteractableKind, Color> MapOnlyColors = new()
    {
        [InteractableKind.Teleporter] = Hex("FFFFFF"),
        [InteractableKind.Portal] = Hex("FFFFFF"),
        [InteractableKind.Totem] = Hex("5EB2F2"),
        [InteractableKind.Special] = Hex("FFFFFF"),
        [InteractableKind.EnemyMonster] = Hex("DC143C"),
        [InteractableKind.EnemyLunar] = Hex("FF8C8C"),
        [InteractableKind.EnemyVoid] = Hex("FF00FF"),
        [InteractableKind.Minion] = Hex("00FF00"),
        [InteractableKind.Player] = Hex("FFFFFF"),
        [InteractableKind.Item] = Hex("5EB2F2"),
        [InteractableKind.Neutral] = Hex("FFFFFF"),
    };
}
