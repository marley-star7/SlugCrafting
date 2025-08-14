


namespace SlugCrafting.Items.Weapons;

public static class WeaponHooks
{
    internal static void Update(On.Weapon.orig_Update orig, Weapon self, bool eu)
    {
        orig(self, eu);

        for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
        {
            if (self.abstractPhysicalObject.stuckObjects[i] is AbstractPhysicalObject.ImpaledOnSpearStick impaledObjectStick)
            {

                var impaledOnObject = impaledObjectStick.A;
                var realizedOnImpaledObject = impaledOnObject.realizedObject;

                if (realizedOnImpaledObject == null)
                    continue;

                if (realizedOnImpaledObject is Weapon impaledOnWeapon)
                {
                    self.rotation = impaledOnWeapon.rotation;
                }
            }
            else if (self.abstractPhysicalObject.stuckObjects[i] is ItemContainer.InsideItemContainerStick insideItemContainerStick)
            {
                self.rotation = Vector2.zero;
            }
        }
    }

    internal static void Weapon_AddToContainer(On.Weapon.orig_AddToContainer orig, Weapon self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        orig(self, sLeaser, rCam, newContainer);
        self.MoveStuckObjectsInFrontWeapon(newContainer);
    }

    internal static bool Weapon_HitSomething(On.Weapon.orig_HitSomething orig, Weapon self, SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj != null)
        {
            for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
            {
                if (self.abstractPhysicalObject.stuckObjects[i].B == result.obj.abstractPhysicalObject || (self.abstractPhysicalObject.stuckObjects[i].A == result.obj.abstractPhysicalObject))
                    return false;
            }
        }

        return orig(self, result, eu);
    }
}
