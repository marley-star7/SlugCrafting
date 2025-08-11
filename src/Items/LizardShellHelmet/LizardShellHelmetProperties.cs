using static CompartmentalizedCreatureGraphics.CCGEnums;

namespace SlugCrafting.Items;

public class LizardShellHelmetProperties : ItemProperties
{
    public static Dictionary<AbstractPhysicalObject.AbstractObjectType, LizardShellHelmetProperties> typesProperties = new();

    public virtual LizardShellHelmet.SpriteInfo[] spritesInfo => new LizardShellHelmet.SpriteInfo[0];
    public virtual SpriteLayerGroup[] spriteLayerGroups => new SpriteLayerGroup[0];
    public virtual SpriteEffectGroup[] spriteEffectGroups => new SpriteEffectGroup[0];

    public virtual Color defaultShellColor => Color.green;

    /// <summary>
    /// How much damage the helmet can take before shattering.
    /// </summary>
    public virtual float maxHealth => 1;
    /// <summary>
    /// The modifier to any damage done to this helmet, how resistant it is to damage.
    /// Also due to damage logic, effects how long stuns last for.
    /// </summary>
    public virtual float toughness => 1;
    /// <summary>
    /// The modifier to any explosive damage done to this helmet, how resistant it is to explosive damage.
    /// (Stacks with normal toughness)
    /// </summary>
    public virtual float explosiveToughness => 1;
    /// <summary>
    /// The chance this accessory will be grabbed instead of the player, saving player from grab.
    /// </summary>
    public virtual float grabProtectionChance => 1f;

    public virtual float runSpeedLinearModifier => 0f;
    public virtual float poleClimbSpeedMultiplier => 1f;
    public virtual float corridorClimbSpeedMultiplier => 1f;

    public virtual float swimForceMultiplier => 1f;
    public virtual float swimBoostMultiplier => 1f;

    public virtual float generalVisibilityBonusMultiplier => 1f;
    public virtual float loudnessMultiplier => 1f;

    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 3;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        => grabability = Player.ObjectGrabability.OneHand;
}
