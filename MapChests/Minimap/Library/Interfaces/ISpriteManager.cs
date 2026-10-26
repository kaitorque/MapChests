using MapChests.Categories;
using System;
using UnityEngine;

namespace MiniMapLibrary
{
    /// <summary>
    /// Represents some object that should provide sprites to be displayed on the minimap
    /// </summary>
    public interface ISpriteManager : IDisposable
    {
        Sprite GetOrCache(string Path);

        /// <summary>
        /// Returns the sprite configured to represent the provided <see cref="MapCategory"/> on the minimap
        /// </summary>
        Sprite GetSprite(MapCategory category);
    }
}