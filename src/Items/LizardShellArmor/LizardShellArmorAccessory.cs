using MRCustom.Modules.PhysicalObjects;

namespace SlugCrafting.Items;

public abstract class LizardShellArmorAccessory : Accessory
{
    public LizardShellArmor lizardShellArmor;

    public PhysicalObjectRepresentationAccessoryModule physicalObjectRepresentationModule;
    public ArmorAccessoryModule armorModule;
    public LizardShellEffectsModule lizardShellEffectsModule;

    public LizardShellArmorItemProperties ItemProperties => lizardShellArmor.ItemProperties;
    public LizardShellArmorAccessoryProperties AccessoryProperties => lizardShellArmor.AccessoryProperties;

    public byte linearRunSpeedModifierId = 0;

    public new Player wearer;

    public LizardShellArmorAccessory(Player wearer, LizardShellArmor lizardShellArmor, int wearingBodyChunkIndex) : base(wearer)
    {
        this.wearer = wearer;
        this.lizardShellArmor = lizardShellArmor;
        SpriteLayerGroups = lizardShellArmor.spriteLayerGroups;

        this.wearingBodyChunkIndex = wearingBodyChunkIndex;

        wearer.graphicsModule.AddCreatureCosmetic(this);
        linearRunSpeedModifierId = wearer.GetMarPlayerData().runSpeedLinearModifiers.AddModifier(new MarPlayerData.RunSpeedLinearModifier(AccessoryProperties.RunSpeedLinearModifier));

        //physicalObjectRepresentationModule = new PhysicalObjectRepresentationAccessoryModule(this, lizardShellArmor.AbstractLizardShellArmor);
        //AddModule(physicalObjectRepresentationModule);

        armorModule = new LizardShellArmorAccessoryModule(this);
        AddModule(armorModule);

        lizardShellEffectsModule = lizardShellArmor.lizardShellEffectsModule;
        AddModule(lizardShellEffectsModule);
    }

    public override void Destroy()
    {
        lizardShellEffectsModule.DoShatterEffects(wearingBodyChunk.pos);

        var wearerCraftingData = wearer.GetPlayerCraftingData();
        wearerCraftingData.accessories.Remove(this);

        Wearer.graphicsModule.RemoveCreatureCosmetic(this);
        Wearer.graphicsModule.ReorderDynamicCosmetics();
        wearer.GetMarPlayerData().runSpeedLinearModifiers.RemoveModifier(linearRunSpeedModifierId);

        base.Destroy();
    }

    public override void Update(bool eu)
    {
        lizardShellArmor.Update(eu);
    }

    public void TakeRawDurabilityDamage(float damage)
    {
        lizardShellArmor.AbstractLizardShellArmor.health -= damage;

        if (lizardShellArmor.AbstractLizardShellArmor.health <= 0)
        {
            lizardShellArmor.Shatter(this, wearingBodyChunk.pos);
        }
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

        if (WearerGraphics != null)
        {
            WearerGraphics.ReorderDynamicCosmetics();
        }
    }

    public override void PostWearerApplyPalette(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, in RoomPalette palette)
    {
        lizardShellArmor.ApplyPalette(_sLeaser, rCam, palette);
    }
}
