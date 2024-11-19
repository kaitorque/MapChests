using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;

internal static class StarStorm2CompatHandler
{
    public static void Register()
    {
        if (StarStorm2Compat.IsInstalled)
        {
            InteractableRegistry.Register("ShockDroneBroken", InteractableCategory.Drone);
            InteractableRegistry.Register("CloneDroneBroken", InteractableCategory.Drone);
            InteractableRegistry.Register("DroneTablePrefab", InteractableCategory.Duplicator);
        }
    }
}
