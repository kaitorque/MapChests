using Faust.QoLChests.Configs;
using UnityEngine;

namespace Faust.QoLChests.Components;

public class InteractableHighlightCategoryMarker : MonoBehaviour
{
    public InteractableCategory Category;

    public InteractableHighlightCategoryMarker SetCategory(InteractableCategory category)
    {
        Category = category;
        return this;
    }
}
