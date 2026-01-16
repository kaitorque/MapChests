using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;

namespace Faust.QoLChests.Compatability;

internal static class QualityCompatHandler
{
    public static void Register()
    {
        if (!QualityCompat.IsInstalled)
        {
            return;
        }

        AssetBundleRegistry.Register("itemqualitiesassets");

        InteractableRegistry.Register("QualityChest1", InteractableCategory.Chest);
        InteractableRegistry.Register("QualityChest2", InteractableCategory.Chest);
        InteractableRegistry.Register("QualityEquipmentBarrel", InteractableCategory.Barrel);

        InteractableRegistry.Register("QualityDuplicator", InteractableCategory.Duplicator);
        InteractableRegistry.Register("QualityDuplicatorLarge", InteractableCategory.Barrel);
        InteractableRegistry.Register("QualityDuplicatorMilitary", InteractableCategory.Barrel);
        InteractableRegistry.Register("QualityDuplicatorWild", InteractableCategory.Barrel);
    }
}
