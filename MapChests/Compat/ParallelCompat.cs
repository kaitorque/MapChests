using System;
using System.Reflection;
using BepInEx.Bootstrap;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using UnityEngine;

namespace Faust.QoLChests.Compatability;

/// <summary>
/// ArtifactOfParallel gives each player private interactables that only they may see. Its <c>ParallelApi</c> is found
/// by reflection, so MapChests builds and runs without it.
/// </summary>
internal static class ParallelCompat
{
    public const string PluginGUID = "kaitorque.ArtifactOfParallel";

    private static Func<GameObject, bool> isVisibleToLocalPlayer;

    /// <summary>False for another player's private object; true for everything else, or when ArtifactOfParallel is not installed.</summary>
    public static bool IsVisible(GameObject gameObject) => isVisibleToLocalPlayer == null || isVisibleToLocalPlayer(gameObject);

    public static void Register()
    {
        if (!Chainloader.PluginInfos.TryGetValue(PluginGUID, out var plugin) || !plugin.Instance)
            return;

        try
        {
            var api = plugin.Instance.GetType().Assembly.GetType("ArtifactOfParallel.ParallelApi", throwOnError: true);
            var isVisible = api.GetMethod("IsVisibleToLocalPlayer", BindingFlags.Public | BindingFlags.Static, null, [typeof(GameObject)], null);
            var visibilityChanged = api.GetEvent("LocalVisibilityChanged", BindingFlags.Public | BindingFlags.Static);
            if (isVisible == null || visibilityChanged == null)
            {
                Log.LogWarning("ArtifactOfParallel is installed but its ParallelApi is missing members; private objects may show for everyone.");
                return;
            }

            isVisibleToLocalPlayer = (Func<GameObject, bool>)Delegate.CreateDelegate(typeof(Func<GameObject, bool>), isVisible);
            visibilityChanged.AddEventHandler(null, new Action<GameObject, bool>(OnLocalVisibilityChanged));
            Log.LogInfo("ArtifactOfParallel found: highlights and minimap icons follow its private objects");
        }
        catch (Exception e)
        {
            isVisibleToLocalPlayer = null;
            Log.LogWarning($"Could not connect to ArtifactOfParallel's ParallelApi; private objects may show for everyone: {e.Message}");
        }
    }

    private static void OnLocalVisibilityChanged(GameObject gameObject, bool visible)
    {
        if (visible)
            InteractableStateHandler.TrackLateInteractable(gameObject);
        else
            InteractableStateHandler.HideInteractable(gameObject);
    }
}
