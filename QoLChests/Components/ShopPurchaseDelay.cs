using System.Collections;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Components;

public class RecheckShopAvailableAfterDelay(float Delay = 0.1f) : MonoBehaviour
{
    private MultiShopController MultiShopController;
    private GameObject[] TerminalObjects = [];
    private bool RemoveHighlightFromUsed = false;

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

    public RecheckShopAvailableAfterDelay RemoveHighlightAfterDelay()
    {
        RemoveHighlightFromUsed = true;
        return this;
    }

    private IEnumerator RecheckShopAfterDelay()
    {
        yield return new WaitForSeconds(Delay);

        if (MultiShopController && !MultiShopController.Networkavailable)
        {
            VisibilityHandler.Hide(
                InteractableCategory.Shop,
                [MultiShopController.gameObject, .. TerminalObjects]
            );

            if (RemoveHighlightFromUsed)
            {
                HighlightHandler.Disable([MultiShopController.gameObject, .. TerminalObjects]);
            }
        }
        Destroy(this);
    }
}
