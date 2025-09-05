namespace SlugCrafting.Items;

public abstract class LizardShellHelmetItemProperties : LizardShellArmorItemProperties
{
    public static Dictionary<AbstractPhysicalObject.AbstractObjectType, LizardShellHelmetItemProperties> typesProperties = new();

    public LizardShellHelmetItemProperties(LizardShellHelmetAccessoryProperties accessoryProperties) : base(accessoryProperties)
    {

    }

    public static LizardShellHelmetItemProperties GetPropertiesForType(AbstractPhysicalObject.AbstractObjectType type)
    {
        if (typesProperties.TryGetValue(type, out var itemProperties))
            return itemProperties;

        return null;
    }
}
