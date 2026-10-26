using Faust.QoLChests.Handlers;
using RoR2;

namespace Faust.QoLChests.Hooks;

public static class CharacterHooks
{
    private const int DrifterSurvivorIndex = 17;

    public static void Register()
    {
        On.RoR2.Run.Start += OnRunStart;
        On.RoR2.Run.OnDestroy += OnRunOnDestroy;
        On.RoR2.Run.BeginGameOver += OnRunBeginGameOver;
    }

    private static void OnRunBeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
    {
        orig(self, gameEndingDef);
        CharacterStateHandler.OnRunBeginGameOver();
    }

    private static void OnRunOnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
    {
        orig(self);
        CharacterStateHandler.OnRunOnDestroy();
    }

    private static void OnRunStart(On.RoR2.Run.orig_Start orig, Run self)
    {
        orig(self);
        CharacterStateHandler.OnRunStart();
    }
}
