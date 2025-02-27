namespace Faust.Shared.Compatability
{
    public class HunkCompat
    {
        public const string PluginGUID = "com.rob.Hunk";
        public static bool IsInstalled =>
            BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PluginGUID);
    }
}
