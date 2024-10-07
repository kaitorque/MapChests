using System;
using BepInEx.Configuration;
using Faust.Shared.Compatability;

namespace Faust.QoLChests.Configs;

public class ModConfig
{
    public ConfigEntry<bool> HideEmptyChests,
        HideUsedShops,
        RemoveHighlightFromUsed,
        FadeInsteadOfHide,
        HighlightChests,
        HighlightShops,
        HighlightScrapper,
        HighlightDuplicator,
        HighlightDrones,
        HightlightTurrets,
        HighlightStealthedChests;

    public ConfigEntry<float> HideTime;
    public ConfigEntry<ConfigHighlightColor> HighlightColor,
        HighlightChestColor,
        HighlightShopColor,
        HighlightScrapperColor,
        HighlightDuplicatorColor,
        HighlightDronesColor,
        HighlightTurretsColor;

    public ModConfig(ConfigFile config)
    {
        HideEmptyChests = config.Bind(
            "Hide",
            "Chest",
            true,
            "Hides empty chests after a few seconds"
        );
        HideUsedShops = config.Bind("Hide", "Shops", true, "Hides used shops after a few seconds");
        HideTime = config.Bind("Hide", "Time", 1f, "Time before stuff is hidden");
        FadeInsteadOfHide = config.Bind("Hide", "Fade", false, "Fade instead of hiding");

        RemoveHighlightFromUsed = config.Bind(
            "Highlight",
            "RemoveWhenUsed",
            false,
            "Remove highlight when used"
        );
        HighlightChests = config.Bind(
            "Highlight",
            "Chest",
            true,
            "Highlight Chests (Chests, Barrels etc.)"
        );
        HighlightStealthedChests = config.Bind(
            "Highlight",
            "Stealthed Chests",
            true,
            "Highlight stealthed chests"
        );
        HighlightDuplicator = config.Bind("Highlight", "Duplicator", true, "Highlight Duplicators");
        HighlightScrapper = config.Bind("Highlight", "Scrapper", true, "Highlight Scrappers");
        HighlightShops = config.Bind("Highlight", "Shops", true, "Highlight Shops");
        HighlightDrones = config.Bind("Highlight", "Drones", true, "Highlight Drones");
        HightlightTurrets = config.Bind("Highlight", "Turrets", true, "Highlight Turrets");

        HighlightColor = config.Bind(
            "Highlight",
            "Color",
            ConfigHighlightColor.Yellow,
            "Highlight color for interactables (Only used for backwards compatibility)"
        );
        HighlightChestColor = config.Bind(
            "Highlight",
            "ChestColor",
            HighlightColor.Value,
            "Highlight color for chests"
        );
        HighlightShopColor = config.Bind(
            "Highlight",
            "ShopColor",
            HighlightColor.Value,
            "Highlight color for shops"
        );
        HighlightScrapperColor = config.Bind(
            "Highlight",
            "ScrapperColor",
            HighlightColor.Value,
            "Highlight color for scrappers"
        );
        HighlightDuplicatorColor = config.Bind(
            "Highlight",
            "DuplicatorColor",
            HighlightColor.Value,
            "Highlight color for duplicators"
        );
        HighlightDronesColor = config.Bind(
            "Highlight",
            "DronesColor",
            HighlightColor.Value,
            "Highlight color for drones"
        );
        HighlightTurretsColor = config.Bind(
            "Highlight",
            "TurretsColor",
            HighlightColor.Value,
            "Highlight color for turrets"
        );

        //Softdependencies
        if (RiskOfOptionsCompat.IsInstalled)
        {
            RiskOfOptionsCompat.SetModDescription(
                $"Hides chests when they are empty.{Environment.NewLine}Hides used shop terminals.{Environment.NewLine}Highlights chests and interactables."
            );
            RiskOfOptionsCompat.AddCheckboxOptions(
                restartRequired: false,
                HideEmptyChests,
                HideUsedShops,
                FadeInsteadOfHide,
                RemoveHighlightFromUsed
            );
            RiskOfOptionsCompat.AddCheckboxOptions(
                restartRequired: false,
                HighlightChests,
                HighlightDuplicator,
                HighlightScrapper,
                HighlightShops,
                HighlightDrones,
                HightlightTurrets,
                HighlightStealthedChests
            );
            RiskOfOptionsCompat.AddSliderNumberOptions(restartRequired: false, 0.1f, 5f, HideTime);
            RiskOfOptionsCompat.AddDropdownOptions(
                false,
                HighlightColor,
                HighlightChestColor,
                HighlightShopColor,
                HighlightScrapperColor,
                HighlightDuplicatorColor,
                HighlightDronesColor,
                HighlightTurretsColor
            );
        }
    }

    internal ConfigEntry<ConfigHighlightColor> GetCategoryHighlightColorConfig(
        InteractableCategory category
    )
    {
        return category switch
        {
            InteractableCategory.Chest => HighlightChestColor,
            InteractableCategory.Shop => HighlightShopColor,
            InteractableCategory.Scrapper => HighlightScrapperColor,
            InteractableCategory.Duplicator => HighlightDuplicatorColor,
            InteractableCategory.Drone => HighlightDronesColor,
            InteractableCategory.Turret => HighlightTurretsColor,
            InteractableCategory.StealthedChest => HighlightChestColor,
            InteractableCategory.Shrine => HighlightColor,
            _ => HighlightColor
        };
    }

    internal bool IsCategoryEnabled(InteractableCategory category)
    {
        return category switch
        {
            InteractableCategory.Chest => HighlightChests.Value,
            InteractableCategory.Shop => HighlightShops.Value,
            InteractableCategory.Scrapper => HighlightScrapper.Value,
            InteractableCategory.Duplicator => HighlightDuplicator.Value,
            InteractableCategory.Drone => HighlightDrones.Value,
            InteractableCategory.Turret => HightlightTurrets.Value,
            InteractableCategory.StealthedChest => HighlightStealthedChests.Value,
            InteractableCategory.Shrine => false,
            _ => false
        };
    }
}
