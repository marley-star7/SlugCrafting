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
    }
}