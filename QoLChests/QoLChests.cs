using BepInEx;
using BepInEx.Configuration;
using BetterUnityPlugin;
using QoLChests;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Faust.QoLChests
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class QoLChests : BetterUnityPlugin<QoLChests>
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = nameof(QoLChests);
        public const string PluginVersion = "1.1.6";

        //Configuration
        public static ConfigEntry<bool> HideEmptyChests, HideUsedShops, HighlightChests, HighlightShops, HighlightScrapper, HighlightDuplicator, HighlightDrones, HightlightTurrets, RemoveHighlightFromUsed, FadeInsteadOfHide;
        public static ConfigEntry<float> HideTime;
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
            "prefabs/networkedobjects/chest/Chest1StealthedVariant",
            "prefabs/networkedobjects/chest/Chest2",
            "prefabs/networkedobjects/chest/EquipmentBarrel",
            "prefabs/networkedobjects/chest/GoldChest",
            "prefabs/networkedobjects/chest/LunarChest",
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

        private List<GameObject> Highlight { get; set; } = new List<GameObject>();

        public override BaseUnityPlugin typeReference => throw new System.NotImplementedException();

        //The Awake() method is run at the very start when the game is initialized.
        protected override void Awake()
        {
            base.Awake();
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            //Configuration
            HideEmptyChests = Config.Bind("Hide", "Chest", true, "Hides empty chests after a few seconds");
            HideUsedShops = Config.Bind("Hide", "Shops", true, "Hides used shops after a few seconds");
            HideTime = Config.Bind("Hide", "Time", 1f, "Time before stuff is hidden");
            FadeInsteadOfHide = Config.Bind("Hide", "Fade", false, "Fade instead of hiding");

            RemoveHighlightFromUsed = Config.Bind("Highlight", "RemoveWhenUsed", false, "Remove highlight when used");
            HighlightChests = Config.Bind("Highlight", "Chest", true, "Highlight Chests (Chests, Barrels etc.)");
            HighlightDuplicator = Config.Bind("Highlight", "Duplicator", true, "Highlight Duplicators");
            HighlightScrapper = Config.Bind("Highlight", "Scrapper", true, "Highlight Scrappers");
            HighlightShops = Config.Bind("Highlight", "Shops", true, "Highlight Shops");
            HighlightDrones = Config.Bind("Highlight", "Drones", true, "Highlight Drones");
            HightlightTurrets = Config.Bind("Highlight", "Turrets", true, "Highlight Turrets");
            HighlightColor = Config.Bind("Highlight", "Color", ConfigHighlightColor.Yellow, string.Join(", ", Enum.GetNames(typeof(ConfigHighlightColor))));

            // Hooks
            Hooks.Add<EntityStates.Barrel.Opened>(nameof(EntityStates.Barrel.Opened.OnEnter), BarrelOpened);
            On.RoR2.MultiShopController.OnPurchase += MultiShopController_OnPurchase;
            On.RoR2.RouletteChestController.Opened.OnEnter += Roulette_Opened;

            // Highlight Resources
            AddResourcesToHighlights(HighlightChests.Value, ChestResourcesPaths);
            AddResourcesToHighlights(HighlightShops.Value, ShopResourcePaths);
            AddResourcesToHighlights(HighlightScrapper.Value, ScrapperResourcePaths);
            AddResourcesToHighlights(HighlightDuplicator.Value, DuplicatorResourcesPaths);
            AddResourcesToHighlights(HighlightDrones.Value, DroneResourcesPaths);
            AddResourcesToHighlights(HightlightTurrets.Value, TurrentResourcePaths);

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        static void BarrelOpened(Action<EntityStates.Barrel.Opened> orig, EntityStates.Barrel.Opened self)
        {
            orig.Invoke(self);
            if (HideEmptyChests.Value)
            {
                DestroyOrFade(self.outer.gameObject, HideTime.Value, self.outer.commonComponents.modelLocator.modelTransform.gameObject);
            }

            if (RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.outer.gameObject);
            }
        }

        private void MultiShopController_OnPurchase(On.RoR2.MultiShopController.orig_OnPurchase orig, MultiShopController self, Interactor interactor, PurchaseInteraction purchaseInteraction)
        {
            orig.Invoke(self, interactor, purchaseInteraction);

            if (self.Networkavailable)
                return;

            if (HideUsedShops.Value)
            {
                var terminalObjects = self.terminalGameObjects.Select(x => x).Concat(new[] { self.gameObject }).ToArray();
                DestroyOrFade(self.gameObject, HideTime.Value, terminalObjects);
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


        private void Roulette_Opened(On.RoR2.RouletteChestController.Opened.orig_OnEnter orig, EntityStates.EntityState self)
        {
            orig.Invoke(self);
            if (HideEmptyChests.Value)
            {
                Destroy(self.outer.gameObject, HideTime.Value);
            }

            if (RemoveHighlightFromUsed.Value)
            {
                RemoveHighlight(self.outer.gameObject);
            }

        }

        protected override void Update()
        {
            AddHighlights();
        }

        private static void DestroyOrFade(GameObject gameObject, float time, params GameObject[] fadeObjects)
        {
            if (FadeInsteadOfHide.Value)
            {
                foreach (var fadeObject in fadeObjects)
                {
                    fadeObject.AddComponent<FadeObject>();
                }
            }
            else
            {
                Destroy(gameObject, time);
            }
        }

        private void AddResourcesToHighlights(bool isEnabled, string[] resources, bool useLegacy = true)
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
                        Highlight.Add(Resources.Load<GameObject>(resource));
                    }
                }
            }
        }

        private static void RemoveHighlight(GameObject self)
        {
            var highlight = self.GetComponent<Highlight>();
            if (highlight)
            {
                Log.LogDebug($"Removing highlight from {self.name}");
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
            if (component != null)
            {
                component.isOn = true;
                component.highlightColor = (Highlight.HighlightColor)(int)HighlightColor.Value;
            }
        }
    }
}
