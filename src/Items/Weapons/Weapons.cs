using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponsExtInit 
{
    public class WeaponsExtInit
    {
        public static void Apply()
        {
            Plugin.LogInfo("Init WeaponsExte");
            WeaponExtHooks.ApplyHooks();
            Plugin.LogInfo("Finalize WeaponsExte");
        }

        public static void Terminate()
        {
            WeaponExtHooks.RemoveHooks();
        }
    }
}