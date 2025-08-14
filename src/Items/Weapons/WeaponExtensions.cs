namespace SlugCrafting.Items.Weapons;

public static class WeaponExtensions
{
    public static void MoveStuckObjectsInFrontWeapon(this Weapon self, FContainer container)
    {
        // Ms7: Make impaled objects render in front of the spear by adding to container.
        for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
        {
            if (self.abstractPhysicalObject.stuckObjects[i] is not AbstractPhysicalObject.ImpaledOnSpearStick impaledObjectStick)
                continue;

            var impaledObject = impaledObjectStick.B;
            var realizedImpaledObject = impaledObject.realizedObject;

            if (realizedImpaledObject is not IDrawable drawable)
                continue;

            for (int j = 0; j < self.room.game.cameras.Length; j++)
            {
                self.room.game.cameras[j].MoveObjectToContainer(drawable, container);
            }
        }
    }
}
