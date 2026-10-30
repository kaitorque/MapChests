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
                loaded = Whiten(loaded);
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

        /// <summary>
        /// Copies the sprite with every pixel turned gray (brightest channel kept), so the icon colour tints it cleanly.
        /// A few game icons, like the wrench, are coloured; tinting those directly gives muddy colours.
        /// </summary>
        private Sprite Whiten(Sprite sprite)
        {
            RenderTexture? target = null;
            var previous = RenderTexture.active;
            try
            {
                var source = sprite.texture;
                var rect = sprite.textureRect;
                int width = (int)rect.width, height = (int)rect.height;
                // the atlas trims transparent padding; the copy must keep the full sprite size or the image stretches
                int fullWidth = (int)sprite.rect.width, fullHeight = (int)sprite.rect.height;
                int offsetX = (int)sprite.textureRectOffset.x, offsetY = (int)sprite.textureRectOffset.y;

                target = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                Graphics.Blit(source, target);
                RenderTexture.active = target;
                var copy = new Texture2D(fullWidth, fullHeight, TextureFormat.RGBA32, false);
                copy.SetPixels32(new Color32[fullWidth * fullHeight]);
                copy.ReadPixels(new Rect(rect.x, rect.y, width, height), offsetX, offsetY);

                var pixels = copy.GetPixels32();
                for (int i = 0; i < pixels.Length; i++)
                {
                    var p = pixels[i];
                    var v = Math.Max(p.r, Math.Max(p.g, p.b));
                    pixels[i] = new Color32(v, v, v, p.a);
                }
                copy.SetPixels32(pixels);
                copy.Apply();

                var pivot = new Vector2(sprite.pivot.x / fullWidth, sprite.pivot.y / fullHeight);
                return Sprite.Create(copy, new Rect(0, 0, fullWidth, fullHeight), pivot, sprite.pixelsPerUnit);
            }
            catch (Exception e)
            {
                logger.LogWarning($"Could not make a gray copy of icon {sprite.name}, using it as is: {e.Message}");
                return sprite;
            }
            finally
            {
                RenderTexture.active = previous;
                if (target != null)
                    RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
