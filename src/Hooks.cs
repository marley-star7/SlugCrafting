using ImprovedInput;
using SlugCrafting.Crafts;
using UnityEngine;

namespace SlugCrafting;

public static partial class Hooks
{
    internal static void ApplyHooks()
    {
        ApplyPlayerHooks();
        ApplyPlayerGraphicsHooks();
        ApplyLizardGraphicsHooks();

        ApplyPlayerCarryableItemHooks();
        ApplySpearHooks();
        ApplySporePlantHooks();

        MRCustom.Events.OnPlayerGrab += PlayerExtension.OnPlayerGrab;
        MRCustom.Events.OnPlayerReleaseGrasp += PlayerExtension.OnPlayerReleaseGrasp;
        MRCustom.Events.OnPlayerSwitchGrasp += PlayerExtension.OnPlayerSwitchGrasp;
    }

    internal static void RemoveHooks()
    {
        On.RainWorld.PostModsInit -= Plugin.RainWorld_PostModsInit;

        RemovePlayerHooks();
        RemovePlayerGraphicsHooks();
        RemoveLizardGraphicsHooks();

        RemovePlayerCarryableItemHooks();
        RemoveSpearHooks();
        RemoveSporePlantHooks();

        MRCustom.Events.OnPlayerGrab -= PlayerExtension.OnPlayerGrab;
        MRCustom.Events.OnPlayerReleaseGrasp -= PlayerExtension.OnPlayerReleaseGrasp;
        MRCustom.Events.OnPlayerSwitchGrasp -= PlayerExtension.OnPlayerSwitchGrasp;
    }
}