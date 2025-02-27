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
        if (ModConfig.Instance.HideEmptyChests.Value)
        {
            VisibilityHandler.Hide(
                self.outer.gameObject,
                self.outer.commonComponents.modelLocator.modelTransform.gameObject
            );
        }
        else if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
        {
            HighlightHandler.Disable(self.outer.gameObject);
        }
    }

    private static void RouletteOpened(
        On.RoR2.RouletteChestController.Opened.orig_OnEnter orig,
        EntityStates.EntityState self
    )
    {
        orig(self);
        if (InteractableRegistry.IsBlackListed(self.outer.gameObject.name))
            return;

        self.outer.gameObject.AddComponent<InteractableUsed>();
        if (ModConfig.Instance.HideEmptyChests.Value)
        {
            VisibilityHandler.Hide(self.outer.gameObject);
        }
        else if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
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
