using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Items.WeaponsExtension
{
    public class Weapons
    {
        public static void Apply()
        {
            Plugin.LogInfo("Start WeaponsExtension");
            WeaponExtHooks.ApplyHooks();
            Plugin.LogInfo("End WeaponsExtension");
        }

        public static void Remove()
        {
            WeaponExtHooks.RemoveHooks();
        }
    }
}