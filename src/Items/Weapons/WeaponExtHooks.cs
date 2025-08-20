using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponsExtInit
{
    public static class WeaponExtHooks
    {
        public static void RemoveHooks()
        {
            On.Weapon.AddToContainer -= Weapon_AddToContainer;
            On.Weapon.HitSomething -= Weapon_HitSomething;
            On.Weapon.Update -= Update;

            On.GarbageWormAI.Update -= GarbageWormAI_Update;
            //IL.GarbageWormAI.Update -= GarbageWormAI_Update1;
        }
        public static void ApplyHooks()
        {
            On.Weapon.AddToContainer += Weapon_AddToContainer;

            On.Weapon.HitSomething += Weapon_HitSomething;
            On.Weapon.Update += Update;

            On.GarbageWormAI.Update += GarbageWormAI_Update; // Implement IL hook to reemplace this hook
            //IL.GarbageWormAI.Update += GarbageWormAI_Update1;

            Plugin.LogInfo("WeaponsExtHooks finally apply");
        }

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

        //Not implemented yet
        private static void GarbageWormAI_Update1(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);
                if(false)
                {
                    //TO DO: Implement a if to avoid tracking no-magnetic objects
                    //The worm track the weapon in a if were check is the target object is a spear,
                    //if yes, so put them to be tracked and try to come near to this...
                    //so figure out somehow to put an aditional condition where only track magnetic spears
                }
                else
                {
                    UnityEngine.Debug.LogError("[slugcrafting] WeaponsExtHooks: Failed to apply GarbageWormAI_Update IL hook.");
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[slugcrafting] Error in GarbageWormAI_Update IL: {ex.Message}");
            }
        }

        //Depracted, should be delete when the IL is implemented
        private static bool show = false;
        private static void GarbageWormAI_Update(On.GarbageWormAI.orig_Update orig, GarbageWormAI self)
        {
            orig(self);
            if (!show)
            {
                //Logic to show this debug only 1 time per game, need to be improved
                show = true;
                Plugin.LogGameWarn("DEPRACTED FUNCTION, should reemplace for his IL hook in \"WeaponsExtHooks.cs\"");
            }
            if (self != null && self.worm != null && self.worm.grasps != null)
            {
                try
                {
                    //The worm always has a grasps place, but shold make sure if exits
                    if (self.worm.grasps[0] != null)
                    {
                        if(GarbajeWormTarget(self))
                        {
                            Plugin.LogGame("The object gras is not magnetic: {self.worm.grasps[0].grabbed}");
                            self.worm.LoseAllGrasps();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Plugin.LogGameWarn($"[{ex.Source}] Error in GarbageWormAI_Update: {ex.Message}");
                }
            }
        }

        // Sekq: Not sure if this should be in MRcustom with the archive "WeaponsExtHelper.cs"
        // Since magnetic field is from slugcrafing, but this is a ext function, so idk...
        private static bool GarbajeWormTarget(GarbageWormAI self)
        {
            return WeaponExtHelper.MagneticValue(self.worm.grasps[0].grabbed);
        }
    }
}
