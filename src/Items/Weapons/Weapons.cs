using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponsExtInit 
{
    public static class WeaponMagnetic
    {
        static ConditionalWeakTable<PhysicalObject, MagneticState> dataTable = new ConditionalWeakTable<PhysicalObject, MagneticState>();
        public static MagneticState GetCustomData(this PhysicalObject self) => dataTable.GetOrCreateValue(self);

        public class MagneticState
        {
            public bool IsMagnetic;
        }

        public static MagneticState GetOrCreateValueManual(PhysicalObject self)
        {
            if (dataTable.TryGetValue(self, out var data))
            {
                return data;
            }
            else
            {
                var newData = new MagneticState();
                newData.IsMagnetic = SetMagneticValue(self);
                dataTable.Add(self, newData);
                return newData;
            }
        }

        public static bool SetMagneticValue(PhysicalObject self)
        {
            if(self == null)
            {
                //Add a error log here
                return false;
            }

            if(self is Spear)
            {
                //Just test
                return false;
            }

            return CustomObjectMagnetic(self);
        }

        //Mods can hook to this function to set their custom properties
        //If not found the object, is the to true by default
        public static bool CustomObjectMagnetic(PhysicalObject self)
        {
            return true;
        }
    }

    public class WeaponsExtInit
    {
        public static void Init()
        {
            WeaponExtHooks.Init();
        }
    }
}