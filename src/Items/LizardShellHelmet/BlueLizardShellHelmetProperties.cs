using Fisobs.Properties;
using static SlugCrafting.Items.LizardShellHelmet;

namespace SlugCrafting.Items;

public class BlueLizardShellHelmetProperties : LizardShellHelmetProperties
{
    private string spriteName = "blueLizardShellHelmet";
    public override LizardShellHelmet.SpriteInfo[] spritesInfo => new SpriteInfo[]
    {
        new SpriteInfo()
        {
            name = spriteName + "_Shell-0_",
            distanceFromHeadModifier = 0.1f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell0_",
            distanceFromHeadModifier = 0.1f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark0_",
            distanceFromHeadModifier = 0.1f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell1_",
            distanceFromHeadModifier = 0.3f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark1_",
            distanceFromHeadModifier = 0.3f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell2_",
            distanceFromHeadModifier = 0.6f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark2_",
            distanceFromHeadModifier = 0.6f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell3_",
            distanceFromHeadModifier = 0.9f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark3_",
            distanceFromHeadModifier = 0.9f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Shell4_",
            distanceFromHeadModifier = 1.1f
        },
        new SpriteInfo()
        {
            name = spriteName + "_Dark4_",
            distanceFromHeadModifier = 1.1f
        },
    };

    public override SpriteLayerGroup[] spriteLayerGroups => new SpriteLayerGroup[]
    {
        new SpriteLayerGroup((int) CCGEnums.SlugcatCosmeticLayer.BaseHead, 0, 1),
        new SpriteLayerGroup((int) CCGEnums.SlugcatCosmeticLayer.FaceMask, 2, 9),
    };

    public override SpriteEffectGroup[] spriteEffectGroups => new SpriteEffectGroup[]
    {
        new SpriteEffectGroup(1,3,5,7,9),
        new SpriteEffectGroup(2,4,6,8,10),
        new SpriteEffectGroup(0)
    };

    public override Color defaultShellColor => Color.blue;

    public override float maxHealth => 1;

    public override float grabProtectionChance => 1f;
    public override float toughness => 0.75f;

    public override float runSpeedLinearModifier => 0f;
    public override float poleClimbSpeedMultiplier => 1f;
    public override float corridorClimbSpeedMultiplier => 1f;

    public override float swimForceMultiplier => 1f;
    public override float swimBoostMultiplier => 1f;

    public override float generalVisibilityBonusMultiplier => 0.3f;
    public override float loudnessMultiplier => 1.15f;
}
