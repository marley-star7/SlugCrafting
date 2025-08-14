


namespace SlugCrafting.Items;

internal static class PhysicalObjectHooks
{
    internal static void PhysicalObject_Collide(On.PhysicalObject.orig_Collide orig, PhysicalObject self, PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        // Don't hit something we are stuck to!
        for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
        {
            if (self.abstractPhysicalObject.stuckObjects[i].A == otherObject.abstractPhysicalObject || self.abstractPhysicalObject.stuckObjects[i].B == otherObject.abstractPhysicalObject)
                return;
        }

        orig(self, otherObject, myChunk, otherChunk);
    }

    internal static void PhysicalObject_HitByWeapon(On.PhysicalObject.orig_HitByWeapon orig, PhysicalObject self, Weapon weapon)
    {
        // Don't hit something we are stuck to!
        for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
        {
            if (self.abstractPhysicalObject.stuckObjects[i].A == weapon.abstractPhysicalObject)
                return;
        }

        orig (self, weapon);
    }

    internal static void PhysicalObject_Update(On.PhysicalObject.orig_Update orig, PhysicalObject self, bool eu)
    {
        orig(self, eu);
        self.BundledStickUpdate(eu);
    }
}
