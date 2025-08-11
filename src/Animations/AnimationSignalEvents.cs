namespace SlugCrafting.Animations;

public static class AnimationSignalEvents
{
    public static void OnKnapSpearImpact(Player player)
    {
        // Play the beat sound.
        player.room.PlaySound(SoundID.Spear_Bounce_Off_Wall, player.firstChunk.pos);
        player.room.InGameNoise(new InGameNoise(player.firstChunk.pos, 2500f, player, 1f));

        //- MS7 Vibrate the spear if it is a spear for extra pizazz.
        if (player.grasps[0].grabbed is Spear)
        {
            //- MS7 Vibration goes down over course of the beat time.
            // This is for thematics of mimicking the spear getting more shakey,
            // As well as for gameplay reasons showing acting as a minor visual indicator on how close to break.

            var vibrationMultiplierWithTime = Mathf.InverseLerp(0, player.GetPlayerCraftingData().currentPossibleCraft.Value.totalAnimationLoops, player.GetHandAnimationPlayer().timesLoopedCurrentAnimation);
            vibrationMultiplierWithTime = 1 / vibrationMultiplierWithTime + 0.01f; // Inverse it so that it goes from 1 to 0 over time, prevent divison by zero.
            vibrationMultiplierWithTime = Mathf.Clamp(vibrationMultiplierWithTime, 0.3f, 1f); // Clamp it to prevent it from going too low.

            ((Spear)player.grasps[0].grabbed).vibrate = (int)(10 * vibrationMultiplierWithTime);
        }
        return;
    }

    public static void OnKnapSpearBreakImpact(Player player)
    {
        // Play the beat sound.
        player.room.PlaySound(SoundID.Spear_Fragment_Bounce, player.firstChunk.pos);
        player.room.InGameNoise(new InGameNoise(player.firstChunk.pos, 5000f, player, 1f));

        //- MS7 Vibrate the spear if it is a spear for extra pizazz.
        if (player.grasps[0].grabbed is Spear)
        {
            //- MS7 Vibration goes down over course of the beat time.
            // This is for thematics of mimicking the spear getting more shakey,
            // As well as for gameplay reasons showing acting as a minor visual indicator on how close to break.

            var vibrationMultiplierWithTime = Mathf.InverseLerp(0, player.GetPlayerCraftingData().currentPossibleCraft.Value.totalAnimationLoops, player.GetHandAnimationPlayer().timesLoopedCurrentAnimation);
            vibrationMultiplierWithTime = 1 / vibrationMultiplierWithTime + 0.01f; // Inverse it so that it goes from 1 to 0 over time, prevent divison by zero.
            vibrationMultiplierWithTime = Mathf.Clamp(vibrationMultiplierWithTime, 0.3f, 1f); // Clamp it to prevent it from going too low.

            ((Spear)player.grasps[0].grabbed).vibrate = (int)(10 * vibrationMultiplierWithTime);
        }
        return;
    }

    public static void OnBiteStruggleNutFinish(Player player)
    {
        player.room.PlaySound(SoundID.Water_Nut_Swell, player.firstChunk.pos);
        player.room.InGameNoise(new InGameNoise(player.firstChunk.pos, 200f, player, 1f));
    }
}
