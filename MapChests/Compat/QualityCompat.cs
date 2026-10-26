namespace Faust.Shared.Compatability
{
    public class QualityCompat
    {
        public const string PluginGUID = "com.Gorakh.ItemQualities";
        public static bool IsInstalled =>
            BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PluginGUID);
    }
}
