namespace SlugCrafting.Core;

public class BundledItemStick : AbstractPhysicalObject.AbstractObjectStick
{
    public BundledItemStick(AbstractPhysicalObject primaryItem, AbstractPhysicalObject secondaryItem) : base(primaryItem, secondaryItem)
    {
        Plugin.LogDebug($"Bundled Item Sticked!");
    }
}
