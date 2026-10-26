using MapChests.Categories;
using MiniMapLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MiniMapLibrary.Scanner;

namespace MiniMapLibrary.Scanner
{
    public class TrackedObject<T> : ITrackedObject
    {
        public GameObject gameObject { get; set; }

        public RectTransform MinimapTransform { get; set; }

        public bool DynamicObject { get; set; } = false;

        public T BackingObject { get; set; }

        public Image MinimapImage
        {
            get
            {
                if (_MinimapImage is null && MinimapTransform != null)
                {
                    _MinimapImage = MinimapTransform.GetComponent<Image>();
                }

                return _MinimapImage;
            }
            set => _MinimapImage = value;
        }

        private Image _MinimapImage;

        public MapCategory Category { get; set; }

        public Func<T, bool> ActiveChecker { get; set; }

        public bool Active { get; private set; } = true;

        private bool PreviousActive = true;

        public TrackedObject(MapCategory category, GameObject gameObject, RectTransform minimapTransform)
        {
            this.gameObject = gameObject;
            MinimapTransform = minimapTransform;
            Category = category;
        }

        public void Destroy()
        {
            try
            {
                GameObject.Destroy(MinimapTransform.gameObject);
            }
            catch (Exception)
            {
                // intentionally eat the exception while trying to destroy the gameobject
                // if other mods or the scene destroys the UI we dont want to throw exceptions
                // this is expected
            }
        }

        public void CheckActive()
        {
            if (BackingObject is null)
            {
                Active = false;

                UpdateColor();

                return;
            }

            try
            {
                Active = ActiveChecker?.Invoke(BackingObject) ?? false;
            }
            catch (Exception)
            {
                // intentionally eat the exception while trying to destroy the gameobject
                // if other mods or the scene destroys the UI we dont want to throw exceptions
                // this is expected
                Active = false;
            }

            UpdateColor();
        }

        private void UpdateColor()
        {
            if (PreviousActive != Active)
            {
                PreviousActive = Active;

                MinimapImage.color = Settings.GetColor(Category, Active);
                MinimapTransform.gameObject.SetActive(Active || !Settings.GetSetting(Category).HideUsedIcon);
            }
        }
    }
}
