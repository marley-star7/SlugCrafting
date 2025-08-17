using System.Reflection;

namespace WeaponsExtInit
{
    public static class WeaponExtHelper
    {
        /// <summary>
        /// Return the magnetic properties of a object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Magnetic(object obj)
        {
            if (obj is IWeaponExtension weaponExt)
            {
                return weaponExt.IsMagnetic;
            }

            return ExtMagneticProperties(obj);
        }

        /// <summary>
        /// Add Magnetic properties to object where is not implemented IWeaponExtension
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
    }
}
