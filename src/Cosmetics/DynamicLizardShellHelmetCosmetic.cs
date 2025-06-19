using UnityEngine;
using RWCustom;
using Noise;

using CompartmentalizedCreatureGraphics.SlugcatCosmetics;

namespace SlugCrafting.Cosmetics;

public class DynamicLizardShellHelmetCosmetic : DynamicSlugcatFullHeadAttachedCosmetic
{
    public float terrainImpactNoiseModifier = 3;

    private static float Rand => UnityEngine.Random.value;
    public override void OnWearerTerrainImpact(Player player, int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        var directionVec2 = new Vector2(direction.x, direction.y);

        if (!firstContact
            || chunk != player.firstChunk.index
            || Vector2.Dot(player.bodyChunks[chunk].Rotation, directionVec2) < 0f) //-- MR7: Only plays the noise if the helmet is roughly facing the same direction as the impact.
            return;

        var vol = Mathf.Clamp(speed * 0.07f, 0, 0.7f); //--MR7: Limit volume to not blow your ears off lol.
        var noiseStrength = Mathf.Clamp(speed * terrainImpactNoiseModifier, 0, 100f);

        player.room.PlaySound(SoundID.Spear_Fragment_Bounce, player.firstChunk, false, vol, UnityEngine.Random.Range(0.9f, 1.1f));
        player.room.InGameNoise(new InGameNoise(player.firstChunk.pos, noiseStrength, player, 1f));

        //-- MR7: Spawn just sparks, just using volume to determine intensity, which spawns more sparks...
        var numOfSparks = UnityEngine.Random.Range(0 + vol, 4 * vol);
        for (int k = 0; k < numOfSparks; k++)
        {
            Vector2 pos = player.firstChunk.pos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
            Vector2 vel = -directionVec2 * -0.1f + Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * speed;
            room.AddObject(new Spark(pos, vel, new Color(1f, 1f, 1f), null, 10, 170));
        }
    }
}
