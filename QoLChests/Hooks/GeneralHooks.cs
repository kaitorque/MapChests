using BepInEx.Configuration;
using RoR2;

namespace Faust.QoLChests.Hooks;

public static class GeneralHooks
{
    public static void Register(ConfigFile config)
    {
        RoR2Application.onLoad += InteractableStateHandler.Init;
        config.SettingChanged += (sender, args) =>
        {
            InteractableStateHandler.Reset();
        };
    }
}
