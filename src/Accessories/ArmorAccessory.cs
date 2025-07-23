using MRCustom;
using MRCustom.Contexts;
using SlugCrafting.Items.Accessories.LizardShellHelmet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Accessories;

public class ArmorAccessory : Accessory
{
    public float terrainImpactNoiseModifier = 3;

    public ArmorAccessory(DynamicCosmetic[] cosmetics, AccessoryArmorStats armorStats) : base(cosmetics, armorStats)
    {
    }

    private static float Rand => Random.value;

    private void SpawnSparks(Vector2 sourcePos, Vector2 directionAndMomentum, int sparkNum)
    {
        Color sparkColor;
        if (cosmetics[0] is DynamicLizardShellHelmetCosmetic lizardShellHelmetCosmetic)
        {
            lizardShellHelmetCosmetic.lizardShellColorGraphics.WhiteFlicker();
            sparkColor = lizardShellHelmetCosmetic.lizardShellColorGraphics.ShellColor(armorStats.health, armorStats.maxHealth);
        }
        else
            sparkColor = Color.white;

        for (int k = 0; k < sparkNum; k++)
        {
            //-- MR7: Figure out how to make sparks have the lizard graphics thing where they change color, without NEEDING lizard graphics.
            Vector2 pos = sourcePos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
            Vector2 vel = -directionAndMomentum * -0.1f + Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * directionAndMomentum.magnitude;
            wearer.room.AddObject(new Spark(pos, vel, sparkColor, null, 10, 170));
        }
    }

    private void DoDeflectEffects(Vector2 sourcePos, Vector2 directionAndMomentum)
    {
        //-- MR7: Required visual and audio queue for deflecting a hit.
        Color sparkColor;

        SpawnSparks(sourcePos, directionAndMomentum, Random.Range(3, 8));
        wearer.room.AddObject(new StationaryEffect(sourcePos, new Color(1f, 1f, 1f), null, StationaryEffect.EffectType.FlashingOrb));
        wearer.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, wearer.bodyChunks[armorStats.wearingBodyChunkIndex]);
    }

    //-- MR7: Have to run the code in here for blocking spear hits thanks to downpour and base game code.
    public override bool OnSpearHitWearer(Spear spear, SharedPhysics.CollisionResult result, bool eu)
    {
        if (armorStats == null ||
        result.chunk.index != armorStats.wearingBodyChunkIndex)
            return true; // This accessory does not modify player.

        armorStats.health -= spear.spearDamageBonus;

        spear.vibrate = 20;
        spear.ChangeMode(Weapon.Mode.Free);
        spear.firstChunk.vel = spear.firstChunk.vel * -0.5f + Custom.DegToVec(Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, Random.value) * spear.firstChunk.vel.magnitude;
        spear.SetRandomSpin();

        DoDeflectEffects(result.collisionPoint, spear.firstChunk.vel);
        return false;
    }

    public override void OnWearerGrabbed(Creature.Grasp grasp)
    {
        //
        // Check if this attack was a grab, and if we free ourselves for it.
        //

        if (Random.value < armorStats.grabProtectionChance)
            grasp.Release();

        //grasp.grabber.Grab();
    }

    public override void OnWearerViolence(ViolenceContext violenceContext)
    {
        if (armorStats == null ||
        violenceContext.hitChunk == null ||
        violenceContext.hitChunk.index != armorStats.wearingBodyChunkIndex)
            return; // This accessory does not modify player.

        //
        // Deflection of damage.
        //

        armorStats.health -= violenceContext.damage;
        violenceContext.damage = 0f; // No damage dealt.

        Vector2 directionAndMomentum;
        if (violenceContext.directionAndMomentum == null)
        {
            //-- MR7: Have to check source, necessary bug fix for explosive spears which can destory themselves.
            if (violenceContext.source == null)
                return; // No source, no direction and momentum.

            //-- MR7: If the direction and momentum is not set, then we use the source position and hit chunk position to calculate some roughly close value.
            directionAndMomentum = violenceContext.source.pos - violenceContext.hitChunk.pos;
        }
        else
            directionAndMomentum = violenceContext.directionAndMomentum.Value;

        DoDeflectEffects(violenceContext.hitChunk.pos, directionAndMomentum);
    }

    public override void OnWearerTerrainImpact(TerrainImpactContext impactContext)
    {
        var directionVec2 = new Vector2(impactContext.direction.x, impactContext.direction.y);
        var impactChunk = wearer.bodyChunks[impactContext.chunkIndex];

        if (!impactContext.firstContact
            || impactContext.chunkIndex != armorStats.wearingBodyChunkIndex
            || Vector2.Dot(wearer.bodyChunks[impactContext.chunkIndex].Rotation, directionVec2) < 0f) //-- MR7: Only plays the noise if the helmet is roughly facing the same direction as the impact.
            return;

        var vol = Mathf.Clamp(impactContext.speed * 0.07f, 0, 0.7f); //--MR7: Limit volume to not blow your ears off lol.
        var noiseStrength = Mathf.Clamp(impactContext.speed * terrainImpactNoiseModifier, 0, 100f);

        wearer.room.PlaySound(SoundID.Spear_Fragment_Bounce, impactChunk, false, vol, UnityEngine.Random.Range(0.8f, 1.2f));
        wearer.room.InGameNoise(new InGameNoise(impactChunk.pos, noiseStrength, wearer, 1f));
        int sparkNum = (int)UnityEngine.Random.Range(vol * 2, vol * 7);

        SpawnSparks(impactChunk.pos, directionVec2 * impactContext.speed, sparkNum);
    }
}
