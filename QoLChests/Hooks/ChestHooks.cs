using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using RoR2;

namespace Faust.QoLChests.Hooks;

public static class ChestHooks
{
    public static void Register()
    {
        On.EntityStates.Barrel.Opened.OnEnter += BarrelOpened;
        On.RoR2.RouletteChestController.Opened.OnEnter += RouletteOpened;
        On.RoR2.DelusionChestController.ResetChestForDelusion += ResetChestForDelusion;
    }

    private static void BarrelOpened(
        On.EntityStates.Barrel.Opened.orig_OnEnter orig,
        EntityStates.Barrel.Opened self
    )
    {
        orig(self);
        if (InteractableRegistry.IsBlackListed(self.outer.gameObject.name))
            return;

        self.outer.gameObject.AddComponent<InteractableUsed>();

        var categoryMarker =
            self.outer.gameObject.GetComponent<InteractableHighlightCategoryMarker>();
        if (categoryMarker)
        {
            VisibilityHandler.Hide(
                categoryMarker.Category,
                self.outer.gameObject,
                self.outer.commonComponents.modelLocator.modelTransform.gameObject
            );
        }
        else
        {
            Log.LogWarning(
                $"Opened barrel {self.outer.gameObject.name} has no interactable category marker; skipping hide"
            );
        }

        if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
        {
            HighlightHandler.Disable(self.outer.gameObject);
        }
    }

    private static void RouletteOpened(
        On.RoR2.RouletteChestController.Opened.orig_OnEnter orig,
        RouletteChestController.Opened self
    )
    {
        orig(self);
        if (InteractableRegistry.IsBlackListed(self.outer.gameObject.name))
            return;

        self.outer.gameObject.AddComponent<InteractableUsed>();
        VisibilityHandler.Hide(InteractableCategory.Chest, self.outer.gameObject);

        if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
        {
            HighlightHandler.Disable(self.outer.gameObject);
        }
    }

    private static void ResetChestForDelusion(
        On.RoR2.DelusionChestController.orig_ResetChestForDelusion orig,
        DelusionChestController self
    )
    {
        orig(self);

        VisibilityHandler.Show(self.gameObject);

        if (ModConfig.Instance.HighlightChests.Value)
        {
            HighlightHandler.Enable(self.gameObject, ignoreUsed: true);
        }
    }
}
