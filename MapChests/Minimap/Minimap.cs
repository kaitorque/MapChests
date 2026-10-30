using MapChests.Categories;
using MiniMapMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MiniMapLibrary
{
    public partial class Minimap
    {
        private readonly MiniMapLibrary.ILogger Logger;

        public Minimap(ILogger logger)
        {
            Logger = logger;
        }
// ignore naming violation, name choses to match unity conventions
#pragma warning disable IDE1006
        public static GameObject gameObject { get; private set; }
#pragma warning restore IDE1006

        public static GameObject Container { get; private set; }

        public Transform PlayerIcon { get; private set; }

        private RectTransform ContainerTransform;

        public bool Created { get; private set; } = false;

        public void CreateMinimap(ISpriteManager spriteManager, GameObject Parent)
        {
            if (Created)
            {
                return;
            }

            // set created flag
            Created = true;

            gameObject = CreateMask();

            SetParent(gameObject, Parent);

            // make sure it's the first thing within the objective box
            gameObject.transform.SetAsFirstSibling();

            CreateIconContainer();

            CreatePlayerIcon(spriteManager);
        }

        public void Destroy()
        {
            if (Created == false)
            {
                return;
            }

            Logger.LogDebug("Destroying minimap gameobject");
            GameObject.Destroy(gameObject);

            gameObject = null;
            Container = null;
            ContainerTransform = null;
            PlayerIcon = null;

            Created = false;
        }

        public void DestroyIcons()
        {
            foreach (Transform child in Container.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void SetRotation(Quaternion rotation)
        {
            var euler = rotation.eulerAngles;

            ContainerTransform.localRotation = Quaternion.Euler(0, 0, euler.y);
        }

        public RectTransform CreateIcon(MapCategory category, Vector3 minimapPosition, ISpriteManager spriteManager)
        {
            GameObject icon = CreateIcon(category, spriteManager.GetSprite(category));

            var transform = icon.GetComponent<RectTransform>();

            transform.localPosition = minimapPosition;

            Helpers.Transforms.SetParent(transform, ContainerTransform);

            return transform;
        }

        private void CreateIconContainer()
        {
            GameObject container = Create(obj =>
            {
                obj.name = "icon_container";

                ContainerTransform = obj.AddComponent<RectTransform>();

                ContainerTransform.sizeDelta = new(Settings.MinimapSize.Width, Settings.MinimapSize.Height);
            });

            SetParent(container, gameObject);

            Container = container;
        }

        private void CreatePlayerIcon(ISpriteManager spriteManager)
        {
            GameObject playerIcon = CreateIcon(MapCategory.Player, spriteManager.GetSprite(MapCategory.Player));

            SetParent(playerIcon, gameObject);

            PlayerIcon = playerIcon.transform;
        }

        private GameObject CreateIcon(MapCategory category, Sprite iconTexture)
        {
            var settings = Settings.GetSetting(category);

            GameObject icon = Helpers.Sprites.CreateIcon(iconTexture, settings.IconWidth.Value, settings.IconHeight.Value, settings.MinimapColor.Value);

            icon.name = category.ToString();

            return icon;
        }

        private GameObject CreateMask()
        {
            return Create(mask =>
            {
                mask.name = "Minimap";

                RectTransform newTransform = mask.AddComponent<RectTransform>();

                newTransform.sizeDelta = new Vector2(Settings.ViewfinderSize.Width, Settings.ViewfinderSize.Height);

                mask.AddComponent<CanvasRenderer>();

                mask.AddComponent<Image>();

                var maskComponent = mask.AddComponent<Mask>();

                maskComponent.showMaskGraphic = false;

                LayoutElement element = mask.AddComponent<LayoutElement>();

                element.minHeight = Settings.MinimapSize.Height;
                element.flexibleHeight = 0;
            });
        }

        private GameObject SetParent(GameObject child, GameObject parent)
        {
            Helpers.Transforms.SetParent(child.transform, parent.transform);

            return child;
        }

        private GameObject Create(Action<GameObject> Expression)
        {
            GameObject newObj = new();

            Expression(newObj);

            return newObj;
        }
    }
}
