using System.Collections.Generic;
using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Handlers;

public static class HighlightHandler
{
    public static void Enable(GameObject[] interactable)
    {
        foreach (var gameObject in interactable)
        {
            Enable(gameObject);
        }
    }

    public static void Enable(GameObject interactable, bool ignoreUsed = false)
    {
        if (!interactable)
            return;

        var highlight = interactable.GetComponent<Highlight>();
        if (!highlight)
            return;

        if (ModConfig.Instance.RemoveHighlightFromUsed.Value && ignoreUsed is false)
        {
            var interactableUsed = interactable.GetComponent<InteractableUsed>();
            if (interactableUsed)
            {
                Log.LogInfo(
                    "HighlightHandler.Enable: Interactable has been used, disabling highlight"
                );
                return;
            }
        }

        var highlightCategory = interactable.GetComponent<InteractableHighlightCategoryMarker>();
        if (!highlightCategory)
        {
            Log.LogWarning(
                $"GameObject {interactable.name} does not have a HighlightCategoryMarker - why are we trying to highlight it?"
            );
            return;
        }
        var highlightConfigColor = ModConfig
            .Instance.GetCategoryHighlightColorConfig(highlightCategory.Category)
            .Value;

        highlight.isOn = true;
        highlight.enabled = true;
        highlight.highlightColor = Highlight.HighlightColor.custom;
        highlight.CustomColor = Constants.GetColor(highlightConfigColor);
    }

    public static Highlight[] Disable(GameObject[] gameObject)
    {
        var highlights = new List<Highlight>();
        foreach (var obj in gameObject)
        {
            highlights.AddRange(Disable(obj));
        }
        return [.. highlights];
    }

    public static Highlight[] Disable(GameObject gameObject)
    {
        return DisableHighlights(gameObject);
    }

    public static Highlight[] GetHighlights(GameObject gameObject)
    {
        if (!gameObject)
            return [];
        var baseHighlight = gameObject.GetComponent<Highlight>();

        return [baseHighlight];
    }

    private static Highlight[] DisableHighlights(GameObject gameObject)
    {
        if (!gameObject)
            return [];

        var allHighlights = GetHighlights(gameObject);
        var disabledHighlights = new List<Highlight>(allHighlights.Length);

        foreach (var highlight in allHighlights)
        {
            if (highlight)
            {
                highlight.isOn = false;
                highlight.enabled = false;
                disabledHighlights.Add(highlight);
            }
        }
        return [.. disabledHighlights];
    }
}
