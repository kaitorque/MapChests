using System;
using System.Collections.Generic;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace MapChests.Categories;

/// <summary>
/// Finds the category of objects that were not tagged at startup (preplaced objects, other mods' interactables)
/// and tags them, so highlighting and the minimap see the same category.
/// </summary>
public static class CategoryClassifier
{
    private static readonly HashSet<string> LoggedUnknown = [];

    private static readonly Type[] CandidateTypes =
    [
        typeof(ChestBehavior),
        typeof(RouletteChestController),
        typeof(ShrineBloodBehavior),
        typeof(ShrineChanceBehavior),
        typeof(ShrineCombatBehavior),
        typeof(ShrineHealingBehavior),
        typeof(ShrineRestackBehavior),
        typeof(ShrineBossBehavior),
        typeof(ScrapperController),
        typeof(SummonMasterBehavior),
        typeof(BarrelInteraction),
        typeof(TeleporterInteraction),
        typeof(PurchaseInteraction),
        typeof(GenericInteraction),
    ];

    /// <summary>
    /// Tags every untagged interactable in the scene that can be classified.
    /// </summary>
    public static void TagUntagged()
    {
        foreach (var type in CandidateTypes)
        {
            foreach (var found in UnityEngine.Object.FindObjectsOfType(type))
            {
                var gameObject = ((Component)found).gameObject;
                if (gameObject.GetComponentInParent<CategoryMarker>() || gameObject.GetComponentInChildren<CategoryMarker>())
                    continue;

                if (TryClassify(gameObject, out var category))
                    CategoryMarker.Tag(gameObject, category);
            }
        }
    }

    /// <summary>
    /// The object's category: its marker if it has one, otherwise the fallback rules.
    /// </summary>
    public static bool TryClassify(GameObject gameObject, out MapCategory category)
    {
        var marker = gameObject.GetComponent<CategoryMarker>();
        if (marker)
        {
            category = marker.Category;
            return true;
        }

        if (CategoryCatalog.TryGetByObjectName(gameObject.name, out category)
            || InteractableRegistry.IsRegistered(gameObject.name, out category)
            || TryClassifyByComponents(gameObject, out category))
        {
            return true;
        }

        if (LoggedUnknown.Add(gameObject.name))
            Log.LogInfo($"No category for interactable {gameObject.name}");

        return false;
    }

    private static bool TryClassifyByComponents(GameObject gameObject, out MapCategory category)
    {
        var token = gameObject.TryGetComponent<PurchaseInteraction>(out var purchase) ? purchase.contextToken ?? "" : "";

        MapCategory? result = gameObject switch
        {
            _ when gameObject.TryGetComponent<ChestBehavior>(out _) && token.Contains("STEALTH") => MapCategory.CloakedChest,
            _ when gameObject.TryGetComponent<ChestBehavior>(out _) && token == "LUNAR_CHEST_CONTEXT" => MapCategory.LunarPod,
            _ when gameObject.TryGetComponent<ChestBehavior>(out _) && token.Contains("CHEST") => MapCategory.OtherChest,
            _ when gameObject.TryGetComponent<RouletteChestController>(out _) => MapCategory.AdaptiveChest,
            _ when gameObject.TryGetComponent<ShrineBloodBehavior>(out _) => MapCategory.BloodShrine,
            _ when gameObject.TryGetComponent<ShrineChanceBehavior>(out _) => MapCategory.ChanceShrine,
            _ when gameObject.TryGetComponent<ShrineCombatBehavior>(out _) => MapCategory.CombatShrine,
            _ when gameObject.TryGetComponent<ShrineHealingBehavior>(out _) => MapCategory.WoodsShrine,
            _ when gameObject.TryGetComponent<ShrineRestackBehavior>(out _) => MapCategory.OrderShrine,
            _ when gameObject.TryGetComponent<ShrineBossBehavior>(out _) => MapCategory.MountainShrine,
            _ when gameObject.TryGetComponent<ScrapperController>(out _) => MapCategory.Scrapper,
            _ when gameObject.TryGetComponent<SummonMasterBehavior>(out _) => MapCategory.Drone,
            _ when gameObject.TryGetComponent<BarrelInteraction>(out _) => MapCategory.Barrel,
            _ when gameObject.TryGetComponent<TeleporterInteraction>(out _) => MapCategory.Teleporter,
            _ when gameObject.TryGetComponent<PortalStatueBehavior>(out var statue)
                && statue.portalType == PortalStatueBehavior.PortalType.Shop => MapCategory.NewtAltar,
            _ when token.Contains("DUPLICATOR") => MapCategory.OtherPrinter,
            _ when token == "FAN_CONTEXT" => MapCategory.Special,
            _ when token.Contains("TERMINAL") => MapCategory.OtherShop,
            _ when token.Contains("EQUIPMENTBARREL") => MapCategory.EquipmentBarrel,
            _ when token.Contains("GOLDSHORE") => MapCategory.GoldAltar,
            _ when token.Contains("TOTEM") => MapCategory.GoldTotem,
            _ when gameObject.TryGetComponent<GenericInteraction>(out var generic) =>
                generic.contextToken?.Contains("PORTAL") == true ? MapCategory.Portal : MapCategory.Special,
            _ => null,
        };

        category = result.GetValueOrDefault();
        return result.HasValue;
    }
}
