using RWCustom;
using UnityEngine;

using SlugCrafting.Items;

namespace SlugCrafting.Hooks;

public static partial class Hooks
{
    // Add hooks
    public static void ApplyCreatureHooks()
    {
        On.Creature.Grab += Creature_Grab;
        On.Creature.ReleaseGrasp += Creature_ReleaseGrasp;
    }

    // Remove hooks
    public static void RemoveCreatureHooks()
    {
        On.Creature.Grab -= Creature_Grab;
        On.Creature.ReleaseGrasp -= Creature_ReleaseGrasp;
    }

    private static bool Creature_Grab(On.Creature.orig_Grab orig, Creature self, PhysicalObject grabbedObj, int graspUsed, int chunkGrabbed, Creature.Grasp.Shareability shareability, float dominance, bool overrideEquallyDominant, bool pacifying)
    {
        bool result = orig(self, grabbedObj, graspUsed, chunkGrabbed, shareability, dominance, overrideEquallyDominant, pacifying);

        if (self is Player)
            PlayerExtension.OnPlayerGrab((Player)self, grabbedObj, graspUsed, chunkGrabbed, shareability, dominance, overrideEquallyDominant, pacifying);

        return result;
    }

    private static void Creature_ReleaseGrasp(On.Creature.orig_ReleaseGrasp orig, Creature self, int grasp)
    {
        orig(self, grasp);

        if (self is Player)
            PlayerExtension.OnPlayerReleaseGrasp((Player)self, grasp);
    }
}