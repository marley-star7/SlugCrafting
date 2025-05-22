using MRCustom.Animations;

namespace SlugCrafting;

public class Enums
{
    public class HandAnimationIndex
    {
        // CRAFT ANIMATIONS

        public static PlayerHandAnimationData.HandAnimationIndex? DoubleSwallowCraft = null;
        public static PlayerHandAnimationData.HandAnimationIndex? SmashIntoCraft = null;

        // SCAVENGE ANIMATIONS

        public static PlayerHandAnimationData.HandAnimationIndex? SawBackForthScavenge = null;

        public static void RegisterValues()
        {
            // CRAFT ANIMATIONS

            DoubleSwallowCraft = new PlayerHandAnimationData.HandAnimationIndex("DoubleSwallowCraft", true);
            SmashIntoCraft = new PlayerHandAnimationData.HandAnimationIndex("SmashIntoCraft", true);

            // SCAVENGE ANIMATIONS

            SawBackForthScavenge = new PlayerHandAnimationData.HandAnimationIndex("SawBackForthScavenge", true);
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
