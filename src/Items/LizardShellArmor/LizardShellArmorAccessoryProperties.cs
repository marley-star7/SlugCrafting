namespace SlugCrafting.Items;

public abstract class LizardShellArmorAccessoryProperties : AccessoryProperties, IArmorAccessoryProperties, IGrabProtectionChanceAccessoryProperties, IDurabilityAccessoryProperties, ISpeedModifyingProperties
{
    protected SpriteLayerGroup[] spriteLayerGroups;
    public sealed override SpriteLayerGroup[] SpriteLayerGroups => spriteLayerGroups;

    protected SpriteEffectGroup[] spriteEffectGroups;
    public sealed override SpriteEffectGroup[] SpriteEffectGroups => spriteEffectGroups;

    public virtual Color DefaultShellColor => Color.green;

    public virtual float MaxHealth => 1;

    public virtual float Toughness => 1;
    public virtual float ExplosiveToughness => 1;

    public virtual float GrabProtectionChance => 1f;

    public virtual float RunSpeedLinearModifier => 0f;
    public virtual float PoleClimbSpeedMultiplier => 1f;
    public virtual float CorridorClimbSpeedMultiplier => 1f;

    public virtual float SwimForceMultiplier => 1f;
    public virtual float SwimBoostMultiplier => 1f;

    public virtual float GeneralVisibilityBonusMultiplier => 1f;
    public virtual float LoudnessMultiplier => 1f;
}
