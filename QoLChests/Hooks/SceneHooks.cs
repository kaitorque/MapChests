using Faust.Shared;
using RoR2;

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
        InteractableStateHandler.Reset();
        Log.LogDebug(
            $"SceneDirector.Start - {InteractableStateHandler.InteractablesToHighlight.Count} interactables highlighted"
        );
    }
}
