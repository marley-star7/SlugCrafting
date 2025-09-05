namespace SlugCrafting.Items;

public abstract class LizardShellArmorAccessory : Accessory
{
    public LizardShellArmor lizardShellArmor;

    public LizardShellArmorItemProperties ItemProperties => lizardShellArmor.ItemProperties;
    public LizardShellArmorAccessoryProperties AccessoryProperties => lizardShellArmor.AccessoryProperties;

    public byte linearRunSpeedModifierId = 0;

    public BodyChunk wearingBodyChunk => wearer.bodyChunks[wearingBodyChunkIndex];

    public LizardShellArmorAccessory(Player owner, LizardShellArmor lizardShellArmor, int wearingBodyChunkIndex) : base(owner)
    {
        this.lizardShellArmor = lizardShellArmor;
        spriteLayerGroups = lizardShellArmor.spriteLayerGroups;

        this.wearingBodyChunkIndex = wearingBodyChunkIndex;

        owner.graphicsModule.AddCreatureCosmetic(this);
        linearRunSpeedModifierId = owner.GetPlayerMarData().runSpeedLinearModifiers.AddModifier(new MarPlayerData.RunSpeedLinearModifier(AccessoryProperties.RunSpeedLinearModifier));
    }

    public override void Destroy()
    {
        base.Destroy();

        var wearerCraftingData = owner.GetPlayerCraftingData();
        wearerCraftingData.accessories.Remove(this);

        owner.graphicsModule.RemoveCreatureCosmetic(this);
        owner.graphicsModule.ReorderDynamicCosmetics();
        owner.GetPlayerMarData().runSpeedLinearModifiers.RemoveModifier(linearRunSpeedModifierId);
    }

    public override void Update(bool eu)
    {
        lizardShellArmor.Update(eu);
    }

    private void TakeRawDurabilityDamage(float damage)
    {
        lizardShellArmor.AbstractLizardShellArmor.health -= damage;

        if (lizardShellArmor.AbstractLizardShellArmor.health <= 0)
        {
            lizardShellArmor.Shatter(this, wearingBodyChunk.pos);
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
        var durabilityDamage = spear.spearDamageBonus / lizardShellArmor.AccessoryProperties.Toughness;
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

        if (violenceContext.type == Creature.DamageType.Bite || violenceContext.type == Creature.DamageType.Stab)
        {
            violenceContext.type = Creature.DamageType.Blunt;
        }

        var durabilityDamage = violenceContext.damage / AccessoryProperties.Toughness;
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

        lizardShellArmor.DoDeflectEffects(wearer.bodyChunks[wearingBodyChunkIndex], violenceContext.hitChunk.pos, directionAndMomentum, violenceContext.damage, violenceContext.stunBonus);
    }

    public override void PostWearerGrabbed(Creature.Grasp grasp)
    {
        //
        // Check if this attack was a grab on our wearing chunk, if so we free ourselves from it.
        //

        if (grasp.grabbedChunk != wearingBodyChunk)
            return;

        if (Random.value < AccessoryProperties.GrabProtectionChance)
        {
            grasp.Release();

            // Spawn item version of accessory.
            lizardShellArmor.AbstractLizardShellArmor.RealizeInRoom();
            // Grabber grabs that.
            Creature grabber = grasp.grabber;
            if (grabber is Lizard lizard)
                lizard.GrabInanimate(lizardShellArmor.AbstractLizardShellArmor.realizedObject.firstChunk);

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
        lizardShellArmor.InitiateSprites(sLeaser, rCam);

        if (wearerGraphics != null)
        {
            wearerGraphics.ReorderDynamicCosmetics();
        }
    }

    public override void PostWearerApplyPalette(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, in RoomPalette palette)
    {
        lizardShellArmor.ApplyPalette(_sLeaser, rCam, palette);
    }
}
