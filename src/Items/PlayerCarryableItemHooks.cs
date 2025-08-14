namespace SlugCrafting;

internal static class PlayerCarryableHooks
{
    internal static void PlayerCarryableItem_Update(On.PlayerCarryableItem.orig_Update orig, PlayerCarryableItem self, bool eu)
    {
        orig(self, eu);

        var stackedWithOtherObject = false;
        for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
        {
            // MS7: The Object impaled / bundled should not pick-uppable, nor should they collide.
            var currentObjectStick = self.abstractPhysicalObject.stuckObjects[i];
            if (currentObjectStick.B != self.abstractPhysicalObject || currentObjectStick is not AbstractPhysicalObject.ImpaledOnSpearStick && currentObjectStick is not BundledItemStick)
                continue;

            stackedWithOtherObject = true;
            break;
        }
        if (stackedWithOtherObject)
        {
            self.forbiddenToPlayer = 1;
            self.firstChunk.collideWithObjects = false;
        }
        else
        {
            self.firstChunk.collideWithObjects = true;
        }
    }

}

