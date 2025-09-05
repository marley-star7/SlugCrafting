using static SlugCrafting.Items.LizardShellHelmet;

namespace SlugCrafting.Items;

/// -- MS7: Green lizard shell helmet is meant as the big slow juggernaut armor,
/// The higher toughness is what reduces stun times so much with this helmet (as well as damage), letting you walk off hits.
/// So this for example has a 25% damage resistance and 25% shorter stun times.

public class GreenLizardShellHelmetAccessoryProperties : LizardShellHelmetAccessoryProperties
{
    public const string spriteName = "greenLizardShellHelmet";

    public GreenLizardShellHelmetAccessoryProperties()
    {
        spritesInfo = new SpriteInfo[]
        {
            new SpriteInfo(name: spriteName + "_Shell-1_", distanceFromHeadModifier: -0.2f),
            new SpriteInfo(name: spriteName + "_Shell0_", distanceFromHeadModifier: 0f),
            new SpriteInfo(name: spriteName + "_Dark0_", distanceFromHeadModifier: 0f),
            new SpriteInfo(name: spriteName + "_Shell0_", distanceFromHeadModifier: 0.3f),
            new SpriteInfo(name: spriteName + "_Shell1_", distanceFromHeadModifier: 0.6f),
            new SpriteInfo(name: spriteName + "_Shell2_", distanceFromHeadModifier: 1f),
            new SpriteInfo(name: spriteName + "_Dark2_", distanceFromHeadModifier: 1f),
            new SpriteInfo(name: spriteName + "_Shell3_", distanceFromHeadModifier: 1.1f),
            new SpriteInfo(name: spriteName + "_Dark3_", distanceFromHeadModifier: 1.1f)
        };

        spriteLayerGroups = new SpriteLayerGroup[]
        {
            new SpriteLayerGroup((int) CompartmentalizedCreatureGraphics.Enums.SlugcatCosmeticLayer.BaseHead, 0, 1),
            new SpriteLayerGroup((int) CompartmentalizedCreatureGraphics.Enums.SlugcatCosmeticLayer.FaceMask, 2, 8),
        };

        spriteEffectGroups = new SpriteEffectGroup[]
        {
            new SpriteEffectGroup(0,1,3,4,5,7),
            new SpriteEffectGroup(2,6,8),
            new SpriteEffectGroup()
        };
    }

    public override Color DefaultShellColor => Color.green;

    public override float MaxHealth => 5;

    public override float Toughness => 1.25f;
    public override float GrabProtectionChance => 0.2f;

    public override float RunSpeedLinearModifier => -0.1f;
    public override float PoleClimbSpeedMultiplier => 0.9f;
    public override float CorridorClimbSpeedMultiplier => 0.9f;

    public override float SwimForceMultiplier => 0.95f;
    public override float SwimBoostMultiplier => 0.95f;

    public override float GeneralVisibilityBonusMultiplier => 0.5f;
    public override float LoudnessMultiplier => 1.5f;
}
