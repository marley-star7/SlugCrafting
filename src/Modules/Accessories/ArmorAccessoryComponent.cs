using MRCustom.Contexts;

namespace SlugCrafting.Modules.Accessories;

public class ArmorAccessoryModule : RWModule
{
    public new Accessory Owner => (Accessory)base.Owner;

    public IArmorAccessoryProperties armorAccessoryProperties;

    public ArmorAccessoryModule(Accessory owner, AccessoryProperties accessoryProperties) : base(owner, typeof(ArmorAccessoryModule))
    {
        this.armorAccessoryProperties = (IArmorAccessoryProperties)accessoryProperties;
    }

    //-- MS7: Have to run the code in here for blocking spear hits thanks to downpour and base game code.
    public bool PreSpearHitWearer(Spear spear, SharedPhysics.CollisionResult result, bool eu)
    {
        if (ValidateSpearHitAccessory(spear, result) == false)
            return true; // The spear acts as normal if not hit the armored chunk.

        PreSpearHitAccessoryModifications(spear, result);

        DoSpearLodge(spear);
        DoWearerSpearViolence(spear, result);

        return false;
    }

    public virtual bool ValidateSpearHitAccessory(Spear spear, SharedPhysics.CollisionResult result)
    {
        if (result.chunk.index == Owner.wearingBodyChunkIndex)
            return true; // The spear acts as normal if not hit the armored chunk.

        return false;
    }

    public virtual void DoSpearLodge(Spear spear)
    {
        // Spear bounces off.
        spear.WeaponDeflect(
            Owner.wearingBodyChunk.pos, 
            (spear.firstChunk.vel.normalized + Vector2.up).normalized,
            spear.firstChunk.vel.magnitude * 0.3f);
    }

    public virtual void DoWearerSpearViolence(Spear spear, SharedPhysics.CollisionResult result)
    {
        // Do violence to player to stun them.
        var durabilityDamage = spear.spearDamageBonus / armorAccessoryProperties.Toughness;

        var damage = 0f;
        var stunBonus = MarCreatureExtensions.ConvertDamageToStunBonus(durabilityDamage);
        var directionAndMomentum = -spear.firstChunk.vel;

        Owner.Wearer.Violence(spear.firstChunk, directionAndMomentum, result.chunk, null, Creature.DamageType.Blunt, damage, stunBonus);
    }

    public virtual void PreSpearHitAccessoryModifications(Spear spear, SharedPhysics.CollisionResult result)
    {

    }

    public void PreWearerViolence(ViolenceContext violenceContext)
    {
        if (violenceContext.hitChunk == null || violenceContext.hitChunk.index != Owner.wearingBodyChunkIndex)
            return; // This accessory does not modify player.

        PreViolenceContextModifications(violenceContext);

        ModifyViolenceContextDamageType(violenceContext);
        ModifyViolenceContextStunBonus(violenceContext);
        ModifyViolenceContextDamage(violenceContext);
    }

    public virtual void PreViolenceContextModifications(ViolenceContext violenceContext)
    {

    }

    public virtual void ModifyViolenceContextDamageType(ViolenceContext violenceContext)
    {
        if (violenceContext.type == Creature.DamageType.Bite || violenceContext.type == Creature.DamageType.Stab)
        {
            violenceContext.type = Creature.DamageType.Blunt;
        }
    }

    public virtual void ModifyViolenceContextDamage(ViolenceContext violenceContext)
    {
        violenceContext.damage = 0f; // As long as there is any bit of helmet left on you, no damage is dealt.
    }

    public virtual void ModifyViolenceContextStunBonus(ViolenceContext violenceContext)
    {
        var durabilityDamage = violenceContext.damage / armorAccessoryProperties.Toughness;

        violenceContext.stunBonus += MarCreatureExtensions.ConvertDamageToStunBonus(durabilityDamage);
    }

    protected Vector2 GetDirectionAndMomentumFromViolenceContext(in ViolenceContext violenceContext)
    {
        Vector2 directionAndMomentum;
        if (violenceContext.directionAndMomentum == null)
        {
            //-- MS7: Have to check source, necessary bug fix for explosive spears which can destory themselves.
            if (violenceContext.source == null)
                return Vector2.zero; // No source, no direction and momentum.

            //-- MS7: If the direction and momentum is not set, then we use the source position and hit chunk position to calculate some roughly close value.
            directionAndMomentum = violenceContext.source.pos - violenceContext.hitChunk.pos;
        }
        else
            directionAndMomentum = violenceContext.directionAndMomentum.Value;

        return directionAndMomentum;
    }
}
