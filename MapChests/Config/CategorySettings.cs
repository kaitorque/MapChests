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
            ? LighterOnMap(highlightColor)
            : legacy.MinimapColor(kindSection, "activeColor", MapOnlyColors.TryGetValue(kind, out var mapColor) ? mapColor : kindDefaults.ActiveColor);

        Minimap = config.Bind(section, "Minimap", legacy.MinimapBool(kindSection, "enabled", kind != InteractableKind.Neutral), $"Show {name} on the minimap");
        MinimapColor = config.Bind(section, "Minimap color", minimapColor, $"Minimap icon colour for {name}");
        MinimapUsedColor = config.Bind(section, "Minimap used color", legacy.MinimapColor(kindSection, "inactiveColor", kindDefaults.InactiveColor), $"Minimap icon colour once {name} are used");
        if (!definition.IsEntity)
            UsedIcon = config.Bind(section, "Used icon", UsedIconMode.Gray, $"What the minimap icon does once {name} are used: turn gray (the used colour) or disappear");

        Icon = config.Bind(section, "Icon", legacy.MinimapString(kindSection, "icon", kindDefaults.IconPath), "Game sprite path of the minimap icon (Textures/MiscIcons/...)");
        IconWidth = config.Bind(section, "Icon width", legacy.MinimapFloat(kindSection, "width", kindDefaults.Dimensions.Width), "Width of the minimap icon");
        IconHeight = config.Bind(section, "Icon height", legacy.MinimapFloat(kindSection, "height", kindDefaults.Dimensions.Height), "Height of the minimap icon");
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

    /// <summary>Pure blue and violet are hard to see on the small dark minimap, so the map uses the owner's lighter shades.</summary>
    private static Color LighterOnMap(Color color)
    {
        Color32 c = color;
        return (c.r, c.g, c.b) switch
        {
            (1, 1, 255) => Hex("4D79FF"),
            (127, 1, 255) => Hex("A04DFF"),
            _ => color,
        };
    }

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
        [HighlightGroup.Turret] = new(false, Hex("01FF01"), false),
        [HighlightGroup.StealthedChest] = new(true, Hex("FFFF01"), true),
        [HighlightGroup.Lockbox] = new(true, Hex("FF0101"), false),
        [HighlightGroup.Barrel] = new(true, Hex("FFFF01"), true),
        [HighlightGroup.EquipmentBarrel] = new(true, Hex("FF8001"), true),
        [HighlightGroup.LunarPod] = new(true, Hex("0101FF"), true),
        [HighlightGroup.VoidCradle] = new(true, Hex("FF01FF"), true),
        [HighlightGroup.NewtStatue] = new(true, Hex("0101FF"), false),
        [HighlightGroup.PressurePlate] = new(true, Hex("808080"), false),
        [HighlightGroup.Shrine] = new(true, Hex("7F01FF"), false),
    };

    /// <summary>Shrines and the equipment multishop are told apart by colour (owner decision); every other category starts with its group colour.</summary>
    private static readonly Dictionary<MapCategory, Color> DistinctColors = new()
    {
        [MapCategory.EquipmentMultishop] = Hex("FF8001"),
        [MapCategory.BloodShrine] = Hex("FF3355"),
        [MapCategory.ChanceShrine] = Hex("33FFAA"),
        [MapCategory.CombatShrine] = Hex("FF8833"),
        [MapCategory.MountainShrine] = Hex("3399FF"),
        [MapCategory.OrderShrine] = Hex("CCFF33"),
        [MapCategory.WoodsShrine] = Hex("33CC66"),
        [MapCategory.CleansingPool] = Hex("66E0FF"),
        [MapCategory.GoldAltar] = Hex("FFD24D"),
        [MapCategory.HalcyoniteShrine] = Hex("D9B36C"),
        [MapCategory.ColossusShrine] = Hex("B37DFF"),
        [MapCategory.RebirthShrine] = Hex("FF66CC"),
        [MapCategory.CatastropheShrine] = Hex("9933FF"),
        [MapCategory.CompulsionShrine] = Hex("FF33FF"),
        [MapCategory.DeathShrine] = Hex("B3B3B3"),
        [MapCategory.DreadShrine] = Hex("6666FF"),
        [MapCategory.HallowedShrine] = Hex("E6FFCC"),
        [MapCategory.AncientSepulcher] = Hex("CC99FF"),
        [MapCategory.OtherShrine] = Hex("A04DFF"),
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
