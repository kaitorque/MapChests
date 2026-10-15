using Faust.QoLChests.Handlers;
using UnityEngine;

namespace Faust.QoLChests.Api;

/// <summary>
/// Public API for other mods to integrate with QoLChests.
/// Reference QoLChests as a soft dependency and guard calls with
/// <c>BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Faust.QoLChests")</c>.
/// </summary>
public static class QoLChestsApi
{
    /// <summary>
    /// Registers an interactable prefab so QoLChests tracks and highlights it under <paramref name="category"/>.
    /// Call this from your plugin's Awake, before the game finishes loading.
    /// </summary>
    /// <param name="prefabName">Prefab name, without "(Clone)"; the clone name is registered too.</param>
    public static void RegisterInteractable(string prefabName, InteractableCategory category) =>
        InteractableRegistry.Register(prefabName, category);

    /// <summary>
    /// Registers an asset bundle so the prefabs and spawn cards in it whose names were passed to
    /// <see cref="RegisterInteractable"/> get tracked, even when they are not spawned through a stage's director pool.
    /// Call this from your plugin's Awake; the bundle must be loaded before the game finishes loading.
    /// </summary>
    /// <param name="assetBundleName">The loaded <see cref="AssetBundle.name"/>.</param>
    public static void RegisterAssetBundle(string assetBundleName) =>
        AssetBundleRegistry.Register(assetBundleName);

    /// <summary>
    /// Tells QoLChests to never hide or fade this interactable when it is used.
    /// </summary>
    /// <param name="prefabName">Prefab name, without "(Clone)"; the clone name is blacklisted too.</param>
    public static void BlackListInteractable(string prefabName) =>
        InteractableRegistry.BlackList(prefabName);

    /// <summary>
    /// Marks an interactable as used. QoLChests hides it and removes its highlight, as set in the user's config.
    /// Only necessary to call this if your mod blocks the vanilla used signals.
    /// Client side only: call it on each client that must show the interactable as used.
    /// </summary>
    public static void MarkInteractableUsed(GameObject interactable) =>
        InteractableUsedHandler.MarkUsed(interactable);

    /// <summary>
    /// Reverts <see cref="MarkInteractableUsed"/>, e.g. when your mod makes an interactable usable again.
    /// </summary>
    public static void MarkInteractableAvailable(GameObject interactable) =>
        InteractableUsedHandler.MarkAvailable(interactable);
}
