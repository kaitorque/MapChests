using System;
using BepInEx.Configuration;
using Faust.Shared.Compatability;

namespace Faust.QoLChests.Configs;

public class ModConfig
{
    private static ModConfig instance = null;
    public static ModConfig Instance
    {
        get
        {
            if (instance == null)
            {
                throw new InvalidOperationException(
                    "ModConfig is not initialized. Call Init() first."
                );
            }
            return instance;
        }
    }

    public static void Init(ConfigFile config)
    {
        instance ??= new ModConfig(config);
    }

    public ConfigEntry<bool> HideEmptyChests,
        HideUsedShops,
        HideUsedChests,
        HideUsedBarrels,
        RemoveHighlightFromUsed,
        FadeInsteadOfHide,
        DoNotHideAsDrifter;

    public ConfigEntry<bool> HighlightChests,
        HighlightShops,
        HighlightScrapper,
        HighlightDuplicator,
        HighlightDrones,
        HighlightTurrets,
        HighlightStealthedChests,
        HighlightLockboxes,
        HighlightBarrels,
        HighlightPressurePlates,
        HighlightNewtStatues,
        HighlightShrines;

    public ConfigEntry<float> HideTime;
    public ConfigEntry<ConfigHighlightColor> HighlightColor,
        HighlightChestColor,
        HighlightShopColor,
        HighlightScrapperColor,
        HighlightDuplicatorColor,
        HighlightDronesColor,
        HighlightTurretsColor,
        HighlightStealthedChestsColor,
        HighlightLockboxesColor,
        HighlightBarrelColor,
        HighlightNewtStatueColor,
        HighlightPressurePlateColor,
        HighlightShrineColor;

    private ModConfig(ConfigFile config)
    {
        HideEmptyChests = config.Bind("Hide", "Chest", true, "Hides empty chests after a few seconds");
        HideUsedShops = config.Bind("Hide", "Shops", true, "Hides used shops after a few seconds");
        HideUsedBarrels = config.Bind("Hide", "Barrels", true, "Hides used barrels after a few seconds");

        RemoveHighlightFromUsed = config.Bind("Highlight", "RemoveWhenUsed", false, "Remove highlight when used");
        DoNotHideAsDrifter = config.Bind("Highlight", "DoNotHideAsDrifter", false, "Do not hide used chests as Drifter");
        FadeInsteadOfHide = config.Bind("Hide", "Fade", false, "Fade instead of hiding");
        HideTime = config.Bind("Hide", "Time", 1f, "Time before stuff is hidden");

        HighlightChests = config.Bind("Highlight", "Chest", true, "Highlight Chests");
        HighlightStealthedChests = config.Bind("Highlight", "Stealthed Chests", true, "Highlight stealthed chests");
        HighlightLockboxes = config.Bind("Highlight", "Lockboxes", true, "Highlight Lockboxes");
        HighlightDuplicator = config.Bind("Highlight", "Duplicator", true, "Highlight Duplicators");
        HighlightScrapper = config.Bind("Highlight", "Scrapper", true, "Highlight Scrappers");
        HighlightShops = config.Bind("Highlight", "Shops", true, "Highlight Shops");
        HighlightDrones = config.Bind("Highlight", "Drones", true, "Highlight Drones");
        HighlightTurrets = config.Bind("Highlight", "Turrets", true, "Highlight Turrets");
        HighlightBarrels = config.Bind("Highlight", "Barrels", true, "Highlight Barrels");
        HighlightNewtStatues = config.Bind("Highlight", "Newt Statues", true, "Highlight Newt Statues");
        HighlightPressurePlates = config.Bind("Highlight", "Pressure Plates", true, "Highlight Pressure Plates");
        HighlightShrines = config.Bind("Highlight", "Shrines", false, "Highlight Shrines");

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
        HighlightStealthedChestsColor = config.Bind(
            "Highlight",
            "StealthedChestsColor",
            HighlightColor.Value,
            "Highlight color for stealthed chests"
        );

        HighlightLockboxesColor = config.Bind(
            "Highlight",
            "LockboxesColor",
            HighlightColor.Value,
            "Highlight color for lockboxes"
        );

        HighlightBarrelColor = config.Bind(
            "Highlight",
            "BarrelColor",
            HighlightColor.Value,
            "Highlight color for barrels"
        );

        HighlightNewtStatueColor = config.Bind(
            "Highlight",
            "NewtStatueColor",
            HighlightColor.Value,
            "Highlight color for Newt Statues"
        );

        HighlightPressurePlateColor = config.Bind(
            "Highlight",
            "PressurePlateColor",
            HighlightColor.Value,
            "Highlight color for Pressure Plates"
        );

        HighlightShrineColor = config.Bind(
            "Highlight",
            "ShrineColor",
            HighlightColor.Value,
            "Highlight color for Shrines"
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
                HideUsedBarrels,
                FadeInsteadOfHide,
                RemoveHighlightFromUsed,
                DoNotHideAsDrifter
            );

            RiskOfOptionsCompat.AddCheckboxOptions(
                restartRequired: false,
                HighlightChests,
                HighlightDuplicator,
                HighlightScrapper,
                HighlightShops,
                HighlightDrones,
                HighlightTurrets,
                HighlightStealthedChests,
                HighlightLockboxes,
                HighlightBarrels,
                HighlightNewtStatues,
                HighlightPressurePlates,
                HighlightShrines
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
                HighlightTurretsColor,
                HighlightStealthedChestsColor,
                HighlightLockboxesColor,
                HighlightBarrelColor,
                HighlightNewtStatueColor,
                HighlightPressurePlateColor,
                HighlightShrineColor
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
            InteractableCategory.StealthedChest => HighlightStealthedChestsColor,
            InteractableCategory.Lockbox => HighlightLockboxesColor,
            InteractableCategory.Barrel => HighlightBarrelColor,
            InteractableCategory.NewtStatue => HighlightNewtStatueColor,
            InteractableCategory.PressurePlate => HighlightPressurePlateColor,
            InteractableCategory.Shrine => HighlightShrineColor,
            _ => HighlightColor
        };
    }

    internal bool IsCategoryHighlightEnabled(InteractableCategory category)
    {
        return category switch
        {
            InteractableCategory.Chest => HighlightChests.Value,
            InteractableCategory.Shop => HighlightShops.Value,
            InteractableCategory.Scrapper => HighlightScrapper.Value,
            InteractableCategory.Duplicator => HighlightDuplicator.Value,
            InteractableCategory.Drone => HighlightDrones.Value,
            InteractableCategory.Turret => HighlightTurrets.Value,
            InteractableCategory.StealthedChest => HighlightStealthedChests.Value,
            InteractableCategory.Lockbox => HighlightLockboxes.Value,
            InteractableCategory.Barrel => HighlightBarrels.Value,
            InteractableCategory.NewtStatue => HighlightNewtStatues.Value,
            InteractableCategory.PressurePlate => HighlightPressurePlates.Value,
            InteractableCategory.Shrine => HighlightShrines.Value,
            _ => false
        };
    }

    internal bool IsCategoryHideEnabled(InteractableCategory category)
    {
        return category switch
        {
            InteractableCategory.Chest => HideEmptyChests.Value,
            InteractableCategory.StealthedChest => HideEmptyChests.Value,
            InteractableCategory.Shop => HideUsedShops.Value,
            InteractableCategory.Barrel => HideUsedBarrels.Value,
            _ => false
        };
    }
}
