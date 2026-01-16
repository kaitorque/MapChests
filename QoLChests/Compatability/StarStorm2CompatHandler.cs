using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;

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

        InteractableRegistry.Register("ShockDroneBroken", InteractableCategory.Drone);
        InteractableRegistry.Register("CloneDroneBroken", InteractableCategory.Drone);
        InteractableRegistry.Register("DroneTablePrefab", InteractableCategory.Duplicator);
    }
}
