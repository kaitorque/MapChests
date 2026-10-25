using R2API;
using RoR2;

namespace Faust.Shared.Compatability
{
    public class BetterUICompat
    {
        public const string PluginGUID = "com.xoxfaby.BetterUI";
        public static bool IsInstalled =>
            BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PluginGUID);

        public static (string nameToken, string descToken) CreateBetterUIBuffInformation(
            string langTokenName,
            string name,
            string description,
            bool isBuff = true
        )
        {
            var nameToken = isBuff ? $"BUFF_{langTokenName}_NAME" : $"DEBUFF_{langTokenName}_NAME";
            var descToken = isBuff ? $"BUFF_{langTokenName}_DESC" : $"DEBUFF_{langTokenName}_DESC";

            LanguageAPI.Add(nameToken, name);
            LanguageAPI.Add(descToken, description);

            return (nameToken, descToken);
        }

        public static void RegisterBuffInfo(
            BuffDef buffDef,
            string nameToken,
            string descriptionToken
        )
        {
            BetterUI.Buffs.RegisterBuffInfo(buffDef, nameToken, descriptionToken);
        }
    }
}
