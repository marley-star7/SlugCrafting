using static SlugCrafting.Items.LizardShellCuirass;

namespace SlugCrafting.Items;

public class GreenLizardShellCuirassAccessoryProperties : LizardShellCuirassAccessoryProperties
{
    public const string bodySpriteName = "greenLizardShellCuirassBody";

    public const string hipsSpriteName = "greenLizardShellCuirassHips";

    public override LizardShellCuirass.SpriteInfo[] BodySpritesInfo => new SpriteInfo[]
    {
        new SpriteInfo(name: bodySpriteName + "_Shell0_"),
        new SpriteInfo(name: bodySpriteName + "_Dark0_")
    };

    public override LizardShellCuirass.SpriteInfo[] HipsSpritesInfo => new SpriteInfo[]
    {
        new SpriteInfo(name: hipsSpriteName + "_Shell0_")
    };

    public GreenLizardShellCuirassAccessoryProperties()
    {
        spriteLayerGroups = new SpriteLayerGroup[]
        {
            new SpriteLayerGroup(Enums.CosmeticLayers.Cuirass, 0, 2),
        };

        spriteEffectGroups = new SpriteEffectGroup[]
        {
            new SpriteEffectGroup(0, 2),
            new SpriteEffectGroup(1),
            new SpriteEffectGroup()
        };
    }
}
