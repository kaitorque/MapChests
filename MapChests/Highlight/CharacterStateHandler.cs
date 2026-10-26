using System;
using System.Collections.Generic;
using Faust.Shared;
using RoR2;

namespace Faust.QoLChests.Handlers;

public static class CharacterStateHandler
{
    public static bool IsDrifter { get; private set; } = false;
    private const int DrifterSurvivorIndex = 17;

    // Store the delegates so we can properly unsubscribe later
    private static readonly Dictionary<LocalUser, Action> _bodyChangedHandlers = new Dictionary<LocalUser, Action>();

    public static void SetIsDrifter(bool isDrifter)
    {
        IsDrifter = isDrifter;
    }

    internal static void OnRunStart()
    {
        Log.LogDebug("CharacterStateHandler.OnRunStart called");
        var users = LocalUserManager.readOnlyLocalUsersList;
        foreach (var user in users)
        {
            Log.LogDebug($"CharacterStateHandler.OnRunStart - Registering OnBodyChanged handler for LocalUser: {user.userProfile?.name}");

            var bodyChangedHandler = () => OnBodyChanged(user);
            _bodyChangedHandlers[user] = bodyChangedHandler;
            user.onBodyChanged += bodyChangedHandler;
        }
    }

    private static void OnBodyChanged(LocalUser localUser)
    {
        Log.LogDebug("CharacterStateHandler.OnBodyChanged called");
        var localUserBody = localUser?.cachedBody;
        if (!localUserBody)
        {
            Log.LogDebug("CharacterStateHandler.OnBodyChanged - No local user body found, skipping");
            return;
        }
        if (!localUserBody.isPlayerControlled)
        {
            Log.LogDebug("CharacterStateHandler.OnBodyChanged - Body is not player controlled, skipping");
            return;
        }

        var survivorIndex = SurvivorCatalog.GetSurvivorIndexFromBodyIndex(localUserBody.bodyIndex);
        var isDrifter = (int)survivorIndex == DrifterSurvivorIndex;
        SetIsDrifter(isDrifter);

        Log.LogDebug(
            $"CharacterStateHandler.OnBodyChanged - Drifter run {isDrifter} - localPlayer: {localUserBody.isLocalPlayer} - isClient: {localUserBody.isClient} - isServer: {localUserBody.isServer}"
        );
    }

    internal static void OnRunBeginGameOver()
    {
        Log.LogDebug("CharacterStateHandler.OnRunBeginGameOver called");
        UnsubscribeAllBodyChangedHandlers();
    }

    internal static void OnRunOnDestroy()
    {
        Log.LogDebug("CharacterStateHandler.OnRunOnDestroy called");
        UnsubscribeAllBodyChangedHandlers();
    }

    private static void UnsubscribeAllBodyChangedHandlers()
    {
        foreach (var kvp in _bodyChangedHandlers)
        {
            var user = kvp.Key;
            var bodyChangedHandler = kvp.Value;
            Log.LogDebug($"CharacterStateHandler - Removing OnBodyChanged handler for LocalUser: {user.userProfile?.name}");
            user.onBodyChanged -= bodyChangedHandler;
        }
        _bodyChangedHandlers.Clear();
    }
}