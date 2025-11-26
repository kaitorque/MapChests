using System.Collections.Generic;
using System.Linq;
using Faust.QoLChests.Components;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Hooks;

public static class ShopHooks
{
    public static void Register()
    {
        On.RoR2.ShopTerminalBehavior.SetHasBeenPurchased += SetHasBeenPurchased;
        On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteractionOnInteractionBegin;

        #region Host only hooks
        On.RoR2.MultiShopController.OnPurchase += MultiShopControllerOnPurchase;
        #endregion
    }

    private static void PurchaseInteractionOnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
    {
        orig(self, activator);
        Log.LogDebug("PurchaseInteraction_OnInteractionBegin called");
        if (InteractableRegistry.IsBlackListed(self.gameObject.name))
            return;

        Log.LogDebug(
            $"PurchaseInteraction_OnInteractionBegin - {self.gameObject.name} - Cost: {self.cost} - NetworkAvailable: {self.Networkavailable}"
        );

        if (self.Networkavailable)
            return;

        var interactableCategoryMarker = self.GetComponent<InteractableHighlightCategoryMarker>();
        if (!interactableCategoryMarker)
            return;

        Log.LogDebug(
                 $"PurchaseInteraction_OnInteractionBegin - Hiding interactable of category: {interactableCategoryMarker.Category}"
        );

        if (interactableCategoryMarker.Category == InteractableCategory.Shop && self.gameObject.name.Contains("TripleDroneShopTerminal"))
        {
            var droneController = FindDroneMultishopController(self);
            if (!droneController)
                return;

            var controllerModelLocator = droneController.GetComponent<ModelLocator>();
            if (!controllerModelLocator)
                return;

            droneController.gameObject.AddComponent<InteractableUsed>();
            VisibilityHandler.Hide(
                interactableCategoryMarker.Category,
                self.gameObject,
                droneController.gameObject,
                controllerModelLocator.modelTransform.gameObject
            );
        }

        VisibilityHandler.Hide(interactableCategoryMarker.Category, self.gameObject);
        if (!self.gameObject.GetComponent<InteractableUsed>())
            self.gameObject.AddComponent<InteractableUsed>();

        if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
        {
            Log.LogDebug(
                $"PurchaseInteraction_OnInteractionBegin - Removing highlight from used interactable: {self.gameObject.name}"
            );
            var disabledHighlights = HighlightHandler.Disable(self.gameObject);
            Log.LogDebug(
                $"PurchaseInteraction_OnInteractionBegin - Disabled {disabledHighlights.Length} highlights"
            );
        }
    }

    private static DroneVendorMultiShopController FindDroneMultishopController(PurchaseInteraction terminalBehavior)
    {
        DroneVendorMultiShopController[] allControllers = Object.FindObjectsOfType<DroneVendorMultiShopController>();

        DroneVendorMultiShopController closestController = null;
        float minDistance = float.MaxValue;
        float maxDistance = 4.3f; // This seems to be max distance between terminal and controller

        foreach (var controller in allControllers)
        {
            Log.LogDebug(
                $"Checking DroneVendorMultiShopController: {controller.gameObject.name} - Available: {controller.available} - NetworkAvailable: {controller.Networkavailable}"
            );

            var distance = Vector3.Distance(
                terminalBehavior.transform.position,
                controller.transform.position
            );

            // If the distance is within the max distance and less than the current min distance, update the closest controller
            if (distance < maxDistance && distance < minDistance)
            {
                minDistance = distance;
                closestController = controller;
            }
        }
        Log.LogDebug(
            $"Found closest DroneVendorMultiShopController: {closestController?.gameObject.name} - Distance: {minDistance}"
        );
        // Return the closest controller within range, or null if none are found
        return closestController;
    }

    private static void MultiShopControllerOnPurchase(
        On.RoR2.MultiShopController.orig_OnPurchase orig,
        MultiShopController self,
        CostTypeDef.PayCostContext payCostContext,
        CostTypeDef.PayCostResults payCostResult
    )
    {
        orig(self, payCostContext, payCostResult);
        if (InteractableRegistry.IsBlackListed(self.gameObject.name))
            return;

        Log.LogDebug(
            $"MultiShopController_OnPurchase - "
                + $"Available: {self.available} NetworkAvailable: {self.Networkavailable} "
                + $"IsActiveAndEnabled: {self.isActiveAndEnabled}"
        );

        if (self.Networkavailable)
            return;

        var shopObjects = self
            .terminalGameObjects.Select(x => x)
            .Concat([self.gameObject])
            .ToArray();

        VisibilityHandler.Hide(InteractableCategory.Shop, shopObjects);
        foreach (var shopObject in shopObjects)
        {
            shopObject.AddComponent<InteractableUsed>();
        }
        if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
        {
            HighlightHandler.Disable(shopObjects);
        }
    }

    private static void SetHasBeenPurchased(
        On.RoR2.ShopTerminalBehavior.orig_SetHasBeenPurchased orig,
        ShopTerminalBehavior self,
        bool newHasBeenPurchased
    )
    {
        orig(self, newHasBeenPurchased);
        if (InteractableRegistry.IsBlackListed(self.gameObject.name))
            return;

        var multiShopControllerForTerminal = FindMultiShopControllerForTerminal(self);
        if (!multiShopControllerForTerminal)
            return;

        var otherShopTerminals = FindOtherShopTerminals(self).Select(x => x.gameObject);
        if (multiShopControllerForTerminal.Networkavailable)
        {
            // Sometimes due to desync or lag, the shop might be available when it shouldn't be, so we queue a check
            var recheckShop = multiShopControllerForTerminal
                .gameObject.AddComponent<RecheckShopAvailableAfterDelay>()
                .SetMultiShopController(multiShopControllerForTerminal)
                .SetTerminalObjects(
                    [.. otherShopTerminals.Select(x => x.gameObject), self.gameObject]
                );
            if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
            {
                recheckShop.RemoveHighlightAfterDelay();
            }
            recheckShop.RecheckShopAvailabilityAfterDelay();
            return;
        }

        GameObject[] shopObjects =
        [
            .. otherShopTerminals.Select(x => x.gameObject),
            self.gameObject,
            multiShopControllerForTerminal.gameObject
        ];

        VisibilityHandler.Hide(InteractableCategory.Shop, shopObjects);
        if (ModConfig.Instance.RemoveHighlightFromUsed.Value)
        {
            HighlightHandler.Disable(shopObjects);
        }
    }

    private static MultiShopController FindMultiShopControllerForTerminal(
        ShopTerminalBehavior terminalBehavior
    )
    {
        MultiShopController[] allControllers = Object.FindObjectsOfType<MultiShopController>();

        MultiShopController closestController = null;
        float minDistance = float.MaxValue;
        float maxDistance = 4.3f; // This seems to be max distance between terminal and controller

        foreach (var controller in allControllers)
        {
            Log.LogDebug(
                $"Checking MultiShopController: {controller.gameObject.name} - Available: {controller.available} - NetworkAvailable: {controller.Networkavailable}"
            );

            var distance = Vector3.Distance(
                terminalBehavior.transform.position,
                controller.transform.position
            );

            // If the distance is within the max distance and less than the current min distance, update the closest controller
            if (distance < maxDistance && distance < minDistance)
            {
                minDistance = distance;
                closestController = controller;
            }
        }
        Log.LogDebug(
            $"Found closest MultiShopController: {closestController?.gameObject.name} - Distance: {minDistance}"
        );
        // Return the closest controller within range, or null if none are found
        return closestController;
    }

    private static List<ShopTerminalBehavior> FindOtherShopTerminals(ShopTerminalBehavior self)
    {
        List<ShopTerminalBehavior> nearbyTerminals = [];

        // This seems to be the max distance between terminals
        float maxDistance = 3.3f;

        ShopTerminalBehavior[] allTerminals = Object.FindObjectsOfType<ShopTerminalBehavior>();

        foreach (var terminal in allTerminals)
        {
            // Skip the current terminal
            if (terminal == self)
            {
                continue;
            }

            var distance = Vector3.Distance(self.transform.position, terminal.transform.position);

            // If the terminal is within the max distance, consider it part of the same shop
            if (distance <= maxDistance)
            {
                Log.LogDebug(
                    $"Found nearby terminal: {terminal.gameObject.name} - Distance: {distance}"
                );
                nearbyTerminals.Add(terminal);
            }
        }

        return nearbyTerminals;
    }
}
