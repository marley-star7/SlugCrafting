using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponsExtInit
{
    public static class WeaponExtHooks
    {
        public static void Init()
        {
            UnityEngine.Debug.Log("WeaponsExtHooks: Initializing WeaponExt Hooks...");
            On.GarbageWormAI.Update += GarbageWormAI_Update;
        }

        private static void GarbageWormAI_Update(On.GarbageWormAI.orig_Update orig, GarbageWormAI self)
        {
            orig(self);
            if (self != null && self.worm != null && self.worm.grasps != null)
            {
                try
                {
                    //The worm always has a gras place, but shold make sure if exits
                    if (self.worm.grasps[0] != null)
                    {
                        if(WeaponExtHelper.Magnetic(self.worm.grasps[0].grabbed))
                        {
                            UnityEngine.Debug.Log($"[Crafter] The object gras is not magnetic: {self.worm.grasps[0].grabbed}");
                            self.worm.LoseAllGrasps();
                        }
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[Crafter] Error in GarbageWormAI_Update: {ex.Message}");
                }
            }
        }
    }
}
