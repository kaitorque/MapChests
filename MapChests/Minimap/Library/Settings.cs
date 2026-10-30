using Faust.QoLChests.Configs;
using MapChests.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MiniMapLibrary
{
    public enum LogLevel
    {
        none,
        info,
        debug,
        all
    }

    public static class Settings
    {
        public static class Icons
        {
            public const string Default = "Textures/MiscIcons/texMysteryIcon";
            public const string LootBag = "Textures/MiscIcons/texLootIconOutlined";
            public const string Chest = "Textures/MiscIcons/texInventoryIconOutlined";
            public const string Circle = "Textures/MiscIcons/texBarrelIcon";
            public const string Shrine = "Textures/MiscIcons/texShrineIconOutlined";
            public const string Boss = "Textures/MiscIcons/texTeleporterIconOutlined";
            public const string Drone = "Textures/MiscIcons/texDroneIconOutlined";
            public const string Cross = "Textures/MiscIcons/texCriticallyHurtIcon";
            public const string Lock = "Textures/MiscIcons/texUnlockIcon";
            public const string Dice = "Textures/MiscIcons/texRuleMapIsRandom";
            public const string Cube = "Textures/MiscIcons/texLunarPillarIcon";
            public const string Wrench = "Textures/MiscIcons/texWIPIcon";
            public const string Attack = "Textures/MiscIcons/texAttackIcon";
            public const string Sprint = "Textures/MiscIcons/texSprintIcon";
            public const string Arrow = "Textures/MiscIcons/texOptionsArrowLeft";
            public const string Pencil = "Textures/MiscIcons/texEditIcon";
            public const string Portal = "Textures/MiscIcons/texQuickplay";
            public const string Lunar = "Textures/ItemIcons/BG/texLunarBGIcon";
        }

        public static class Colors
        {
            public static Color Yellow = new Color(255 / 255.0f, 219 / 255.0f, 88 / 255.0f);
            public static Color Purple = new Color(122 / 255.0f, 105 / 255.0f, 244 / 255.0f);
            public static Color DarkPurple = new Color(76 / 255.0f, 53 / 255.0f, 244 / 255.0f);
            public static Color Teal = new Color(94 / 255.0f, 178 / 255.0f, 242 / 255.0f);
            public static Color Pink = new Color(172 / 255.0f, 95 / 255.0f, 243 / 255.0f);
            public static Color DarkPink = new Color(146 / 255.0f, 39 / 255.0f, 243 / 255.0f);
            public static Color Orange = new Color(255 / 255.0f, 146 / 255.0f, 0 / 255.0f);
            public static Color DarkTeal = new Color(94 / 255.0f, 178 / 255.0f, 242 / 255.0f);
        }

        public static Dimension2D MinimapSize { get; set; } = new Dimension2D(100, 100);

        public static Dimension2D ViewfinderSize { get; set; } = new Dimension2D(10, 100);

        public static IDictionary<InteractableKind, InteractibleSetting> InteractibleSettings { get; } = new Dictionary<InteractableKind, InteractibleSetting>();

        private static readonly Dimension2D DefaultUIElementSize = new Dimension2D(10, 10);

        public static Color DefaultActiveColor { get; set; } = Colors.Yellow;

        public static Color DefaultInactiveColor { get; set; } = Color.grey;

        public static Color DefaultElevationMarkerColor { get; set; } = Color.white;

        public static Dimension2D DefaultElevationSize { get; set; } = new Dimension2D(3.0f, 3.0f);

        public static LogLevel LogLevel => ModConfig.Instance.MinimapLogLevel.Value;

        public static float MinimapScale => ModConfig.Instance.ZoomLevel.Value;

        public static float IconScale => ModConfig.Instance.IconScale.Value;

        public static KeyCode MinimapKey => ModConfig.Instance.MinimapKey.Value;

        public static KeyCode MinimapIncreaseScaleKey => ModConfig.Instance.ZoomInKey.Value;

        public static KeyCode MinimapDecreaseScaleKey => ModConfig.Instance.ZoomOutKey.Value;

        /// <summary>Live settings of a category, read from the merged config.</summary>
        public static CategorySettings GetSetting(MapCategory category) => ModConfig.Instance.Get(category);

        public static Color GetColor(MapCategory category, bool active)
        {
            var setting = GetSetting(category);
            return active ? setting.MinimapColor.Value : setting.MinimapUsedColor.Value;
        }

        static Settings()
        {
            InitializeDefaultSettings();
        }

        private static void InitializeDefaultSettings()
        {
            static void Add(InteractableKind type,
                float width = -1,
                float height = -1,
                Color activeColor = default,
                Color inactiveColor = default,
                string description = "",
                string path = Icons.Default,
                Color elevationMarkerColor = default
            )
            {
                activeColor = activeColor == default ? DefaultActiveColor : activeColor;
                inactiveColor = inactiveColor == default ? DefaultInactiveColor : inactiveColor;
                elevationMarkerColor = elevationMarkerColor == default ? DefaultElevationMarkerColor : elevationMarkerColor;

                Dimension2D size = DefaultUIElementSize;

                if (width != -1 || height != -1)
                {
                    size = new Dimension2D(width, height);
                }

                var setting = new InteractibleSetting()
                {
                    ActiveColor = activeColor,
                    InactiveColor = inactiveColor,
                    Dimensions = size
                };

                setting.Description = description;
                setting.IconPath = path;

                setting.ElevationMarkerColor = elevationMarkerColor;
                setting.ElevationMarkerEnabled = true;
                setting.ElevationMarkerDimensions = new Dimension2D(DefaultElevationSize.Width, DefaultElevationSize.Height);
                setting.ElevationMarkerOffset = new Dimension2D(
                    (size.Width / 2.0f) + (DefaultElevationSize.Width / 2.0f),
                    (size.Height / 2.0f) + (DefaultElevationSize.Height / 2.0f)
                );

                InteractibleSettings.Add(type, setting);
            }

            Add(InteractableKind.Chest, 8, 6,
                description: "Chests, Roulette Chests",
                path: Icons.Chest);

            Add(InteractableKind.Shop, 7, 5,
                activeColor: Colors.Teal,
                description: "Shops",
                path: Icons.Chest);

            Add(InteractableKind.Equipment, 8, 6,
                activeColor: Colors.Orange,
                description: "Equipment Barrels",
                path: Icons.Chest);

            Add(InteractableKind.Printer, 10, 8,
                activeColor: Colors.Purple,
                description: "Printers",
                path: Icons.Chest);

            Add(InteractableKind.Utility, 5, 5,
                description: "Scrappers",
                path: Icons.Wrench);

            Add(InteractableKind.LunarPod, 7, 7,
                description: "Lunar pods (chests)",
                activeColor: Color.cyan,
                path: Icons.LootBag);

            Add(InteractableKind.Shrine,
                description: "All shrines (excluding Newt)",
                path: Icons.Shrine);

            Add(InteractableKind.NewtAltar,
                description: "Newt Altar",
                path: Icons.Lunar);

            Add(InteractableKind.Teleporter, 15, 15,
                activeColor: Color.white,
                inactiveColor: Color.green,
                description: "Boss teleporters",
                path: Icons.Boss);

            Add(InteractableKind.Barrel, 3, 3,
                description: "Barrels",
                path: Icons.Circle);

            Add(InteractableKind.Drone, 7, 7,
                description: "Drones",
                path: Icons.Drone);

            Add(InteractableKind.Special, 5, 5,
                description: "Special interactibles such as the landing pod and fans",
                path: Icons.Default);

            Add(InteractableKind.EnemyMonster, 3, 3,
                activeColor: Color.red,
                elevationMarkerColor: Color.red,
                description: "Enemies",
                path: Icons.Circle);

            Add(InteractableKind.EnemyLunar, 3, 3,
                activeColor: Color.red,
                elevationMarkerColor: Color.red,
                description: "Lunar enemies",
                path: Icons.Circle);

            Add(InteractableKind.EnemyVoid, 12, 12,
                activeColor: Color.magenta,
                elevationMarkerColor: Color.magenta,
                description: "Void touched enemies",
                path: Icons.Attack);

            Add(InteractableKind.Minion, 3, 3,
                activeColor: Color.green,
                elevationMarkerColor: Color.green,
                description: "Minions",
                path: Icons.Circle);

            Add(InteractableKind.Player, 8, 8,
                activeColor: Color.white,
                inactiveColor: Color.white,
                description: "Player, including friends",
                path: Icons.Sprint);

            Add(InteractableKind.Item, 3, 3,
                activeColor: Colors.Teal,
                inactiveColor: Color.cyan,
                description: "Dropped items and lunar coins",
                path: Icons.Circle);

            Add(InteractableKind.Portal, 7, 7,
               activeColor: DefaultActiveColor,
               inactiveColor: DefaultInactiveColor,
               description: "Portal markers, or objects that spawn portals when interacted with (excluding newt altar)",
               path: Icons.Portal);

            Add(InteractableKind.Totem, 7, 7,
               activeColor: Colors.DarkTeal,
               inactiveColor: DefaultInactiveColor,
               description: "Totems, or totem adjacent objects",
               path: Icons.Cube);

            Add(InteractableKind.Neutral, 4, 4,
               activeColor: Color.white,
               inactiveColor: DefaultInactiveColor,
               description: "Neutral objects or NPCs",
               path: Icons.Circle);
        }

        public static InteractibleSetting GetSetting(InteractableKind type)
        {
            if (InteractibleSettings.ContainsKey(type))
            {
                return InteractibleSettings[type];
            }

            return new InteractibleSetting()
            {
                Dimensions = DefaultUIElementSize,
                ActiveColor = DefaultActiveColor,
                InactiveColor = DefaultInactiveColor,
                Description = "NO_DESCRIPTION",
                IconPath = Icons.Default
            };
        }

    }
}
