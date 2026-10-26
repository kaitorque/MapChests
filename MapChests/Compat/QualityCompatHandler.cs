using Faust.QoLChests.Handlers;
using Faust.Shared.Compatability;
using MapChests.Categories;

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

        InteractableRegistry.Register("QualityChest1", MapCategory.SmallChest);
        InteractableRegistry.Register("QualityChest2", MapCategory.LargeChest);

        InteractableRegistry.Register("QualityEquipmentBarrel", MapCategory.EquipmentBarrel);

        InteractableRegistry.Register("Chest2StealthedVariant", MapCategory.CloakedChest);

        InteractableRegistry.Register("SpeedOnPickupBarrel", MapCategory.Barrel);

        InteractableRegistry.Register("QualityDuplicator", MapCategory.CommonPrinter);
        InteractableRegistry.Register("QualityDuplicatorLarge", MapCategory.UncommonPrinter);
        InteractableRegistry.Register("QualityDuplicatorMilitary", MapCategory.LegendaryPrinter);
        InteractableRegistry.Register("QualityDuplicatorWild", MapCategory.BossPrinter);
    }
}
