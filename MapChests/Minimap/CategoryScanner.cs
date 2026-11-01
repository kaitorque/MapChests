using System;
using System.Collections.Generic;
using Faust.QoLChests.Compatability;
using Faust.QoLChests.Components;
using MapChests.Categories;
using MiniMapLibrary;
using MiniMapLibrary.Scanner;
using RoR2;
using UnityEngine;

namespace MiniMapMod
{
    /// <summary>
    /// Tracks objects by their <see cref="MapCategory"/>, using each category's minimap icon.
    /// </summary>
    public class CategoryScanner : ITrackedObjectScanner
    {
        private readonly bool dynamic;
        private readonly Func<IEnumerable<GameObject>> source;
        private readonly Range3D range;
        private readonly ISpriteManager spriteManager;
        private readonly Func<float> playerHeightRetriever;

        public CategoryScanner(bool dynamic, Func<IEnumerable<GameObject>> source, Range3D range, ISpriteManager spriteManager, Func<float> playerHeightRetriever)
        {
            this.dynamic = dynamic;
            this.source = source;
            this.range = range;
            this.spriteManager = spriteManager;
            this.playerHeightRetriever = playerHeightRetriever;
        }

        public void ScanScene(IList<ITrackedObject> list)
        {
            foreach (var gameObject in source())
                Track(gameObject, list);
        }

        /// <summary>Adds one object to the list if it is visible, classified and its category is shown on the minimap.</summary>
        public void Track(GameObject gameObject, IList<ITrackedObject> list)
        {
            if (!gameObject || !ParallelCompat.IsVisible(gameObject) || !CategoryClassifier.TryClassify(gameObject, out var category))
                return;

            if (!Settings.GetSetting(category).Minimap.Value)
                return;

            list.Add(new ElevationTrackedObject(new TrackedObject<GameObject>(category, gameObject, null)
            {
                BackingObject = gameObject,
                ActiveChecker = CreateActiveChecker(gameObject),
                DynamicObject = dynamic
            }, spriteManager, playerHeightRetriever));

            range.CheckValue(gameObject.transform.position);
        }

        private static Func<GameObject, bool> CreateActiveChecker(GameObject gameObject)
        {
            var interactionChecker = CreateInteractionChecker(gameObject);
            return x => !x.GetComponent<InteractableUsed>() && interactionChecker(x);
        }

        private static Func<GameObject, bool> CreateInteractionChecker(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<TeleporterInteraction>(out var teleporter))
                return _ => teleporter && teleporter.activationState != TeleporterInteraction.ActivationState.Charged;

            if (gameObject.TryGetComponent<BarrelInteraction>(out var barrel))
                return _ => barrel && !barrel.Networkopened;

            if (gameObject.TryGetComponent<PurchaseInteraction>(out var purchase))
                return _ => purchase && purchase.available;

            if (gameObject.TryGetComponent<GenericInteraction>(out var generic))
                return _ => generic && generic.isActiveAndEnabled;

            return _ => true;
        }
    }
}
