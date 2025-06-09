using MRCustom.Animations;

namespace SlugCrafting;

public static class Enums
{
    public static readonly SlugcatStats.Name Crafter = new(nameof(Crafter), false);

    //-- MR7: TODO: probably move the abstractobjecttypes from fisobs to here.

    public class HandAnimationIndex
    {
        // CRAFT ANIMATIONS

        public static PlayerHandAnimationPlayer.HandAnimationIndex? DoubleSwallowCraft = null;
        public static PlayerHandAnimationPlayer.HandAnimationIndex? SmashIntoCraft = null;

        // SCAVENGE ANIMATIONS

        public static PlayerHandAnimationPlayer.HandAnimationIndex? SawBackForthScavenge = null;

        public static void RegisterValues()
        {
            // CRAFT ANIMATIONS

            DoubleSwallowCraft = new PlayerHandAnimationPlayer.HandAnimationIndex("DoubleSwallowCraft", true);
            SmashIntoCraft = new PlayerHandAnimationPlayer.HandAnimationIndex("SmashIntoCraft", true);

            // SCAVENGE ANIMATIONS

            SawBackForthScavenge = new PlayerHandAnimationPlayer.HandAnimationIndex("SawBackForthScavenge", true);
        }

        public static void UnregisterValues()
        {
            // CRAFT ANIMATIONS

            if (DoubleSwallowCraft != null) 
            {
                DoubleSwallowCraft.Unregister();
                DoubleSwallowCraft = null; 
            }
            if (SmashIntoCraft != null)
            {
                SmashIntoCraft.Unregister();
                SmashIntoCraft = null;
            }

            // SCAVENGE ANIMATIONS

            if (SawBackForthScavenge != null) 
            {
                SawBackForthScavenge.Unregister();
                SawBackForthScavenge = null; 
            }
        }
    }
}
