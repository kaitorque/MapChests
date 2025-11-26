using System.Linq;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using RoR2;

public static class CharacterHooks
{
    private const int DrifterSurvivorIndex = 17;

    public static void Register()
    {
        CharacterBody.onBodyStartGlobal += IsPlayingAsDrifter;
    }

    private static void IsPlayingAsDrifter(CharacterBody body)
    {
        if (!body.isPlayerControlled)
            return;

        var master = body.master;
        if (!master)
            return;

        var pcmc = master.playerCharacterMasterController;
        if (!pcmc)
            return;

        var networkUser = pcmc.networkUser;
        if (!networkUser)
            return;

        var isLocalUser = LocalUserManager.readOnlyLocalUsersList.Any(lu => lu.currentNetworkUser == networkUser);

        if (!isLocalUser)
        {
            Log.LogDebug(
                $"CharacterBody.onBodyStartGlobal - Not the local player - Skipping Drifter check - isClient: {body.isClient} - isServer: {body.isServer}"
            );
            return;
        }

        var survivorIndex = SurvivorCatalog.GetSurvivorIndexFromBodyIndex(body.bodyIndex);

        if ((int)survivorIndex == DrifterSurvivorIndex)
        {
            CharacterStateHandler.SetIsDrifter(true);
            Log.LogDebug(
                $"CharacterBody.onBodyStartGlobal - Drifter run detected - localPlayer: {body.isLocalPlayer} - isClient: {body.isClient} - isServer: {body.isServer}"
            );
        }
    }
}
