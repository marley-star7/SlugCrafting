using static SlugCrafting.Items.LizardShellHelmet;

namespace SlugCrafting.Items;

public class BlueLizardShellHelmetAccessoryProperties : LizardShellHelmetAccessoryProperties
{
    public const string spriteName = "blueLizardShellHelmet";

    public BlueLizardShellHelmetAccessoryProperties()
    {
        spritesInfo = new SpriteInfo[]
        {
            new SpriteInfo(name: spriteName + "_Shell-0_", distanceFromHeadModifier: 0.1f),
            new SpriteInfo(name: spriteName + "_Shell0_", distanceFromHeadModifier: 0.1f),
            new SpriteInfo(name: spriteName + "_Dark0_", distanceFromHeadModifier: 0.1f),
            new SpriteInfo(name: spriteName + "_Shell1_", distanceFromHeadModifier: 0.3f),
            new SpriteInfo(name: spriteName + "_Dark1_", distanceFromHeadModifier: 0.3f),
            new SpriteInfo(name: spriteName + "_Shell2_", distanceFromHeadModifier: 0.6f),
            new SpriteInfo(name: spriteName + "_Dark2_", distanceFromHeadModifier: 0.6f),
            new SpriteInfo(name: spriteName + "_Shell3_", distanceFromHeadModifier: 0.9f),
            new SpriteInfo(name: spriteName + "_Dark3_", distanceFromHeadModifier: 0.9f),
            new SpriteInfo(name: spriteName + "_Shell4_", distanceFromHeadModifier: 1.1f),
            new SpriteInfo(name: spriteName + "_Dark4_", distanceFromHeadModifier: 1.1f)
        };

        spriteLayerGroups = new SpriteLayerGroup[]
        {
            new SpriteLayerGroup((int) CompartmentalizedCreatureGraphics.Enums.SlugcatCosmeticLayer.BaseHead, 0, 1),
            new SpriteLayerGroup((int) CompartmentalizedCreatureGraphics.Enums.SlugcatCosmeticLayer.FaceMask, 2, 9),
        };

        spriteEffectGroups = new SpriteEffectGroup[]
        {
            new SpriteEffectGroup(1,3,5,7,9),
            new SpriteEffectGroup(2,4,6,8,10),
            new SpriteEffectGroup(0)
        };
    }


    public override Color DefaultShellColor => Color.blue;

    public override float MaxHealth => 1;

    public override float GrabProtectionChance => 1f;
    public override float Toughness => 0.75f;

    public override float RunSpeedLinearModifier => 0f;
    public override float PoleClimbSpeedMultiplier => 1f;
    public override float CorridorClimbSpeedMultiplier => 1f;

    public override float SwimForceMultiplier => 1f;
    public override float SwimBoostMultiplier => 1f;

    public override float GeneralVisibilityBonusMultiplier => 0.3f;
    public override float LoudnessMultiplier => 1.15f;
}
