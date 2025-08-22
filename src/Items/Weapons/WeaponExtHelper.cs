namespace SlugCrafting.Items.WeaponsExtension
{
    public static class WeaponExtHelper
    {
        /// <summary>
        /// Return the magnetic properties of a object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool MagneticValue(object obj)
        {
            if (obj is IWeaponExtension weaponExt)
            {
                return weaponExt.IsMagnetic;
            }

            return ExtMagneticProperties(obj);
        }

        /// <summary>
        /// Add MagneticValue properties to object where is not implemented IWeaponExtension
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool ExtMagneticProperties(object obj)
        {
            if (obj == null)
            {
                // Add an error log here
                return false;
            }
            if (obj is ExplosiveSpear)
            {
                // Just test
                return true;
            }
            return false;
        }

        //To do, add documentation
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
}
