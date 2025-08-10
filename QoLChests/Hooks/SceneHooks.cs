using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.Shared;
using RoR2;
using UnityEngine;

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
        AddCustomHandlingOfSpecialScenes();
        InteractableStateHandler.Reset();
        Log.LogDebug(
            $"SceneDirector.Start - {InteractableStateHandler.InteractablesToHighlight.Count} interactables highlighted"
        );
    }

    private static void AddCustomHandlingOfSpecialScenes()
    {
        var primeMeridian = GameObject.Find("Chests");
        var goldenShores = GameObject.Find("HOLDER: Preplaced Goodies");
        AddInteractableMarkerToChildren(primeMeridian, InteractableCategory.Chest);
        AddInteractableMarkerToChildren(goldenShores, InteractableCategory.Chest);
    }

    private static void AddInteractableMarkerToChildren(GameObject holder, InteractableCategory category)
    {
        if (!holder)
            return;

        var highlights = holder.GetComponentsInChildren<Highlight>();
        foreach (var highlight in highlights)
        {
            if (!highlight.gameObject.TryGetComponent<InteractableHighlightCategoryMarker>(out _))
            {
                highlight.gameObject
                    .AddComponent<InteractableHighlightCategoryMarker>()
                    .SetCategory(category);

                Log.LogDebug(
                    $"AddInteractableMarkerToChildren - Adding interactable marker for {highlight.gameObject.name}"
                );
            }
        }
    }
}
