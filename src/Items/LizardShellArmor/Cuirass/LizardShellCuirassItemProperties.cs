namespace SlugCrafting.Items;

public abstract class LizardShellCuirassItemProperties : LizardShellArmorItemProperties
{
    public static Dictionary<AbstractPhysicalObject.AbstractObjectType, LizardShellCuirassItemProperties> typesProperties = new();

    public LizardShellCuirassItemProperties(LizardShellCuirassAccessoryProperties accessoryProperties) : base(accessoryProperties)
    {

    }

    public static LizardShellCuirassItemProperties GetPropertiesForType(AbstractPhysicalObject.AbstractObjectType type)
    {
        if (LizardShellCuirassItemProperties.typesProperties.TryGetValue(type, out var itemProperties))
            return itemProperties;

        return null;
    }
}
