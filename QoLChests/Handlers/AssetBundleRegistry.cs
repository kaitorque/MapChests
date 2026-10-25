using System.Collections.Generic;
using Faust.Shared;

namespace Faust.QoLChests.Handlers;

public static class AssetBundleRegistry
{
    private static readonly HashSet<string> RegisteredAssetBundles = new();

    /// <summary>
    /// Register an asset bundle by name, to track them dynamically.
    /// </summary>
    /// <param name="assetBundleName"></param>
    public static void Register(string assetBundleName)
    {
        Log.LogDebug($"Registering asset bundle {assetBundleName}");
        RegisteredAssetBundles.Add(assetBundleName);
    }

    public static bool IsRegistered(string assetBundleName)
    {
        return RegisteredAssetBundles.Contains(assetBundleName);
    }
}
