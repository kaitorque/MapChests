using System.Linq;
using Faust.QoLChests.Components;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using RoR2;

public static class InteractableSpawnHooks
{
    public static void Register()
    {
        On.RoR2.ClassicStageInfo.Start += ClassicStageInfoStart;
    }

    private static void ClassicStageInfoStart(
        On.RoR2.ClassicStageInfo.orig_Start orig,
        ClassicStageInfo self
    )
    {
        orig(self);

        var poolCategories = self.interactableDccsPool.poolCategories;
        var enabledDirectorCardSelections = poolCategories.SelectMany(x =>
            x.alwaysIncluded.Select(x => x.dccs)
        );
        var anyIncludedIfCondtionsMet = false;
        foreach (var poolCategory in poolCategories.Select(x => x.includedIfConditionsMet))
        {
            var requiredExpansions = poolCategory.SelectMany(x => x.requiredExpansions).Distinct();
            var expansionsEnabled = requiredExpansions.All(Run.instance.IsExpansionEnabled);

            if (expansionsEnabled)
            {
                enabledDirectorCardSelections = enabledDirectorCardSelections.Concat(
                    poolCategory.Select(x => x.dccs)
                );
                anyIncludedIfCondtionsMet = true;
            }
        }
        if (!anyIncludedIfCondtionsMet)
        {
            enabledDirectorCardSelections = enabledDirectorCardSelections.Concat(
                poolCategories.SelectMany(x => x.includedIfNoConditionsMet.Select(x => x.dccs))
            );
        }

        var interactableSpawnCards = enabledDirectorCardSelections
            .SelectMany(x => x.categories)
            .SelectMany(x => x.cards)
            .Select(x => x.spawnCard)
            .Where(x => x is InteractableSpawnCard)
            .Cast<InteractableSpawnCard>()
            .ToHashSet();

        foreach (var isc in interactableSpawnCards)
        {
            if (isc.prefab == null)
                continue;

            if (InteractableRegistry.IsRegistered(isc.prefab.name, out var category))
            {
                if (!isc.prefab.GetComponent<InteractableHighlightCategoryMarker>())
                {
                    isc.prefab.AddComponent<InteractableHighlightCategoryMarker>()
                        .SetCategory(category);

                    Log.LogDebug(
                        $"Added category marker for registered modded interactable - {isc.prefab.name} - Category: ({category})"
                    );
                }
            }
        }
    }
}
