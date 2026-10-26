using BepInEx;
using RoR2;
using UnityEngine;
using MiniMapLibrary;
using System.Collections.Generic;
using System.Linq;
using System;
using BepInEx.Configuration;
using BepInEx.Logging;
using MiniMapMod.Adapters;
using MiniMapLibrary.Config;
using MiniMapLibrary.Scanner;
using MapChests.Categories;

namespace MiniMapMod
{
    public class MinimapController : MonoBehaviour
    {
        private ISpriteManager SpriteManager;

        private readonly List<ITrackedObject> TrackedObjects = new();

        private Minimap Minimap;

        private readonly Range3D TrackedDimensions = new();

        private bool Enable = true;

        private bool ScannedStaticObjects = false;

        private IConfig config;

        private MiniMapLibrary.ILogger logger;

        private ITrackedObjectScanner[] staticScanners;
        private ITrackedObjectScanner[] dynamicScanners;

        private readonly Timer dynamicScanTimer = new(5.0f);
        private readonly Timer cooldownTimer = new(2.0f, false) { Value = -1.0f };
        private CameraRigController cameraRig;

        public void Init(ConfigFile configFile, ManualLogSource logSource)
        {
            // wrap the bepinex logger with an adapter so 
            // we can pass it to the business layer
            logger = new Log(logSource);

            SpriteManager = new SpriteManager(logger);

            // create the minimap controller
            Minimap = new(logger);

            // SETUP CONFIG

            // wrap bepinex config so we can pass it to business layer
            config = new ConfigAdapter(configFile);

            Settings.LoadApplicationSettings(logger, config);

            // bind options
            InteractableKind[] kinds = Enum.GetValues(typeof(InteractableKind)).Cast<InteractableKind>().Where(x => x != InteractableKind.none && x != InteractableKind.All).ToArray();

            foreach (var item in kinds)
            {
                Settings.LoadConfigEntries(item, config);
            }

            logger.LogInfo("Creating scene scan hooks");

            // fill the scanner arrays
            CreateStaticScanners();

            CreateDynamicScanners();

            // hook events so the minimaps updates
            // scan scene should NEVER throw exceptions
            // doing so prevents all other subscribing events to not fire (after the exception)

            // this will re-scan the scene every time any npc, player dies
            GlobalEventManager.onCharacterDeathGlobal += (x) => ScanScene();

            // this will re-scan when the player uses anything like a chest
            // or the landing pod
            GlobalEventManager.OnInteractionsGlobal += (x, y, z) => ScanScene();

            // update the minimap automatically every N seconds regardless of deaths etc
            dynamicScanTimer.OnFinished += (x) => ScanScene();
        }

        //The Update() method is run on every frame of the game.
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(Settings.MinimapKey))
            {
                Enable = !Enable;

                if (Enable == false)
                {
                    logger.LogInfo("Resetting minimap");
                    Reset();
                    return;
                }
            }

            cooldownTimer.Update(Time.deltaTime);
            dynamicScanTimer.Update(Time.deltaTime);

            if (Enable)
            {
                // the main camera becomes null when the scene ends on death or quits
                if (Camera.main == null)
                {
                    logger.LogDebug("Main camera was null, resetting minimap");
                    Reset();
                    return;
                }

                if (Minimap.Created)
                {
                    try
                    {
                        Minimap.SetRotation(Camera.main.transform.rotation);

                        UpdateIconPositions();

                        if (Input.GetKeyDown(Settings.MinimapIncreaseScaleKey))
                        {
                            Minimap.Container.transform.localScale *= 1.1f;
                        }
                        else
                            if (Input.GetKeyDown(Settings.MinimapDecreaseScaleKey))
                            {
                                Minimap.Container.transform.localScale *= 0.90f;
                            }
                    }
                    catch (NullReferenceException)
                    {
                        // we'll encounter null references when other mods or the game itself
                        // destroys entities we are tracking at runtime
                        logger.LogDebug($"{nameof(NullReferenceException)} was encountered while updating positions, reseting minimap");
                        Reset();
                    }
                }
                else
                {
                    if (TryCreateMinimap())
                    {
                        TrackedDimensions.Clear();

                        ScanScene();
                    }
                }
            }
        }

        private void UpdateIconPositions()
        {
            // only perform this calculation once per frame
            Vector2 cameraPositionMinimap = GetPlayerPosition().ToMinimapPosition(TrackedDimensions);

            for (int i = 0; i < TrackedObjects.Count; i++)
            {
                ITrackedObject item = TrackedObjects[i];

                // if we dont have a reference to the gameobject any more, remove it and continue
                if (item.gameObject == null)
                {
                    TrackedObjects.RemoveAt(i);

                    // if we still have a icon for the now discarded item destroy it
                    if (item.MinimapTransform != null)
                    {
                        GameObject.Destroy(item.MinimapTransform.gameObject);
                    }

                    continue;
                }

                // convert the world positions to minimap positions
                // remember the minimap is calculated on a scale from 0d to 1d where 0d is the least most coord of any interactible and 1d is the largest coord of any interactible

                Vector2 itemMinimapPosition = item.gameObject.transform.position.ToMinimapPosition(TrackedDimensions) - cameraPositionMinimap;

                // there exists no icon when .MinimapTransform is null
                if (item.MinimapTransform == null)
                {
                    // create one
                    item.MinimapTransform = Minimap.CreateIcon(item.InteractableType, itemMinimapPosition, this.SpriteManager);
                }
                else
                {
                    // since it was already created update the position
                    item.MinimapTransform.localPosition = itemMinimapPosition;

                    // becuase we don't want the icons to spin WITH the minimap set their rotation
                    // every frame so they're always facing right-side up
                    // (they inherit their position from the minimap)
                    item.MinimapTransform.rotation = Quaternion.identity;
                }

                // check to see if its active and whether to change its color
                // check the position to see if we should show arrow
                item.CheckActive();
            }
        }

        private bool TryCreateMinimap()
        {
            GameObject objectivePanel = GameObject.Find("ObjectivePanel");

            if (objectivePanel == null || this.SpriteManager == null)
            {
                Minimap.Destroy();
                return false;
            }

            logger.LogInfo("Creating Minimap object");

            Minimap.CreateMinimap(this.SpriteManager, objectivePanel.gameObject);

            Minimap.Container.transform.localScale = Vector3.one * Settings.MinimapScale;

            logger.LogInfo("Finished creating Minimap");

            return true;
        }

        private void Reset()
        {
            logger.LogDebug($"Clearing {nameof(TrackedObjects)}");
            TrackedObjects.Clear();

            logger.LogDebug($"Clearing {nameof(TrackedDimensions)}");
            TrackedDimensions.Clear();

            logger.LogDebug($"Destroying {nameof(Minimap)}");
            Minimap.Destroy();

            dynamicScanTimer.Reset();
            cooldownTimer.Reset();

            // mark the scene as scannable again so we scan for chests etc..
            ScannedStaticObjects = false;

            cameraRig = null;
        }

        private void ScanScene()
        {
            // don't scan if the minimap isn't enabled
            if (Enable == false)
            {
                return;
            }
            // when other mods hook into the various global events
            // and this method throws exceptions, the entire event will throw and fail to invoke their methods
            // as a result, this method should never throw an exception and should output meaningful
            // errors
            try
            {
                if (cooldownTimer.Started is false)
                {
                    cooldownTimer.Start();
                }
                else if (cooldownTimer.Expired is false)
                {
                    return;
                }

                cooldownTimer.Reset();
                cooldownTimer.Start();

                logger.LogDebug("Scanning Scene");

                logger.LogDebug("Clearing dynamically tracked objects");
                ClearDynamicTrackedObjects();

                logger.LogDebug("Scanning static types");
                ScanStaticTypes();

                logger.LogDebug("Scanning dynamic types");
                ScanDynamicTypes();
            }
            catch (Exception e)
            {
                logger.LogException(e, $"Fatal exception within minimap");

                // intentionally consume the error, again we never want to throw an exception in
                // a global event delegate (unless we're the last event, but that is never garunteed)
            }
        }

        private void CreateStaticScanners()
        {
            staticScanners = new ITrackedObjectScanner[] {
                new CategoryScanner(false, FindTaggedInteractables, TrackedDimensions, SpriteManager, () => GetPlayerPosition().y),
            };
        }

        private static IEnumerable<GameObject> FindTaggedInteractables()
        {
            CategoryClassifier.TagUntagged();

            // generic interactions (portals) can spawn mid-stage, so the dynamic scan tracks them
            return GameObject.FindObjectsOfType<CategoryMarker>()
                .Select(marker => marker.gameObject)
                .Where(gameObject => !gameObject.GetComponent<GenericInteraction>());
        }

        private ITrackedObjectScanner CreateGenericInteractionScanner()
        {
            return new CategoryScanner(true,
                () => GameObject.FindObjectsOfType<GenericInteraction>().Select(interaction => interaction.gameObject),
                TrackedDimensions, SpriteManager, () => GetPlayerPosition().y);
        }

        private ITrackedObjectScanner CreateAliveEntityScanner()
        {
            bool EnemyMonsterSelector(TeamComponent team) => team?.teamIndex == TeamIndex.Monster;

            bool EnemyLunarSelector(TeamComponent team) => team?.teamIndex == TeamIndex.Lunar;

            bool EnemyVoidSelector(TeamComponent team) => team?.teamIndex == TeamIndex.Void;

            bool NeutralSelector(TeamComponent team) => team?.teamIndex == TeamIndex.Neutral;

            bool MinionSelector(TeamComponent team)
            {
                var isAlly = team?.teamIndex == TeamIndex.Player;
                var isPlayer = team?.GetComponent<CharacterBody>()?.isPlayerControlled;
                if (isPlayer is null)
                {
                    return isAlly;
                }
                return isAlly && (isPlayer == false);
            }

            bool PlayerSelector(TeamComponent team)
            {
                var isOwner = team?.GetComponent<NetworkStateMachine>()?.hasAuthority;
                var isPlayer = team?.GetComponent<CharacterBody>()?.isPlayerControlled;
                if (isOwner is null || isPlayer is null)
                {
                    return false;
                }
                return (isOwner == false) && (isPlayer == true);
            }

            GameObject DefaultConverter<T>(T value) where T : MonoBehaviour => value?.gameObject;

            return new MultiKindScanner<TeamComponent>(true,
                new MonoBehaviorScanner<TeamComponent>(logger), new MonoBehaviourSorter<TeamComponent>(
                    new ISorter<TeamComponent>[]
                    {
                        new DefaultSorter<TeamComponent>(InteractableKind.EnemyMonster, DefaultConverter, EnemyMonsterSelector, x => true),
                        new DefaultSorter<TeamComponent>(InteractableKind.EnemyLunar, DefaultConverter, EnemyLunarSelector, x => true),
                        new DefaultSorter<TeamComponent>(InteractableKind.EnemyVoid, DefaultConverter, EnemyVoidSelector, x => true),
                        new DefaultSorter<TeamComponent>(InteractableKind.Minion, DefaultConverter, MinionSelector, x => true),
                        new DefaultSorter<TeamComponent>(InteractableKind.Player, DefaultConverter, PlayerSelector, x => true),
                        new DefaultSorter<TeamComponent>(InteractableKind.Neutral, DefaultConverter, NeutralSelector, x => x?.gameObject.activeSelf ?? true),
                    }
                ), TrackedDimensions, SpriteManager, () => GetPlayerPosition().y);
        }

        private Vector3 GetPlayerPosition()
        {
            if (cameraRig == null)
            {
                cameraRig = Camera.main.transform.parent.GetComponent<CameraRigController>();

                if (cameraRig == null)
                {
                    logger.LogError("Failed to retrieve camera rig in scene to retrieve camera position");
                }
            }

            return cameraRig.target?.transform?.position ?? Camera.main.transform.position;
        }

        private void CreateDynamicScanners()
        {
            dynamicScanners = new ITrackedObjectScanner[]
            {
                CreateAliveEntityScanner(),
                new SingleKindScanner<GenericPickupController>(
                    kind: InteractableKind.Item,
                    dynamic: true,
                    scanner: new MonoBehaviorScanner<GenericPickupController>(logger),
                    range: TrackedDimensions,
                    spriteManager: SpriteManager,
                    playerHeightRetriever: () => GetPlayerPosition().y,
                    converter: x => x.gameObject,
                    activeChecker: x => true
                ),
                CreateGenericInteractionScanner()
            };
        }

        private void ScanStaticTypes()
        {
            // if we have alreadys scanned don't scan again until we die or the scene changes (this method has sever performance implications)
            if (ScannedStaticObjects)
            {
                return;
            }

            for (int i = 0; i < staticScanners.Length; i++)
            {
                staticScanners[i].ScanScene(TrackedObjects);
            }

            ScannedStaticObjects = true;
        }

        private void ScanDynamicTypes()
        {
            for (int i = 0; i < dynamicScanners.Length; i++)
            {
                dynamicScanners[i].ScanScene(TrackedObjects);
            }
        }

        private void ClearDynamicTrackedObjects()
        {
            if (ScannedStaticObjects is false)
            {
                return;
            }

            for (int i = 0; i < TrackedObjects.Count; i++)
            {
                var obj = TrackedObjects[i];

                if (obj.DynamicObject)
                {
                    obj.Destroy();
                    TrackedObjects.RemoveAt(i);
                }
            }
        }
    }
}
