using System;
using System.Collections.Generic;
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
        public const string PluginVersion = "1.1.9";

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
            // FadeInsteadOfHide,
            HighlightStealthedChests;

        // public static ConfigEntry<float> HideTime;
        public static ConfigEntry<ConfigHighlightColor> HighlightColor;

        // Resource Paths
        public static string[] ChestResourcesPaths = new[]
        {
            "prefabs/networkedobjects/chest/Barrel1",
            "prefabs/networkedobjects/chest/CasinoChest",
            "prefabs/networkedobjects/chest/CategoryChestDamage",
            "prefabs/networkedobjects/chest/CategoryChestHealing",
            "prefabs/networkedobjects/chest/CategoryChestUtility",
            "prefabs/networkedobjects/chest/Chest1",
            "prefabs/networkedobjects/chest/Chest2",
            "prefabs/networkedobjects/chest/EquipmentBarrel",
            "prefabs/networkedobjects/chest/GoldChest",
            "prefabs/networkedobjects/chest/LunarChest",
        };

        public static string[] StealthedChestResourcePaths = new[]
        {
            "prefabs/networkedobjects/chest/Chest1StealthedVariant",
        };

        public static string[] ShopResourcePaths = new[]
        {
            "prefabs/networkedobjects/chest/TripleShop",
            "prefabs/networkedobjects/chest/TripleShopEquipment",
            "prefabs/networkedobjects/chest/TripleShopLarge",
            "prefabs/networkedobjects/chest/LunarShopTerminal",
            "prefabs/networkedobjects/chest/MultiShopEquipmentTerminal",
            "prefabs/networkedobjects/chest/MultiShopLargeTerminal",
            "prefabs/networkedobjects/chest/MultiShopTerminal",
        };

        public static string[] ScrapperResourcePaths = new[]
        {
            "prefabs/networkedobjects/chest/Scrapper",
        };

        public static string[] DuplicatorResourcesPaths = new[]
        {
            "prefabs/networkedobjects/chest/Duplicator",
            "prefabs/networkedobjects/chest/DuplicatorLarge",
            "prefabs/networkedobjects/chest/DuplicatorMilitary",
            "prefabs/networkedobjects/chest/DuplicatorWild",
        };

        public static string[] DroneResourcesPaths = new[]
        {
            "prefabs/networkedobjects/brokendrones/Drone1Broken",
            "prefabs/networkedobjects/brokendrones/Drone2Broken",
            "prefabs/networkedobjects/brokendrones/EmergencyDroneBroken",
            "prefabs/networkedobjects/brokendrones/EquipmentDroneBroken",
            "prefabs/networkedobjects/brokendrones/FlameDroneBroken",
            "prefabs/networkedobjects/brokendrones/MegaDroneBroken",
            "prefabs/networkedobjects/brokendrones/MissileDroneBroken",
        };

        public static string[] TurrentResourcePaths = new[]
        {
            "prefabs/networkedobjects/brokendrones/Turret1Broken",
        };

        public static string[] ArtifactOfDevotionResourcePaths = new[]
        {
            "RoR2/CU8/LemurianEgg/LemurianEgg.prefab"
        };

        public static string[] DLCChests =
        [
            "RoR2/DLC1/CategoryChest2/CategoryChest2Damage Variant.prefab",
            "RoR2/DLC1/CategoryChest2/CategoryChest2Healing Variant.prefab",
            "RoR2/DLC1/CategoryChest2/CategoryChest2Utility Variant.prefab",
            "RoR2/DLC1/VoidChest/VoidChest.prefab"
        ];

        private List<GameObject> Highlight { get; set; } = new List<GameObject>();

        //The Awake() method is run at the very start when the game is initialized.
        protected void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            //Configuration
            HideEmptyChests = Config.Bind(
                "Hide",
                "Chest",
                true,
                "Hides empty chests after a few seconds"
            );
            HideUsedShops = Config.Bind(
                "Hide",
                "Shops",
                true,
                "Hides used shops after a few seconds"
            );
            // HideTime = Config.Bind("Hide", "Time", 1f, "Time before stuff is hidden/faded");
            // FadeInsteadOfHide = Config.Bind("Hide", "Fade", false, "Fade instead of hiding");

            RemoveHighlightFromUsed = Config.Bind(
                "Highlight",
                "RemoveWhenUsed",
                false,
                "Remove highlight when used"
            );
            HighlightChests = Config.Bind(
                "Highlight",
                "Chest",
                true,
                "Highlight Chests (Chests, Barrels etc.)"
            );
            HighlightStealthedChests = Config.Bind(
                "Highlight",
                "Stealthed Chests",
                true,
                "Highlight stealthed chests"
            );
            HighlightDuplicator = Config.Bind(
                "Highlight",
                "Duplicator",
                true,
                "Highlight Duplicators"
            );
            HighlightScrapper = Config.Bind("Highlight", "Scrapper", true, "Highlight Scrappers");
            HighlightShops = Config.Bind("Highlight", "Shops", true, "Highlight Shops");
            HighlightDrones = Config.Bind("Highlight", "Drones", true, "Highlight Drones");
            HightlightTurrets = Config.Bind("Highlight", "Turrets", true, "Highlight Turrets");
            HighlightColor = Config.Bind(
                "Highlight",
                "Color",
                ConfigHighlightColor.Yellow,
                string.Join(", ", Enum.GetNames(typeof(ConfigHighlightColor)))
            );

            //Softdependencies
            if (RiskOfOptionsCompat.IsInstalled)
            {
                RiskOfOptionsCompat.SetModDescription(
                    $"Hides chests when they are empty.{Environment.NewLine}Hides used shop terminals.{Environment.NewLine}Highlights chests and interactables."
                );
                RiskOfOptionsCompat.AddCheckboxOptions(
                    restartRequired: false,
                    HideEmptyChests,
                    HideUsedShops,
                    // FadeInsteadOfHide,
                    RemoveHighlightFromUsed,
                    HighlightChests,
                    HighlightDuplicator,
                    HighlightScrapper,
                    HighlightShops,
                    HighlightDrones,
                    HightlightTurrets,
                    HighlightStealthedChests
                );
                // RiskOfOptionsCompat.AddSliderNumberOptions(
                //     restartRequired: false,
                //     0.1f,
                //     5f,
                //     HideTime
                // );
                RiskOfOptionsCompat.AddDropdownOptions(false, HighlightColor);
            }

            On.EntityStates.Barrel.Opened.OnEnter += BarrelOpened;
            On.RoR2.MultiShopController.OnPurchase += MultiShopController_OnPurchase;
            On.RoR2.RouletteChestController.Opened.OnEnter += Roulette_Opened;
            On.RoR2.DelusionChestController.ResetChestForDelusion +=
                On_DelusionChestController_ResetChestForDelusion;

            // Highlight Resources
            AddResourcesToHighlights(HighlightChests.Value, ChestResourcesPaths);
            AddResourcesToHighlights(HighlightStealthedChests.Value, StealthedChestResourcePaths);
            AddResourcesToHighlights(HighlightShops.Value, ShopResourcePaths);
            AddResourcesToHighlights(HighlightScrapper.Value, ScrapperResourcePaths);
            AddResourcesToHighlights(HighlightDuplicator.Value, DuplicatorResourcesPaths);
            AddResourcesToHighlights(HighlightDrones.Value, DroneResourcesPaths);
            AddResourcesToHighlights(HightlightTurrets.Value, TurrentResourcePaths);
            AddResourcesToHighlights(HightlightTurrets.Value || HighlightDrones.Value, ArtifactOfDevotionResourcePaths, false);
            AddResourcesToHighlights(HighlightChests.Value, DLCChests, false);

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        protected void Update()
        {
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

            if (RemoveHighlightFromUsed.Value)
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

            if (HideUsedShops.Value)
            {
                var terminalObjects = self
                    .terminalGameObjects.Select(x => x)
                    .Concat(new[] { self.gameObject })
                    .ToArray();
                HideOrFade(self.gameObject, terminalObjects);
            }

            if (RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.gameObject);
                foreach (var terminal in self.terminalGameObjects)
                {
                    RemoveHighlight(terminal);
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
                HideOrFade(self.outer.gameObject, self.outer.gameObject);
            }

            if (RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.outer.gameObject);
            }
        }

        private static void HideOrFade(GameObject gameObject, params GameObject[] fadeObjects)
        {
            var allObjects = new List<GameObject> { gameObject }
                .Concat(fadeObjects)
                .ToArray();
            var renderers = allObjects
                .SelectMany(x => x.GetComponentsInChildren<Renderer>())
                .ToArray();

            Log.LogDebug(
                $"HideOrFade {gameObject.name} - allObjects: {allObjects.Length} - renderers: {renderers.Length}"
            );

            foreach (var rend in renderers)
            {
                rend.enabled = false;
            }

            foreach (var gameObj in allObjects)
            {
                RemoveHighlight(gameObj);
            }
        }

        private void AddResourcesToHighlights(
            bool isEnabled,
            string[] resources,
            bool useLegacy = true
        )
        {
            if (isEnabled)
            {
                foreach (var resource in resources)
                {
                    if (useLegacy)
                    {
                        Highlight.Add(LegacyResourcesAPI.Load<GameObject>(resource));
                    }
                    else
                    {
                        var addressable = Addressables
                            .LoadAssetAsync<GameObject>(resource)
                            .WaitForCompletion();
                        if (addressable)
                            Highlight.Add(addressable);
                    }
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
            if (Highlight.Any())
            {
                foreach (var gameObject in Highlight)
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
