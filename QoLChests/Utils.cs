using System.Collections.Generic;
using Faust.Shared;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

public static class Utils
{
    public static Renderer[] GetRenderers(GameObject gameObject)
    {
        var modelLocator = gameObject.GetComponent<ModelLocator>();
        var childRenderers =
            modelLocator?.modelTransform?.GetComponentsInChildren<Renderer>()
            ?? gameObject.GetComponentsInChildren<Renderer>();
        var baseRenderers =
            modelLocator?.modelTransform?.GetComponents<Renderer>()
            ?? gameObject.GetComponents<Renderer>();
        Renderer[] allRenderers = [.. baseRenderers, .. childRenderers];

        return allRenderers;
    }

    public static Renderer[] DisableRenderers(GameObject gameObject)
    {
        var allRenderers = GetRenderers(gameObject);
        var disabledRenderes = new List<Renderer>(allRenderers.Length);
        foreach (var renderer in allRenderers)
        {
            if (renderer)
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
            if (collider)
            {
                Log.LogDebug($"Disabling collider {collider.name}");
                collider.enabled = false;
                disabledColliders.Add(collider);
            }
        }
        return [.. disabledColliders];
    }

    public static void DisableCommonVisualEffects(GameObject gameObject)
    {
        SetParticleSystem(gameObject, active: false);
        SetTrailRenderer(gameObject, enabled: false);
        SetLights(gameObject, enabled: false);
    }

    private static ParticleSystem[] SetParticleSystem(GameObject gameObject, bool active)
    {
        ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            Log.LogDebug($"Disabling particle system {ps.name}");
            ps.Stop(); // Stop the particle system
            ps.gameObject.SetActive(active); // Or disable it completely
        }
        return particleSystems;
    }

    private static TrailRenderer[] SetTrailRenderer(GameObject gameObject, bool enabled)
    {
        TrailRenderer[] trailRenderers = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer trail in trailRenderers)
        {
            Log.LogDebug($"Disabling trail renderer {trail.name}");
            trail.enabled = enabled;
        }
        return trailRenderers;
    }

    private static Light[] SetLights(GameObject gameObject, bool enabled)
    {
        Light[] lights = gameObject.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            Log.LogDebug($"Disabling light {light.name}");
            light.enabled = enabled;
        }
        return lights;
    }
}
