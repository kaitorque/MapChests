using System.Collections.Generic;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

public static class Utils
{
    public static Renderer[] GetRenderers(GameObject gameObject)
    {
        var model = gameObject.GetComponent<ModelLocator>()?.modelTransform;
        return model ? model.GetComponentsInChildren<Renderer>() : gameObject.GetComponentsInChildren<Renderer>();
    }

    public static Renderer[] DisableRenderers(GameObject gameObject, bool includeModels = true)
    {
        var allRenderers = GetRenderers(gameObject);
        var disabledRenderes = new List<Renderer>(allRenderers.Length);
        foreach (var renderer in allRenderers)
        {
            if (renderer && renderer.enabled
                && (includeModels || renderer is not MeshRenderer and not SkinnedMeshRenderer))
            {
                Log.LogDebug($"Disabling renderer {renderer.name}");
                renderer.enabled = false;
                disabledRenderes.Add(renderer);
            }
        }
        return [.. disabledRenderes];
    }

    public static Collider[] DisableColliders(GameObject gameObject)
    {
        Collider[] allColliders = gameObject.GetComponentsInChildren<Collider>();
        var disabledColliders = new List<Collider>(allColliders.Length);
        foreach (var collider in allColliders)
        {
            if (collider && collider.enabled)
            {
                Log.LogDebug($"Disabling collider {collider.name}");
                collider.enabled = false;
                disabledColliders.Add(collider);
            }
        }
        return [.. disabledColliders];
    }

    public static System.Action DisableUsedVisuals(GameObject gameObject, bool fade)
    {
        var renderers = DisableRenderers(gameObject, includeModels: !fade);
        var colliders = DisableColliders(gameObject);
        var restoreEffects = DisableCommonVisualEffects(gameObject, deactivateParticleObjects: !fade);
        return () =>
        {
            restoreEffects();
            foreach (var collider in colliders)
                if (collider) collider.enabled = true;
            foreach (var renderer in renderers)
                if (renderer) renderer.enabled = true;
        };
    }

    public static System.Action DisableCommonVisualEffects(GameObject gameObject, bool deactivateParticleObjects = true)
    {
        var restore = new List<System.Action>();
        var reactivate = new List<GameObject>();
        foreach (var particle in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if (!particle)
                continue;
            var wasPlaying = particle.isPlaying;
            Log.LogDebug($"Disabling particle system {particle.name}");
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (wasPlaying)
                restore.Add(() => { if (particle) particle.Play(false); });
            if (deactivateParticleObjects)
            {
                var particleObject = particle.gameObject;
                if (particleObject.activeSelf)
                {
                    particleObject.SetActive(false);
                    reactivate.Add(particleObject);
                }
            }
        }

        foreach (var trail in gameObject.GetComponentsInChildren<TrailRenderer>())
        {
            if (!trail || !trail.enabled)
                continue;
            Log.LogDebug($"Disabling trail renderer {trail.name}");
            trail.enabled = false;
            restore.Add(() => { if (trail) trail.enabled = true; });
        }

        foreach (var light in gameObject.GetComponentsInChildren<Light>())
        {
            if (!light || !light.enabled)
                continue;
            Log.LogDebug($"Disabling light {light.name}");
            light.enabled = false;
            restore.Add(() => { if (light) light.enabled = true; });
        }

        return () =>
        {
            foreach (var obj in reactivate)
                if (obj) obj.SetActive(true);
            for (var i = restore.Count - 1; i >= 0; i--)
                restore[i]();
        };
    }
}
