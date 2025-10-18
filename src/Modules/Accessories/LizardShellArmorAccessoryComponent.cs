
using SlugCrafting.Items;

namespace SlugCrafting.Modules.Accessories;

public class LizardShellArmorAccessoryModule : ArmorAccessoryModule
{
    public new LizardShellArmorAccessory Owner => (LizardShellArmorAccessory)base.Owner;

    public LizardShellArmorAccessoryModule(LizardShellArmorAccessory owner) : base(owner, owner.AccessoryProperties)
    {

    }

    public override void PreSpearHitAccessoryModifications(Spear spear, SharedPhysics.CollisionResult result)
    {
        var durabilityDamage = spear.spearDamageBonus / Owner.lizardShellArmor.AccessoryProperties.Toughness;
        Owner.TakeRawDurabilityDamage(durabilityDamage);
    }

    public override void PreViolenceContextModifications(ViolenceContext violenceContext)
    {
        var durabilityDamage = violenceContext.damage / Owner.AccessoryProperties.Toughness;
        Owner.TakeRawDurabilityDamage(durabilityDamage);

        if (violenceContext.hitChunk == null)
            return;

        Vector2 effectsSpawnPos;
        if (violenceContext.source != null)
        {
            effectsSpawnPos = violenceContext.source.pos;
        }
        else
        {
            effectsSpawnPos = violenceContext.hitChunk.pos;
        }

        Owner.lizardShellArmor.DoDeflectEffects(violenceContext.hitChunk, effectsSpawnPos, GetDirectionAndMomentumFromViolenceContext(violenceContext), violenceContext.damage, violenceContext.stunBonus);
    }
}
