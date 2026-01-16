using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;

namespace Faust.QoLChests.Compatability;

internal static class SandsweptCompatHandler
{
    public static void Register()
    {
        if (!SandSweptCompat.IsInstalled)
        {
            return;
        }

        AssetBundleRegistry.Register("sandsweptassets2");

        InteractableRegistry.Register("InfernoDroneBroken", InteractableCategory.Drone);
        InteractableRegistry.Register("VoltaicDroneBroken", InteractableCategory.Drone);
        InteractableRegistry.Register("Shrine of Ruin", InteractableCategory.Shrine);
        InteractableRegistry.Register("Shrine of Sacrifice", InteractableCategory.Shrine);
        InteractableRegistry.Register("Shrine of the Future", InteractableCategory.Shrine);
        
    }
}
