using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponsExtInit
{
    //not sure now if we should use this
    internal abstract class WeaponExt : Weapon
    {
        bool magnetic = true;

        public WeaponExt(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {

        }
    }

   
}