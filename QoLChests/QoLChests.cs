using System.Diagnostics;
using BepInEx;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Hooks;
using Faust.Shared;
using Faust.Shared.Compatability;

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
        public const string PluginVersion = "1.2.1";

        protected void Awake()
        {
            var stopwatch = Stopwatch.StartNew();
            Log.Init(Logger);
            ModConfig.Init(Config);

            GeneralHooks.Register(Config);
            ChestHooks.Register();
            ShopHooks.Register();
            SceneHooks.Register();
            InteractableSpawnHooks.Register();

            StarStorm2CompatHandler.Register();
            HunkCompatHandler.Register();

            stopwatch.Stop();

            Log.LogDebug($"{nameof(Awake)} took {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
