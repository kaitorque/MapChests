using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Faust.Shared.Compatability;
using MapChests.Categories;
using MiniMapLibrary;
using UnityEngine;

namespace Faust.QoLChests.Configs;

/// <summary>
/// The one config file, <c>kaitorque.MapChests.cfg</c>: global hide and minimap settings plus one section per <see cref="MapCategory"/>.
/// </summary>
public class ModConfig
{
    private static ModConfig instance = null;
    public static ModConfig Instance =>
        instance ?? throw new InvalidOperationException("ModConfig is not initialized. Call Init() first.");

    public static void Init(ConfigFile config, LegacyConfig legacy)
    {
        instance ??= new ModConfig(config, legacy);
    }

    public ConfigFile File { get; }

    public ConfigEntry<bool> RemoveHighlightFromUsed, FadeInsteadOfHide, DoNotHideAsDrifter;
    public ConfigEntry<float> HideTime;

    public ConfigEntry<KeyCode> MinimapKey, ZoomInKey, ZoomOutKey;
    public ConfigEntry<float> ZoomLevel;
    public ConfigEntry<LogLevel> MinimapLogLevel;

    private readonly Dictionary<MapCategory, CategorySettings> categories = [];

    private ModConfig(ConfigFile config, LegacyConfig legacy)
    {
        File = config;

        HideTime = config.Bind("Hide", "Time", legacy.HighlightFloat("Hide", "Time", 1f), "Seconds before a used interactable is hidden");
        FadeInsteadOfHide = config.Bind("Hide", "Fade", legacy.HighlightBool("Hide", "Fade", true), "Fade used interactables instead of hiding them");
        RemoveHighlightFromUsed = config.Bind("Hide", "Remove highlight when used", legacy.HighlightBool("Highlight", "RemoveWhenUsed", true), "Remove the highlight once an interactable is used");
        DoNotHideAsDrifter = config.Bind("Hide", "Do not hide as Drifter", legacy.HighlightBool("Highlight", "DoNotHideAsDrifter", true), "Do not hide used chests while playing Drifter");

        MinimapKey = config.Bind("Minimap", "Toggle key", legacy.MinimapEnum("Settings.General", "EnableKey", KeyCode.M), "Key that shows or hides the minimap");
        ZoomInKey = config.Bind("Minimap", "Zoom in key", legacy.MinimapEnum("Settings.Zoom", "ZoomKey", KeyCode.Equals), "Zooms the minimap in");
        ZoomOutKey = config.Bind("Minimap", "Zoom out key", legacy.MinimapEnum("Settings.Zoom", "UnZoomKey", KeyCode.Minus), "Zooms the minimap out");
        ZoomLevel = config.Bind("Minimap", "Zoom level", legacy.MinimapFloat("Settings.Zoom", "zoomLevel", 1.5f), "Zoom of the minimap; the zoom keys change it and it is kept between stages");
        MinimapLogLevel = config.Bind("Minimap", "Log level", legacy.MinimapEnum("Settings.General", "LogLevel", LogLevel.info), "How much the minimap writes to the log");

        foreach (var definition in CategoryCatalog.All)
            categories[definition.Category] = new CategorySettings(config, definition, legacy);

        if (RiskOfOptionsCompat.IsInstalled)
            RegisterRiskOfOptions();
    }

    public CategorySettings Get(MapCategory category) => categories[category];

    internal bool IsCategoryHighlightEnabled(MapCategory category) => Get(category).HighlightEnabled;

    internal bool IsCategoryHideEnabled(MapCategory category) => Get(category).HideEnabled;

    internal Color GetHighlightColor(MapCategory category) => Get(category).HighlightColor?.Value ?? Color.white;

    private void RegisterRiskOfOptions()
    {
        RiskOfOptionsCompat.SetModDescription(
            $"Highlights interactables, hides them once used, and shows them on a minimap.{Environment.NewLine}Icon paths, sizes and elevation markers are set in the config file."
        );
        RiskOfOptionsCompat.AddCheckboxOptions(false, FadeInsteadOfHide, RemoveHighlightFromUsed, DoNotHideAsDrifter);
        RiskOfOptionsCompat.AddSliderNumberOptions(false, 0.1f, 5f, HideTime);
        RiskOfOptionsCompat.AddSliderNumberOptions(false, 0.2f, 5f, ZoomLevel);

        foreach (var settings in categories.Values)
        {
            if (settings.Highlight != null)
            {
                RiskOfOptionsCompat.AddCheckboxOptions(false, settings.Highlight);
                RiskOfOptionsCompat.AddColorOptions(false, settings.HighlightColor);
                RiskOfOptionsCompat.AddCheckboxOptions(false, settings.HideWhenUsed);
            }
            RiskOfOptionsCompat.AddCheckboxOptions(false, settings.Minimap);
            RiskOfOptionsCompat.AddColorOptions(false, settings.MinimapColor);
            if (settings.UsedIcon != null)
                RiskOfOptionsCompat.AddDropdownOptions(false, settings.UsedIcon);
        }
    }
}
