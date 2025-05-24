using UnityEngine;
using RWCustom;

using SlugCrafting.Items;

namespace SlugCrafting.Hooks;

public static partial class Hooks
{
    private static void ApplySpearHooks()
    {
        On.Spear.Update += Spear_Update;
        On.Spear.ChangeMode += Spear_ChangeMode;
    }

    private static void RemoveSpearHooks()
    {
        On.Spear.Update -= Spear_Update;
        On.Spear.ChangeMode -= Spear_ChangeMode;
    }

    private static void Spear_ChangeMode(On.Spear.orig_ChangeMode orig, Spear self, Weapon.Mode newMode)
    {
        orig(self, newMode);

        /*
        var spearData = self.SlugCrafting();
        if (newMode == Weapon.Mode.StuckInWall)
            spearData.skewer.active = true;
        else
            spearData.skewer.active = false;
        */
    }

    // TODO: add an extension stuff to hold data for this, then can impale

    private static void Spear_Update(On.Spear.orig_Update orig, Spear self, bool eu)
    {
        // TODO: add sound for the impale
        // TODO: give custom sprite for these kinda spear traps.

        orig(self, eu);

        var spearData = self.SlugCrafting();
        if (self.mode == Weapon.Mode.StuckInWall)
        {
            // TODO: move these to permanent variables
            var spearHitboxDistFromChunk = 20f;
            var impaledChunkBaseVelocityModifier = 0.1f;
            var impaledChunkMovingIntoSpearModifier = 0.3f;

            //
            // REMEMBER ROTATION OF SPEARS ARE SAVED AS DIRECTION OF -1 to 1 UP AND DOWN FOR SOME REASON.
            // CONFUSED ME BAD TIME AT FIRST.
            //

            var chunk = self.firstChunk;

            Vector2 impaleDir = new Vector2(-self.rotation.x, -self.rotation.y);
            var offsetX = impaleDir.x * spearHitboxDistFromChunk;
            var offsetY = impaleDir.y * spearHitboxDistFromChunk;
            // Get the hitbox area of the spear's enter pointy end, based off chunk radius.
            Vector2 hitboxPos = new Vector2(chunk.pos.x + offsetX, chunk.pos.y + offsetY);

            var leftX = hitboxPos.x - chunk.rad;
            var rightX = hitboxPos.x + chunk.rad;
            var bottomY = hitboxPos.y - chunk.rad;
            var topY = hitboxPos.y + chunk.rad;

            var startPos = new Vector2(leftX, bottomY);
            var endPos = new Vector2(rightX, topY);

            var collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(null, self.room, startPos, ref endPos, 1f, 1, self, false);
            var intersectedBodyChunk = collisionResult.chunk;

            // If found new body chunk, and it is not already skewered by the spear, add it.
            if (intersectedBodyChunk != null)
            {
                intersectedBodyChunk.owner.Destroy();
            }
        }
        else
        {
            //spearData.skewer.active = false;
        }
    }
}