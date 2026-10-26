using UnityEngine;

namespace MapChests.Categories;

public class CategoryMarker : MonoBehaviour
{
    public MapCategory Category;

    public CategoryMarker SetCategory(MapCategory category)
    {
        Category = category;
        return this;
    }

    /// <summary>Adds a marker unless the object already has one.</summary>
    public static void Tag(GameObject gameObject, MapCategory category)
    {
        if (!gameObject.GetComponent<CategoryMarker>())
            gameObject.AddComponent<CategoryMarker>().SetCategory(category);
    }
}
