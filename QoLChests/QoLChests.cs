using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using Faust.Shared;
using Faust.Shared.Compatability;
using QoLChests;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Faust.QoLChests
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(
        RiskOfOptionsCompat.PluginGUID,
        BepInDependency.DependencyFlags.SoftDependency
    )]
    public class QoLChests : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = nameof(QoLChests);
        public const string PluginVersion = "1.1.10";

        //Configuration
        public static ConfigEntry<bool> HideEmptyChests,
            HideUsedShops,
            HighlightChests,
            HighlightShops,
            HighlightScrapper,
            HighlightDuplicator,
            HighlightDrones,
            HightlightTurrets,
            RemoveHighlightFromUsed,
            FadeInsteadOfHide,
            HighlightStealthedChests;

        public static ConfigEntry<float> HideTime;
        public static ConfigEntry<ConfigHighlightColor> HighlightColor;

        private List<GameObject> InteractablesToHighlight { get; set; } = [];

        private const string TakesEffectAfterSceneChangeOrReboot =
            "(Changes to this takes effect after scene change or run restart)";
        private const string TakesEffectGoingForward = "(Takes effect going forward)";

        protected void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            var stopwatch = Stopwatch.StartNew();
            Log.Init(Logger);

            //Configuration
            HideEmptyChests = Config.Bind(
                "Hide",
                "Chest",
                true,
                $"Hides empty chests after a few seconds {TakesEffectGoingForward}"
            );
            HideUsedShops = Config.Bind(
                "Hide",
                "Shops",
                true,
                $"Hides used shops after a few seconds {TakesEffectGoingForward}"
            );
            HideTime = Config.Bind(
                "Hide",
                "Time",
                1f,
                $"Time before stuff is hidden/faded {TakesEffectGoingForward}"
            );
            FadeInsteadOfHide = Config.Bind(
                "Hide",
                "Fade",
                false,
                $"Fade instead of hiding {TakesEffectGoingForward}"
            );

            RemoveHighlightFromUsed = Config.Bind(
                "Highlight",
                "RemoveWhenUsed",
                false,
                $"Remove highlight when used {TakesEffectGoingForward}"
            );
            HighlightChests = Config.Bind(
                "Highlight",
                "Chest",
                true,
                $"Highlight Chests (Chests, Barrels etc.) {TakesEffectAfterSceneChangeOrReboot}"
            );
            HighlightStealthedChests = Config.Bind(
                "Highlight",
                "Stealthed Chests",
                true,
                $"Highlight stealthed chests {TakesEffectAfterSceneChangeOrReboot}"
            );
            HighlightDuplicator = Config.Bind(
                "Highlight",
                "Duplicator",
                true,
                $"Highlight Duplicators {TakesEffectAfterSceneChangeOrReboot}"
            );
            HighlightScrapper = Config.Bind(
                "Highlight",
                "Scrapper",
                true,
                $"Highlight Scrappers {TakesEffectAfterSceneChangeOrReboot}"
            );
            HighlightShops = Config.Bind(
                "Highlight",
                "Shops",
                true,
                $"Highlight Shops {TakesEffectAfterSceneChangeOrReboot}"
            );
            HighlightDrones = Config.Bind(
                "Highlight",
                "Drones",
                true,
                $"Highlight Drones {TakesEffectAfterSceneChangeOrReboot}"
            );
            HightlightTurrets = Config.Bind(
                "Highlight",
                "Turrets",
                true,
                $"Highlight Turrets {TakesEffectAfterSceneChangeOrReboot}"
            );
            HighlightColor = Config.Bind(
                "Highlight",
                "Color",
                ConfigHighlightColor.Yellow,
                $"Highlight Color {TakesEffectAfterSceneChangeOrReboot}"
            );

            //Softdependencies
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.SetModDescription(
                    $"Hides chests when they are empty."
                        + $"{Environment.NewLine}Hides used shop terminals."
                        + $"{Environment.NewLine}Highlights chests and interactables."
                        + $"{Environment.NewLine}Do note that many of these options will not take effect until a scene change or restart."
                );
                RiskOfOptionsCompat.AddCheckboxOptions(
                    restartRequired: false,
                    HideEmptyChests,
                    HideUsedShops,
                    FadeInsteadOfHide,
                    RemoveHighlightFromUsed
                );
                RiskOfOptionsCompat.AddCheckboxOptions(
                    restartRequired: false,
                    HighlightChests,
                    HighlightDuplicator,
                    HighlightScrapper,
                    HighlightShops,
                    HighlightDrones,
                    HightlightTurrets,
                    HighlightStealthedChests
                );
                RiskOfOptionsCompat.AddSliderNumberOptions(
                    restartRequired: false,
                    0.1f,
                    5f,
                    HideTime
                );
                RiskOfOptionsCompat.AddDropdownOptions(false, HighlightColor);
            }

            On.EntityStates.Barrel.Opened.OnEnter += BarrelOpened;
            On.RoR2.MultiShopController.OnPurchase += MultiShopController_OnPurchase;
            On.RoR2.RouletteChestController.Opened.OnEnter += Roulette_Opened;
            On.RoR2.DelusionChestController.ResetChestForDelusion +=
                On_DelusionChestController_ResetChestForDelusion;

            RoR2Application.onLoad += ConfigureHighlights;

            stopwatch.Stop();

            Log.LogDebug($"{nameof(Awake)} took {stopwatch.ElapsedMilliseconds}ms");
        }

        private void ConfigureHighlights()
        {
            // Load highlightable resources
            AddResourcesToHighlights(HighlightChests.Value, Constants.ChestResourcesPaths);
            AddResourcesToHighlights(
                HighlightStealthedChests.Value,
                Constants.StealthedChestResourcePaths
            );
            AddResourcesToHighlights(HighlightShops.Value, Constants.ShopResourcePaths);
            AddResourcesToHighlights(HighlightScrapper.Value, Constants.ScrapperResourcePaths);
            AddResourcesToHighlights(HighlightDuplicator.Value, Constants.DuplicatorResourcesPaths);
            AddResourcesToHighlights(HighlightDrones.Value, Constants.DroneResourcesPaths);
            AddResourcesToHighlights(HightlightTurrets.Value, Constants.TurrentResourcePaths);
            AddResourcesToHighlights(
                HightlightTurrets.Value || HighlightDrones.Value,
                Constants.ArtifactOfDevotionResourcePaths
            );
            // Add highlights to tracked resources
            AddHighlights();
        }

        private void BarrelOpened(
            On.EntityStates.Barrel.Opened.orig_OnEnter orig,
            EntityStates.Barrel.Opened self
        )
        {
            orig.Invoke(self);
            if (HideEmptyChests.Value)
            {
                HideOrFade(
                    self.outer.gameObject,
                    self.outer.commonComponents.modelLocator.modelTransform.gameObject
                );
            }
            else if (RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.outer.gameObject);
            }
        }

        private void On_DelusionChestController_ResetChestForDelusion(
            On.RoR2.DelusionChestController.orig_ResetChestForDelusion orig,
            DelusionChestController self
        )
        {
            orig.Invoke(self);
            var renderers = self
                .gameObject.GetComponent<ModelLocator>()
                .modelTransform.GetComponentsInChildren<Renderer>();

            foreach (var rend in renderers)
            {
                rend.enabled = true;
            }
            if (HighlightChests.Value)
            {
                HighlightValues(self.gameObject);
            }
        }

        private void MultiShopController_OnPurchase(
            On.RoR2.MultiShopController.orig_OnPurchase orig,
            MultiShopController self,
            Interactor interactor,
            PurchaseInteraction purchaseInteraction
        )
        {
            orig.Invoke(self, interactor, purchaseInteraction);

            if (self.Networkavailable)
                return;

            var terminalObjects = self
                .terminalGameObjects.Select(x => x)
                .Concat([self.gameObject])
                .ToArray();

            if (HideUsedShops.Value)
            {
                HideOrFade(terminalObjects);
            }
            else if (RemoveHighlightFromUsed.Value)
            {
                foreach (var terminalObj in terminalObjects)
                {
                    RemoveHighlight(terminalObj);
                }
            }
        }

        private void Roulette_Opened(
            On.RoR2.RouletteChestController.Opened.orig_OnEnter orig,
            EntityStates.EntityState self
        )
        {
            orig.Invoke(self);
            if (HideEmptyChests.Value)
            {
                HideOrFade(self.outer.gameObject);
            }
            else if (RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.outer.gameObject);
            }
        }

        private void HideOrFade(params GameObject[] gameObjects)
        {
            if (FadeInsteadOfHide.Value)
            {
                foreach (var obj in gameObjects)
                {
                    obj.AddComponent<FadeWithDelay>()
                        .SetDelay(HideTime.Value)
                        .DisableRendererAfterDelay();
                }
                return;
            }

            foreach (var obj in gameObjects)
            {
                obj.AddComponent<HideWithDelay>()
                    .SetDelay(HideTime.Value)
                    .DisableRendererAfterDelay();
            }
        }

        private void AddResourcesToHighlights(bool isEnabled, HighlightableResource[] resources)
        {
            if (isEnabled)
            {
                foreach (var resource in resources)
                {
                    var addressable = Addressables
                        .LoadAssetAsync<GameObject>(resource.ResourcePath)
                        .WaitForCompletion();
                    if (addressable)
                        InteractablesToHighlight.Add(addressable);
                }
            }
        }

        private static void RemoveHighlight(GameObject self)
        {
            var highlight = self.GetComponent<Highlight>();
            if (highlight)
            {
                highlight.isOn = false;
                highlight.enabled = false;
            }
        }

        private void AddHighlights()
        {
            if (InteractablesToHighlight.Any())
            {
                foreach (var gameObject in InteractablesToHighlight)
                {
                    HighlightValues(gameObject);
                }
            }
        }

        private void HighlightValues(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            if (RemoveHighlightFromUsed.Value)
            {
                var multiShop = gameObject.GetComponent<MultiShopController>();
                if (multiShop && !multiShop.Networkavailable)
                {
                    return;
                }

                var barrel = gameObject.GetComponent<BarrelInteraction>();
                if (barrel && barrel.Networkopened)
                {
                    return;
                }

                var rouletteChestController = gameObject.GetComponent<RouletteChestController>();
                if (rouletteChestController && !rouletteChestController.enabled)
                {
                    return;
                }
            }

            var component = gameObject.GetComponent<Highlight>();
            if (component)
            {
                component.isOn = true;
                component.enabled = true;
                component.highlightColor = (Highlight.HighlightColor)(int)HighlightColor.Value;
            }
        }
    }
}
