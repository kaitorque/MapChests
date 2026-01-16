using BepInEx.Configuration;
using RoR2;

namespace Faust.QoLChests.Hooks;

public static class GeneralHooks
{
    public static void Register(ConfigFile config)
    {
        RoR2Application.onLoad += InteractableStateHandler.Init;
        RoR2Application.onLoadFinished += InteractableStateHandler.PostInit;
        config.SettingChanged += (sender, args) =>
        {
            InteractableStateHandler.Reset();
        };
    }
}
