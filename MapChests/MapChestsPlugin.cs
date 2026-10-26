using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using Faust.QoLChests.Compatability;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Hooks;
using Faust.Shared;
using Faust.Shared.Compatability;
namespace MapChests
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(
        RiskOfOptionsCompat.PluginGUID,
        BepInDependency.DependencyFlags.SoftDependency
    )]
    [BepInIncompatibility("MiniMap")]
    [BepInIncompatibility("Faust.QoLChests")]
    public class MapChestsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "kaitorque";
        public const string PluginName = "MapChests";
        public const string PluginVersion = "0.1.0";

        protected void Awake()
        {
            var stopwatch = Stopwatch.StartNew();
            Log.Init(Logger);

            // Temporary until the configs are merged into one file; seeded from the original mods' files.
            ConfigFile highlightConfig = OpenConfig($"{PluginGUID}.Highlight.cfg", "Faust.QoLChests.cfg");
            ConfigFile minimapConfig = OpenConfig($"{PluginGUID}.Minimap.cfg", "MiniMap.cfg");

            ModConfig.Init(highlightConfig);

            GeneralHooks.Register(highlightConfig);
            ChestHooks.Register();
            ShopHooks.Register();
            SceneHooks.Register();
            InteractableSpawnHooks.Register();
            StageEventHooks.Register();
            CharacterHooks.Register();

            QualityCompatHandler.Register();
            StarStorm2CompatHandler.Register();
            HunkCompatHandler.Register();
            SandsweptCompatHandler.Register();

            gameObject.AddComponent<MiniMapMod.MinimapController>().Init(minimapConfig, Logger);

            stopwatch.Stop();

            Log.LogDebug($"{nameof(Awake)} took {stopwatch.ElapsedMilliseconds}ms");
        }

        private ConfigFile OpenConfig(string fileName, string seedFileName)
        {
            string path = Path.Combine(Paths.ConfigPath, fileName);
            string seedPath = Path.Combine(Paths.ConfigPath, seedFileName);

            if (!File.Exists(path) && File.Exists(seedPath))
            {
                File.Copy(seedPath, path);
                Logger.LogInfo($"Created {fileName} from {seedFileName}");
            }

            return new ConfigFile(path, true, Info.Metadata);
        }
    }
}
