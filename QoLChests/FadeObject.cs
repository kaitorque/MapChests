using Faust.QoLChests;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QoLChests
{
    //This is pretty much a direct copy of https://github.com/SuperSupermario24/FadeEmptyChests/blob/master/FadeObject.cs and all credits should go to the original author.
    public class FadeObject : MonoBehaviour
    {
        private float TargetFade => 0.25f;
        private float TargetBrightness => 0.5f;
        private float FadeOutTime => 1f;

        // specialized chests have two renderers
        private Renderer[] renderers;
        private MaterialPropertyBlock propertyStorage;
        private List<Color> originalColors = new List<Color>();

        private float currentFade = 1f;
        private float currentBrightness = 1f;

        private void Start()
        {
            propertyStorage = new MaterialPropertyBlock();
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            StartCoroutine(LerpBrightnessAndFade());
            StartCoroutine(WaitUntilVisible());
            Log.LogDebug($"Object name: {gameObject.name}");
        }

        private void RefreshRenderers()
        {
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            originalColors.Clear();
            foreach (Renderer renderer in renderers)
            {
                originalColors.Add(renderer.material.color);
            }
        }

        private bool RenderersAreVisible()
        {
            bool result = true;
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    // epic 1
                    RefreshRenderers();
                    //Log.LogWarning(
                    //    $"Renderers became null, refreshing reference to renderers - methodName: {methodName}");
                    result = false;
                    break;
                }
                else if (!renderer.isVisible)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private IEnumerator LerpBrightnessAndFade()
        {
            float currentLerp = 0f;
            while (currentLerp <= 1f)
            {
                currentFade = Mathf.Lerp(1f, TargetFade, currentLerp);
                currentBrightness = Mathf.Lerp(1f, TargetBrightness, currentLerp);
                currentLerp += Time.deltaTime / FadeOutTime;
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        private IEnumerator WaitUntilVisible()
        {
            // delay until container is on-screen or else things break
            // also WaitUntil throws NRE when isVisible becomes true?
            bool ready = false;
            while (!ready)
            {
                yield return new WaitForEndOfFrame();
                ready = RenderersAreVisible();
            }
            // waiting a few frames prevents errors
            yield return new WaitForSecondsRealtime(0.05f);
            RefreshRenderers();
            SceneCamera.onSceneCameraPreRender += OnSceneCameraPreRender;
            yield break;
        }

        private void OnSceneCameraPreRender(SceneCamera _)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                Color color = originalColors[i];
                ChangeColor(renderer, color);
                ChangeFade(renderer);
            }
        }

        private void ChangeColor(Renderer renderer, Color color)
        {
            try
            {
                renderer.material.color = color * currentBrightness;
            }
            catch (NullReferenceException)
            {
                // epic 2
                Log.LogWarning(
                    "Setting color failed, refreshing reference to renderers");
                RefreshRenderers();
                return;
            }
        }

        private void ChangeFade(Renderer renderer)
        {
            try
            {
                renderer.GetPropertyBlock(propertyStorage);
            }
            catch (NullReferenceException)
            {
                // epic 3
                Log.LogWarning(
                    "GetPropertyBlock failed, refreshing reference to renderers");
                RefreshRenderers();
                return;
            }
            if (gameObject.name == "mdlBarrel1")
            {
                propertyStorage.SetFloat("_Fade", currentFade);
            }
            else
            {
                float oldFade = propertyStorage.GetFloat("_Fade");
                propertyStorage.SetFloat("_Fade", oldFade * currentFade);
            }
            renderer.SetPropertyBlock(propertyStorage);
        }

        // clean up leftover hooks
        private void OnDestroy()
        {
            SceneCamera.onSceneCameraPreRender -= OnSceneCameraPreRender;
        }
    }
}
