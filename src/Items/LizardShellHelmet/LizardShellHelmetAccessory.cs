using CompartmentalizedCreatureGraphics.Extensions;
using MRCustom.Contexts;
using System;

namespace SlugCrafting.Items;

public class LizardShellHelmetAccessory : Accessory
{
    public LizardShellHelmet lizardShellHelmet;
    public LizardShellHelmetProperties properties => lizardShellHelmet.properties;

    public byte linearRunSpeedModifierId = 0;

    public BodyChunk wearingBodyChunk => wearer.bodyChunks[wearingBodyChunkIndex];

    public LizardShellHelmetAccessory(Player owner, LizardShellHelmet lizardShellHelmet) : base(owner)
    {
        this.lizardShellHelmet = lizardShellHelmet;
        spriteLayerGroups = lizardShellHelmet.spriteLayerGroups;

        wearingBodyChunkIndex = 0;

        owner.graphicsModule.AddCreatureCosmetic(this);
        linearRunSpeedModifierId = owner.GetPlayerMarData().runSpeedLinearModifiers.AddModifier(new MarPlayerData.RunSpeedLinearModifier(properties.runSpeedLinearModifier));
    }

    public override void Destroy()
    {
        base.Destroy();

        var wearerCraftingData = owner.GetPlayerCraftingData();
        wearerCraftingData.accessories.Remove(this);

        owner.graphicsModule.RemoveCreatureCosmetic(this);
        owner.GetPlayerMarData().runSpeedLinearModifiers.RemoveModifier(linearRunSpeedModifierId);
    }

    public override void Update(bool eu)
    {
        lizardShellHelmet.Update(eu);
    }

    private void TakeRawDurabilityDamage(float damage)
    {
        lizardShellHelmet.abstractLizardShellHelmet.health -= damage;

        if (lizardShellHelmet.abstractLizardShellHelmet.health <= 0)
        {
            lizardShellHelmet.Shatter(this, wearingBodyChunk.pos);
        }
    }

    //-- MS7: Have to run the code in here for blocking spear hits thanks to downpour and base game code.
    public override bool PreSpearHitWearer(Spear spear, SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.chunk.index != wearingBodyChunkIndex)
            return true; // The spear acts as normal if not hit the armored chunk.

        // Spear bounces off.
        spear.vibrate = 20;
        spear.ChangeMode(Weapon.Mode.Free);
        spear.firstChunk.vel = spear.firstChunk.vel * -0.5f + Custom.DegToVec(Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, Random.value) * spear.firstChunk.vel.magnitude;
        spear.SetRandomSpin();

        // Do violence to player to stun them.
        var durabilityDamage = spear.spearDamageBonus / lizardShellHelmet.properties.toughness;
        TakeRawDurabilityDamage(durabilityDamage);

        var damage = 0f;
        var stunBonus = MarCreatureExtensions.ConvertDamageToStunBonus(durabilityDamage);
        var directionAndMomentum = -spear.firstChunk.vel;

        wearer.Violence(spear.firstChunk, directionAndMomentum, result.chunk, null, Creature.DamageType.Blunt, damage, stunBonus);

        return false;
    }

    public override void PreWearerViolence(ViolenceContext violenceContext)
    {
        if (violenceContext.hitChunk == null || violenceContext.hitChunk.index != wearingBodyChunkIndex)
            return; // This accessory does not modify player.

        //
        // Deflection of damage.
        //

        if(violenceContext.type == Creature.DamageType.Bite || violenceContext.type == Creature.DamageType.Stab)
        {
            violenceContext.type = Creature.DamageType.Blunt;
        }

        var durabilityDamage = violenceContext.damage / lizardShellHelmet.properties.toughness;
        TakeRawDurabilityDamage(durabilityDamage);

        violenceContext.damage = 0f; // As long as there is any bit of helmet left on you, no damage is dealt.
        violenceContext.stunBonus += MarCreatureExtensions.ConvertDamageToStunBonus(durabilityDamage);

        Vector2 directionAndMomentum;
        if (violenceContext.directionAndMomentum == null)
        {
            //-- MS7: Have to check source, necessary bug fix for explosive spears which can destory themselves.
            if (violenceContext.source == null)
                return; // No source, no direction and momentum.

            //-- MS7: If the direction and momentum is not set, then we use the source position and hit chunk position to calculate some roughly close value.
            directionAndMomentum = violenceContext.source.pos - violenceContext.hitChunk.pos;
        }
        else
            directionAndMomentum = violenceContext.directionAndMomentum.Value;

        lizardShellHelmet.DoDeflectEffects(wearer.bodyChunks[wearingBodyChunkIndex], violenceContext.hitChunk.pos, directionAndMomentum, violenceContext.damage, violenceContext.stunBonus);
    }

    public override void PostWearerTerrainImpact(Player player, int chunkIndex, IntVector2 direction, float speed, bool firstContact)
    {
        var impactChunk = wearer.bodyChunks[chunkIndex];
        var directionVec2 = new Vector2(direction.x, direction.y);

        if (!firstContact
            || chunkIndex != wearingBodyChunkIndex)
            return;

        //-- MS7: Only do terrain impact effects when worn if the impact direction is angled roughly the same as the helmet.
        if (Vector2.Dot(wearer.bodyChunks[chunkIndex].Rotation, directionVec2) < 0f)
            return;

        lizardShellHelmet.DoTerrainImpactEffects(impactChunk, directionVec2, speed, firstContact);
    }

    public override void PostWearerGrabbed(Creature.Grasp grasp)
    {
        //
        // Check if this attack was a grab on our wearing chunk, if so we free ourselves from it.
        //

        if (grasp.grabbedChunk != wearingBodyChunk)
            return;

        if (Random.value < properties.grabProtectionChance)
        {
            grasp.Release();

            // Spawn helmet version.
            lizardShellHelmet.abstractLizardShellHelmet.RealizeInRoom();
            // Grabber grabs that.
            Creature grabber = grasp.grabber;
            if (grabber is Lizard lizard)
                lizard.GrabInanimate(lizardShellHelmet.abstractLizardShellHelmet.realizedObject.firstChunk);

            wearer.room.PlaySound(SoundID.Spear_Fragment_Bounce, wearingBodyChunk);

            // Destroy accessory
            this.Destroy();
        }
    }

    //
    // Drawing
    //

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        this._sLeaser = sLeaser;
        lizardShellHelmet.InitiateSprites(sLeaser, rCam);

        if (wearerGraphics != null)
        {
            wearerGraphics.ReorderDynamicCosmetics();
        }
    }

    public override void PostWearerDrawSprites(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var playerGraphics = (PlayerGraphics)wearer.graphicsModule;
        var playerGraphicsCCGData = playerGraphics.GetPlayerGraphicsCCGData();

        if (playerGraphicsCCGData.sLeaser == null)
            return;

        //-- MS7: To achieve the effect of being behind we make get an offset from face angle different to position the head.
        var lookDirX = playerGraphicsCCGData.BaseFaceSprite.x - playerGraphicsCCGData.BaseHeadSprite.x;
        var lookDirY = playerGraphicsCCGData.BaseFaceSprite.y - playerGraphicsCCGData.BaseHeadSprite.y;

        var faceRotationTimeStacked = Vector2.Lerp(playerGraphicsCCGData.lastFaceRotation, playerGraphicsCCGData.faceRotation, timeStacker);
        var rot = Custom.VecToDeg(faceRotationTimeStacked);

        var helmetRotation = (rot + playerGraphicsCCGData.BaseHeadSprite.rotation) / 2;

        LizardShellHelmet.DrawSpritesContext context = new LizardShellHelmet.DrawSpritesContext(
            playerGraphicsCCGData.BaseHeadSprite.x, playerGraphicsCCGData.BaseHeadSprite.y,
            playerGraphicsCCGData.BaseFaceSprite.scaleX, playerGraphicsCCGData.BaseFaceSprite.scaleY,
            rot,
            playerGraphicsCCGData.faceSpriteAngleAsymmetrical,
            lookDirX, lookDirY
        );

        lizardShellHelmet.DrawSprites(sLeaser, rCam, timeStacker, camPos, context);
    }

    public override void PostWearerApplyPalette(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, in RoomPalette palette)
    {
        lizardShellHelmet.ApplyPalette(_sLeaser, rCam, palette);
    }
}
