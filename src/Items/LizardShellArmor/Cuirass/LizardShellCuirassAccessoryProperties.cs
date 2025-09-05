namespace SlugCrafting.Items;

public abstract class LizardShellCuirassAccessoryProperties : LizardShellArmorAccessoryProperties
{
    public abstract LizardShellCuirass.SpriteInfo[] BodySpritesInfo { get; }

    public abstract LizardShellCuirass.SpriteInfo[] HipsSpritesInfo { get; }

    public LizardShellCuirassAccessoryProperties()
    {
        spriteLayerGroups = new SpriteLayerGroup[]
        {
            new SpriteLayerGroup(Enums.CosmeticLayers.Cuirass, 0),
        };

        spriteEffectGroups = new SpriteEffectGroup[]
        {
            new SpriteEffectGroup(0),
            new SpriteEffectGroup(),
            new SpriteEffectGroup()
        };
    }
}
