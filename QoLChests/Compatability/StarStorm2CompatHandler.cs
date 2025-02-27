using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;

internal static class StarStorm2CompatHandler
{
    public static void Register()
    {
        InteractableRegistry.Register("ShockDroneBroken", InteractableCategory.Drone);
        InteractableRegistry.Register("CloneDroneBroken", InteractableCategory.Drone);
        InteractableRegistry.Register("DroneTablePrefab", InteractableCategory.Duplicator);
    }
}
