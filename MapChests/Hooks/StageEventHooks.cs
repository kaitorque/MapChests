using Faust.QoLChests.Handlers;
using RoR2;

namespace Faust.QoLChests.Hooks;

/// <summary>
/// Interactables that appear after the stage-start highlight pass: the Wake's Ancient Sepulcher is enabled later,
/// the Hallowed shrine and its two hands spawn once the teleporter is charged, and Mimics are enemies disguised as chests.
/// </summary>
public static class StageEventHooks
{
    public static void Register()
    {
        On.RoR2.CharacterBody.Start += CharacterBodyStart;
        On.EntityStates.Mimic.Chest.Opening.OnEnter += MimicOpeningOnEnter;
        On.RoR2.WakeController.Start += WakeControllerStart;
        On.HallowingShrineSplitBase.Start += HallowingShrineStart;
        On.RoR2.DrainItemsAndDropMultiChoicePotentialController.OnEnable += DrainItemsOnEnable;
        On.RoR2.GrantHexesController.OnEnable += GrantHexesOnEnable;
    }

    private static void CharacterBodyStart(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
    {
        orig(self);
        if (self.GetComponent<MimicController>())
            InteractableStateHandler.TrackLateInteractable(self.gameObject);
    }

    private static void MimicOpeningOnEnter(On.EntityStates.Mimic.Chest.Opening.orig_OnEnter orig, EntityStates.Mimic.Chest.Opening self)
    {
        orig(self);
        InteractableStateHandler.UntrackInteractable(self.outer.gameObject);
    }

    private static void WakeControllerStart(On.RoR2.WakeController.orig_Start orig, WakeController self)
    {
        orig(self);
        InteractableStateHandler.TrackLateInteractable(self.gameObject);
    }

    private static void HallowingShrineStart(On.HallowingShrineSplitBase.orig_Start orig, HallowingShrineSplitBase self)
    {
        orig(self);
        InteractableStateHandler.TrackLateInteractable(self.gameObject);
    }

    private static void DrainItemsOnEnable(
        On.RoR2.DrainItemsAndDropMultiChoicePotentialController.orig_OnEnable orig,
        DrainItemsAndDropMultiChoicePotentialController self
    )
    {
        orig(self);
        InteractableStateHandler.TrackLateInteractable(self.gameObject);
    }

    private static void GrantHexesOnEnable(On.RoR2.GrantHexesController.orig_OnEnable orig, GrantHexesController self)
    {
        orig(self);
        InteractableStateHandler.TrackLateInteractable(self.gameObject);
    }
}
