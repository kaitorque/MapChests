using System;
using BepInEx.Configuration;
using Faust.QoLChests.Configs;
using Faust.QoLChests.Handlers;
using RoR2;

namespace Faust.QoLChests.Hooks;

public static class GeneralHooks
{
    public static void Register(ConfigFile config, Action resetMinimap)
    {
        RoR2Application.onLoad += InteractableStateHandler.Init;
        RoR2Application.onLoadFinished += InteractableStateHandler.PostInit;
        // RoR2Application.onLoadFinished += AddressableDumper.DumpAddressables;

        config.SettingChanged += (sender, args) =>
        {
            // the minimap applies zoom and icon scale itself; rebuilding everything on every zoom key press would stutter
            if (args.ChangedSetting == ModConfig.Instance.ZoomLevel || args.ChangedSetting == ModConfig.Instance.IconScale)
                return;
            InteractableStateHandler.Reset();
            resetMinimap();
        };
    }
}
