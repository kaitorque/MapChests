using System.Collections.Generic;
using System.Linq;
using Faust.QoLChests;
using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            var assetSpawnCards = assetBundle.LoadAllAssets<SpawnCard>();
            foreach (var spawnCard in assetSpawnCards)
            {
                Log.LogDebug($"Processing SpawnCard: {spawnCard.name} ");
                if (InteractableRegistry.IsRegistered(spawnCard.prefab.name, out var category))
                {
                    if (!spawnCard.prefab.GetComponent<InteractableHighlightCategoryMarker>())
                    {
                        spawnCard.prefab.AddComponent<InteractableHighlightCategoryMarker>()
                            .SetCategory(category);

                        Log.LogDebug(
                            $"Added category marker for registered modded interactable - {spawnCard.prefab.name} - Category: ({category})"
                        );
                    }
                }
            }
        }
        stopwatch.Stop();
        Log.LogDebug($"{nameof(PostInit)} processing AssetBundles took {stopwatch.ElapsedMilliseconds}ms");
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
