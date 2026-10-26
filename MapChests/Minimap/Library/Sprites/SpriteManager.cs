using MapChests.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;

#nullable enable
namespace MiniMapLibrary
{
    public sealed class SpriteManager : IDisposable, ISpriteManager
    {
        private readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();

        private readonly ILogger logger;

        public SpriteManager(ILogger logger)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            SpriteCache.Clear();
        }

        public Sprite? GetSprite(MapCategory category)
        {
            string? path = Settings.GetSetting(category).Icon.Value;

            return GetOrCache(string.IsNullOrWhiteSpace(path) ? Settings.Icons.Default : path!);
        }

        public Sprite? GetOrCache(string Path)
        {
            if (SpriteCache.ContainsKey(Path))
            {
                return SpriteCache[Path];
            }

            Sprite? loaded = RoR2.LegacyResourcesAPI.Load<Sprite>(Path);

            if (loaded != null)
            {
                SpriteCache.Add(Path, loaded);
            }
            else 
            {
                loaded = RoR2.LegacyResourcesAPI.Load<Sprite>(Settings.Icons.Default);

                if (loaded is null)
                {
                    logger.LogError($"Attempted to use default icon for non-existen texture at {Path} but default icon path of {Settings.Icons.Default} also failed to load from the streaming assets path.");
                    return null;
                }

                logger.LogWarning($"Attempted to load icon texture at streaming asset path: {Path}, but it was not found, using default [?] instead.");

                SpriteCache.Add(Path, loaded);
            }

            return loaded;
        }
    }
}
