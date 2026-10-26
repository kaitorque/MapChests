using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;
using MapChests.Categories;

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

        InteractableRegistry.Register("InfernoDroneBroken", MapCategory.Drone);
        InteractableRegistry.Register("VoltaicDroneBroken", MapCategory.Drone);
        InteractableRegistry.Register("Shrine of Ruin", MapCategory.OtherShrine);
        InteractableRegistry.Register("Shrine of Sacrifice", MapCategory.OtherShrine);
        InteractableRegistry.Register("Shrine of the Future", MapCategory.OtherShrine);
        
    }
}
