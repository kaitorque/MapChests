using System.Collections;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

/// <summary>
/// Attach this to a Renderer component to disable it after a specified delay.
/// </summary>
/// <param name="Delay"></param>
public class HideWithDelay(float Delay = 1f) : MonoBehaviour
{
    public void DisableRendererAfterDelay()
    {
        StartCoroutine(DisableRendererCoroutine());
    }

    public HideWithDelay SetDelay(float delay)
    {
        Delay = delay;
        return this;
    }

    private IEnumerator DisableRendererCoroutine()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(Delay);
        var baseRenderer = GetComponent<Renderer>();
        var childRenderer = GetComponentInChildren<Renderer>();
        Renderer[] allRenderes = [baseRenderer, childRenderer];
        var baseHighlights = GetComponent<Highlight>();
        var childHighlights = GetComponentsInChildren<Highlight>();
        Highlight[] allHighlights = [baseHighlights, .. childHighlights];
        foreach (var renderer in allRenderes)
        {
            if (renderer)
            {
                renderer.enabled = false;
            }
        }
        foreach (var highlight in allHighlights)
        {
            if (highlight)
            {
                highlight.isOn = false;
                highlight.enabled = false;
            }
        }
        Destroy(this);
    }
}
