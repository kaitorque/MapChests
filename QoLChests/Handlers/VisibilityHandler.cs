using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Handlers;

public static class VisibilityHandler
{
    public static void Hide(InteractableCategory category, params GameObject[] gameObjects)
    {
        if (ModConfig.Instance.IsCategoryHideEnabled(category))
        {
            Hide(gameObjects);
        }
    }

    public static void Hide(params GameObject[] gameObjects)
    {
        if (ModConfig.Instance.FadeInsteadOfHide.Value)
        {
            foreach (var obj in gameObjects)
            {
                if (!obj.GetComponent<FadeWithDelay>())
                    obj.AddComponent<FadeWithDelay>()
                        .SetDelay(ModConfig.Instance.HideTime.Value)
                        .DisableRendererAfterDelay();
            }
            return;
        }

        foreach (var obj in gameObjects)
        {
            if (!obj.GetComponent<HideWithDelay>())
                obj.AddComponent<HideWithDelay>()
                    .SetDelay(ModConfig.Instance.HideTime.Value)
                    .DisableRendererAfterDelay();
        }
    }

    public static void Show(params GameObject[] gameObjects)
    {
        foreach (var obj in gameObjects)
        {
            Show(obj);
        }
    }

    private static void Show(GameObject interactable)
    {
        var modelLocator = interactable.gameObject.GetComponent<ModelLocator>();
        if (modelLocator)
        {
            var modelTransformHide =
                modelLocator.modelTransform.gameObject.GetComponent<HideWithDelay>();
            var modelTransformFade =
                modelLocator.modelTransform.gameObject.GetComponent<FadeWithDelay>();
            var modelTransformHasBeenUsed =
                modelLocator.modelTransform.gameObject.GetComponent<InteractableUsed>();
            if (modelTransformHasBeenUsed)
                Object.Destroy(modelTransformHasBeenUsed);
            if (modelTransformHide)
                Object.Destroy(modelTransformHide);
            if (modelTransformFade)
                Object.Destroy(modelTransformFade);
        }

        var hasBeenUsed = interactable.gameObject.GetComponent<InteractableUsed>();
        if (hasBeenUsed)
            Object.Destroy(hasBeenUsed);

        var hide = interactable.gameObject.GetComponent<HideWithDelay>();
        if (hide)
            Object.Destroy(hide);

        var fade = interactable.gameObject.GetComponent<FadeWithDelay>();
        if (fade)
            Object.Destroy(fade);
    }
}
