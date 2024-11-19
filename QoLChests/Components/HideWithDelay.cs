using System.Collections;
using Faust.QoLChests.Handlers;
using UnityEngine;

namespace Faust.QoLChests.Components;

/// <summary>
/// Attach this to a Renderer component to disable it after a specified delay.
/// </summary>
/// <param name="Delay"></param>
public class HideWithDelay(float Delay = 1f) : MonoBehaviour
{
    private Collider[] colliders;
    private Renderer[] renderers;

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

        renderers = Utils.DisableRenderers(gameObject);
        colliders = Utils.DisableColliders(gameObject);
        HighlightHandler.Disable(gameObject);

        Utils.DisableCommonVisualEffects(gameObject);
    }

    public void OnDestroy()
    {
        foreach (var collider in colliders)
        {
            if (collider)
                collider.enabled = true;
        }
        foreach (var renderer in renderers)
        {
            if (renderer)
                renderer.enabled = true;
        }
    }
}
