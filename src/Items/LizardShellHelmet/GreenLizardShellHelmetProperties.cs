using Fisobs.Properties;
using static CompartmentalizedCreatureGraphics.CCGEnums;
using static SlugCrafting.Items.LizardShellHelmet;

namespace SlugCrafting.Items;

public class GreenLizardShellHelmetProperties : LizardShellHelmetProperties
{
    private string spriteName = "greenLizardShellHelmet";
    public override LizardShellHelmet.SpriteInfo[] spritesInfo => new SpriteInfo[]
    {
        new SpriteInfo()
        {
            name = spriteName + "_Shell-1_",
            distanceFromHeadModifier = -0.2f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell0_",
            distanceFromHeadModifier = 0f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark0_",
            distanceFromHeadModifier = 0f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell0_",
            distanceFromHeadModifier = 0.3f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell1_",
            distanceFromHeadModifier = 0.6f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell2_",
            distanceFromHeadModifier = 1f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark2_",
            distanceFromHeadModifier = 1f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell3_",
            distanceFromHeadModifier = 1.1f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark3_",
            distanceFromHeadModifier = 1.1f
        }
    };

    public override SpriteLayerGroup[] spriteLayerGroups => new SpriteLayerGroup[]
    {
        new SpriteLayerGroup((int) CCGEnums.SlugcatCosmeticLayer.BaseHead, 0, 1),
        new SpriteLayerGroup((int) CCGEnums.SlugcatCosmeticLayer.FaceMask, 2, 8),
    };

    public override SpriteEffectGroup[] spriteEffectGroups => new SpriteEffectGroup[]
    {
        new SpriteEffectGroup(0,1,3,4,5,7),
        new SpriteEffectGroup(2,6,8),
        new SpriteEffectGroup()
    };

    public override float maxHealth => 5;
    /// -- MS7: Green lizard shell helmet is meant as the big slow juggernaut armor,
    /// The higher toughness is what reduces stun times so much with this helmet (as well as damage), letting you walk off hits.
    /// So this for example has a 25% damage resistance and 25% shorter stun times.
    public override float toughness => 1.25f;
    public override float grabProtectionChance => 0.2f;

    public override float runSpeedLinearModifier => -0.1f;
    public override float poleClimbSpeedMultiplier => 0.9f;
    public override float corridorClimbSpeedMultiplier => 0.9f;

    public override float swimForceMultiplier => 0.95f;
    public override float swimBoostMultiplier => 0.95f;

    public override float generalVisibilityBonusMultiplier => 0.5f;
    public override float loudnessMultiplier => 1.5f;
}
