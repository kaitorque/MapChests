namespace Faust.Shared.Compatability
{
    public class SandSweptCompat
    {
        public const string PluginGUID = "com.TeamSandswept.Sandswept";
        public static bool IsInstalled =>
            BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PluginGUID);
    }
}
