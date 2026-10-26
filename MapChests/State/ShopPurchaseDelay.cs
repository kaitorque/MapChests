using Faust.QoLChests.Api;
using System.Collections;
using Faust.QoLChests.Handlers;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Components;

public class RecheckShopAvailableAfterDelay(float Delay = 0.1f) : MonoBehaviour
{
    private MultiShopController MultiShopController;
    private GameObject[] TerminalObjects = [];

    public void RecheckShopAvailabilityAfterDelay()
    {
        StartCoroutine(RecheckShopAfterDelay());
    }

    public RecheckShopAvailableAfterDelay SetMultiShopController(
        MultiShopController multiShopController
    )
    {
        MultiShopController = multiShopController;
        return this;
    }

    public RecheckShopAvailableAfterDelay SetTerminalObjects(GameObject[] terminalObjects)
    {
        TerminalObjects = terminalObjects;
        return this;
    }

    private IEnumerator RecheckShopAfterDelay()
    {
        yield return new WaitForSeconds(Delay);

        if (MultiShopController && !MultiShopController.Networkavailable)
        {
            InteractableUsedHandler.MarkUsed(
                InteractableCategory.Shop,
                [MultiShopController.gameObject, .. TerminalObjects]
            );
        }
        Destroy(this);
    }
}
