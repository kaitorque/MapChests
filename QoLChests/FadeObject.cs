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
        private float FadeOutTime => Faust.QoLChests.QoLChests.HideTime.Value;

        // specialized chests have two renderers
        private Renderer[] renderers;
        private MaterialPropertyBlock propertyStorage;
        private readonly List<Color> _originalColors = new();

        private float currentFade = 1f;
        private float currentBrightness = 1f;

        private void Start()
        {
            propertyStorage = new MaterialPropertyBlock();
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            StartCoroutine(LerpBrightnessAndFade());
            StartCoroutine(WaitUntilVisible());
        }

        private void RefreshRenderers()
        {
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            _originalColors.Clear();
            foreach (var renderer in renderers)
            {
                _originalColors.Add(renderer.material.color);
            }
        }

        private bool RenderersAreVisible()
        {
            bool result = true;
            foreach (var renderer in renderers)
            {
                if (renderer == null)
                {
                    RefreshRenderers();
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
            var currentLerp = 0f;
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
            var ready = false;
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
            for (var i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                var color = _originalColors[i];
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
                RefreshRenderers();
                return;
            }
            if (gameObject.name == "mdlBarrel1")
            {
                propertyStorage.SetFloat("_Fade", currentFade);
            }
            else
            {
                var oldFade = propertyStorage.GetFloat("_Fade");
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
