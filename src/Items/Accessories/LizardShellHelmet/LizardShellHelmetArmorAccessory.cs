namespace SlugCrafting.Items.Accessories.LizardShellHelmet;

public class LizardShellHelmetArmorAccessory : ArmorAccessory
{
    public DynamicLizardShellHelmetCosmetic lizardShellHelmetCosmetic => (DynamicLizardShellHelmetCosmetic)cosmetics[0];

    //-- MR7: Constructor here forces a single cosmetic, the Lizard Shell Helmet Cosmetic, to be used with this armor accessory.
    public LizardShellHelmetArmorAccessory(DynamicLizardShellHelmetCosmetic lizardShellHelmetCosmetic, AccessoryArmorStats armorStats) : base(new DynamicCosmetic[]{ lizardShellHelmetCosmetic, }, armorStats)
    {
    }

    public override bool OnSpearHitWearer(Spear spear, SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.chunk.index == armorStats.wearingBodyChunkIndex)
            lizardShellHelmetCosmetic.lizardShellColorGraphics.WhiteFlicker();

        return base.OnSpearHitWearer(spear, result, eu);
    }

    public override void OnWearerViolence(ViolenceContext violenceContext)
    {
        if (violenceContext.hitChunk.index == armorStats.wearingBodyChunkIndex)
            lizardShellHelmetCosmetic.lizardShellColorGraphics.WhiteFlicker();

        base.OnWearerViolence(violenceContext);
    }
}
