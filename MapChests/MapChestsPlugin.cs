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
    [BepInDependency(
        ParallelCompat.PluginGUID,
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

            string configPath = Path.Combine(Paths.ConfigPath, $"{PluginGUID}.cfg");
            // Defaults come from the original mods' files only the first time the merged file is created.
            LegacyConfig legacy = File.Exists(configPath) ? LegacyConfig.Empty() : LegacyConfig.Load();
            var config = new ConfigFile(configPath, true, Info.Metadata);

            ModConfig.Init(config, legacy);

            var minimap = gameObject.AddComponent<MiniMapMod.MinimapController>();
            minimap.Init(Logger);

            GeneralHooks.Register(config, minimap.RequestReset);
            Faust.QoLChests.Handlers.InteractableStateHandler.LateInteractableChanged += _ => minimap.RequestReset();
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
            ParallelCompat.Register();

            stopwatch.Stop();

            Log.LogDebug($"{nameof(Awake)} took {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
