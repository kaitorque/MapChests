using Faust.QoLChests.Api;
using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.Shared;
using UnityEngine;

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

        var categoryMarker = interactable.GetComponent<InteractableHighlightCategoryMarker>();
        if (!categoryMarker)
        {
            Log.LogWarning(
                $"Used interactable {interactable.name} has no interactable category marker; skipping hide"
            );
        }

        MarkUsed(interactable, categoryMarker ? categoryMarker.Category : null);
    }

    /// <summary>
    /// Same as <see cref="MarkUsed(GameObject)"/>, with an explicit hide category.
    /// </summary>
    public static void MarkUsed(InteractableCategory category, params GameObject[] interactables)
    {
        foreach (var interactable in interactables)
        {
            if (interactable)
                MarkUsed(interactable, category);
        }
    }

    private static void MarkUsed(GameObject interactable, InteractableCategory? category)
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

        var categoryMarker = interactable.GetComponent<InteractableHighlightCategoryMarker>();
        if (categoryMarker && ModConfig.Instance.IsCategoryHighlightEnabled(categoryMarker.Category))
        {
            HighlightHandler.Enable(interactable, ignoreUsed: true);
        }
    }
}
