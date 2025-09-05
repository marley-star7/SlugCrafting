namespace SlugCrafting.Properties;

/// <summary>
/// Interface added to an item's properties to tell it is has accessory information.
/// </summary>
public interface IAccessoryItemProperties
{
    /// <summary>
    /// The Accessory properties relating to this item.
    /// </summary>
    public AccessoryProperties AccessoryProperties { get; }
}
