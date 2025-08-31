namespace SlugCrafting;

public static partial class Content
{
    public static readonly Dictionary<AbstractPhysicalObject.AbstractObjectType, ItemBundleProperties> ItemsBundleProperties = new Dictionary<AbstractPhysicalObject.AbstractObjectType, ItemBundleProperties>();

    public static void RegisterItemBundleProperties(AbstractPhysicalObject.AbstractObjectType type, ItemBundleProperties properties)
    {
        ItemsBundleProperties[type] = properties;
    }

}
