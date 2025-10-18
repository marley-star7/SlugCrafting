using SlugCrafting.Items.WeaponsExtension;

namespace SlugCrafting.Creatures;

public class GarbageWormHooks
{
    public static void RemoveHooks()
    {
        On.GarbageWormAI.Update -= GarbageWormAI_Update;
        //IL.GarbageWormAI.Update -= GarbageWormAI_Update1;
    }
    public static void ApplyHooks()
    {
        On.GarbageWormAI.Update += GarbageWormAI_Update; // Implement IL hook to reemplace this hook
        //IL.GarbageWormAI.Update += GarbageWormAI_Update1;
    }

    //Not implemented yet
    private static void GarbageWormAI_Update1(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);
            if (false)
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
            if (GarbageWormTarget(self))
            {
                Plugin.LogGame($"The object grasps is not magnetic: {self.worm.grasps[0].grabbed}");
                self.worm.LoseAllGrasps();
            }
        }
    }

    // Sekq: Not sure if this should be in MRcustom with the archive "WeaponsExtHelper.cs"
    // Since magnetic field is from slugcrafing, but this is a ext function, so idk...
    private static bool GarbageWormTarget(GarbageWormAI self)
    {
        //The worm always has a grasps place, but shold make sure if exits
        if (self == null || self.worm.grasps[0] == null)
        {
            return false;
        }
        return WeaponExtHelper.MagneticValue(self.worm.grasps[0].grabbed);
    }
}
