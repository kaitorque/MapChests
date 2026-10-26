using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.Shared;
using UnityEngine;
using MapChests.Categories;

namespace Faust.QoLChests.Handlers;

/// <summary>
/// The single flow for used interactables. Every hook and the public API go through here,
/// so used interactables behave the same no matter how they were used.
/// </summary>
public static class InteractableUsedHandler
{
    /// <summary>
    /// Marks the interactable as used, hides it and removes its highlight.
    /// Blacklisted interactables are not hidden.
    /// </summary>
    public static void MarkUsed(GameObject interactable)
    {
        if (!interactable)
            return;

        var categoryMarker = interactable.GetComponent<CategoryMarker>();
        if (!categoryMarker)
        {
            Log.LogWarning(
                $"Used interactable {interactable.name} has no interactable category marker; skipping hide"
            );
        }

        MarkUsed(interactable, categoryMarker ? categoryMarker.Category : null);
    }

    /// <summary>
    /// Same as <see cref="MarkUsed(GameObject)"/>, using <paramref name="category"/> for objects without a category marker.
    /// </summary>
    public static void MarkUsed(MapCategory category, params GameObject[] interactables)
    {
        foreach (var interactable in interactables)
        {
            if (!interactable)
                continue;

            var categoryMarker = interactable.GetComponent<CategoryMarker>();
            MarkUsed(interactable, categoryMarker ? categoryMarker.Category : category);
        }
    }

    private static void MarkUsed(GameObject interactable, MapCategory? category)
    {
        InteractableUsed.Mark(interactable);

        if (category.HasValue && !InteractableRegistry.IsBlackListed(interactable.name))
        {
            VisibilityHandler.Hide(category.Value, interactable);
        }

        if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
        {
            HighlightHandler.Disable(interactable);
        }
    }

    /// <summary>
    /// Reverts <see cref="MarkUsed(GameObject)"/>: shows the interactable again and restores its highlight.
    /// </summary>
    public static void MarkAvailable(GameObject interactable)
    {
        if (!interactable)
            return;

        VisibilityHandler.Show(interactable);

        var categoryMarker = interactable.GetComponent<CategoryMarker>();
        if (categoryMarker && ModConfig.Instance.IsCategoryHighlightEnabled(categoryMarker.Category))
        {
            HighlightHandler.Enable(interactable, ignoreUsed: true);
        }
    }
}
