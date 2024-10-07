using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;

namespace Faust.QoLChests;

public static class Utils
{
    public static Renderer[] GetRenderers(GameObject gameObject)
    {
        var renders = new List<Renderer> { gameObject.GetComponent<Renderer>() };
        renders.AddRange(gameObject.GetComponentsInChildren<Renderer>());
        var modelLocator = gameObject.GetComponent<ModelLocator>();
        if (modelLocator)
        {
            var modelTransform = modelLocator.modelTransform;
            if (modelTransform)
            {
                var renderer = modelTransform.GetComponent<Renderer>();
                var childRenderes = modelTransform.GetComponentsInChildren<Renderer>();
                renders.Add(renderer);
                renders.AddRange(childRenderes);
            }
        }

        return renders.Where(x => x).ToArray();
    }
}
