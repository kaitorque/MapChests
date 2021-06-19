using BepInEx;
using BepInEx.Configuration;
using On.EntityStates.Barrel;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Faust.QoLChests
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

    //This attribute specifies that we have a dependency on R2API, as we're using it to add our item to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.bepis.r2api")]

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    //We will be using 3 modules from R2API: ItemAPI to add our item, ItemDropAPI to have our item drop ingame, and LanguageAPI to add our language tokens.
    [R2APISubmoduleDependency()]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class QoLChests : BaseUnityPlugin
    {
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Faust";
        public const string PluginName = nameof(QoLChests);
        public const string PluginVersion = "1.1.0";

        //Configuration
        public static ConfigEntry<bool> HideEmptyChests { get; set; }
        public static ConfigEntry<bool> HideUsedShops { get; set; }
        public static ConfigEntry<float> HideTime { get; set; }
        public static ConfigEntry<bool> HighlightChests { get; set; }
        public static ConfigEntry<bool> HighlightDrones { get; set; }

        // Chest resources
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
            "prefabs/networkedobjects/chest/Duplicator",
            "prefabs/networkedobjects/chest/DuplicatorLarge",
            "prefabs/networkedobjects/chest/DuplicatorMilitary",
            "prefabs/networkedobjects/chest/DuplicatorWild",
            "prefabs/networkedobjects/chest/EquipmentBarrel",
            "prefabs/networkedobjects/chest/GoldChest",
            "prefabs/networkedobjects/chest/LunarChest",
            "prefabs/networkedobjects/chest/LunarShopTerminal",
            "prefabs/networkedobjects/chest/MultiShopEquipmentTerminal",
            "prefabs/networkedobjects/chest/MultiShopLargeTerminal",
            "prefabs/networkedobjects/chest/MultiShopTerminal",
            "prefabs/networkedobjects/chest/Scrapper",
            "prefabs/networkedobjects/chest/TripleShop",
            "prefabs/networkedobjects/chest/TripleShopEquipment",
            "prefabs/networkedobjects/chest/TripleShopLarge",
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
            "prefabs/networkedobjects/brokendrones/Turret1Broken",
        };

        private List<GameObject> Highlight { get; set; } = new List<GameObject>();

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            //Configuration
            HideEmptyChests = Config.Bind("Hide", "Chest", true, "Hides empty chests after a few seconds");
            HideUsedShops = Config.Bind("Hide", "Shops", true, "Hides used shops after a few seconds");
            HideTime = Config.Bind("Hide", "Time", 1f, "Time before stuff is hidden");
            HighlightChests = Config.Bind("Highlight", "Chest", true, "Highlight chests (Chests, Shops, Scrappers, Barrels etc.)");
            HighlightDrones = Config.Bind("Highlight", "Drones", true, "Highlight drones");

            // Hooks
            Opened.OnEnter += Opened_OnEnter;
            On.RoR2.MultiShopController.DisableAllTerminals += MultiShopController_DisableAllTerminals;

            // Highlight Resources
            if (HighlightChests.Value)
            {
                foreach (var chest in ChestResourcesPaths)
                {
                    Highlight.Add(Resources.Load<GameObject>(chest));
                }
            }

            if (HighlightDrones.Value)
            {
                foreach (var drone in DroneResourcesPaths)
                {
                    Highlight.Add(Resources.Load<GameObject>(drone));
                }
            }

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        private void MultiShopController_DisableAllTerminals(On.RoR2.MultiShopController.orig_DisableAllTerminals orig, MultiShopController self, Interactor interactor)
        {
            orig.Invoke(self, interactor);
            if (!HideUsedShops.Value)
                return;

            Destroy(self.gameObject, HideTime.Value);
        }

        public void Update()
        {
            AddHighlights();
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
                return;

            var component = gameObject.GetComponent<Highlight>();
            if (component != null)
            {
                component.isOn = true;
                component.highlightColor = RoR2.Highlight.HighlightColor.interactive;
            }
        }

        private void Opened_OnEnter(Opened.orig_OnEnter orig, EntityStates.Barrel.Opened self)
        {
            orig.Invoke(self);
            if (!HideEmptyChests.Value)
                return;

            Destroy(self.outer.gameObject, HideTime.Value);
        }
    }
}
