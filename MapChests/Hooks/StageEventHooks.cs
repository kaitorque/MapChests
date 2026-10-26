using Faust.QoLChests.Handlers;
using RoR2;

namespace Faust.QoLChests.Hooks;

/// <summary>
/// Stage events (e.g. the Wake's Ancient Sepulcher) are scene objects enabled after the stage-start highlight pass.
/// </summary>
public static class StageEventHooks
{
    public static void Register()
    {
        On.RoR2.WakeController.Start += WakeControllerStart;
    }

    private static void WakeControllerStart(On.RoR2.WakeController.orig_Start orig, WakeController self)
    {
        orig(self);
        InteractableStateHandler.TrackLateInteractable(self.gameObject);
    }
}
