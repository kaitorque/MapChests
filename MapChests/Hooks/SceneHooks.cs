using System;
using System.Linq;
using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using RoR2;
using UnityEngine;
using MapChests.Categories;

namespace Faust.QoLChests.Hooks;

public static class SceneHooks
{
    public static void Register()
    {
        On.RoR2.SceneDirector.Start += Start;
    }

    private static void Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
    {
        orig(self);
        AddHighlightCategoryToPreplaced();
        AddCustomHandlingOfSpecialScenes();
        CategoryClassifier.TagUntagged();
        InteractableStateHandler.Reset();
        Log.LogDebug(
            $"SceneDirector.Start - {InteractableStateHandler.InteractablesToHighlight.Count} interactables highlighted"
        );
    }

    private static void AddCustomHandlingOfSpecialScenes()
    {
        var primeMeridian = GameObject.Find("Chests");
        var goldenShores = GameObject.Find("HOLDER: Preplaced Goodies");
        AddChestMarkersToChildren(primeMeridian);
        AddChestMarkersToChildren(goldenShores);
    }

    private static void AddHighlightCategoryToPreplaced()
    {
        var allGameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        var preplacedHolders = new string[]
        {
            "HOLDER: Preplaced",
            "HOLDER: Newt",
            "HOLDER: Secret"
        };
        foreach (var gameObject in allGameObjects)
        {
            if (preplacedHolders.Any(holder => gameObject.name.Contains(holder, StringComparison.OrdinalIgnoreCase)))
            {
                Log.LogDebug($"Found Relevant Holder: {gameObject.name}");
                FindNewtStatueInHolder(gameObject);
                FindChestsInHolder(gameObject);
                FindPressurePlateInHolder(gameObject);
            }
        }
    }

    private static void FindPressurePlateInHolder(GameObject holder)
    {
        if (!holder)
            return;

        var highlights = holder.GetComponentsInChildren<Highlight>();
        foreach (var highlight in highlights)
        {
            if (!highlight.gameObject || !highlight.gameObject.name.Contains("GLPressure", StringComparison.OrdinalIgnoreCase))
                continue;

            Log.LogDebug($"Pressure Plate found in Holder: {holder.name} - {highlight.gameObject.name}");
            AddInteractableMarker(MapCategory.PressurePlate, highlight);
        }
    }

    private static void FindNewtStatueInHolder(GameObject holder)
    {
        if (!holder)
            return;

        var highlights = holder.GetComponentsInChildren<Highlight>();
        foreach (var highlight in highlights)
        {
            if (!highlight.gameObject || !highlight.gameObject.name.Contains("NewtStatue", StringComparison.OrdinalIgnoreCase))
                continue;

            Log.LogDebug($"Newt Statue found in Holder: {holder.name} - {highlight.gameObject.name}");
            AddInteractableMarker(MapCategory.NewtAltar, highlight);
        }
    }

    private static void FindChestsInHolder(GameObject holder)
    {
        if (!holder)
            return;

        var highlights = holder.GetComponentsInChildren<Highlight>();
        foreach (var highlight in highlights)
        {
            if (!highlight.gameObject || !highlight.gameObject.name.Contains("Chest", StringComparison.OrdinalIgnoreCase))
                continue;

            Log.LogDebug($"Chest found in Holder: {holder.name} - {highlight.gameObject.name}");
            if (holder.name.Contains("Secret", StringComparison.OrdinalIgnoreCase))
                AddInteractableMarker(MapCategory.SecretChest, highlight);
            else
                AddInteractableMarker(ChestCategory(highlight.gameObject), highlight);
        }
    }

    private static void AddChestMarkersToChildren(GameObject holder)
    {
        if (!holder)
            return;

        var highlights = holder.GetComponentsInChildren<Highlight>();
        foreach (var highlight in highlights)
        {
            AddInteractableMarker(ChestCategory(highlight.gameObject), highlight);
        }
    }

    private static MapCategory ChestCategory(GameObject chest) =>
        CategoryCatalog.TryGetByObjectName(chest.name, out var category) ? category : MapCategory.OtherChest;

    private static void AddInteractableMarker(MapCategory category, Highlight highlight)
    {
        if (!highlight.gameObject.TryGetComponent<CategoryMarker>(out _))
        {
            highlight.gameObject
                .AddComponent<CategoryMarker>()
                .SetCategory(category);

            Log.LogDebug(
                $"AddInteractableMarkerToChildren - Adding interactable marker for {highlight.gameObject.name}"
            );
        }
    }
}
