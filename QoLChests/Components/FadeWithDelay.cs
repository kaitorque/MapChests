using System;
using System.Collections;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests.Components;

public class FadeWithDelay : MonoBehaviour
{
    private const float TargetFade = 0.25f;
    private float delay = 1f;
    private Action restore;

    public FadeWithDelay SetDelay(float value)
    {
        delay = value;
        return this;
    }

    public void DisableRendererAfterDelay() => StartCoroutine(FadeAfterDelay());

    private IEnumerator FadeAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        if (restore != null)
            yield break;

        // Keep model renderers enabled; the game's shaders dither their _Fade property.
        var restoreVisuals = restore = Utils.DisableUsedVisuals(gameObject, fade: true);
        var restoreBlocks = new List<Action>();
        foreach (var renderer in Utils.GetRenderers(gameObject))
        {
            if (!renderer || !renderer.enabled
                || renderer is not MeshRenderer and not SkinnedMeshRenderer)
                continue;

            var original = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(original);
            var faded = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(faded);
            faded.SetFloat(CommonShaderProperties._Fade, TargetFade);
            renderer.SetPropertyBlock(faded);
            restoreBlocks.Add(() =>
            {
                if (renderer) renderer.SetPropertyBlock(original);
            });
        }

        restore = () =>
        {
            foreach (var undo in restoreBlocks)
                undo();
            restoreVisuals();
        };
    }

    public void OnDestroy() => restore?.Invoke();
}
