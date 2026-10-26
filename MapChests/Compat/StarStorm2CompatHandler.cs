using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;
using MapChests.Categories;

namespace Faust.QoLChests.Compatability;

internal static class StarStorm2CompatHandler
{
    public static void Register()
    {
        if (!StarStorm2Compat.IsInstalled)
        {
            return;
        }

        AssetBundleRegistry.Register("ss2interactables");

        InteractableRegistry.Register("ShockDroneBroken", MapCategory.Drone);
        InteractableRegistry.Register("CloneDroneBroken", MapCategory.Drone);
        InteractableRegistry.Register("DroneTablePrefab", MapCategory.OtherPrinter);
    }
}
