using System.Collections.Generic;
using System.Linq;
using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.Shared;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Faust.QoLChests.Handlers;

public static class InteractableStateHandler
{
    private static List<GameObject> TrackedSceneInteractables { get; set; } = [];
    private static List<GameObject> TrackedInteractables { get; set; } = [];
    public static List<GameObject> InteractablesToHighlight { get; set; } = [];

    public static void Reset()
    {
        // Remove highlights from tracked resources
        HighlightHandler.Disable([.. InteractablesToHighlight, .. TrackedSceneInteractables, .. TrackedInteractables]);

        TrackedSceneInteractables.Clear();
        InteractablesToHighlight.Clear();
        // Enable highlights for tracked resources
        TrackInteractables();
    }

    public static void Init()
    {
        LoadHighlightableResources();
    }

    public static void PostInit()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        foreach (var assetBundle in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (!AssetBundleRegistry.IsRegistered(assetBundle.name))
                continue;

            Log.LogDebug($"Processing AssetBundle: {assetBundle.name} ");

            var assets = assetBundle.LoadAllAssets();

            foreach (var asset in assets)
            {
                if (asset is GameObject go)
                    TagRegisteredPrefab(go);
                else if (asset is SpawnCard sc)
                    TagRegisteredPrefab(sc.prefab);
            }
        }

        // Prefabs created at runtime (e.g. PrefabAPI.InstantiateClone) are in no asset bundle or director pool
        foreach (var prefab in ContentManager.networkedObjectPrefabs)
        {
            TagRegisteredPrefab(prefab);
        }

        stopwatch.Stop();
        Log.LogDebug($"{nameof(PostInit)} processing AssetBundles and content packs took {stopwatch.ElapsedMilliseconds}ms");
    }

    private static void TagRegisteredPrefab(GameObject prefab)
    {
        if (!prefab || !InteractableRegistry.IsRegistered(prefab.name, out var category))
            return;

        if (prefab.GetComponent<InteractableHighlightCategoryMarker>())
            return;

        prefab.AddComponent<InteractableHighlightCategoryMarker>().SetCategory(category);
        Log.LogDebug(
            $"Added category marker for registered modded interactable - {prefab.name} - Category: ({category})"
        );
    }

    private static void TrackInteractables()
    {
        if (InteractablesToHighlight.Count > 0)
        {
            Log.LogWarning(
                $"InteractablesToHighlight is not empty - Call Reset() before calling TrackInteractables()"
            );
            return;
        }

        var sceneInteractables = Object
            .FindObjectsOfType<InteractableHighlightCategoryMarker>()
            .Select(x => x.gameObject);

        var trackedInteractables = sceneInteractables.Concat(TrackedInteractables).ToArray();
        Log.LogDebug(
            $"Configuring highlights - {trackedInteractables.Length} tracked interactables"
        );
        foreach (var interactable in trackedInteractables)
        {
            if (!interactable)
                continue;
            var highlightCategoryMarker =
                interactable.GetComponent<InteractableHighlightCategoryMarker>();
            if (!highlightCategoryMarker)
            {
                Log.LogWarning(
                    $"Interactable {interactable.name} does not have a HighlightCategoryMarker - why are we tracking it?"
                );
                continue;
            }
            if (ModConfig.Instance.IsCategoryHighlightEnabled(highlightCategoryMarker.Category))
                InteractablesToHighlight.Add(interactable);
        }

        Log.LogDebug(
            $"Configuring highlights - {trackedInteractables.Length} tracked interactables out of which {InteractablesToHighlight.Count} will be highlighted"
        );

        // Add highlights to tracked resources
        foreach (var interactable in InteractablesToHighlight)
        {
            var hideWithDelay = interactable.GetComponent<HideWithDelay>();
            var fadeWithDelay = interactable.GetComponent<FadeWithDelay>();
            var hasBeenUsed = interactable.GetComponent<InteractableUsed>();
            if (fadeWithDelay || hideWithDelay || hasBeenUsed)
                continue;

            HighlightHandler.Enable(interactable);
        }
    }

    private static void LoadHighlightableResources()
    {
        // Load highlightable resources
        AddResourcesToHighlights(Constants.ChestResourcesPaths);
        AddResourcesToHighlights(Constants.BarrelResourcesPaths);
        AddResourcesToHighlights(Constants.LockboxResourcesPaths);
        AddResourcesToHighlights(Constants.StealthedChestResourcePaths);
        AddResourcesToHighlights(Constants.ShopResourcePaths);
        AddResourcesToHighlights(Constants.ScrapperResourcePaths);
        AddResourcesToHighlights(Constants.DuplicatorResourcesPaths);
        AddResourcesToHighlights(Constants.DroneResourcesPaths);
        AddResourcesToHighlights(Constants.TurrentResourcePaths);
        AddResourcesToHighlights(Constants.ArtifactOfDevotionResourcePaths);
        AddResourcesToHighlights(Constants.ShrineResourcePaths);
    }

    private static void AddResourcesToHighlights(HighlightableResource[] resources)
    {
        foreach (var resource in resources)
        {
            var addressable = Addressables
                .LoadAssetAsync<GameObject>(resource.ResourcePath)
                .WaitForCompletion();
            if (addressable)
            {
                addressable
                    .AddComponent<InteractableHighlightCategoryMarker>()
                    .SetCategory(resource.Category);
                TrackedInteractables.Add(addressable);
            }
        }
    }
}
