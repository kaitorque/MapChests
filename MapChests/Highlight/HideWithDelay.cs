using System;
using System.Collections;
using Faust.QoLChests.Handlers;
using UnityEngine;

namespace Faust.QoLChests.Components;

public class HideWithDelay : MonoBehaviour
{
    private float delay = 1f;
    private Action restore;

    public HideWithDelay SetDelay(float value)
    {
        delay = value;
        return this;
    }

    public void DisableRendererAfterDelay() => StartCoroutine(HideAfterDelay());

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        if (restore != null)
            yield break;
        restore = Utils.DisableUsedVisuals(gameObject, fade: false);
        HighlightHandler.Disable(gameObject);
    }

    public void OnDestroy() => restore?.Invoke();
}
